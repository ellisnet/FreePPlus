// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

/// <summary>
///     Reads and parses EXIF data from a byte array.
/// </summary>
internal sealed class ExifReader
{
    private readonly byte[] exifData;
    private uint exifOffset;
    private uint gpsOffset;
    private List<ExifTag> invalidTags;
    private bool isBigEndian;
    private int position;

    public ExifReader(byte[] exifData)
    {
        this.exifData = exifData ?? throw new ArgumentNullException(nameof(exifData));
    }

    /// <summary>
    ///     Gets the invalid tags.
    /// </summary>
    public IReadOnlyList<ExifTag> InvalidTags => invalidTags ?? (IReadOnlyList<ExifTag>)Array.Empty<ExifTag>();

    /// <summary>
    ///     Gets the thumbnail length in the byte stream.
    /// </summary>
    public uint ThumbnailLength { get; private set; }

    /// <summary>
    ///     Gets the thumbnail offset position in the byte stream.
    /// </summary>
    public uint ThumbnailOffset { get; private set; }

    /// <summary>
    ///     Gets the remaining length.
    /// </summary>
    private int RemainingLength
    {
        get
        {
            if (position >= exifData.Length) return 0;

            return exifData.Length - position;
        }
    }

    /// <summary>
    ///     Reads and returns the collection of EXIF values.
    /// </summary>
    /// <returns>
    ///     The <see cref="Collection{ExifValue}" />.
    /// </returns>
    public List<IExifValue> ReadValues()
    {
        var values = new List<IExifValue>();

        // II == 0x4949
        isBigEndian = ReadUInt16() != 0x4949;

        if (ReadUInt16() != 0x002A) return values;

        var ifdOffset = ReadUInt32();
        AddValues(values, ifdOffset);

        var thumbnailOffset = ReadUInt32();
        GetThumbnail(thumbnailOffset);

        if (exifOffset != 0) AddValues(values, exifOffset);

        if (gpsOffset != 0) AddValues(values, gpsOffset);

        return values;
    }

    private static TDataType[] ToArray<TDataType>(ExifDataType dataType, ReadOnlySpan<byte> data,
        ConverterMethod<TDataType> converter)
    {
        var dataTypeSize = (int)ExifDataTypes.GetSize(dataType);
        var length = data.Length / dataTypeSize;

        var result = new TDataType[length];

        for (var i = 0; i < length; i++)
        {
            var buffer = data.Slice(i * dataTypeSize, dataTypeSize);

            result.SetValue(converter(buffer), i);
        }

        return result;
    }

    private byte ConvertToByte(ReadOnlySpan<byte> buffer)
    {
        return buffer[0];
    }

    private string ConvertToString(ReadOnlySpan<byte> buffer)
    {
        var nullCharIndex = buffer.IndexOf((byte)0);

        if (nullCharIndex > -1) buffer = buffer.Slice(0, nullCharIndex);

        return Encoding.UTF8.GetString(buffer);
    }

    /// <summary>
    ///     Adds the collection of EXIF values to the reader.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="index">The index.</param>
    private void AddValues(List<IExifValue> values, uint index)
    {
        if (index > (uint)exifData.Length) return;

        position = (int)index;
        int count = ReadUInt16();

        for (var i = 0; i < count; i++)
        {
            if (!TryReadValue(out var value)) continue;

            var duplicate = false;
            foreach (var val in values)
                if (val == value)
                {
                    duplicate = true;
                    break;
                }

            if (duplicate) continue;

            if (value == ExifTag.SubIFDOffset)
                exifOffset = ((ExifLong)value).Value;
            else if (value == ExifTag.GPSIFDOffset)
                gpsOffset = ((ExifLong)value).Value;
            else
                values.Add(value);
        }
    }

    private object ConvertValue(ExifDataType dataType, ReadOnlySpan<byte> buffer, uint numberOfComponents)
    {
        if (buffer.Length == 0) return null;

        switch (dataType)
        {
            case ExifDataType.Unknown:
                return null;
            case ExifDataType.Ascii:
                return ConvertToString(buffer);
            case ExifDataType.Byte:
                if (numberOfComponents == 1) return ConvertToByte(buffer);

                return buffer.ToArray();
            case ExifDataType.DoubleFloat:
                if (numberOfComponents == 1) return ConvertToDouble(buffer);

                return ToArray(dataType, buffer, ConvertToDouble);
            case ExifDataType.Long:
                if (numberOfComponents == 1) return ConvertToUInt32(buffer);

                return ToArray(dataType, buffer, ConvertToUInt32);
            case ExifDataType.Rational:
                if (numberOfComponents == 1) return ToRational(buffer);

                return ToArray(dataType, buffer, ToRational);
            case ExifDataType.Short:
                if (numberOfComponents == 1) return ConvertToShort(buffer);

                return ToArray(dataType, buffer, ConvertToShort);
            case ExifDataType.SignedByte:
                if (numberOfComponents == 1) return ConvertToSignedByte(buffer);

                return ToArray(dataType, buffer, ConvertToSignedByte);
            case ExifDataType.SignedLong:
                if (numberOfComponents == 1) return ConvertToInt32(buffer);

                return ToArray(dataType, buffer, ConvertToInt32);
            case ExifDataType.SignedRational:
                if (numberOfComponents == 1) return ToSignedRational(buffer);

                return ToArray(dataType, buffer, ToSignedRational);
            case ExifDataType.SignedShort:
                if (numberOfComponents == 1) return ConvertToSignedShort(buffer);

                return ToArray(dataType, buffer, ConvertToSignedShort);
            case ExifDataType.SingleFloat:
                if (numberOfComponents == 1) return ConvertToSingle(buffer);

                return ToArray(dataType, buffer, ConvertToSingle);
            case ExifDataType.Undefined:
                if (numberOfComponents == 1) return ConvertToByte(buffer);

                return buffer.ToArray();
            default:
                throw new NotSupportedException();
        }
    }

    private bool TryReadValue(out ExifValue exifValue)
    {
        exifValue = default;

        // 2   | 2    | 4     | 4
        // tag | type | count | value offset
        if (RemainingLength < 12) return false;

        var tag = (ExifTagValue)ReadUInt16();
        var dataType = EnumUtils.Parse(ReadUInt16(), ExifDataType.Unknown);

        // Ensure that the data type is valid
        if (dataType == ExifDataType.Unknown) return false;

        var numberOfComponents = ReadUInt32();

        // Issue #132: ExifDataType == Undefined is treated like a byte array.
        // If numberOfComponents == 0 this value can only be handled as an inline value and must fallback to 4 (bytes)
        if (dataType == ExifDataType.Undefined && numberOfComponents == 0) numberOfComponents = 4;

        var size = numberOfComponents * ExifDataTypes.GetSize(dataType);

        TryReadSpan(4, out var offsetBuffer);

        object value;
        if (size > 4)
        {
            var oldIndex = position;
            var newIndex = ConvertToUInt32(offsetBuffer);

            // Ensure that the new index does not overrun the data
            if (newIndex > int.MaxValue)
            {
                AddInvalidTag(new UnkownExifTag(tag));
                return false;
            }

            position = (int)newIndex;

            if (RemainingLength < size)
            {
                AddInvalidTag(new UnkownExifTag(tag));

                position = oldIndex;
                return false;
            }

            TryReadSpan((int)size, out var dataBuffer);

            value = ConvertValue(dataType, dataBuffer, numberOfComponents);
            position = oldIndex;
        }
        else
        {
            value = ConvertValue(dataType, offsetBuffer, numberOfComponents);
        }

        exifValue = ExifValues.Create(tag) ?? ExifValues.Create(tag, dataType, numberOfComponents);

        if (exifValue is null)
        {
            AddInvalidTag(new UnkownExifTag(tag));
            return false;
        }

        if (!exifValue.TrySetValue(value)) return false;

        return true;
    }

    private void AddInvalidTag(ExifTag tag)
    {
        (invalidTags ?? (invalidTags = new List<ExifTag>())).Add(tag);
    }

    private bool TryReadSpan(int length, out ReadOnlySpan<byte> span)
    {
        if (RemainingLength < length)
        {
            span = default;

            return false;
        }

        span = new ReadOnlySpan<byte>(exifData, position, length);

        position += length;

        return true;
    }

    private uint ReadUInt32()
    {
        // Known as Long in Exif Specification
        return TryReadSpan(4, out var span)
            ? ConvertToUInt32(span)
            : default;
    }

    private ushort ReadUInt16()
    {
        return TryReadSpan(2, out var span)
            ? ConvertToShort(span)
            : default;
    }

    private void GetThumbnail(uint offset)
    {
        var values = new List<IExifValue>();
        AddValues(values, offset);

        foreach (ExifValue value in values)
            if (value == ExifTag.JPEGInterchangeFormat)
                ThumbnailOffset = ((ExifLong)value).Value;
            else if (value == ExifTag.JPEGInterchangeFormatLength) ThumbnailLength = ((ExifLong)value).Value;
    }

    private double ConvertToDouble(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 8) return default;

        var intValue = isBigEndian
            ? BinaryPrimitives.ReadInt64BigEndian(buffer)
            : BinaryPrimitives.ReadInt64LittleEndian(buffer);

        return Unsafe.As<long, double>(ref intValue);
    }

    private uint ConvertToUInt32(ReadOnlySpan<byte> buffer)
    {
        // Known as Long in Exif Specification
        if (buffer.Length < 4) return default;

        return isBigEndian
            ? BinaryPrimitives.ReadUInt32BigEndian(buffer)
            : BinaryPrimitives.ReadUInt32LittleEndian(buffer);
    }

    private ushort ConvertToShort(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 2) return default;

        return isBigEndian
            ? BinaryPrimitives.ReadUInt16BigEndian(buffer)
            : BinaryPrimitives.ReadUInt16LittleEndian(buffer);
    }

    private float ConvertToSingle(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 4) return default;

        var intValue = isBigEndian
            ? BinaryPrimitives.ReadInt32BigEndian(buffer)
            : BinaryPrimitives.ReadInt32LittleEndian(buffer);

        return Unsafe.As<int, float>(ref intValue);
    }

    private Rational ToRational(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 8) return default;

        var numerator = ConvertToUInt32(buffer.Slice(0, 4));
        var denominator = ConvertToUInt32(buffer.Slice(4, 4));

        return new Rational(numerator, denominator, false);
    }

    private sbyte ConvertToSignedByte(ReadOnlySpan<byte> buffer)
    {
        return unchecked((sbyte)buffer[0]);
    }

    private int ConvertToInt32(ReadOnlySpan<byte> buffer) // SignedLong in Exif Specification
    {
        if (buffer.Length < 4) return default;

        return isBigEndian
            ? BinaryPrimitives.ReadInt32BigEndian(buffer)
            : BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    private SignedRational ToSignedRational(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 8) return default;

        var numerator = ConvertToInt32(buffer.Slice(0, 4));
        var denominator = ConvertToInt32(buffer.Slice(4, 4));

        return new SignedRational(numerator, denominator, false);
    }

    private short ConvertToSignedShort(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 2) return default;

        return isBigEndian
            ? BinaryPrimitives.ReadInt16BigEndian(buffer)
            : BinaryPrimitives.ReadInt16LittleEndian(buffer);
    }

    private delegate TDataType ConverterMethod<TDataType>(ReadOnlySpan<byte> data);
}