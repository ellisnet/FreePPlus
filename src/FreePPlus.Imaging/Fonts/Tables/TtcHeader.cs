// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables;

//was previously: namespace SixLabors.Fonts.Tables;

/// <summary>
///     Represents a font collection header (for .ttc font collections).
///     A font collection contains one or more fonts where typically the glyf table is shared by multiple fonts to save
///     space,
///     but other tables are not.
///     Each font in the collection has its own set of tables.
/// </summary>
internal class TtcHeader
{
    internal const string TableName = "ttcf";

    public TtcHeader(string ttcTag, ushort majorVersion, ushort minorVersion, uint numFonts, uint[] offsetTable,
        uint dsigTag, uint dsigLength, uint dsigOffset)
    {
        TtcTag = ttcTag;
        MajorVersion = majorVersion;
        MinorVersion = minorVersion;
        NumFonts = numFonts;
        OffsetTable = offsetTable;
        DsigTag = dsigTag;
        DsigLength = dsigLength;
        DsigOffset = dsigOffset;
    }

    /// <summary>
    ///     Gets the tag, should be "ttcf".
    /// </summary>
    public string TtcTag { get; }

    public ushort MajorVersion { get; }

    public ushort MinorVersion { get; }

    public uint NumFonts { get; }

    /// <summary>
    ///     Gets the array of offsets to the OffsetTable of each font. Use <see cref="FontReader" /> for each font.
    /// </summary>
    public uint[] OffsetTable { get; }

    public uint DsigTag { get; }

    public uint DsigLength { get; }

    public uint DsigOffset { get; }

    public static TtcHeader Read(BigEndianBinaryReader reader)
    {
        var tag = reader.ReadTag();

        if (tag != TableName) throw new InvalidFontTableException($"Expected tag = {TableName} found {tag}", TableName);

        var majorVersion = reader.ReadUInt16();
        var minorVersion = reader.ReadUInt16();
        var numFonts = reader.ReadUInt32();
        var offsetTable = new uint[numFonts];
        for (var i = 0; i < numFonts; ++i) offsetTable[i] = reader.ReadOffset32();

        // Version 2 fields
        uint dsigTag = 0;
        uint dsigLength = 0;
        uint dsigOffset = 0;
        if (majorVersion >= 2)
        {
            dsigTag = reader.ReadUInt32();
            dsigLength = reader.ReadUInt32();
            dsigOffset = reader.ReadUInt32();
        }

        return new TtcHeader(tag, majorVersion, minorVersion, numFonts, offsetTable, dsigTag, dsigLength, dsigOffset);
    }
}