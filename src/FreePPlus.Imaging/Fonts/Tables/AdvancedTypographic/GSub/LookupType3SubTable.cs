// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GSub;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GSub;

/// <summary>
///     An Alternate Substitution (AlternateSubst) subtable identifies any number of aesthetic alternatives
///     from which a user can choose a glyph variant to replace the input glyph.
///     <see
///         href="https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#lookuptype-3-alternate-substitution-subtable" />
/// </summary>
internal static class LookupType3SubTable
{
    public static LookupSubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        reader.Seek(offset, SeekOrigin.Begin);
        var substFormat = reader.ReadUInt16();

        return substFormat switch
        {
            1 => LookupType3Format1SubTable.Load(reader, offset, lookupFlags),
            _ => new NotImplementedSubTable()
        };
    }
}

internal sealed class LookupType3Format1SubTable : LookupSubTable
{
    private readonly AlternateSetTable[] alternateSetTables;
    private readonly CoverageTable coverageTable;

    private LookupType3Format1SubTable(AlternateSetTable[] alternateSetTables, CoverageTable coverageTable,
        LookupFlags lookupFlags)
        : base(lookupFlags)
    {
        this.alternateSetTables = alternateSetTables;
        this.coverageTable = coverageTable;
    }

    public static LookupType3Format1SubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        // Alternate Substitution Format 1
        // +----------+----------------------------------------+---------------------------------------------------------------+
        // | Type     | Name                                   | Description                                                   |
        // +==========+========================================+===============================================================+
        // | uint16   | substFormat                            | Format identifier: format = 1                                 |
        // +----------+----------------------------------------+---------------------------------------------------------------+
        // | Offset16 | coverageOffset                         | Offset to Coverage table, from beginning of substitution      |
        // |          |                                        | subtable                                                      |
        // +----------+----------------------------------------+---------------------------------------------------------------+
        // | uint16   | alternateSetCount                      | Number of AlternateSet tables                                 |
        // +----------+----------------------------------------+---------------------------------------------------------------+
        // | Offset16 | alternateSetOffsets[alternateSetCount] | Array of offsets to AlternateSet tables. Offsets are from     |
        // |          |                                        | beginning of substitution subtable, ordered by Coverage index |
        // +----------+----------------------------------------+---------------------------------------------------------------+
        var coverageOffset = reader.ReadOffset16();
        var alternateSetCount = reader.ReadUInt16();

        using Buffer<ushort> alternateSetOffsetsBuffer = new(alternateSetCount);
        var alternateSetOffsets = alternateSetOffsetsBuffer.GetSpan();
        reader.ReadUInt16Array(alternateSetOffsets);

        var alternateTables = new AlternateSetTable[alternateSetCount];
        for (var i = 0; i < alternateTables.Length; i++)
        {
            // AlternateSet Table
            // +--------+-------------------------------+----------------------------------------------------+
            // | Type   | Name                          | Description                                        |
            // +========+===============================+====================================================+
            // | uint16 | glyphCount                    | Number of glyph IDs in the alternateGlyphIDs array |
            // +--------+-------------------------------+----------------------------------------------------+
            // | uint16 | alternateGlyphIDs[glyphCount] | Array of alternate glyph IDs, in arbitrary order   |
            // +--------+-------------------------------+----------------------------------------------------+
            reader.Seek(offset + alternateSetOffsets[i], SeekOrigin.Begin);
            var glyphCount = reader.ReadUInt16();
            alternateTables[i] = new AlternateSetTable(reader.ReadUInt16Array(glyphCount));
        }

        var coverageTable = CoverageTable.Load(reader, offset + coverageOffset);

        return new LookupType3Format1SubTable(alternateTables, coverageTable, lookupFlags);
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
            // TODO: We're just choosing the first alternative here.
            // It looks like the choice is arbitrary and should be determined by
            // the client.
            collection.Replace(index, alternateSetTables[offset].AlternateGlyphs[0]);
            return true;
        }

        return false;
    }

    public readonly struct AlternateSetTable
    {
        public AlternateSetTable(ushort[] alternateGlyphs)
        {
            AlternateGlyphs = alternateGlyphs;
        }

        public readonly ushort[] AlternateGlyphs { get; }
    }
}