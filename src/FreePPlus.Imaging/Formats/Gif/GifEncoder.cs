// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing;
using FreePPlus.Imaging.Processing.Processors.Quantization;

namespace FreePPlus.Imaging.Formats.Gif;

//was previously: namespace SixLabors.ImageSharp.Formats.Gif;

/// <summary>
///     Image encoder for writing image data to a stream in gif format.
/// </summary>
public sealed class GifEncoder : IImageEncoder, IGifEncoderOptions
{
    /// <summary>
    ///     Gets or sets the quantizer for reducing the color count.
    ///     Defaults to the <see cref="OctreeQuantizer" />
    /// </summary>
    public IQuantizer Quantizer { get; set; } = KnownQuantizers.Octree;

    /// <summary>
    ///     Gets or sets the color table mode: Global or local.
    /// </summary>
    public GifColorTableMode? ColorTableMode { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="IPixelSamplingStrategy" /> used for quantization
    ///     when building a global color table in case of <see cref="GifColorTableMode.Global" />.
    /// </summary>
    public IPixelSamplingStrategy GlobalPixelSamplingStrategy { get; set; } = new DefaultPixelSamplingStrategy();

    /// <inheritdoc />
    public void Encode<TPixel>(Image<TPixel> image, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var encoder = new GifEncoderCore(image.GetConfiguration(), this);
        encoder.Encode(image, stream);
    }
}