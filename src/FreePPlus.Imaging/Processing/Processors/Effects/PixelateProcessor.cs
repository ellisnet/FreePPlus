// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Effects;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Effects;

/// <summary>
///     Defines a pixelation effect of a given size.
/// </summary>
public sealed class PixelateProcessor : IImageProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PixelateProcessor" /> class.
    /// </summary>
    /// <param name="size">The size of the pixels. Must be greater than 0.</param>
    /// <exception cref="System.ArgumentException">
    ///     <paramref name="size" /> is less than 0 or equal to 0.
    /// </exception>
    public PixelateProcessor(int size)
    {
        Guard.MustBeGreaterThan(size, 0, nameof(size));
        Size = size;
    }

    /// <summary>
    ///     Gets or the pixel size.
    /// </summary>
    public int Size { get; }

    /// <inheritdoc />
    public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration,
        Image<TPixel> source, Rectangle sourceRectangle)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return new PixelateProcessor<TPixel>(configuration, this, source, sourceRectangle);
    }
}