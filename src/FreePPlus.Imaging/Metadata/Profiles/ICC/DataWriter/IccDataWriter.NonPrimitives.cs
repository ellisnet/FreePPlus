// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Provides methods to write ICC data types
/// </summary>
internal sealed partial class IccDataWriter
{
    /// <summary>
    ///     Writes a DateTime
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteDateTime(DateTime value)
    {
        return WriteUInt16((ushort)value.Year)
               + WriteUInt16((ushort)value.Month)
               + WriteUInt16((ushort)value.Day)
               + WriteUInt16((ushort)value.Hour)
               + WriteUInt16((ushort)value.Minute)
               + WriteUInt16((ushort)value.Second);
    }

    /// <summary>
    ///     Writes an ICC profile version number
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteVersionNumber(in IccVersion value)
    {
        var major = value.Major.Clamp(0, byte.MaxValue);
        var minor = value.Minor.Clamp(0, 15);
        var bugfix = value.Patch.Clamp(0, 15);

        var version = (major << 24) | (minor << 20) | (bugfix << 16);
        return WriteInt32(version);
    }

    /// <summary>
    ///     Writes an XYZ number
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteXyzNumber(Vector3 value)
    {
        return WriteFix16(value.X)
               + WriteFix16(value.Y)
               + WriteFix16(value.Z);
    }

    /// <summary>
    ///     Writes a profile ID
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteProfileId(in IccProfileId value)
    {
        return WriteUInt32(value.Part1)
               + WriteUInt32(value.Part2)
               + WriteUInt32(value.Part3)
               + WriteUInt32(value.Part4);
    }

    /// <summary>
    ///     Writes a position number
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WritePositionNumber(in IccPositionNumber value)
    {
        return WriteUInt32(value.Offset)
               + WriteUInt32(value.Size);
    }

    /// <summary>
    ///     Writes a response number
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteResponseNumber(in IccResponseNumber value)
    {
        return WriteUInt16(value.DeviceCode)
               + WriteFix16(value.MeasurementValue);
    }

    /// <summary>
    ///     Writes a named color
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteNamedColor(in IccNamedColor value)
    {
        return WriteAsciiString(value.Name, 32, true)
               + WriteArray(value.PcsCoordinates)
               + WriteArray(value.DeviceCoordinates);
    }

    /// <summary>
    ///     Writes a profile description
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteProfileDescription(in IccProfileDescription value)
    {
        return WriteUInt32(value.DeviceManufacturer)
               + WriteUInt32(value.DeviceModel)
               + WriteInt64((long)value.DeviceAttributes)
               + WriteUInt32((uint)value.TechnologyInformation)
               + WriteTagDataEntryHeader(IccTypeSignature.MultiLocalizedUnicode)
               + WriteMultiLocalizedUnicodeTagDataEntry(
                   new IccMultiLocalizedUnicodeTagDataEntry(value.DeviceManufacturerInfo))
               + WriteTagDataEntryHeader(IccTypeSignature.MultiLocalizedUnicode)
               + WriteMultiLocalizedUnicodeTagDataEntry(
                   new IccMultiLocalizedUnicodeTagDataEntry(value.DeviceModelInfo));
    }

    /// <summary>
    ///     Writes a screening channel
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <returns>the number of bytes written</returns>
    public int WriteScreeningChannel(in IccScreeningChannel value)
    {
        return WriteFix16(value.Frequency)
               + WriteFix16(value.Angle)
               + WriteInt32((int)value.SpotShape);
    }
}