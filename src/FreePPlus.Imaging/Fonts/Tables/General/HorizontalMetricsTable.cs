// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.General;

//was previously: namespace SixLabors.Fonts.Tables.General;

internal sealed class HorizontalMetricsTable : Table
{
    internal const string TableName = "hmtx";
    private readonly ushort[] advancedWidths;
    private readonly short[] leftSideBearings;

    public HorizontalMetricsTable(ushort[] advancedWidths, short[] leftSideBearings)
    {
        this.advancedWidths = advancedWidths;
        this.leftSideBearings = leftSideBearings;
    }

    public ushort GetAdvancedWidth(int glyphIndex)
    {
        if (glyphIndex >= advancedWidths.Length) return advancedWidths[0];

        return advancedWidths[glyphIndex];
    }

    internal short GetLeftSideBearing(int glyphIndex)
    {
        if (glyphIndex >= leftSideBearings.Length) return leftSideBearings[0];

        return leftSideBearings[glyphIndex];
    }

    public static HorizontalMetricsTable Load(FontReader reader)
    {
        // you should load all dependent tables prior to manipulating the reader
        var headTable = reader.GetTable<HorizontalHeadTable>();
        var profileTable = reader.GetTable<MaximumProfileTable>();

        // Move to start of table
        using var binaryReader = reader.GetReaderAtTablePosition(TableName);
        return Load(binaryReader, headTable.NumberOfHMetrics, profileTable.GlyphCount);
    }

    public static HorizontalMetricsTable Load(BigEndianBinaryReader reader, int metricCount, int glyphCount)
    {
        // Type           | Name                                          | Description
        // longHorMetric  | hMetrics[numberOfHMetrics]                    | Paired advance width and left side bearing values for each glyph. Records are indexed by glyph ID.
        // int16          | leftSideBearing[numGlyphs - numberOfHMetrics] | Left side bearings for glyph IDs greater than or equal to numberOfHMetrics.
        var bearingCount = glyphCount - metricCount;
        var advancedWidth = new ushort[metricCount];
        var leftSideBearings = new short[glyphCount];

        for (var i = 0; i < metricCount; i++)
        {
            // longHorMetric Record:
            // Type   | Name         | Description
            // uint16 | advanceWidth | Glyph advance width, in font design units.
            // int16  | lsb          | Glyph left side bearing, in font design units.
            advancedWidth[i] = reader.ReadUInt16();
            leftSideBearings[i] = reader.ReadInt16();
        }

        for (var i = 0; i < bearingCount; i++) leftSideBearings[metricCount + i] = reader.ReadInt16();

        return new HorizontalMetricsTable(advancedWidth, leftSideBearings);
    }
}