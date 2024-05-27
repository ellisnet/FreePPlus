// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

/// <summary>
///     A single adjustment positioning subtable (SinglePos) is used to adjust the placement or advance of a single glyph,
///     such as a subscript or superscript. In addition, a SinglePos subtable is commonly used to implement lookup data for
///     contextual positioning.
///     A SinglePos subtable will have one of two formats: one that applies the same adjustment to a series of
///     glyphs(Format 1),
///     and one that applies a different adjustment for each unique glyph(Format 2).
///     <see
///         href="https://docs.microsoft.com/en-us/typography/opentype/spec/gpos#lookup-type-1-single-adjustment-positioning-subtable" />
/// </summary>
internal static class LookupType1SubTable
{
    public static LookupSubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        reader.Seek(offset, SeekOrigin.Begin);
        var posFormat = reader.ReadUInt16();

        return posFormat switch
        {
            1 => LookupType1Format1SubTable.Load(reader, offset, lookupFlags),
            2 => LookupType1Format2SubTable.Load(reader, offset, lookupFlags),
            _ => new NotImplementedSubTable()
        };
    }
}

internal sealed class LookupType1Format1SubTable : LookupSubTable
{
    private readonly CoverageTable coverageTable;
    private readonly ValueRecord valueRecord;

    private LookupType1Format1SubTable(ValueRecord valueRecord, CoverageTable coverageTable, LookupFlags lookupFlags)
        : base(lookupFlags)
    {
        this.valueRecord = valueRecord;
        this.coverageTable = coverageTable;
    }

    public static LookupType1Format1SubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        // SinglePosFormat1
        // +-------------+----------------+-----------------------------------------------+
        // | Type        | Name           | Description                                   |
        // +=============+================+===============================================+
        // | uint16      | posFormat      | Format identifier: format = 1                 |
        // +-------------+----------------+-----------------------------------------------+
        // | Offset16    | coverageOffset | Offset to Coverage table, from beginning      |
        // |             |                | of SinglePos subtable.                        |
        // +-------------+----------------+-----------------------------------------------+
        // | uint16      | valueFormat    | Defines the types of data in the ValueRecord. |
        // +-------------+----------------+-----------------------------------------------+
        // | ValueRecord | valueRecord    | Defines positioning value(s) — applied to     |
        // |             |                | all glyphs in the Coverage table.             |
        // +-------------+----------------+-----------------------------------------------+
        var coverageOffset = reader.ReadOffset16();
        var valueFormat = reader.ReadUInt16<ValueFormat>();
        var valueRecord = new ValueRecord(reader, valueFormat);

        var coverageTable = CoverageTable.Load(reader, offset + coverageOffset);

        return new LookupType1Format1SubTable(valueRecord, coverageTable, lookupFlags);
    }

    public override bool TryUpdatePosition(
        FontMetrics fontMetrics,
        GPosTable table,
        GlyphPositioningCollection collection,
        Tag feature,
        int index,
        int count)
    {
        var glyphId = collection[index];
        if (glyphId == 0) return false;

        var coverage = coverageTable.CoverageIndexOf(glyphId);
        if (coverage > -1)
        {
            var record = valueRecord;
            AdvancedTypographicUtils.ApplyPosition(collection, index, record);

            return true;
        }

        return false;
    }
}

internal sealed class LookupType1Format2SubTable : LookupSubTable
{
    private readonly CoverageTable coverageTable;
    private readonly ValueRecord[] valueRecords;

    private LookupType1Format2SubTable(ValueRecord[] valueRecords, CoverageTable coverageTable, LookupFlags lookupFlags)
        : base(lookupFlags)
    {
        this.valueRecords = valueRecords;
        this.coverageTable = coverageTable;
    }

    public static LookupType1Format2SubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        // SinglePosFormat2
        // +-------------+--------------------------+-----------------------------------------------+
        // |    Type     |   Name                   | Description                                   |
        // +=============+==========================+===============================================+
        // |    uint16   |   posFormat              | Format identifier: format = 2                 |
        // +-------------+--------------------------+-----------------------------------------------+
        // | Offset16    | coverageOffset           | Offset to Coverage table, from beginning      |
        // |             |                          | of SinglePos subtable.                        |
        // +-------------+--------------------------+-----------------------------------------------+
        // | uint16      | valueFormat              | Defines the types of data in the ValueRecords.|
        // +-------------+--------------------------+-----------------------------------------------+
        // | uint16      | valueCount               | Number of ValueRecords — must equal glyphCount|
        // |             |                          | in the Coverage table.                        |
        // | ValueRecord | valueRecords[valueCount] | Array of ValueRecords — positioning values    |
        // |             |                          | applied to glyphs.                            |
        // +-------------+--------------------------+-----------------------------------------------+
        var coverageOffset = reader.ReadOffset16();
        var valueFormat = reader.ReadUInt16<ValueFormat>();
        var valueCount = reader.ReadUInt16();
        var valueRecords = new ValueRecord[valueCount];
        for (var i = 0; i < valueCount; i++) valueRecords[i] = new ValueRecord(reader, valueFormat);

        var coverageTable = CoverageTable.Load(reader, offset + coverageOffset);

        return new LookupType1Format2SubTable(valueRecords, coverageTable, lookupFlags);
    }

    public override bool TryUpdatePosition(
        FontMetrics fontMetrics,
        GPosTable table,
        GlyphPositioningCollection collection,
        Tag feature,
        int index,
        int count)
    {
        var glyphId = collection[index];
        if (glyphId == 0) return false;

        var coverage = coverageTable.CoverageIndexOf(glyphId);
        if (coverage > -1)
        {
            var record = valueRecords[coverage];
            AdvancedTypographicUtils.ApplyPosition(collection, index, record);

            return true;
        }

        return false;
    }
}