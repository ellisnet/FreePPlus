// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing.Processors.Filters;

namespace FreePPlus.Imaging.Processing.Processors.Convolution;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

/// <summary>
///     Defines a processor that detects edges within an image using a eight two dimensional matrices.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class EdgeDetectorCompassProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EdgeDetectorCompassProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="kernels">Gets the kernels to use.</param>
    /// <param name="grayscale">Whether to convert the image to grayscale before performing edge detection.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    internal EdgeDetectorCompassProcessor(Configuration configuration, CompassKernels kernels, bool grayscale,
        Image<TPixel> source, Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        Grayscale = grayscale;
        Kernels = kernels;
    }

    private CompassKernels Kernels { get; }

    private bool Grayscale { get; }

    /// <inheritdoc />
    protected override void BeforeImageApply()
    {
        using (IImageProcessor<TPixel> opaque = new OpaqueProcessor<TPixel>(Configuration, Source, SourceRectangle))
        {
            opaque.Execute();
        }

        if (Grayscale) new GrayscaleBt709Processor(1F).Execute(Configuration, Source, SourceRectangle);

        base.BeforeImageApply();
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        var kernels = Kernels.Flatten();

        var interest = Rectangle.Intersect(SourceRectangle, source.Bounds());

        // We need a clean copy for each pass to start from
        using var cleanCopy = source.Clone();

        using (var processor = new ConvolutionProcessor<TPixel>(Configuration, kernels[0], true, Source, interest))
        {
            processor.Apply(source);
        }

        if (kernels.Length == 1) return;

        // Additional runs
        for (var i = 1; i < kernels.Length; i++)
        {
            using var pass = cleanCopy.Clone();

            using (var processor = new ConvolutionProcessor<TPixel>(Configuration, kernels[i], true, Source, interest))
            {
                processor.Apply(pass);
            }

            var operation = new RowOperation(source.PixelBuffer, pass.PixelBuffer, interest);
            ParallelRowIterator.IterateRows(
                Configuration,
                interest,
                in operation);
        }
    }

    /// <summary>
    ///     A <see langword="struct" /> implementing the convolution logic for <see cref="EdgeDetectorCompassProcessor{T}" />.
    /// </summary>
    private readonly struct RowOperation : IRowOperation
    {
        private readonly Buffer2D<TPixel> targetPixels;
        private readonly Buffer2D<TPixel> passPixels;
        private readonly int minX;
        private readonly int maxX;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowOperation(
            Buffer2D<TPixel> targetPixels,
            Buffer2D<TPixel> passPixels,
            Rectangle bounds)
        {
            this.targetPixels = targetPixels;
            this.passPixels = passPixels;
            minX = bounds.X;
            maxX = bounds.Right;
        }

        /// <inheritdoc />
        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y)
        {
            ref var passPixelsBase = ref MemoryMarshal.GetReference(passPixels.GetRowSpan(y));
            ref var targetPixelsBase = ref MemoryMarshal.GetReference(targetPixels.GetRowSpan(y));

            for (var x = minX; x < maxX; x++)
            {
                // Grab the max components of the two pixels
                ref var currentPassPixel = ref Unsafe.Add(ref passPixelsBase, x);
                ref var currentTargetPixel = ref Unsafe.Add(ref targetPixelsBase, x);

                var pixelValue = Vector4.Max(currentPassPixel.ToVector4(), currentTargetPixel.ToVector4());

                currentTargetPixel.FromVector4(pixelValue);
            }
        }
    }
}