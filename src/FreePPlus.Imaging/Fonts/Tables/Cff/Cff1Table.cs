// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

#pragma warning disable IDE0059
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal sealed class Cff1Table : Table, ICffTable
{
    internal const string TableName = "CFF "; // 4 chars

    private readonly CffGlyphData[] glyphs;

    public Cff1Table(CffFont cff1Font)
    {
        glyphs = cff1Font.Glyphs;
    }

    public int GlyphCount => glyphs.Length;

    public CffGlyphData GetGlyph(int index)
    {
        return glyphs[index];
    }

    public static Cff1Table? Load(FontReader fontReader)
    {
        if (!fontReader.TryGetReaderAtTablePosition(TableName, out var binaryReader)) return null;

        using (binaryReader)
        {
            return Load(binaryReader);
        }
    }

    public static Cff1Table Load(BigEndianBinaryReader reader)
    {
        // +------+---------------+----------------------------------------+
        // | Type | Name          | Description                            |
        // +======+===============+========================================+
        // | byte | majorVersion  | Format major version. Set to 1.        |
        // +------+---------------+----------------------------------------+
        // | byte | minorVersion  | Format minor version. Set to zero.     |
        // +------+---------------+----------------------------------------+
        // | byte | headerSize    | Header size (bytes).                   |
        // +------+---------------+----------------------------------------+
        // | byte | topDictLength | Length of Top DICT structure in bytes. |
        // +------+---------------+----------------------------------------+
        var position = reader.BaseStream.Position;
        var header = reader.ReadBytes(4);
        var major = header[0];
        var minor = header[1];
        var hdrSize = header[2];
        var offSize = header[3];

        switch (major)
        {
            case 1:
                CffParser parser = new();
                return new Cff1Table(parser.Load(reader, position));

            default:
                throw new NotSupportedException();
        }
    }
}