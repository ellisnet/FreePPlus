// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

#pragma warning disable CA1822
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

/// <summary>
///     Parses a Compact Font Format (CFF) font program as described in The Compact Font Format specification (Adobe
///     Technical Note #5176).
///     A CFF font may contain multiple fonts and achieves compression by sharing details between fonts in the set.
/// </summary>
internal class CffParser
{
    /// <summary>
    ///     Latin 1 Encoding: ISO 8859-1 is a single-byte encoding that can represent the first 256 Unicode characters.
    /// </summary>
    private static readonly Encoding Iso88591 = Encoding.GetEncoding("ISO-8859-1");

    private readonly StringBuilder pooledStringBuilder = new();
    private int charsetOffset;
    private int charStringsOffset;
    private int encodingOffset = -1;
    private long offset;
    private int privateDICTLength;
    private int privateDICTOffset;

    public CffFont Load(BigEndianBinaryReader reader, long offset)
    {
        this.offset = offset;

        var fontName = ReadNameIndex(reader);

        var dataDicEntries = ReadTopDICTIndex(reader);
        var stringIndex = ReadStringIndex(reader);

        var topDictionary = ResolveTopDictInfo(dataDicEntries, stringIndex);

        var globalSubrRawBuffers = ReadGlobalSubrIndex(reader);

        ReadFDSelect(reader, topDictionary.CidFontInfo);
        var fontDicts = ReadFDArray(reader, topDictionary.CidFontInfo);

        var privateDictionary = ReadPrivateDict(reader);
        var glyphs = ReadCharStringsIndex(reader, topDictionary, globalSubrRawBuffers, fontDicts, privateDictionary);

        ReadCharsets(reader, stringIndex, glyphs);
        ReadEncodings(reader);

        return new CffFont(fontName, topDictionary, glyphs);
    }

    private string ReadNameIndex(BigEndianBinaryReader reader)
    {
        if (!TryReadIndexDataOffsets(reader, out var offsets))
            throw new InvalidFontFileException("No name index found.");

        // For Open Type the Name INDEX in the CFF data must contain only one entry;
        // that is, there must be only one font in the CFF FontSet.
        var offset = offsets[0];
        return reader.ReadString(offset.Length, Iso88591);
    }

    private List<CffDataDicEntry> ReadTopDICTIndex(BigEndianBinaryReader reader)
    {
        // 8. Top DICT INDEX
        // This contains the top - level DICTs of all the fonts in the FontSet
        // stored in an INDEX structure.Objects contained within this
        // INDEX correspond to those in the Name INDEX in both order
        // and number. Each object is a DICT structure that corresponds to
        // the top-level dictionary of a PostScript font.
        // A font is identified by an entry in the Name INDEX and its data
        // is accessed via the corresponding Top DICT
        if (!TryReadIndexDataOffsets(reader, out var offsets))
            throw new InvalidFontFileException("No Top DICT index found.");

        // 9. Top DICT Data
        // The names of the Top DICT operators shown in
        // Table 9 are, where possible, the same as the corresponding Type 1 dict key.
        // Operators that have no corresponding Type1 dict key are noted
        // in the table below along with a default value, if any. (Several
        // operators have been derived from FontInfo dict keys but have
        // been grouped together with the Top DICT operators for
        // simplicity.The keys from the FontInfo dict are indicated in the
        // Default, notes  column of Table 9)
        return ReadDICTData(reader, offsets[0].Length);
    }

    private string[] ReadStringIndex(BigEndianBinaryReader reader)
    {
        if (!TryReadIndexDataOffsets(reader, out var offsets)) return Array.Empty<string>();

        var stringIndex = new string[offsets.Length];

        // Allow reusing the same buffer for shorter reads.
        using var buffer = new Buffer<byte>(512);
        var bufferSpan = buffer.GetSpan();

        for (var i = 0; i < offsets.Length; ++i)
        {
            var length = offsets[i].Length;
            if (length < bufferSpan.Length)
            {
                var slice = bufferSpan[..length];
                var actualRead = reader.BaseStream.Read(slice);
                if (actualRead != length) throw new InvalidFontFileException("Invalid string length.");

                stringIndex[i] = Iso88591.GetString(slice);
            }
            else
            {
                stringIndex[i] = reader.ReadString(length, Iso88591);
            }
        }

        return stringIndex;
    }

    private string GetSid(int index, string[] stringIndex)
    {
        if (index >= 0 && index <= CffStandardStrings.Count - 1)
            // Use standard name
            return CffStandardStrings.GetName(index);

        if (index - CffStandardStrings.Count < stringIndex.Length) return stringIndex[index - CffStandardStrings.Count];

        // Technically this maps to .notdef, but PDFBox uses this
        return "SID" + index;
    }

    private CffTopDictionary ResolveTopDictInfo(List<CffDataDicEntry> entries, string[] stringIndex)
    {
        // TODO: Is CID mandatory?
        CffTopDictionary metrics = new();
        foreach (var entry in entries)
            switch (entry.Operator.Name)
            {
                default:
#if DEBUG
                    Debug.WriteLine("topdic:" + entry.Operator.Name);
#endif
                    break;
                case "XUID":
                    break; // nothing
                case "version":
                    metrics.Version = GetSid((int)entry.Operands[0].RealNumValue, stringIndex);
                    break;
                case "Notice":
                    metrics.Notice = GetSid((int)entry.Operands[0].RealNumValue, stringIndex);
                    break;
                case "Copyright":
                    metrics.CopyRight = GetSid((int)entry.Operands[0].RealNumValue, stringIndex);
                    break;
                case "FullName":
                    metrics.FullName = GetSid((int)entry.Operands[0].RealNumValue, stringIndex);
                    break;
                case "FamilyName":
                    metrics.FamilyName = GetSid((int)entry.Operands[0].RealNumValue, stringIndex);
                    break;
                case "Weight":
                    metrics.Weight = GetSid((int)entry.Operands[0].RealNumValue, stringIndex);
                    break;
                case "UnderlinePosition":
                    metrics.UnderlinePosition = entry.Operands[0].RealNumValue;
                    break;
                case "UnderlineThickness":
                    metrics.UnderlineThickness = entry.Operands[0].RealNumValue;
                    break;
                case "FontBBox":
                    metrics.FontBBox = new[]
                    {
                        entry.Operands[0].RealNumValue,
                        entry.Operands[1].RealNumValue,
                        entry.Operands[2].RealNumValue,
                        entry.Operands[3].RealNumValue
                    };
                    break;
                case "CharStrings":
                    charStringsOffset = (int)entry.Operands[0].RealNumValue;
                    break;
                case "charset":
                    charsetOffset = (int)entry.Operands[0].RealNumValue;
                    break;
                case "Encoding":
                    encodingOffset = (int)entry.Operands[0].RealNumValue;
                    break;
                case "Private":
                    // private DICT size and offset
                    privateDICTLength = (int)entry.Operands[0].RealNumValue;
                    privateDICTOffset = (int)entry.Operands[1].RealNumValue;
                    break;
                case "ROS":

                    // http://wwwimages.adobe.com/www.adobe.com/content/dam/acom/en/devnet/font/pdfs/5176.CFF.pdf
                    // A CFF CIDFont has the CIDFontName in the Name INDEX and a corresponding Top DICT.
                    // The Top DICT begins with ROS operator which specifies the Registry-Ordering - Supplement for the font.
                    // This will indicate to a CFF parser that special CID processing should be applied to this font. Specifically:
                    // ROS operator combines the Registry, Ordering, and Supplement keys together.
                    // see Adobe Cmap resource , https://github.com/adobe-type-tools/cmap-resources
                    metrics.CidFontInfo.ROS_Register = GetSid((int)entry.Operands[0].RealNumValue, stringIndex);
                    metrics.CidFontInfo.ROS_Ordering = GetSid((int)entry.Operands[1].RealNumValue, stringIndex);
                    metrics.CidFontInfo.ROS_Supplement = GetSid((int)entry.Operands[2].RealNumValue, stringIndex);

                    break;
                case "CIDFontVersion":
                    metrics.CidFontInfo.CIDFontVersion = entry.Operands[0].RealNumValue;
                    break;
                case "CIDCount":
                    metrics.CidFontInfo.CIDFountCount = (int)entry.Operands[0].RealNumValue;
                    break;
                case "FDSelect":
                    metrics.CidFontInfo.FDSelect = (int)entry.Operands[0].RealNumValue;
                    break;
                case "FDArray":
                    metrics.CidFontInfo.FDArray = (int)entry.Operands[0].RealNumValue;
                    break;
            }

        return metrics;
    }

    private byte[][] ReadGlobalSubrIndex(BigEndianBinaryReader reader) // 16. Local / Global Subrs INDEXes
    // Both Type 1 and Type 2 charstrings support the notion of
    // subroutines or subrs.
    // A subr is typically a sequence of charstring
    // bytes representing a sub - program that occurs in more than one
    //  place in a font’s charstring data.
    // This subr may be stored once
    // but referenced many times from within one or more charstrings
    // by the use of the call subr  operator whose operand is the
    // number of the subr to be called.
    // The subrs are local to a  particular font and
    // cannot be shared between fonts.
    // Type 2 charstrings also permit global subrs which function in the same
    // way but are called by the call gsubr operator and may be shared
    // across fonts.
    // Local subrs are stored in an INDEX structure which is located via
    // the offset operand of the Subrs  operator in the Private DICT.
    // A font without local subrs has no Subrs operator in the Private DICT.
    // Global subrs are stored in an INDEX structure which follows the
    // String INDEX. A FontSet without any global subrs is represented
    // by an empty Global Subrs INDEX.
    {
    return ReadSubrBuffer(reader);
    }

    private byte[][] ReadLocalSubrs(BigEndianBinaryReader reader)
    {
        return ReadSubrBuffer(reader);
    }

    // TODO: We don't actually need this right now. Will be important though if we ever introduce subsetting.
    private void ReadEncodings(BigEndianBinaryReader reader)
    {
        // Encoding data is located via the offset operand to the
        // Encoding operator in the Top DICT.

        // Only one Encoding operator can be
        // specified per font except for CIDFonts which specify no
        // encoding.

        // A glyph’s encoding is specified by a 1 - byte code that
        // permits values in the range 0 - 255.

        // Each encoding is described by a format-type identifier byte
        // followed by format-specific data.Two formats are currently
        // defined as specified in Tables 11(Format 0) and 12(Format 1).
        if (encodingOffset != -1)
        {
            var encoding = reader.ReadByte();
            switch (encoding)
            {
                case 0:
                    ReadFormat0Encoding(reader);
                    break;
                case 1:
                    ReadFormat1Encoding(reader);
                    break;
            }
        }
    }

    private void ReadCharsets(BigEndianBinaryReader reader, string[] stringIndex, CffGlyphData[] glyphs)
    {
        // Charset data is located via the offset operand to the
        // charset operator in the Top DICT.

        // Each charset is described by a format-
        // type identifier byte followed by format-specific data.
        // Three formats are currently defined as shown in Tables
        // 17, 18, and 20.
        reader.BaseStream.Position = offset + charsetOffset;
        switch (reader.ReadByte())
        {
            default:
                throw new NotSupportedException();
            case 0:
                ReadCharsetsFormat0(reader, stringIndex, glyphs);
                break;
            case 1:
                ReadCharsetsFormat1(reader, stringIndex, glyphs);
                break;
            case 2:
                ReadCharsetsFormat2(reader, stringIndex, glyphs);
                break;
        }
    }

    private void ReadCharsetsFormat0(BigEndianBinaryReader reader, string[] stringIndex, CffGlyphData[] glyphs)
    {
        // Table 17: Format 0
        // Type     Name                Description
        // Card8     format             =0
        // SID       glyph[nGlyphs-1]   Glyph name array

        // Each element of the glyph array represents the name of the
        // corresponding glyph. This format should be used when the SIDs
        // are in a fairly random order. The number of glyphs (nGlyphs) is
        // the value of the count field in the
        // CharStrings INDEX. (There is
        // one less element in the glyph name array than nGlyphs because
        // the .notdef glyph name is omitted.)
        for (var i = 1; i < glyphs.Length; ++i)
        {
            ref var data = ref glyphs[i];
            data.GlyphName = GetSid(reader.ReadUInt16(), stringIndex);
        }
    }

    private void ReadCharsetsFormat1(BigEndianBinaryReader reader, string[] stringIndex, CffGlyphData[] glyphs)
    {
        // Table 18 Format 1
        // Type     Name                Description
        // Card8        format              =1
        // struct   Range1[<varies>]    Range1 array (see Table  19)

        // Table 19 Range1 Format (Charset)
        // Type      Name          Description
        // SID       first         First glyph in range
        // Card8     nLeft         Glyphs left in range(excluding first)

        // Each Range1 describes a group of sequential SIDs. The number
        // of ranges is not explicitly specified in the font. Instead, software
        // utilizing this data simply processes ranges until all glyphs in the
        // font are covered. This format is particularly suited to charsets
        // that are well ordered
        for (var i = 1; i < glyphs.Length;)
        {
            int sid = reader.ReadUInt16(); // First glyph in range
            var count = reader.ReadByte() + 1; // since it does not include first element.
            do
            {
                ref var data = ref glyphs[i];
                data.GlyphName = GetSid(sid, stringIndex);

                count--;
                i++;
                sid++;
            } while (count > 0);
        }
    }

    private void ReadCharsetsFormat2(BigEndianBinaryReader reader, string[] stringIndex, CffGlyphData[] glyphs)
    {
        // note:eg, Adobe's source-code-pro font

        // Table 20 Format 2
        // Type          Name              Description
        // Card8         format            2
        // struct        Range2[<varies>]  Range2 array (see Table 21)
        //
        //-----------------------------------------------
        // Table 21 Range2 Format
        // Type          Name             Description
        // SID           first            First glyph in range
        // Card16        nLeft            Glyphs left in range (excluding first)
        //-----------------------------------------------

        // Format 2 differs from format 1 only in the size of the nLeft field in each range.
        // This format is most suitable for fonts with a large well - ordered charset — for example, for Asian CIDFonts.
        for (var i = 1; i < glyphs.Length;)
        {
            int sid = reader.ReadUInt16(); // First glyph in range
            var count = reader.ReadUInt16() + 1; // since it does not include first element.
            do
            {
                ref var data = ref glyphs[i];
                data.GlyphName = GetSid(sid, stringIndex);

                count--;
                i++;
                sid++;
            } while (count > 0);
        }
    }

    private void ReadFDSelect(BigEndianBinaryReader reader, CidFontInfo cidFontInfo)
    {
        if (cidFontInfo.FDSelect == 0) return;

        reader.BaseStream.Position = offset + cidFontInfo.FDSelect;
        switch (reader.ReadByte())
        {
            case 0:
                cidFontInfo.FdSelectFormat = 0;
                for (var i = 0; i < cidFontInfo.CIDFountCount; i++) cidFontInfo.FdSelectMap[i] = reader.ReadByte();

                break;

            case 3:
                cidFontInfo.FdSelectFormat = 3;
                var nRanges = reader.ReadUInt16();
                var ranges = new FDRange3[nRanges + 1];

                cidFontInfo.FdSelectFormat = 3;
                cidFontInfo.FdRanges = ranges;
                for (var i = 0; i < nRanges; ++i) ranges[i] = new FDRange3(reader.ReadUInt16(), reader.ReadByte());

                ranges[nRanges] = new FDRange3(reader.ReadUInt16(), 0); // sentinel
                break;

            default:
                throw new NotSupportedException("Only FD Select format 0 and 3 are supported");
        }
    }

    private FontDict[] ReadFDArray(BigEndianBinaryReader reader, CidFontInfo cidFontInfo)
    {
        if (cidFontInfo.FDArray == 0) return Array.Empty<FontDict>();

        reader.BaseStream.Position = this.offset + cidFontInfo.FDArray;

        if (!TryReadIndexDataOffsets(reader, out var offsets)) return Array.Empty<FontDict>();

        var fontDicts = new FontDict[offsets.Length];
        for (var i = 0; i < fontDicts.Length; ++i)
        {
            // read DICT data
            var dic = ReadDICTData(reader, offsets[i].Length);

            // translate
            var offset = 0;
            var size = 0;
            var name = 0;

            foreach (var entry in dic)
                switch (entry.Operator.Name)
                {
                    default:
                        throw new NotSupportedException();
                    case "FontName":
                        name = (int)entry.Operands[0].RealNumValue;
                        break;
                    case "Private": // private dic
                        size = (int)entry.Operands[0].RealNumValue;
                        offset = (int)entry.Operands[1].RealNumValue;
                        break;
                }

            fontDicts[i] = new FontDict(name, size, offset);
        }

        foreach (var fdict in fontDicts)
        {
            reader.BaseStream.Position = offset + fdict.PrivateDicOffset;

            var dicData = ReadDICTData(reader, fdict.PrivateDicSize);

            if (dicData.Count > 0)
                // Interpret the values of private dict
                foreach (var dicEntry in dicData)
                    switch (dicEntry.Operator.Name)
                    {
                        case "Subrs":
                            var localSubrsOffset = (int)dicEntry.Operands[0].RealNumValue;
                            reader.BaseStream.Position = offset + fdict.PrivateDicOffset + localSubrsOffset;
                            fdict.LocalSubr = ReadSubrBuffer(reader);
                            break;

                        case "defaultWidthX":
                        case "nominalWidthX":
                            break;
                    }
        }

        return fontDicts;
    }

    private CffGlyphData[] ReadCharStringsIndex(
        BigEndianBinaryReader reader,
        CffTopDictionary topDictionary,
        byte[][] globalSubrBuffers,
        FontDict[] fontDicts,
        CffPrivateDictionary? privateDictionary)
    {
        // 14. CharStrings INDEX

        // This contains the charstrings of all the glyphs in a font stored in
        // an INDEX structure.

        // Charstring objects contained within this
        // INDEX are accessed by GID.

        // The first charstring(GID 0) must be
        // the.notdef glyph.

        // The number of glyphs available in a font may
        // be determined from the count field in the INDEX.

        //

        // The format of the charstring data, and therefore the method of
        // interpretation, is specified by the
        // CharstringType  operator in the Top DICT.

        // The CharstringType operator has a default value
        // of 2 indicating the Type 2 charstring format which was designed
        // in conjunction with CFF.

        // Type 1 charstrings are documented in
        // the “Adobe Type 1 Font Format” published by Addison - Wesley.

        // Type 2 charstrings are described in Adobe Technical Note #5177:
        // “Type 2 Charstring Format.” Other charstring types may also be
        // supported by this method.
        reader.BaseStream.Position = this.offset + charStringsOffset;
        if (!TryReadIndexDataOffsets(reader, out var offsets))
            throw new InvalidFontFileException("No glyph data found.");

        var glyphCount = offsets.Length;
        var glyphs = new CffGlyphData[glyphCount];
        var localSubBuffer = privateDictionary?.LocalSubrRawBuffers;

        // Is the font a CID font?
        FDRangeProvider fdRangeProvider = new(topDictionary.CidFontInfo);
        var isCidFont = topDictionary.CidFontInfo.FdRanges.Length > 0;

        for (var i = 0; i < glyphCount; ++i)
        {
            var offset = offsets[i];
            var charstringsBuffer = reader.ReadBytes(offset.Length);

            // Now we can parse the raw glyph instructions
            if (isCidFont)
            {
                // Select  proper local private dict
                fdRangeProvider.SetCurrentGlyphIndex((ushort)i);
                localSubBuffer = fontDicts[fdRangeProvider.SelectedFDArray].LocalSubr;
            }

            glyphs[i] = new CffGlyphData(
                (ushort)i,
                globalSubrBuffers,
                localSubBuffer ?? Array.Empty<byte[]>(),
                privateDictionary?.NominalWidthX ?? 0,
                charstringsBuffer);
        }

        return glyphs;
    }

    private CffPrivateDictionary? ReadPrivateDict(BigEndianBinaryReader reader)
    {
        // per-font
        if (privateDICTLength == 0) return null;

        reader.BaseStream.Position = offset + privateDICTOffset;
        var dicData = ReadDICTData(reader, privateDICTLength);
        var localSubrRawBuffers = Array.Empty<byte[]>();
        var defaultWidthX = 0;
        var nominalWidthX = 0;

        if (dicData.Count > 0)
            // Interpret the values of private dict
            foreach (var dicEntry in dicData)
                switch (dicEntry.Operator.Name)
                {
                    case "Subrs":
                        var localSubrsOffset = (int)dicEntry.Operands[0].RealNumValue;
                        reader.BaseStream.Position = offset + privateDICTOffset + localSubrsOffset;
                        localSubrRawBuffers = ReadLocalSubrs(reader);
                        break;

                    case "defaultWidthX":
                        defaultWidthX = (int)dicEntry.Operands[0].RealNumValue;
                        break;

                    case "nominalWidthX":
                        nominalWidthX = (int)dicEntry.Operands[0].RealNumValue;
                        break;
                }

        return new CffPrivateDictionary(localSubrRawBuffers, defaultWidthX, nominalWidthX);
    }

    private byte[][] ReadSubrBuffer(BigEndianBinaryReader reader)
    {
        if (!TryReadIndexDataOffsets(reader, out var offsets)) return Array.Empty<byte[]>();

        var rawBufferList = new byte[offsets.Length][];

        for (var i = 0; i < rawBufferList.Length; ++i)
        {
            var offset = offsets[i];
            rawBufferList[i] = reader.ReadBytes(offset.Length);
        }

        return rawBufferList;
    }

    private List<CffDataDicEntry> ReadDICTData(BigEndianBinaryReader reader, int length)
    {
        // 4. DICT Data

        // Font dictionary data comprising key-value pairs is represented
        // in a compact tokenized format that is similar to that used to
        // represent Type 1 charstrings.

        // Dictionary keys are encoded as 1- or 2-byte operators and dictionary values are encoded as
        // variable-size numeric operands that represent either integer or
        // real values.

        //-----------------------------
        // A DICT is simply a sequence of
        // operand(s)/operator bytes concatenated together.
        var maxIndex = (int)(reader.BaseStream.Position + length);
        List<CffDataDicEntry> dicData = new();
        while (reader.BaseStream.Position < maxIndex)
        {
            var dicEntry = ReadEntry(reader);
            dicData.Add(dicEntry);
        }

        return dicData;
    }

    private CffDataDicEntry ReadEntry(BigEndianBinaryReader reader)
    {
        List<CffOperand> operands = new();

        //-----------------------------
        // An operator is preceded by the operand(s) that
        // specify its value.
        //--------------------------------

        //-----------------------------
        // Operators and operands may be distinguished by inspection of
        // their first byte:
        // 0–21 specify operators and
        // 28, 29, 30, and 32–254 specify operands(numbers).
        // Byte values 22–27, 31, and 255 are reserved.

        // An operator may be preceded by up to a maximum of 48 operands
        CFFOperator? @operator;
        while (true)
        {
            var b0 = reader.ReadByte();

            if (b0 is >= 0 and <= 21)
            {
                // operators
                @operator = ReadOperator(reader, b0);
                break; // **break after found operator
            }

            if (b0 is 28 or 29)
            {
                var num = ReadIntegerNumber(reader, b0);
                operands.Add(new CffOperand(num, OperandKind.IntNumber));
            }
            else if (b0 == 30)
            {
                var num = ReadRealNumber(reader);
                operands.Add(new CffOperand(num, OperandKind.RealNumber));
            }
            else if (b0 is >= 32 and <= 254)
            {
                var num = ReadIntegerNumber(reader, b0);
                operands.Add(new CffOperand(num, OperandKind.IntNumber));
            }
            else
            {
                throw new NotSupportedException("invalid DICT data b0 byte: " + b0);
            }
        }

        // I'm fairly confident that the operator can never be null.
        return new CffDataDicEntry(@operator!, operands.ToArray());
    }

    private CFFOperator ReadOperator(BigEndianBinaryReader reader, byte b0)
    {
        // read operator key
        byte b1 = 0;
        if (b0 == 12)
            // 2 bytes
            b1 = reader.ReadByte();

        // get registered operator by its key
        return CFFOperator.GetOperatorByKey(b0, b1);
    }

    private double ReadRealNumber(BigEndianBinaryReader reader)
    {
        // from https://typekit.files.wordpress.com/2013/05/5176.cff.pdf
        // A real number operand is provided in addition to integer
        // operands.This operand begins with a byte value of 30 followed
        // by a variable-length sequence of bytes.Each byte is composed
        // of two 4 - bit nibbles asdefined in Table 5.

        // The first nibble of a
        // pair is stored in the most significant 4 bits of a byte and the
        // second nibble of a pair is stored in the least significant 4 bits of a byte
        var sb = pooledStringBuilder;
        sb.Clear(); // reset

        var done = false;
        var exponentMissing = false;
        while (!done)
        {
            int b = reader.ReadByte();

            var nb_0 = (b >> 4) & 0xf;
            var nb_1 = b & 0xf;

            for (var i = 0; !done && i < 2; ++i)
            {
                var nibble = i == 0 ? nb_0 : nb_1;

                switch (nibble)
                {
                    case 0x0:
                    case 0x1:
                    case 0x2:
                    case 0x3:
                    case 0x4:
                    case 0x5:
                    case 0x6:
                    case 0x7:
                    case 0x8:
                    case 0x9:
                        sb.Append(nibble);
                        exponentMissing = false;
                        break;
                    case 0xa:
                        sb.Append('.');
                        break;
                    case 0xb:
                        sb.Append('E');
                        exponentMissing = true;
                        break;
                    case 0xc:
                        sb.Append("E-");
                        exponentMissing = true;
                        break;
                    case 0xd:
                        break;
                    case 0xe:
                        sb.Append('-');
                        break;
                    case 0xf:
                        done = true;
                        break;
                    default:
                        throw new Exception("IllegalArgumentException");
                }
            }
        }

        if (exponentMissing)
            // the exponent is missing, just append "0" to avoid an exception
            // not sure if 0 is the correct value, but it seems to fit
            // see PDFBOX-1522
            sb.Append('0');

        if (sb.Length == 0) return 0d;

        if (!double.TryParse(
                sb.ToString(),
                NumberStyles.Number | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture,
                out var value))
            throw new NotSupportedException();

        return value;
    }

    private int ReadIntegerNumber(BigEndianBinaryReader reader, byte b0)
    {
        if (b0 == 28) return reader.ReadInt16();

        if (b0 == 29) return reader.ReadInt32();

        if (b0 is >= 32 and <= 246) return b0 - 139;

        if (b0 is >= 247 and <= 250)
        {
            int b1 = reader.ReadByte();
            return (b0 - 247) * 256 + b1 + 108;
        }

        if (b0 is >= 251 and <= 254)
        {
            int b1 = reader.ReadByte();
            return -(b0 - 251) * 256 - b1 - 108;
        }

        throw new InvalidFontFileException("Invalid DICT data b0 byte: " + b0);
    }

    private bool TryReadIndexDataOffsets(BigEndianBinaryReader reader, [NotNullWhen(true)] out CffIndexOffset[]? value)
    {
        // INDEX Data
        // An INDEX is an array of variable-sized objects.It comprises a
        // header, an offset array, and object data.
        // The offset array specifies offsets within the object data.
        // An object is retrieved by
        // indexing the offset array and fetching the object at the
        // specified offset.
        // The object’s length can be determined by subtracting its offset
        // from the next offset in the offset array.
        // An additional offset is added at the end of the offset array so the
        // length of the last object may be determined.
        // The INDEX format is shown in Table 7

        // Table 7 INDEX Format
        // Type        Name                  Description
        // Card16      count                 Number of objects stored in INDEX
        // OffSize     offSize               Offset array element size
        // Offset      offset[count + 1]     Offset array(from byte preceding object data)
        // Card8       data[<varies>]        Object data

        // Offsets in the offset array are relative to the byte that precedes
        // the object data. Therefore the first element of the offset array
        // is always 1. (This ensures that every object has a corresponding
        // offset which is always nonzero and permits the efficient
        // implementation of dynamic object loading.)

        // An empty INDEX is represented by a count field with a 0 value
        // and no additional fields.Thus, the total size of an empty INDEX
        // is 2 bytes.

        // Note 2
        // An INDEX may be skipped by jumping to the offset specified by the last
        // element of the offset array
        var count = reader.ReadUInt16();
        if (count == 0)
        {
            value = null;
            return false;
        }

        int offSize = reader.ReadByte();
        var offsets = new int[count + 1];
        var indexElems = new CffIndexOffset[count];
        for (var i = 0; i <= count; ++i) offsets[i] = reader.ReadOffset(offSize);

        for (var i = 0; i < count; ++i) indexElems[i] = new CffIndexOffset(offsets[i], offsets[i + 1] - offsets[i]);

        value = indexElems;
        return true;
    }

#pragma warning disable IDE0059

    private void ReadFormat0Encoding(BigEndianBinaryReader reader)
    {
        // Table 11: Format 0
        // Type      Name            Description
        // Card8     format          = 0
        // Card8     nCodes          Number of encoded glyphs
        // Card8     code[nCodes]    Code array
        //-------
        // Each element of the code array represents the encoding for the
        // corresponding glyph. This format should be used when the
        // codes are in a fairly random order

        // we have read format field( 1st field) ..
        // so start with 2nd field
        int nCodes = reader.ReadByte();
        var codes = reader.ReadBytes(nCodes);

        // TODO: Implement based on PDFPig
    }

    private void ReadFormat1Encoding(BigEndianBinaryReader reader)
    {
        // Table 12 Format 1
        // Type      Name              Description
        // Card8     format             = 1
        // Card8     nRanges           Number of code ranges
        // struct    Range1[nRanges]   Range1 array(see Table  13)
        //--------------
        int nRanges = reader.ReadByte();

        // Table 13 Range1 Format(Encoding)
        // Type        Name        Description
        // Card8       first       First code in range
        // Card8       nLeft       Codes left in range(excluding first)
        //--------------
        // Each Range1 describes a group of sequential codes. For
        // example, the codes 51 52 53 54 55 could be represented by the
        // Range1: 51 4, and a perfectly ordered encoding of 256 codes can
        // be described with the Range1: 0 255.

        // This format is particularly suited to encodings that are well ordered.

        // A few fonts have multiply - encoded glyphs which are not
        // supported directly by any of the above formats. This situation is
        // indicated by setting the high - order bit in the format byte and
        // supplementing the encoding, regardless of format type, as
        // shown in Table 14.

        // Table 14 Supplemental Encoding Data
        // Type         Name                Description
        // Card8        nSups               Number of supplementary mappings
        // struct    Supplement[nSups]   Supplementary encoding array(see Table  15 below)

        // Table 15 Supplement Format
        // Type      Name        Description
        // Card8     code        Encoding
        // SID       glyph       Name
    }

#pragma warning restore IDE0059
}