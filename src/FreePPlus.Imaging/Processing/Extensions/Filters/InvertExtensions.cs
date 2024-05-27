// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Filters;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines extensions that allow the inversion of colors of an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class InvertExtensions
{
    /// <summary>
    ///     Inverts the colors of the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Invert(this IImageProcessingContext source)
    {
        return source.ApplyProcessor(new InvertProcessor(1F));
    }

    /// <summary>
    ///     Inverts the colors of the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Invert(this IImageProcessingContext source, Rectangle rectangle)
    {
        return source.ApplyProcessor(new InvertProcessor(1F), rectangle);
    }
}