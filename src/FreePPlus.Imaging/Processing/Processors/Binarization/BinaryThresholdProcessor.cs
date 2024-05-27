// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Binarization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Binarization;

/// <summary>
///     Performs simple binary threshold filtering against an image.
/// </summary>
public class BinaryThresholdProcessor : IImageProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BinaryThresholdProcessor" /> class.
    /// </summary>
    /// <param name="threshold">The threshold to split the image. Must be between 0 and 1.</param>
    public BinaryThresholdProcessor(float threshold)
        : this(threshold, Color.White, Color.Black) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BinaryThresholdProcessor" /> class.
    /// </summary>
    /// <param name="threshold">The threshold to split the image. Must be between 0 and 1.</param>
    /// <param name="upperColor">The color to use for pixels that are above the threshold.</param>
    /// <param name="lowerColor">The color to use for pixels that are below the threshold.</param>
    public BinaryThresholdProcessor(float threshold, Color upperColor, Color lowerColor)
    {
        Guard.MustBeBetweenOrEqualTo(threshold, 0, 1, nameof(threshold));
        Threshold = threshold;
        UpperColor = upperColor;
        LowerColor = lowerColor;
    }

    /// <summary>
    ///     Gets the threshold value.
    /// </summary>
    public float Threshold { get; }

    /// <summary>
    ///     Gets the color to use for pixels that are above the threshold.
    /// </summary>
    public Color UpperColor { get; }

    /// <summary>
    ///     Gets the color to use for pixels that fall below the threshold.
    /// </summary>
    public Color LowerColor { get; }

    /// <inheritdoc />
    public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration,
        Image<TPixel> source, Rectangle sourceRectangle)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return new BinaryThresholdProcessor<TPixel>(configuration, this, source, sourceRectangle);
    }
}