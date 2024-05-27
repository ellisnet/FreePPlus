// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GSub;
using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.Shapers;
using FreePPlus.Imaging.Fonts.Unicode;

#pragma warning disable IDE0059
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

/// <summary>
///     The Glyph Substitution (GSUB) table provides data for substitution of glyphs for appropriate rendering of scripts,
///     such as cursively-connecting forms in Arabic script, or for advanced typographic effects, such as ligatures.
///     <see href="https://docs.microsoft.com/en-us/typography/opentype/spec/gsub" />
/// </summary>
internal class GSubTable : Table
{
    internal const string TableName = "GSUB";

    public GSubTable(ScriptList? scriptList, FeatureListTable featureList, LookupListTable lookupList)
    {
        ScriptList = scriptList;
        FeatureList = featureList;
        LookupList = lookupList;
    }

    public ScriptList? ScriptList { get; }

    public FeatureListTable FeatureList { get; }

    public LookupListTable LookupList { get; }

    public static GSubTable? Load(FontReader fontReader)
    {
        if (!fontReader.TryGetReaderAtTablePosition(TableName, out var binaryReader)) return null;

        using (binaryReader)
        {
            return Load(binaryReader);
        }
    }

    internal static GSubTable Load(BigEndianBinaryReader reader)
    {
        // GSUB Header, Version 1.0
        // +----------+-------------------+-----------------------------------------------------------+
        // | Type     | Name              | Description                                               |
        // +==========+===================+===========================================================+
        // | uint16   | majorVersion      | Major version of the GSUB table, = 1                      |
        // +----------+-------------------+-----------------------------------------------------------+
        // | uint16   | minorVersion      | Minor version of the GSUB table, = 0                      |
        // +----------+-------------------+-----------------------------------------------------------+
        // | Offset16 | scriptListOffset  | Offset to ScriptList table, from beginning of GSUB table  |
        // +----------+-------------------+-----------------------------------------------------------+
        // | Offset16 | featureListOffset | Offset to FeatureList table, from beginning of GSUB table |
        // +----------+-------------------+-----------------------------------------------------------+
        // | Offset16 | lookupListOffset  | Offset to LookupList table, from beginning of GSUB table  |
        // +----------+-------------------+-----------------------------------------------------------+

        // GSUB Header, Version 1.1
        // +----------+-------------------------+-------------------------------------------------------------------------------+
        // | Type     | Name                    | Description                                                                   |
        // +==========+=========================+===============================================================================+
        // | uint16   | majorVersion            | Major version of the GSUB table, = 1                                          |
        // +----------+-------------------------+-------------------------------------------------------------------------------+
        // | uint16   | minorVersion            | Minor version of the GSUB table, = 1                                          |
        // +----------+-------------------------+-------------------------------------------------------------------------------+
        // | Offset16 | scriptListOffset        | Offset to ScriptList table, from beginning of GSUB table                      |
        // +----------+-------------------------+-------------------------------------------------------------------------------+
        // | Offset16 | featureListOffset       | Offset to FeatureList table, from beginning of GSUB table                     |
        // +----------+-------------------------+-------------------------------------------------------------------------------+
        // | Offset16 | lookupListOffset        | Offset to LookupList table, from beginning of GSUB table                      |
        // +----------+-------------------------+-------------------------------------------------------------------------------+
        // | Offset32 | featureVariationsOffset | Offset to FeatureVariations table, from beginning of GSUB table (may be NULL) |
        // +----------+-------------------------+-------------------------------------------------------------------------------+
        var majorVersion = reader.ReadUInt16();
        var minorVersion = reader.ReadUInt16();

        var scriptListOffset = reader.ReadOffset16();
        var featureListOffset = reader.ReadOffset16();
        var lookupListOffset = reader.ReadOffset16();
        var featureVariationsOffset = minorVersion == 1 ? reader.ReadOffset32() : 0;

        // TODO: Optimization. Allow only reading the scriptList.
        var scriptList = ScriptList.Load(reader, scriptListOffset);

        var featureList = FeatureListTable.Load(reader, featureListOffset);

        var lookupList = LookupListTable.Load(reader, lookupListOffset);

        // TODO: Feature Variations.
        return new GSubTable(scriptList, featureList, lookupList);
    }

    public void ApplySubstitution(FontMetrics fontMetrics, GlyphSubstitutionCollection collection)
    {
        // Set max constraints to prevent OutOfMemoryException or infinite loops from attacks.
        var maxCount = AdvancedTypographicUtils.GetMaxAllowableShapingCollectionCount(collection.Count);
        var maxOperationsCount = AdvancedTypographicUtils.GetMaxAllowableShapingOperationsCount(collection.Count);
        var currentOperations = 0;

        for (var i = 0; i < collection.Count; i++)
        {
            // Choose a shaper based on the script.
            // This determines which features to apply to which glyphs.
            var current = CodePoint.GetScriptClass(collection.GetGlyphShapingData(i).CodePoint);
            var shaper = ShaperFactory.Create(current, collection.TextOptions);

            var index = i;
            var count = 1;
            while (i < collection.Count - 1)
            {
                // We want to assign the same feature lookups to individual sections of the text rather
                // than the text as a whole to ensure that different language shapers do not interfere
                // with each other when the text contains multiple languages.
                var nextData = collection.GetGlyphShapingData(i + 1);
                var next = CodePoint.GetScriptClass(nextData.CodePoint);
                if (next is not ScriptClass.Common and not ScriptClass.Unknown and not ScriptClass.Inherited &&
                    next != current) break;

                i++;
                count++;

                if (i >= maxCount) break;
            }

            // Assign Substitution features to each glyph.
            shaper.AssignFeatures(collection, index, count);

            // Shapers can adjust the count.
            count = Math.Min(count, collection.Count - index);

            var stageFeatures = shaper.GetShapingStageFeatures();

            var currentCount = collection.Count;
            SkippingGlyphIterator iterator = new(fontMetrics, collection, index, default);
            foreach (var stageFeature in stageFeatures)
            {
                if (!TryGetFeatureLookups(in stageFeature, current, out var lookups)) continue;

                // Apply features in order.
                foreach (var featureLookup in lookups)
                {
                    var feature = featureLookup.Feature;
                    iterator.Reset(index, featureLookup.LookupTable.LookupFlags);

                    while (iterator.Index < index + count)
                    {
                        if (collection.Count >= maxCount || currentOperations++ >= maxOperationsCount) return;

                        var glyphFeatures = collection.GetGlyphShapingData(iterator.Index).Features;
                        if (!HasFeature(glyphFeatures, in feature))
                        {
                            iterator.Next();
                            continue;
                        }

                        featureLookup.LookupTable.TrySubstitution(fontMetrics, this, collection, featureLookup.Feature,
                            iterator.Index, count - (iterator.Index - index));
                        iterator.Next();

                        // Account for substitutions changing the length of the collection.
                        if (collection.Count != currentCount)
                        {
                            count -= currentCount - collection.Count;
                            currentCount = collection.Count;
                        }
                    }
                }
            }
        }
    }

    private bool TryGetFeatureLookups(
        in Tag stageFeature,
        ScriptClass script,
        [NotNullWhen(true)] out List<(Tag Feature, ushort Index, LookupTable LookupTable)>? value)
    {
        if (ScriptList is null)
        {
            value = null;
            return false;
        }

        var scriptListTable = ScriptList.Default();
        var tags = UnicodeScriptTagMap.Instance[script];
        for (var i = 0; i < tags.Length; i++)
            if (ScriptList.TryGetValue(tags[i].Value, out var table))
            {
                scriptListTable = table;
                break;
            }

        var defaultLangSysTable = scriptListTable.DefaultLangSysTable;
        if (defaultLangSysTable != null)
        {
            value = GetFeatureLookups(stageFeature, defaultLangSysTable);
            return value.Count > 0;
        }

        value = GetFeatureLookups(stageFeature, scriptListTable.LangSysTables);
        return value.Count > 0;
    }

    private List<(Tag Feature, ushort Index, LookupTable LookupTable)> GetFeatureLookups(in Tag stageFeature,
        params LangSysTable[] langSysTables)
    {
        List<(Tag Feature, ushort Index, LookupTable LookupTable)> lookups = new();
        for (var i = 0; i < langSysTables.Length; i++)
        {
            var featureIndices = langSysTables[i].FeatureIndices;
            for (var j = 0; j < featureIndices.Length; j++)
            {
                var featureTable = FeatureList.FeatureTables[featureIndices[j]];
                var feature = featureTable.FeatureTag;

                if (stageFeature != feature) continue;

                var lookupListIndices = featureTable.LookupListIndices;
                for (var k = 0; k < lookupListIndices.Length; k++)
                {
                    var lookupIndex = lookupListIndices[k];
                    var lookupTable = LookupList.LookupTables[lookupIndex];
                    lookups.Add(new ValueTuple<Tag, ushort, LookupTable>(feature, lookupIndex, lookupTable));
                }
            }
        }

        lookups.Sort((x, y) => x.Index - y.Index);
        return lookups;
    }

    private static bool HasFeature(List<TagEntry> glyphFeatures, in Tag feature)
    {
        for (var i = 0; i < glyphFeatures.Count; i++)
        {
            var entry = glyphFeatures[i];
            if (entry.Tag == feature && entry.Enabled) return true;
        }

        return false;
    }
}