// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Binarization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Binarization;

/// <summary>
///     Performs simple binary threshold filtering against an image.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class BinaryThresholdProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly BinaryThresholdProcessor definition;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BinaryThresholdProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="BinaryThresholdProcessor" /> defining the processor parameters.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public BinaryThresholdProcessor(Configuration configuration, BinaryThresholdProcessor definition,
        Image<TPixel> source, Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        this.definition = definition;
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        var threshold = (byte)MathF.Round(definition.Threshold * 255F);
        var upper = definition.UpperColor.ToPixel<TPixel>();
        var lower = definition.LowerColor.ToPixel<TPixel>();

        var sourceRectangle = SourceRectangle;
        var configuration = Configuration;

        var interest = Rectangle.Intersect(sourceRectangle, source.Bounds());
        var isAlphaOnly = typeof(TPixel) == typeof(A8);

        var operation = new RowOperation(interest, source, upper, lower, threshold, isAlphaOnly);
        ParallelRowIterator.IterateRows(
            configuration,
            interest,
            in operation);
    }

    /// <summary>
    ///     A <see langword="struct" /> implementing the clone logic for <see cref="BinaryThresholdProcessor{TPixel}" />.
    /// </summary>
    private readonly struct RowOperation : IRowOperation
    {
        private readonly ImageFrame<TPixel> source;
        private readonly TPixel upper;
        private readonly TPixel lower;
        private readonly byte threshold;
        private readonly int minX;
        private readonly int maxX;
        private readonly bool isAlphaOnly;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowOperation(
            Rectangle bounds,
            ImageFrame<TPixel> source,
            TPixel upper,
            TPixel lower,
            byte threshold,
            bool isAlphaOnly)
        {
            this.source = source;
            this.upper = upper;
            this.lower = lower;
            this.threshold = threshold;
            minX = bounds.X;
            maxX = bounds.Right;
            this.isAlphaOnly = isAlphaOnly;
        }

        /// <inheritdoc />
        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y)
        {
            Rgba32 rgba = default;
            var row = source.GetPixelRowSpan(y);
            ref var rowRef = ref MemoryMarshal.GetReference(row);

            for (var x = minX; x < maxX; x++)
            {
                ref var color = ref Unsafe.Add(ref rowRef, x);
                color.ToRgba32(ref rgba);

                // Convert to grayscale using ITU-R Recommendation BT.709 if required
                var luminance = isAlphaOnly ? rgba.A : ImageMaths.Get8BitBT709Luminance(rgba.R, rgba.G, rgba.B);
                color = luminance >= threshold ? upper : lower;
            }
        }
    }
}