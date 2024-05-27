// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;
using FreePPlus.Imaging.Fonts.Tables.Cff;
using FreePPlus.Imaging.Fonts.Tables.General;
using FreePPlus.Imaging.Fonts.Tables.General.Colr;
using FreePPlus.Imaging.Fonts.Tables.General.Kern;
using FreePPlus.Imaging.Fonts.Tables.General.Name;
using FreePPlus.Imaging.Fonts.Tables.General.Post;
using FreePPlus.Imaging.Fonts.Tables.TrueType;
using FreePPlus.Imaging.Fonts.Tables.TrueType.Glyphs;
using FreePPlus.Imaging.Fonts.Tables.TrueType.Hinting;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables;

//was previously: namespace SixLabors.Fonts.Tables;

internal class TableLoader
{
    private readonly Dictionary<string, Func<FontReader, Table?>> loaders = new();
    private readonly Dictionary<Type, string> types = new();
    private readonly Dictionary<Type, Func<FontReader, Table?>> typesLoaders = new();

    public TableLoader()
    {
        // We will hard code mapping registration in here for all the tables
        Register(NameTable.TableName, NameTable.Load);
        Register(CMapTable.TableName, CMapTable.Load);
        Register(HeadTable.TableName, HeadTable.Load);
        Register(HorizontalHeadTable.TableName, HorizontalHeadTable.Load);
        Register(HorizontalMetricsTable.TableName, HorizontalMetricsTable.Load);
        Register(VerticalHeadTable.TableName, VerticalHeadTable.Load);
        Register(VerticalMetricsTable.TableName, VerticalMetricsTable.Load);
        Register(MaximumProfileTable.TableName, MaximumProfileTable.Load);
        Register(OS2Table.TableName, OS2Table.Load);
        Register(IndexLocationTable.TableName, IndexLocationTable.Load);
        Register(GlyphTable.TableName, GlyphTable.Load);
        Register(KerningTable.TableName, KerningTable.Load);
        Register(ColrTable.TableName, ColrTable.Load);
        Register(CpalTable.TableName, CpalTable.Load);
        Register(GPosTable.TableName, GPosTable.Load);
        Register(GSubTable.TableName, GSubTable.Load);
        Register(CvtTable.TableName, CvtTable.Load);
        Register(FpgmTable.TableName, FpgmTable.Load);
        Register(PrepTable.TableName, PrepTable.Load);
        Register(GlyphDefinitionTable.TableName, GlyphDefinitionTable.Load);
        Register(PostTable.TableName, PostTable.Load);
        Register(Cff1Table.TableName, Cff1Table.Load);
        Register(Cff2Table.TableName, Cff2Table.Load);
    }

    public static TableLoader Default { get; } = new();

    public string? GetTag(Type type)
    {
        types.TryGetValue(type, out var value);

        return value;
    }

    public string GetTag<TType>()
    {
        types.TryGetValue(typeof(TType), out var value);
        return value!;
    }

    internal IEnumerable<Type> RegisteredTypes()
    {
        return types.Keys;
    }

    internal IEnumerable<string> RegisteredTags()
    {
        return types.Values;
    }

    private void Register<T>(string tag, Func<FontReader, T?> createFunc)
        where T : Table
    {
        lock (loaders)
        {
            if (!loaders.ContainsKey(tag))
            {
                loaders.Add(tag, createFunc);
                types.Add(typeof(T), tag);
                typesLoaders.Add(typeof(T), createFunc);
            }
        }
    }

    internal Table? Load(string tag, FontReader reader) // loader missing? register an unknown type loader and carry on
    {
    return loaders.TryGetValue(tag, out var func)
        ? func.Invoke(reader)
        : new UnknownTable(tag);
    }

    internal TTable? Load<TTable>(FontReader reader)
        where TTable : Table
    {
        // loader missing register an unknown type loader and carry on
        if (typesLoaders.TryGetValue(typeof(TTable), out var func)) return (TTable)func.Invoke(reader);

        throw new Exception("Font table not registered.");
    }
}