// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Provides methods to read ICC data types
/// </summary>
internal sealed partial class IccDataReader
{
    /// <summary>
    ///     Reads a DateTime
    /// </summary>
    /// <returns>the value</returns>
    public DateTime ReadDateTime()
    {
        try
        {
            return new DateTime(
                ReadUInt16(),
                ReadUInt16(),
                ReadUInt16(),
                ReadUInt16(),
                ReadUInt16(),
                ReadUInt16(),
                DateTimeKind.Utc);
        }
        catch (ArgumentOutOfRangeException)
        {
            return DateTime.MinValue;
        }
    }

    /// <summary>
    ///     Reads an ICC profile version number
    /// </summary>
    /// <returns>the version number</returns>
    public IccVersion ReadVersionNumber()
    {
        var version = ReadInt32();

        var major = (version >> 24) & 0xFF;
        var minor = (version >> 20) & 0x0F;
        var bugfix = (version >> 16) & 0x0F;

        return new IccVersion(major, minor, bugfix);
    }

    /// <summary>
    ///     Reads an XYZ number
    /// </summary>
    /// <returns>the XYZ number</returns>
    public Vector3 ReadXyzNumber()
    {
        return new Vector3(
            ReadFix16(),
            ReadFix16(),
            ReadFix16());
    }

    /// <summary>
    ///     Reads a profile ID
    /// </summary>
    /// <returns>the profile ID</returns>
    public IccProfileId ReadProfileId()
    {
        return new IccProfileId(
            ReadUInt32(),
            ReadUInt32(),
            ReadUInt32(),
            ReadUInt32());
    }

    /// <summary>
    ///     Reads a position number
    /// </summary>
    /// <returns>the position number</returns>
    public IccPositionNumber ReadPositionNumber()
    {
        return new IccPositionNumber(
            ReadUInt32(),
            ReadUInt32());
    }

    /// <summary>
    ///     Reads a response number
    /// </summary>
    /// <returns>the response number</returns>
    public IccResponseNumber ReadResponseNumber()
    {
        return new IccResponseNumber(
            ReadUInt16(),
            ReadFix16());
    }

    /// <summary>
    ///     Reads a named color
    /// </summary>
    /// <param name="deviceCoordCount">Number of device coordinates</param>
    /// <returns>the named color</returns>
    public IccNamedColor ReadNamedColor(uint deviceCoordCount)
    {
        var name = ReadAsciiString(32);
        ushort[] pcsCoord = { ReadUInt16(), ReadUInt16(), ReadUInt16() };
        var deviceCoord = new ushort[deviceCoordCount];

        for (var i = 0; i < deviceCoordCount; i++) deviceCoord[i] = ReadUInt16();

        return new IccNamedColor(name, pcsCoord, deviceCoord);
    }

    /// <summary>
    ///     Reads a profile description
    /// </summary>
    /// <returns>the profile description</returns>
    public IccProfileDescription ReadProfileDescription()
    {
        var manufacturer = ReadUInt32();
        var model = ReadUInt32();
        var attributes = (IccDeviceAttribute)ReadInt64();
        var technologyInfo = (IccProfileTag)ReadUInt32();

        var manufacturerInfo = ReadText();
        var modelInfo = ReadText();

        return new IccProfileDescription(
            manufacturer,
            model,
            attributes,
            technologyInfo,
            manufacturerInfo.Texts,
            modelInfo.Texts);

        IccMultiLocalizedUnicodeTagDataEntry ReadText()
        {
            var type = ReadTagDataEntryHeader();
            switch (type)
            {
                case IccTypeSignature.MultiLocalizedUnicode:
                    return ReadMultiLocalizedUnicodeTagDataEntry();
                case IccTypeSignature.TextDescription:
                    return (IccMultiLocalizedUnicodeTagDataEntry)ReadTextDescriptionTagDataEntry();

                default:
                    throw new InvalidIccProfileException(
                        "Profile description can only have multi-localized Unicode or text description entries");
            }
        }
    }

    /// <summary>
    ///     Reads a colorant table entry
    /// </summary>
    /// <returns>the profile description</returns>
    public IccColorantTableEntry ReadColorantTableEntry()
    {
        return new IccColorantTableEntry(
            ReadAsciiString(32),
            ReadUInt16(),
            ReadUInt16(),
            ReadUInt16());
    }

    /// <summary>
    ///     Reads a screening channel
    /// </summary>
    /// <returns>the screening channel</returns>
    public IccScreeningChannel ReadScreeningChannel()
    {
        return new IccScreeningChannel(
            ReadFix16(),
            ReadFix16(),
            (IccScreeningSpotType)ReadInt32());
    }
}