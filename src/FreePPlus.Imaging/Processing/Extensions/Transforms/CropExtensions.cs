﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Transforms;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines extensions that allow the application of cropping operations on an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class CropExtensions
{
    /// <summary>
    ///     Crops an image to the given width and height.
    /// </summary>
    /// <param name="source">The image to resize.</param>
    /// <param name="width">The target image width.</param>
    /// <param name="height">The target image height.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Crop(this IImageProcessingContext source, int width, int height)
    {
        return Crop(source, new Rectangle(0, 0, width, height));
    }

    /// <summary>
    ///     Crops an image to the given rectangle.
    /// </summary>
    /// <param name="source">The image to crop.</param>
    /// <param name="cropRectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to retain.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Crop(this IImageProcessingContext source, Rectangle cropRectangle)
    {
        return source.ApplyProcessor(new CropProcessor(cropRectangle, source.GetCurrentSize()));
    }
}