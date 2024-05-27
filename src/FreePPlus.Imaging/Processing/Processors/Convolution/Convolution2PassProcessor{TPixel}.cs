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
///     Defines a processor that uses two one-dimensional matrices to perform two-pass convolution against an image.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class Convolution2PassProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Convolution2PassProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="kernelX">The horizontal gradient operator.</param>
    /// <param name="kernelY">The vertical gradient operator.</param>
    /// <param name="preserveAlpha">Whether the convolution filter is applied to alpha as well as the color channels.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public Convolution2PassProcessor(
        Configuration configuration,
        in DenseMatrix<float> kernelX,
        in DenseMatrix<float> kernelY,
        bool preserveAlpha,
        Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        KernelX = kernelX;
        KernelY = kernelY;
        PreserveAlpha = preserveAlpha;
    }

    /// <summary>
    ///     Gets the horizontal gradient operator.
    /// </summary>
    public DenseMatrix<float> KernelX { get; }

    /// <summary>
    ///     Gets the vertical gradient operator.
    /// </summary>
    public DenseMatrix<float> KernelY { get; }

    /// <summary>
    ///     Gets a value indicating whether the convolution filter is applied to alpha as well as the color channels.
    /// </summary>
    public bool PreserveAlpha { get; }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        using var firstPassPixels = Configuration.MemoryAllocator.Allocate2D<TPixel>(source.Size());

        var interest = Rectangle.Intersect(SourceRectangle, source.Bounds());

        // Horizontal convolution
        var horizontalOperation = new RowOperation(interest, firstPassPixels, source.PixelBuffer, KernelX,
            Configuration, PreserveAlpha);
        ParallelRowIterator.IterateRows<RowOperation, Vector4>(
            Configuration,
            interest,
            in horizontalOperation);

        // Vertical convolution
        var verticalOperation = new RowOperation(interest, source.PixelBuffer, firstPassPixels, KernelY, Configuration,
            PreserveAlpha);
        ParallelRowIterator.IterateRows<RowOperation, Vector4>(
            Configuration,
            interest,
            in verticalOperation);
    }

    /// <summary>
    ///     A <see langword="struct" /> implementing the convolution logic for <see cref="Convolution2PassProcessor{T}" />.
    /// </summary>
    private readonly struct RowOperation : IRowOperation<Vector4>
    {
        private readonly Rectangle bounds;
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

            var maxY = bounds.Bottom - 1;
            var maxX = bounds.Right - 1;

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