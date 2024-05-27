// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing.Processors.Convolution;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

/// <summary>
///     Defines edge detection using the Robinson operator filter.
///     See <see href="http://www.tutorialspoint.com/dip/Robinson_Compass_Mask.htm" />.
/// </summary>
public sealed class RobinsonProcessor : EdgeDetectorProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RobinsonProcessor" /> class.
    /// </summary>
    /// <param name="grayscale">Whether to convert the image to grayscale before performing edge detection.</param>
    public RobinsonProcessor(bool grayscale)
        : base(grayscale) { }

    /// <inheritdoc />
    public override IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration,
        Image<TPixel> source, Rectangle sourceRectangle)
    {
        return new EdgeDetectorCompassProcessor<TPixel>(configuration, new RobinsonKernels(), Grayscale, source,
            sourceRectangle);
    }
}