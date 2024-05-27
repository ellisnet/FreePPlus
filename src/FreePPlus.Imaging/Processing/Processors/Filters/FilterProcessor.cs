// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Filters;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Filters;

/// <summary>
///     Defines a free-form color filter by a <see cref="ColorMatrix" />.
/// </summary>
public class FilterProcessor : IImageProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FilterProcessor" /> class.
    /// </summary>
    /// <param name="matrix">The matrix used to apply the image filter</param>
    public FilterProcessor(ColorMatrix matrix)
    {
        Matrix = matrix;
    }

    /// <summary>
    ///     Gets the <see cref="ColorMatrix" /> used to apply the image filter.
    /// </summary>
    public ColorMatrix Matrix { get; }

    /// <inheritdoc />
    public virtual IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration,
        Image<TPixel> source, Rectangle sourceRectangle)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return new FilterProcessor<TPixel>(configuration, this, source, sourceRectangle);
    }
}