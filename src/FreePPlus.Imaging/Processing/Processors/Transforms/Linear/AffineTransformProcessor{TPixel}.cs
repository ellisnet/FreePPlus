// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     Provides the base methods to perform affine transforms on an image.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class AffineTransformProcessor<TPixel> : TransformProcessor<TPixel>, IResamplingTransformImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly Size destinationSize;
    private readonly IResampler resampler;
    private readonly Matrix3x2 transformMatrix;
    private ImageFrame<TPixel> destination;
    private ImageFrame<TPixel> source;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AffineTransformProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="AffineTransformProcessor" /> defining the processor parameters.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public AffineTransformProcessor(Configuration configuration, AffineTransformProcessor definition,
        Image<TPixel> source, Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        destinationSize = definition.DestinationSize;
        transformMatrix = definition.TransformMatrix;
        resampler = definition.Sampler;
    }

    /// <inheritdoc />
    public void ApplyTransform<TResampler>(in TResampler sampler)
        where TResampler : struct, IResampler
    {
        var configuration = Configuration;
        var source = this.source;
        var destination = this.destination;
        var matrix = transformMatrix;

        // Handle transforms that result in output identical to the original.
        if (matrix.Equals(default) || matrix.Equals(Matrix3x2.Identity))
        {
            // The clone will be blank here copy all the pixel data over
            source.GetPixelMemoryGroup().CopyTo(destination.GetPixelMemoryGroup());
            return;
        }

        // Convert from screen to world space.
        Matrix3x2.Invert(matrix, out matrix);

        if (sampler is NearestNeighborResampler)
        {
            var nnOperation = new NNAffineOperation(source, destination, matrix);
            ParallelRowIterator.IterateRows(
                configuration,
                destination.Bounds(),
                in nnOperation);

            return;
        }

        var yRadius = LinearTransformUtilities.GetSamplingRadius(in sampler, source.Height, destination.Height);
        var xRadius = LinearTransformUtilities.GetSamplingRadius(in sampler, source.Width, destination.Width);
        var radialExtents = new Vector2(xRadius, yRadius);
        var yLength = yRadius * 2 + 1;
        var xLength = xRadius * 2 + 1;

        // We use 2D buffers so that we can access the weight spans per row in parallel.
        using var yKernelBuffer = configuration.MemoryAllocator.Allocate2D<float>(yLength, destination.Height);
        using var xKernelBuffer = configuration.MemoryAllocator.Allocate2D<float>(xLength, destination.Height);

        var maxX = source.Width - 1;
        var maxY = source.Height - 1;
        var maxSourceExtents = new Vector4(maxX, maxY, maxX, maxY);

        var operation = new AffineOperation<TResampler>(
            configuration,
            source,
            destination,
            yKernelBuffer,
            xKernelBuffer,
            in sampler,
            matrix,
            radialExtents,
            maxSourceExtents);

        ParallelRowIterator.IterateRows<AffineOperation<TResampler>, Vector4>(
            configuration,
            destination.Bounds(),
            in operation);
    }

    protected override Size GetDestinationSize()
    {
        return destinationSize;
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination)
    {
        this.source = source;
        this.destination = destination;
        resampler.ApplyTransform(this);
    }

    private readonly struct NNAffineOperation : IRowOperation
    {
        private readonly ImageFrame<TPixel> source;
        private readonly ImageFrame<TPixel> destination;
        private readonly Rectangle bounds;
        private readonly Matrix3x2 matrix;
        private readonly int maxX;

        [MethodImpl(InliningOptions.ShortMethod)]
        public NNAffineOperation(
            ImageFrame<TPixel> source,
            ImageFrame<TPixel> destination,
            Matrix3x2 matrix)
        {
            this.source = source;
            this.destination = destination;
            bounds = source.Bounds();
            this.matrix = matrix;
            maxX = destination.Width;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y)
        {
            var destRow = destination.GetPixelRowSpan(y);

            for (var x = 0; x < maxX; x++)
            {
                var point = Vector2.Transform(new Vector2(x, y), matrix);
                var px = (int)MathF.Round(point.X);
                var py = (int)MathF.Round(point.Y);

                if (bounds.Contains(px, py)) destRow[x] = source[px, py];
            }
        }
    }

    private readonly struct AffineOperation<TResampler> : IRowOperation<Vector4>
        where TResampler : struct, IResampler
    {
        private readonly Configuration configuration;
        private readonly ImageFrame<TPixel> source;
        private readonly ImageFrame<TPixel> destination;
        private readonly Buffer2D<float> yKernelBuffer;
        private readonly Buffer2D<float> xKernelBuffer;
        private readonly TResampler sampler;
        private readonly Matrix3x2 matrix;
        private readonly Vector2 radialExtents;
        private readonly Vector4 maxSourceExtents;
        private readonly int maxX;

        [MethodImpl(InliningOptions.ShortMethod)]
        public AffineOperation(
            Configuration configuration,
            ImageFrame<TPixel> source,
            ImageFrame<TPixel> destination,
            Buffer2D<float> yKernelBuffer,
            Buffer2D<float> xKernelBuffer,
            in TResampler sampler,
            Matrix3x2 matrix,
            Vector2 radialExtents,
            Vector4 maxSourceExtents)
        {
            this.configuration = configuration;
            this.source = source;
            this.destination = destination;
            this.yKernelBuffer = yKernelBuffer;
            this.xKernelBuffer = xKernelBuffer;
            this.sampler = sampler;
            this.matrix = matrix;
            this.radialExtents = radialExtents;
            this.maxSourceExtents = maxSourceExtents;
            maxX = destination.Width;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y, Span<Vector4> span)
        {
            var sourceBuffer = source.PixelBuffer;

            PixelOperations<TPixel>.Instance.ToVector4(
                configuration,
                destination.GetPixelRowSpan(y),
                span);

            ref var yKernelSpanRef = ref MemoryMarshal.GetReference(yKernelBuffer.GetRowSpan(y));
            ref var xKernelSpanRef = ref MemoryMarshal.GetReference(xKernelBuffer.GetRowSpan(y));

            for (var x = 0; x < maxX; x++)
            {
                // Use the single precision position to calculate correct bounding pixels
                // otherwise we get rogue pixels outside of the bounds.
                var point = Vector2.Transform(new Vector2(x, y), matrix);
                LinearTransformUtilities.Convolve(
                    in sampler,
                    point,
                    sourceBuffer,
                    span,
                    x,
                    ref yKernelSpanRef,
                    ref xKernelSpanRef,
                    radialExtents,
                    maxSourceExtents);
            }

            PixelOperations<TPixel>.Instance.FromVector4Destructive(
                configuration,
                span,
                destination.GetPixelRowSpan(y));
        }
    }
}