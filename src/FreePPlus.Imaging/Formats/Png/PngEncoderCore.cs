// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Formats.Png.Chunks;
using FreePPlus.Imaging.Formats.Png.Filters;
using FreePPlus.Imaging.Formats.Png.Zlib;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.Metadata;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Formats.Png;

//was previously: namespace SixLabors.ImageSharp.Formats.Png;

/// <summary>
///     Performs the png encoding operation.
/// </summary>
internal sealed class PngEncoderCore : IDisposable
{
    /// <summary>
    ///     The maximum block size, defaults at 64k for uncompressed blocks.
    /// </summary>
    private const int MaxBlockSize = 65535;

    /// <summary>
    ///     Reusable buffer for writing general data.
    /// </summary>
    private readonly byte[] buffer = new byte[8];

    /// <summary>
    ///     Reusable buffer for writing chunk data.
    /// </summary>
    private readonly byte[] chunkDataBuffer = new byte[16];

    /// <summary>
    ///     The configuration instance for the decoding operation.
    /// </summary>
    private readonly Configuration configuration;

    /// <summary>
    ///     Reusable CRC for validating chunks.
    /// </summary>
    private readonly Crc32 crc = new();

    /// <summary>
    ///     Used the manage memory allocations.
    /// </summary>
    private readonly MemoryAllocator memoryAllocator;

    /// <summary>
    ///     The encoder options
    /// </summary>
    private readonly PngEncoderOptions options;

    /// <summary>
    ///     The ext buffer for the average filter, <see cref="PngFilterMethod.Adaptive" />.
    /// </summary>
    private IManagedByteBuffer averageFilter;

    /// <summary>
    ///     The bit depth.
    /// </summary>
    private byte bitDepth;

    /// <summary>
    ///     The number of bytes per pixel.
    /// </summary>
    private int bytesPerPixel;

    /// <summary>
    ///     The raw data of current scanline.
    /// </summary>
    private IManagedByteBuffer currentScanline;

    /// <summary>
    ///     The common buffer for the filters.
    /// </summary>
    private IManagedByteBuffer filterBuffer;

    /// <summary>
    ///     The image height.
    /// </summary>
    private int height;

    /// <summary>
    ///     The ext buffer for the Paeth filter, <see cref="PngFilterMethod.Adaptive" />.
    /// </summary>
    private IManagedByteBuffer paethFilter;

    /// <summary>
    ///     The raw data of previous scanline.
    /// </summary>
    private IManagedByteBuffer previousScanline;

    /// <summary>
    ///     The ext buffer for the sub filter, <see cref="PngFilterMethod.Adaptive" />.
    /// </summary>
    private IManagedByteBuffer subFilter;

    /// <summary>
    ///     Gets or sets a value indicating whether to use 16 bit encoding for supported color types.
    /// </summary>
    private bool use16Bit;

    /// <summary>
    ///     The image width.
    /// </summary>
    private int width;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PngEncoderCore" /> class.
    /// </summary>
    /// <param name="memoryAllocator">The <see cref="MemoryAllocator" /> to use for buffer allocations.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="options">The options for influencing the encoder</param>
    public PngEncoderCore(MemoryAllocator memoryAllocator, Configuration configuration, PngEncoderOptions options)
    {
        this.memoryAllocator = memoryAllocator;
        this.configuration = configuration;
        this.options = options;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        previousScanline?.Dispose();
        currentScanline?.Dispose();
        subFilter?.Dispose();
        averageFilter?.Dispose();
        paethFilter?.Dispose();
        filterBuffer?.Dispose();

        previousScanline = null;
        currentScanline = null;
        subFilter = null;
        averageFilter = null;
        paethFilter = null;
        filterBuffer = null;
    }

    /// <summary>
    ///     Encodes the image to the specified stream from the <see cref="Image{TPixel}" />.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="image">The <see cref="ImageFrame{TPixel}" /> to encode from.</param>
    /// <param name="stream">The <see cref="Stream" /> to encode the image data to.</param>
    public void Encode<TPixel>(Image<TPixel> image, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(image, nameof(image));
        Guard.NotNull(stream, nameof(stream));

        width = image.Width;
        height = image.Height;

        var metadata = image.Metadata;
        var pngMetadata = metadata.GetPngMetadata();
        PngEncoderOptionsHelpers.AdjustOptions<TPixel>(options, pngMetadata, out use16Bit, out bytesPerPixel);
        var quantized = PngEncoderOptionsHelpers.CreateQuantizedFrame(options, image);
        bitDepth = PngEncoderOptionsHelpers.CalculateBitDepth(options, image, quantized);

        stream.Write(PngConstants.HeaderBytes);

        WriteHeaderChunk(stream);
        WriteGammaChunk(stream);
        WritePaletteChunk(stream, quantized);
        WriteTransparencyChunk(stream, pngMetadata);
        WritePhysicalChunk(stream, metadata);
        WriteExifChunk(stream, metadata);
        WriteTextChunks(stream, pngMetadata);
        WriteDataChunks(image.Frames.RootFrame, quantized, stream);
        WriteEndChunk(stream);
        stream.Flush();

        quantized?.Dispose();
    }

    /// <summary>Collects a row of grayscale pixels.</summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="rowSpan">The image row span.</param>
    private void CollectGrayscaleBytes<TPixel>(ReadOnlySpan<TPixel> rowSpan)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        ref var rowSpanRef = ref MemoryMarshal.GetReference(rowSpan);
        var rawScanlineSpan = currentScanline.GetSpan();
        ref var rawScanlineSpanRef = ref MemoryMarshal.GetReference(rawScanlineSpan);

        if (options.ColorType == PngColorType.Grayscale)
        {
            if (use16Bit)
            {
                // 16 bit grayscale
                using (var luminanceBuffer = memoryAllocator.Allocate<L16>(rowSpan.Length))
                {
                    var luminanceSpan = luminanceBuffer.GetSpan();
                    ref var luminanceRef = ref MemoryMarshal.GetReference(luminanceSpan);
                    PixelOperations<TPixel>.Instance.ToL16(configuration, rowSpan, luminanceSpan);

                    // Can't map directly to byte array as it's big-endian.
                    for (int x = 0, o = 0; x < luminanceSpan.Length; x++, o += 2)
                    {
                        var luminance = Unsafe.Add(ref luminanceRef, x);
                        BinaryPrimitives.WriteUInt16BigEndian(rawScanlineSpan.Slice(o, 2), luminance.PackedValue);
                    }
                }
            }
            else
            {
                if (bitDepth == 8)
                    // 8 bit grayscale
                    PixelOperations<TPixel>.Instance.ToL8Bytes(
                        configuration,
                        rowSpan,
                        rawScanlineSpan,
                        rowSpan.Length);
                else
                    // 1, 2, and 4 bit grayscale
                    using (var temp = memoryAllocator.AllocateManagedByteBuffer(
                               rowSpan.Length,
                               AllocationOptions.Clean))
                    {
                        var scaleFactor = 255 / (ImageMaths.GetColorCountForBitDepth(bitDepth) - 1);
                        var tempSpan = temp.GetSpan();

                        // We need to first create an array of luminance bytes then scale them down to the correct bit depth.
                        PixelOperations<TPixel>.Instance.ToL8Bytes(
                            configuration,
                            rowSpan,
                            tempSpan,
                            rowSpan.Length);
                        PngEncoderHelpers.ScaleDownFrom8BitArray(tempSpan, rawScanlineSpan, bitDepth, scaleFactor);
                    }
            }
        }
        else
        {
            if (use16Bit)
            {
                // 16 bit grayscale + alpha
                // TODO: Should we consider in the future a GrayAlpha32 type.
                using (var rgbaBuffer = memoryAllocator.Allocate<Rgba64>(rowSpan.Length))
                {
                    var rgbaSpan = rgbaBuffer.GetSpan();
                    ref var rgbaRef = ref MemoryMarshal.GetReference(rgbaSpan);
                    PixelOperations<TPixel>.Instance.ToRgba64(configuration, rowSpan, rgbaSpan);

                    // Can't map directly to byte array as it's big endian.
                    for (int x = 0, o = 0; x < rgbaSpan.Length; x++, o += 4)
                    {
                        var rgba = Unsafe.Add(ref rgbaRef, x);
                        var luminance = ImageMaths.Get16BitBT709Luminance(rgba.R, rgba.G, rgba.B);
                        BinaryPrimitives.WriteUInt16BigEndian(rawScanlineSpan.Slice(o, 2), luminance);
                        BinaryPrimitives.WriteUInt16BigEndian(rawScanlineSpan.Slice(o + 2, 2), rgba.A);
                    }
                }
            }
            else
            {
                // 8 bit grayscale + alpha
                // TODO: Should we consider in the future a GrayAlpha16 type.
                Rgba32 rgba = default;
                for (int x = 0, o = 0; x < rowSpan.Length; x++, o += 2)
                {
                    Unsafe.Add(ref rowSpanRef, x).ToRgba32(ref rgba);
                    Unsafe.Add(ref rawScanlineSpanRef, o) =
                        ImageMaths.Get8BitBT709Luminance(rgba.R, rgba.G, rgba.B);
                    Unsafe.Add(ref rawScanlineSpanRef, o + 1) = rgba.A;
                }
            }
        }
    }

    /// <summary>
    ///     Collects a row of true color pixel data.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="rowSpan">The row span.</param>
    private void CollectTPixelBytes<TPixel>(ReadOnlySpan<TPixel> rowSpan)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var rawScanlineSpan = currentScanline.GetSpan();

        switch (bytesPerPixel)
        {
            case 4:
            {
                // 8 bit Rgba
                PixelOperations<TPixel>.Instance.ToRgba32Bytes(
                    configuration,
                    rowSpan,
                    rawScanlineSpan,
                    rowSpan.Length);
                break;
            }

            case 3:
            {
                // 8 bit Rgb
                PixelOperations<TPixel>.Instance.ToRgb24Bytes(
                    configuration,
                    rowSpan,
                    rawScanlineSpan,
                    rowSpan.Length);
                break;
            }

            case 8:
            {
                // 16 bit Rgba
                using (var rgbaBuffer = memoryAllocator.Allocate<Rgba64>(rowSpan.Length))
                {
                    var rgbaSpan = rgbaBuffer.GetSpan();
                    ref var rgbaRef = ref MemoryMarshal.GetReference(rgbaSpan);
                    PixelOperations<TPixel>.Instance.ToRgba64(configuration, rowSpan, rgbaSpan);

                    // Can't map directly to byte array as it's big endian.
                    for (int x = 0, o = 0; x < rowSpan.Length; x++, o += 8)
                    {
                        var rgba = Unsafe.Add(ref rgbaRef, x);
                        BinaryPrimitives.WriteUInt16BigEndian(rawScanlineSpan.Slice(o, 2), rgba.R);
                        BinaryPrimitives.WriteUInt16BigEndian(rawScanlineSpan.Slice(o + 2, 2), rgba.G);
                        BinaryPrimitives.WriteUInt16BigEndian(rawScanlineSpan.Slice(o + 4, 2), rgba.B);
                        BinaryPrimitives.WriteUInt16BigEndian(rawScanlineSpan.Slice(o + 6, 2), rgba.A);
                    }
                }

                break;
            }

            default:
            {
                // 16 bit Rgb
                using (var rgbBuffer = memoryAllocator.Allocate<Rgb48>(rowSpan.Length))
                {
                    var rgbSpan = rgbBuffer.GetSpan();
                    ref var rgbRef = ref MemoryMarshal.GetReference(rgbSpan);
                    PixelOperations<TPixel>.Instance.ToRgb48(configuration, rowSpan, rgbSpan);

                    // Can't map directly to byte array as it's big endian.
                    for (int x = 0, o = 0; x < rowSpan.Length; x++, o += 6)
                    {
                        var rgb = Unsafe.Add(ref rgbRef, x);
                        BinaryPrimitives.WriteUInt16BigEndian(rawScanlineSpan.Slice(o, 2), rgb.R);
                        BinaryPrimitives.WriteUInt16BigEndian(rawScanlineSpan.Slice(o + 2, 2), rgb.G);
                        BinaryPrimitives.WriteUInt16BigEndian(rawScanlineSpan.Slice(o + 4, 2), rgb.B);
                    }
                }

                break;
            }
        }
    }

    /// <summary>
    ///     Encodes the pixel data line by line.
    ///     Each scanline is encoded in the most optimal manner to improve compression.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="rowSpan">The row span.</param>
    /// <param name="quantized">The quantized pixels. Can be null.</param>
    /// <param name="row">The row.</param>
    private void CollectPixelBytes<TPixel>(ReadOnlySpan<TPixel> rowSpan, IndexedImageFrame<TPixel> quantized, int row)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        switch (options.ColorType)
        {
            case PngColorType.Palette:

                if (bitDepth < 8)
                    PngEncoderHelpers.ScaleDownFrom8BitArray(quantized.GetPixelRowSpan(row), currentScanline.GetSpan(),
                        bitDepth);
                else
                    quantized.GetPixelRowSpan(row).CopyTo(currentScanline.GetSpan());

                break;
            case PngColorType.Grayscale:
            case PngColorType.GrayscaleWithAlpha:
                CollectGrayscaleBytes(rowSpan);
                break;
            default:
                CollectTPixelBytes(rowSpan);
                break;
        }
    }

    /// <summary>
    ///     Apply filter for the raw scanline.
    /// </summary>
    private IManagedByteBuffer FilterPixelBytes()
    {
        switch (options.FilterMethod)
        {
            case PngFilterMethod.None:
                NoneFilter.Encode(currentScanline.GetSpan(), filterBuffer.GetSpan());
                return filterBuffer;

            case PngFilterMethod.Sub:
                SubFilter.Encode(currentScanline.GetSpan(), filterBuffer.GetSpan(), bytesPerPixel, out var _);
                return filterBuffer;

            case PngFilterMethod.Up:
                UpFilter.Encode(currentScanline.GetSpan(), previousScanline.GetSpan(), filterBuffer.GetSpan(),
                    out var _);
                return filterBuffer;

            case PngFilterMethod.Average:
                AverageFilter.Encode(currentScanline.GetSpan(), previousScanline.GetSpan(), filterBuffer.GetSpan(),
                    bytesPerPixel, out var _);
                return filterBuffer;

            case PngFilterMethod.Paeth:
                PaethFilter.Encode(currentScanline.GetSpan(), previousScanline.GetSpan(), filterBuffer.GetSpan(),
                    bytesPerPixel, out var _);
                return filterBuffer;

            default:
                return GetOptimalFilteredScanline();
        }
    }

    /// <summary>
    ///     Encodes the pixel data line by line.
    ///     Each scanline is encoded in the most optimal manner to improve compression.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="rowSpan">The row span.</param>
    /// <param name="quantized">The quantized pixels. Can be null.</param>
    /// <param name="row">The row.</param>
    /// <returns>The <see cref="IManagedByteBuffer" /></returns>
    private IManagedByteBuffer EncodePixelRow<TPixel>(ReadOnlySpan<TPixel> rowSpan, IndexedImageFrame<TPixel> quantized,
        int row)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        CollectPixelBytes(rowSpan, quantized, row);
        return FilterPixelBytes();
    }

    /// <summary>
    ///     Encodes the indexed pixel data (with palette) for Adam7 interlaced mode.
    /// </summary>
    /// <param name="rowSpan">The row span.</param>
    private IManagedByteBuffer EncodeAdam7IndexedPixelRow(ReadOnlySpan<byte> rowSpan)
    {
        // CollectPixelBytes
        if (bitDepth < 8)
            PngEncoderHelpers.ScaleDownFrom8BitArray(rowSpan, currentScanline.GetSpan(), bitDepth);
        else
            rowSpan.CopyTo(currentScanline.GetSpan());

        return FilterPixelBytes();
    }

    /// <summary>
    ///     Applies all PNG filters to the given scanline and returns the filtered scanline that is deemed
    ///     to be most compressible, using lowest total variation as proxy for compressibility.
    /// </summary>
    /// <returns>The <see cref="T:byte[]" /></returns>
    private IManagedByteBuffer GetOptimalFilteredScanline()
    {
        // Palette images don't compress well with adaptive filtering.
        if (options.ColorType == PngColorType.Palette || bitDepth < 8)
        {
            NoneFilter.Encode(currentScanline.GetSpan(), filterBuffer.GetSpan());
            return filterBuffer;
        }

        AllocateExtBuffers();
        var scanSpan = currentScanline.GetSpan();
        var prevSpan = previousScanline.GetSpan();

        // This order, while different to the enumerated order is more likely to produce a smaller sum
        // early on which shaves a couple of milliseconds off the processing time.
        UpFilter.Encode(scanSpan, prevSpan, filterBuffer.GetSpan(), out var currentSum);

        // TODO: PERF.. We should be breaking out of the encoding for each line as soon as we hit the sum.
        // That way the above comment would actually be true. It used to be anyway...
        // If we could use SIMD for none branching filters we could really speed it up.
        var lowestSum = currentSum;
        var actualResult = filterBuffer;

        PaethFilter.Encode(scanSpan, prevSpan, paethFilter.GetSpan(), bytesPerPixel, out currentSum);

        if (currentSum < lowestSum)
        {
            lowestSum = currentSum;
            actualResult = paethFilter;
        }

        SubFilter.Encode(scanSpan, subFilter.GetSpan(), bytesPerPixel, out currentSum);

        if (currentSum < lowestSum)
        {
            lowestSum = currentSum;
            actualResult = subFilter;
        }

        AverageFilter.Encode(scanSpan, prevSpan, averageFilter.GetSpan(), bytesPerPixel, out currentSum);

        if (currentSum < lowestSum) actualResult = averageFilter;

        return actualResult;
    }

    /// <summary>
    ///     Writes the header chunk to the stream.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> containing image data.</param>
    private void WriteHeaderChunk(Stream stream)
    {
        var header = new PngHeader(
            width,
            height,
            bitDepth,
            options.ColorType.Value,
            0, // None
            0,
            options.InterlaceMethod.Value);

        header.WriteTo(chunkDataBuffer);

        WriteChunk(stream, PngChunkType.Header, chunkDataBuffer, 0, PngHeader.Size);
    }

    /// <summary>
    ///     Writes the palette chunk to the stream.
    ///     Should be written before the first IDAT chunk.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="stream">The <see cref="Stream" /> containing image data.</param>
    /// <param name="quantized">The quantized frame.</param>
    private void WritePaletteChunk<TPixel>(Stream stream, IndexedImageFrame<TPixel> quantized)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        if (quantized is null) return;

        // Grab the palette and write it to the stream.
        var palette = quantized.Palette.Span;
        var paletteLength = palette.Length;
        var colorTableLength = paletteLength * Unsafe.SizeOf<Rgb24>();
        var hasAlpha = false;

        using var colorTable = memoryAllocator.AllocateManagedByteBuffer(colorTableLength);
        using var alphaTable = memoryAllocator.AllocateManagedByteBuffer(paletteLength);

        ref var colorTableRef = ref MemoryMarshal.GetReference(MemoryMarshal.Cast<byte, Rgb24>(colorTable.GetSpan()));
        ref var alphaTableRef = ref MemoryMarshal.GetReference(alphaTable.GetSpan());

        // Bulk convert our palette to RGBA to allow assignment to tables.
        using var rgbaOwner = quantized.Configuration.MemoryAllocator.Allocate<Rgba32>(paletteLength);
        var rgbaPaletteSpan = rgbaOwner.GetSpan();
        PixelOperations<TPixel>.Instance.ToRgba32(quantized.Configuration, quantized.Palette.Span, rgbaPaletteSpan);
        ref var rgbaPaletteRef = ref MemoryMarshal.GetReference(rgbaPaletteSpan);

        // Loop, assign, and extract alpha values from the palette.
        for (var i = 0; i < paletteLength; i++)
        {
            var rgba = Unsafe.Add(ref rgbaPaletteRef, i);
            var alpha = rgba.A;

            Unsafe.Add(ref colorTableRef, i) = rgba.Rgb;
            if (alpha > options.Threshold) alpha = byte.MaxValue;

            hasAlpha = hasAlpha || alpha < byte.MaxValue;
            Unsafe.Add(ref alphaTableRef, i) = alpha;
        }

        WriteChunk(stream, PngChunkType.Palette, colorTable.Array, 0, colorTableLength);

        // Write the transparency data
        if (hasAlpha) WriteChunk(stream, PngChunkType.Transparency, alphaTable.Array, 0, paletteLength);
    }

    /// <summary>
    ///     Writes the physical dimension information to the stream.
    ///     Should be written before IDAT chunk.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> containing image data.</param>
    /// <param name="meta">The image metadata.</param>
    private void WritePhysicalChunk(Stream stream, ImageMetadata meta)
    {
        PhysicalChunkData.FromMetadata(meta).WriteTo(chunkDataBuffer);

        WriteChunk(stream, PngChunkType.Physical, chunkDataBuffer, 0, PhysicalChunkData.Size);
    }

    /// <summary>
    ///     Writes the eXIf chunk to the stream, if any EXIF Profile values are present in the metadata.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> containing image data.</param>
    /// <param name="meta">The image metadata.</param>
    private void WriteExifChunk(Stream stream, ImageMetadata meta)
    {
        if (meta.ExifProfile is null || meta.ExifProfile.Values.Count == 0) return;

        meta.SyncProfiles();
        WriteChunk(stream, PngChunkType.Exif, meta.ExifProfile.ToByteArray());
    }

    /// <summary>
    ///     Writes a text chunk to the stream. Can be either a tTXt, iTXt or zTXt chunk,
    ///     depending whether the text contains any latin characters or should be compressed.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> containing image data.</param>
    /// <param name="meta">The image metadata.</param>
    private void WriteTextChunks(Stream stream, PngMetadata meta)
    {
        const int MaxLatinCode = 255;
        for (var i = 0; i < meta.TextData.Count; i++)
        {
            var textData = meta.TextData[i];
            var hasUnicodeCharacters = false;
            foreach (var c in textData.Value)
                if (c > MaxLatinCode)
                {
                    hasUnicodeCharacters = true;
                    break;
                }

            if (hasUnicodeCharacters || !string.IsNullOrWhiteSpace(textData.LanguageTag) ||
                !string.IsNullOrWhiteSpace(textData.TranslatedKeyword))
            {
                // Write iTXt chunk.
                var keywordBytes = PngConstants.Encoding.GetBytes(textData.Keyword);
                var textBytes = textData.Value.Length > options.TextCompressionThreshold
                    ? GetCompressedTextBytes(PngConstants.TranslatedEncoding.GetBytes(textData.Value))
                    : PngConstants.TranslatedEncoding.GetBytes(textData.Value);

                var translatedKeyword = PngConstants.TranslatedEncoding.GetBytes(textData.TranslatedKeyword);
                var languageTag = PngConstants.LanguageEncoding.GetBytes(textData.LanguageTag);

                Span<byte> outputBytes = new byte[keywordBytes.Length + textBytes.Length +
                                                  translatedKeyword.Length + languageTag.Length + 5];
                keywordBytes.CopyTo(outputBytes);
                if (textData.Value.Length > options.TextCompressionThreshold)
                    // Indicate that the text is compressed.
                    outputBytes[keywordBytes.Length + 1] = 1;

                var keywordStart = keywordBytes.Length + 3;
                languageTag.CopyTo(outputBytes.Slice(keywordStart));
                var translatedKeywordStart = keywordStart + languageTag.Length + 1;
                translatedKeyword.CopyTo(outputBytes.Slice(translatedKeywordStart));
                textBytes.CopyTo(outputBytes.Slice(translatedKeywordStart + translatedKeyword.Length + 1));
                WriteChunk(stream, PngChunkType.InternationalText, outputBytes.ToArray());
            }
            else
            {
                if (textData.Value.Length > options.TextCompressionThreshold)
                {
                    // Write zTXt chunk.
                    var compressedData =
                        GetCompressedTextBytes(PngConstants.Encoding.GetBytes(textData.Value));
                    Span<byte> outputBytes = new byte[textData.Keyword.Length + compressedData.Length + 2];
                    PngConstants.Encoding.GetBytes(textData.Keyword).CopyTo(outputBytes);
                    compressedData.CopyTo(outputBytes.Slice(textData.Keyword.Length + 2));
                    WriteChunk(stream, PngChunkType.CompressedText, outputBytes.ToArray());
                }
                else
                {
                    // Write tEXt chunk.
                    Span<byte> outputBytes = new byte[textData.Keyword.Length + textData.Value.Length + 1];
                    PngConstants.Encoding.GetBytes(textData.Keyword).CopyTo(outputBytes);
                    PngConstants.Encoding.GetBytes(textData.Value)
                        .CopyTo(outputBytes.Slice(textData.Keyword.Length + 1));
                    WriteChunk(stream, PngChunkType.Text, outputBytes.ToArray());
                }
            }
        }
    }

    /// <summary>
    ///     Compresses a given text using Zlib compression.
    /// </summary>
    /// <param name="textBytes">The text bytes to compress.</param>
    /// <returns>The compressed text byte array.</returns>
    private byte[] GetCompressedTextBytes(byte[] textBytes)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var deflateStream = new ZlibDeflateStream(memoryAllocator, memoryStream, options.CompressionLevel))
            {
                deflateStream.Write(textBytes);
            }

            return memoryStream.ToArray();
        }
    }

    /// <summary>
    ///     Writes the gamma information to the stream.
    ///     Should be written before PLTE and IDAT chunk.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> containing image data.</param>
    private void WriteGammaChunk(Stream stream)
    {
        if (options.Gamma > 0)
        {
            // 4-byte unsigned integer of gamma * 100,000.
            var gammaValue = (uint)(options.Gamma * 100_000F);

            BinaryPrimitives.WriteUInt32BigEndian(chunkDataBuffer.AsSpan(0, 4), gammaValue);

            WriteChunk(stream, PngChunkType.Gamma, chunkDataBuffer, 0, 4);
        }
    }

    /// <summary>
    ///     Writes the transparency chunk to the stream.
    ///     Should be written after PLTE and before IDAT.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> containing image data.</param>
    /// <param name="pngMetadata">The image metadata.</param>
    private void WriteTransparencyChunk(Stream stream, PngMetadata pngMetadata)
    {
        if (!pngMetadata.HasTransparency) return;

        var alpha = chunkDataBuffer.AsSpan();
        if (pngMetadata.ColorType == PngColorType.Rgb)
        {
            if (pngMetadata.TransparentRgb48.HasValue && use16Bit)
            {
                var rgb = pngMetadata.TransparentRgb48.Value;
                BinaryPrimitives.WriteUInt16LittleEndian(alpha, rgb.R);
                BinaryPrimitives.WriteUInt16LittleEndian(alpha.Slice(2, 2), rgb.G);
                BinaryPrimitives.WriteUInt16LittleEndian(alpha.Slice(4, 2), rgb.B);

                WriteChunk(stream, PngChunkType.Transparency, chunkDataBuffer, 0, 6);
            }
            else if (pngMetadata.TransparentRgb24.HasValue)
            {
                alpha.Clear();
                var rgb = pngMetadata.TransparentRgb24.Value;
                alpha[1] = rgb.R;
                alpha[3] = rgb.G;
                alpha[5] = rgb.B;
                WriteChunk(stream, PngChunkType.Transparency, chunkDataBuffer, 0, 6);
            }
        }
        else if (pngMetadata.ColorType == PngColorType.Grayscale)
        {
            if (pngMetadata.TransparentL16.HasValue && use16Bit)
            {
                BinaryPrimitives.WriteUInt16LittleEndian(alpha, pngMetadata.TransparentL16.Value.PackedValue);
                WriteChunk(stream, PngChunkType.Transparency, chunkDataBuffer, 0, 2);
            }
            else if (pngMetadata.TransparentL8.HasValue)
            {
                alpha.Clear();
                alpha[1] = pngMetadata.TransparentL8.Value.PackedValue;
                WriteChunk(stream, PngChunkType.Transparency, chunkDataBuffer, 0, 2);
            }
        }
    }

    /// <summary>
    ///     Writes the pixel information to the stream.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="pixels">The image.</param>
    /// <param name="quantized">The quantized pixel data. Can be null.</param>
    /// <param name="stream">The stream.</param>
    private void WriteDataChunks<TPixel>(ImageFrame<TPixel> pixels, IndexedImageFrame<TPixel> quantized, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        byte[] buffer;
        int bufferLength;

        using (var memoryStream = new MemoryStream())
        {
            using (var deflateStream = new ZlibDeflateStream(memoryAllocator, memoryStream, options.CompressionLevel))
            {
                if (options.InterlaceMethod == PngInterlaceMode.Adam7)
                {
                    if (quantized != null)
                        EncodeAdam7IndexedPixels(quantized, deflateStream);
                    else
                        EncodeAdam7Pixels(pixels, deflateStream);
                }
                else
                {
                    EncodePixels(pixels, quantized, deflateStream);
                }
            }

            buffer = memoryStream.ToArray();
            bufferLength = buffer.Length;
        }

        // Store the chunks in repeated 64k blocks.
        // This reduces the memory load for decoding the image for many decoders.
        var numChunks = bufferLength / MaxBlockSize;

        if (bufferLength % MaxBlockSize != 0) numChunks++;

        for (var i = 0; i < numChunks; i++)
        {
            var length = bufferLength - i * MaxBlockSize;

            if (length > MaxBlockSize) length = MaxBlockSize;

            WriteChunk(stream, PngChunkType.Data, buffer, i * MaxBlockSize, length);
        }
    }

    /// <summary>
    ///     Allocates the buffers for each scanline.
    /// </summary>
    /// <param name="bytesPerScanline">The bytes per scanline.</param>
    /// <param name="resultLength">Length of the result.</param>
    private void AllocateBuffers(int bytesPerScanline, int resultLength)
    {
        // Clean up from any potential previous runs.
        subFilter?.Dispose();
        averageFilter?.Dispose();
        paethFilter?.Dispose();
        subFilter = null;
        averageFilter = null;
        paethFilter = null;

        previousScanline?.Dispose();
        currentScanline?.Dispose();
        filterBuffer?.Dispose();
        previousScanline = memoryAllocator.AllocateManagedByteBuffer(bytesPerScanline, AllocationOptions.Clean);
        currentScanline = memoryAllocator.AllocateManagedByteBuffer(bytesPerScanline, AllocationOptions.Clean);
        filterBuffer = memoryAllocator.AllocateManagedByteBuffer(resultLength, AllocationOptions.Clean);
    }

    /// <summary>
    ///     Allocates the ext buffers for adaptive filter.
    /// </summary>
    private void AllocateExtBuffers()
    {
        if (subFilter == null)
        {
            var resultLength = filterBuffer.Length();

            subFilter = memoryAllocator.AllocateManagedByteBuffer(resultLength, AllocationOptions.Clean);
            averageFilter = memoryAllocator.AllocateManagedByteBuffer(resultLength, AllocationOptions.Clean);
            paethFilter = memoryAllocator.AllocateManagedByteBuffer(resultLength, AllocationOptions.Clean);
        }
    }

    /// <summary>
    ///     Encodes the pixels.
    /// </summary>
    /// <typeparam name="TPixel">The type of the pixel.</typeparam>
    /// <param name="pixels">The pixels.</param>
    /// <param name="quantized">The quantized pixels span.</param>
    /// <param name="deflateStream">The deflate stream.</param>
    private void EncodePixels<TPixel>(ImageFrame<TPixel> pixels, IndexedImageFrame<TPixel> quantized,
        ZlibDeflateStream deflateStream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var bytesPerScanline = CalculateScanlineLength(width);
        var resultLength = bytesPerScanline + 1;
        AllocateBuffers(bytesPerScanline, resultLength);

        for (var y = 0; y < height; y++)
        {
            var r = EncodePixelRow(pixels.GetPixelRowSpan(y), quantized, y);
            deflateStream.Write(r.Array, 0, resultLength);

            var temp = currentScanline;
            currentScanline = previousScanline;
            previousScanline = temp;
        }
    }

    /// <summary>
    ///     Interlaced encoding the pixels.
    /// </summary>
    /// <typeparam name="TPixel">The type of the pixel.</typeparam>
    /// <param name="pixels">The pixels.</param>
    /// <param name="deflateStream">The deflate stream.</param>
    private void EncodeAdam7Pixels<TPixel>(ImageFrame<TPixel> pixels, ZlibDeflateStream deflateStream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var width = pixels.Width;
        var height = pixels.Height;
        for (var pass = 0; pass < 7; pass++)
        {
            var startRow = Adam7.FirstRow[pass];
            var startCol = Adam7.FirstColumn[pass];
            var blockWidth = Adam7.ComputeBlockWidth(width, pass);

            var bytesPerScanline = bytesPerPixel <= 1
                ? (blockWidth * bitDepth + 7) / 8
                : blockWidth * bytesPerPixel;

            var resultLength = bytesPerScanline + 1;

            AllocateBuffers(bytesPerScanline, resultLength);

            using (var passData = memoryAllocator.Allocate<TPixel>(blockWidth))
            {
                var destSpan = passData.Memory.Span;
                for (var row = startRow;
                     row < height;
                     row += Adam7.RowIncrement[pass])
                {
                    // collect data
                    var srcRow = pixels.GetPixelRowSpan(row);
                    for (int col = startCol, i = 0;
                         col < width;
                         col += Adam7.ColumnIncrement[pass])
                        destSpan[i++] = srcRow[col];

                    // encode data
                    // note: quantized parameter not used
                    // note: row parameter not used
                    var r = EncodePixelRow((ReadOnlySpan<TPixel>)destSpan, null, -1);
                    deflateStream.Write(r.Array, 0, resultLength);

                    var temp = currentScanline;
                    currentScanline = previousScanline;
                    previousScanline = temp;
                }
            }
        }
    }

    /// <summary>
    ///     Interlaced encoding the quantized (indexed, with palette) pixels.
    /// </summary>
    /// <typeparam name="TPixel">The type of the pixel.</typeparam>
    /// <param name="quantized">The quantized.</param>
    /// <param name="deflateStream">The deflate stream.</param>
    private void EncodeAdam7IndexedPixels<TPixel>(IndexedImageFrame<TPixel> quantized, ZlibDeflateStream deflateStream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var width = quantized.Width;
        var height = quantized.Height;
        for (var pass = 0; pass < 7; pass++)
        {
            var startRow = Adam7.FirstRow[pass];
            var startCol = Adam7.FirstColumn[pass];
            var blockWidth = Adam7.ComputeBlockWidth(width, pass);

            var bytesPerScanline = bytesPerPixel <= 1
                ? (blockWidth * bitDepth + 7) / 8
                : blockWidth * bytesPerPixel;

            var resultLength = bytesPerScanline + 1;

            AllocateBuffers(bytesPerScanline, resultLength);

            using (var passData = memoryAllocator.Allocate<byte>(blockWidth))
            {
                var destSpan = passData.Memory.Span;
                for (var row = startRow;
                     row < height;
                     row += Adam7.RowIncrement[pass])
                {
                    // collect data
                    var srcRow = quantized.GetPixelRowSpan(row);
                    for (int col = startCol, i = 0;
                         col < width;
                         col += Adam7.ColumnIncrement[pass])
                        destSpan[i++] = srcRow[col];

                    // encode data
                    var r = EncodeAdam7IndexedPixelRow(destSpan);
                    deflateStream.Write(r.Array, 0, resultLength);
                }
            }
        }
    }

    /// <summary>
    ///     Writes the chunk end to the stream.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> containing image data.</param>
    private void WriteEndChunk(Stream stream)
    {
        WriteChunk(stream, PngChunkType.End, null);
    }

    /// <summary>
    ///     Writes a chunk to the stream.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> to write to.</param>
    /// <param name="type">The type of chunk to write.</param>
    /// <param name="data">The <see cref="T:byte[]" /> containing data.</param>
    private void WriteChunk(Stream stream, PngChunkType type, byte[] data)
    {
        WriteChunk(stream, type, data, 0, data?.Length ?? 0);
    }

    /// <summary>
    ///     Writes a chunk of a specified length to the stream at the given offset.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> to write to.</param>
    /// <param name="type">The type of chunk to write.</param>
    /// <param name="data">The <see cref="T:byte[]" /> containing data.</param>
    /// <param name="offset">The position to offset the data at.</param>
    /// <param name="length">The of the data to write.</param>
    private void WriteChunk(Stream stream, PngChunkType type, byte[] data, int offset, int length)
    {
        BinaryPrimitives.WriteInt32BigEndian(buffer, length);
        BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan(4, 4), (uint)type);

        stream.Write(buffer, 0, 8);

        crc.Reset();

        crc.Update(buffer.AsSpan(4, 4)); // Write the type buffer

        if (data != null && length > 0)
        {
            stream.Write(data, offset, length);

            crc.Update(data.AsSpan(offset, length));
        }

        BinaryPrimitives.WriteUInt32BigEndian(buffer, (uint)crc.Value);

        stream.Write(buffer, 0, 4); // write the crc
    }

    /// <summary>
    ///     Calculates the scanline length.
    /// </summary>
    /// <param name="width">The width of the row.</param>
    /// <returns>
    ///     The <see cref="int" /> representing the length.
    /// </returns>
    private int CalculateScanlineLength(int width)
    {
        var mod = bitDepth == 16 ? 16 : 8;
        var scanlineLength = width * bitDepth * bytesPerPixel;

        var amount = scanlineLength % mod;
        if (amount != 0) scanlineLength += mod - amount;

        return scanlineLength / mod;
    }
}