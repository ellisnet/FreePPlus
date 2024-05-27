// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing.Processors.Filters;

namespace FreePPlus.Imaging.Processing.Processors.Convolution;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

/// <summary>
///     Defines a processor that detects edges within an image using a single two dimensional matrix.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class EdgeDetectorProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EdgeDetectorProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="kernelXY">The 2d gradient operator.</param>
    /// <param name="grayscale">Whether to convert the image to grayscale before performing edge detection.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The target area to process for the current processor instance.</param>
    public EdgeDetectorProcessor(
        Configuration configuration,
        in DenseMatrix<float> kernelXY,
        bool grayscale,
        Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        KernelXY = kernelXY;
        Grayscale = grayscale;
    }

    public bool Grayscale { get; }

    /// <summary>
    ///     Gets the 2d gradient operator.
    /// </summary>
    public DenseMatrix<float> KernelXY { get; }

    /// <inheritdoc />
    protected override void BeforeImageApply()
    {
        using (IImageProcessor<TPixel> opaque = new OpaqueProcessor<TPixel>(Configuration, Source, SourceRectangle))
        {
            opaque.Execute();
        }

        if (Grayscale) new GrayscaleBt709Processor(1F).Execute(Configuration, Source, SourceRectangle);

        base.BeforeImageApply();
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        using (var processor = new ConvolutionProcessor<TPixel>(Configuration, KernelXY, true, Source, SourceRectangle))
        {
            processor.Apply(source);
        }
    }
}