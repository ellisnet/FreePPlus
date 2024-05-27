// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Common.Helpers;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.Metadata;
using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing;
using FreePPlus.Imaging.Processing.Processors.Quantization;

namespace FreePPlus.Imaging.Formats.Bmp;

//was previously: namespace SixLabors.ImageSharp.Formats.Bmp;

/// <summary>
///     Image encoder for writing an image to a stream as a Windows bitmap.
/// </summary>
internal sealed class BmpEncoderCore
{
    /// <summary>
    ///     The mask for the alpha channel of the color for 32 bit rgba bitmaps.
    /// </summary>
    private const int Rgba32AlphaMask = 0xFF << 24;

    /// <summary>
    ///     The mask for the red part of the color for 32 bit rgba bitmaps.
    /// </summary>
    private const int Rgba32RedMask = 0xFF << 16;

    /// <summary>
    ///     The mask for the green part of the color for 32 bit rgba bitmaps.
    /// </summary>
    private const int Rgba32GreenMask = 0xFF << 8;

    /// <summary>
    ///     The mask for the blue part of the color for 32 bit rgba bitmaps.
    /// </summary>
    private const int Rgba32BlueMask = 0xFF;

    /// <summary>
    ///     The color palette for an 8 bit image will have 256 entry's with 4 bytes for each entry.
    /// </summary>
    private const int ColorPaletteSize8Bit = 1024;

    /// <summary>
    ///     Used for allocating memory during processing operations.
    /// </summary>
    private readonly MemoryAllocator memoryAllocator;

    /// <summary>
    ///     The quantizer for reducing the color count for 8-Bit images.
    /// </summary>
    private readonly IQuantizer quantizer;

    /// <summary>
    ///     A bitmap v4 header will only be written, if the user explicitly wants support for transparency.
    ///     In this case the compression type BITFIELDS will be used.
    ///     Otherwise a bitmap v3 header will be written, which is supported by almost all decoders.
    /// </summary>
    private readonly bool writeV4Header;

    /// <summary>
    ///     The color depth, in number of bits per pixel.
    /// </summary>
    private BmpBitsPerPixel? bitsPerPixel;

    /// <summary>
    ///     The global configuration.
    /// </summary>
    private Configuration configuration;

    /// <summary>
    ///     The amount to pad each row by.
    /// </summary>
    private int padding;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BmpEncoderCore" /> class.
    /// </summary>
    /// <param name="options">The encoder options.</param>
    /// <param name="memoryAllocator">The memory manager.</param>
    public BmpEncoderCore(IBmpEncoderOptions options, MemoryAllocator memoryAllocator)
    {
        this.memoryAllocator = memoryAllocator;
        bitsPerPixel = options.BitsPerPixel;
        writeV4Header = options.SupportTransparency;
        quantizer = options.Quantizer ?? KnownQuantizers.Octree;
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
        var bmpMetadata = metadata.GetBmpMetadata();
        bitsPerPixel = bitsPerPixel ?? bmpMetadata.BitsPerPixel;

        var bpp = (short)bitsPerPixel;
        var bytesPerLine = 4 * ((image.Width * bpp + 31) / 32);
        padding = bytesPerLine - (int)(image.Width * (bpp / 8F));

        // Set Resolution.
        var hResolution = 0;
        var vResolution = 0;

        if (metadata.ResolutionUnits != PixelResolutionUnit.AspectRatio)
            if (metadata.HorizontalResolution > 0 && metadata.VerticalResolution > 0)
                switch (metadata.ResolutionUnits)
                {
                    case PixelResolutionUnit.PixelsPerInch:

                        hResolution = (int)Math.Round(UnitConverter.InchToMeter(metadata.HorizontalResolution));
                        vResolution = (int)Math.Round(UnitConverter.InchToMeter(metadata.VerticalResolution));
                        break;

                    case PixelResolutionUnit.PixelsPerCentimeter:

                        hResolution = (int)Math.Round(UnitConverter.CmToMeter(metadata.HorizontalResolution));
                        vResolution = (int)Math.Round(UnitConverter.CmToMeter(metadata.VerticalResolution));
                        break;

                    case PixelResolutionUnit.PixelsPerMeter:
                        hResolution = (int)Math.Round(metadata.HorizontalResolution);
                        vResolution = (int)Math.Round(metadata.VerticalResolution);

                        break;
                }

        var infoHeaderSize = writeV4Header ? BmpInfoHeader.SizeV4 : BmpInfoHeader.SizeV3;
        var infoHeader = new BmpInfoHeader(
            infoHeaderSize,
            height: image.Height,
            width: image.Width,
            bitsPerPixel: bpp,
            planes: 1,
            imageSize: image.Height * bytesPerLine,
            clrUsed: 0,
            clrImportant: 0,
            xPelsPerMeter: hResolution,
            yPelsPerMeter: vResolution);

        if (writeV4Header && bitsPerPixel == BmpBitsPerPixel.Pixel32)
        {
            infoHeader.AlphaMask = Rgba32AlphaMask;
            infoHeader.RedMask = Rgba32RedMask;
            infoHeader.GreenMask = Rgba32GreenMask;
            infoHeader.BlueMask = Rgba32BlueMask;
            infoHeader.Compression = BmpCompression.BitFields;
        }

        var colorPaletteSize = bitsPerPixel == BmpBitsPerPixel.Pixel8 ? ColorPaletteSize8Bit : 0;

        var fileHeader = new BmpFileHeader(
            BmpConstants.TypeMarkers.Bitmap,
            BmpFileHeader.Size + infoHeaderSize + infoHeader.ImageSize,
            0,
            BmpFileHeader.Size + infoHeaderSize + colorPaletteSize);

        Span<byte> buffer = stackalloc byte[infoHeaderSize];
        fileHeader.WriteTo(buffer);

        stream.Write(buffer, 0, BmpFileHeader.Size);

        if (writeV4Header)
            infoHeader.WriteV4Header(buffer);
        else
            infoHeader.WriteV3Header(buffer);

        stream.Write(buffer, 0, infoHeaderSize);

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
            case BmpBitsPerPixel.Pixel32:
                Write32Bit(stream, pixels);
                break;

            case BmpBitsPerPixel.Pixel24:
                Write24Bit(stream, pixels);
                break;

            case BmpBitsPerPixel.Pixel16:
                Write16Bit(stream, pixels);
                break;

            case BmpBitsPerPixel.Pixel8:
                Write8Bit(stream, image);
                break;
        }
    }

    private IManagedByteBuffer AllocateRow(int width, int bytesPerPixel)
    {
        return memoryAllocator.AllocatePaddedPixelRowBuffer(width, bytesPerPixel, padding);
    }

    /// <summary>
    ///     Writes the 32bit color palette to the stream.
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
    ///     Writes the 24bit color palette to the stream.
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
    ///     Writes the 16bit color palette to the stream.
    /// </summary>
    /// <typeparam name="TPixel">The type of the pixel.</typeparam>
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
    ///     Writes an 8 Bit image with a color palette. The color palette has 256 entry's with 4 bytes for each entry.
    /// </summary>
    /// <typeparam name="TPixel">The type of the pixel.</typeparam>
    /// <param name="stream">The <see cref="Stream" /> to write to.</param>
    /// <param name="image"> The <see cref="ImageFrame{TPixel}" /> containing pixel data.</param>
    private void Write8Bit<TPixel>(Stream stream, ImageFrame<TPixel> image)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var isL8 = typeof(TPixel) == typeof(L8);
        using (IMemoryOwner<byte> colorPaletteBuffer =
               memoryAllocator.AllocateManagedByteBuffer(ColorPaletteSize8Bit, AllocationOptions.Clean))
        {
            var colorPalette = colorPaletteBuffer.GetSpan();
            if (isL8)
                Write8BitGray(stream, image, colorPalette);
            else
                Write8BitColor(stream, image, colorPalette);
        }
    }

    /// <summary>
    ///     Writes an 8 Bit color image with a color palette. The color palette has 256 entry's with 4 bytes for each entry.
    /// </summary>
    /// <typeparam name="TPixel">The type of the pixel.</typeparam>
    /// <param name="stream">The <see cref="Stream" /> to write to.</param>
    /// <param name="image"> The <see cref="ImageFrame{TPixel}" /> containing pixel data.</param>
    /// <param name="colorPalette">A byte span of size 1024 for the color palette.</param>
    private void Write8BitColor<TPixel>(Stream stream, ImageFrame<TPixel> image, Span<byte> colorPalette)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        using var frameQuantizer = quantizer.CreatePixelSpecificQuantizer<TPixel>(configuration);
        using var quantized = frameQuantizer.BuildPaletteAndQuantizeFrame(image, image.Bounds());

        var quantizedColors = quantized.Palette.Span;
        var color = default(Rgba32);

        // TODO: Use bulk conversion here for better perf
        var idx = 0;
        foreach (var quantizedColor in quantizedColors)
        {
            quantizedColor.ToRgba32(ref color);
            colorPalette[idx] = color.B;
            colorPalette[idx + 1] = color.G;
            colorPalette[idx + 2] = color.R;

            // Padding byte, always 0.
            colorPalette[idx + 3] = 0;
            idx += 4;
        }

        stream.Write(colorPalette);

        for (var y = image.Height - 1; y >= 0; y--)
        {
            var pixelSpan = quantized.GetPixelRowSpan(y);
            stream.Write(pixelSpan);

            for (var i = 0; i < padding; i++) stream.WriteByte(0);
        }
    }

    /// <summary>
    ///     Writes an 8 Bit gray image with a color palette. The color palette has 256 entry's with 4 bytes for each entry.
    /// </summary>
    /// <typeparam name="TPixel">The type of the pixel.</typeparam>
    /// <param name="stream">The <see cref="Stream" /> to write to.</param>
    /// <param name="image"> The <see cref="ImageFrame{TPixel}" /> containing pixel data.</param>
    /// <param name="colorPalette">A byte span of size 1024 for the color palette.</param>
    private void Write8BitGray<TPixel>(Stream stream, ImageFrame<TPixel> image, Span<byte> colorPalette)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        // Create a color palette with 256 different gray values.
        for (var i = 0; i <= 255; i++)
        {
            var idx = i * 4;
            var grayValue = (byte)i;
            colorPalette[idx] = grayValue;
            colorPalette[idx + 1] = grayValue;
            colorPalette[idx + 2] = grayValue;

            // Padding byte, always 0.
            colorPalette[idx + 3] = 0;
        }

        stream.Write(colorPalette);

        for (var y = image.Height - 1; y >= 0; y--)
        {
            ReadOnlySpan<TPixel> inputPixelRow = image.GetPixelRowSpan(y);
            var outputPixelRow = MemoryMarshal.AsBytes(inputPixelRow);
            stream.Write(outputPixelRow);

            for (var i = 0; i < padding; i++) stream.WriteByte(0);
        }
    }
}