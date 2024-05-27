﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Filters;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines extensions that allow the application of composable filters to an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class FilterExtensions
{
    /// <summary>
    ///     Filters an image by the given color matrix
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="matrix">The filter color matrix</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Filter(this IImageProcessingContext source, ColorMatrix matrix)
    {
        return source.ApplyProcessor(new FilterProcessor(matrix));
    }

    /// <summary>
    ///     Filters an image by the given color matrix
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="matrix">The filter color matrix</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Filter(this IImageProcessingContext source, ColorMatrix matrix,
        Rectangle rectangle)
    {
        return source.ApplyProcessor(new FilterProcessor(matrix), rectangle);
    }
}