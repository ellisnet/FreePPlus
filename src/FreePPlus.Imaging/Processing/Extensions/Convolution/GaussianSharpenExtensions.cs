// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Convolution;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines Gaussian sharpening extensions to apply on an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class GaussianSharpenExtensions
{
    /// <summary>
    ///     Applies a Gaussian sharpening filter to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext GaussianSharpen(this IImageProcessingContext source)
    {
        return source.ApplyProcessor(new GaussianSharpenProcessor());
    }

    /// <summary>
    ///     Applies a Gaussian sharpening filter to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="sigma">The 'sigma' value representing the weight of the blur.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext GaussianSharpen(this IImageProcessingContext source, float sigma)
    {
        return source.ApplyProcessor(new GaussianSharpenProcessor(sigma));
    }

    /// <summary>
    ///     Applies a Gaussian sharpening filter to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="sigma">The 'sigma' value representing the weight of the blur.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext GaussianSharpen(
        this IImageProcessingContext source,
        float sigma,
        Rectangle rectangle)
    {
        return source.ApplyProcessor(new GaussianSharpenProcessor(sigma), rectangle);
    }
}