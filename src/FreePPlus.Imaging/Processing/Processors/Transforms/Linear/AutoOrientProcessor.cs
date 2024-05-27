// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     Adjusts an image so that its orientation is suitable for viewing. Adjustments are based on EXIF metadata embedded
///     in the image.
/// </summary>
public sealed class AutoOrientProcessor : IImageProcessor
{
    /// <inheritdoc />
    public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration,
        Image<TPixel> source, Rectangle sourceRectangle)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return new AutoOrientProcessor<TPixel>(configuration, source, sourceRectangle);
    }
}