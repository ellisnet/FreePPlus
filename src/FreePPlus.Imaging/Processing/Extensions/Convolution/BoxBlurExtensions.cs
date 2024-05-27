// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Convolution;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines extensions methods to apply box blurring to an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class BoxBlurExtensions
{
    /// <summary>
    ///     Applies a box blur to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext BoxBlur(this IImageProcessingContext source)
    {
        return source.ApplyProcessor(new BoxBlurProcessor());
    }

    /// <summary>
    ///     Applies a box blur to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="radius">The 'radius' value representing the size of the area to sample.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext BoxBlur(this IImageProcessingContext source, int radius)
    {
        return source.ApplyProcessor(new BoxBlurProcessor(radius));
    }

    /// <summary>
    ///     Applies a box blur to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="radius">The 'radius' value representing the size of the area to sample.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext BoxBlur(this IImageProcessingContext source, int radius, Rectangle rectangle)
    {
        return source.ApplyProcessor(new BoxBlurProcessor(radius), rectangle);
    }
}