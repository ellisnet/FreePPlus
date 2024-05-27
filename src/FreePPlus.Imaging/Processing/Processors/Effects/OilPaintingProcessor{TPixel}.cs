// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Effects;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Effects;

/// <summary>
///     Applies oil painting effect processing to the image.
/// </summary>
/// <remarks>
///     Adapted from <see href="https://softwarebydefault.com/2013/06/29/oil-painting-cartoon-filter/" /> by Dewald
///     Esterhuizen.
/// </remarks>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class OilPaintingProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly OilPaintingProcessor definition;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OilPaintingProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="OilPaintingProcessor" /> defining the processor parameters.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public OilPaintingProcessor(Configuration configuration, OilPaintingProcessor definition, Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        this.definition = definition;
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        var brushSize = definition.BrushSize;
        if (brushSize <= 0 || brushSize > source.Height || brushSize > source.Width)
            throw new ArgumentOutOfRangeException(nameof(brushSize));

        using var targetPixels = Configuration.MemoryAllocator.Allocate2D<TPixel>(source.Size());

        source.CopyTo(targetPixels);

        var operation = new RowIntervalOperation(SourceRectangle, targetPixels, source, Configuration, brushSize >> 1,
            definition.Levels);
        ParallelRowIterator.IterateRowIntervals(
            Configuration,
            SourceRectangle,
            in operation);

        Buffer2D<TPixel>.SwapOrCopyContent(source.PixelBuffer, targetPixels);
    }

    /// <summary>
    ///     A <see langword="struct" /> implementing the convolution logic for <see cref="OilPaintingProcessor{T}" />.
    /// </summary>
    private readonly struct RowIntervalOperation : IRowIntervalOperation
    {
        private readonly Rectangle bounds;
        private readonly Buffer2D<TPixel> targetPixels;
        private readonly ImageFrame<TPixel> source;
        private readonly Configuration configuration;
        private readonly int radius;
        private readonly int levels;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowIntervalOperation(
            Rectangle bounds,
            Buffer2D<TPixel> targetPixels,
            ImageFrame<TPixel> source,
            Configuration configuration,
            int radius,
            int levels)
        {
            this.bounds = bounds;
            this.targetPixels = targetPixels;
            this.source = source;
            this.configuration = configuration;
            this.radius = radius;
            this.levels = levels;
        }

        /// <inheritdoc />
        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(in RowInterval rows)
        {
            var maxY = bounds.Bottom - 1;
            var maxX = bounds.Right - 1;

            /* Allocate the two temporary Vector4 buffers, one for the source row and one for the target row.
             * The ParallelHelper.IterateRowsWithTempBuffers overload is not used in this case because
             * the two allocated buffers have a length equal to the width of the source image,
             * and not just equal to the width of the target rectangle to process.
             * Furthermore, there are two buffers being allocated in this case, so using that overload would
             * have still required the explicit allocation of the secondary buffer.
             * Similarly, one temporary float buffer is also allocated from the pool, and that is used
             * to create the target bins for all the color channels being processed.
             * This buffer is only rented once outside of the main processing loop, and its contents
             * are cleared for each loop iteration, to avoid the repeated allocation for each processed pixel. */
            using var sourceRowBuffer = configuration.MemoryAllocator.Allocate<Vector4>(source.Width);
            using var targetRowBuffer = configuration.MemoryAllocator.Allocate<Vector4>(source.Width);
            using var bins = configuration.MemoryAllocator.Allocate<float>(levels * 4);

            var sourceRowVector4Span = sourceRowBuffer.Memory.Span;
            var sourceRowAreaVector4Span = sourceRowVector4Span.Slice(bounds.X, bounds.Width);

            var targetRowVector4Span = targetRowBuffer.Memory.Span;
            var targetRowAreaVector4Span = targetRowVector4Span.Slice(bounds.X, bounds.Width);

            ref var binsRef = ref bins.GetReference();
            ref var intensityBinRef = ref Unsafe.As<float, int>(ref binsRef);
            ref var redBinRef = ref Unsafe.Add(ref binsRef, levels);
            ref var blueBinRef = ref Unsafe.Add(ref redBinRef, levels);
            ref var greenBinRef = ref Unsafe.Add(ref blueBinRef, levels);

            for (var y = rows.Min; y < rows.Max; y++)
            {
                var sourceRowPixelSpan = source.GetPixelRowSpan(y);
                var sourceRowAreaPixelSpan = sourceRowPixelSpan.Slice(bounds.X, bounds.Width);

                PixelOperations<TPixel>.Instance.ToVector4(configuration, sourceRowAreaPixelSpan,
                    sourceRowAreaVector4Span);

                for (var x = bounds.X; x < bounds.Right; x++)
                {
                    var maxIntensity = 0;
                    var maxIndex = 0;

                    // Clear the current shared buffer before processing each target pixel
                    bins.Memory.Span.Clear();

                    for (var fy = 0; fy <= radius; fy++)
                    {
                        var fyr = fy - radius;
                        var offsetY = y + fyr;

                        offsetY = offsetY.Clamp(0, maxY);

                        var sourceOffsetRow = source.GetPixelRowSpan(offsetY);

                        for (var fx = 0; fx <= radius; fx++)
                        {
                            var fxr = fx - radius;
                            var offsetX = x + fxr;
                            offsetX = offsetX.Clamp(0, maxX);

                            var vector = sourceOffsetRow[offsetX].ToVector4();

                            var sourceRed = vector.X;
                            var sourceBlue = vector.Z;
                            var sourceGreen = vector.Y;

                            var currentIntensity =
                                (int)MathF.Round((sourceBlue + sourceGreen + sourceRed) / 3F * (levels - 1));

                            Unsafe.Add(ref intensityBinRef, currentIntensity)++;
                            Unsafe.Add(ref redBinRef, currentIntensity) += sourceRed;
                            Unsafe.Add(ref blueBinRef, currentIntensity) += sourceBlue;
                            Unsafe.Add(ref greenBinRef, currentIntensity) += sourceGreen;

                            if (Unsafe.Add(ref intensityBinRef, currentIntensity) > maxIntensity)
                            {
                                maxIntensity = Unsafe.Add(ref intensityBinRef, currentIntensity);
                                maxIndex = currentIntensity;
                            }
                        }

                        var red = MathF.Abs(Unsafe.Add(ref redBinRef, maxIndex) / maxIntensity);
                        var blue = MathF.Abs(Unsafe.Add(ref blueBinRef, maxIndex) / maxIntensity);
                        var green = MathF.Abs(Unsafe.Add(ref greenBinRef, maxIndex) / maxIntensity);
                        var alpha = sourceRowVector4Span[x].W;

                        targetRowVector4Span[x] = new Vector4(red, green, blue, alpha);
                    }
                }

                var targetRowAreaPixelSpan = targetPixels.GetRowSpan(y).Slice(bounds.X, bounds.Width);

                PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, targetRowAreaVector4Span,
                    targetRowAreaPixelSpan);
            }
        }
    }
}