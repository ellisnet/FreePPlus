// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GSub;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GSub;

/// <summary>
///     A Chained Contexts Substitution subtable describes glyph substitutions in context
///     with an ability to look back and/or look ahead in the sequence of glyphs.
///     <see
///         href="https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#lookuptype-6-chained-contexts-substitution-subtable" />
/// </summary>
internal static class LookupType6SubTable
{
    public static LookupSubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        reader.Seek(offset, SeekOrigin.Begin);
        var substFormat = reader.ReadUInt16();

        return substFormat switch
        {
            1 => LookupType6Format1SubTable.Load(reader, offset, lookupFlags),
            2 => LookupType6Format2SubTable.Load(reader, offset, lookupFlags),
            3 => LookupType6Format3SubTable.Load(reader, offset, lookupFlags),
            _ => new NotImplementedSubTable()
        };
    }
}

internal sealed class LookupType6Format1SubTable : LookupSubTable
{
    private readonly CoverageTable coverageTable;
    private readonly ChainedSequenceRuleSetTable[] seqRuleSetTables;

    private LookupType6Format1SubTable(CoverageTable coverageTable, ChainedSequenceRuleSetTable[] seqRuleSetTables,
        LookupFlags lookupFlags)
        : base(lookupFlags)
    {
        this.coverageTable = coverageTable;
        this.seqRuleSetTables = seqRuleSetTables;
    }

    public static LookupType6Format1SubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        var seqRuleSets = TableLoadingUtils.LoadChainedSequenceContextFormat1(reader, offset, out var coverageTable);
        return new LookupType6Format1SubTable(coverageTable, seqRuleSets, lookupFlags);
    }

    public override bool TrySubstitution(
        FontMetrics fontMetrics,
        GSubTable table,
        GlyphSubstitutionCollection collection,
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

        // Apply ruleset for the given glyph id.
        SkippingGlyphIterator iterator = new(fontMetrics, collection, index, LookupFlags);
        var seqRuleSet = seqRuleSetTables[offset];
        var rules = seqRuleSet.SequenceRuleTables;
        for (var i = 0; i < rules.Length; i++)
        {
            var ruleTable = rules[i];
            if (!AdvancedTypographicUtils.ApplyChainedSequenceRule(iterator, ruleTable)) continue;

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

internal sealed class LookupType6Format2SubTable : LookupSubTable
{
    private readonly ClassDefinitionTable backtrackClassDefinitionTable;
    private readonly CoverageTable coverageTable;
    private readonly ClassDefinitionTable inputClassDefinitionTable;
    private readonly ClassDefinitionTable lookaheadClassDefinitionTable;
    private readonly ChainedClassSequenceRuleSetTable[] sequenceRuleSetTables;

    private LookupType6Format2SubTable(
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

    public static LookupType6Format2SubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        var seqRuleSets = TableLoadingUtils.LoadChainedSequenceContextFormat2(
            reader,
            offset,
            out var coverageTable,
            out var backtrackClassDefTable,
            out var inputClassDefTable,
            out var lookaheadClassDefTable);

        return new LookupType6Format2SubTable(seqRuleSets, backtrackClassDefTable, inputClassDefTable,
            lookaheadClassDefTable, coverageTable, lookupFlags);
    }

    public override bool TrySubstitution(
        FontMetrics fontMetrics,
        GSubTable table,
        GlyphSubstitutionCollection collection,
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
            ? sequenceRuleSetTables[classId]?.SubRules
            : null;
        if (rules is null) return false;

        // Apply ruleset for the given glyph class id.
        SkippingGlyphIterator iterator = new(fontMetrics, collection, index, LookupFlags);
        for (var lookupIndex = 0; lookupIndex < rules.Length; lookupIndex++)
        {
            var ruleTable = rules[lookupIndex];
            if (!AdvancedTypographicUtils.ApplyChainedClassSequenceRule(iterator, ruleTable, inputClassDefinitionTable,
                    backtrackClassDefinitionTable, lookaheadClassDefinitionTable)) continue;

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

internal sealed class LookupType6Format3SubTable : LookupSubTable
{
    private readonly CoverageTable[] backtrackCoverageTables;
    private readonly CoverageTable[] inputCoverageTables;
    private readonly CoverageTable[] lookaheadCoverageTables;
    private readonly SequenceLookupRecord[] sequenceLookupRecords;

    private LookupType6Format3SubTable(
        SequenceLookupRecord[] seqLookupRecords,
        CoverageTable[] backtrackCoverageTables,
        CoverageTable[] inputCoverageTables,
        CoverageTable[] lookaheadCoverageTables,
        LookupFlags lookupFlags)
        : base(lookupFlags)
    {
        sequenceLookupRecords = seqLookupRecords;
        this.backtrackCoverageTables = backtrackCoverageTables;
        this.inputCoverageTables = inputCoverageTables;
        this.lookaheadCoverageTables = lookaheadCoverageTables;
    }

    public static LookupType6Format3SubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        var seqLookupRecords = TableLoadingUtils.LoadChainedSequenceContextFormat3(
            reader,
            offset,
            out var backtrackCoverageTables,
            out var inputCoverageTables,
            out var lookaheadCoverageTables);

        return new LookupType6Format3SubTable(seqLookupRecords, backtrackCoverageTables, inputCoverageTables,
            lookaheadCoverageTables, lookupFlags);
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

        if (!AdvancedTypographicUtils.CheckAllCoverages(
                fontMetrics,
                LookupFlags,
                collection,
                index,
                count,
                inputCoverageTables,
                backtrackCoverageTables,
                lookaheadCoverageTables))
            return false;

        // It's a match. Perform substitutions and return true if anything changed.
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