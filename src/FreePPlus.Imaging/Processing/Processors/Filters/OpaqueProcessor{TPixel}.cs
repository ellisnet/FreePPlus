// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Filters;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Filters;

internal sealed class OpaqueProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    public OpaqueProcessor(
        Configuration configuration,
        Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle) { }

    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        var interest = Rectangle.Intersect(SourceRectangle, source.Bounds());

        var operation = new OpaqueRowOperation(Configuration, source, interest);
        ParallelRowIterator.IterateRows<OpaqueRowOperation, Vector4>(Configuration, interest, in operation);
    }

    private readonly struct OpaqueRowOperation : IRowOperation<Vector4>
    {
        private readonly Configuration configuration;
        private readonly ImageFrame<TPixel> target;
        private readonly Rectangle bounds;

        [MethodImpl(InliningOptions.ShortMethod)]
        public OpaqueRowOperation(
            Configuration configuration,
            ImageFrame<TPixel> target,
            Rectangle bounds)
        {
            this.configuration = configuration;
            this.target = target;
            this.bounds = bounds;
        }

        /// <inheritdoc />
        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y, Span<Vector4> span)
        {
            var targetRowSpan = target.GetPixelRowSpan(y).Slice(bounds.X);
            PixelOperations<TPixel>.Instance.ToVector4(configuration, targetRowSpan.Slice(0, span.Length), span,
                PixelConversionModifiers.Scale);
            ref var baseRef = ref MemoryMarshal.GetReference(span);

            for (var x = 0; x < bounds.Width; x++)
            {
                ref var v = ref Unsafe.Add(ref baseRef, x);
                v.W = 1F;
            }

            PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span, targetRowSpan,
                PixelConversionModifiers.Scale);
        }
    }
}