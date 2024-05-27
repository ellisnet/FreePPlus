// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing.Processors.Convolution;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

/// <summary>
///     Defines edge detection processing using the Scharr operator filter.
///     <see href="http://en.wikipedia.org/wiki/Sobel_operator#Alternative_operators" />
/// </summary>
public sealed class ScharrProcessor : EdgeDetectorProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScharrProcessor" /> class.
    /// </summary>
    /// <param name="grayscale">Whether to convert the image to grayscale before performing edge detection.</param>
    public ScharrProcessor(bool grayscale)
        : base(grayscale) { }

    /// <inheritdoc />
    public override IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration,
        Image<TPixel> source, Rectangle sourceRectangle)
    {
        return new EdgeDetector2DProcessor<TPixel>(
            configuration,
            ScharrKernels.ScharrX,
            ScharrKernels.ScharrY,
            Grayscale,
            source,
            sourceRectangle);
    }
}