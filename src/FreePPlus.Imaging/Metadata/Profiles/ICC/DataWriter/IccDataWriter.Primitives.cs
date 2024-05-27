// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Text;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Provides methods to write ICC data types
/// </summary>
internal sealed partial class IccDataWriter
{
    /// <summary>
    ///     Writes a byte
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteByte(byte value)
    {
        dataStream.WriteByte(value);
        return 1;
    }

    /// <summary>
    ///     Writes an ushort
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public unsafe int WriteUInt16(ushort value)
    {
        return WriteBytes((byte*)&value, 2);
    }

    /// <summary>
    ///     Writes a short
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public unsafe int WriteInt16(short value)
    {
        return WriteBytes((byte*)&value, 2);
    }

    /// <summary>
    ///     Writes an uint
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public unsafe int WriteUInt32(uint value)
    {
        return WriteBytes((byte*)&value, 4);
    }

    /// <summary>
    ///     Writes an int
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public unsafe int WriteInt32(int value)
    {
        return WriteBytes((byte*)&value, 4);
    }

    /// <summary>
    ///     Writes an ulong
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public unsafe int WriteUInt64(ulong value)
    {
        return WriteBytes((byte*)&value, 8);
    }

    /// <summary>
    ///     Writes a long
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public unsafe int WriteInt64(long value)
    {
        return WriteBytes((byte*)&value, 8);
    }

    /// <summary>
    ///     Writes a float
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public unsafe int WriteSingle(float value)
    {
        return WriteBytes((byte*)&value, 4);
    }

    /// <summary>
    ///     Writes a double
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public unsafe int WriteDouble(double value)
    {
        return WriteBytes((byte*)&value, 8);
    }

    /// <summary>
    ///     Writes a signed 32bit number with 1 sign bit, 15 value bits and 16 fractional bits
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteFix16(double value)
    {
        const double Max = short.MaxValue + 65535d / 65536d;
        const double Min = short.MinValue;

        value = value.Clamp(Min, Max);
        value *= 65536d;

        return WriteInt32((int)Math.Round(value, MidpointRounding.AwayFromZero));
    }

    /// <summary>
    ///     Writes an unsigned 32bit number with 16 value bits and 16 fractional bits
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteUFix16(double value)
    {
        const double Max = ushort.MaxValue + 65535d / 65536d;
        const double Min = ushort.MinValue;

        value = value.Clamp(Min, Max);
        value *= 65536d;

        return WriteUInt32((uint)Math.Round(value, MidpointRounding.AwayFromZero));
    }

    /// <summary>
    ///     Writes an unsigned 16bit number with 1 value bit and 15 fractional bits
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteU1Fix15(double value)
    {
        const double Max = 1 + 32767d / 32768d;
        const double Min = 0;

        value = value.Clamp(Min, Max);
        value *= 32768d;

        return WriteUInt16((ushort)Math.Round(value, MidpointRounding.AwayFromZero));
    }

    /// <summary>
    ///     Writes an unsigned 16bit number with 8 value bits and 8 fractional bits
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteUFix8(double value)
    {
        const double Max = byte.MaxValue + 255d / 256d;
        const double Min = byte.MinValue;

        value = value.Clamp(Min, Max);
        value *= 256d;

        return WriteUInt16((ushort)Math.Round(value, MidpointRounding.AwayFromZero));
    }

    /// <summary>
    ///     Writes an ASCII encoded string
    /// </summary>
    /// <param name="value">the string to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteAsciiString(string value)
    {
        if (string.IsNullOrEmpty(value)) return 0;

        var data = Encoding.ASCII.GetBytes(value);
        dataStream.Write(data, 0, data.Length);
        return data.Length;
    }

    /// <summary>
    ///     Writes an ASCII encoded string resizes it to the given length
    /// </summary>
    /// <param name="value">The string to write</param>
    /// <param name="length">The desired length of the string (including potential null terminator)</param>
    /// <param name="ensureNullTerminator">If True, there will be a \0 added at the end</param>
    /// <returns>the number of bytes written</returns>
    public int WriteAsciiString(string value, int length, bool ensureNullTerminator)
    {
        if (length == 0) return 0;

        Guard.MustBeGreaterThan(length, 0, nameof(length));

        if (value is null) value = string.Empty;

        var paddingChar = (byte)' ';
        var lengthAdjust = 0;

        if (ensureNullTerminator)
        {
            paddingChar = 0;
            lengthAdjust = 1;
        }

        value = value.Substring(0, Math.Min(length - lengthAdjust, value.Length));

        var textData = Encoding.ASCII.GetBytes(value);
        var actualLength = Math.Min(length - lengthAdjust, textData.Length);
        dataStream.Write(textData, 0, actualLength);
        for (var i = 0; i < length - actualLength; i++) dataStream.WriteByte(paddingChar);

        return length;
    }

    /// <summary>
    ///     Writes an UTF-16 big-endian encoded string
    /// </summary>
    /// <param name="value">the string to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteUnicodeString(string value)
    {
        if (string.IsNullOrEmpty(value)) return 0;

        var data = Encoding.BigEndianUnicode.GetBytes(value);
        dataStream.Write(data, 0, data.Length);
        return data.Length;
    }
}