// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

/// <summary>
///     Lookup Type 7: Contextual Positioning Subtables.
///     A Contextual Positioning subtable describes glyph positioning in context so a text-processing client can adjust the
///     position
///     of one or more glyphs within a certain pattern of glyphs.
///     <see
///         href="https://docs.microsoft.com/en-us/typography/opentype/spec/gpos#lookup-type-7-contextual-positioning-subtables" />
/// </summary>
internal static class LookupType7SubTable
{
    public static LookupSubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        reader.Seek(offset, SeekOrigin.Begin);
        var subTableFormat = reader.ReadUInt16();

        return subTableFormat switch
        {
            1 => LookupType7Format1SubTable.Load(reader, offset, lookupFlags),
            2 => LookupType7Format2SubTable.Load(reader, offset, lookupFlags),
            3 => LookupType7Format3SubTable.Load(reader, offset, lookupFlags),
            _ => new NotImplementedSubTable()
        };
    }

    internal sealed class LookupType7Format1SubTable : LookupSubTable
    {
        private readonly CoverageTable coverageTable;
        private readonly SequenceRuleSetTable[] seqRuleSetTables;

        public LookupType7Format1SubTable(CoverageTable coverageTable, SequenceRuleSetTable[] seqRuleSetTables,
            LookupFlags lookupFlags)
            : base(lookupFlags)
        {
            this.seqRuleSetTables = seqRuleSetTables;
            this.coverageTable = coverageTable;
        }

        public static LookupType7Format1SubTable Load(BigEndianBinaryReader reader, long offset,
            LookupFlags lookupFlags)
        {
            var seqRuleSets = TableLoadingUtils.LoadSequenceContextFormat1(reader, offset, out var coverageTable);

            return new LookupType7Format1SubTable(coverageTable, seqRuleSets, lookupFlags);
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

            var offset = coverageTable.CoverageIndexOf(glyphId);
            if (offset <= -1) return false;

            // TODO: Check this.
            // https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#example-7-contextual-substitution-format-1
            var ruleSetTable = seqRuleSetTables[offset];
            SkippingGlyphIterator iterator = new(fontMetrics, collection, index, LookupFlags);
            foreach (var ruleTable in ruleSetTable.SequenceRuleTables)
            {
                var remaining = count - 1;
                var seqLength = ruleTable.InputSequence.Length;
                if (seqLength > remaining) continue;

                if (!AdvancedTypographicUtils.MatchSequence(iterator, 1, ruleTable.InputSequence)) continue;

                // It's a match. Perform position update and return true if anything changed.
                return AdvancedTypographicUtils.ApplyLookupList(
                    fontMetrics,
                    table,
                    feature,
                    LookupFlags,
                    ruleTable.SequenceLookupRecords,
                    collection,
                    index,
                    count);
            }

            return false;
        }
    }

    internal sealed class LookupType7Format2SubTable : LookupSubTable
    {
        private readonly ClassDefinitionTable classDefinitionTable;
        private readonly CoverageTable coverageTable;
        private readonly ClassSequenceRuleSetTable[] sequenceRuleSetTables;

        public LookupType7Format2SubTable(
            CoverageTable coverageTable,
            ClassDefinitionTable classDefinitionTable,
            ClassSequenceRuleSetTable[] sequenceRuleSetTables,
            LookupFlags lookupFlags)
            : base(lookupFlags)
        {
            this.coverageTable = coverageTable;
            this.classDefinitionTable = classDefinitionTable;
            this.sequenceRuleSetTables = sequenceRuleSetTables;
        }

        public static LookupType7Format2SubTable Load(BigEndianBinaryReader reader, long offset,
            LookupFlags lookupFlags)
        {
            var coverageTable =
                TableLoadingUtils.LoadSequenceContextFormat2(reader, offset, out var classDefTable,
                    out var classSeqRuleSets);

            return new LookupType7Format2SubTable(coverageTable, classDefTable, classSeqRuleSets, lookupFlags);
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

            if (coverageTable.CoverageIndexOf(glyphId) < 0) return false;

            var offset = classDefinitionTable.ClassIndexOf(glyphId);
            if (offset < 0) return false;

            var ruleSetTable = sequenceRuleSetTables[offset];
            SkippingGlyphIterator iterator = new(fontMetrics, collection, index, LookupFlags);
            foreach (var ruleTable in ruleSetTable.SequenceRuleTables)
            {
                var remaining = count - 1;
                var seqLength = ruleTable.InputSequence.Length;
                if (seqLength > remaining) continue;

                if (!AdvancedTypographicUtils.MatchClassSequence(iterator, 1, ruleTable.InputSequence,
                        classDefinitionTable)) continue;

                // It's a match. Perform position update and return true if anything changed.
                return AdvancedTypographicUtils.ApplyLookupList(
                    fontMetrics,
                    table,
                    feature,
                    LookupFlags,
                    ruleTable.SequenceLookupRecords,
                    collection,
                    index,
                    count);
            }

            return false;
        }
    }

    internal sealed class LookupType7Format3SubTable : LookupSubTable
    {
        private readonly CoverageTable[] coverageTables;
        private readonly SequenceLookupRecord[] sequenceLookupRecords;

        public LookupType7Format3SubTable(CoverageTable[] coverageTables, SequenceLookupRecord[] sequenceLookupRecords,
            LookupFlags lookupFlags)
            : base(lookupFlags)
        {
            this.coverageTables = coverageTables;
            this.sequenceLookupRecords = sequenceLookupRecords;
        }

        public static LookupType7Format3SubTable Load(BigEndianBinaryReader reader, long offset,
            LookupFlags lookupFlags)
        {
            var seqLookupRecords = TableLoadingUtils.LoadSequenceContextFormat3(reader, offset, out var coverageTables);

            return new LookupType7Format3SubTable(coverageTables, seqLookupRecords, lookupFlags);
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

            SkippingGlyphIterator iterator = new(fontMetrics, collection, index, LookupFlags);
            if (!AdvancedTypographicUtils.MatchCoverageSequence(iterator, coverageTables, 0)) return false;

            return AdvancedTypographicUtils.ApplyLookupList(
                fontMetrics,
                table,
                feature,
                LookupFlags,
                sequenceLookupRecords,
                collection,
                index,
                count);
        }
    }
}