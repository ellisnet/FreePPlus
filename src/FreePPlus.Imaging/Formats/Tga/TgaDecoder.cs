// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Formats.Tga;

//was previously: namespace SixLabors.ImageSharp.Formats.Tga;

/// <summary>
///     Image decoder for Truevision TGA images.
/// </summary>
public sealed class TgaDecoder : IImageDecoder, ITgaDecoderOptions, IImageInfoDetector
{
    /// <inheritdoc />
    public Image<TPixel> Decode<TPixel>(Configuration configuration, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(stream, nameof(stream));

        var decoder = new TgaDecoderCore(configuration, this);

        try
        {
            return decoder.Decode<TPixel>(stream);
        }
        catch (InvalidMemoryOperationException ex)
        {
            var dims = decoder.Dimensions;

            TgaThrowHelper.ThrowInvalidImageContentException(
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

        return new TgaDecoderCore(configuration, this).Identify(stream);
    }
}