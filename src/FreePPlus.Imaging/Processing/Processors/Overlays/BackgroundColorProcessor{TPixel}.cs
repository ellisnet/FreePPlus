// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Buffers;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Overlays;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Overlays;

/// <summary>
///     Sets the background color of the image.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class BackgroundColorProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly BackgroundColorProcessor definition;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BackgroundColorProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="BackgroundColorProcessor" /> defining the processor parameters.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public BackgroundColorProcessor(Configuration configuration, BackgroundColorProcessor definition,
        Image<TPixel> source, Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        this.definition = definition;
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        var color = definition.Color.ToPixel<TPixel>();
        var graphicsOptions = definition.GraphicsOptions;

        var interest = Rectangle.Intersect(SourceRectangle, source.Bounds());

        var configuration = Configuration;
        var memoryAllocator = configuration.MemoryAllocator;

        using var colors = memoryAllocator.Allocate<TPixel>(interest.Width);
        using var amount = memoryAllocator.Allocate<float>(interest.Width);

        colors.GetSpan().Fill(color);
        amount.GetSpan().Fill(graphicsOptions.BlendPercentage);

        var blender = PixelOperations<TPixel>.Instance.GetPixelBlender(graphicsOptions);

        var operation = new RowOperation(configuration, interest, blender, amount, colors, source);
        ParallelRowIterator.IterateRows(
            configuration,
            interest,
            in operation);
    }

    private readonly struct RowOperation : IRowOperation
    {
        private readonly Configuration configuration;
        private readonly Rectangle bounds;
        private readonly PixelBlender<TPixel> blender;
        private readonly IMemoryOwner<float> amount;
        private readonly IMemoryOwner<TPixel> colors;
        private readonly ImageFrame<TPixel> source;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowOperation(
            Configuration configuration,
            Rectangle bounds,
            PixelBlender<TPixel> blender,
            IMemoryOwner<float> amount,
            IMemoryOwner<TPixel> colors,
            ImageFrame<TPixel> source)
        {
            this.configuration = configuration;
            this.bounds = bounds;
            this.blender = blender;
            this.amount = amount;
            this.colors = colors;
            this.source = source;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y)
        {
            var destination =
                source.GetPixelRowSpan(y)
                    .Slice(bounds.X, bounds.Width);

            // Switch color & destination in the 2nd and 3rd places because we are
            // applying the target color under the current one.
            blender.Blend(
                configuration,
                destination,
                colors.GetSpan(),
                destination,
                amount.GetSpan());
        }
    }
}