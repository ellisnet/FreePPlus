// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Convolution;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

/// <summary>
///     Defines a processor that uses a 2 dimensional matrix to perform convolution against an image.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class ConvolutionProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ConvolutionProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="kernelXY">The 2d gradient operator.</param>
    /// <param name="preserveAlpha">Whether the convolution filter is applied to alpha as well as the color channels.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public ConvolutionProcessor(
        Configuration configuration,
        in DenseMatrix<float> kernelXY,
        bool preserveAlpha,
        Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        KernelXY = kernelXY;
        PreserveAlpha = preserveAlpha;
    }

    /// <summary>
    ///     Gets the 2d gradient operator.
    /// </summary>
    public DenseMatrix<float> KernelXY { get; }

    /// <summary>
    ///     Gets a value indicating whether the convolution filter is applied to alpha as well as the color channels.
    /// </summary>
    public bool PreserveAlpha { get; }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        using var targetPixels = Configuration.MemoryAllocator.Allocate2D<TPixel>(source.Size());

        source.CopyTo(targetPixels);

        var interest = Rectangle.Intersect(SourceRectangle, source.Bounds());
        var operation = new RowOperation(interest, targetPixels, source.PixelBuffer, KernelXY, Configuration,
            PreserveAlpha);
        ParallelRowIterator.IterateRows<RowOperation, Vector4>(
            Configuration,
            interest,
            in operation);

        Buffer2D<TPixel>.SwapOrCopyContent(source.PixelBuffer, targetPixels);
    }

    /// <summary>
    ///     A <see langword="struct" /> implementing the convolution logic for <see cref="ConvolutionProcessor{T}" />.
    /// </summary>
    private readonly struct RowOperation : IRowOperation<Vector4>
    {
        private readonly Rectangle bounds;
        private readonly int maxY;
        private readonly int maxX;
        private readonly Buffer2D<TPixel> targetPixels;
        private readonly Buffer2D<TPixel> sourcePixels;
        private readonly DenseMatrix<float> kernel;
        private readonly Configuration configuration;
        private readonly bool preserveAlpha;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowOperation(
            Rectangle bounds,
            Buffer2D<TPixel> targetPixels,
            Buffer2D<TPixel> sourcePixels,
            DenseMatrix<float> kernel,
            Configuration configuration,
            bool preserveAlpha)
        {
            this.bounds = bounds;
            maxY = this.bounds.Bottom - 1;
            maxX = this.bounds.Right - 1;
            this.targetPixels = targetPixels;
            this.sourcePixels = sourcePixels;
            this.kernel = kernel;
            this.configuration = configuration;
            this.preserveAlpha = preserveAlpha;
        }

        /// <inheritdoc />
        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y, Span<Vector4> span)
        {
            ref var spanRef = ref MemoryMarshal.GetReference(span);

            var targetRowSpan = targetPixels.GetRowSpan(y).Slice(bounds.X);
            PixelOperations<TPixel>.Instance.ToVector4(configuration, targetRowSpan.Slice(0, span.Length), span);

            if (preserveAlpha)
                for (var x = 0; x < bounds.Width; x++)
                    DenseMatrixUtils.Convolve3(
                        in kernel,
                        sourcePixels,
                        ref spanRef,
                        y,
                        x,
                        bounds.Y,
                        maxY,
                        bounds.X,
                        maxX);
            else
                for (var x = 0; x < bounds.Width; x++)
                    DenseMatrixUtils.Convolve4(
                        in kernel,
                        sourcePixels,
                        ref spanRef,
                        y,
                        x,
                        bounds.Y,
                        maxY,
                        bounds.X,
                        maxX);

            PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, span, targetRowSpan);
        }
    }
}