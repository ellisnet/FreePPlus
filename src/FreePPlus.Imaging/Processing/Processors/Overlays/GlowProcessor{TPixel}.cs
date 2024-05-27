// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Overlays;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Overlays;

/// <summary>
///     An <see cref="IImageProcessor{TPixel}" /> that applies a radial glow effect an <see cref="Image{TPixel}" />.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class GlowProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly PixelBlender<TPixel> blender;
    private readonly GlowProcessor definition;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GlowProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="GlowProcessor" /> defining the processor parameters.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public GlowProcessor(Configuration configuration, GlowProcessor definition, Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        this.definition = definition;
        blender = PixelOperations<TPixel>.Instance.GetPixelBlender(definition.GraphicsOptions);
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        var glowColor = definition.GlowColor.ToPixel<TPixel>();
        var blendPercent = definition.GraphicsOptions.BlendPercentage;

        var interest = Rectangle.Intersect(SourceRectangle, source.Bounds());

        Vector2 center = Rectangle.Center(interest);
        var finalRadius = definition.Radius.Calculate(interest.Size);
        var maxDistance = finalRadius > 0
            ? MathF.Min(finalRadius, interest.Width * .5F)
            : interest.Width * .5F;

        var configuration = Configuration;
        var allocator = configuration.MemoryAllocator;

        using var rowColors = allocator.Allocate<TPixel>(interest.Width);
        rowColors.GetSpan().Fill(glowColor);

        var operation = new RowOperation(configuration, interest, rowColors, blender, center, maxDistance, blendPercent,
            source);
        ParallelRowIterator.IterateRows<RowOperation, float>(
            configuration,
            interest,
            in operation);
    }

    private readonly struct RowOperation : IRowOperation<float>
    {
        private readonly Configuration configuration;
        private readonly Rectangle bounds;
        private readonly PixelBlender<TPixel> blender;
        private readonly Vector2 center;
        private readonly float maxDistance;
        private readonly float blendPercent;
        private readonly IMemoryOwner<TPixel> colors;
        private readonly ImageFrame<TPixel> source;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowOperation(
            Configuration configuration,
            Rectangle bounds,
            IMemoryOwner<TPixel> colors,
            PixelBlender<TPixel> blender,
            Vector2 center,
            float maxDistance,
            float blendPercent,
            ImageFrame<TPixel> source)
        {
            this.configuration = configuration;
            this.bounds = bounds;
            this.colors = colors;
            this.blender = blender;
            this.center = center;
            this.maxDistance = maxDistance;
            this.blendPercent = blendPercent;
            this.source = source;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y, Span<float> span)
        {
            var colorSpan = colors.GetSpan();

            for (var i = 0; i < bounds.Width; i++)
            {
                var distance = Vector2.Distance(center, new Vector2(i + bounds.X, y));
                span[i] = (blendPercent * (1 - .95F * (distance / maxDistance))).Clamp(0, 1);
            }

            var destination = source.GetPixelRowSpan(y).Slice(bounds.X, bounds.Width);

            blender.Blend(
                configuration,
                destination,
                destination,
                colorSpan,
                span);
        }
    }
}