// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

/// <summary>
///     Lookup Type 6: Mark-to-Mark Attachment Positioning Subtable.
///     The MarkToMark attachment (MarkMarkPos) subtable is identical in form to the MarkToBase attachment subtable,
///     although its function is different.
///     MarkToMark attachment defines the position of one mark relative to another mark as when, for example,
///     positioning tone marks with respect to vowel diacritical marks in Vietnamese.
///     <see
///         href="https://docs.microsoft.com/en-us/typography/opentype/spec/gpos#lookup-type-6-mark-to-mark-attachment-positioning-subtable" />
/// </summary>
internal static class LookupType6SubTable
{
    public static LookupSubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        reader.Seek(offset, SeekOrigin.Begin);
        var subTableFormat = reader.ReadUInt16();

        return subTableFormat switch
        {
            1 => LookupType6Format1SubTable.Load(reader, offset, lookupFlags),
            _ => new NotImplementedSubTable()
        };
    }

    internal sealed class LookupType6Format1SubTable : LookupSubTable
    {
        private readonly MarkArrayTable mark1ArrayTable;
        private readonly CoverageTable mark1Coverage;
        private readonly Mark2ArrayTable mark2ArrayTable;
        private readonly CoverageTable mark2Coverage;

        public LookupType6Format1SubTable(
            CoverageTable mark1Coverage,
            CoverageTable mark2Coverage,
            MarkArrayTable mark1ArrayTable,
            Mark2ArrayTable mark2ArrayTable,
            LookupFlags lookupFlags)
            : base(lookupFlags)
        {
            this.mark1Coverage = mark1Coverage;
            this.mark2Coverage = mark2Coverage;
            this.mark1ArrayTable = mark1ArrayTable;
            this.mark2ArrayTable = mark2ArrayTable;
        }

        public static LookupType6Format1SubTable Load(BigEndianBinaryReader reader, long offset,
            LookupFlags lookupFlags)
        {
            // MarkMarkPosFormat1 Subtable.
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | Type               |  Name                           | Description                                          |
            // +====================+=================================+======================================================+
            // | uint16             | posFormat                       | Format identifier: format = 1                        |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | Offset16           | mark1CoverageOffset             | Offset to Combining Mark Coverage table,             |
            // |                    |                                 | from beginning of MarkMarkPos subtable.              |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | Offset16           | mark2CoverageOffset             | Offset to Base Mark Coverage table,                  |
            // |                    |                                 | from beginning of MarkMarkPos subtable.              |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | uint16             | markClassCount                  | Number of Combining Mark classes defined             |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | Offset16           | mark1ArrayOffset                | Offset to MarkArray table for mark1,                 |
            // |                    |                                 | from beginning of MarkMarkPos subtable.              |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | Offset16           | mark2ArrayOffset                | Offset to Mark2Array table for mark2,                |
            // |                    |                                 | from beginning of MarkMarkPos subtable.              |
            // +--------------------+---------------------------------+------------------------------------------------------+
            var mark1CoverageOffset = reader.ReadOffset16();
            var mark2CoverageOffset = reader.ReadOffset16();
            var markClassCount = reader.ReadUInt16();
            var mark1ArrayOffset = reader.ReadOffset16();
            var mark2ArrayOffset = reader.ReadOffset16();

            var mark1Coverage = CoverageTable.Load(reader, offset + mark1CoverageOffset);
            var mark2Coverage = CoverageTable.Load(reader, offset + mark2CoverageOffset);
            var mark1ArrayTable = new MarkArrayTable(reader, offset + mark1ArrayOffset);
            var mark2ArrayTable = new Mark2ArrayTable(reader, markClassCount, offset + mark2ArrayOffset);

            return new LookupType6Format1SubTable(mark1Coverage, mark2Coverage, mark1ArrayTable, mark2ArrayTable,
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
            // Mark to mark positioning.
            // Implements: https://docs.microsoft.com/en-us/typography/opentype/spec/gpos#lookup-type-6-mark-to-mark-attachment-positioning-subtable
            var glyphId = collection[index];
            if (glyphId == 0) return false;

            var mark1Index = mark1Coverage.CoverageIndexOf(glyphId);
            if (mark1Index == -1) return false;

            // Get the previous mark to attach to.
            if (index < 1) return false;

            var prevIdx = index - 1;
            var prevGlyphId = collection[prevIdx];
            var prevGlyph = collection.GetGlyphShapingData(prevIdx);
            if (!AdvancedTypographicUtils.IsMarkGlyph(fontMetrics, prevGlyphId, prevGlyph)) return false;

            // The following logic was borrowed from Harfbuzz,
            // see: https://github.com/harfbuzz/harfbuzz/blob/3e635cf5e26e33d6210d3092256a49291752deec/src/hb-ot-layout-gpos-table.hh#L2525
            var good = false;
            var curGlyph = collection.GetGlyphShapingData(index);
            if (curGlyph.LigatureId == prevGlyph.LigatureId)
            {
                if (curGlyph.LigatureId > 0)
                    // Marks belonging to the same base.
                    good = true;
                else if (curGlyph.LigatureComponent == prevGlyph.LigatureComponent)
                    // Marks belonging to the same ligature component.
                    good = true;
            }
            else
            {
                // If ligature ids don't match, it may be the case that one of the marks
                // itself is a ligature, in which case match.
                if ((curGlyph.LigatureId > 0 && curGlyph.LigatureComponent <= 0)
                    || (prevGlyph.LigatureId > 0 && prevGlyph.LigatureComponent <= 0))
                    good = true;
            }

            if (!good) return false;

            var mark2Index = mark2Coverage.CoverageIndexOf(prevGlyphId);
            if (mark2Index == -1) return false;

            var markRecord = mark1ArrayTable.MarkRecords[mark1Index];
            var baseAnchor = mark2ArrayTable.Mark2Records[mark2Index].MarkAnchorTable[markRecord.MarkClass];
            AdvancedTypographicUtils.ApplyAnchor(fontMetrics, collection, index, baseAnchor, markRecord, prevIdx);

            return true;
        }
    }
}