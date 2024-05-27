// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Quantization;

namespace FreePPlus.Imaging.Formats.Png;

//was previously: namespace SixLabors.ImageSharp.Formats.Png;

/// <summary>
///     The options available for manipulating the encoder pipeline.
/// </summary>
internal interface IPngEncoderOptions
{
    /// <summary>
    ///     Gets the number of bits per sample or per palette index (not per pixel).
    ///     Not all values are allowed for all <see cref="ColorType" /> values.
    /// </summary>
    PngBitDepth? BitDepth { get; }

    /// <summary>
    ///     Gets the color type.
    /// </summary>
    PngColorType? ColorType { get; }

    /// <summary>
    ///     Gets the filter method.
    /// </summary>
    PngFilterMethod? FilterMethod { get; }

    /// <summary>
    ///     Gets the compression level 1-9.
    ///     <remarks>Defaults to <see cref="PngCompressionLevel.DefaultCompression" />.</remarks>
    /// </summary>
    PngCompressionLevel CompressionLevel { get; }

    /// <summary>
    ///     Gets the threshold of characters in text metadata, when compression should be used.
    /// </summary>
    int TextCompressionThreshold { get; }

    /// <summary>
    ///     Gets the gamma value, that will be written the image.
    /// </summary>
    /// <value>The gamma value of the image.</value>
    float? Gamma { get; }

    /// <summary>
    ///     Gets the quantizer for reducing the color count.
    /// </summary>
    IQuantizer Quantizer { get; }

    /// <summary>
    ///     Gets the transparency threshold.
    /// </summary>
    byte Threshold { get; }

    /// <summary>
    ///     Gets a value indicating whether this instance should write an Adam7 interlaced image.
    /// </summary>
    PngInterlaceMode? InterlaceMethod { get; }
}