// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <content>
///     Provides methods to write ICC data types
/// </content>
internal sealed partial class IccDataWriter
{
    /// <summary>
    ///     Writes an 8bit lookup table
    /// </summary>
    /// <param name="value">The LUT to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteLut8(IccLut value)
    {
        foreach (var item in value.Values) WriteByte((byte)(item * byte.MaxValue + 0.5f).Clamp(0, byte.MaxValue));

        return value.Values.Length;
    }

    /// <summary>
    ///     Writes an 16bit lookup table
    /// </summary>
    /// <param name="value">The LUT to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteLut16(IccLut value)
    {
        foreach (var item in value.Values)
            WriteUInt16((ushort)(item * ushort.MaxValue + 0.5f).Clamp(0, ushort.MaxValue));

        return value.Values.Length * 2;
    }

    /// <summary>
    ///     Writes an color lookup table
    /// </summary>
    /// <param name="value">The CLUT to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteClut(IccClut value)
    {
        var count = WriteArray(value.GridPointCount);
        count += WriteEmpty(16 - value.GridPointCount.Length);

        switch (value.DataType)
        {
            case IccClutDataType.Float:
                return count + WriteClutF32(value);
            case IccClutDataType.UInt8:
                count += WriteByte(1);
                count += WriteEmpty(3);
                return count + WriteClut8(value);
            case IccClutDataType.UInt16:
                count += WriteByte(2);
                count += WriteEmpty(3);
                return count + WriteClut16(value);

            default:
                throw new InvalidIccProfileException($"Invalid CLUT data type of {value.DataType}");
        }
    }

    /// <summary>
    ///     Writes a 8bit color lookup table
    /// </summary>
    /// <param name="value">The CLUT to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteClut8(IccClut value)
    {
        var count = 0;
        foreach (var inArray in value.Values)
        foreach (var item in inArray)
            count += WriteByte((byte)(item * byte.MaxValue + 0.5f).Clamp(0, byte.MaxValue));

        return count;
    }

    /// <summary>
    ///     Writes a 16bit color lookup table
    /// </summary>
    /// <param name="value">The CLUT to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteClut16(IccClut value)
    {
        var count = 0;
        foreach (var inArray in value.Values)
        foreach (var item in inArray)
            count += WriteUInt16((ushort)(item * ushort.MaxValue + 0.5f).Clamp(0, ushort.MaxValue));

        return count;
    }

    /// <summary>
    ///     Writes a 32bit float color lookup table
    /// </summary>
    /// <param name="value">The CLUT to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteClutF32(IccClut value)
    {
        var count = 0;
        foreach (var inArray in value.Values)
        foreach (var item in inArray)
            count += WriteSingle(item);

        return count;
    }
}