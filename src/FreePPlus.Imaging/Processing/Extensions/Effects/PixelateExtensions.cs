// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Effects;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines pixelation effect extensions applicable on an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class PixelateExtensions
{
    /// <summary>
    ///     Pixelates an image with the given pixel size.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Pixelate(this IImageProcessingContext source)
    {
        return Pixelate(source, 4);
    }

    /// <summary>
    ///     Pixelates an image with the given pixel size.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="size">The size of the pixels.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Pixelate(this IImageProcessingContext source, int size)
    {
        return source.ApplyProcessor(new PixelateProcessor(size));
    }

    /// <summary>
    ///     Pixelates an image with the given pixel size.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="size">The size of the pixels.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Pixelate(
        this IImageProcessingContext source,
        int size,
        Rectangle rectangle)
    {
        return source.ApplyProcessor(new PixelateProcessor(size), rectangle);
    }
}