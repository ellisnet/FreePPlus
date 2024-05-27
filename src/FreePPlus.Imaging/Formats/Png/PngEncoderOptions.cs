// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Quantization;

namespace FreePPlus.Imaging.Formats.Png;

//was previously: namespace SixLabors.ImageSharp.Formats.Png;

/// <summary>
///     The options structure for the <see cref="PngEncoderCore" />.
/// </summary>
internal class PngEncoderOptions : IPngEncoderOptions
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PngEncoderOptions" /> class.
    /// </summary>
    /// <param name="source">The source.</param>
    public PngEncoderOptions(IPngEncoderOptions source)
    {
        BitDepth = source.BitDepth;
        ColorType = source.ColorType;

        // Specification recommends default filter method None for paletted images and Paeth for others.
        FilterMethod = source.FilterMethod ?? (source.ColorType == PngColorType.Palette
            ? PngFilterMethod.None
            : PngFilterMethod.Paeth);
        CompressionLevel = source.CompressionLevel;
        TextCompressionThreshold = source.TextCompressionThreshold;
        Gamma = source.Gamma;
        Quantizer = source.Quantizer;
        Threshold = source.Threshold;
        InterlaceMethod = source.InterlaceMethod;
    }

    /// <inheritdoc />
    public PngBitDepth? BitDepth { get; set; }

    /// <inheritdoc />
    public PngColorType? ColorType { get; set; }

    /// <inheritdoc />
    public PngFilterMethod? FilterMethod { get; }

    /// <inheritdoc />
    public PngCompressionLevel CompressionLevel { get; } = PngCompressionLevel.DefaultCompression;

    /// <inheritdoc />
    public int TextCompressionThreshold { get; }

    /// <inheritdoc />
    public float? Gamma { get; set; }

    /// <inheritdoc />
    public IQuantizer Quantizer { get; set; }

    /// <inheritdoc />
    public byte Threshold { get; }

    /// <inheritdoc />
    public PngInterlaceMode? InterlaceMethod { get; set; }
}