// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Formats.Tga;

//was previously: namespace SixLabors.ImageSharp.Formats.Tga;

/// <summary>
///     Image encoder for writing an image to a stream as a truevision targa image.
/// </summary>
internal sealed class TgaEncoderCore
{
    /// <summary>
    ///     Reusable buffer for writing data.
    /// </summary>
    private readonly byte[] buffer = new byte[2];

    /// <summary>
    ///     Indicates if run length compression should be used.
    /// </summary>
    private readonly TgaCompression compression;

    /// <summary>
    ///     Used for allocating memory during processing operations.
    /// </summary>
    private readonly MemoryAllocator memoryAllocator;

    /// <summary>
    ///     The color depth, in number of bits per pixel.
    /// </summary>
    private TgaBitsPerPixel? bitsPerPixel;

    /// <summary>
    ///     The global configuration.
    /// </summary>
    private Configuration configuration;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TgaEncoderCore" /> class.
    /// </summary>
    /// <param name="options">The encoder options.</param>
    /// <param name="memoryAllocator">The memory manager.</param>
    public TgaEncoderCore(ITgaEncoderOptions options, MemoryAllocator memoryAllocator)
    {
        this.memoryAllocator = memoryAllocator;
        bitsPerPixel = options.BitsPerPixel;
        compression = options.Compression;
    }

    /// <summary>
    ///     Encodes the image to the specified stream from the <see cref="ImageFrame{TPixel}" />.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="image">The <see cref="ImageFrame{TPixel}" /> to encode from.</param>
    /// <param name="stream">The <see cref="Stream" /> to encode the image data to.</param>
    public void Encode<TPixel>(Image<TPixel> image, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(image, nameof(image));
        Guard.NotNull(stream, nameof(stream));

        configuration = image.GetConfiguration();
        var metadata = image.Metadata;
        var tgaMetadata = metadata.GetTgaMetadata();
        bitsPerPixel = bitsPerPixel ?? tgaMetadata.BitsPerPixel;

        var imageType = compression is TgaCompression.RunLength ? TgaImageType.RleTrueColor : TgaImageType.TrueColor;
        if (bitsPerPixel == TgaBitsPerPixel.Pixel8)
            imageType = compression is TgaCompression.RunLength
                ? TgaImageType.RleBlackAndWhite
                : TgaImageType.BlackAndWhite;

        byte imageDescriptor = 0;
        if (compression is TgaCompression.RunLength)
            // If compression is used, set bit 5 of the image descriptor to indicate a left top origin.
            imageDescriptor |= 0x20;

        if (bitsPerPixel is TgaBitsPerPixel.Pixel32)
            // Indicate, that 8 bit are used for the alpha channel.
            imageDescriptor |= 0x8;

        if (bitsPerPixel is TgaBitsPerPixel.Pixel16)
            // Indicate, that 1 bit is used for the alpha channel.
            imageDescriptor |= 0x1;

        var fileHeader = new TgaFileHeader(
            0,
            0,
            imageType,
            0,
            0,
            0,
            0,
            compression is TgaCompression.RunLength
                ? (short)image.Height
                : (short)0, // When run length encoding is used, the origin should be top left instead of the default bottom left.
            (short)image.Width,
            (short)image.Height,
            (byte)bitsPerPixel.Value,
            imageDescriptor);

        Span<byte> buffer = stackalloc byte[TgaFileHeader.Size];
        fileHeader.WriteTo(buffer);

        stream.Write(buffer, 0, TgaFileHeader.Size);

        if (compression is TgaCompression.RunLength)
            WriteRunLengthEncodedImage(stream, image.Frames.RootFrame);
        else
            WriteImage(stream, image.Frames.RootFrame);

        stream.Flush();
    }

    /// <summary>
    ///     Writes the pixel data to the binary stream.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="stream">The <see cref="Stream" /> to write to.</param>
    /// <param name="image">
    ///     The <see cref="ImageFrame{TPixel}" /> containing pixel data.
    /// </param>
    private void WriteImage<TPixel>(Stream stream, ImageFrame<TPixel> image)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var pixels = image.PixelBuffer;
        switch (bitsPerPixel)
        {
            case TgaBitsPerPixel.Pixel8:
                Write8Bit(stream, pixels);
                break;

            case TgaBitsPerPixel.Pixel16:
                Write16Bit(stream, pixels);
                break;

            case TgaBitsPerPixel.Pixel24:
                Write24Bit(stream, pixels);
                break;

            case TgaBitsPerPixel.Pixel32:
                Write32Bit(stream, pixels);
                break;
        }
    }

    /// <summary>
    ///     Writes a run length encoded tga image to the stream.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type.</typeparam>
    /// <param name="stream">The stream to write the image to.</param>
    /// <param name="image">The image to encode.</param>
    private void WriteRunLengthEncodedImage<TPixel>(Stream stream, ImageFrame<TPixel> image)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Rgba32 color = default;
        var pixels = image.PixelBuffer;
        var totalPixels = image.Width * image.Height;
        var encodedPixels = 0;
        while (encodedPixels < totalPixels)
        {
            var x = encodedPixels % pixels.Width;
            var y = encodedPixels / pixels.Width;
            var currentPixel = pixels[x, y];
            currentPixel.ToRgba32(ref color);
            var equalPixelCount = FindEqualPixels(pixels, x, y);

            // Write the number of equal pixels, with the high bit set, indicating ist a compressed pixel run.
            stream.WriteByte((byte)(equalPixelCount | 128));
            switch (bitsPerPixel)
            {
                case TgaBitsPerPixel.Pixel8:
                    var luminance = GetLuminance(currentPixel);
                    stream.WriteByte((byte)luminance);
                    break;

                case TgaBitsPerPixel.Pixel16:
                    var bgra5551 = new Bgra5551(color.ToVector4());
                    BinaryPrimitives.TryWriteInt16LittleEndian(buffer, (short)bgra5551.PackedValue);
                    stream.WriteByte(buffer[0]);
                    stream.WriteByte(buffer[1]);

                    break;

                case TgaBitsPerPixel.Pixel24:
                    stream.WriteByte(color.B);
                    stream.WriteByte(color.G);
                    stream.WriteByte(color.R);
                    break;

                case TgaBitsPerPixel.Pixel32:
                    stream.WriteByte(color.B);
                    stream.WriteByte(color.G);
                    stream.WriteByte(color.R);
                    stream.WriteByte(color.A);
                    break;
            }

            encodedPixels += equalPixelCount + 1;
        }
    }

    /// <summary>
    ///     Finds consecutive pixels which have the same value.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type.</typeparam>
    /// <param name="pixels">The pixels of the image.</param>
    /// <param name="xStart">X coordinate to start searching for the same pixels.</param>
    /// <param name="yStart">Y coordinate to start searching for the same pixels.</param>
    /// <returns>The number of equal pixels.</returns>
    private byte FindEqualPixels<TPixel>(Buffer2D<TPixel> pixels, int xStart, int yStart)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        byte equalPixelCount = 0;
        var firstRow = true;
        var startPixel = pixels[xStart, yStart];
        for (var y = yStart; y < pixels.Height; y++)
        {
            for (var x = firstRow ? xStart + 1 : 0; x < pixels.Width; x++)
            {
                var nextPixel = pixels[x, y];
                if (startPixel.Equals(nextPixel))
                    equalPixelCount++;
                else
                    return equalPixelCount;

                if (equalPixelCount >= 127) return equalPixelCount;
            }

            firstRow = false;
        }

        return equalPixelCount;
    }

    private IManagedByteBuffer AllocateRow(int width, int bytesPerPixel)
    {
        return memoryAllocator.AllocatePaddedPixelRowBuffer(width, bytesPerPixel, 0);
    }

    /// <summary>
    ///     Writes the 8bit pixels uncompressed to the stream.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="stream">The <see cref="Stream" /> to write to.</param>
    /// <param name="pixels">The <see cref="Buffer2D{TPixel}" /> containing pixel data.</param>
    private void Write8Bit<TPixel>(Stream stream, Buffer2D<TPixel> pixels)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        using (var row = AllocateRow(pixels.Width, 1))
        {
            for (var y = pixels.Height - 1; y >= 0; y--)
            {
                var pixelSpan = pixels.GetRowSpan(y);
                PixelOperations<TPixel>.Instance.ToL8Bytes(
                    configuration,
                    pixelSpan,
                    row.GetSpan(),
                    pixelSpan.Length);
                stream.Write(row.Array, 0, row.Length());
            }
        }
    }

    /// <summary>
    ///     Writes the 16bit pixels uncompressed to the stream.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="stream">The <see cref="Stream" /> to write to.</param>
    /// <param name="pixels">The <see cref="Buffer2D{TPixel}" /> containing pixel data.</param>
    private void Write16Bit<TPixel>(Stream stream, Buffer2D<TPixel> pixels)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        using (var row = AllocateRow(pixels.Width, 2))
        {
            for (var y = pixels.Height - 1; y >= 0; y--)
            {
                var pixelSpan = pixels.GetRowSpan(y);
                PixelOperations<TPixel>.Instance.ToBgra5551Bytes(
                    configuration,
                    pixelSpan,
                    row.GetSpan(),
                    pixelSpan.Length);
                stream.Write(row.Array, 0, row.Length());
            }
        }
    }

    /// <summary>
    ///     Writes the 24bit pixels uncompressed to the stream.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="stream">The <see cref="Stream" /> to write to.</param>
    /// <param name="pixels">The <see cref="Buffer2D{TPixel}" /> containing pixel data.</param>
    private void Write24Bit<TPixel>(Stream stream, Buffer2D<TPixel> pixels)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        using (var row = AllocateRow(pixels.Width, 3))
        {
            for (var y = pixels.Height - 1; y >= 0; y--)
            {
                var pixelSpan = pixels.GetRowSpan(y);
                PixelOperations<TPixel>.Instance.ToBgr24Bytes(
                    configuration,
                    pixelSpan,
                    row.GetSpan(),
                    pixelSpan.Length);
                stream.Write(row.Array, 0, row.Length());
            }
        }
    }

    /// <summary>
    ///     Writes the 32bit pixels uncompressed to the stream.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="stream">The <see cref="Stream" /> to write to.</param>
    /// <param name="pixels">The <see cref="Buffer2D{TPixel}" /> containing pixel data.</param>
    private void Write32Bit<TPixel>(Stream stream, Buffer2D<TPixel> pixels)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        using (var row = AllocateRow(pixels.Width, 4))
        {
            for (var y = pixels.Height - 1; y >= 0; y--)
            {
                var pixelSpan = pixels.GetRowSpan(y);
                PixelOperations<TPixel>.Instance.ToBgra32Bytes(
                    configuration,
                    pixelSpan,
                    row.GetSpan(),
                    pixelSpan.Length);
                stream.Write(row.Array, 0, row.Length());
            }
        }
    }

    /// <summary>
    ///     Convert the pixel values to grayscale using ITU-R Recommendation BT.709.
    /// </summary>
    /// <param name="sourcePixel">The pixel to get the luminance from.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static int GetLuminance<TPixel>(TPixel sourcePixel)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var vector = sourcePixel.ToVector4();
        return ImageMaths.GetBT709Luminance(ref vector, 256);
    }
}