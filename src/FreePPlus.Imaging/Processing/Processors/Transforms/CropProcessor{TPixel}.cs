// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     Provides methods to allow the cropping of an image.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class CropProcessor<TPixel> : TransformProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly Rectangle cropRectangle;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CropProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="CropProcessor" />.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public CropProcessor(Configuration configuration, CropProcessor definition, Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        cropRectangle = definition.CropRectangle;
    }

    /// <inheritdoc />
    protected override Size GetDestinationSize()
    {
        return new Size(cropRectangle.Width, cropRectangle.Height);
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination)
    {
        // Handle crop dimensions identical to the original
        if (source.Width == destination.Width
            && source.Height == destination.Height
            && SourceRectangle == cropRectangle)
        {
            // the cloned will be blank here copy all the pixel data over
            source.GetPixelMemoryGroup().CopyTo(destination.GetPixelMemoryGroup());
            return;
        }

        var bounds = cropRectangle;

        // Copying is cheap, we should process more pixels per task:
        var parallelSettings =
            ParallelExecutionSettings.FromConfiguration(Configuration).MultiplyMinimumPixelsPerTask(4);

        var operation = new RowOperation(bounds, source, destination);

        ParallelRowIterator.IterateRows(
            bounds,
            in parallelSettings,
            in operation);
    }

    /// <summary>
    ///     A <see langword="struct" /> implementing the processor logic for <see cref="CropProcessor{T}" />.
    /// </summary>
    private readonly struct RowOperation : IRowOperation
    {
        private readonly Rectangle bounds;
        private readonly ImageFrame<TPixel> source;
        private readonly ImageFrame<TPixel> destination;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RowOperation" /> struct.
        /// </summary>
        /// <param name="bounds">The target processing bounds for the current instance.</param>
        /// <param name="source">The source <see cref="Image{TPixel}" /> for the current instance.</param>
        /// <param name="destination">The destination <see cref="Image{TPixel}" /> for the current instance.</param>
        [MethodImpl(InliningOptions.ShortMethod)]
        public RowOperation(Rectangle bounds, ImageFrame<TPixel> source, ImageFrame<TPixel> destination)
        {
            this.bounds = bounds;
            this.source = source;
            this.destination = destination;
        }

        /// <inheritdoc />
        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y)
        {
            var sourceRow = source.GetPixelRowSpan(y).Slice(bounds.Left);
            var targetRow = destination.GetPixelRowSpan(y - bounds.Top);
            sourceRow.Slice(0, bounds.Width).CopyTo(targetRow);
        }
    }
}