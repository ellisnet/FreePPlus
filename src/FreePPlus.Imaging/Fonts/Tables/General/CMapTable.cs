// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FreePPlus.Imaging.Fonts.Tables.General.CMap;
using FreePPlus.Imaging.Fonts.Unicode;
using FreePPlus.Imaging.Fonts.WellKnownIds;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.General;

//was previously: namespace SixLabors.Fonts.Tables.General;

internal sealed class CMapTable : Table
{
    internal const string TableName = "cmap";

    private readonly Format14SubTable[] format14SubTables = Array.Empty<Format14SubTable>();

    public CMapTable(IEnumerable<CMapSubTable> tables)
    {
        Tables = tables.OrderBy(t => GetPreferredPlatformOrder(t.Platform)).ToArray();
        format14SubTables = Tables.OfType<Format14SubTable>().ToArray();
    }

    internal CMapSubTable[] Tables { get; }

    private static int GetPreferredPlatformOrder(PlatformIDs platform)
    {
        return platform switch
        {
            PlatformIDs.Windows => 0,
            PlatformIDs.Unicode => 1,
            PlatformIDs.Macintosh => 2,
            _ => int.MaxValue
        };
    }

    public bool TryGetGlyphId(CodePoint codePoint, CodePoint? nextCodePoint, out ushort glyphId,
        out bool skipNextCodePoint)
    {
        skipNextCodePoint = false;
        if (TryGetGlyphId(codePoint, out glyphId))
        {
            // If there is a second codepoint, we are asked whether this is an UVS sequence
            // - If true, return a glyph Id.
            // - Otherwise, return 0.
            if (nextCodePoint != null && format14SubTables.Length > 0)
                foreach (var cmap14 in format14SubTables)
                {
                    var pairGlyphId = cmap14.CharacterPairToGlyphId(codePoint, glyphId, nextCodePoint.Value);
                    if (pairGlyphId > 0)
                    {
                        glyphId = pairGlyphId;
                        skipNextCodePoint = true;
                        return true;
                    }
                }

            return true;
        }

        return false;
    }

    private bool TryGetGlyphId(CodePoint codePoint, out ushort glyphId)
    {
        foreach (var t in Tables)
            // Keep looking until we have an index that's not the fallback.
            // Regardless of the encoding scheme, character codes that do
            // not correspond to any glyph in the font should be mapped to glyph index 0.
            // The glyph at this location must be a special glyph representing a missing character, commonly known as .notdef.
            if (t.TryGetGlyphId(codePoint, out glyphId) && glyphId > 0)
                return true;

        glyphId = 0;
        return false;
    }

    public static CMapTable Load(FontReader reader)
    {
        using var binaryReader = reader.GetReaderAtTablePosition(TableName);
        return Load(binaryReader);
    }

    public static CMapTable Load(BigEndianBinaryReader reader)
    {
        var version = reader.ReadUInt16();
        var numTables = reader.ReadUInt16();

        var encodings = new EncodingRecord[numTables];
        for (var i = 0; i < numTables; i++) encodings[i] = EncodingRecord.Read(reader);

        // foreach encoding we move forward looking for the subtables
        var tables = new List<CMapSubTable>(numTables);
        foreach (var encoding in encodings.GroupBy(x => x.Offset))
        {
            long offset = encoding.Key;
            reader.Seek(offset, SeekOrigin.Begin);

            // Subtable format.
            switch (reader.ReadUInt16())
            {
                case 0:
                    tables.AddRange(Format0SubTable.Load(encoding, reader));
                    break;
                case 4:
                    tables.AddRange(Format4SubTable.Load(encoding, reader));
                    break;
                case 12:
                    tables.AddRange(Format12SubTable.Load(encoding, reader));
                    break;
                case 14:
                    tables.AddRange(Format14SubTable.Load(encoding, reader, offset));
                    break;
            }
        }

        return new CMapTable(tables);
    }
}