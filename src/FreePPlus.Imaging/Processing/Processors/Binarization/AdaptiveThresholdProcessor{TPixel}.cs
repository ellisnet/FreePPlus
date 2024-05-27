// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Binarization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Binarization;

/// <summary>
///     Performs Bradley Adaptive Threshold filter against an image.
/// </summary>
internal class AdaptiveThresholdProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly AdaptiveThresholdProcessor definition;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AdaptiveThresholdProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="AdaptiveThresholdProcessor" /> defining the processor parameters.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public AdaptiveThresholdProcessor(Configuration configuration, AdaptiveThresholdProcessor definition,
        Image<TPixel> source, Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        this.definition = definition;
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        var intersect = Rectangle.Intersect(SourceRectangle, source.Bounds());

        var configuration = Configuration;
        var upper = definition.Upper.ToPixel<TPixel>();
        var lower = definition.Lower.ToPixel<TPixel>();
        var thresholdLimit = definition.ThresholdLimit;

        var startY = intersect.Y;
        var endY = intersect.Bottom;
        var startX = intersect.X;
        var endX = intersect.Right;

        var width = intersect.Width;
        var height = intersect.Height;

        // ClusterSize defines the size of cluster to used to check for average. Tweaked to support up to 4k wide pixels and not more. 4096 / 16 is 256 thus the '-1'
        var clusterSize = (byte)Math.Truncate(width / 16f - 1);

        // Using pooled 2d buffer for integer image table and temp memory to hold Rgb24 converted pixel data.
        using (var intImage = Configuration.MemoryAllocator.Allocate2D<ulong>(width, height))
        {
            Rgba32 rgb = default;
            for (var x = startX; x < endX; x++)
            {
                ulong sum = 0;
                for (var y = startY; y < endY; y++)
                {
                    var row = source.GetPixelRowSpan(y);
                    ref var rowRef = ref MemoryMarshal.GetReference(row);
                    ref var color = ref Unsafe.Add(ref rowRef, x);
                    color.ToRgba32(ref rgb);

                    sum += (ulong)(rgb.R + rgb.G + rgb.G);
                    if (x - startX != 0)
                        intImage[x - startX, y - startY] = intImage[x - startX - 1, y - startY] + sum;
                    else
                        intImage[x - startX, y - startY] = sum;
                }
            }

            var operation = new RowOperation(intersect, source, intImage, upper, lower, thresholdLimit, clusterSize,
                startX, endX, startY);
            ParallelRowIterator.IterateRows(
                configuration,
                intersect,
                in operation);
        }
    }

    private readonly struct RowOperation : IRowOperation
    {
        private readonly Rectangle bounds;
        private readonly ImageFrame<TPixel> source;
        private readonly Buffer2D<ulong> intImage;
        private readonly TPixel upper;
        private readonly TPixel lower;
        private readonly float thresholdLimit;
        private readonly int startX;
        private readonly int endX;
        private readonly int startY;
        private readonly byte clusterSize;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowOperation(
            Rectangle bounds,
            ImageFrame<TPixel> source,
            Buffer2D<ulong> intImage,
            TPixel upper,
            TPixel lower,
            float thresholdLimit,
            byte clusterSize,
            int startX,
            int endX,
            int startY)
        {
            this.bounds = bounds;
            this.source = source;
            this.intImage = intImage;
            this.upper = upper;
            this.lower = lower;
            this.thresholdLimit = thresholdLimit;
            this.startX = startX;
            this.endX = endX;
            this.startY = startY;
            this.clusterSize = clusterSize;
        }

        /// <inheritdoc />
        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y)
        {
            Rgba32 rgb = default;
            var pixelRow = source.GetPixelRowSpan(y);

            for (var x = startX; x < endX; x++)
            {
                var pixel = pixelRow[x];
                pixel.ToRgba32(ref rgb);

                var x1 = Math.Max(x - startX - clusterSize + 1, 0);
                var x2 = Math.Min(x - startX + clusterSize + 1, bounds.Width - 1);
                var y1 = Math.Max(y - startY - clusterSize + 1, 0);
                var y2 = Math.Min(y - startY + clusterSize + 1, bounds.Height - 1);

                var count = (uint)((x2 - x1) * (y2 - y1));
                var sum = (long)Math.Min(intImage[x2, y2] - intImage[x1, y2] - intImage[x2, y1] + intImage[x1, y1],
                    long.MaxValue);

                if ((rgb.R + rgb.G + rgb.B) * count <= sum * thresholdLimit)
                    source[x, y] = lower;
                else
                    source[x, y] = upper;
            }
        }
    }
}