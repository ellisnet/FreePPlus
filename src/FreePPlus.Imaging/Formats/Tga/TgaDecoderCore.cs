// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.Metadata;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Formats.Tga;

//was previously: namespace SixLabors.ImageSharp.Formats.Tga;

/// <summary>
///     Performs the tga decoding operation.
/// </summary>
internal sealed class TgaDecoderCore
{
    /// <summary>
    ///     The global configuration.
    /// </summary>
    private readonly Configuration configuration;

    /// <summary>
    ///     Used for allocating memory during processing operations.
    /// </summary>
    private readonly MemoryAllocator memoryAllocator;

    /// <summary>
    ///     The bitmap decoder options.
    /// </summary>
    private readonly ITgaDecoderOptions options;

    /// <summary>
    ///     A scratch buffer to reduce allocations.
    /// </summary>
    private readonly byte[] scratchBuffer = new byte[4];

    /// <summary>
    ///     The stream to decode from.
    /// </summary>
    private Stream currentStream;

    /// <summary>
    ///     The file header containing general information about the image.
    /// </summary>
    private TgaFileHeader fileHeader;

    /// <summary>
    ///     Indicates whether there is a alpha channel present.
    /// </summary>
    private bool hasAlpha;

    /// <summary>
    ///     The metadata.
    /// </summary>
    private ImageMetadata metadata;

    /// <summary>
    ///     The tga specific metadata.
    /// </summary>
    private TgaMetadata tgaMetadata;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TgaDecoderCore" /> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="options">The options.</param>
    public TgaDecoderCore(Configuration configuration, ITgaDecoderOptions options)
    {
        this.configuration = configuration;
        memoryAllocator = configuration.MemoryAllocator;
        this.options = options;
    }

    /// <summary>
    ///     Gets the dimensions of the image.
    /// </summary>
    public Size Dimensions => new(fileHeader.Width, fileHeader.Height);

    /// <summary>
    ///     Decodes the image from the specified stream.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="stream">The stream, where the image should be decoded from. Cannot be null.</param>
    /// <exception cref="System.ArgumentNullException">
    ///     <para><paramref name="stream" /> is null.</para>
    /// </exception>
    /// <returns>The decoded image.</returns>
    public Image<TPixel> Decode<TPixel>(Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        try
        {
            var origin = ReadFileHeader(stream);
            currentStream.Skip(fileHeader.IdLength);

            // Parse the color map, if present.
            if (fileHeader.ColorMapType != 0 && fileHeader.ColorMapType != 1)
                TgaThrowHelper.ThrowNotSupportedException($"Unknown tga colormap type {fileHeader.ColorMapType} found");

            if (fileHeader.Width == 0 || fileHeader.Height == 0)
                throw new UnknownImageFormatException("Width or height cannot be 0");

            var image = Image.CreateUninitialized<TPixel>(configuration, fileHeader.Width, fileHeader.Height, metadata,
                TgaFormat.Instance);
            var pixels = image.GetRootFramePixelBuffer();

            if (fileHeader.ColorMapType == 1)
            {
                if (fileHeader.CMapLength <= 0)
                    TgaThrowHelper.ThrowInvalidImageContentException("Missing tga color map length");

                if (fileHeader.CMapDepth <= 0)
                    TgaThrowHelper.ThrowInvalidImageContentException("Missing tga color map depth");

                var colorMapPixelSizeInBytes = fileHeader.CMapDepth / 8;
                var colorMapSizeInBytes = fileHeader.CMapLength * colorMapPixelSizeInBytes;
                using (var palette =
                       memoryAllocator.AllocateManagedByteBuffer(colorMapSizeInBytes, AllocationOptions.Clean))
                {
                    currentStream.Read(palette.Array, fileHeader.CMapStart, colorMapSizeInBytes);

                    if (fileHeader.ImageType == TgaImageType.RleColorMapped)
                        ReadPalettedRle(
                            fileHeader.Width,
                            fileHeader.Height,
                            pixels,
                            palette.Array,
                            colorMapPixelSizeInBytes,
                            origin);
                    else
                        ReadPaletted(
                            fileHeader.Width,
                            fileHeader.Height,
                            pixels,
                            palette.Array,
                            colorMapPixelSizeInBytes,
                            origin);
                }

                return image;
            }

            // Even if the image type indicates it is not a paletted image, it can still contain a palette. Skip those bytes.
            if (fileHeader.CMapLength > 0)
            {
                var colorMapPixelSizeInBytes = fileHeader.CMapDepth / 8;
                currentStream.Skip(fileHeader.CMapLength * colorMapPixelSizeInBytes);
            }

            switch (fileHeader.PixelDepth)
            {
                case 8:
                    if (fileHeader.ImageType.IsRunLengthEncoded())
                        ReadRle(fileHeader.Width, fileHeader.Height, pixels, 1, origin);
                    else
                        ReadMonoChrome(fileHeader.Width, fileHeader.Height, pixels, origin);

                    break;

                case 15:
                case 16:
                    if (fileHeader.ImageType.IsRunLengthEncoded())
                        ReadRle(fileHeader.Width, fileHeader.Height, pixels, 2, origin);
                    else
                        ReadBgra16(fileHeader.Width, fileHeader.Height, pixels, origin);

                    break;

                case 24:
                    if (fileHeader.ImageType.IsRunLengthEncoded())
                        ReadRle(fileHeader.Width, fileHeader.Height, pixels, 3, origin);
                    else
                        ReadBgr24(fileHeader.Width, fileHeader.Height, pixels, origin);

                    break;

                case 32:
                    if (fileHeader.ImageType.IsRunLengthEncoded())
                        ReadRle(fileHeader.Width, fileHeader.Height, pixels, 4, origin);
                    else
                        ReadBgra32(fileHeader.Width, fileHeader.Height, pixels, origin);

                    break;

                default:
                    TgaThrowHelper.ThrowNotSupportedException("ImageSharp does not support this kind of tga files.");
                    break;
            }

            return image;
        }
        catch (IndexOutOfRangeException e)
        {
            throw new ImageFormatException("TGA image does not have a valid format.", e);
        }
    }

    /// <summary>
    ///     Reads a uncompressed TGA image with a palette.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type.</typeparam>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="pixels">The <see cref="Buffer2D{TPixel}" /> to assign the palette to.</param>
    /// <param name="palette">The color palette.</param>
    /// <param name="colorMapPixelSizeInBytes">Color map size of one entry in bytes.</param>
    /// <param name="origin">The image origin.</param>
    private void ReadPaletted<TPixel>(int width, int height, Buffer2D<TPixel> pixels, byte[] palette,
        int colorMapPixelSizeInBytes, TgaImageOrigin origin)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        TPixel color = default;
        var invertX = InvertX(origin);

        for (var y = 0; y < height; y++)
        {
            var newY = InvertY(y, height, origin);
            var pixelRow = pixels.GetRowSpan(newY);

            switch (colorMapPixelSizeInBytes)
            {
                case 2:
                    if (invertX)
                        for (var x = width - 1; x >= 0; x--)
                            ReadPalettedBgra16Pixel(palette, colorMapPixelSizeInBytes, x, color, pixelRow);
                    else
                        for (var x = 0; x < width; x++)
                            ReadPalettedBgra16Pixel(palette, colorMapPixelSizeInBytes, x, color, pixelRow);

                    break;

                case 3:
                    if (invertX)
                        for (var x = width - 1; x >= 0; x--)
                            ReadPalettedBgr24Pixel(palette, colorMapPixelSizeInBytes, x, color, pixelRow);
                    else
                        for (var x = 0; x < width; x++)
                            ReadPalettedBgr24Pixel(palette, colorMapPixelSizeInBytes, x, color, pixelRow);

                    break;

                case 4:
                    if (invertX)
                        for (var x = width - 1; x >= 0; x--)
                            ReadPalettedBgra32Pixel(palette, colorMapPixelSizeInBytes, x, color, pixelRow);
                    else
                        for (var x = 0; x < width; x++)
                            ReadPalettedBgra32Pixel(palette, colorMapPixelSizeInBytes, x, color, pixelRow);

                    break;
            }
        }
    }

    /// <summary>
    ///     Reads a run length encoded TGA image with a palette.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type.</typeparam>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="pixels">The <see cref="Buffer2D{TPixel}" /> to assign the palette to.</param>
    /// <param name="palette">The color palette.</param>
    /// <param name="colorMapPixelSizeInBytes">Color map size of one entry in bytes.</param>
    /// <param name="origin">The image origin.</param>
    private void ReadPalettedRle<TPixel>(int width, int height, Buffer2D<TPixel> pixels, byte[] palette,
        int colorMapPixelSizeInBytes, TgaImageOrigin origin)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var bytesPerPixel = 1;
        using (var buffer = memoryAllocator.Allocate<byte>(width * height * bytesPerPixel, AllocationOptions.Clean))
        {
            TPixel color = default;
            var bufferSpan = buffer.GetSpan();
            UncompressRle(width, height, bufferSpan, 1);

            for (var y = 0; y < height; y++)
            {
                var newY = InvertY(y, height, origin);
                var pixelRow = pixels.GetRowSpan(newY);
                var rowStartIdx = y * width * bytesPerPixel;
                for (var x = 0; x < width; x++)
                {
                    var idx = rowStartIdx + x;
                    switch (colorMapPixelSizeInBytes)
                    {
                        case 1:
                            color.FromL8(Unsafe.As<byte, L8>(ref palette[bufferSpan[idx] * colorMapPixelSizeInBytes]));
                            break;
                        case 2:
                            ReadPalettedBgra16Pixel(palette, bufferSpan[idx], colorMapPixelSizeInBytes, ref color);
                            break;
                        case 3:
                            color.FromBgr24(
                                Unsafe.As<byte, Bgr24>(ref palette[bufferSpan[idx] * colorMapPixelSizeInBytes]));
                            break;
                        case 4:
                            color.FromBgra32(
                                Unsafe.As<byte, Bgra32>(ref palette[bufferSpan[idx] * colorMapPixelSizeInBytes]));
                            break;
                    }

                    var newX = InvertX(x, width, origin);
                    pixelRow[newX] = color;
                }
            }
        }
    }

    /// <summary>
    ///     Reads a uncompressed monochrome TGA image.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type.</typeparam>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="pixels">The <see cref="Buffer2D{TPixel}" /> to assign the palette to.</param>
    /// <param name="origin">the image origin.</param>
    private void ReadMonoChrome<TPixel>(int width, int height, Buffer2D<TPixel> pixels, TgaImageOrigin origin)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var invertX = InvertX(origin);
        if (invertX)
        {
            TPixel color = default;
            for (var y = 0; y < height; y++)
            {
                var newY = InvertY(y, height, origin);
                var pixelSpan = pixels.GetRowSpan(newY);
                for (var x = width - 1; x >= 0; x--) ReadL8Pixel(color, x, pixelSpan);
            }

            return;
        }

        using (var row = memoryAllocator.AllocatePaddedPixelRowBuffer(width, 1, 0))
        {
            var invertY = InvertY(origin);
            if (invertY)
                for (var y = height - 1; y >= 0; y--)
                    ReadL8Row(width, pixels, row, y);
            else
                for (var y = 0; y < height; y++)
                    ReadL8Row(width, pixels, row, y);
        }
    }

    /// <summary>
    ///     Reads a uncompressed TGA image where each pixels has 16 bit.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type.</typeparam>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="pixels">The <see cref="Buffer2D{TPixel}" /> to assign the palette to.</param>
    /// <param name="origin">The image origin.</param>
    private void ReadBgra16<TPixel>(int width, int height, Buffer2D<TPixel> pixels, TgaImageOrigin origin)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        TPixel color = default;
        var invertX = InvertX(origin);
        using (var row = memoryAllocator.AllocatePaddedPixelRowBuffer(width, 2, 0))
        {
            for (var y = 0; y < height; y++)
            {
                var newY = InvertY(y, height, origin);
                var pixelSpan = pixels.GetRowSpan(newY);

                if (invertX)
                {
                    for (var x = width - 1; x >= 0; x--)
                    {
                        currentStream.Read(scratchBuffer, 0, 2);
                        if (!hasAlpha) scratchBuffer[1] |= 1 << 7;

                        if (fileHeader.ImageType == TgaImageType.BlackAndWhite)
                            color.FromLa16(Unsafe.As<byte, La16>(ref scratchBuffer[0]));
                        else
                            color.FromBgra5551(Unsafe.As<byte, Bgra5551>(ref scratchBuffer[0]));

                        pixelSpan[x] = color;
                    }
                }
                else
                {
                    currentStream.Read(row);
                    var rowSpan = row.GetSpan();

                    if (!hasAlpha)
                        // We need to set the alpha component value to fully opaque.
                        for (var x = 1; x < rowSpan.Length; x += 2)
                            rowSpan[x] |= 1 << 7;

                    if (fileHeader.ImageType == TgaImageType.BlackAndWhite)
                        PixelOperations<TPixel>.Instance.FromLa16Bytes(configuration, rowSpan, pixelSpan, width);
                    else
                        PixelOperations<TPixel>.Instance.FromBgra5551Bytes(configuration, rowSpan, pixelSpan, width);
                }
            }
        }
    }

    /// <summary>
    ///     Reads a uncompressed TGA image where each pixels has 24 bit.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type.</typeparam>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="pixels">The <see cref="Buffer2D{TPixel}" /> to assign the palette to.</param>
    /// <param name="origin">The image origin.</param>
    private void ReadBgr24<TPixel>(int width, int height, Buffer2D<TPixel> pixels, TgaImageOrigin origin)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var invertX = InvertX(origin);
        if (invertX)
        {
            TPixel color = default;
            for (var y = 0; y < height; y++)
            {
                var newY = InvertY(y, height, origin);
                var pixelSpan = pixels.GetRowSpan(newY);
                for (var x = width - 1; x >= 0; x--) ReadBgr24Pixel(color, x, pixelSpan);
            }

            return;
        }

        using (var row = memoryAllocator.AllocatePaddedPixelRowBuffer(width, 3, 0))
        {
            var invertY = InvertY(origin);

            if (invertY)
                for (var y = height - 1; y >= 0; y--)
                    ReadBgr24Row(width, pixels, row, y);
            else
                for (var y = 0; y < height; y++)
                    ReadBgr24Row(width, pixels, row, y);
        }
    }

    /// <summary>
    ///     Reads a uncompressed TGA image where each pixels has 32 bit.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type.</typeparam>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="pixels">The <see cref="Buffer2D{TPixel}" /> to assign the palette to.</param>
    /// <param name="origin">The image origin.</param>
    private void ReadBgra32<TPixel>(int width, int height, Buffer2D<TPixel> pixels, TgaImageOrigin origin)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        TPixel color = default;
        var invertX = InvertX(origin);
        if (tgaMetadata.AlphaChannelBits == 8 && !invertX)
        {
            using (var row = memoryAllocator.AllocatePaddedPixelRowBuffer(width, 4, 0))
            {
                if (InvertY(origin))
                    for (var y = height - 1; y >= 0; y--)
                        ReadBgra32Row(width, pixels, row, y);
                else
                    for (var y = 0; y < height; y++)
                        ReadBgra32Row(width, pixels, row, y);
            }

            return;
        }

        for (var y = 0; y < height; y++)
        {
            var newY = InvertY(y, height, origin);
            var pixelRow = pixels.GetRowSpan(newY);
            if (invertX)
                for (var x = width - 1; x >= 0; x--)
                    ReadBgra32Pixel(x, color, pixelRow);
            else
                for (var x = 0; x < width; x++)
                    ReadBgra32Pixel(x, color, pixelRow);
        }
    }

    /// <summary>
    ///     Reads a run length encoded TGA image.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type.</typeparam>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="pixels">The <see cref="Buffer2D{TPixel}" /> to assign the palette to.</param>
    /// <param name="bytesPerPixel">The bytes per pixel.</param>
    /// <param name="origin">The image origin.</param>
    private void ReadRle<TPixel>(int width, int height, Buffer2D<TPixel> pixels, int bytesPerPixel,
        TgaImageOrigin origin)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        TPixel color = default;
        var alphaBits = tgaMetadata.AlphaChannelBits;
        using (var buffer = memoryAllocator.Allocate<byte>(width * height * bytesPerPixel, AllocationOptions.Clean))
        {
            var bufferSpan = buffer.GetSpan();
            UncompressRle(width, height, bufferSpan, bytesPerPixel);
            for (var y = 0; y < height; y++)
            {
                var newY = InvertY(y, height, origin);
                var pixelRow = pixels.GetRowSpan(newY);
                var rowStartIdx = y * width * bytesPerPixel;
                for (var x = 0; x < width; x++)
                {
                    var idx = rowStartIdx + x * bytesPerPixel;
                    switch (bytesPerPixel)
                    {
                        case 1:
                            color.FromL8(Unsafe.As<byte, L8>(ref bufferSpan[idx]));
                            break;
                        case 2:
                            if (!hasAlpha)
                                // Set alpha value to 1, to treat it as opaque for Bgra5551.
                                bufferSpan[idx + 1] = (byte)(bufferSpan[idx + 1] | 128);

                            if (fileHeader.ImageType == TgaImageType.RleBlackAndWhite)
                                color.FromLa16(Unsafe.As<byte, La16>(ref bufferSpan[idx]));
                            else
                                color.FromBgra5551(Unsafe.As<byte, Bgra5551>(ref bufferSpan[idx]));

                            break;
                        case 3:
                            color.FromBgr24(Unsafe.As<byte, Bgr24>(ref bufferSpan[idx]));
                            break;
                        case 4:
                            if (hasAlpha)
                            {
                                color.FromBgra32(Unsafe.As<byte, Bgra32>(ref bufferSpan[idx]));
                            }
                            else
                            {
                                var alpha = alphaBits == 0 ? byte.MaxValue : bufferSpan[idx + 3];
                                color.FromBgra32(new Bgra32(bufferSpan[idx + 2], bufferSpan[idx + 1], bufferSpan[idx],
                                    alpha));
                            }

                            break;
                    }

                    var newX = InvertX(x, width, origin);
                    pixelRow[newX] = color;
                }
            }
        }
    }

    /// <summary>
    ///     Reads the raw image information from the specified stream.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> containing image data.</param>
    public IImageInfo Identify(Stream stream)
    {
        ReadFileHeader(stream);
        return new ImageInfo(
            new PixelTypeInfo(fileHeader.PixelDepth),
            fileHeader.Width,
            fileHeader.Height,
            metadata,
            TgaFormat.Instance);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadL8Row<TPixel>(int width, Buffer2D<TPixel> pixels, IManagedByteBuffer row, int y)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        currentStream.Read(row);
        var pixelSpan = pixels.GetRowSpan(y);
        PixelOperations<TPixel>.Instance.FromL8Bytes(configuration, row.GetSpan(), pixelSpan, width);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadL8Pixel<TPixel>(TPixel color, int x, Span<TPixel> pixelSpan)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var pixelValue = (byte)currentStream.ReadByte();
        color.FromL8(Unsafe.As<byte, L8>(ref pixelValue));
        pixelSpan[x] = color;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadBgr24Pixel<TPixel>(TPixel color, int x, Span<TPixel> pixelSpan)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        currentStream.Read(scratchBuffer, 0, 3);
        color.FromBgr24(Unsafe.As<byte, Bgr24>(ref scratchBuffer[0]));
        pixelSpan[x] = color;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadBgr24Row<TPixel>(int width, Buffer2D<TPixel> pixels, IManagedByteBuffer row, int y)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        currentStream.Read(row);
        var pixelSpan = pixels.GetRowSpan(y);
        PixelOperations<TPixel>.Instance.FromBgr24Bytes(configuration, row.GetSpan(), pixelSpan, width);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadBgra32Pixel<TPixel>(int x, TPixel color, Span<TPixel> pixelRow)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        currentStream.Read(scratchBuffer, 0, 4);
        var alpha = tgaMetadata.AlphaChannelBits == 0 ? byte.MaxValue : scratchBuffer[3];
        color.FromBgra32(new Bgra32(scratchBuffer[2], scratchBuffer[1], scratchBuffer[0], alpha));
        pixelRow[x] = color;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadBgra32Row<TPixel>(int width, Buffer2D<TPixel> pixels, IManagedByteBuffer row, int y)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        currentStream.Read(row);
        var pixelSpan = pixels.GetRowSpan(y);
        PixelOperations<TPixel>.Instance.FromBgra32Bytes(configuration, row.GetSpan(), pixelSpan, width);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadPalettedBgra16Pixel<TPixel>(byte[] palette, int colorMapPixelSizeInBytes, int x, TPixel color,
        Span<TPixel> pixelRow)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var colorIndex = currentStream.ReadByte();
        ReadPalettedBgra16Pixel(palette, colorIndex, colorMapPixelSizeInBytes, ref color);
        pixelRow[x] = color;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadPalettedBgra16Pixel<TPixel>(byte[] palette, int index, int colorMapPixelSizeInBytes,
        ref TPixel color)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Bgra5551 bgra = default;
        bgra.FromBgra5551(Unsafe.As<byte, Bgra5551>(ref palette[index * colorMapPixelSizeInBytes]));

        if (!hasAlpha)
            // Set alpha value to 1, to treat it as opaque.
            bgra.PackedValue = (ushort)(bgra.PackedValue | 0x8000);

        color.FromBgra5551(bgra);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadPalettedBgr24Pixel<TPixel>(byte[] palette, int colorMapPixelSizeInBytes, int x, TPixel color,
        Span<TPixel> pixelRow)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var colorIndex = currentStream.ReadByte();
        color.FromBgr24(Unsafe.As<byte, Bgr24>(ref palette[colorIndex * colorMapPixelSizeInBytes]));
        pixelRow[x] = color;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadPalettedBgra32Pixel<TPixel>(byte[] palette, int colorMapPixelSizeInBytes, int x, TPixel color,
        Span<TPixel> pixelRow)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var colorIndex = currentStream.ReadByte();
        color.FromBgra32(Unsafe.As<byte, Bgra32>(ref palette[colorIndex * colorMapPixelSizeInBytes]));
        pixelRow[x] = color;
    }

    /// <summary>
    ///     Produce uncompressed tga data from a run length encoded stream.
    /// </summary>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="buffer">Buffer for uncompressed data.</param>
    /// <param name="bytesPerPixel">The bytes used per pixel.</param>
    private void UncompressRle(int width, int height, Span<byte> buffer, int bytesPerPixel)
    {
        var uncompressedPixels = 0;
        var pixel = new byte[bytesPerPixel];
        var totalPixels = width * height;
        while (uncompressedPixels < totalPixels)
        {
            var runLengthByte = (byte)currentStream.ReadByte();

            // The high bit of a run length packet is set to 1.
            var highBit = runLengthByte >> 7;
            if (highBit == 1)
            {
                var runLength = runLengthByte & 127;
                currentStream.Read(pixel, 0, bytesPerPixel);
                var bufferIdx = uncompressedPixels * bytesPerPixel;
                for (var i = 0; i < runLength + 1; i++, uncompressedPixels++)
                {
                    pixel.AsSpan().CopyTo(buffer.Slice(bufferIdx));
                    bufferIdx += bytesPerPixel;
                }
            }
            else
            {
                // Non-run-length encoded packet.
                int runLength = runLengthByte;
                var bufferIdx = uncompressedPixels * bytesPerPixel;
                for (var i = 0; i < runLength + 1; i++, uncompressedPixels++)
                {
                    currentStream.Read(pixel, 0, bytesPerPixel);
                    pixel.AsSpan().CopyTo(buffer.Slice(bufferIdx));
                    bufferIdx += bytesPerPixel;
                }
            }
        }
    }

    /// <summary>
    ///     Returns the y- value based on the given height.
    /// </summary>
    /// <param name="y">The y- value representing the current row.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="origin">The image origin.</param>
    /// <returns>The <see cref="int" /> representing the inverted value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int InvertY(int y, int height, TgaImageOrigin origin)
    {
        if (InvertY(origin)) return height - y - 1;

        return y;
    }

    /// <summary>
    ///     Indicates whether the y coordinates needs to be inverted, to keep a top left origin.
    /// </summary>
    /// <param name="origin">The image origin.</param>
    /// <returns>True, if y coordinate needs to be inverted.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool InvertY(TgaImageOrigin origin)
    {
        switch (origin)
        {
            case TgaImageOrigin.BottomLeft:
            case TgaImageOrigin.BottomRight:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    ///     Returns the x- value based on the given width.
    /// </summary>
    /// <param name="x">The x- value representing the current column.</param>
    /// <param name="width">The width of the image.</param>
    /// <param name="origin">The image origin.</param>
    /// <returns>The <see cref="int" /> representing the inverted value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int InvertX(int x, int width, TgaImageOrigin origin)
    {
        if (InvertX(origin)) return width - x - 1;

        return x;
    }

    /// <summary>
    ///     Indicates whether the x coordinates needs to be inverted, to keep a top left origin.
    /// </summary>
    /// <param name="origin">The image origin.</param>
    /// <returns>True, if x coordinate needs to be inverted.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool InvertX(TgaImageOrigin origin)
    {
        switch (origin)
        {
            case TgaImageOrigin.TopRight:
            case TgaImageOrigin.BottomRight:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    ///     Reads the tga file header from the stream.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> containing image data.</param>
    /// <returns>The image origin.</returns>
    private TgaImageOrigin ReadFileHeader(Stream stream)
    {
        currentStream = stream;

        Span<byte> buffer = stackalloc byte[TgaFileHeader.Size];

        currentStream.Read(buffer, 0, TgaFileHeader.Size);
        fileHeader = TgaFileHeader.Parse(buffer);
        metadata = new ImageMetadata();
        tgaMetadata = metadata.GetTgaMetadata();
        tgaMetadata.BitsPerPixel = (TgaBitsPerPixel)fileHeader.PixelDepth;

        var alphaBits = fileHeader.ImageDescriptor & 0xf;
        if (alphaBits != 0 && alphaBits != 1 && alphaBits != 8)
            TgaThrowHelper.ThrowInvalidImageContentException("Invalid alpha channel bits");

        tgaMetadata.AlphaChannelBits = (byte)alphaBits;
        hasAlpha = alphaBits > 0;

        // Bits 4 and 5 describe the image origin.
        var origin = (TgaImageOrigin)((fileHeader.ImageDescriptor & 0x30) >> 4);
        return origin;
    }
}