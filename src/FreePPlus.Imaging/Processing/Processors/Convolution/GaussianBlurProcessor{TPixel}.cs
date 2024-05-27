// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Convolution;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

/// <summary>
///     Applies Gaussian blur processing to an image.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class GaussianBlurProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GaussianBlurProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="GaussianBlurProcessor" /> defining the processor parameters.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public GaussianBlurProcessor(
        Configuration configuration,
        GaussianBlurProcessor definition,
        Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        var kernelSize = definition.Radius * 2 + 1;
        KernelX = ConvolutionProcessorHelpers.CreateGaussianBlurKernel(kernelSize, definition.Sigma);
        KernelY = KernelX.Transpose();
    }

    /// <summary>
    ///     Gets the horizontal gradient operator.
    /// </summary>
    public DenseMatrix<float> KernelX { get; }

    /// <summary>
    ///     Gets the vertical gradient operator.
    /// </summary>
    public DenseMatrix<float> KernelY { get; }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        using var processor =
            new Convolution2PassProcessor<TPixel>(Configuration, KernelX, KernelY, false, Source, SourceRectangle);

        processor.Apply(source);
    }
}