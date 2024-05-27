// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.Metadata;
using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing.Processors.Quantization;

namespace FreePPlus.Imaging.Formats.Gif;

//was previously: namespace SixLabors.ImageSharp.Formats.Gif;

/// <summary>
///     Implements the GIF encoding protocol.
/// </summary>
internal sealed class GifEncoderCore
{
    /// <summary>
    ///     A reusable buffer used to reduce allocations.
    /// </summary>
    private readonly byte[] buffer = new byte[20];

    /// <summary>
    ///     Configuration bound to the encoding operation.
    /// </summary>
    private readonly Configuration configuration;

    /// <summary>
    ///     Used for allocating memory during processing operations.
    /// </summary>
    private readonly MemoryAllocator memoryAllocator;

    /// <summary>
    ///     The quantizer used to generate the color palette.
    /// </summary>
    private readonly IQuantizer quantizer;

    /// <summary>
    ///     The number of bits requires to store the color palette.
    /// </summary>
    private int bitDepth;

    /// <summary>
    ///     The color table mode: Global or local.
    /// </summary>
    private GifColorTableMode? colorTableMode;

    /// <summary>
    ///     The pixel sampling strategy for global quantization.
    /// </summary>
    private readonly IPixelSamplingStrategy pixelSamplingStrategy;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GifEncoderCore" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="options">The options for the encoder.</param>
    public GifEncoderCore(Configuration configuration, IGifEncoderOptions options)
    {
        this.configuration = configuration;
        memoryAllocator = configuration.MemoryAllocator;
        quantizer = options.Quantizer;
        colorTableMode = options.ColorTableMode;
        pixelSamplingStrategy = options.GlobalPixelSamplingStrategy;
    }

    /// <summary>
    ///     Encodes the image to the specified stream from the <see cref="Image{TPixel}" />.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="image">The <see cref="Image{TPixel}" /> to encode from.</param>
    /// <param name="stream">The <see cref="Stream" /> to encode the image data to.</param>
    public void Encode<TPixel>(Image<TPixel> image, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(image, nameof(image));
        Guard.NotNull(stream, nameof(stream));

        var metadata = image.Metadata;
        var gifMetadata = metadata.GetGifMetadata();
        colorTableMode ??= gifMetadata.ColorTableMode;
        var useGlobalTable = colorTableMode == GifColorTableMode.Global;

        // Quantize the image returning a palette.
        IndexedImageFrame<TPixel> quantized;

        using (var frameQuantizer = quantizer.CreatePixelSpecificQuantizer<TPixel>(configuration))
        {
            if (useGlobalTable)
            {
                frameQuantizer.BuildPalette(pixelSamplingStrategy, image);
                quantized = frameQuantizer.QuantizeFrame(image.Frames.RootFrame, image.Bounds());
            }
            else
            {
                quantized = frameQuantizer.BuildPaletteAndQuantizeFrame(image.Frames.RootFrame, image.Bounds());
            }
        }

        // Get the number of bits.
        bitDepth = ImageMaths.GetBitsNeededForColorDepth(quantized.Palette.Length);

        // Write the header.
        WriteHeader(stream);

        // Write the LSD.
        var index = GetTransparentIndex(quantized);
        WriteLogicalScreenDescriptor(metadata, image.Width, image.Height, index, useGlobalTable, stream);

        if (useGlobalTable) WriteColorTable(quantized, stream);

        // Write the comments.
        WriteComments(gifMetadata, stream);

        // Write application extension to allow additional frames.
        if (image.Frames.Count > 1) WriteApplicationExtension(stream, gifMetadata.RepeatCount);

        if (useGlobalTable)
            EncodeGlobal(image, quantized, index, stream);
        else
            EncodeLocal(image, quantized, stream);

        // Clean up.
        quantized.Dispose();

        // TODO: Write extension etc
        stream.WriteByte(GifConstants.EndIntroducer);
    }

    private void EncodeGlobal<TPixel>(Image<TPixel> image, IndexedImageFrame<TPixel> quantized, int transparencyIndex,
        Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        // The palette quantizer can reuse the same pixel map across multiple frames
        // since the palette is unchanging. This allows a reduction of memory usage across
        // multi frame gifs using a global palette.
        EuclideanPixelMap<TPixel> pixelMap = default;
        var pixelMapSet = false;
        for (var i = 0; i < image.Frames.Count; i++)
        {
            var frame = image.Frames[i];
            var metadata = frame.Metadata;
            var frameMetadata = metadata.GetGifMetadata();
            WriteGraphicalControlExtension(frameMetadata, transparencyIndex, stream);
            WriteImageDescriptor(frame, false, stream);

            if (i == 0)
            {
                WriteImageData(quantized, stream);
            }
            else
            {
                if (!pixelMapSet)
                {
                    pixelMapSet = true;
                    pixelMap = new EuclideanPixelMap<TPixel>(configuration, quantized.Palette);
                }

                using var paletteFrameQuantizer =
                    new PaletteQuantizer<TPixel>(configuration, quantizer.Options, pixelMap);
                using var paletteQuantized = paletteFrameQuantizer.QuantizeFrame(frame, frame.Bounds());
                WriteImageData(paletteQuantized, stream);
            }
        }
    }

    private void EncodeLocal<TPixel>(Image<TPixel> image, IndexedImageFrame<TPixel> quantized, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        ImageFrame<TPixel> previousFrame = null;
        GifFrameMetadata previousMeta = null;
        for (var i = 0; i < image.Frames.Count; i++)
        {
            var frame = image.Frames[i];
            var metadata = frame.Metadata;
            var frameMetadata = metadata.GetGifMetadata();
            if (quantized is null)
            {
                // Allow each frame to be encoded at whatever color depth the frame designates if set.
                if (previousFrame != null && previousMeta.ColorTableLength != frameMetadata.ColorTableLength
                                          && frameMetadata.ColorTableLength > 0)
                {
                    var options = new QuantizerOptions
                    {
                        Dither = quantizer.Options.Dither,
                        DitherScale = quantizer.Options.DitherScale,
                        MaxColors = frameMetadata.ColorTableLength
                    };

                    using var frameQuantizer = quantizer.CreatePixelSpecificQuantizer<TPixel>(configuration, options);
                    quantized = frameQuantizer.BuildPaletteAndQuantizeFrame(frame, frame.Bounds());
                }
                else
                {
                    using var frameQuantizer = quantizer.CreatePixelSpecificQuantizer<TPixel>(configuration);
                    quantized = frameQuantizer.BuildPaletteAndQuantizeFrame(frame, frame.Bounds());
                }
            }

            bitDepth = ImageMaths.GetBitsNeededForColorDepth(quantized.Palette.Length);
            WriteGraphicalControlExtension(frameMetadata, GetTransparentIndex(quantized), stream);
            WriteImageDescriptor(frame, true, stream);
            WriteColorTable(quantized, stream);
            WriteImageData(quantized, stream);

            quantized.Dispose();
            quantized = null; // So next frame can regenerate it
            previousFrame = frame;
            previousMeta = frameMetadata;
        }
    }

    /// <summary>
    ///     Returns the index of the most transparent color in the palette.
    /// </summary>
    /// <param name="quantized">The quantized frame.</param>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <returns>
    ///     The <see cref="int" />.
    /// </returns>
    private int GetTransparentIndex<TPixel>(IndexedImageFrame<TPixel> quantized)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        // Transparent pixels are much more likely to be found at the end of a palette.
        var index = -1;
        var paletteSpan = quantized.Palette.Span;

        using var rgbaOwner = quantized.Configuration.MemoryAllocator.Allocate<Rgba32>(paletteSpan.Length);
        var rgbaSpan = rgbaOwner.GetSpan();
        PixelOperations<TPixel>.Instance.ToRgba32(quantized.Configuration, paletteSpan, rgbaSpan);
        ref var rgbaSpanRef = ref MemoryMarshal.GetReference(rgbaSpan);

        for (var i = rgbaSpan.Length - 1; i >= 0; i--)
            if (Unsafe.Add(ref rgbaSpanRef, i).Equals(default))
                index = i;

        return index;
    }

    /// <summary>
    ///     Writes the file header signature and version to the stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteHeader(Stream stream)
    {
        stream.Write(GifConstants.MagicNumber);
    }

    /// <summary>
    ///     Writes the logical screen descriptor to the stream.
    /// </summary>
    /// <param name="metadata">The image metadata.</param>
    /// <param name="width">The image width.</param>
    /// <param name="height">The image height.</param>
    /// <param name="transparencyIndex">The transparency index to set the default background index to.</param>
    /// <param name="useGlobalTable">Whether to use a global or local color table.</param>
    /// <param name="stream">The stream to write to.</param>
    private void WriteLogicalScreenDescriptor(
        ImageMetadata metadata,
        int width,
        int height,
        int transparencyIndex,
        bool useGlobalTable,
        Stream stream)
    {
        var packedValue = GifLogicalScreenDescriptor.GetPackedValue(useGlobalTable, bitDepth - 1, false, bitDepth - 1);

        // The Pixel Aspect Ratio is defined to be the quotient of the pixel's
        // width over its height.  The value range in this field allows
        // specification of the widest pixel of 4:1 to the tallest pixel of
        // 1:4 in increments of 1/64th.
        //
        // Values :        0 -   No aspect ratio information is given.
        //            1..255 -   Value used in the computation.
        //
        // Aspect Ratio = (Pixel Aspect Ratio + 15) / 64
        byte ratio = 0;

        if (metadata.ResolutionUnits == PixelResolutionUnit.AspectRatio)
        {
            var hr = metadata.HorizontalResolution;
            var vr = metadata.VerticalResolution;
            if (hr != vr)
            {
                if (hr > vr)
                    ratio = (byte)(hr * 64 - 15);
                else
                    ratio = (byte)(1 / vr * 64 - 15);
            }
        }

        var descriptor = new GifLogicalScreenDescriptor(
            (ushort)width,
            (ushort)height,
            packedValue,
            unchecked((byte)transparencyIndex),
            ratio);

        descriptor.WriteTo(buffer);

        stream.Write(buffer, 0, GifLogicalScreenDescriptor.Size);
    }

    /// <summary>
    ///     Writes the application extension to the stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="repeatCount">The animated image repeat count.</param>
    private void WriteApplicationExtension(Stream stream, ushort repeatCount)
    {
        // Application Extension Header
        if (repeatCount != 1)
        {
            var loopingExtension = new GifNetscapeLoopingApplicationExtension(repeatCount);
            WriteExtension(loopingExtension, stream);
        }
    }

    /// <summary>
    ///     Writes the image comments to the stream.
    /// </summary>
    /// <param name="metadata">The metadata to be extract the comment data.</param>
    /// <param name="stream">The stream to write to.</param>
    private void WriteComments(GifMetadata metadata, Stream stream)
    {
        if (metadata.Comments.Count == 0) return;

        for (var i = 0; i < metadata.Comments.Count; i++)
        {
            var comment = metadata.Comments[i];
            buffer[0] = GifConstants.ExtensionIntroducer;
            buffer[1] = GifConstants.CommentLabel;
            stream.Write(buffer, 0, 2);

            // Comment will be stored in chunks of 255 bytes, if it exceeds this size.
            var commentSpan = comment.AsSpan();
            var idx = 0;
            for (;
                 idx <= comment.Length - GifConstants.MaxCommentSubBlockLength;
                 idx += GifConstants.MaxCommentSubBlockLength)
                WriteCommentSubBlock(stream, commentSpan, idx, GifConstants.MaxCommentSubBlockLength);

            // Write the length bytes, if any, to another sub block.
            if (idx < comment.Length)
            {
                var remaining = comment.Length - idx;
                WriteCommentSubBlock(stream, commentSpan, idx, remaining);
            }

            stream.WriteByte(GifConstants.Terminator);
        }
    }

    /// <summary>
    ///     Writes a comment sub-block to the stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="commentSpan">Comment as a Span.</param>
    /// <param name="idx">Current start index.</param>
    /// <param name="length">The length of the string to write. Should not exceed 255 bytes.</param>
    private static void WriteCommentSubBlock(Stream stream, ReadOnlySpan<char> commentSpan, int idx, int length)
    {
        var subComment = commentSpan.Slice(idx, length).ToString();
        var subCommentBytes = GifConstants.Encoding.GetBytes(subComment);
        stream.WriteByte((byte)length);
        stream.Write(subCommentBytes, 0, length);
    }

    /// <summary>
    ///     Writes the graphics control extension to the stream.
    /// </summary>
    /// <param name="metadata">The metadata of the image or frame.</param>
    /// <param name="transparencyIndex">The index of the color in the color palette to make transparent.</param>
    /// <param name="stream">The stream to write to.</param>
    private void WriteGraphicalControlExtension(GifFrameMetadata metadata, int transparencyIndex, Stream stream)
    {
        var packedValue = GifGraphicControlExtension.GetPackedValue(
            metadata.DisposalMethod,
            transparencyFlag: transparencyIndex > -1);

        var extension = new GifGraphicControlExtension(
            packedValue,
            (ushort)metadata.FrameDelay,
            unchecked((byte)transparencyIndex));

        WriteExtension(extension, stream);
    }

    /// <summary>
    ///     Writes the provided extension to the stream.
    /// </summary>
    /// <param name="extension">The extension to write to the stream.</param>
    /// <param name="stream">The stream to write to.</param>
    private void WriteExtension<TGifExtension>(TGifExtension extension, Stream stream)
        where TGifExtension : struct, IGifExtension
    {
        buffer[0] = GifConstants.ExtensionIntroducer;
        buffer[1] = extension.Label;

        var extensionSize = extension.WriteTo(buffer.AsSpan(2));

        buffer[extensionSize + 2] = GifConstants.Terminator;

        stream.Write(buffer, 0, extensionSize + 3);
    }

    /// <summary>
    ///     Writes the image descriptor to the stream.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="image">The <see cref="ImageFrame{TPixel}" /> to be encoded.</param>
    /// <param name="hasColorTable">Whether to use the global color table.</param>
    /// <param name="stream">The stream to write to.</param>
    private void WriteImageDescriptor<TPixel>(ImageFrame<TPixel> image, bool hasColorTable, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var packedValue = GifImageDescriptor.GetPackedValue(
            hasColorTable,
            false,
            false,
            bitDepth - 1);

        var descriptor = new GifImageDescriptor(
            0,
            0,
            (ushort)image.Width,
            (ushort)image.Height,
            packedValue);

        descriptor.WriteTo(buffer);

        stream.Write(buffer, 0, GifImageDescriptor.Size);
    }

    /// <summary>
    ///     Writes the color table to the stream.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="image">The <see cref="ImageFrame{TPixel}" /> to encode.</param>
    /// <param name="stream">The stream to write to.</param>
    private void WriteColorTable<TPixel>(IndexedImageFrame<TPixel> image, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        // The maximum number of colors for the bit depth
        var colorTableLength = ImageMaths.GetColorCountForBitDepth(bitDepth) * Unsafe.SizeOf<Rgb24>();

        using var colorTable = memoryAllocator.AllocateManagedByteBuffer(colorTableLength, AllocationOptions.Clean);
        PixelOperations<TPixel>.Instance.ToRgb24Bytes(
            configuration,
            image.Palette.Span,
            colorTable.GetSpan(),
            image.Palette.Length);

        stream.Write(colorTable.Array, 0, colorTableLength);
    }

    /// <summary>
    ///     Writes the image pixel data to the stream.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="image">The <see cref="IndexedImageFrame{TPixel}" /> containing indexed pixels.</param>
    /// <param name="stream">The stream to write to.</param>
    private void WriteImageData<TPixel>(IndexedImageFrame<TPixel> image, Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        using var encoder = new LzwEncoder(memoryAllocator, (byte)bitDepth);
        encoder.Encode(((IPixelSource)image).PixelBuffer, stream);
    }
}