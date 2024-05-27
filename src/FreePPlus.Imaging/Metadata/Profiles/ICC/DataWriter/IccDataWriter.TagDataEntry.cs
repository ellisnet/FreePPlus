// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Linq;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Provides methods to write ICC data types
/// </summary>
internal sealed partial class IccDataWriter
{
    /// <summary>
    ///     Writes a tag data entry
    /// </summary>
    /// <param name="data">The entry to write</param>
    /// <param name="table">The table entry for the written data entry</param>
    /// <returns>The number of bytes written (excluding padding)</returns>
    public int WriteTagDataEntry(IccTagDataEntry data, out IccTagTableEntry table)
    {
        var offset = (uint)dataStream.Position;
        var count = WriteTagDataEntry(data);
        WritePadding();
        table = new IccTagTableEntry(data.TagSignature, offset, (uint)count);
        return count;
    }

    /// <summary>
    ///     Writes a tag data entry (without padding)
    /// </summary>
    /// <param name="entry">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteTagDataEntry(IccTagDataEntry entry)
    {
        var count = WriteTagDataEntryHeader(entry.Signature);

        switch (entry.Signature)
        {
            case IccTypeSignature.Chromaticity:
                count += WriteChromaticityTagDataEntry((IccChromaticityTagDataEntry)entry);
                break;
            case IccTypeSignature.ColorantOrder:
                count += WriteColorantOrderTagDataEntry((IccColorantOrderTagDataEntry)entry);
                break;
            case IccTypeSignature.ColorantTable:
                count += WriteColorantTableTagDataEntry((IccColorantTableTagDataEntry)entry);
                break;
            case IccTypeSignature.Curve:
                count += WriteCurveTagDataEntry((IccCurveTagDataEntry)entry);
                break;
            case IccTypeSignature.Data:
                count += WriteDataTagDataEntry((IccDataTagDataEntry)entry);
                break;
            case IccTypeSignature.DateTime:
                count += WriteDateTimeTagDataEntry((IccDateTimeTagDataEntry)entry);
                break;
            case IccTypeSignature.Lut16:
                count += WriteLut16TagDataEntry((IccLut16TagDataEntry)entry);
                break;
            case IccTypeSignature.Lut8:
                count += WriteLut8TagDataEntry((IccLut8TagDataEntry)entry);
                break;
            case IccTypeSignature.LutAToB:
                count += WriteLutAtoBTagDataEntry((IccLutAToBTagDataEntry)entry);
                break;
            case IccTypeSignature.LutBToA:
                count += WriteLutBtoATagDataEntry((IccLutBToATagDataEntry)entry);
                break;
            case IccTypeSignature.Measurement:
                count += WriteMeasurementTagDataEntry((IccMeasurementTagDataEntry)entry);
                break;
            case IccTypeSignature.MultiLocalizedUnicode:
                count += WriteMultiLocalizedUnicodeTagDataEntry((IccMultiLocalizedUnicodeTagDataEntry)entry);
                break;
            case IccTypeSignature.MultiProcessElements:
                count += WriteMultiProcessElementsTagDataEntry((IccMultiProcessElementsTagDataEntry)entry);
                break;
            case IccTypeSignature.NamedColor2:
                count += WriteNamedColor2TagDataEntry((IccNamedColor2TagDataEntry)entry);
                break;
            case IccTypeSignature.ParametricCurve:
                count += WriteParametricCurveTagDataEntry((IccParametricCurveTagDataEntry)entry);
                break;
            case IccTypeSignature.ProfileSequenceDesc:
                count += WriteProfileSequenceDescTagDataEntry((IccProfileSequenceDescTagDataEntry)entry);
                break;
            case IccTypeSignature.ProfileSequenceIdentifier:
                count += WriteProfileSequenceIdentifierTagDataEntry((IccProfileSequenceIdentifierTagDataEntry)entry);
                break;
            case IccTypeSignature.ResponseCurveSet16:
                count += WriteResponseCurveSet16TagDataEntry((IccResponseCurveSet16TagDataEntry)entry);
                break;
            case IccTypeSignature.S15Fixed16Array:
                count += WriteFix16ArrayTagDataEntry((IccFix16ArrayTagDataEntry)entry);
                break;
            case IccTypeSignature.Signature:
                count += WriteSignatureTagDataEntry((IccSignatureTagDataEntry)entry);
                break;
            case IccTypeSignature.Text:
                count += WriteTextTagDataEntry((IccTextTagDataEntry)entry);
                break;
            case IccTypeSignature.U16Fixed16Array:
                count += WriteUFix16ArrayTagDataEntry((IccUFix16ArrayTagDataEntry)entry);
                break;
            case IccTypeSignature.UInt16Array:
                count += WriteUInt16ArrayTagDataEntry((IccUInt16ArrayTagDataEntry)entry);
                break;
            case IccTypeSignature.UInt32Array:
                count += WriteUInt32ArrayTagDataEntry((IccUInt32ArrayTagDataEntry)entry);
                break;
            case IccTypeSignature.UInt64Array:
                count += WriteUInt64ArrayTagDataEntry((IccUInt64ArrayTagDataEntry)entry);
                break;
            case IccTypeSignature.UInt8Array:
                count += WriteUInt8ArrayTagDataEntry((IccUInt8ArrayTagDataEntry)entry);
                break;
            case IccTypeSignature.ViewingConditions:
                count += WriteViewingConditionsTagDataEntry((IccViewingConditionsTagDataEntry)entry);
                break;
            case IccTypeSignature.Xyz:
                count += WriteXyzTagDataEntry((IccXyzTagDataEntry)entry);
                break;

            // V2 Types:
            case IccTypeSignature.TextDescription:
                count += WriteTextDescriptionTagDataEntry((IccTextDescriptionTagDataEntry)entry);
                break;
            case IccTypeSignature.CrdInfo:
                count += WriteCrdInfoTagDataEntry((IccCrdInfoTagDataEntry)entry);
                break;
            case IccTypeSignature.Screening:
                count += WriteScreeningTagDataEntry((IccScreeningTagDataEntry)entry);
                break;
            case IccTypeSignature.UcrBg:
                count += WriteUcrBgTagDataEntry((IccUcrBgTagDataEntry)entry);
                break;

            // Unsupported or unknown
            case IccTypeSignature.DeviceSettings:
            case IccTypeSignature.NamedColor:
            case IccTypeSignature.Unknown:
            default:
                count += WriteUnknownTagDataEntry(entry as IccUnknownTagDataEntry);
                break;
        }

        return count;
    }

    /// <summary>
    ///     Writes the header of a <see cref="IccTagDataEntry" />
    /// </summary>
    /// <param name="signature">The signature of the entry</param>
    /// <returns>The number of bytes written</returns>
    public int WriteTagDataEntryHeader(IccTypeSignature signature)
    {
        return WriteUInt32((uint)signature)
               + WriteEmpty(4);
    }

    /// <summary>
    ///     Writes a <see cref="IccUnknownTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteUnknownTagDataEntry(IccUnknownTagDataEntry value)
    {
        return WriteArray(value.Data);
    }

    /// <summary>
    ///     Writes a <see cref="IccChromaticityTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteChromaticityTagDataEntry(IccChromaticityTagDataEntry value)
    {
        var count = WriteUInt16((ushort)value.ChannelCount);
        count += WriteUInt16((ushort)value.ColorantType);

        for (var i = 0; i < value.ChannelCount; i++)
        {
            count += WriteUFix16(value.ChannelValues[i][0]);
            count += WriteUFix16(value.ChannelValues[i][1]);
        }

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccColorantOrderTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteColorantOrderTagDataEntry(IccColorantOrderTagDataEntry value)
    {
        return WriteUInt32((uint)value.ColorantNumber.Length)
               + WriteArray(value.ColorantNumber);
    }

    /// <summary>
    ///     Writes a <see cref="IccColorantTableTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteColorantTableTagDataEntry(IccColorantTableTagDataEntry value)
    {
        var count = WriteUInt32((uint)value.ColorantData.Length);

        for (var i = 0; i < value.ColorantData.Length; i++)
        {
            ref var colorant = ref value.ColorantData[i];

            count += WriteAsciiString(colorant.Name, 32, true);
            count += WriteUInt16(colorant.Pcs1);
            count += WriteUInt16(colorant.Pcs2);
            count += WriteUInt16(colorant.Pcs3);
        }

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccCurveTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteCurveTagDataEntry(IccCurveTagDataEntry value)
    {
        var count = 0;

        if (value.IsIdentityResponse)
        {
            count += WriteUInt32(0);
        }
        else if (value.IsGamma)
        {
            count += WriteUInt32(1);
            count += WriteUFix8(value.Gamma);
        }
        else
        {
            count += WriteUInt32((uint)value.CurveData.Length);
            for (var i = 0; i < value.CurveData.Length; i++)
                count += WriteUInt16((ushort)(value.CurveData[i] * ushort.MaxValue + 0.5f).Clamp(0, ushort.MaxValue));
        }

        return count;

        // TODO: Page 48: If the input is PCSXYZ, 1+(32 767/32 768) shall be mapped to the value 1,0. If the output is PCSXYZ, the value 1,0 shall be mapped to 1+(32 767/32 768).
    }

    /// <summary>
    ///     Writes a <see cref="IccDataTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteDataTagDataEntry(IccDataTagDataEntry value)
    {
        return WriteEmpty(3)
               + WriteByte((byte)(value.IsAscii ? 0x01 : 0x00))
               + WriteArray(value.Data);
    }

    /// <summary>
    ///     Writes a <see cref="IccDateTimeTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteDateTimeTagDataEntry(IccDateTimeTagDataEntry value)
    {
        return WriteDateTime(value.Value);
    }

    /// <summary>
    ///     Writes a <see cref="IccLut16TagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteLut16TagDataEntry(IccLut16TagDataEntry value)
    {
        var count = WriteByte((byte)value.InputValues.Length);
        count += WriteByte((byte)value.OutputValues.Length);
        count += WriteByte(value.ClutValues.GridPointCount[0]);
        count += WriteEmpty(1);

        count += WriteMatrix(value.Matrix, false);

        count += WriteUInt16((ushort)value.InputValues[0].Values.Length);
        count += WriteUInt16((ushort)value.OutputValues[0].Values.Length);

        foreach (var lut in value.InputValues) count += WriteLut16(lut);

        count += WriteClut16(value.ClutValues);

        foreach (var lut in value.OutputValues) count += WriteLut16(lut);

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccLut8TagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteLut8TagDataEntry(IccLut8TagDataEntry value)
    {
        var count = WriteByte((byte)value.InputChannelCount);
        count += WriteByte((byte)value.OutputChannelCount);
        count += WriteByte((byte)value.ClutValues.Values[0].Length);
        count += WriteEmpty(1);

        count += WriteMatrix(value.Matrix, false);

        foreach (var lut in value.InputValues) count += WriteLut8(lut);

        count += WriteClut8(value.ClutValues);

        foreach (var lut in value.OutputValues) count += WriteLut8(lut);

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccLutAToBTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteLutAtoBTagDataEntry(IccLutAToBTagDataEntry value)
    {
        var start = dataStream.Position - 8; // 8 is the tag header size

        var count = WriteByte((byte)value.InputChannelCount);
        count += WriteByte((byte)value.OutputChannelCount);
        count += WriteEmpty(2);

        long bCurveOffset = 0;
        long matrixOffset = 0;
        long mCurveOffset = 0;
        long clutOffset = 0;
        long aCurveOffset = 0;

        // Jump over offset values
        var offsetpos = dataStream.Position;
        dataStream.Position += 5 * 4;

        if (value.CurveB != null)
        {
            bCurveOffset = dataStream.Position;
            count += WriteCurves(value.CurveB);
            count += WritePadding();
        }

        if (value.Matrix3x1 != null && value.Matrix3x3 != null)
        {
            matrixOffset = dataStream.Position;
            count += WriteMatrix(value.Matrix3x3.Value, false);
            count += WriteMatrix(value.Matrix3x1.Value, false);
            count += WritePadding();
        }

        if (value.CurveM != null)
        {
            mCurveOffset = dataStream.Position;
            count += WriteCurves(value.CurveM);
            count += WritePadding();
        }

        if (value.ClutValues != null)
        {
            clutOffset = dataStream.Position;
            count += WriteClut(value.ClutValues);
            count += WritePadding();
        }

        if (value.CurveA != null)
        {
            aCurveOffset = dataStream.Position;
            count += WriteCurves(value.CurveA);
            count += WritePadding();
        }

        // Set offset values
        var lpos = dataStream.Position;
        dataStream.Position = offsetpos;

        if (bCurveOffset != 0) bCurveOffset -= start;

        if (matrixOffset != 0) matrixOffset -= start;

        if (mCurveOffset != 0) mCurveOffset -= start;

        if (clutOffset != 0) clutOffset -= start;

        if (aCurveOffset != 0) aCurveOffset -= start;

        count += WriteUInt32((uint)bCurveOffset);
        count += WriteUInt32((uint)matrixOffset);
        count += WriteUInt32((uint)mCurveOffset);
        count += WriteUInt32((uint)clutOffset);
        count += WriteUInt32((uint)aCurveOffset);

        dataStream.Position = lpos;
        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccLutBToATagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteLutBtoATagDataEntry(IccLutBToATagDataEntry value)
    {
        var start = dataStream.Position - 8; // 8 is the tag header size

        var count = WriteByte((byte)value.InputChannelCount);
        count += WriteByte((byte)value.OutputChannelCount);
        count += WriteEmpty(2);

        long bCurveOffset = 0;
        long matrixOffset = 0;
        long mCurveOffset = 0;
        long clutOffset = 0;
        long aCurveOffset = 0;

        // Jump over offset values
        var offsetpos = dataStream.Position;
        dataStream.Position += 5 * 4;

        if (value.CurveB != null)
        {
            bCurveOffset = dataStream.Position;
            count += WriteCurves(value.CurveB);
            count += WritePadding();
        }

        if (value.Matrix3x1 != null && value.Matrix3x3 != null)
        {
            matrixOffset = dataStream.Position;
            count += WriteMatrix(value.Matrix3x3.Value, false);
            count += WriteMatrix(value.Matrix3x1.Value, false);
            count += WritePadding();
        }

        if (value.CurveM != null)
        {
            mCurveOffset = dataStream.Position;
            count += WriteCurves(value.CurveM);
            count += WritePadding();
        }

        if (value.ClutValues != null)
        {
            clutOffset = dataStream.Position;
            count += WriteClut(value.ClutValues);
            count += WritePadding();
        }

        if (value.CurveA != null)
        {
            aCurveOffset = dataStream.Position;
            count += WriteCurves(value.CurveA);
            count += WritePadding();
        }

        // Set offset values
        var lpos = dataStream.Position;
        dataStream.Position = offsetpos;

        if (bCurveOffset != 0) bCurveOffset -= start;

        if (matrixOffset != 0) matrixOffset -= start;

        if (mCurveOffset != 0) mCurveOffset -= start;

        if (clutOffset != 0) clutOffset -= start;

        if (aCurveOffset != 0) aCurveOffset -= start;

        count += WriteUInt32((uint)bCurveOffset);
        count += WriteUInt32((uint)matrixOffset);
        count += WriteUInt32((uint)mCurveOffset);
        count += WriteUInt32((uint)clutOffset);
        count += WriteUInt32((uint)aCurveOffset);

        dataStream.Position = lpos;
        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccMeasurementTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteMeasurementTagDataEntry(IccMeasurementTagDataEntry value)
    {
        return WriteUInt32((uint)value.Observer)
               + WriteXyzNumber(value.XyzBacking)
               + WriteUInt32((uint)value.Geometry)
               + WriteUFix16(value.Flare)
               + WriteUInt32((uint)value.Illuminant);
    }

    /// <summary>
    ///     Writes a <see cref="IccMultiLocalizedUnicodeTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteMultiLocalizedUnicodeTagDataEntry(IccMultiLocalizedUnicodeTagDataEntry value)
    {
        var start = dataStream.Position - 8; // 8 is the tag header size

        var cultureCount = value.Texts.Length;

        var count = WriteUInt32((uint)cultureCount);
        count += WriteUInt32(12); // One record has always 12 bytes size

        // Jump over position table
        var tpos = dataStream.Position;
        dataStream.Position += cultureCount * 12;

        // TODO: Investigate cost of Linq GroupBy
        var texts = value.Texts.GroupBy(t => t.Text).ToArray();

        var offset = new uint[texts.Length];
        var lengths = new int[texts.Length];

        for (var i = 0; i < texts.Length; i++)
        {
            offset[i] = (uint)(dataStream.Position - start);
            count += lengths[i] = WriteUnicodeString(texts[i].Key);
        }

        // Write position table
        var lpos = dataStream.Position;
        dataStream.Position = tpos;
        for (var i = 0; i < texts.Length; i++)
            foreach (var localizedString in texts[i])
            {
                var cultureName = localizedString.Culture.Name;
                if (string.IsNullOrEmpty(cultureName))
                {
                    count += WriteAsciiString("xx", 2, false);
                    count += WriteAsciiString("\0\0", 2, false);
                }
                else if (cultureName.Contains("-"))
                {
                    var code = cultureName.Split('-');
                    count += WriteAsciiString(code[0].ToLower(), 2, false);
                    count += WriteAsciiString(code[1].ToUpper(), 2, false);
                }
                else
                {
                    count += WriteAsciiString(cultureName, 2, false);
                    count += WriteAsciiString("\0\0", 2, false);
                }

                count += WriteUInt32((uint)lengths[i]);
                count += WriteUInt32(offset[i]);
            }

        dataStream.Position = lpos;
        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccMultiProcessElementsTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteMultiProcessElementsTagDataEntry(IccMultiProcessElementsTagDataEntry value)
    {
        var start = dataStream.Position - 8; // 8 is the tag header size

        var count = WriteUInt16((ushort)value.InputChannelCount);
        count += WriteUInt16((ushort)value.OutputChannelCount);
        count += WriteUInt32((uint)value.Data.Length);

        // Jump over position table
        var tpos = dataStream.Position;
        dataStream.Position += value.Data.Length * 8;

        var posTable = new IccPositionNumber[value.Data.Length];
        for (var i = 0; i < value.Data.Length; i++)
        {
            var offset = (uint)(dataStream.Position - start);
            var size = WriteMultiProcessElement(value.Data[i]);
            count += WritePadding();
            posTable[i] = new IccPositionNumber(offset, (uint)size);
            count += size;
        }

        // Write position table
        var lpos = dataStream.Position;
        dataStream.Position = tpos;
        foreach (var pos in posTable) count += WritePositionNumber(pos);

        dataStream.Position = lpos;
        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccNamedColor2TagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteNamedColor2TagDataEntry(IccNamedColor2TagDataEntry value)
    {
        var count = WriteInt32(value.VendorFlags)
                    + WriteUInt32((uint)value.Colors.Length)
                    + WriteUInt32((uint)value.CoordinateCount)
                    + WriteAsciiString(value.Prefix, 32, true)
                    + WriteAsciiString(value.Suffix, 32, true);

        foreach (var color in value.Colors) count += WriteNamedColor(color);

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccParametricCurveTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteParametricCurveTagDataEntry(IccParametricCurveTagDataEntry value)
    {
        return WriteParametricCurve(value.Curve);
    }

    /// <summary>
    ///     Writes a <see cref="IccProfileSequenceDescTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteProfileSequenceDescTagDataEntry(IccProfileSequenceDescTagDataEntry value)
    {
        var count = WriteUInt32((uint)value.Descriptions.Length);

        for (var i = 0; i < value.Descriptions.Length; i++)
        {
            ref var desc = ref value.Descriptions[i];

            count += WriteProfileDescription(desc);
        }

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccProfileSequenceIdentifierTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteProfileSequenceIdentifierTagDataEntry(IccProfileSequenceIdentifierTagDataEntry value)
    {
        var start = dataStream.Position - 8; // 8 is the tag header size
        var length = value.Data.Length;

        var count = WriteUInt32((uint)length);

        // Jump over position table
        var tablePosition = dataStream.Position;
        dataStream.Position += length * 8;
        var table = new IccPositionNumber[length];

        for (var i = 0; i < length; i++)
        {
            ref var sequenceIdentifier = ref value.Data[i];

            var offset = (uint)(dataStream.Position - start);
            var size = WriteProfileId(sequenceIdentifier.Id);
            size += WriteTagDataEntry(new IccMultiLocalizedUnicodeTagDataEntry(sequenceIdentifier.Description));
            size += WritePadding();
            table[i] = new IccPositionNumber(offset, (uint)size);
            count += size;
        }

        // Write position table
        var lpos = dataStream.Position;
        dataStream.Position = tablePosition;
        foreach (var pos in table) count += WritePositionNumber(pos);

        dataStream.Position = lpos;
        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccResponseCurveSet16TagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteResponseCurveSet16TagDataEntry(IccResponseCurveSet16TagDataEntry value)
    {
        var start = dataStream.Position - 8;

        var count = WriteUInt16(value.ChannelCount);
        count += WriteUInt16((ushort)value.Curves.Length);

        // Jump over position table
        var tablePosition = dataStream.Position;
        dataStream.Position += value.Curves.Length * 4;

        var offset = new uint[value.Curves.Length];

        for (var i = 0; i < value.Curves.Length; i++)
        {
            offset[i] = (uint)(dataStream.Position - start);
            count += WriteResponseCurve(value.Curves[i]);
            count += WritePadding();
        }

        // Write position table
        var lpos = dataStream.Position;
        dataStream.Position = tablePosition;
        count += WriteArray(offset);

        dataStream.Position = lpos;
        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccFix16ArrayTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteFix16ArrayTagDataEntry(IccFix16ArrayTagDataEntry value)
    {
        var count = 0;
        for (var i = 0; i < value.Data.Length; i++) count += WriteFix16(value.Data[i] * 256d);

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccSignatureTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteSignatureTagDataEntry(IccSignatureTagDataEntry value)
    {
        return WriteAsciiString(value.SignatureData, 4, false);
    }

    /// <summary>
    ///     Writes a <see cref="IccTextTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteTextTagDataEntry(IccTextTagDataEntry value)
    {
        return WriteAsciiString(value.Text);
    }

    /// <summary>
    ///     Writes a <see cref="IccUFix16ArrayTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteUFix16ArrayTagDataEntry(IccUFix16ArrayTagDataEntry value)
    {
        var count = 0;
        for (var i = 0; i < value.Data.Length; i++) count += WriteUFix16(value.Data[i]);

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccUInt16ArrayTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteUInt16ArrayTagDataEntry(IccUInt16ArrayTagDataEntry value)
    {
        return WriteArray(value.Data);
    }

    /// <summary>
    ///     Writes a <see cref="IccUInt32ArrayTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteUInt32ArrayTagDataEntry(IccUInt32ArrayTagDataEntry value)
    {
        return WriteArray(value.Data);
    }

    /// <summary>
    ///     Writes a <see cref="IccUInt64ArrayTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteUInt64ArrayTagDataEntry(IccUInt64ArrayTagDataEntry value)
    {
        return WriteArray(value.Data);
    }

    /// <summary>
    ///     Writes a <see cref="IccUInt8ArrayTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteUInt8ArrayTagDataEntry(IccUInt8ArrayTagDataEntry value)
    {
        return WriteArray(value.Data);
    }

    /// <summary>
    ///     Writes a <see cref="IccViewingConditionsTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteViewingConditionsTagDataEntry(IccViewingConditionsTagDataEntry value)
    {
        return WriteXyzNumber(value.IlluminantXyz)
               + WriteXyzNumber(value.SurroundXyz)
               + WriteUInt32((uint)value.Illuminant);
    }

    /// <summary>
    ///     Writes a <see cref="IccXyzTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteXyzTagDataEntry(IccXyzTagDataEntry value)
    {
        var count = 0;
        for (var i = 0; i < value.Data.Length; i++) count += WriteXyzNumber(value.Data[i]);

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccTextDescriptionTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteTextDescriptionTagDataEntry(IccTextDescriptionTagDataEntry value)
    {
        int size, count = 0;

        if (value.Ascii is null)
        {
            count += WriteUInt32(0);
        }
        else
        {
            dataStream.Position += 4;
            count += size = WriteAsciiString(value.Ascii + '\0');
            dataStream.Position -= size + 4;
            count += WriteUInt32((uint)size);
            dataStream.Position += size;
        }

        if (value.Unicode is null)
        {
            count += WriteUInt32(0);
            count += WriteUInt32(0);
        }
        else
        {
            dataStream.Position += 8;
            count += size = WriteUnicodeString(value.Unicode + '\0');
            dataStream.Position -= size + 8;
            count += WriteUInt32(value.UnicodeLanguageCode);
            count += WriteUInt32((uint)value.Unicode.Length + 1);
            dataStream.Position += size;
        }

        if (value.ScriptCode is null)
        {
            count += WriteUInt16(0);
            count += WriteByte(0);
            count += WriteEmpty(67);
        }
        else
        {
            dataStream.Position += 3;
            count += size = WriteAsciiString(value.ScriptCode, 67, true);
            dataStream.Position -= size + 3;
            count += WriteUInt16(value.ScriptCodeCode);
            count += WriteByte((byte)(value.ScriptCode.Length > 66 ? 67 : value.ScriptCode.Length + 1));
            dataStream.Position += size;
        }

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccCrdInfoTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteCrdInfoTagDataEntry(IccCrdInfoTagDataEntry value)
    {
        var count = 0;
        WriteString(value.PostScriptProductName);
        WriteString(value.RenderingIntent0Crd);
        WriteString(value.RenderingIntent1Crd);
        WriteString(value.RenderingIntent2Crd);
        WriteString(value.RenderingIntent3Crd);

        return count;

        void WriteString(string text)
        {
            int textLength;
            if (string.IsNullOrEmpty(text))
                textLength = 0;
            else
                textLength = text.Length + 1; // + 1 for null terminator

            count += WriteUInt32((uint)textLength);
            count += WriteAsciiString(text, textLength, true);
        }
    }

    /// <summary>
    ///     Writes a <see cref="IccScreeningTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteScreeningTagDataEntry(IccScreeningTagDataEntry value)
    {
        var count = 0;

        count += WriteInt32((int)value.Flags);
        count += WriteUInt32((uint)value.Channels.Length);
        for (var i = 0; i < value.Channels.Length; i++) count += WriteScreeningChannel(value.Channels[i]);

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccUcrBgTagDataEntry" />
    /// </summary>
    /// <param name="value">The entry to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteUcrBgTagDataEntry(IccUcrBgTagDataEntry value)
    {
        var count = 0;

        count += WriteUInt32((uint)value.UcrCurve.Length);
        for (var i = 0; i < value.UcrCurve.Length; i++) count += WriteUInt16(value.UcrCurve[i]);

        count += WriteUInt32((uint)value.BgCurve.Length);
        for (var i = 0; i < value.BgCurve.Length; i++) count += WriteUInt16(value.BgCurve[i]);

        count += WriteAsciiString(value.Description + '\0');

        return count;
    }
}