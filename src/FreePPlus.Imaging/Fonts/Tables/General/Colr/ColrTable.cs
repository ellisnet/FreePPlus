// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;

#pragma warning disable IDE0059
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.General.Colr;

//was previously: namespace SixLabors.Fonts.Tables.General.Colr;

internal class ColrTable : Table
{
    internal const string TableName = "COLR";
    private readonly BaseGlyphRecord[] glyphRecords;
    private readonly LayerRecord[] layers;

    public ColrTable(BaseGlyphRecord[] glyphRecords, LayerRecord[] layers)
    {
        this.glyphRecords = glyphRecords;
        this.layers = layers;
    }

    public static ColrTable? Load(FontReader fontReader)
    {
        if (!fontReader.TryGetReaderAtTablePosition(TableName, out var binaryReader)) return null;

        using (binaryReader)
        {
            return Load(binaryReader);
        }
    }

    internal Span<LayerRecord> GetLayers(ushort glyph)
    {
        foreach (var g in glyphRecords)
            if (g.GlyphId == glyph)
                return layers.AsSpan().Slice(g.FirstLayerIndex, g.LayerCount);

        return Span<LayerRecord>.Empty;
    }

    public static ColrTable Load(BigEndianBinaryReader reader)
    {
        // HEADER

        // Type      | Name                   | Description
        // ----------|------------------------|----------------------------------------------------------------------------------------------------
        // uint16    | version                | Table version number(starts at 0).
        // uint16    | numBaseGlyphRecords    | Number of Base Glyph Records.
        // Offset32  | baseGlyphRecordsOffset | Offset(from beginning of COLR table) to Base Glyph records.
        // Offset32  | layerRecordsOffset     | Offset(from beginning of COLR table) to Layer Records.
        // uint16    | numLayerRecords        | Number of Layer Records.

        // Base Glyph Record
        // Type      | Name                   | Description
        // ----------|------------------------|----------------------------------------------------------------------------------------------------
        // uint16    | gID                    | Glyph ID of reference glyph. This glyph is for reference only and is not rendered for color.
        // uint16    | firstLayerIndex        | Index(from beginning of the Layer Records) to the layer record. There will be numLayers consecutive entries for this base glyph.
        // uint16    | numLayers              | Number of color layers associated with this glyph.

        // Layer Record
        // Type      | Name                   | Description
        // ----------|------------------------|----------------------------------------------------------------------------------------------------
        // uint16    | gID                    | Glyph ID of layer glyph (must be in z-order from bottom to top).
        // uint16    | paletteIndex           | Index value to use with a selected color palette. This value must be less than numPaletteEntries in
        //           |                        | > the CPAL table. A palette entry index value of 0xFFFF is a special case indicating that the text
        //           |                        | > foreground color (defined by a higher-level client) should be used and shall not be treated as
        //           |                        | > actual index into CPAL ColorRecord array.
        var version = reader.ReadUInt16();
        var numBaseGlyphRecords = reader.ReadUInt16();
        var baseGlyphRecordsOffset = reader.ReadOffset32();
        var layerRecordsOffset = reader.ReadOffset32();
        var numLayerRecords = reader.ReadUInt16();

        reader.Seek(baseGlyphRecordsOffset, SeekOrigin.Begin);

        var glyphs = new BaseGlyphRecord[numBaseGlyphRecords];
        var layers = new LayerRecord[numLayerRecords];
        for (var i = 0; i < numBaseGlyphRecords; i++)
        {
            var gi = reader.ReadUInt16();
            var idx = reader.ReadUInt16();
            var num = reader.ReadUInt16();
            glyphs[i] = new BaseGlyphRecord(gi, idx, num);
        }

        reader.Seek(layerRecordsOffset, SeekOrigin.Begin);

        for (var i = 0; i < numLayerRecords; i++)
        {
            var gi = reader.ReadUInt16();
            var pi = reader.ReadUInt16();
            layers[i] = new LayerRecord(gi, pi);
        }

        return new ColrTable(glyphs, layers);
    }
}