// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal sealed class Cff2Table : Table, ICffTable
{
    internal const string TableName = "CFF2";

    private readonly CffGlyphData[] glyphs;

    public Cff2Table(CffFont cff1Font)
    {
        glyphs = cff1Font.Glyphs;
    }

    public int GlyphCount => glyphs.Length;

    public CffGlyphData GetGlyph(int index)
    {
        return glyphs[index];
    }

    public static Cff2Table? Load(FontReader fontReader)
    {
        if (!fontReader.TryGetReaderAtTablePosition(TableName, out var binaryReader)) return null;

        using (binaryReader)
        {
            return Load(binaryReader);
        }
    }

    public static Cff2Table Load(BigEndianBinaryReader reader)
    {
        throw new NotSupportedException("CFF2 Fonts are not currently supported.");
    }
}