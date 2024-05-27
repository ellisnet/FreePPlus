// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

/// <summary>
///     Mark-to-Ligature Attachment Positioning Subtable.
///     The MarkToLigature attachment (MarkLigPos) subtable is used to position combining mark glyphs with respect to
///     ligature base glyphs.
///     With MarkToBase attachment, described previously, each base glyph has an attachment point defined for each class of
///     marks.
///     MarkToLigature attachment is similar, except that each ligature glyph is defined to have multiple components (in a
///     virtual sense — not actual glyphs),
///     and each component has a separate set of attachment points defined for the different mark classes.
///     <see
///         href="https://docs.microsoft.com/en-us/typography/opentype/spec/gpos#lookup-type-5-mark-to-ligature-attachment-positioning-subtable" />
/// </summary>
internal static class LookupType5SubTable
{
    public static LookupSubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        reader.Seek(offset, SeekOrigin.Begin);
        var subTableFormat = reader.ReadUInt16();

        return subTableFormat switch
        {
            1 => LookupType5Format1SubTable.Load(reader, offset, lookupFlags),
            _ => new NotImplementedSubTable()
        };
    }

    internal sealed class LookupType5Format1SubTable : LookupSubTable
    {
        private readonly LigatureArrayTable ligatureArrayTable;
        private readonly CoverageTable ligatureCoverage;
        private readonly MarkArrayTable markArrayTable;
        private readonly CoverageTable markCoverage;

        public LookupType5Format1SubTable(
            CoverageTable markCoverage,
            CoverageTable ligatureCoverage,
            MarkArrayTable markArrayTable,
            LigatureArrayTable ligatureArrayTable,
            LookupFlags lookupFlags)
            : base(lookupFlags)
        {
            this.markCoverage = markCoverage;
            this.ligatureCoverage = ligatureCoverage;
            this.markArrayTable = markArrayTable;
            this.ligatureArrayTable = ligatureArrayTable;
        }

        public static LookupType5Format1SubTable Load(BigEndianBinaryReader reader, long offset,
            LookupFlags lookupFlags)
        {
            // MarkLigPosFormat1 Subtable.
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | Type               |  Name                           | Description                                          |
            // +====================+=================================+======================================================+
            // | uint16             | posFormat                       | Format identifier: format = 1                        |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | Offset16           | markCoverageOffset              | Offset to markCoverage table,                        |
            // |                    |                                 | from beginning of MarkLigPos subtable.               |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | Offset16           | ligatureCoverageOffset          | Offset to ligatureCoverage table,                    |
            // |                    |                                 | from beginning of MarkLigPos subtable.               |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | uint16             | markClassCount                  | Number of defined mark classes                       |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | Offset16           | markArrayOffset                 | Offset to MarkArray table, from beginning            |
            // |                    |                                 | of MarkLigPos subtable.                              |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | Offset16           | ligatureArrayOffset             | Offset to LigatureArray table,                       |
            // |                    |                                 | from beginning of MarkLigPos subtable.               |
            // +--------------------+---------------------------------+------------------------------------------------------+
            var markCoverageOffset = reader.ReadOffset16();
            var ligatureCoverageOffset = reader.ReadOffset16();
            var markClassCount = reader.ReadUInt16();
            var markArrayOffset = reader.ReadOffset16();
            var ligatureArrayOffset = reader.ReadOffset16();

            var markCoverage = CoverageTable.Load(reader, offset + markCoverageOffset);
            var ligatureCoverage = CoverageTable.Load(reader, offset + ligatureCoverageOffset);
            var markArrayTable = new MarkArrayTable(reader, offset + markArrayOffset);
            var ligatureArrayTable = new LigatureArrayTable(reader, offset + ligatureArrayOffset, markClassCount);

            return new LookupType5Format1SubTable(markCoverage, ligatureCoverage, markArrayTable, ligatureArrayTable,
                lookupFlags);
        }

        public override bool TryUpdatePosition(
            FontMetrics fontMetrics,
            GPosTable table,
            GlyphPositioningCollection collection,
            Tag feature,
            int index,
            int count)
        {
            // Mark-to-Ligature Attachment Positioning.
            // Implements: https://docs.microsoft.com/en-us/typography/opentype/spec/gpos#lookup-type-5-mark-to-ligature-attachment-positioning-subtable
            var glyphId = collection[index];
            if (glyphId == 0) return false;

            var markIndex = markCoverage.CoverageIndexOf(glyphId);
            if (markIndex < 0) return false;

            // Search backward for a base glyph.
            var baseGlyphIndex = index;
            while (--baseGlyphIndex >= 0)
            {
                var data = collection.GetGlyphShapingData(baseGlyphIndex);
                if (!AdvancedTypographicUtils.IsMarkGlyph(fontMetrics, data.GlyphId, data)) break;
            }

            if (baseGlyphIndex < 0) return false;

            var baseGlyphId = collection[baseGlyphIndex];
            var ligatureIndex = ligatureCoverage.CoverageIndexOf(baseGlyphId);
            if (ligatureIndex < 0) return false;

            // We must now check whether the ligature ID of the current mark glyph
            // is identical to the ligature ID of the found ligature.
            // If yes, we can directly use the component index. If not, we attach the mark
            // glyph to the last component of the ligature.
            var ligatureAttach = ligatureArrayTable.LigatureAttachTables[ligatureIndex];
            var markGlyph = collection.GetGlyphShapingData(index);
            var ligGlyph = collection.GetGlyphShapingData(baseGlyphIndex);
            var compIndex = ligGlyph.LigatureId > 0 && ligGlyph.LigatureId == markGlyph.LigatureId &&
                            markGlyph.LigatureComponent > 0
                ? Math.Min(markGlyph.LigatureComponent, ligGlyph.CodePointCount) - 1
                : ligGlyph.CodePointCount - 1;

            var markRecord = markArrayTable.MarkRecords[markIndex];
            var baseAnchor = ligatureAttach.ComponentRecords[compIndex].LigatureAnchorTables[markRecord.MarkClass];
            AdvancedTypographicUtils.ApplyAnchor(fontMetrics, collection, index, baseAnchor, markRecord,
                baseGlyphIndex);

            return true;
        }
    }
}