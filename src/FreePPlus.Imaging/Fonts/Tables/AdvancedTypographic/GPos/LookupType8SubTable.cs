// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

/// <summary>
///     LookupType 8: Chained Contexts Positioning Subtable.
///     A Chained Contexts Positioning subtable describes glyph positioning in context with an ability to look back and/or
///     look ahead in the sequence of glyphs.
///     The design of the Chained Contexts Positioning subtable is parallel to that of the Contextual Positioning subtable,
///     including the availability of three formats.
///     Each format can describe one or more chained backtrack, input, and lookahead sequence combinations, and one or more
///     positioning adjustments for glyphs in each input sequence.
///     <see
///         href="https://docs.microsoft.com/en-us/typography/opentype/spec/gpos#lookuptype-8-chained-contexts-positioning-subtable" />
/// </summary>
internal static class LookupType8SubTable
{
    public static LookupSubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        reader.Seek(offset, SeekOrigin.Begin);
        var substFormat = reader.ReadUInt16();

        return substFormat switch
        {
            1 => LookupType8Format1SubTable.Load(reader, offset, lookupFlags),
            2 => LookupType8Format2SubTable.Load(reader, offset, lookupFlags),
            3 => LookupType8Format3SubTable.Load(reader, offset, lookupFlags),
            _ => new NotImplementedSubTable()
        };
    }

    internal sealed class LookupType8Format1SubTable : LookupSubTable
    {
        private readonly CoverageTable coverageTable;
        private readonly ChainedSequenceRuleSetTable[] seqRuleSetTables;

        private LookupType8Format1SubTable(
            CoverageTable coverageTable,
            ChainedSequenceRuleSetTable[] seqRuleSetTables,
            LookupFlags lookupFlags)
            : base(lookupFlags)
        {
            this.coverageTable = coverageTable;
            this.seqRuleSetTables = seqRuleSetTables;
        }

        public static LookupType8Format1SubTable Load(BigEndianBinaryReader reader, long offset,
            LookupFlags lookupFlags)
        {
            var seqRuleSets =
                TableLoadingUtils.LoadChainedSequenceContextFormat1(reader, offset, out var coverageTable);
            return new LookupType8Format1SubTable(coverageTable, seqRuleSets, lookupFlags);
        }

        public override bool TryUpdatePosition(
            FontMetrics fontMetrics,
            GPosTable table,
            GlyphPositioningCollection collection,
            Tag feature,
            int index,
            int count)
        {
            // Implements Chained Contexts Substitution, Format 1:
            // https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#61-chained-contexts-substitution-format-1-simple-glyph-contexts
            var glyphId = collection[index];
            if (glyphId == 0) return false;

            // Search for the current glyph in the Coverage table.
            var offset = coverageTable.CoverageIndexOf(glyphId);
            if (offset <= -1) return false;

            if (seqRuleSetTables is null || seqRuleSetTables.Length is 0) return false;

            var seqRuleSet = seqRuleSetTables[offset];
            if (seqRuleSet is null) return false;

            // Apply ruleset for the given glyph id.
            var rules = seqRuleSet.SequenceRuleTables;
            SkippingGlyphIterator iterator = new(fontMetrics, collection, index, LookupFlags);
            for (var lookupIndex = 0; lookupIndex < rules.Length; lookupIndex++)
            {
                var rule = rules[lookupIndex];
                if (!AdvancedTypographicUtils.ApplyChainedSequenceRule(iterator, rule)) continue;

                var hasChanged = false;
                for (var j = 0; j < rule.SequenceLookupRecords.Length; j++)
                {
                    var sequenceLookupRecord = rule.SequenceLookupRecords[j];
                    var lookup = table.LookupList.LookupTables[sequenceLookupRecord.LookupListIndex];
                    var sequenceIndex = sequenceLookupRecord.SequenceIndex;
                    if (lookup.TryUpdatePosition(fontMetrics, table, collection, feature, index + sequenceIndex, 1))
                        hasChanged = true;
                }

                return hasChanged;
            }

            return false;
        }
    }

    internal sealed class LookupType8Format2SubTable : LookupSubTable
    {
        private readonly ClassDefinitionTable backtrackClassDefinitionTable;
        private readonly CoverageTable coverageTable;
        private readonly ClassDefinitionTable inputClassDefinitionTable;
        private readonly ClassDefinitionTable lookaheadClassDefinitionTable;
        private readonly ChainedClassSequenceRuleSetTable[] sequenceRuleSetTables;

        private LookupType8Format2SubTable(
            ChainedClassSequenceRuleSetTable[] sequenceRuleSetTables,
            ClassDefinitionTable backtrackClassDefinitionTable,
            ClassDefinitionTable inputClassDefinitionTable,
            ClassDefinitionTable lookaheadClassDefinitionTable,
            CoverageTable coverageTable,
            LookupFlags lookupFlags)
            : base(lookupFlags)
        {
            this.sequenceRuleSetTables = sequenceRuleSetTables;
            this.backtrackClassDefinitionTable = backtrackClassDefinitionTable;
            this.inputClassDefinitionTable = inputClassDefinitionTable;
            this.lookaheadClassDefinitionTable = lookaheadClassDefinitionTable;
            this.coverageTable = coverageTable;
        }

        public static LookupType8Format2SubTable Load(BigEndianBinaryReader reader, long offset,
            LookupFlags lookupFlags)
        {
            var seqRuleSets = TableLoadingUtils.LoadChainedSequenceContextFormat2(
                reader,
                offset,
                out var coverageTable,
                out var backtrackClassDefTable,
                out var inputClassDefTable,
                out var lookaheadClassDefTable);

            return new LookupType8Format2SubTable(seqRuleSets, backtrackClassDefTable, inputClassDefTable,
                lookaheadClassDefTable, coverageTable, lookupFlags);
        }

        public override bool TryUpdatePosition(
            FontMetrics fontMetrics,
            GPosTable table,
            GlyphPositioningCollection collection,
            Tag feature,
            int index,
            int count)
        {
            // Implements Chained Contexts Substitution for Format 2:
            // https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#62-chained-contexts-substitution-format-2-class-based-glyph-contexts
            var glyphId = collection[index];
            if (glyphId == 0) return false;

            // Search for the current glyph in the Coverage table.
            var offset = coverageTable.CoverageIndexOf(glyphId);
            if (offset <= -1) return false;

            // Search in the class definition table to find the class value assigned to the currently glyph.
            var classId = inputClassDefinitionTable.ClassIndexOf(glyphId);
            var rules = classId >= 0 && classId < sequenceRuleSetTables.Length
                ? sequenceRuleSetTables[classId].SubRules
                : null;
            if (rules is null) return false;

            // Apply ruleset for the given glyph class id.
            SkippingGlyphIterator iterator = new(fontMetrics, collection, index, LookupFlags);
            for (var lookupIndex = 0; lookupIndex < rules.Length; lookupIndex++)
            {
                var rule = rules[lookupIndex];
                if (!AdvancedTypographicUtils.ApplyChainedClassSequenceRule(iterator, rule, inputClassDefinitionTable,
                        backtrackClassDefinitionTable, lookaheadClassDefinitionTable)) continue;

                // It's a match. Perform position update and return true if anything changed.
                var hasChanged = false;
                for (var j = 0; j < rule.SequenceLookupRecords.Length; j++)
                {
                    var sequenceLookupRecord = rule.SequenceLookupRecords[j];
                    var lookup = table.LookupList.LookupTables[sequenceLookupRecord.LookupListIndex];
                    var sequenceIndex = sequenceLookupRecord.SequenceIndex;
                    if (lookup.TryUpdatePosition(fontMetrics, table, collection, feature, index + sequenceIndex, 1))
                        hasChanged = true;
                }

                return hasChanged;
            }

            return false;
        }
    }

    internal sealed class LookupType8Format3SubTable : LookupSubTable
    {
        private readonly CoverageTable[] backtrackCoverageTables;
        private readonly CoverageTable[] inputCoverageTables;
        private readonly CoverageTable[] lookaheadCoverageTables;
        private readonly SequenceLookupRecord[] seqLookupRecords;

        private LookupType8Format3SubTable(
            SequenceLookupRecord[] seqLookupRecords,
            CoverageTable[] backtrackCoverageTables,
            CoverageTable[] inputCoverageTables,
            CoverageTable[] lookaheadCoverageTables,
            LookupFlags lookupFlags)
            : base(lookupFlags)
        {
            this.seqLookupRecords = seqLookupRecords;
            this.backtrackCoverageTables = backtrackCoverageTables;
            this.inputCoverageTables = inputCoverageTables;
            this.lookaheadCoverageTables = lookaheadCoverageTables;
        }

        public static LookupType8Format3SubTable Load(BigEndianBinaryReader reader, long offset,
            LookupFlags lookupFlags)
        {
            var seqLookupRecords = TableLoadingUtils.LoadChainedSequenceContextFormat3(
                reader,
                offset,
                out var backtrackCoverageTables,
                out var inputCoverageTables,
                out var lookaheadCoverageTables);

            return new LookupType8Format3SubTable(seqLookupRecords, backtrackCoverageTables, inputCoverageTables,
                lookaheadCoverageTables, lookupFlags);
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

            if (!AdvancedTypographicUtils.CheckAllCoverages(fontMetrics, LookupFlags, collection, index, count,
                    inputCoverageTables, backtrackCoverageTables, lookaheadCoverageTables)) return false;

            // It's a match. Perform position update and return true if anything changed.
            var hasChanged = false;
            foreach (var lookupRecord in seqLookupRecords)
            {
                var sequenceIndex = lookupRecord.SequenceIndex;
                var lookupIndex = lookupRecord.LookupListIndex;

                var lookup = table.LookupList.LookupTables[lookupIndex];
                if (lookup.TryUpdatePosition(fontMetrics, table, collection, feature, index + sequenceIndex,
                        count - sequenceIndex)) hasChanged = true;
            }

            return hasChanged;
        }
    }
}