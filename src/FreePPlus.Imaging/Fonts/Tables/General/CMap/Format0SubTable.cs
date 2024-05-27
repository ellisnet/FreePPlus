// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using FreePPlus.Imaging.Fonts.Unicode;
using FreePPlus.Imaging.Fonts.WellKnownIds;

namespace FreePPlus.Imaging.Fonts.Tables.General.CMap;

//was previously: namespace SixLabors.Fonts.Tables.General.CMap;

internal sealed class Format0SubTable : CMapSubTable
{
    public Format0SubTable(ushort language, PlatformIDs platform, ushort encoding, byte[] glyphIds)
        : base(platform, encoding, 0)
    {
        Language = language;
        GlyphIds = glyphIds;
    }

    public ushort Language { get; }

    public byte[] GlyphIds { get; }

    public override bool TryGetGlyphId(CodePoint codePoint, out ushort glyphId)
    {
        var b = codePoint.Value;
        if (b >= GlyphIds.Length)
        {
            glyphId = 0;
            return false;
        }

        glyphId = GlyphIds[b];
        return true;
    }

    public static IEnumerable<Format0SubTable> Load(IEnumerable<EncodingRecord> encodings, BigEndianBinaryReader reader)
    {
        // format has already been read by this point skip it
        var length = reader.ReadUInt16();
        var language = reader.ReadUInt16();
        var glyphsCount = length - 6;

        // char 'A' == 65 thus glyph = glyphIds[65];
        var glyphIds = reader.ReadBytes(glyphsCount);

        foreach (var encoding in encodings)
            yield return new Format0SubTable(language, encoding.PlatformID, encoding.EncodingID, glyphIds);
    }
}