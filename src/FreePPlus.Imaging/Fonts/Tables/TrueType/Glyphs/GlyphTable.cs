// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using FreePPlus.Imaging.Fonts.Tables.General;
using FreePPlus.Imaging.Fonts.Tables.Woff;

namespace FreePPlus.Imaging.Fonts.Tables.TrueType.Glyphs;

//was previously: namespace SixLabors.Fonts.Tables.TrueType.Glyphs;

internal class GlyphTable : Table
{
    internal const string TableName = "glyf";
    private readonly GlyphLoader[] loaders;

    public GlyphTable(GlyphLoader[] glyphLoaders)
    {
        loaders = glyphLoaders;
    }

    public int GlyphCount => loaders.Length;

    // TODO: Make this non-virtual
    internal virtual GlyphVector GetGlyph(int index)
    {
        return loaders[index].CreateGlyph(this);
    }

    public static GlyphTable Load(FontReader reader)
    {
        var locations = reader.GetTable<IndexLocationTable>().GlyphOffsets;
        var fallbackEmptyBounds = reader.GetTable<HeadTable>().Bounds;

        using (var binaryReader = reader.GetReaderAtTablePosition(TableName))
        {
            return Load(binaryReader, reader.TableFormat, locations, fallbackEmptyBounds);
        }
    }

    public static GlyphTable Load(BigEndianBinaryReader reader, TableFormat format, uint[] locations,
        in Bounds fallbackEmptyBounds)
    {
        var empty = new EmptyGlyphLoader(fallbackEmptyBounds);
        var entryCount = locations.Length;
        var glyphCount = entryCount - 1; // last entry is a placeholder to the end of the table
        var glyphs = new GlyphLoader[glyphCount];

        // Special case for WOFF2 format where all glyphs need to be read in one go.
        if (format is TableFormat.Woff2) return new GlyphTable(Woff2Utils.LoadAllGlyphs(reader, empty));

        for (var i = 0; i < glyphCount; i++)
            if (locations[i] == locations[i + 1])
            {
                // This is an empty glyph;
                glyphs[i] = empty;
            }
            else
            {
                // Move to start of glyph.
                reader.Seek(locations[i], SeekOrigin.Begin);
                glyphs[i] = GlyphLoader.Load(reader);
            }

        return new GlyphTable(glyphs);
    }
}