// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Dithering;

namespace FreePPlus.Imaging.Processing.Processors.Quantization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

/// <summary>
///     Defines options for quantization.
/// </summary>
public class QuantizerOptions
{
    private float ditherScale = QuantizerConstants.MaxDitherScale;
    private int maxColors = QuantizerConstants.MaxColors;

    /// <summary>
    ///     Gets or sets the  algorithm to apply to the output image.
    ///     Defaults to <see cref="QuantizerConstants.DefaultDither" />; set to <see langword="null" /> for no dithering.
    /// </summary>
    public IDither Dither { get; set; } = QuantizerConstants.DefaultDither;

    /// <summary>
    ///     Gets or sets the dithering scale used to adjust the amount of dither. Range 0..1.
    ///     Defaults to <see cref="QuantizerConstants.MaxDitherScale" />.
    /// </summary>
    public float DitherScale
    {
        get => ditherScale;
        set => ditherScale = value.Clamp(QuantizerConstants.MinDitherScale, QuantizerConstants.MaxDitherScale);
    }

    /// <summary>
    ///     Gets or sets the maximum number of colors to hold in the color palette. Range 0..256.
    ///     Defaults to <see cref="QuantizerConstants.MaxColors" />.
    /// </summary>
    public int MaxColors
    {
        get => maxColors;
        set => maxColors = value.Clamp(QuantizerConstants.MinColors, QuantizerConstants.MaxColors);
    }
}