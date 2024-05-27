// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GSub;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GSub;

/// <summary>
///     A Contextual Substitution subtable describes glyph substitutions in context that replace one
///     or more glyphs within a certain pattern of glyphs.
///     <see
///         href="https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#lookuptype-5-contextual-substitution-subtable" />
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
            2 => LookupType5Format2SubTable.Load(reader, offset, lookupFlags),
            3 => LookupType5Format3SubTable.Load(reader, offset, lookupFlags),
            _ => new NotImplementedSubTable()
        };
    }
}

internal sealed class LookupType5Format1SubTable : LookupSubTable
{
    private readonly CoverageTable coverageTable;
    private readonly SequenceRuleSetTable[] seqRuleSetTables;

    private LookupType5Format1SubTable(CoverageTable coverageTable, SequenceRuleSetTable[] seqRuleSetTables,
        LookupFlags lookupFlags)
        : base(lookupFlags)
    {
        this.coverageTable = coverageTable;
        this.seqRuleSetTables = seqRuleSetTables;
    }

    public static LookupType5Format1SubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        var seqRuleSets = TableLoadingUtils.LoadSequenceContextFormat1(reader, offset, out var coverageTable);

        return new LookupType5Format1SubTable(coverageTable, seqRuleSets, lookupFlags);
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

            // It's a match. Perform substitutions and return true if anything changed.
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

internal sealed class LookupType5Format2SubTable : LookupSubTable
{
    private readonly ClassDefinitionTable classDefinitionTable;
    private readonly CoverageTable coverageTable;
    private readonly ClassSequenceRuleSetTable[] sequenceRuleSetTables;

    private LookupType5Format2SubTable(
        ClassSequenceRuleSetTable[] sequenceRuleSetTables,
        ClassDefinitionTable classDefinitionTable,
        CoverageTable coverageTable,
        LookupFlags lookupFlags)
        : base(lookupFlags)
    {
        this.sequenceRuleSetTables = sequenceRuleSetTables;
        this.classDefinitionTable = classDefinitionTable;
        this.coverageTable = coverageTable;
    }

    public static LookupType5Format2SubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        var coverageTable =
            TableLoadingUtils.LoadSequenceContextFormat2(reader, offset, out var classDefTable,
                out var classSeqRuleSets);

        return new LookupType5Format2SubTable(classSeqRuleSets, classDefTable, coverageTable, lookupFlags);
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

        if (coverageTable.CoverageIndexOf(glyphId) <= -1) return false;

        // TODO: Check this.
        // https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#52-context-substitution-format-2-class-based-glyph-contexts
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

            // It's a match. Perform substitutions and return true if anything changed.
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

internal sealed class LookupType5Format3SubTable : LookupSubTable
{
    private readonly CoverageTable[] coverageTables;
    private readonly SequenceLookupRecord[] sequenceLookupRecords;

    private LookupType5Format3SubTable(CoverageTable[] coverageTables, SequenceLookupRecord[] sequenceLookupRecords,
        LookupFlags lookupFlags)
        : base(lookupFlags)
    {
        this.coverageTables = coverageTables;
        this.sequenceLookupRecords = sequenceLookupRecords;
    }

    public static LookupType5Format3SubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        var seqLookupRecords = TableLoadingUtils.LoadSequenceContextFormat3(reader, offset, out var coverageTables);

        return new LookupType5Format3SubTable(coverageTables, seqLookupRecords, lookupFlags);
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

        // https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#53-context-substitution-format-3-coverage-based-glyph-contexts
        SkippingGlyphIterator iterator = new(fontMetrics, collection, index, LookupFlags);
        if (!AdvancedTypographicUtils.MatchCoverageSequence(iterator, coverageTables, 0)) return false;

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