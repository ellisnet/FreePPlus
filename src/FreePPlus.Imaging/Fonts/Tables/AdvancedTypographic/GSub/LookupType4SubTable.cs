// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GSub;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GSub;

/// <summary>
///     A Ligature Substitution (LigatureSubst) subtable identifies ligature substitutions where a single glyph replaces
///     multiple glyphs.
///     One LigatureSubst subtable can specify any number of ligature substitutions.
///     The subtable has one format: LigatureSubstFormat1.
///     <see
///         href="https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#lookuptype-4-ligature-substitution-subtable" />
/// </summary>
internal static class LookupType4SubTable
{
    public static LookupSubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        reader.Seek(offset, SeekOrigin.Begin);
        var substFormat = reader.ReadUInt16();

        return substFormat switch
        {
            1 => LookupType4Format1SubTable.Load(reader, offset, lookupFlags),
            _ => new NotImplementedSubTable()
        };
    }
}

internal sealed class LookupType4Format1SubTable : LookupSubTable
{
    private readonly CoverageTable coverageTable;
    private readonly LigatureSetTable[] ligatureSetTables;

    private LookupType4Format1SubTable(LigatureSetTable[] ligatureSetTables, CoverageTable coverageTable,
        LookupFlags lookupFlags)
        : base(lookupFlags)
    {
        this.ligatureSetTables = ligatureSetTables;
        this.coverageTable = coverageTable;
    }

    public static LookupType4Format1SubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        // Ligature Substitution Format 1
        // +----------+--------------------------------------+--------------------------------------------------------------------+
        // | Type     | Name                                 | Description                                                        |
        // +==========+======================================+====================================================================+
        // | uint16   | substFormat                          | Format identifier: format = 1                                      |
        // +----------+--------------------------------------+--------------------------------------------------------------------+
        // | Offset16 | coverageOffset                       | Offset to Coverage table, from beginning of substitution           |
        // |          |                                      | subtable                                                           |
        // +----------+--------------------------------------+--------------------------------------------------------------------+
        // | uint16   | ligatureSetCount                     | Number of LigatureSet tables                                       |
        // +----------+--------------------------------------+--------------------------------------------------------------------+
        // | Offset16 | ligatureSetOffsets[ligatureSetCount] | Array of offsets to LigatureSet tables. Offsets are from beginning |
        // |          |                                      | of substitution subtable, ordered by Coverage index                |
        // +----------+--------------------------------------+--------------------------------------------------------------------+
        var coverageOffset = reader.ReadOffset16();
        var ligatureSetCount = reader.ReadUInt16();

        using Buffer<ushort> ligatureSetOffsetsBuffer = new(ligatureSetCount);
        var ligatureSetOffsets = ligatureSetOffsetsBuffer.GetSpan();
        reader.ReadUInt16Array(ligatureSetOffsets);

        var ligatureSetTables = new LigatureSetTable[ligatureSetCount];
        for (var i = 0; i < ligatureSetTables.Length; i++)
        {
            // LigatureSet Table
            // +----------+--------------------------------+--------------------------------------------------------------------+
            // | Type     | Name                           | Description                                                        |
            // +==========+================================+====================================================================+
            // | uint16   | ligatureCount                  | Number of Ligature tables                                          |
            // +----------+--------------------------------+--------------------------------------------------------------------+
            // | Offset16 | ligatureOffsets[LigatureCount] | Array of offsets to Ligature tables. Offsets are from beginning of |
            // |          |                                | LigatureSet table, ordered by preference.                          |
            // +----------+--------------------------------+--------------------------------------------------------------------+
            var ligatureSetOffset = offset + ligatureSetOffsets[i];
            reader.Seek(ligatureSetOffset, SeekOrigin.Begin);
            var ligatureCount = reader.ReadUInt16();

            using Buffer<ushort> ligatureOffsetsBuffer = new(ligatureCount);
            var ligatureOffsets = ligatureOffsetsBuffer.GetSpan();
            reader.ReadUInt16Array(ligatureOffsets);

            var ligatureTables = new LigatureTable[ligatureCount];

            // Ligature Table
            // +--------+---------------------------------------+------------------------------------------------------+
            // | Type   | Name                                  | Description                                          |
            // +========+=======================================+======================================================+
            // | uint16 | ligatureGlyph                         | glyph ID of ligature to substitute                   |
            // +--------+---------------------------------------+------------------------------------------------------+
            // | uint16 | componentCount                        | Number of components in the ligature                 |
            // +--------+---------------------------------------+------------------------------------------------------+
            // | uint16 | componentGlyphIDs[componentCount - 1] | Array of component glyph IDs — start with the second |
            // |        |                                       | component, ordered in writing direction              |
            // +--------+---------------------------------------+------------------------------------------------------+
            for (var j = 0; j < ligatureTables.Length; j++)
            {
                reader.Seek(ligatureSetOffset + ligatureOffsets[j], SeekOrigin.Begin);
                var ligatureGlyph = reader.ReadUInt16();
                var componentCount = reader.ReadUInt16();
                var componentGlyphIds = reader.ReadUInt16Array(componentCount - 1);
                ligatureTables[j] = new LigatureTable(ligatureGlyph, componentGlyphIds);
            }

            ligatureSetTables[i] = new LigatureSetTable(ligatureTables);
        }

        var coverageTable = CoverageTable.Load(reader, offset + coverageOffset);

        return new LookupType4Format1SubTable(ligatureSetTables, coverageTable, lookupFlags);
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
        if (offset <= -1) return false;

        var ligatureSetTable = ligatureSetTables[offset];
        SkippingGlyphIterator iterator = new(fontMetrics, collection, index, LookupFlags);
        Span<int> matchBuffer = stackalloc int[AdvancedTypographicUtils.MaxContextLength];
        for (var i = 0; i < ligatureSetTable.Ligatures.Length; i++)
        {
            var ligatureTable = ligatureSetTable.Ligatures[i];
            var remaining = count - 1;
            var compLength = ligatureTable.ComponentGlyphs.Length;
            if (compLength > remaining) continue;

            if (!AdvancedTypographicUtils.MatchInputSequence(iterator, feature, 1, ligatureTable.ComponentGlyphs,
                    matchBuffer)) continue;

            // From Harfbuzz:
            // - If it *is* a mark ligature, we don't allocate a new ligature id, and leave
            //   the ligature to keep its old ligature id.  This will allow it to attach to
            //   a base ligature in GPOS.  Eg. if the sequence is: LAM,LAM,SHADDA,FATHA,HEH,
            //   and LAM,LAM,HEH for a ligature, they will leave SHADDA and FATHA with a
            //   ligature id and component value of 2.  Then if SHADDA,FATHA form a ligature
            //   later, we don't want them to lose their ligature id/component, otherwise
            //   GPOS will fail to correctly position the mark ligature on top of the
            //   LAM,LAM,HEH ligature. See https://bugzilla.gnome.org/show_bug.cgi?id=676343
            //
            // - If a ligature is formed of components that some of which are also ligatures
            //   themselves, and those ligature components had marks attached to *their*
            //   components, we have to attach the marks to the new ligature component
            //   positions!  Now *that*'s tricky!  And these marks may be following the
            //   last component of the whole sequence, so we should loop forward looking
            //   for them and update them.
            //
            //   Eg. the sequence is LAM,LAM,SHADDA,FATHA,HEH, and the font first forms a
            //   'calt' ligature of LAM,HEH, leaving the SHADDA and FATHA with a ligature
            //   id and component == 1.  Now, during 'liga', the LAM and the LAM-HEH ligature
            //   form a LAM-LAM-HEH ligature.  We need to reassign the SHADDA and FATHA to
            //   the new ligature with a component value of 2.
            //
            //   This in fact happened to a font...  See https://bugzilla.gnome.org/show_bug.cgi?id=437633
            var data = collection.GetGlyphShapingData(index);
            var shapingClass = AdvancedTypographicUtils.GetGlyphShapingClass(fontMetrics, glyphId, data);
            var isBaseLigature = shapingClass.IsBase;
            var isMarkLigature = shapingClass.IsMark;

            var matches = matchBuffer.Slice(0, Math.Min(ligatureTable.ComponentGlyphs.Length, matchBuffer.Length));
            for (var j = 0; j < matches.Length && isMarkLigature; j++)
            {
                var match = collection.GetGlyphShapingData(matches[j]);
                if (!AdvancedTypographicUtils.IsMarkGlyph(fontMetrics, match.GlyphId, match))
                {
                    isBaseLigature = false;
                    isMarkLigature = false;
                    break;
                }
            }

            var isLigature = !isBaseLigature && !isMarkLigature;

            var ligatureId = isLigature ? 0 : collection.LigatureId++;
            var lastLigatureId = data.LigatureId;
            var lastComponentCount = data.CodePointCount;
            var currentComponentCount = lastComponentCount;
            var idx = index + 1;

            // Set ligatureID and ligatureComponent on glyphs that were skipped in the matched sequence.
            // This allows GPOS to attach marks to the correct ligature components.
            foreach (var matchIndex in matches)
            {
                // Don't assign new ligature components for mark ligatures (see above).
                if (isLigature)
                    idx = matchIndex;
                else
                    while (idx < matchIndex)
                    {
                        var current = collection.GetGlyphShapingData(idx);
                        var currentLC = current.LigatureComponent == -1 ? 1 : current.LigatureComponent;
                        var ligatureComponent = currentComponentCount - lastComponentCount +
                                                Math.Min(currentLC, lastComponentCount);
                        current.LigatureId = ligatureId;
                        current.LigatureComponent = ligatureComponent;

                        idx++;
                    }

                var last = collection.GetGlyphShapingData(idx);
                lastLigatureId = last.LigatureId;
                lastComponentCount = last.CodePointCount;
                currentComponentCount += lastComponentCount;
                idx++; // Skip base glyph
            }

            // Adjust ligature components for any marks following
            if (lastLigatureId > 0 && !isLigature)
            {
                // Only check glyphs managed by current shaper.
                var followingCount = count - (idx - index);
                for (var j = idx; j < followingCount; j++)
                {
                    var current = collection.GetGlyphShapingData(j);
                    if (current.LigatureId == lastLigatureId)
                    {
                        var currentLC = current.LigatureComponent == -1 ? 1 : current.LigatureComponent;
                        var ligatureComponent = currentComponentCount - lastComponentCount +
                                                Math.Min(currentLC, lastComponentCount);
                        current.LigatureId = ligatureId;
                        current.LigatureComponent = ligatureComponent;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Delete the matched glyphs, and replace the current glyph with the ligature glyph
            collection.Replace(index, matches, ligatureTable.GlyphId, ligatureId);
            return true;
        }

        return false;
    }

    public readonly struct LigatureSetTable
    {
        public LigatureSetTable(LigatureTable[] ligatures)
        {
            Ligatures = ligatures;
        }

        public LigatureTable[] Ligatures { get; }
    }

    public readonly struct LigatureTable
    {
        public LigatureTable(ushort glyphId, ushort[] componentGlyphs)
        {
            GlyphId = glyphId;
            ComponentGlyphs = componentGlyphs;
        }

        public ushort GlyphId { get; }

        public ushort[] ComponentGlyphs { get; }
    }
}