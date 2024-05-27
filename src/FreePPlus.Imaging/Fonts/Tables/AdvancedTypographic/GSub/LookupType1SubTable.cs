// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GSub;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GSub;

/// <summary>
///     Single substitution (SingleSubst) subtables tell a client to replace a single glyph with another glyph.
///     The subtables can be either of two formats. Both formats require two distinct sets of glyph indices:
///     one that defines input glyphs (specified in the Coverage table), and one that defines the output glyphs.
///     Format 1 requires less space than Format 2, but it is less flexible.
///     <see
///         href="https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#lookuptype-1-single-substitution-subtable" />
/// </summary>
internal static class LookupType1SubTable
{
    public static LookupSubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        reader.Seek(offset, SeekOrigin.Begin);
        var substFormat = reader.ReadUInt16();

        return substFormat switch
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
    private readonly ushort deltaGlyphId;

    private LookupType1Format1SubTable(ushort deltaGlyphId, CoverageTable coverageTable, LookupFlags lookupFlags)
        : base(lookupFlags)
    {
        this.deltaGlyphId = deltaGlyphId;
        this.coverageTable = coverageTable;
    }

    public static LookupType1Format1SubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        // SingleSubstFormat1
        // +----------+----------------+----------------------------------------------------------+
        // | Type     | Name           | Description                                              |
        // +==========+================+==========================================================+
        // | uint16   | substFormat    | Format identifier: format = 1                            |
        // +----------+----------------+----------------------------------------------------------+
        // | Offset16 | coverageOffset | Offset to Coverage table, from beginning of substitution |
        // |          |                | subtable                                                 |
        // +----------+----------------+----------------------------------------------------------+
        // | int16    | deltaGlyphID   | Add to original glyph ID to get substitute glyph ID      |
        // +----------+----------------+----------------------------------------------------------+
        var coverageOffset = reader.ReadOffset16();
        var deltaGlyphId = reader.ReadUInt16();
        var coverageTable = CoverageTable.Load(reader, offset + coverageOffset);

        return new LookupType1Format1SubTable(deltaGlyphId, coverageTable, lookupFlags);
    }

    public override bool TrySubstitution(
        FontMetrics fontMetrics,
        GSubTable table,
        GlyphSubstitutionCollection collection,
        Tag feature,
        int index,
        int count)
    {
        var glyphId = collection[index];
        if (glyphId == 0) return false;

        if (coverageTable.CoverageIndexOf(glyphId) > -1)
        {
            collection.Replace(index, (ushort)(glyphId + deltaGlyphId));
            return true;
        }

        return false;
    }
}

internal sealed class LookupType1Format2SubTable : LookupSubTable
{
    private readonly CoverageTable coverageTable;
    private readonly ushort[] substituteGlyphs;

    private LookupType1Format2SubTable(ushort[] substituteGlyphs, CoverageTable coverageTable, LookupFlags lookupFlags)
        : base(lookupFlags)
    {
        this.substituteGlyphs = substituteGlyphs;
        this.coverageTable = coverageTable;
    }

    public static LookupType1Format2SubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        // SingleSubstFormat2
        // +----------+--------------------------------+-----------------------------------------------------------+
        // | Type     | Name                           | Description                                               |
        // +==========+================================+===========================================================+
        // | uint16   | substFormat                    | Format identifier: format = 2                             |
        // +----------+--------------------------------+-----------------------------------------------------------+
        // | Offset16 | coverageOffset                 | Offset to Coverage table, from beginning of substitution  |
        // |          |                                | subtable                                                  |
        // +----------+--------------------------------+-----------------------------------------------------------+
        // | uint16   | glyphCount                     | Number of glyph IDs in the substituteGlyphIDs array       |
        // +----------+--------------------------------+-----------------------------------------------------------+
        // | uint16   | substituteGlyphIDs[glyphCount] | Array of substitute glyph IDs — ordered by Coverage index |
        // +----------+--------------------------------+-----------------------------------------------------------+
        var coverageOffset = reader.ReadOffset16();
        var glyphCount = reader.ReadUInt16();
        var substituteGlyphIds = reader.ReadUInt16Array(glyphCount);
        var coverageTable = CoverageTable.Load(reader, offset + coverageOffset);

        return new LookupType1Format2SubTable(substituteGlyphIds, coverageTable, lookupFlags);
    }

    public override bool TrySubstitution(
        FontMetrics fontMetrics,
        GSubTable table,
        GlyphSubstitutionCollection collection,
        Tag feature,
        int index,
        int count)
    {
        var glyphId = collection[index];
        if (glyphId == 0) return false;

        var offset = coverageTable.CoverageIndexOf(glyphId);

        if (offset > -1)
        {
            collection.Replace(index, substituteGlyphs[offset]);
            return true;
        }

        return false;
    }
}