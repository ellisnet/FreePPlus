// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;
using FreePPlus.Imaging.Fonts.Unicode;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

internal static class AdvancedTypographicUtils
{
    // The following properties are used to prevent overflows caused
    // by maliciously crafted fonts.
    // Based on HarfBuzz hb-buffer.hh
    public const int MaxContextLength = 64;
    private const int MaxLengthFactor = 64;
    private const int MaxLengthMinimum = 16384;
    private const int MaxOperationsFactor = 1024;
    private const int MaxOperationsMinimum = 16384;
    private const int MaxShapingCharsLength = 0x3FFFFFFF; // Half int max.

    public static int GetMaxAllowableShapingCollectionCount(int length)
    {
        return (int)Math.Min(Math.Max((long)length * MaxLengthFactor, MaxLengthMinimum), MaxShapingCharsLength);
    }

    public static int GetMaxAllowableShapingOperationsCount(int length)
    {
        return (int)Math.Min(Math.Max((long)length * MaxOperationsFactor, MaxOperationsMinimum), MaxShapingCharsLength);
    }

    public static bool ApplyLookupList(
        FontMetrics fontMetrics,
        GSubTable table,
        Tag feature,
        LookupFlags lookupFlags,
        SequenceLookupRecord[] records,
        GlyphSubstitutionCollection collection,
        int index,
        int count)
    {
        var hasChanged = false;
        SkippingGlyphIterator iterator = new(fontMetrics, collection, index, lookupFlags);
        var currentCount = collection.Count;

        foreach (var lookupRecord in records)
        {
            var sequenceIndex = lookupRecord.SequenceIndex;
            var lookupIndex = lookupRecord.LookupListIndex;
            iterator.Index = index;
            iterator.Increment(sequenceIndex);
            var lookup = table.LookupList.LookupTables[lookupIndex];
            hasChanged |= lookup.TrySubstitution(fontMetrics, table, collection, feature, iterator.Index,
                count - (iterator.Index - index));

            // Account for substitutions changing the length of the collection.
            if (collection.Count != currentCount)
            {
                count -= currentCount - collection.Count;
                currentCount = collection.Count;
            }
        }

        return hasChanged;
    }

    public static bool ApplyLookupList(
        FontMetrics fontMetrics,
        GPosTable table,
        Tag feature,
        LookupFlags lookupFlags,
        SequenceLookupRecord[] records,
        GlyphPositioningCollection collection,
        int index,
        int count)
    {
        var hasChanged = false;
        SkippingGlyphIterator iterator = new(fontMetrics, collection, index, lookupFlags);
        foreach (var lookupRecord in records)
        {
            var sequenceIndex = lookupRecord.SequenceIndex;
            var lookupIndex = lookupRecord.LookupListIndex;
            iterator.Index = index;
            iterator.Increment(sequenceIndex);
            var lookup = table.LookupList.LookupTables[lookupIndex];
            hasChanged |= lookup.TryUpdatePosition(fontMetrics, table, collection, feature, iterator.Index,
                count - (iterator.Index - index));
        }

        return hasChanged;
    }

    public static bool MatchInputSequence(SkippingGlyphIterator iterator, Tag feature, ushort increment,
        ushort[] sequence, Span<int> matches)
    {
        return Match(
            increment,
            sequence,
            iterator,
            (component, data) =>
            {
                if (!ContainsFeatureTag(data.Features, feature)) return false;

                return component == data.GlyphId;
            },
            matches);
    }

    private static bool ContainsFeatureTag(List<TagEntry> featureList, Tag feature)
    {
        foreach (var tagEntry in featureList)
            if (tagEntry.Tag == feature && tagEntry.Enabled)
                return true;

        return false;
    }

    public static bool MatchSequence(SkippingGlyphIterator iterator, int increment, ushort[] sequence)
    {
        return Match(
            increment,
            sequence,
            iterator,
            (component, data) => component == data.GlyphId,
            default);
    }

    public static bool MatchClassSequence(
        SkippingGlyphIterator iterator,
        int increment,
        ushort[] sequence,
        ClassDefinitionTable classDefinitionTable)
    {
        return Match(
            increment,
            sequence,
            iterator,
            (component, data) => component == classDefinitionTable.ClassIndexOf(data.GlyphId),
            default);
    }

    public static bool MatchCoverageSequence(
        SkippingGlyphIterator iterator,
        CoverageTable[] coverageTable,
        int increment)
    {
        return Match(
            increment,
            coverageTable,
            iterator,
            (component, data) => component.CoverageIndexOf(data.GlyphId) >= 0,
            default);
    }

    public static bool ApplyChainedSequenceRule(SkippingGlyphIterator iterator, ChainedSequenceRuleTable rule)
    {
        if (rule.BacktrackSequence.Length > 0
            && !MatchSequence(iterator, -rule.BacktrackSequence.Length, rule.BacktrackSequence))
            return false;

        if (rule.InputSequence.Length > 0
            && !MatchSequence(iterator, 1, rule.InputSequence))
            return false;

        if (rule.LookaheadSequence.Length > 0
            && !MatchSequence(iterator, 1 + rule.InputSequence.Length, rule.LookaheadSequence))
            return false;

        return true;
    }

    public static bool ApplyChainedClassSequenceRule(
        SkippingGlyphIterator iterator,
        ChainedClassSequenceRuleTable rule,
        ClassDefinitionTable inputClassDefinitionTable,
        ClassDefinitionTable backtrackClassDefinitionTable,
        ClassDefinitionTable lookaheadClassDefinitionTable)
    {
        if (rule.BacktrackSequence.Length > 0
            && !MatchClassSequence(iterator, -rule.BacktrackSequence.Length, rule.BacktrackSequence,
                backtrackClassDefinitionTable))
            return false;

        if (rule.InputSequence.Length > 0 &&
            !MatchClassSequence(iterator, 1, rule.InputSequence, inputClassDefinitionTable))
            return false;

        if (rule.LookaheadSequence.Length > 0
            && !MatchClassSequence(iterator, 1 + rule.InputSequence.Length, rule.LookaheadSequence,
                lookaheadClassDefinitionTable))
            return false;

        return true;
    }

    public static bool CheckAllCoverages(
        FontMetrics fontMetrics,
        LookupFlags lookupFlags,
        IGlyphShapingCollection collection,
        int index,
        int count,
        CoverageTable[] input,
        CoverageTable[] backtrack,
        CoverageTable[] lookahead)
    {
        // Check that there are enough context glyphs.
        if (index - backtrack.Length < 0 || input.Length + lookahead.Length > count) return false;

        // Check all coverages: if any of them does not match, abort update.
        SkippingGlyphIterator iterator = new(fontMetrics, collection, index, lookupFlags);
        if (!MatchCoverageSequence(iterator, backtrack, -backtrack.Length)) return false;

        if (!MatchCoverageSequence(iterator, input, 0)) return false;

        if (!MatchCoverageSequence(iterator, lookahead, input.Length)) return false;

        return true;
    }

    public static void ApplyAnchor(
        FontMetrics fontMetrics,
        GlyphPositioningCollection collection,
        int index,
        AnchorTable baseAnchor,
        MarkRecord markRecord,
        int baseGlyphIndex)
    {
        var baseData = collection.GetGlyphShapingData(baseGlyphIndex);
        var baseXY = baseAnchor.GetAnchor(fontMetrics, baseData, collection);

        var markData = collection.GetGlyphShapingData(index);
        var markXY = markRecord.MarkAnchorTable.GetAnchor(fontMetrics, markData, collection);

        markData.Bounds.X = baseXY.XCoordinate - markXY.XCoordinate;
        markData.Bounds.Y = baseXY.YCoordinate - markXY.YCoordinate;
        markData.MarkAttachment = baseGlyphIndex;
    }

    public static void ApplyPosition(
        GlyphPositioningCollection collection,
        int index,
        ValueRecord record)
    {
        var current = collection.GetGlyphShapingData(index);
        current.Bounds.Width += record.XAdvance;
        current.Bounds.Height += record.YAdvance;
        current.Bounds.X += record.XPlacement;
        current.Bounds.Y += record.YPlacement;
    }

    public static bool IsMarkGlyph(FontMetrics fontMetrics, ushort glyphId, GlyphShapingData shapingData)
    {
        if (!fontMetrics.TryGetGlyphClass(glyphId, out var glyphClass) &&
            !CodePoint.IsMark(shapingData.CodePoint))
            return false;

        if (glyphClass != GlyphClassDef.MarkGlyph) return false;

        return true;
    }

    public static GlyphShapingClass GetGlyphShapingClass(FontMetrics fontMetrics, ushort glyphId,
        GlyphShapingData shapingData)
    {
        bool isMark;
        bool isBase;
        bool isLigature;
        ushort markAttachmentType = 0;
        if (fontMetrics.TryGetGlyphClass(glyphId, out var glyphClass))
        {
            isMark = glyphClass == GlyphClassDef.MarkGlyph;
            isBase = glyphClass == GlyphClassDef.BaseGlyph;
            isLigature = glyphClass == GlyphClassDef.LigatureGlyph;
            if (fontMetrics.TryGetMarkAttachmentClass(glyphId, out var markAttachmentClass))
                markAttachmentType = (ushort)markAttachmentClass;
        }
        else
        {
            // TODO: We may have to store each codepoint. FontKit checks all.
            isMark = CodePoint.IsMark(shapingData.CodePoint);
            isBase = !isMark;
            isLigature = shapingData.CodePointCount > 1;
        }

        return new GlyphShapingClass(isMark, isBase, isLigature, markAttachmentType);
    }

    private static bool Match<T>(
        int increment,
        T[] sequence,
        SkippingGlyphIterator iterator,
        Func<T, GlyphShapingData, bool> condition,
        Span<int> matches)
    {
        var position = iterator.Index;
        var offset = iterator.Increment(increment);
        var collection = iterator.Collection;

        var i = 0;
        while (i < sequence.Length && i < MaxContextLength && offset < collection.Count)
        {
            if (!condition(sequence[i], collection.GetGlyphShapingData(offset))) break;

            if (matches.Length == MaxContextLength) matches[i] = iterator.Index;

            i++;
            offset = iterator.Next();
        }

        iterator.Index = position;
        return i == sequence.Length;
    }
}