﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Transforms;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines extensions that allow the application of auto-orientation operations to an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class AutoOrientExtensions
{
    /// <summary>
    ///     Adjusts an image so that its orientation is suitable for viewing. Adjustments are based on EXIF metadata embedded
    ///     in the image.
    /// </summary>
    /// <param name="source">The image to auto rotate.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext AutoOrient(this IImageProcessingContext source)
    {
        return source.ApplyProcessor(new AutoOrientProcessor());
    }
}