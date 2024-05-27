// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.Metadata;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Formats.Gif;

//was previously: namespace SixLabors.ImageSharp.Formats.Gif;

/// <summary>
///     Decoder for generating an image out of a gif encoded stream.
/// </summary>
public sealed class GifDecoder : IImageDecoder, IGifDecoderOptions, IImageInfoDetector
{
    /// <summary>
    ///     Gets or sets a value indicating whether the metadata should be ignored when the image is being decoded.
    /// </summary>
    public bool IgnoreMetadata { get; set; } = false;

    /// <summary>
    ///     Gets or sets the decoding mode for multi-frame images
    /// </summary>
    public FrameDecodingMode DecodingMode { get; set; } = FrameDecodingMode.All;

    /// <inheritdoc />
    public Image<TPixel> Decode<TPixel>(Configuration configuration, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var decoder = new GifDecoderCore(configuration, this);

        try
        {
            return decoder.Decode<TPixel>(stream);
        }
        catch (InvalidMemoryOperationException ex)
        {
            var dims = decoder.Dimensions;

            GifThrowHelper.ThrowInvalidImageContentException(
                $"Can not decode image. Failed to allocate buffers for possibly degenerate dimensions: {dims.Width}x{dims.Height}.",
                ex);

            // Not reachable, as the previous statement will throw a exception.
            return null;
        }
    }

    /// <inheritdoc />
    public Image Decode(Configuration configuration, Stream stream)
    {
        return Decode<Rgba32>(configuration, stream);
    }

    /// <inheritdoc />
    public IImageInfo Identify(Configuration configuration, Stream stream)
    {
        Guard.NotNull(stream, nameof(stream));

        var decoder = new GifDecoderCore(configuration, this);
        return decoder.Identify(stream);
    }
}