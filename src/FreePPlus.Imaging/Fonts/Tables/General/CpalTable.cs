// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

#pragma warning disable IDE0059
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.General;

//was previously: namespace SixLabors.Fonts.Tables.General;

internal class CpalTable : Table
{
    internal const string TableName = "CPAL";
    private readonly GlyphColor[] paletteEntries;
    private readonly ushort[] paletteOffsets;

    public CpalTable(ushort[] paletteOffsets, GlyphColor[] paletteEntries)
    {
        this.paletteEntries = paletteEntries;
        this.paletteOffsets = paletteOffsets;
    }

    public GlyphColor GetGlyphColor(int paletteIndex, int paletteEntryIndex)
    {
        return paletteEntries[paletteOffsets[paletteIndex] + paletteEntryIndex];
    }

    public static CpalTable? Load(FontReader fontReader)
    {
        if (!fontReader.TryGetReaderAtTablePosition(TableName, out var binaryReader)) return null;

        using (binaryReader)
        {
            return Load(binaryReader);
        }
    }

    public static CpalTable Load(BigEndianBinaryReader reader)
    {
        // FORMAT 0

        // Type      | Name                            | Description
        // ----------|---------------------------------|----------------------------------------------------------------------------------------------------
        // uint16    | version                         | Table version number (=0).
        // uint16    | numPaletteEntries               | Number of palette entries in each palette.
        // uint16    | numPalettes                     | Number of palettes in the table.
        // uint16    | numColorRecords                 | Total number of color records, combined for all palettes.
        // Offset32  | offsetFirstColorRecord          | Offset from the beginning of CPAL table to the first ColorRecord.
        // uint16    | colorRecordIndices[numPalettes] | Index of each palette’s first color record in the combined color record array.

        // additional format 1 fields
        // Offset32  | offsetPaletteTypeArray          | Offset from the beginning of CPAL table to the Palette Type Array. Set to 0 if no array is provided.
        // Offset32  | offsetPaletteLabelArray         | Offset from the beginning of CPAL table to the Palette Labels Array. Set to 0 if no array is provided.
        // Offset32  | offsetPaletteEntryLabelArray    | Offset from the beginning of CPAL table to the Palette Entry Label Array.Set to 0 if no array is provided.
        var version = reader.ReadUInt16();
        var numPaletteEntries = reader.ReadUInt16();
        var numPalettes = reader.ReadUInt16();
        var numColorRecords = reader.ReadUInt16();
        var offsetFirstColorRecord = reader.ReadOffset32();

        var colorRecordIndices = reader.ReadUInt16Array(numPalettes);

        uint offsetPaletteTypeArray = 0;
        uint offsetPaletteLabelArray = 0;
        uint offsetPaletteEntryLabelArray = 0;
        if (version == 1)
        {
            offsetPaletteTypeArray = reader.ReadOffset32();
            offsetPaletteLabelArray = reader.ReadOffset32();
            offsetPaletteEntryLabelArray = reader.ReadOffset32();
        }

        reader.Seek(offsetFirstColorRecord, SeekOrigin.Begin);
        var palettes = new GlyphColor[numColorRecords];
        for (var n = 0; n < numColorRecords; n++)
        {
            var blue = reader.ReadByte();
            var green = reader.ReadByte();
            var red = reader.ReadByte();
            var alpha = reader.ReadByte();
            palettes[n] = new GlyphColor(blue, green, red, alpha);
        }

        return new CpalTable(colorRecordIndices, palettes);
    }
}