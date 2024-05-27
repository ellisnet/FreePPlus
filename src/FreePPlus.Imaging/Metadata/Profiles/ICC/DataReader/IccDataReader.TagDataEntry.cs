// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Globalization;
using System.Numerics;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Provides methods to read ICC data types
/// </summary>
internal sealed partial class IccDataReader
{
    /// <summary>
    ///     Reads a tag data entry
    /// </summary>
    /// <param name="info">The table entry with reading information</param>
    /// <returns>the tag data entry</returns>
    public IccTagDataEntry ReadTagDataEntry(IccTagTableEntry info)
    {
        currentIndex = (int)info.Offset;
        var type = ReadTagDataEntryHeader();

        switch (type)
        {
            case IccTypeSignature.Chromaticity:
                return ReadChromaticityTagDataEntry();
            case IccTypeSignature.ColorantOrder:
                return ReadColorantOrderTagDataEntry();
            case IccTypeSignature.ColorantTable:
                return ReadColorantTableTagDataEntry();
            case IccTypeSignature.Curve:
                return ReadCurveTagDataEntry();
            case IccTypeSignature.Data:
                return ReadDataTagDataEntry(info.DataSize);
            case IccTypeSignature.DateTime:
                return ReadDateTimeTagDataEntry();
            case IccTypeSignature.Lut16:
                return ReadLut16TagDataEntry();
            case IccTypeSignature.Lut8:
                return ReadLut8TagDataEntry();
            case IccTypeSignature.LutAToB:
                return ReadLutAtoBTagDataEntry();
            case IccTypeSignature.LutBToA:
                return ReadLutBtoATagDataEntry();
            case IccTypeSignature.Measurement:
                return ReadMeasurementTagDataEntry();
            case IccTypeSignature.MultiLocalizedUnicode:
                return ReadMultiLocalizedUnicodeTagDataEntry();
            case IccTypeSignature.MultiProcessElements:
                return ReadMultiProcessElementsTagDataEntry();
            case IccTypeSignature.NamedColor2:
                return ReadNamedColor2TagDataEntry();
            case IccTypeSignature.ParametricCurve:
                return ReadParametricCurveTagDataEntry();
            case IccTypeSignature.ProfileSequenceDesc:
                return ReadProfileSequenceDescTagDataEntry();
            case IccTypeSignature.ProfileSequenceIdentifier:
                return ReadProfileSequenceIdentifierTagDataEntry();
            case IccTypeSignature.ResponseCurveSet16:
                return ReadResponseCurveSet16TagDataEntry();
            case IccTypeSignature.S15Fixed16Array:
                return ReadFix16ArrayTagDataEntry(info.DataSize);
            case IccTypeSignature.Signature:
                return ReadSignatureTagDataEntry();
            case IccTypeSignature.Text:
                return ReadTextTagDataEntry(info.DataSize);
            case IccTypeSignature.U16Fixed16Array:
                return ReadUFix16ArrayTagDataEntry(info.DataSize);
            case IccTypeSignature.UInt16Array:
                return ReadUInt16ArrayTagDataEntry(info.DataSize);
            case IccTypeSignature.UInt32Array:
                return ReadUInt32ArrayTagDataEntry(info.DataSize);
            case IccTypeSignature.UInt64Array:
                return ReadUInt64ArrayTagDataEntry(info.DataSize);
            case IccTypeSignature.UInt8Array:
                return ReadUInt8ArrayTagDataEntry(info.DataSize);
            case IccTypeSignature.ViewingConditions:
                return ReadViewingConditionsTagDataEntry();
            case IccTypeSignature.Xyz:
                return ReadXyzTagDataEntry(info.DataSize);

            // V2 Types:
            case IccTypeSignature.TextDescription:
                return ReadTextDescriptionTagDataEntry();
            case IccTypeSignature.CrdInfo:
                return ReadCrdInfoTagDataEntry();
            case IccTypeSignature.Screening:
                return ReadScreeningTagDataEntry();
            case IccTypeSignature.UcrBg:
                return ReadUcrBgTagDataEntry(info.DataSize);

            // Unsupported or unknown
            case IccTypeSignature.DeviceSettings:
            case IccTypeSignature.NamedColor:
            case IccTypeSignature.Unknown:
            default:
                return ReadUnknownTagDataEntry(info.DataSize);
        }
    }

    /// <summary>
    ///     Reads the header of a <see cref="IccTagDataEntry" />
    /// </summary>
    /// <returns>The read signature</returns>
    public IccTypeSignature ReadTagDataEntryHeader()
    {
        var type = (IccTypeSignature)ReadUInt32();
        AddIndex(4); // 4 bytes are not used
        return type;
    }

    /// <summary>
    ///     Reads the header of a <see cref="IccTagDataEntry" /> and checks if it's the expected value
    /// </summary>
    /// <param name="expected">expected value to check against</param>
    public void ReadCheckTagDataEntryHeader(IccTypeSignature expected)
    {
        var type = ReadTagDataEntryHeader();
        if (expected != (IccTypeSignature)uint.MaxValue && type != expected)
            throw new InvalidIccProfileException($"Read signature {type} is not the expected {expected}");
    }

    /// <summary>
    ///     Reads a <see cref="IccTagDataEntry" /> with an unknown <see cref="IccTypeSignature" />
    /// </summary>
    /// <param name="size">The size of the entry in bytes</param>
    /// <returns>The read entry</returns>
    public IccUnknownTagDataEntry ReadUnknownTagDataEntry(uint size)
    {
        var count = (int)size - 8; // 8 is the tag header size
        return new IccUnknownTagDataEntry(ReadBytes(count));
    }

    /// <summary>
    ///     Reads a <see cref="IccChromaticityTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccChromaticityTagDataEntry ReadChromaticityTagDataEntry()
    {
        var channelCount = ReadUInt16();
        var colorant = (IccColorantEncoding)ReadUInt16();

        if (Enum.IsDefined(typeof(IccColorantEncoding), colorant) && colorant != IccColorantEncoding.Unknown)
            // The type is known and so are the values (they are constant)
            // channelCount should always be 3 but it doesn't really matter if it's not
            return new IccChromaticityTagDataEntry(colorant);

        // The type is not know, so the values need be read
        var values = new double[channelCount][];
        for (var i = 0; i < channelCount; i++) values[i] = new double[] { ReadUFix16(), ReadUFix16() };

        return new IccChromaticityTagDataEntry(values);
    }

    /// <summary>
    ///     Reads a <see cref="IccColorantOrderTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccColorantOrderTagDataEntry ReadColorantOrderTagDataEntry()
    {
        var colorantCount = ReadUInt32();
        var number = ReadBytes((int)colorantCount);
        return new IccColorantOrderTagDataEntry(number);
    }

    /// <summary>
    ///     Reads a <see cref="IccColorantTableTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccColorantTableTagDataEntry ReadColorantTableTagDataEntry()
    {
        var colorantCount = ReadUInt32();
        var cdata = new IccColorantTableEntry[colorantCount];
        for (var i = 0; i < colorantCount; i++) cdata[i] = ReadColorantTableEntry();

        return new IccColorantTableTagDataEntry(cdata);
    }

    /// <summary>
    ///     Reads a <see cref="IccCurveTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccCurveTagDataEntry ReadCurveTagDataEntry()
    {
        var pointCount = ReadUInt32();

        if (pointCount == 0) return new IccCurveTagDataEntry();

        if (pointCount == 1) return new IccCurveTagDataEntry(ReadUFix8());

        var cdata = new float[pointCount];
        for (var i = 0; i < pointCount; i++) cdata[i] = ReadUInt16() / 65535f;

        return new IccCurveTagDataEntry(cdata);

        // TODO: If the input is PCSXYZ, 1+(32 767/32 768) shall be mapped to the value 1,0. If the output is PCSXYZ, the value 1,0 shall be mapped to 1+(32 767/32 768).
    }

    /// <summary>
    ///     Reads a <see cref="IccDataTagDataEntry" />
    /// </summary>
    /// <param name="size">The size of the entry in bytes</param>
    /// <returns>The read entry</returns>
    public IccDataTagDataEntry ReadDataTagDataEntry(uint size)
    {
        AddIndex(3); // first 3 bytes are zero
        var b = data[AddIndex(1)];

        // last bit of 4th byte is either 0 = ASCII or 1 = binary
        var ascii = GetBit(b, 7);
        var length = (int)size - 12;
        var cdata = ReadBytes(length);

        return new IccDataTagDataEntry(cdata, ascii);
    }

    /// <summary>
    ///     Reads a <see cref="IccDateTimeTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccDateTimeTagDataEntry ReadDateTimeTagDataEntry()
    {
        return new IccDateTimeTagDataEntry(ReadDateTime());
    }

    /// <summary>
    ///     Reads a <see cref="IccLut16TagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccLut16TagDataEntry ReadLut16TagDataEntry()
    {
        var inChCount = data[AddIndex(1)];
        var outChCount = data[AddIndex(1)];
        var clutPointCount = data[AddIndex(1)];
        AddIndex(1); // 1 byte reserved

        var matrix = ReadMatrix(3, 3, false);

        var inTableCount = ReadUInt16();
        var outTableCount = ReadUInt16();

        // Input LUT
        var inValues = new IccLut[inChCount];
        var gridPointCount = new byte[inChCount];
        for (var i = 0; i < inChCount; i++)
        {
            inValues[i] = ReadLut16(inTableCount);
            gridPointCount[i] = clutPointCount;
        }

        // CLUT
        var clut = ReadClut16(inChCount, outChCount, gridPointCount);

        // Output LUT
        var outValues = new IccLut[outChCount];
        for (var i = 0; i < outChCount; i++) outValues[i] = ReadLut16(outTableCount);

        return new IccLut16TagDataEntry(matrix, inValues, clut, outValues);
    }

    /// <summary>
    ///     Reads a <see cref="IccLut8TagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccLut8TagDataEntry ReadLut8TagDataEntry()
    {
        var inChCount = data[AddIndex(1)];
        var outChCount = data[AddIndex(1)];
        var clutPointCount = data[AddIndex(1)];
        AddIndex(1); // 1 byte reserved

        var matrix = ReadMatrix(3, 3, false);

        // Input LUT
        var inValues = new IccLut[inChCount];
        var gridPointCount = new byte[inChCount];
        for (var i = 0; i < inChCount; i++)
        {
            inValues[i] = ReadLut8();
            gridPointCount[i] = clutPointCount;
        }

        // CLUT
        var clut = ReadClut8(inChCount, outChCount, gridPointCount);

        // Output LUT
        var outValues = new IccLut[outChCount];
        for (var i = 0; i < outChCount; i++) outValues[i] = ReadLut8();

        return new IccLut8TagDataEntry(matrix, inValues, clut, outValues);
    }

    /// <summary>
    ///     Reads a <see cref="IccLutAToBTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccLutAToBTagDataEntry ReadLutAtoBTagDataEntry()
    {
        var start = currentIndex - 8; // 8 is the tag header size

        var inChCount = data[AddIndex(1)];
        var outChCount = data[AddIndex(1)];
        AddIndex(2); // 2 bytes reserved

        var bCurveOffset = ReadUInt32();
        var matrixOffset = ReadUInt32();
        var mCurveOffset = ReadUInt32();
        var clutOffset = ReadUInt32();
        var aCurveOffset = ReadUInt32();

        IccTagDataEntry[] bCurve = null;
        IccTagDataEntry[] mCurve = null;
        IccTagDataEntry[] aCurve = null;
        IccClut clut = null;
        float[,] matrix3x3 = null;
        float[] matrix3x1 = null;

        if (bCurveOffset != 0)
        {
            currentIndex = (int)bCurveOffset + start;
            bCurve = ReadCurves(outChCount);
        }

        if (mCurveOffset != 0)
        {
            currentIndex = (int)mCurveOffset + start;
            mCurve = ReadCurves(outChCount);
        }

        if (aCurveOffset != 0)
        {
            currentIndex = (int)aCurveOffset + start;
            aCurve = ReadCurves(inChCount);
        }

        if (clutOffset != 0)
        {
            currentIndex = (int)clutOffset + start;
            clut = ReadClut(inChCount, outChCount, false);
        }

        if (matrixOffset != 0)
        {
            currentIndex = (int)matrixOffset + start;
            matrix3x3 = ReadMatrix(3, 3, false);
            matrix3x1 = ReadMatrix(3, false);
        }

        return new IccLutAToBTagDataEntry(bCurve, matrix3x3, matrix3x1, mCurve, clut, aCurve);
    }

    /// <summary>
    ///     Reads a <see cref="IccLutBToATagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccLutBToATagDataEntry ReadLutBtoATagDataEntry()
    {
        var start = currentIndex - 8; // 8 is the tag header size

        var inChCount = data[AddIndex(1)];
        var outChCount = data[AddIndex(1)];
        AddIndex(2); // 2 bytes reserved

        var bCurveOffset = ReadUInt32();
        var matrixOffset = ReadUInt32();
        var mCurveOffset = ReadUInt32();
        var clutOffset = ReadUInt32();
        var aCurveOffset = ReadUInt32();

        IccTagDataEntry[] bCurve = null;
        IccTagDataEntry[] mCurve = null;
        IccTagDataEntry[] aCurve = null;
        IccClut clut = null;
        float[,] matrix3x3 = null;
        float[] matrix3x1 = null;

        if (bCurveOffset != 0)
        {
            currentIndex = (int)bCurveOffset + start;
            bCurve = ReadCurves(inChCount);
        }

        if (mCurveOffset != 0)
        {
            currentIndex = (int)mCurveOffset + start;
            mCurve = ReadCurves(inChCount);
        }

        if (aCurveOffset != 0)
        {
            currentIndex = (int)aCurveOffset + start;
            aCurve = ReadCurves(outChCount);
        }

        if (clutOffset != 0)
        {
            currentIndex = (int)clutOffset + start;
            clut = ReadClut(inChCount, outChCount, false);
        }

        if (matrixOffset != 0)
        {
            currentIndex = (int)matrixOffset + start;
            matrix3x3 = ReadMatrix(3, 3, false);
            matrix3x1 = ReadMatrix(3, false);
        }

        return new IccLutBToATagDataEntry(bCurve, matrix3x3, matrix3x1, mCurve, clut, aCurve);
    }

    /// <summary>
    ///     Reads a <see cref="IccMeasurementTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccMeasurementTagDataEntry ReadMeasurementTagDataEntry()
    {
        return new IccMeasurementTagDataEntry(
            (IccStandardObserver)ReadUInt32(),
            ReadXyzNumber(),
            (IccMeasurementGeometry)ReadUInt32(),
            ReadUFix16(),
            (IccStandardIlluminant)ReadUInt32());
    }

    /// <summary>
    ///     Reads a <see cref="IccMultiLocalizedUnicodeTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccMultiLocalizedUnicodeTagDataEntry ReadMultiLocalizedUnicodeTagDataEntry()
    {
        var start = currentIndex - 8; // 8 is the tag header size
        var recordCount = ReadUInt32();

        ReadUInt32(); // Record size (always 12)
        var text = new IccLocalizedString[recordCount];

        var culture = new CultureInfo[recordCount];
        var length = new uint[recordCount];
        var offset = new uint[recordCount];

        for (var i = 0; i < recordCount; i++)
        {
            var languageCode = ReadAsciiString(2);
            var countryCode = ReadAsciiString(2);

            culture[i] = ReadCulture(languageCode, countryCode);
            length[i] = ReadUInt32();
            offset[i] = ReadUInt32();
        }

        for (var i = 0; i < recordCount; i++)
        {
            currentIndex = (int)(start + offset[i]);
            text[i] = new IccLocalizedString(culture[i], ReadUnicodeString((int)length[i]));
        }

        return new IccMultiLocalizedUnicodeTagDataEntry(text);

        CultureInfo ReadCulture(string language, string country)
        {
            if (string.IsNullOrWhiteSpace(language))
                return CultureInfo.InvariantCulture;
            if (string.IsNullOrWhiteSpace(country))
                try
                {
                    return new CultureInfo(language);
                }
                catch (CultureNotFoundException)
                {
                    return CultureInfo.InvariantCulture;
                }

            try
            {
                return new CultureInfo($"{language}-{country}");
            }
            catch (CultureNotFoundException)
            {
                return ReadCulture(language, null);
            }
        }
    }

    /// <summary>
    ///     Reads a <see cref="IccMultiProcessElementsTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccMultiProcessElementsTagDataEntry ReadMultiProcessElementsTagDataEntry()
    {
        var start = currentIndex - 8;

        ReadUInt16();
        ReadUInt16();
        var elementCount = ReadUInt32();

        var positionTable = new IccPositionNumber[elementCount];
        for (var i = 0; i < elementCount; i++) positionTable[i] = ReadPositionNumber();

        var elements = new IccMultiProcessElement[elementCount];
        for (var i = 0; i < elementCount; i++)
        {
            currentIndex = (int)positionTable[i].Offset + start;
            elements[i] = ReadMultiProcessElement();
        }

        return new IccMultiProcessElementsTagDataEntry(elements);
    }

    /// <summary>
    ///     Reads a <see cref="IccNamedColor2TagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccNamedColor2TagDataEntry ReadNamedColor2TagDataEntry()
    {
        var vendorFlag = ReadInt32();
        var colorCount = ReadUInt32();
        var coordCount = ReadUInt32();
        var prefix = ReadAsciiString(32);
        var suffix = ReadAsciiString(32);

        var colors = new IccNamedColor[colorCount];
        for (var i = 0; i < colorCount; i++) colors[i] = ReadNamedColor(coordCount);

        return new IccNamedColor2TagDataEntry(vendorFlag, prefix, suffix, colors);
    }

    /// <summary>
    ///     Reads a <see cref="IccParametricCurveTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccParametricCurveTagDataEntry ReadParametricCurveTagDataEntry()
    {
        return new IccParametricCurveTagDataEntry(ReadParametricCurve());
    }

    /// <summary>
    ///     Reads a <see cref="IccProfileSequenceDescTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccProfileSequenceDescTagDataEntry ReadProfileSequenceDescTagDataEntry()
    {
        var count = ReadUInt32();
        var description = new IccProfileDescription[count];
        for (var i = 0; i < count; i++) description[i] = ReadProfileDescription();

        return new IccProfileSequenceDescTagDataEntry(description);
    }

    /// <summary>
    ///     Reads a <see cref="IccProfileSequenceIdentifierTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccProfileSequenceIdentifierTagDataEntry ReadProfileSequenceIdentifierTagDataEntry()
    {
        var start = currentIndex - 8; // 8 is the tag header size
        var count = ReadUInt32();
        var table = new IccPositionNumber[count];
        for (var i = 0; i < count; i++) table[i] = ReadPositionNumber();

        var entries = new IccProfileSequenceIdentifier[count];
        for (var i = 0; i < count; i++)
        {
            currentIndex = (int)(start + table[i].Offset);
            var id = ReadProfileId();
            ReadCheckTagDataEntryHeader(IccTypeSignature.MultiLocalizedUnicode);
            var description = ReadMultiLocalizedUnicodeTagDataEntry();
            entries[i] = new IccProfileSequenceIdentifier(id, description.Texts);
        }

        return new IccProfileSequenceIdentifierTagDataEntry(entries);
    }

    /// <summary>
    ///     Reads a <see cref="IccResponseCurveSet16TagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccResponseCurveSet16TagDataEntry ReadResponseCurveSet16TagDataEntry()
    {
        var start = currentIndex - 8; // 8 is the tag header size
        var channelCount = ReadUInt16();
        var measurementCount = ReadUInt16();

        var offset = new uint[measurementCount];
        for (var i = 0; i < measurementCount; i++) offset[i] = ReadUInt32();

        var curves = new IccResponseCurve[measurementCount];
        for (var i = 0; i < measurementCount; i++)
        {
            currentIndex = (int)(start + offset[i]);
            curves[i] = ReadResponseCurve(channelCount);
        }

        return new IccResponseCurveSet16TagDataEntry(curves);
    }

    /// <summary>
    ///     Reads a <see cref="IccFix16ArrayTagDataEntry" />
    /// </summary>
    /// <param name="size">The size of the entry in bytes</param>
    /// <returns>The read entry</returns>
    public IccFix16ArrayTagDataEntry ReadFix16ArrayTagDataEntry(uint size)
    {
        var count = (size - 8) / 4;
        var arrayData = new float[count];
        for (var i = 0; i < count; i++) arrayData[i] = ReadFix16() / 256f;

        return new IccFix16ArrayTagDataEntry(arrayData);
    }

    /// <summary>
    ///     Reads a <see cref="IccSignatureTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccSignatureTagDataEntry ReadSignatureTagDataEntry()
    {
        return new IccSignatureTagDataEntry(ReadAsciiString(4));
    }

    /// <summary>
    ///     Reads a <see cref="IccTextTagDataEntry" />
    /// </summary>
    /// <param name="size">The size of the entry in bytes</param>
    /// <returns>The read entry</returns>
    public IccTextTagDataEntry ReadTextTagDataEntry(uint size)
    {
        return new IccTextTagDataEntry(ReadAsciiString((int)size - 8)); // 8 is the tag header size
    }

    /// <summary>
    ///     Reads a <see cref="IccUFix16ArrayTagDataEntry" />
    /// </summary>
    /// <param name="size">The size of the entry in bytes</param>
    /// <returns>The read entry</returns>
    public IccUFix16ArrayTagDataEntry ReadUFix16ArrayTagDataEntry(uint size)
    {
        var count = (size - 8) / 4;
        var arrayData = new float[count];
        for (var i = 0; i < count; i++) arrayData[i] = ReadUFix16();

        return new IccUFix16ArrayTagDataEntry(arrayData);
    }

    /// <summary>
    ///     Reads a <see cref="IccUInt16ArrayTagDataEntry" />
    /// </summary>
    /// <param name="size">The size of the entry in bytes</param>
    /// <returns>The read entry</returns>
    public IccUInt16ArrayTagDataEntry ReadUInt16ArrayTagDataEntry(uint size)
    {
        var count = (size - 8) / 2;
        var arrayData = new ushort[count];
        for (var i = 0; i < count; i++) arrayData[i] = ReadUInt16();

        return new IccUInt16ArrayTagDataEntry(arrayData);
    }

    /// <summary>
    ///     Reads a <see cref="IccUInt32ArrayTagDataEntry" />
    /// </summary>
    /// <param name="size">The size of the entry in bytes</param>
    /// <returns>The read entry</returns>
    public IccUInt32ArrayTagDataEntry ReadUInt32ArrayTagDataEntry(uint size)
    {
        var count = (size - 8) / 4;
        var arrayData = new uint[count];
        for (var i = 0; i < count; i++) arrayData[i] = ReadUInt32();

        return new IccUInt32ArrayTagDataEntry(arrayData);
    }

    /// <summary>
    ///     Reads a <see cref="IccUInt64ArrayTagDataEntry" />
    /// </summary>
    /// <param name="size">The size of the entry in bytes</param>
    /// <returns>The read entry</returns>
    public IccUInt64ArrayTagDataEntry ReadUInt64ArrayTagDataEntry(uint size)
    {
        var count = (size - 8) / 8;
        var arrayData = new ulong[count];
        for (var i = 0; i < count; i++) arrayData[i] = ReadUInt64();

        return new IccUInt64ArrayTagDataEntry(arrayData);
    }

    /// <summary>
    ///     Reads a <see cref="IccUInt8ArrayTagDataEntry" />
    /// </summary>
    /// <param name="size">The size of the entry in bytes</param>
    /// <returns>The read entry</returns>
    public IccUInt8ArrayTagDataEntry ReadUInt8ArrayTagDataEntry(uint size)
    {
        var count = (int)size - 8; // 8 is the tag header size
        var adata = ReadBytes(count);

        return new IccUInt8ArrayTagDataEntry(adata);
    }

    /// <summary>
    ///     Reads a <see cref="IccViewingConditionsTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccViewingConditionsTagDataEntry ReadViewingConditionsTagDataEntry()
    {
        return new IccViewingConditionsTagDataEntry(
            ReadXyzNumber(),
            ReadXyzNumber(),
            (IccStandardIlluminant)ReadUInt32());
    }

    /// <summary>
    ///     Reads a <see cref="IccXyzTagDataEntry" />
    /// </summary>
    /// <param name="size">The size of the entry in bytes</param>
    /// <returns>The read entry</returns>
    public IccXyzTagDataEntry ReadXyzTagDataEntry(uint size)
    {
        var count = (size - 8) / 12;
        var arrayData = new Vector3[count];
        for (var i = 0; i < count; i++) arrayData[i] = ReadXyzNumber();

        return new IccXyzTagDataEntry(arrayData);
    }

    /// <summary>
    ///     Reads a <see cref="IccTextDescriptionTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccTextDescriptionTagDataEntry ReadTextDescriptionTagDataEntry()
    {
        string unicodeValue, scriptcodeValue;
        var asciiValue = unicodeValue = scriptcodeValue = null;

        var asciiCount = (int)ReadUInt32();
        if (asciiCount > 0)
        {
            asciiValue = ReadAsciiString(asciiCount - 1);
            AddIndex(1); // Null terminator
        }

        var unicodeLangCode = ReadUInt32();
        var unicodeCount = (int)ReadUInt32();
        if (unicodeCount > 0)
        {
            unicodeValue = ReadUnicodeString(unicodeCount * 2 - 2);
            AddIndex(2); // Null terminator
        }

        var scriptcodeCode = ReadUInt16();
        int scriptcodeCount = Math.Min(data[AddIndex(1)], (byte)67);
        if (scriptcodeCount > 0)
        {
            scriptcodeValue = ReadAsciiString(scriptcodeCount - 1);
            AddIndex(1); // Null terminator
        }

        return new IccTextDescriptionTagDataEntry(
            asciiValue,
            unicodeValue,
            scriptcodeValue,
            unicodeLangCode,
            scriptcodeCode);
    }

    /// <summary>
    ///     Reads a <see cref="IccTextDescriptionTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccCrdInfoTagDataEntry ReadCrdInfoTagDataEntry()
    {
        var productNameCount = ReadUInt32();
        var productName = ReadAsciiString((int)productNameCount);

        var crd0Count = ReadUInt32();
        var crd0Name = ReadAsciiString((int)crd0Count);

        var crd1Count = ReadUInt32();
        var crd1Name = ReadAsciiString((int)crd1Count);

        var crd2Count = ReadUInt32();
        var crd2Name = ReadAsciiString((int)crd2Count);

        var crd3Count = ReadUInt32();
        var crd3Name = ReadAsciiString((int)crd3Count);

        return new IccCrdInfoTagDataEntry(productName, crd0Name, crd1Name, crd2Name, crd3Name);
    }

    /// <summary>
    ///     Reads a <see cref="IccScreeningTagDataEntry" />
    /// </summary>
    /// <returns>The read entry</returns>
    public IccScreeningTagDataEntry ReadScreeningTagDataEntry()
    {
        var flags = (IccScreeningFlag)ReadInt32();
        var channelCount = ReadUInt32();
        var channels = new IccScreeningChannel[channelCount];
        for (var i = 0; i < channels.Length; i++) channels[i] = ReadScreeningChannel();

        return new IccScreeningTagDataEntry(flags, channels);
    }

    /// <summary>
    ///     Reads a <see cref="IccUcrBgTagDataEntry" />
    /// </summary>
    /// <param name="size">The size of the entry in bytes</param>
    /// <returns>The read entry</returns>
    public IccUcrBgTagDataEntry ReadUcrBgTagDataEntry(uint size)
    {
        var ucrCount = ReadUInt32();
        var ucrCurve = new ushort[ucrCount];
        for (var i = 0; i < ucrCurve.Length; i++) ucrCurve[i] = ReadUInt16();

        var bgCount = ReadUInt32();
        var bgCurve = new ushort[bgCount];
        for (var i = 0; i < bgCurve.Length; i++) bgCurve[i] = ReadUInt16();

        // ((ucr length + bg length) * UInt16 size) + (ucrCount + bgCount)
        var dataSize = (ucrCount + bgCount) * 2 + 8;
        var descriptionLength = (int)(size - 8 - dataSize); // 8 is the tag header size
        var description = ReadAsciiString(descriptionLength);

        return new IccUcrBgTagDataEntry(ucrCurve, bgCurve, description);
    }
}