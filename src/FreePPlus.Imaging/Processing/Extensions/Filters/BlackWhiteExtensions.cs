﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Filters;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines extension methods that allow the application of black and white toning to an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class BlackWhiteExtensions
{
    /// <summary>
    ///     Applies black and white toning to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext BlackWhite(this IImageProcessingContext source)
    {
        return source.ApplyProcessor(new BlackWhiteProcessor());
    }

    /// <summary>
    ///     Applies black and white toning to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext BlackWhite(this IImageProcessingContext source, Rectangle rectangle)
    {
        return source.ApplyProcessor(new BlackWhiteProcessor(), rectangle);
    }
}