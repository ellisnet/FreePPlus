// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Effects;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Effects;

/// <summary>
///     Applies a pixelation effect processing to the image.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class PixelateProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly PixelateProcessor definition;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PixelateProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="PixelateProcessor" />.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public PixelateProcessor(Configuration configuration, PixelateProcessor definition, Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        this.definition = definition;
    }

    private int Size => definition.Size;

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        var interest = Rectangle.Intersect(SourceRectangle, source.Bounds());
        var size = Size;

        Guard.MustBeBetweenOrEqualTo(size, 0, interest.Width, nameof(size));
        Guard.MustBeBetweenOrEqualTo(size, 0, interest.Height, nameof(size));

        // Get the range on the y-plane to choose from.
        // TODO: It would be nice to be able to pool this somehow but neither Memory<T> nor Span<T>
        // implement IEnumerable<T>.
        var range = EnumerableExtensions.SteppedRange(interest.Y, i => i < interest.Bottom, size);
        Parallel.ForEach(
            range,
            Configuration.GetParallelOptions(),
            new RowOperation(interest, size, source).Invoke);
    }

    private readonly struct RowOperation
    {
        private readonly int minX;
        private readonly int maxX;
        private readonly int maxXIndex;
        private readonly int maxY;
        private readonly int maxYIndex;
        private readonly int size;
        private readonly int radius;
        private readonly ImageFrame<TPixel> source;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowOperation(
            Rectangle bounds,
            int size,
            ImageFrame<TPixel> source)
        {
            minX = bounds.X;
            maxX = bounds.Right;
            maxXIndex = bounds.Right - 1;
            maxY = bounds.Bottom;
            maxYIndex = bounds.Bottom - 1;
            this.size = size;
            radius = size >> 1;
            this.source = source;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y)
        {
            var rowSpan = source.GetPixelRowSpan(Math.Min(y + radius, maxYIndex));

            for (var x = minX; x < maxX; x += size)
            {
                // Get the pixel color in the centre of the soon to be pixelated area.
                var pixel = rowSpan[Math.Min(x + radius, maxXIndex)];

                // For each pixel in the pixelate size, set it to the centre color.
                for (var oY = y; oY < y + size && oY < maxY; oY++)
                for (var oX = x; oX < x + size && oX < maxX; oX++)
                    source[oX, oY] = pixel;
            }
        }
    }
}