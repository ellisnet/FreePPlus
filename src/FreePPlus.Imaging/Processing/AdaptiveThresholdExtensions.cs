// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Binarization;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Extensions to perform AdaptiveThreshold through Mutator.
/// </summary>
public static class AdaptiveThresholdExtensions
{
    /// <summary>
    ///     Applies Bradley Adaptive Threshold to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <returns>The <see cref="Image{TPixel}" />.</returns>
    public static IImageProcessingContext AdaptiveThreshold(this IImageProcessingContext source)
    {
        return source.ApplyProcessor(new AdaptiveThresholdProcessor());
    }

    /// <summary>
    ///     Applies Bradley Adaptive Threshold to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="thresholdLimit">Threshold limit (0.0-1.0) to consider for binarization.</param>
    /// <returns>The <see cref="Image{TPixel}" />.</returns>
    public static IImageProcessingContext AdaptiveThreshold(this IImageProcessingContext source, float thresholdLimit)
    {
        return source.ApplyProcessor(new AdaptiveThresholdProcessor(thresholdLimit));
    }

    /// <summary>
    ///     Applies Bradley Adaptive Threshold to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="upper">Upper (white) color for thresholding.</param>
    /// <param name="lower">Lower (black) color for thresholding.</param>
    /// <returns>The <see cref="Image{TPixel}" />.</returns>
    public static IImageProcessingContext AdaptiveThreshold(this IImageProcessingContext source, Color upper,
        Color lower)
    {
        return source.ApplyProcessor(new AdaptiveThresholdProcessor(upper, lower));
    }

    /// <summary>
    ///     Applies Bradley Adaptive Threshold to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="upper">Upper (white) color for thresholding.</param>
    /// <param name="lower">Lower (black) color for thresholding.</param>
    /// <param name="thresholdLimit">Threshold limit (0.0-1.0) to consider for binarization.</param>
    /// <returns>The <see cref="Image{TPixel}" />.</returns>
    public static IImageProcessingContext AdaptiveThreshold(this IImageProcessingContext source, Color upper,
        Color lower, float thresholdLimit)
    {
        return source.ApplyProcessor(new AdaptiveThresholdProcessor(upper, lower, thresholdLimit));
    }

    /// <summary>
    ///     Applies Bradley Adaptive Threshold to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="upper">Upper (white) color for thresholding.</param>
    /// <param name="lower">Lower (black) color for thresholding.</param>
    /// <param name="rectangle">Rectangle region to apply the processor on.</param>
    /// <returns>The <see cref="Image{TPixel}" />.</returns>
    public static IImageProcessingContext AdaptiveThreshold(this IImageProcessingContext source, Color upper,
        Color lower, Rectangle rectangle)
    {
        return source.ApplyProcessor(new AdaptiveThresholdProcessor(upper, lower), rectangle);
    }

    /// <summary>
    ///     Applies Bradley Adaptive Threshold to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="upper">Upper (white) color for thresholding.</param>
    /// <param name="lower">Lower (black) color for thresholding.</param>
    /// <param name="thresholdLimit">Threshold limit (0.0-1.0) to consider for binarization.</param>
    /// <param name="rectangle">Rectangle region to apply the processor on.</param>
    /// <returns>The <see cref="Image{TPixel}" />.</returns>
    public static IImageProcessingContext AdaptiveThreshold(this IImageProcessingContext source, Color upper,
        Color lower, float thresholdLimit, Rectangle rectangle)
    {
        return source.ApplyProcessor(new AdaptiveThresholdProcessor(upper, lower, thresholdLimit), rectangle);
    }
}