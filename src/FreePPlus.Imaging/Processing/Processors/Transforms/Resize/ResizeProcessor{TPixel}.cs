// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     Implements resizing of images using various resamplers.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class ResizeProcessor<TPixel> : TransformProcessor<TPixel>, IResamplingTransformImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly bool compand;
    private readonly int destinationHeight;
    private readonly Rectangle destinationRectangle;
    private readonly int destinationWidth;
    private readonly IResampler resampler;
    private Image<TPixel> destination;

    public ResizeProcessor(Configuration configuration, ResizeProcessor definition, Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        destinationWidth = definition.DestinationWidth;
        destinationHeight = definition.DestinationHeight;
        destinationRectangle = definition.DestinationRectangle;
        resampler = definition.Sampler;
        compand = definition.Compand;
    }

    public void ApplyTransform<TResampler>(in TResampler sampler)
        where TResampler : struct, IResampler
    {
        var configuration = Configuration;
        var source = Source;
        var destination = this.destination;
        var sourceRectangle = SourceRectangle;
        var destinationRectangle = this.destinationRectangle;
        var compand = this.compand;

        // Handle resize dimensions identical to the original
        if (source.Width == destination.Width
            && source.Height == destination.Height
            && sourceRectangle == destinationRectangle)
        {
            for (var i = 0; i < source.Frames.Count; i++)
            {
                var sourceFrame = source.Frames[i];
                var destinationFrame = destination.Frames[i];

                // The cloned will be blank here copy all the pixel data over
                sourceFrame.GetPixelMemoryGroup().CopyTo(destinationFrame.GetPixelMemoryGroup());
            }

            return;
        }

        var interest = Rectangle.Intersect(destinationRectangle, destination.Bounds());

        if (sampler is NearestNeighborResampler)
        {
            for (var i = 0; i < source.Frames.Count; i++)
            {
                var sourceFrame = source.Frames[i];
                var destinationFrame = destination.Frames[i];

                ApplyNNResizeFrameTransform(
                    configuration,
                    sourceFrame,
                    destinationFrame,
                    sourceRectangle,
                    destinationRectangle,
                    interest);
            }

            return;
        }

        // Since all image frame dimensions have to be the same we can calculate
        // the kernel maps and reuse for all frames.
        var allocator = configuration.MemoryAllocator;
        using var horizontalKernelMap = ResizeKernelMap.Calculate(
            in sampler,
            destinationRectangle.Width,
            sourceRectangle.Width,
            allocator);

        using var verticalKernelMap = ResizeKernelMap.Calculate(
            in sampler,
            destinationRectangle.Height,
            sourceRectangle.Height,
            allocator);

        for (var i = 0; i < source.Frames.Count; i++)
        {
            var sourceFrame = source.Frames[i];
            var destinationFrame = destination.Frames[i];

            ApplyResizeFrameTransform(
                configuration,
                sourceFrame,
                destinationFrame,
                horizontalKernelMap,
                verticalKernelMap,
                sourceRectangle,
                destinationRectangle,
                interest,
                compand);
        }
    }

    /// <inheritdoc />
    protected override Size GetDestinationSize()
    {
        return new Size(destinationWidth, destinationHeight);
    }

    /// <inheritdoc />
    protected override void BeforeImageApply(Image<TPixel> destination)
    {
        this.destination = destination;
        resampler.ApplyTransform(this);

        base.BeforeImageApply(destination);
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination)
    {
        // Everything happens in BeforeImageApply.
    }

    private static void ApplyNNResizeFrameTransform(
        Configuration configuration,
        ImageFrame<TPixel> source,
        ImageFrame<TPixel> destination,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle,
        Rectangle interest)
    {
        // Scaling factors
        var widthFactor = sourceRectangle.Width / (float)destinationRectangle.Width;
        var heightFactor = sourceRectangle.Height / (float)destinationRectangle.Height;

        var operation = new NNRowOperation(
            sourceRectangle,
            destinationRectangle,
            widthFactor,
            heightFactor,
            source,
            destination);

        ParallelRowIterator.IterateRows(
            configuration,
            interest,
            in operation);
    }

    private static void ApplyResizeFrameTransform(
        Configuration configuration,
        ImageFrame<TPixel> source,
        ImageFrame<TPixel> destination,
        ResizeKernelMap horizontalKernelMap,
        ResizeKernelMap verticalKernelMap,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle,
        Rectangle interest,
        bool compand)
    {
        var conversionModifiers =
            PixelConversionModifiers.Premultiply.ApplyCompanding(compand);

        var sourceRegion = source.PixelBuffer.GetRegion(sourceRectangle);

        // To reintroduce parallel processing, we would launch multiple workers
        // for different row intervals of the image.
        using (var worker = new ResizeWorker<TPixel>(
                   configuration,
                   sourceRegion,
                   conversionModifiers,
                   horizontalKernelMap,
                   verticalKernelMap,
                   destination.Width,
                   interest,
                   destinationRectangle.Location))
        {
            worker.Initialize();

            var workingInterval = new RowInterval(interest.Top, interest.Bottom);
            worker.FillDestinationPixels(workingInterval, destination.PixelBuffer);
        }
    }

    private readonly struct NNRowOperation : IRowOperation
    {
        private readonly Rectangle sourceBounds;
        private readonly Rectangle destinationBounds;
        private readonly float widthFactor;
        private readonly float heightFactor;
        private readonly ImageFrame<TPixel> source;
        private readonly ImageFrame<TPixel> destination;

        [MethodImpl(InliningOptions.ShortMethod)]
        public NNRowOperation(
            Rectangle sourceBounds,
            Rectangle destinationBounds,
            float widthFactor,
            float heightFactor,
            ImageFrame<TPixel> source,
            ImageFrame<TPixel> destination)
        {
            this.sourceBounds = sourceBounds;
            this.destinationBounds = destinationBounds;
            this.widthFactor = widthFactor;
            this.heightFactor = heightFactor;
            this.source = source;
            this.destination = destination;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y)
        {
            var sourceX = sourceBounds.X;
            var sourceY = sourceBounds.Y;
            var destX = destinationBounds.X;
            var destY = destinationBounds.Y;
            var destLeft = destinationBounds.Left;
            var destRight = destinationBounds.Right;

            // Y coordinates of source points
            var sourceRow = source.GetPixelRowSpan((int)((y - destY) * heightFactor + sourceY));
            var targetRow = destination.GetPixelRowSpan(y);

            for (var x = destLeft; x < destRight; x++)
                // X coordinates of source points
                targetRow[x] = sourceRow[(int)((x - destX) * widthFactor + sourceX)];
        }
    }
}