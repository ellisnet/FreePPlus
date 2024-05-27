// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Diagnostics;
using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

/// <summary>
///     Features provide information about how to use the glyphs in a font to render a script or language.
///     For example, an Arabic font might have a feature for substituting initial glyph forms, and a Kanji font
///     might have a feature for positioning glyphs vertically. All OpenType Layout features define data for
///     glyph substitution, glyph positioning, or both.
///     <see href="https://docs.microsoft.com/en-us/typography/opentype/spec/featurelist" />
///     <see href="https://docs.microsoft.com/en-us/typography/opentype/spec/chapter2#feature-list-table" />
/// </summary>
internal class FeatureListTable
{
    private FeatureListTable(FeatureTable[] featureTables)
    {
        FeatureTables = featureTables;
    }

    public FeatureTable[] FeatureTables { get; }

    public static FeatureListTable Load(BigEndianBinaryReader reader, long offset)
    {
        // FeatureList
        // +---------------+------------------------------+-----------------------------------------------------------------------------------------------------------------+
        // | Type          | Name                         | Description                                                                                                     |
        // +===============+==============================+=================================================================================================================+
        // | uint16        | featureCount                 | Number of FeatureRecords in this table                                                                          |
        // +---------------+------------------------------+-----------------------------------------------------------------------------------------------------------------+
        // | FeatureRecord | featureRecords[featureCount] | Array of FeatureRecords — zero-based (first feature has FeatureIndex = 0), listed alphabetically by feature tag |
        // +---------------+------------------------------+-----------------------------------------------------------------------------------------------------------------+
        reader.Seek(offset, SeekOrigin.Begin);

        var featureCount = reader.ReadUInt16();
        var featureRecords = new FeatureRecord[featureCount];
        for (var i = 0; i < featureRecords.Length; i++)
        {
            // FeatureRecord
            // +----------+---------------+--------------------------------------------------------+
            // | Type     | Name          | Description                                            |
            // +==========+===============+========================================================+
            // | Tag      | featureTag    | 4-byte feature identification tag                      |
            // +----------+---------------+--------------------------------------------------------+
            // | Offset16 | featureOffset | Offset to Feature table, from beginning of FeatureList |
            // +----------+---------------+--------------------------------------------------------+
            var featureTag = reader.ReadUInt32();
            var featureOffset = reader.ReadOffset16();
            featureRecords[i] = new FeatureRecord(featureTag, featureOffset);
        }

        // Load the other table features.
        // We do this last to avoid excessive seeking.
        var featureTables = new FeatureTable[featureCount];
        for (var i = 0; i < featureTables.Length; i++)
        {
            var featureRecord = featureRecords[i];
            featureTables[i] =
                FeatureTable.Load(featureRecord.FeatureTag, reader, offset + featureRecord.FeatureOffset);
        }

        return new FeatureListTable(featureTables);
    }

    [DebuggerDisplay("FeatureTag: {FeatureTag}, Offset: {FeatureOffset}")]
    private readonly struct FeatureRecord
    {
        public FeatureRecord(uint featureTag, ushort featureOffset)
        {
            FeatureTag = new Tag(featureTag);
            FeatureOffset = featureOffset;
        }

        public Tag FeatureTag { get; }

        public ushort FeatureOffset { get; }
    }
}

[DebuggerDisplay("Tag: {FeatureTag}")]
internal sealed class FeatureTable
{
    private FeatureTable(Tag featureTag, ushort[] lookupListIndices)
    {
        FeatureTag = featureTag;
        LookupListIndices = lookupListIndices;
    }

    public Tag FeatureTag { get; }

    public ushort[] LookupListIndices { get; }

    public static FeatureTable Load(Tag featureTag, BigEndianBinaryReader reader, long offset)
    {
        // FeatureListTable
        // +----------+-------------------------------------+--------------------------------------------------------------------------------------------------------------+
        // | Type     | Name                                | Description                                                                                                  |
        // +==========+=====================================+==============================================================================================================+
        // | Offset16 | featureParamsOffset                 | Offset from start of Feature table to FeatureParams table, if defined for the feature and present, else NULL |
        // +----------+-------------------------------------+--------------------------------------------------------------------------------------------------------------+
        // | uint16   | lookupIndexCount                    | Number of LookupList indices for this feature                                                                |
        // +----------+-------------------------------------+--------------------------------------------------------------------------------------------------------------+
        // | uint16   | lookupListIndices[lookupIndexCount] | Array of indices into the LookupList — zero-based (first lookup is LookupListIndex = 0)                      |
        // +----------+-------------------------------------+--------------------------------------------------------------------------------------------------------------+
        reader.Seek(offset, SeekOrigin.Begin);

        // TODO: How do we use this?
        var featureParamsOffset = reader.ReadOffset16();
        var lookupIndexCount = reader.ReadUInt16();

        var lookupListIndices = reader.ReadUInt16Array(lookupIndexCount);
        return new FeatureTable(featureTag, lookupListIndices);
    }
}