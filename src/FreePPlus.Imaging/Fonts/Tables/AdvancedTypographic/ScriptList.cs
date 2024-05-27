// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.IO;

#pragma warning disable IDE0059
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

/// <summary>
///     OpenType Layout fonts may contain one or more groups of glyphs used to render various scripts,
///     which are enumerated in a ScriptList table. Both the GSUB and GPOS tables define
///     Script List tables (ScriptList):
///     <see href="https://docs.microsoft.com/en-us/typography/opentype/spec/chapter2#slTbl_sRec" />
/// </summary>
internal sealed class ScriptList : Dictionary<Tag, ScriptListTable>
{
    private readonly Tag scriptTag;

    private ScriptList(Tag scriptTag)
    {
        this.scriptTag = scriptTag;
    }

    public static ScriptList? Load(BigEndianBinaryReader reader, long offset)
    {
        // ScriptListTable
        // +--------------+----------------------------+-------------------------------------------------------------+
        // | Type         | Name                       | Description                                                 |
        // +==============+============================+=============================================================+
        // | uint16       | scriptCount                | Number of ScriptRecords                                     |
        // +--------------+----------------------------+-------------------------------------------------------------+
        // | ScriptRecord | scriptRecords[scriptCount] | Array of ScriptRecords, listed alphabetically by script tag |
        // +--------------+----------------------------+-------------------------------------------------------------+
        reader.Seek(offset, SeekOrigin.Begin);

        var scriptCount = reader.ReadUInt16();

        // Read records (tags and table offsets)
        var scriptTags = new Tag[scriptCount];
        var scriptOffsets = new ushort[scriptCount];

        for (var i = 0; i < scriptTags.Length; i++)
        {
            scriptTags[i] = reader.ReadUInt32();
            scriptOffsets[i] = reader.ReadUInt16();
        }

        // Read each table and add it to the dictionary
        ScriptList? scriptList = null;
        for (var i = 0; i < scriptCount; ++i)
        {
            var scriptTag = scriptTags[i];
            if (i == 0) scriptList = new ScriptList(scriptTag);

            var scriptTable = ScriptListTable.Load(scriptTag, reader, offset + scriptOffsets[i]);
            scriptList!.Add(scriptTag, scriptTable);
        }

        return scriptList;
    }

    // Dictionaries are unordered.
    public ScriptListTable Default()
    {
        return this[scriptTag];
    }
}

internal sealed class ScriptListTable
{
    private ScriptListTable(LangSysTable[] langSysTables, LangSysTable? defaultLang, Tag scriptTag)
    {
        LangSysTables = langSysTables;
        DefaultLangSysTable = defaultLang;
        ScriptTag = scriptTag;
    }

    public Tag ScriptTag { get; }

    public LangSysTable? DefaultLangSysTable { get; }

    public LangSysTable[] LangSysTables { get; }

    public static ScriptListTable Load(Tag scriptTag, BigEndianBinaryReader reader, long offset)
    {
        // ScriptListTable
        // +---------------+------------------------------+-------------------------------------------------------------------------------+
        // | Type          | Name                         | Description                                                                   |
        // +===============+==============================+===============================================================================+
        // | Offset16      | defaultLangSysOffset         | Offset to default LangSys table, from beginning of Script table — may be NULL |
        // +---------------+------------------------------+-------------------------------------------------------------------------------+
        // | uint16        | langSysCount                 | Number of LangSysRecords for this script — excluding the default LangSys      |
        // +---------------+------------------------------+-------------------------------------------------------------------------------+
        // | LangSysRecord | langSysRecords[langSysCount] | Array of LangSysRecords, listed alphabetically by LangSys tag                 |
        // +---------------+------------------------------+-------------------------------------------------------------------------------+
        reader.Seek(offset, SeekOrigin.Begin);

        var defaultLangSysOffset = reader.ReadOffset16();
        var langSysCount = reader.ReadUInt16();

        var langSysRecords = new LangSysRecord[langSysCount];
        for (var i = 0; i < langSysRecords.Length; i++)
        {
            // LangSysRecord
            // +----------+---------------+---------------------------------------------------------+
            // | Type     | Name          | Description                                             |
            // +==========+===============+=========================================================+
            // | Tag      | langSysTag    | 4-byte LangSysTag identifier                            |
            // +----------+---------------+---------------------------------------------------------+
            // | Offset16 | langSysOffset | Offset to LangSys table, from beginning of Script table |
            // +----------+---------------+---------------------------------------------------------+
            var langSysTag = reader.ReadUInt32();
            var langSysOffset = reader.ReadOffset16();
            langSysRecords[i] = new LangSysRecord(langSysTag, langSysOffset);
        }

        // Load the default table.
        LangSysTable? defaultLangSysTable = null;
        if (defaultLangSysOffset > 0) defaultLangSysTable = LangSysTable.Load(0, reader, offset + defaultLangSysOffset);

        // Load the other table features.
        // We do this last to avoid excessive seeking.
        var langSysTables = new LangSysTable[langSysCount];
        for (var i = 0; i < langSysTables.Length; i++)
        {
            var langSysRecord = langSysRecords[i];
            langSysTables[i] =
                LangSysTable.Load(langSysRecord.LangSysTag, reader, offset + langSysRecord.LangSysOffset);
        }

        return new ScriptListTable(langSysTables, defaultLangSysTable, scriptTag);
    }

    private readonly struct LangSysRecord
    {
        public LangSysRecord(uint langSysTag, ushort langSysOffset)
        {
            LangSysTag = langSysTag;
            LangSysOffset = langSysOffset;
        }

        public uint LangSysTag { get; }

        public ushort LangSysOffset { get; }
    }
}

internal sealed class LangSysTable
{
    private LangSysTable(uint langSysTag, ushort requiredFeatureIndex, ushort[] featureIndices)
    {
        LangSysTag = langSysTag;
        RequiredFeatureIndex = requiredFeatureIndex;
        FeatureIndices = featureIndices;
    }

    public uint LangSysTag { get; }

    public ushort RequiredFeatureIndex { get; }

    public ushort[] FeatureIndices { get; } = Array.Empty<ushort>();

    public static LangSysTable Load(uint langSysTag, BigEndianBinaryReader reader, long offset)
    {
        // +----------+-----------------------------------+-----------------------------------------------------------------------------------------+
        // | Type     | Name                              | Description                                                                             |
        // +==========+===================================+=========================================================================================+
        // | Offset16 | lookupOrderOffset                 | = NULL(reserved for an offset to a reordering table)                                   |
        // +----------+-----------------------------------+-----------------------------------------------------------------------------------------+
        // | uint16   | requiredFeatureIndex              | Index of a feature required for this language system; if no required features = 0xFFFF  |
        // +----------+-----------------------------------+-----------------------------------------------------------------------------------------+
        // | uint16   | featureIndexCount                 | Number of feature index values for this language system — excludes the required feature |
        // +----------+-----------------------------------+-----------------------------------------------------------------------------------------+
        // | uint16   | featureIndices[featureIndexCount] | Array of indices into the FeatureList, in arbitrary order                               |
        // +----------+-----------------------------------+-----------------------------------------------------------------------------------------+
        reader.Seek(offset, SeekOrigin.Begin);
        var lookupOrderOffset = reader.ReadOffset16();
        var requiredFeatureIndex = reader.ReadUInt16();
        var featureIndexCount = reader.ReadUInt16();

        var featureIndices = reader.ReadUInt16Array(featureIndexCount);
        return new LangSysTable(langSysTag, requiredFeatureIndex, featureIndices);
    }
}