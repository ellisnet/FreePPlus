// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.Metadata;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Formats.Gif;

//was previously: namespace SixLabors.ImageSharp.Formats.Gif;

/// <summary>
///     Performs the gif decoding operation.
/// </summary>
internal sealed class GifDecoderCore
{
    /// <summary>
    ///     The temp buffer used to reduce allocations.
    /// </summary>
    private readonly byte[] buffer = new byte[16];

    /// <summary>
    ///     The global configuration.
    /// </summary>
    private readonly Configuration configuration;

    /// <summary>
    ///     The gif specific metadata.
    /// </summary>
    private GifMetadata gifMetadata;

    /// <summary>
    ///     The global color table.
    /// </summary>
    private IManagedByteBuffer globalColorTable;

    /// <summary>
    ///     The graphics control extension.
    /// </summary>
    private GifGraphicControlExtension graphicsControlExtension;

    /// <summary>
    ///     The image descriptor.
    /// </summary>
    private GifImageDescriptor imageDescriptor;

    /// <summary>
    ///     The logical screen descriptor.
    /// </summary>
    private GifLogicalScreenDescriptor logicalScreenDescriptor;

    /// <summary>
    ///     The abstract metadata.
    /// </summary>
    private ImageMetadata metadata;

    /// <summary>
    ///     The area to restore.
    /// </summary>
    private Rectangle? restoreArea;

    /// <summary>
    ///     The currently loaded stream.
    /// </summary>
    private Stream stream;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GifDecoderCore" /> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="options">The decoder options.</param>
    public GifDecoderCore(Configuration configuration, IGifDecoderOptions options)
    {
        IgnoreMetadata = options.IgnoreMetadata;
        DecodingMode = options.DecodingMode;
        this.configuration = configuration ?? Configuration.Default;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the metadata should be ignored when the image is being decoded.
    /// </summary>
    public bool IgnoreMetadata { get; internal set; }

    /// <summary>
    ///     Gets the decoding mode for multi-frame images.
    /// </summary>
    public FrameDecodingMode DecodingMode { get; }

    /// <summary>
    ///     Gets the dimensions of the image.
    /// </summary>
    public Size Dimensions => new(imageDescriptor.Width, imageDescriptor.Height);

    private MemoryAllocator MemoryAllocator => configuration.MemoryAllocator;

    /// <summary>
    ///     Decodes the stream to the image.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="stream">The stream containing image data.</param>
    /// <returns>The decoded image</returns>
    public Image<TPixel> Decode<TPixel>(Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Image<TPixel> image = null;
        ImageFrame<TPixel> previousFrame = null;
        try
        {
            ReadLogicalScreenDescriptorAndGlobalColorTable(stream);

            // Loop though the respective gif parts and read the data.
            var nextFlag = stream.ReadByte();
            while (nextFlag != GifConstants.Terminator)
            {
                if (nextFlag == GifConstants.ImageLabel)
                {
                    if (previousFrame != null && DecodingMode == FrameDecodingMode.First) break;

                    ReadFrame(ref image, ref previousFrame);
                }
                else if (nextFlag == GifConstants.ExtensionIntroducer)
                {
                    switch (stream.ReadByte())
                    {
                        case GifConstants.GraphicControlLabel:
                            ReadGraphicalControlExtension();
                            break;
                        case GifConstants.CommentLabel:
                            ReadComments();
                            break;
                        case GifConstants.ApplicationExtensionLabel:
                            ReadApplicationExtension();
                            break;
                        case GifConstants.PlainTextLabel:
                            SkipBlock(); // Not supported by any known decoder.
                            break;
                    }
                }
                else if (nextFlag == GifConstants.EndIntroducer)
                {
                    break;
                }

                nextFlag = stream.ReadByte();
                if (nextFlag == -1) break;
            }
        }
        finally
        {
            globalColorTable?.Dispose();
        }

        return image;
    }

    /// <summary>
    ///     Reads the raw image information from the specified stream.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> containing image data.</param>
    public IImageInfo Identify(Stream stream)
    {
        try
        {
            ReadLogicalScreenDescriptorAndGlobalColorTable(stream);

            // Loop though the respective gif parts and read the data.
            var nextFlag = stream.ReadByte();
            while (nextFlag != GifConstants.Terminator)
            {
                if (nextFlag == GifConstants.ImageLabel)
                    ReadImageDescriptor();
                else if (nextFlag == GifConstants.ExtensionIntroducer)
                    switch (stream.ReadByte())
                    {
                        case GifConstants.GraphicControlLabel:
                            SkipBlock(); // Skip graphic control extension block
                            break;
                        case GifConstants.CommentLabel:
                            ReadComments();
                            break;
                        case GifConstants.ApplicationExtensionLabel:
                            ReadApplicationExtension();
                            break;
                        case GifConstants.PlainTextLabel:
                            SkipBlock(); // Not supported by any known decoder.
                            break;
                    }
                else if (nextFlag == GifConstants.EndIntroducer) break;

                nextFlag = stream.ReadByte();
                if (nextFlag == -1) break;
            }
        }
        finally
        {
            globalColorTable?.Dispose();
        }

        return new ImageInfo(
            new PixelTypeInfo(logicalScreenDescriptor.BitsPerPixel),
            logicalScreenDescriptor.Width,
            logicalScreenDescriptor.Height,
            metadata,
            GifFormat.Instance);
    }

    /// <summary>
    ///     Reads the graphic control extension.
    /// </summary>
    private void ReadGraphicalControlExtension()
    {
        stream.Read(buffer, 0, 6);

        graphicsControlExtension = GifGraphicControlExtension.Parse(buffer);
    }

    /// <summary>
    ///     Reads the image descriptor.
    /// </summary>
    private void ReadImageDescriptor()
    {
        stream.Read(buffer, 0, 9);

        imageDescriptor = GifImageDescriptor.Parse(buffer);
        if (imageDescriptor.Height == 0 || imageDescriptor.Width == 0)
            GifThrowHelper.ThrowInvalidImageContentException("Width or height should not be 0");
    }

    /// <summary>
    ///     Reads the logical screen descriptor.
    /// </summary>
    private void ReadLogicalScreenDescriptor()
    {
        stream.Read(buffer, 0, 7);

        logicalScreenDescriptor = GifLogicalScreenDescriptor.Parse(buffer);
    }

    /// <summary>
    ///     Reads the application extension block parsing any animation information
    ///     if present.
    /// </summary>
    private void ReadApplicationExtension()
    {
        var appLength = stream.ReadByte();

        // If the length is 11 then it's a valid extension and most likely
        // a NETSCAPE or ANIMEXTS extension. We want the loop count from this.
        if (appLength == GifConstants.ApplicationBlockSize)
        {
            stream.Skip(appLength);
            var subBlockSize = stream.ReadByte();

            // TODO: There's also a NETSCAPE buffer extension.
            // http://www.vurdalakov.net/misc/gif/netscape-buffering-application-extension
            if (subBlockSize == GifConstants.NetscapeLoopingSubBlockSize)
            {
                stream.Read(buffer, 0, GifConstants.NetscapeLoopingSubBlockSize);
                gifMetadata.RepeatCount = GifNetscapeLoopingApplicationExtension.Parse(buffer.AsSpan(1)).RepeatCount;
                stream.Skip(1); // Skip the terminator.
                return;
            }

            // Could be XMP or something else not supported yet.
            // Skip the subblock and terminator.
            SkipBlock(subBlockSize);
            return;
        }

        SkipBlock(appLength); // Not supported by any known decoder.
    }

    /// <summary>
    ///     Skips over a block or reads its terminator.
    ///     <param name="blockSize">The length of the block to skip.</param>
    /// </summary>
    private void SkipBlock(int blockSize = 0)
    {
        if (blockSize > 0) stream.Skip(blockSize);

        int flag;

        while ((flag = stream.ReadByte()) > 0) stream.Skip(flag);
    }

    /// <summary>
    ///     Reads the gif comments.
    /// </summary>
    private void ReadComments()
    {
        int length;

        var stringBuilder = new StringBuilder();
        while ((length = stream.ReadByte()) != 0)
        {
            if (length > GifConstants.MaxCommentSubBlockLength)
                GifThrowHelper.ThrowInvalidImageContentException(
                    $"Gif comment length '{length}' exceeds max '{GifConstants.MaxCommentSubBlockLength}' of a comment data block");

            if (IgnoreMetadata)
            {
                stream.Seek(length, SeekOrigin.Current);
                continue;
            }

            using (var commentsBuffer = MemoryAllocator.AllocateManagedByteBuffer(length))
            {
                stream.Read(commentsBuffer.Array, 0, length);
                var commentPart = GifConstants.Encoding.GetString(commentsBuffer.Array, 0, length);
                stringBuilder.Append(commentPart);
            }
        }

        if (stringBuilder.Length > 0) gifMetadata.Comments.Add(stringBuilder.ToString());
    }

    /// <summary>
    ///     Reads an individual gif frame.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="image">The image to decode the information to.</param>
    /// <param name="previousFrame">The previous frame.</param>
    private void ReadFrame<TPixel>(ref Image<TPixel> image, ref ImageFrame<TPixel> previousFrame)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        ReadImageDescriptor();

        IManagedByteBuffer localColorTable = null;
        Buffer2D<byte> indices = null;
        try
        {
            // Determine the color table for this frame. If there is a local one, use it otherwise use the global color table.
            if (imageDescriptor.LocalColorTableFlag)
            {
                var length = imageDescriptor.LocalColorTableSize * 3;
                localColorTable =
                    configuration.MemoryAllocator.AllocateManagedByteBuffer(length, AllocationOptions.Clean);
                stream.Read(localColorTable.Array, 0, length);
            }

            indices = configuration.MemoryAllocator.Allocate2D<byte>(imageDescriptor.Width, imageDescriptor.Height,
                AllocationOptions.Clean);

            ReadFrameIndices(indices);
            ReadOnlySpan<Rgb24> colorTable =
                MemoryMarshal.Cast<byte, Rgb24>((localColorTable ?? globalColorTable).GetSpan());
            ReadFrameColors(ref image, ref previousFrame, indices, colorTable, imageDescriptor);

            // Skip any remaining blocks
            SkipBlock();
        }
        finally
        {
            localColorTable?.Dispose();
            indices?.Dispose();
        }
    }

    /// <summary>
    ///     Reads the frame indices marking the color to use for each pixel.
    /// </summary>
    /// <param name="indices">The 2D pixel buffer to write to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadFrameIndices(Buffer2D<byte> indices)
    {
        var dataSize = stream.ReadByte();
        using var lzwDecoder = new LzwDecoder(configuration.MemoryAllocator, stream);
        lzwDecoder.DecodePixels(dataSize, indices);
    }

    /// <summary>
    ///     Reads the frames colors, mapping indices to colors.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="image">The image to decode the information to.</param>
    /// <param name="previousFrame">The previous frame.</param>
    /// <param name="indices">The indexed pixels.</param>
    /// <param name="colorTable">The color table containing the available colors.</param>
    /// <param name="descriptor">The <see cref="GifImageDescriptor" /></param>
    private void ReadFrameColors<TPixel>(ref Image<TPixel> image, ref ImageFrame<TPixel> previousFrame,
        Buffer2D<byte> indices, ReadOnlySpan<Rgb24> colorTable, in GifImageDescriptor descriptor)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        int imageWidth = logicalScreenDescriptor.Width;
        int imageHeight = logicalScreenDescriptor.Height;

        ImageFrame<TPixel> prevFrame = null;
        ImageFrame<TPixel> currentFrame = null;
        ImageFrame<TPixel> imageFrame;

        if (previousFrame is null)
        {
            // This initializes the image to become fully transparent because the alpha channel is zero.
            image = new Image<TPixel>(configuration, imageWidth, imageHeight, metadata, GifFormat.Instance);

            SetFrameMetadata(image.Frames.RootFrame.Metadata);

            imageFrame = image.Frames.RootFrame;
        }
        else
        {
            if (graphicsControlExtension.DisposalMethod == GifDisposalMethod.RestoreToPrevious)
                prevFrame = previousFrame;

            currentFrame = image.Frames.AddFrame(previousFrame); // This clones the frame and adds it the collection

            SetFrameMetadata(currentFrame.Metadata);

            imageFrame = currentFrame;

            RestoreToBackground(imageFrame);
        }

        var interlacePass = 0; // The interlace pass
        var interlaceIncrement = 8; // The interlacing line increment
        var interlaceY = 0; // The current interlaced line
        int descriptorTop = descriptor.Top;
        var descriptorBottom = descriptorTop + descriptor.Height;
        int descriptorLeft = descriptor.Left;
        var descriptorRight = descriptorLeft + descriptor.Width;
        var transFlag = graphicsControlExtension.TransparencyFlag;
        var transIndex = graphicsControlExtension.TransparencyIndex;

        for (var y = descriptorTop; y < descriptorBottom && y < imageHeight; y++)
        {
            ref var indicesRowRef = ref MemoryMarshal.GetReference(indices.GetRowSpan(y - descriptorTop));

            // Check if this image is interlaced.
            int writeY; // the target y offset to write to
            if (descriptor.InterlaceFlag)
            {
                // If so then we read lines at predetermined offsets.
                // When an entire image height worth of offset lines has been read we consider this a pass.
                // With each pass the number of offset lines changes and the starting line changes.
                if (interlaceY >= descriptor.Height)
                {
                    interlacePass++;
                    switch (interlacePass)
                    {
                        case 1:
                            interlaceY = 4;
                            break;
                        case 2:
                            interlaceY = 2;
                            interlaceIncrement = 4;
                            break;
                        case 3:
                            interlaceY = 1;
                            interlaceIncrement = 2;
                            break;
                    }
                }

                writeY = interlaceY + descriptor.Top;
                interlaceY += interlaceIncrement;
            }
            else
            {
                writeY = y;
            }

            ref var rowRef = ref MemoryMarshal.GetReference(imageFrame.GetPixelRowSpan(writeY));

            if (!transFlag)
                // #403 The left + width value can be larger than the image width
                for (var x = descriptorLeft; x < descriptorRight && x < imageWidth; x++)
                {
                    int index = Unsafe.Add(ref indicesRowRef, x - descriptorLeft);
                    ref var pixel = ref Unsafe.Add(ref rowRef, x);
                    var rgb = colorTable[index];
                    pixel.FromRgb24(rgb);
                }
            else
                for (var x = descriptorLeft; x < descriptorRight && x < imageWidth; x++)
                {
                    int index = Unsafe.Add(ref indicesRowRef, x - descriptorLeft);
                    if (transIndex != index)
                    {
                        ref var pixel = ref Unsafe.Add(ref rowRef, x);
                        var rgb = colorTable[index];
                        pixel.FromRgb24(rgb);
                    }
                }
        }

        if (prevFrame != null)
        {
            previousFrame = prevFrame;
            return;
        }

        previousFrame = currentFrame ?? image.Frames.RootFrame;

        if (graphicsControlExtension.DisposalMethod == GifDisposalMethod.RestoreToBackground)
            restoreArea = new Rectangle(descriptor.Left, descriptor.Top, descriptor.Width, descriptor.Height);
    }

    /// <summary>
    ///     Restores the current frame area to the background.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="frame">The frame.</param>
    private void RestoreToBackground<TPixel>(ImageFrame<TPixel> frame)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        if (restoreArea is null) return;

        var pixelRegion = frame.PixelBuffer.GetRegion(restoreArea.Value);
        pixelRegion.Clear();

        restoreArea = null;
    }

    /// <summary>
    ///     Sets the frames metadata.
    /// </summary>
    /// <param name="meta">The metadata.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetFrameMetadata(ImageFrameMetadata meta)
    {
        var gifMeta = meta.GetGifMetadata();
        if (graphicsControlExtension.DelayTime > 0) gifMeta.FrameDelay = graphicsControlExtension.DelayTime;

        // Frames can either use the global table or their own local table.
        if (logicalScreenDescriptor.GlobalColorTableFlag
            && logicalScreenDescriptor.GlobalColorTableSize > 0)
            gifMeta.ColorTableLength = logicalScreenDescriptor.GlobalColorTableSize;
        else if (imageDescriptor.LocalColorTableFlag
                 && imageDescriptor.LocalColorTableSize > 0)
            gifMeta.ColorTableLength = imageDescriptor.LocalColorTableSize;

        gifMeta.DisposalMethod = graphicsControlExtension.DisposalMethod;
    }

    /// <summary>
    ///     Reads the logical screen descriptor and global color table blocks
    /// </summary>
    /// <param name="stream">The stream containing image data. </param>
    private void ReadLogicalScreenDescriptorAndGlobalColorTable(Stream stream)
    {
        this.stream = stream;

        // Skip the identifier
        this.stream.Skip(6);
        ReadLogicalScreenDescriptor();

        var meta = new ImageMetadata();

        // The Pixel Aspect Ratio is defined to be the quotient of the pixel's
        // width over its height.  The value range in this field allows
        // specification of the widest pixel of 4:1 to the tallest pixel of
        // 1:4 in increments of 1/64th.
        //
        // Values :        0 -   No aspect ratio information is given.
        //            1..255 -   Value used in the computation.
        //
        // Aspect Ratio = (Pixel Aspect Ratio + 15) / 64
        if (logicalScreenDescriptor.PixelAspectRatio > 0)
        {
            meta.ResolutionUnits = PixelResolutionUnit.AspectRatio;
            var ratio = (logicalScreenDescriptor.PixelAspectRatio + 15) / 64F;

            if (ratio > 1)
            {
                meta.HorizontalResolution = ratio;
                meta.VerticalResolution = 1;
            }
            else
            {
                meta.VerticalResolution = 1 / ratio;
                meta.HorizontalResolution = 1;
            }
        }

        metadata = meta;
        gifMetadata = meta.GetGifMetadata();
        gifMetadata.ColorTableMode = logicalScreenDescriptor.GlobalColorTableFlag
            ? GifColorTableMode.Global
            : GifColorTableMode.Local;

        if (logicalScreenDescriptor.GlobalColorTableFlag)
        {
            var globalColorTableLength = logicalScreenDescriptor.GlobalColorTableSize * 3;
            gifMetadata.GlobalColorTableLength = globalColorTableLength;

            globalColorTable =
                MemoryAllocator.AllocateManagedByteBuffer(globalColorTableLength, AllocationOptions.Clean);

            // Read the global color table data from the stream
            stream.Read(globalColorTable.Array, 0, globalColorTableLength);
        }
    }
}