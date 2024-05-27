// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Common.Helpers;
using FreePPlus.Imaging.Formats.Jpeg.Components;
using FreePPlus.Imaging.Formats.Jpeg.Components.Decoder;
using FreePPlus.Imaging.IO;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.Metadata;
using FreePPlus.Imaging.Metadata.Profiles.Exif;
using FreePPlus.Imaging.Metadata.Profiles.Icc;
using FreePPlus.Imaging.Metadata.Profiles.Iptc;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Formats.Jpeg;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg;

/// <summary>
///     Performs the jpeg decoding operation.
///     Originally ported from <see href="https://github.com/mozilla/pdf.js/blob/master/src/core/jpg.js" />
///     with additional fixes for both performance and common encoding errors.
/// </summary>
internal sealed class JpegDecoderCore : IRawJpegData
{
    /// <summary>
    ///     The global configuration
    /// </summary>
    private readonly Configuration configuration;

    /// <summary>
    ///     The buffer used to read markers from the stream.
    /// </summary>
    private readonly byte[] markerBuffer = new byte[2];

    /// <summary>
    ///     The only supported precision
    /// </summary>
    private readonly int[] supportedPrecisions = { 8, 12 };

    /// <summary>
    ///     The buffer used to temporarily store bytes read from the stream.
    /// </summary>
    private readonly byte[] temp = new byte[2 * 16 * 4];

    /// <summary>
    ///     The AC Huffman tables
    /// </summary>
    private HuffmanTable[] acHuffmanTables;

    /// <summary>
    ///     Contains information about the Adobe marker.
    /// </summary>
    private AdobeMarker adobe;

    /// <summary>
    ///     The DC Huffman tables.
    /// </summary>
    private HuffmanTable[] dcHuffmanTables;

    /// <summary>
    ///     Contains exif data.
    /// </summary>
    private byte[] exifData;

    /// <summary>
    ///     Contains ICC data.
    /// </summary>
    private byte[] iccData;

    /// <summary>
    ///     Contains IPTC data.
    /// </summary>
    private byte[] iptcData;

    /// <summary>
    ///     Whether the image has an EXIF marker.
    /// </summary>
    private bool isExif;

    /// <summary>
    ///     Whether the image has an ICC marker.
    /// </summary>
    private bool isIcc;

    /// <summary>
    ///     Whether the image has a IPTC data.
    /// </summary>
    private bool isIptc;

    /// <summary>
    ///     Contains information about the JFIF marker.
    /// </summary>
    private JFifMarker jFif;

    /// <summary>
    ///     The reset interval determined by RST markers.
    /// </summary>
    private ushort resetInterval;

    /// <summary>
    ///     Initializes a new instance of the <see cref="JpegDecoderCore" /> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="options">The options.</param>
    public JpegDecoderCore(Configuration configuration, IJpegDecoderOptions options)
    {
        this.configuration = configuration ?? Configuration.Default;
        IgnoreMetadata = options.IgnoreMetadata;
    }

    /// <summary>
    ///     Gets the frame
    /// </summary>
    public JpegFrame Frame { get; private set; }

    /// <summary>
    ///     Gets the number of MCU blocks in the image as <see cref="Size" />.
    /// </summary>
    public Size ImageSizeInMCU { get; private set; }

    /// <summary>
    ///     Gets the image width
    /// </summary>
    public int ImageWidth => ImageSizeInPixels.Width;

    /// <summary>
    ///     Gets the image height
    /// </summary>
    public int ImageHeight => ImageSizeInPixels.Height;

    /// <summary>
    ///     Gets the color depth, in number of bits per pixel.
    /// </summary>
    public int BitsPerPixel => ComponentCount * Frame.Precision;

    /// <summary>
    ///     Gets the input stream.
    /// </summary>
    public DoubleBufferedStreamReader InputStream { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether the metadata should be ignored when the image is being decoded.
    /// </summary>
    public bool IgnoreMetadata { get; }

    /// <summary>
    ///     Gets the <see cref="ImageMetadata" /> decoded by this decoder instance.
    /// </summary>
    public ImageMetadata Metadata { get; private set; }

    /// <summary>
    ///     Gets the components.
    /// </summary>
    public JpegComponent[] Components => Frame.Components;

    /// <inheritdoc />
    public Size ImageSizeInPixels { get; private set; }

    /// <inheritdoc />
    public int ComponentCount { get; private set; }

    /// <inheritdoc />
    public JpegColorSpace ColorSpace { get; private set; }

    /// <inheritdoc />
    public int Precision { get; private set; }

    /// <inheritdoc />
    IJpegComponent[] IRawJpegData.Components => Components;

    /// <inheritdoc />
    public Block8x8F[] QuantizationTables { get; private set; }

    /// <inheritdoc />
    public void Dispose()
    {
        InputStream?.Dispose();
        Frame?.Dispose();

        // Set large fields to null.
        InputStream = null;
        Frame = null;
        dcHuffmanTables = null;
        acHuffmanTables = null;
    }

    /// <summary>
    ///     Finds the next file marker within the byte stream.
    /// </summary>
    /// <param name="marker">The buffer to read file markers to</param>
    /// <param name="stream">The input stream</param>
    /// <returns>The <see cref="JpegFileMarker" /></returns>
    public static JpegFileMarker FindNextFileMarker(byte[] marker, DoubleBufferedStreamReader stream)
    {
        var value = stream.Read(marker, 0, 2);

        if (value == 0) return new JpegFileMarker(JpegConstants.Markers.EOI, stream.Length - 2);

        if (marker[0] == JpegConstants.Markers.XFF)
        {
            // According to Section B.1.1.2:
            // "Any marker may optionally be preceded by any number of fill bytes, which are bytes assigned code 0xFF."
            int m = marker[1];
            while (m == JpegConstants.Markers.XFF)
            {
                var suffix = stream.ReadByte();
                if (suffix == -1) return new JpegFileMarker(JpegConstants.Markers.EOI, stream.Length - 2);

                m = suffix;
            }

            return new JpegFileMarker((byte)m, stream.Position - 2);
        }

        return new JpegFileMarker(marker[1], stream.Position - 2, true);
    }

    /// <summary>
    ///     Decodes the image from the specified <see cref="Stream" />  and sets the data to image.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="stream">The stream, where the image should be.</param>
    /// <returns>The decoded image.</returns>
    public Image<TPixel> Decode<TPixel>(Stream stream)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        ParseStream(stream);
        InitExifProfile();
        InitIccProfile();
        InitIptcProfile();
        InitDerivedMetadataProperties();
        return PostProcessIntoImage<TPixel>();
    }

    /// <summary>
    ///     Reads the raw image information from the specified stream.
    /// </summary>
    /// <param name="stream">The <see cref="Stream" /> containing image data.</param>
    public IImageInfo Identify(Stream stream)
    {
        ParseStream(stream, true);
        InitExifProfile();
        InitIccProfile();
        InitIptcProfile();
        InitDerivedMetadataProperties();

        return new ImageInfo(new PixelTypeInfo(BitsPerPixel), ImageWidth, ImageHeight,
            Metadata, JpegFormat.Instance);
    }

    /// <summary>
    ///     Parses the input stream for file markers
    /// </summary>
    /// <param name="stream">The input stream</param>
    /// <param name="metadataOnly">Whether to decode metadata only.</param>
    public void ParseStream(Stream stream, bool metadataOnly = false)
    {
        Metadata = new ImageMetadata();
        InputStream = new DoubleBufferedStreamReader(configuration.MemoryAllocator, stream);

        // Check for the Start Of Image marker.
        InputStream.Read(markerBuffer, 0, 2);
        var fileMarker = new JpegFileMarker(markerBuffer[1], 0);
        if (fileMarker.Marker != JpegConstants.Markers.SOI)
            JpegThrowHelper.ThrowInvalidImageContentException("Missing SOI marker.");

        InputStream.Read(markerBuffer, 0, 2);
        var marker = markerBuffer[1];
        fileMarker = new JpegFileMarker(marker, (int)InputStream.Position - 2);
        QuantizationTables = new Block8x8F[4];

        // Only assign what we need
        if (!metadataOnly)
        {
            const int maxTables = 4;
            dcHuffmanTables = new HuffmanTable[maxTables];
            acHuffmanTables = new HuffmanTable[maxTables];
        }

        // Break only when we discover a valid EOI marker.
        // https://github.com/SixLabors/ImageSharp/issues/695
        while (fileMarker.Marker != JpegConstants.Markers.EOI
               || (fileMarker.Marker == JpegConstants.Markers.EOI && fileMarker.Invalid))
        {
            if (!fileMarker.Invalid)
            {
                // Get the marker length
                var remaining = ReadUint16() - 2;

                switch (fileMarker.Marker)
                {
                    case JpegConstants.Markers.SOF0:
                    case JpegConstants.Markers.SOF1:
                    case JpegConstants.Markers.SOF2:
                        ProcessStartOfFrameMarker(remaining, fileMarker, metadataOnly);
                        break;

                    case JpegConstants.Markers.SOS:
                        if (!metadataOnly)
                        {
                            ProcessStartOfScanMarker();
                            break;
                        }

                        // It's highly unlikely that APPn related data will be found after the SOS marker
                        // We should have gathered everything we need by now.
                        return;

                    case JpegConstants.Markers.DHT:

                        if (metadataOnly)
                            InputStream.Skip(remaining);
                        else
                            ProcessDefineHuffmanTablesMarker(remaining);

                        break;

                    case JpegConstants.Markers.DQT:
                        ProcessDefineQuantizationTablesMarker(remaining);
                        break;

                    case JpegConstants.Markers.DRI:
                        if (metadataOnly)
                            InputStream.Skip(remaining);
                        else
                            ProcessDefineRestartIntervalMarker(remaining);

                        break;

                    case JpegConstants.Markers.APP0:
                        ProcessApplicationHeaderMarker(remaining);
                        break;

                    case JpegConstants.Markers.APP1:
                        ProcessApp1Marker(remaining);
                        break;

                    case JpegConstants.Markers.APP2:
                        ProcessApp2Marker(remaining);
                        break;

                    case JpegConstants.Markers.APP3:
                    case JpegConstants.Markers.APP4:
                    case JpegConstants.Markers.APP5:
                    case JpegConstants.Markers.APP6:
                    case JpegConstants.Markers.APP7:
                    case JpegConstants.Markers.APP8:
                    case JpegConstants.Markers.APP9:
                    case JpegConstants.Markers.APP10:
                    case JpegConstants.Markers.APP11:
                    case JpegConstants.Markers.APP12:
                        InputStream.Skip(remaining);
                        break;

                    case JpegConstants.Markers.APP13:
                        ProcessApp13Marker(remaining);
                        break;

                    case JpegConstants.Markers.APP14:
                        ProcessApp14Marker(remaining);
                        break;

                    case JpegConstants.Markers.APP15:
                    case JpegConstants.Markers.COM:
                        InputStream.Skip(remaining);
                        break;
                }
            }

            // Read on.
            fileMarker = FindNextFileMarker(markerBuffer, InputStream);
        }
    }

    /// <summary>
    ///     Returns the correct colorspace based on the image component count
    /// </summary>
    /// <returns>The <see cref="JpegColorSpace" /></returns>
    private JpegColorSpace DeduceJpegColorSpace()
    {
        if (ComponentCount == 1) return JpegColorSpace.Grayscale;

        if (ComponentCount == 3)
        {
            if (!adobe.Equals(default) && adobe.ColorTransform == JpegConstants.Adobe.ColorTransformUnknown)
                return JpegColorSpace.RGB;

            // Some images are poorly encoded and contain incorrect colorspace transform metadata.
            // We ignore that and always fall back to the default colorspace.
            return JpegColorSpace.YCbCr;
        }

        if (ComponentCount == 4)
            return adobe.ColorTransform == JpegConstants.Adobe.ColorTransformYcck
                ? JpegColorSpace.Ycck
                : JpegColorSpace.Cmyk;

        JpegThrowHelper.ThrowInvalidImageContentException(
            $"Unsupported color mode. Supported component counts 1, 3, and 4; found {ComponentCount}");
        return default;
    }

    /// <summary>
    ///     Initializes the EXIF profile.
    /// </summary>
    private void InitExifProfile()
    {
        if (isExif) Metadata.ExifProfile = new ExifProfile(exifData);
    }

    /// <summary>
    ///     Initializes the ICC profile.
    /// </summary>
    private void InitIccProfile()
    {
        if (isIcc)
        {
            var profile = new IccProfile(iccData);
            if (profile.CheckIsValid()) Metadata.IccProfile = profile;
        }
    }

    /// <summary>
    ///     Initializes the IPTC profile.
    /// </summary>
    private void InitIptcProfile()
    {
        if (isIptc)
        {
            var profile = new IptcProfile(iptcData);
            Metadata.IptcProfile = profile;
        }
    }

    /// <summary>
    ///     Assigns derived metadata properties to <see cref="Metadata" />, eg. horizontal and vertical resolution if it has a
    ///     JFIF header.
    /// </summary>
    private void InitDerivedMetadataProperties()
    {
        if (jFif.XDensity > 0 && jFif.YDensity > 0)
        {
            Metadata.HorizontalResolution = jFif.XDensity;
            Metadata.VerticalResolution = jFif.YDensity;
            Metadata.ResolutionUnits = jFif.DensityUnits;
        }
        else if (isExif)
        {
            var horizontalValue = GetExifResolutionValue(ExifTag.XResolution);
            var verticalValue = GetExifResolutionValue(ExifTag.YResolution);

            if (horizontalValue > 0 && verticalValue > 0)
            {
                Metadata.HorizontalResolution = horizontalValue;
                Metadata.VerticalResolution = verticalValue;
                Metadata.ResolutionUnits = UnitConverter.ExifProfileToResolutionUnit(Metadata.ExifProfile);
            }
        }
    }

    private double GetExifResolutionValue(ExifTag<Rational> tag)
    {
        var resolution = Metadata.ExifProfile.GetValue(tag);

        return resolution is null ? 0 : resolution.Value.ToDouble();
    }

    /// <summary>
    ///     Extends the profile with additional data.
    /// </summary>
    /// <param name="profile">The profile data array.</param>
    /// <param name="extension">The array containing addition profile data.</param>
    private void ExtendProfile(ref byte[] profile, byte[] extension)
    {
        var currentLength = profile.Length;

        Array.Resize(ref profile, currentLength + extension.Length);
        Buffer.BlockCopy(extension, 0, profile, currentLength, extension.Length);
    }

    /// <summary>
    ///     Processes the application header containing the JFIF identifier plus extra data.
    /// </summary>
    /// <param name="remaining">The remaining bytes in the segment block.</param>
    private void ProcessApplicationHeaderMarker(int remaining)
    {
        // We can only decode JFif identifiers.
        if (remaining < JFifMarker.Length)
        {
            // Skip the application header length
            InputStream.Skip(remaining);
            return;
        }

        InputStream.Read(temp, 0, JFifMarker.Length);
        remaining -= JFifMarker.Length;

        JFifMarker.TryParse(temp, out jFif);

        // TODO: thumbnail
        if (remaining > 0) InputStream.Skip(remaining);
    }

    /// <summary>
    ///     Processes the App1 marker retrieving any stored metadata
    /// </summary>
    /// <param name="remaining">The remaining bytes in the segment block.</param>
    private void ProcessApp1Marker(int remaining)
    {
        const int Exif00 = 6;
        if (remaining < Exif00 || IgnoreMetadata)
        {
            // Skip the application header length
            InputStream.Skip(remaining);
            return;
        }

        var profile = new byte[remaining];
        InputStream.Read(profile, 0, remaining);

        if (ProfileResolver.IsProfile(profile, ProfileResolver.ExifMarker))
        {
            isExif = true;
            if (exifData is null)
                // The first 6 bytes (Exif00) will be skipped, because this is Jpeg specific
                exifData = profile.AsSpan(Exif00).ToArray();
            else
                // If the EXIF information exceeds 64K, it will be split over multiple APP1 markers
                ExtendProfile(ref exifData, profile.AsSpan(Exif00).ToArray());
        }
    }

    /// <summary>
    ///     Processes the App2 marker retrieving any stored ICC profile information
    /// </summary>
    /// <param name="remaining">The remaining bytes in the segment block.</param>
    private void ProcessApp2Marker(int remaining)
    {
        // Length is 14 though we only need to check 12.
        const int Icclength = 14;
        if (remaining < Icclength || IgnoreMetadata)
        {
            InputStream.Skip(remaining);
            return;
        }

        var identifier = new byte[Icclength];
        InputStream.Read(identifier, 0, Icclength);
        remaining -= Icclength; // We have read it by this point

        if (ProfileResolver.IsProfile(identifier, ProfileResolver.IccMarker))
        {
            isIcc = true;
            var profile = new byte[remaining];
            InputStream.Read(profile, 0, remaining);

            if (iccData is null)
                iccData = profile;
            else
                // If the ICC information exceeds 64K, it will be split over multiple APP2 markers
                ExtendProfile(ref iccData, profile);
        }
        else
        {
            // Not an ICC profile we can handle. Skip the remaining bytes so we can carry on and ignore this.
            InputStream.Skip(remaining);
        }
    }

    /// <summary>
    ///     Processes a App13 marker, which contains IPTC data stored with Adobe Photoshop.
    ///     The content of an APP13 segment is formed by an identifier string followed by a sequence of resource data blocks.
    /// </summary>
    /// <param name="remaining">The remaining bytes in the segment block.</param>
    private void ProcessApp13Marker(int remaining)
    {
        if (remaining < ProfileResolver.AdobePhotoshopApp13Marker.Length || IgnoreMetadata)
        {
            InputStream.Skip(remaining);
            return;
        }

        InputStream.Read(temp, 0, ProfileResolver.AdobePhotoshopApp13Marker.Length);
        remaining -= ProfileResolver.AdobePhotoshopApp13Marker.Length;
        if (ProfileResolver.IsProfile(temp, ProfileResolver.AdobePhotoshopApp13Marker))
        {
            var resourceBlockData = new byte[remaining];
            InputStream.Read(resourceBlockData, 0, remaining);
            var blockDataSpan = resourceBlockData.AsSpan();

            while (blockDataSpan.Length > 12)
            {
                if (!ProfileResolver.IsProfile(blockDataSpan.Slice(0, 4),
                        ProfileResolver.AdobeImageResourceBlockMarker)) return;

                blockDataSpan = blockDataSpan.Slice(4);
                var imageResourceBlockId = blockDataSpan.Slice(0, 2);
                if (ProfileResolver.IsProfile(imageResourceBlockId, ProfileResolver.AdobeIptcMarker))
                {
                    var resourceBlockNameLength = ReadImageResourceNameLength(blockDataSpan);
                    var resourceDataSize = ReadResourceDataLength(blockDataSpan, resourceBlockNameLength);
                    var dataStartIdx = 2 + resourceBlockNameLength + 4;
                    if (resourceDataSize > 0 && blockDataSpan.Length >= dataStartIdx + resourceDataSize)
                    {
                        isIptc = true;
                        iptcData = blockDataSpan.Slice(dataStartIdx, resourceDataSize).ToArray();
                        break;
                    }
                }
                else
                {
                    var resourceBlockNameLength = ReadImageResourceNameLength(blockDataSpan);
                    var resourceDataSize = ReadResourceDataLength(blockDataSpan, resourceBlockNameLength);
                    var dataStartIdx = 2 + resourceBlockNameLength + 4;
                    if (blockDataSpan.Length < dataStartIdx + resourceDataSize)
                        // Not enough data or the resource data size is wrong.
                        break;

                    blockDataSpan = blockDataSpan.Slice(dataStartIdx + resourceDataSize);
                }
            }
        }
    }

    /// <summary>
    ///     Reads the adobe image resource block name: a Pascal string (padded to make size even).
    /// </summary>
    /// <param name="blockDataSpan">The span holding the block resource data.</param>
    /// <returns>The length of the name.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    private static int ReadImageResourceNameLength(Span<byte> blockDataSpan)
    {
        var nameLength = blockDataSpan[2];
        var nameDataSize = nameLength == 0 ? 2 : nameLength;
        if (nameDataSize % 2 != 0) nameDataSize++;

        return nameDataSize;
    }

    /// <summary>
    ///     Reads the length of a adobe image resource data block.
    /// </summary>
    /// <param name="blockDataSpan">The span holding the block resource data.</param>
    /// <param name="resourceBlockNameLength">The length of the block name.</param>
    /// <returns>The block length.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    private static int ReadResourceDataLength(Span<byte> blockDataSpan, int resourceBlockNameLength)
    {
        return BinaryPrimitives.ReadInt32BigEndian(blockDataSpan.Slice(2 + resourceBlockNameLength, 4));
    }

    /// <summary>
    ///     Processes the application header containing the Adobe identifier
    ///     which stores image encoding information for DCT filters.
    /// </summary>
    /// <param name="remaining">The remaining bytes in the segment block.</param>
    private void ProcessApp14Marker(int remaining)
    {
        const int MarkerLength = AdobeMarker.Length;
        if (remaining < MarkerLength)
        {
            // Skip the application header length
            InputStream.Skip(remaining);
            return;
        }

        InputStream.Read(temp, 0, MarkerLength);
        remaining -= MarkerLength;

        AdobeMarker.TryParse(temp, out adobe);

        if (remaining > 0) InputStream.Skip(remaining);
    }

    /// <summary>
    ///     Processes the Define Quantization Marker and tables. Specified in section B.2.4.1.
    /// </summary>
    /// <param name="remaining">The remaining bytes in the segment block.</param>
    /// <exception cref="ImageFormatException">
    ///     Thrown if the tables do not match the header
    /// </exception>
    private void ProcessDefineQuantizationTablesMarker(int remaining)
    {
        while (remaining > 0)
        {
            var done = false;
            remaining--;
            var quantizationTableSpec = InputStream.ReadByte();
            var tableIndex = quantizationTableSpec & 15;

            // Max index. 4 Tables max.
            if (tableIndex > 3) JpegThrowHelper.ThrowBadQuantizationTable();

            switch (quantizationTableSpec >> 4)
            {
                case 0:
                {
                    // 8 bit values
                    if (remaining < 64)
                    {
                        done = true;
                        break;
                    }

                    InputStream.Read(temp, 0, 64);
                    remaining -= 64;

                    ref var table = ref QuantizationTables[tableIndex];
                    for (var j = 0; j < 64; j++) table[j] = temp[j];
                }

                    break;
                case 1:
                {
                    // 16 bit values
                    if (remaining < 128)
                    {
                        done = true;
                        break;
                    }

                    InputStream.Read(temp, 0, 128);
                    remaining -= 128;

                    ref var table = ref QuantizationTables[tableIndex];
                    for (var j = 0; j < 64; j++) table[j] = (temp[2 * j] << 8) | temp[2 * j + 1];
                }

                    break;

                default:
                {
                    JpegThrowHelper.ThrowBadQuantizationTable();
                    break;
                }
            }

            if (done) break;
        }

        if (remaining != 0) JpegThrowHelper.ThrowBadMarker(nameof(JpegConstants.Markers.DQT), remaining);

        Metadata.GetFormatMetadata(JpegFormat.Instance).Quality = QualityEvaluator.EstimateQuality(QuantizationTables);
    }

    /// <summary>
    ///     Processes the Start of Frame marker.  Specified in section B.2.2.
    /// </summary>
    /// <param name="remaining">The remaining bytes in the segment block.</param>
    /// <param name="frameMarker">The current frame marker.</param>
    /// <param name="metadataOnly">Whether to parse metadata only</param>
    private void ProcessStartOfFrameMarker(int remaining, in JpegFileMarker frameMarker, bool metadataOnly)
    {
        if (Frame != null)
            JpegThrowHelper.ThrowInvalidImageContentException(
                "Multiple SOF markers. Only single frame jpegs supported.");

        // Read initial marker definitions.
        const int length = 6;
        InputStream.Read(temp, 0, length);

        // We only support 8-bit and 12-bit precision.
        if (Array.IndexOf(supportedPrecisions, temp[0]) == -1)
            JpegThrowHelper.ThrowInvalidImageContentException("Only 8-Bit and 12-Bit precision supported.");

        Precision = temp[0];

        Frame = new JpegFrame
        {
            Extended = frameMarker.Marker == JpegConstants.Markers.SOF1,
            Progressive = frameMarker.Marker == JpegConstants.Markers.SOF2,
            Precision = temp[0],
            Scanlines = (short)((temp[1] << 8) | temp[2]),
            SamplesPerLine = (short)((temp[3] << 8) | temp[4]),
            ComponentCount = temp[5]
        };

        if (Frame.SamplesPerLine == 0 || Frame.Scanlines == 0)
            JpegThrowHelper.ThrowInvalidImageDimensions(Frame.SamplesPerLine, Frame.Scanlines);

        ImageSizeInPixels = new Size(Frame.SamplesPerLine, Frame.Scanlines);
        ComponentCount = Frame.ComponentCount;

        if (!metadataOnly)
        {
            remaining -= length;

            const int componentBytes = 3;
            if (remaining > ComponentCount * componentBytes) JpegThrowHelper.ThrowBadMarker("SOFn", remaining);

            InputStream.Read(temp, 0, remaining);

            // No need to pool this. They max out at 4
            Frame.ComponentIds = new byte[ComponentCount];
            Frame.ComponentOrder = new byte[ComponentCount];
            Frame.Components = new JpegComponent[ComponentCount];
            ColorSpace = DeduceJpegColorSpace();

            var maxH = 0;
            var maxV = 0;
            var index = 0;
            for (var i = 0; i < ComponentCount; i++)
            {
                var hv = temp[index + 1];
                var h = (hv >> 4) & 15;
                var v = hv & 15;

                if (maxH < h) maxH = h;

                if (maxV < v) maxV = v;

                var component = new JpegComponent(configuration.MemoryAllocator, Frame, temp[index], h, v,
                    temp[index + 2], i);

                Frame.Components[i] = component;
                Frame.ComponentIds[i] = component.Id;

                index += componentBytes;
            }

            Frame.MaxHorizontalFactor = maxH;
            Frame.MaxVerticalFactor = maxV;
            ColorSpace = DeduceJpegColorSpace();
            Frame.InitComponents();
            ImageSizeInMCU = new Size(Frame.McusPerLine, Frame.McusPerColumn);
        }
    }

    /// <summary>
    ///     Processes a Define Huffman Table marker, and initializes a huffman
    ///     struct from its contents. Specified in section B.2.4.2.
    /// </summary>
    /// <param name="remaining">The remaining bytes in the segment block.</param>
    private void ProcessDefineHuffmanTablesMarker(int remaining)
    {
        var length = remaining;

        using (var huffmanData = configuration.MemoryAllocator.AllocateManagedByteBuffer(256, AllocationOptions.Clean))
        {
            ref var huffmanDataRef = ref MemoryMarshal.GetReference(huffmanData.GetSpan());
            for (var i = 2; i < remaining;)
            {
                var huffmanTableSpec = (byte)InputStream.ReadByte();
                var tableType = huffmanTableSpec >> 4;
                var tableIndex = huffmanTableSpec & 15;

                // Types 0..1 DC..AC
                if (tableType > 1) JpegThrowHelper.ThrowInvalidImageContentException("Bad Huffman Table type.");

                // Max tables of each type
                if (tableIndex > 3) JpegThrowHelper.ThrowInvalidImageContentException("Bad Huffman Table index.");

                InputStream.Read(huffmanData.Array, 0, 16);

                using (var codeLengths =
                       configuration.MemoryAllocator.AllocateManagedByteBuffer(17, AllocationOptions.Clean))
                {
                    ref var codeLengthsRef = ref MemoryMarshal.GetReference(codeLengths.GetSpan());
                    var codeLengthSum = 0;

                    for (var j = 1; j < 17; j++)
                        codeLengthSum += Unsafe.Add(ref codeLengthsRef, j) = Unsafe.Add(ref huffmanDataRef, j - 1);

                    length -= 17;

                    if (codeLengthSum > 256 || codeLengthSum > length)
                        JpegThrowHelper.ThrowInvalidImageContentException("Huffman table has excessive length.");

                    using (var huffmanValues =
                           configuration.MemoryAllocator.AllocateManagedByteBuffer(256, AllocationOptions.Clean))
                    {
                        InputStream.Read(huffmanValues.Array, 0, codeLengthSum);

                        i += 17 + codeLengthSum;

                        BuildHuffmanTable(
                            tableType == 0 ? dcHuffmanTables : acHuffmanTables,
                            tableIndex,
                            codeLengths.GetSpan(),
                            huffmanValues.GetSpan());
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Processes the DRI (Define Restart Interval Marker) Which specifies the interval between RSTn markers, in
    ///     macroblocks
    /// </summary>
    /// <param name="remaining">The remaining bytes in the segment block.</param>
    private void ProcessDefineRestartIntervalMarker(int remaining)
    {
        if (remaining != 2) JpegThrowHelper.ThrowBadMarker(nameof(JpegConstants.Markers.DRI), remaining);

        resetInterval = ReadUint16();
    }

    /// <summary>
    ///     Processes the SOS (Start of scan marker).
    /// </summary>
    private void ProcessStartOfScanMarker()
    {
        if (Frame is null)
            JpegThrowHelper.ThrowInvalidImageContentException("No readable SOFn (Start Of Frame) marker found.");

        var selectorsCount = InputStream.ReadByte();
        for (var i = 0; i < selectorsCount; i++)
        {
            var componentIndex = -1;
            var selector = InputStream.ReadByte();

            for (var j = 0; j < Frame.ComponentIds.Length; j++)
            {
                var id = Frame.ComponentIds[j];
                if (selector == id)
                {
                    componentIndex = j;
                    break;
                }
            }

            if (componentIndex < 0)
                JpegThrowHelper.ThrowInvalidImageContentException($"Unknown component selector {componentIndex}.");

            ref var component = ref Frame.Components[componentIndex];
            var tableSpec = InputStream.ReadByte();
            component.DCHuffmanTableId = tableSpec >> 4;
            component.ACHuffmanTableId = tableSpec & 15;
            Frame.ComponentOrder[i] = (byte)componentIndex;
        }

        InputStream.Read(temp, 0, 3);

        int spectralStart = temp[0];
        int spectralEnd = temp[1];
        int successiveApproximation = temp[2];

        var sd = new HuffmanScanDecoder(
            InputStream,
            Frame,
            dcHuffmanTables,
            acHuffmanTables,
            selectorsCount,
            resetInterval,
            spectralStart,
            spectralEnd,
            successiveApproximation >> 4,
            successiveApproximation & 15);

        sd.ParseEntropyCodedData();
    }

    /// <summary>
    ///     Builds the huffman tables
    /// </summary>
    /// <param name="tables">The tables</param>
    /// <param name="index">The table index</param>
    /// <param name="codeLengths">The codelengths</param>
    /// <param name="values">The values</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    private void BuildHuffmanTable(HuffmanTable[] tables, int index, ReadOnlySpan<byte> codeLengths,
        ReadOnlySpan<byte> values)
    {
        tables[index] = new HuffmanTable(codeLengths, values);
    }

    /// <summary>
    ///     Reads a <see cref="ushort" /> from the stream advancing it by two bytes
    /// </summary>
    /// <returns>The <see cref="ushort" /></returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    private ushort ReadUint16()
    {
        InputStream.Read(markerBuffer, 0, 2);
        return BinaryPrimitives.ReadUInt16BigEndian(markerBuffer);
    }

    /// <summary>
    ///     Post processes the pixels into the destination image.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <returns>The <see cref="Image{TPixel}" />.</returns>
    private Image<TPixel> PostProcessIntoImage<TPixel>()
        where TPixel : unmanaged, IPixel<TPixel>
    {
        if (ImageWidth == 0 || ImageHeight == 0) JpegThrowHelper.ThrowInvalidImageDimensions(ImageWidth, ImageHeight);

        var image = Image.CreateUninitialized<TPixel>(
            configuration,
            ImageWidth,
            ImageHeight,
            Metadata,
            JpegFormat.Instance);

        using (var postProcessor = new JpegImagePostProcessor(configuration, this))
        {
            postProcessor.PostProcess(image.Frames.RootFrame);
        }

        return image;
    }
}