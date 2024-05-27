// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     Provides methods that allow the flipping of an image around its center point.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class FlipProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly FlipProcessor definition;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FlipProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="FlipProcessor" />.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public FlipProcessor(Configuration configuration, FlipProcessor definition, Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        this.definition = definition;
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        switch (definition.FlipMode)
        {
            // No default needed as we have already set the pixels.
            case FlipMode.Vertical:
                FlipX(source, Configuration);
                break;
            case FlipMode.Horizontal:
                FlipY(source, Configuration);
                break;
        }
    }

    /// <summary>
    ///     Swaps the image at the X-axis, which goes horizontally through the middle at half the height of the image.
    /// </summary>
    /// <param name="source">The source image to apply the process to.</param>
    /// <param name="configuration">The configuration.</param>
    private void FlipX(ImageFrame<TPixel> source, Configuration configuration)
    {
        var height = source.Height;
        using var tempBuffer = configuration.MemoryAllocator.Allocate<TPixel>(source.Width);
        var temp = tempBuffer.Memory.Span;

        for (var yTop = 0; yTop < height / 2; yTop++)
        {
            var yBottom = height - yTop - 1;
            var topRow = source.GetPixelRowSpan(yBottom);
            var bottomRow = source.GetPixelRowSpan(yTop);
            topRow.CopyTo(temp);
            bottomRow.CopyTo(topRow);
            temp.CopyTo(bottomRow);
        }
    }

    /// <summary>
    ///     Swaps the image at the Y-axis, which goes vertically through the middle at half of the width of the image.
    /// </summary>
    /// <param name="source">The source image to apply the process to.</param>
    /// <param name="configuration">The configuration.</param>
    private void FlipY(ImageFrame<TPixel> source, Configuration configuration)
    {
        var operation = new RowOperation(source);
        ParallelRowIterator.IterateRows(
            configuration,
            source.Bounds(),
            in operation);
    }

    private readonly struct RowOperation : IRowOperation
    {
        private readonly ImageFrame<TPixel> source;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowOperation(ImageFrame<TPixel> source)
        {
            this.source = source;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y)
        {
            source.GetPixelRowSpan(y).Reverse();
        }
    }
}