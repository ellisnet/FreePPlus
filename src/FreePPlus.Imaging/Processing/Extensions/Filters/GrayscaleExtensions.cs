// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors;
using FreePPlus.Imaging.Processing.Processors.Filters;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines extensions that allow the application of grayscale toning to an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class GrayscaleExtensions
{
    /// <summary>
    ///     Applies <see cref="GrayscaleMode.Bt709" /> grayscale toning to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <returns>The <see cref="IImageProcessingContext" />.</returns>
    public static IImageProcessingContext Grayscale(this IImageProcessingContext source)
    {
        return Grayscale(source, GrayscaleMode.Bt709);
    }

    /// <summary>
    ///     Applies <see cref="GrayscaleMode.Bt709" /> grayscale toning to the image using the given amount.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="amount">The proportion of the conversion. Must be between 0 and 1.</param>
    /// <returns>The <see cref="IImageProcessingContext" />.</returns>
    public static IImageProcessingContext Grayscale(this IImageProcessingContext source, float amount)
    {
        return Grayscale(source, GrayscaleMode.Bt709, amount);
    }

    /// <summary>
    ///     Applies grayscale toning to the image with the given <see cref="GrayscaleMode" />.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="mode">The formula to apply to perform the operation.</param>
    /// <returns>The <see cref="IImageProcessingContext" />.</returns>
    public static IImageProcessingContext Grayscale(this IImageProcessingContext source, GrayscaleMode mode)
    {
        return Grayscale(source, mode, 1F);
    }

    /// <summary>
    ///     Applies grayscale toning to the image with the given <see cref="GrayscaleMode" /> using the given amount.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="mode">The formula to apply to perform the operation.</param>
    /// <param name="amount">The proportion of the conversion. Must be between 0 and 1.</param>
    /// <returns>The <see cref="IImageProcessingContext" />.</returns>
    public static IImageProcessingContext Grayscale(this IImageProcessingContext source, GrayscaleMode mode,
        float amount)
    {
        IImageProcessor processor = mode == GrayscaleMode.Bt709
            ? new GrayscaleBt709Processor(amount)
            : new GrayscaleBt601Processor(amount);

        source.ApplyProcessor(processor);
        return source;
    }

    /// <summary>
    ///     Applies <see cref="GrayscaleMode.Bt709" /> grayscale toning to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" />.</returns>
    public static IImageProcessingContext Grayscale(this IImageProcessingContext source, Rectangle rectangle)
    {
        return Grayscale(source, 1F, rectangle);
    }

    /// <summary>
    ///     Applies <see cref="GrayscaleMode.Bt709" /> grayscale toning to the image using the given amount.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="amount">The proportion of the conversion. Must be between 0 and 1.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" />.</returns>
    public static IImageProcessingContext Grayscale(this IImageProcessingContext source, float amount,
        Rectangle rectangle)
    {
        return Grayscale(source, GrayscaleMode.Bt709, amount, rectangle);
    }

    /// <summary>
    ///     Applies grayscale toning to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="mode">The formula to apply to perform the operation.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" />.</returns>
    public static IImageProcessingContext Grayscale(this IImageProcessingContext source, GrayscaleMode mode,
        Rectangle rectangle)
    {
        return Grayscale(source, mode, 1F, rectangle);
    }

    /// <summary>
    ///     Applies grayscale toning to the image using the given amount.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="mode">The formula to apply to perform the operation.</param>
    /// <param name="amount">The proportion of the conversion. Must be between 0 and 1.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" />.</returns>
    public static IImageProcessingContext Grayscale(this IImageProcessingContext source, GrayscaleMode mode,
        float amount, Rectangle rectangle)
    {
        IImageProcessor processor = mode == GrayscaleMode.Bt709
            ? new GrayscaleBt709Processor(amount)
            : new GrayscaleBt601Processor(amount);

        source.ApplyProcessor(processor, rectangle);
        return source;
    }
}