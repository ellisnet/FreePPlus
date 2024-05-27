// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Formats.Jpeg;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg;

/// <summary>
///     Image decoder for generating an image out of a jpg stream.
/// </summary>
public sealed class JpegDecoder : IImageDecoder, IJpegDecoderOptions, IImageInfoDetector
{
    /// <inheritdoc />
    public Image<TPixel> Decode<TPixel>(Configuration configuration, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(stream, nameof(stream));

        using var decoder = new JpegDecoderCore(configuration, this);
        try
        {
            return decoder.Decode<TPixel>(stream);
        }
        catch (InvalidMemoryOperationException ex)
        {
            (var w, var h) = (decoder.ImageWidth, decoder.ImageHeight);

            JpegThrowHelper.ThrowInvalidImageContentException(
                $"Can not decode image. Failed to allocate buffers for possibly degenerate dimensions: {w}x{h}.", ex);

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

        using (var decoder = new JpegDecoderCore(configuration, this))
        {
            return decoder.Identify(stream);
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the metadata should be ignored when the image is being decoded.
    /// </summary>
    public bool IgnoreMetadata { get; set; }
}