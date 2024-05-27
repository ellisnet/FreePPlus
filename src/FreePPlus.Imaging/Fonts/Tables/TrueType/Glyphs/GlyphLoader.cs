// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.TrueType.Glyphs;

//was previously: namespace SixLabors.Fonts.Tables.TrueType.Glyphs;

internal abstract class GlyphLoader
{
    public abstract GlyphVector CreateGlyph(GlyphTable table);

    public static GlyphLoader Load(BigEndianBinaryReader reader)
    {
        var contoursCount = reader.ReadInt16();
        var bounds = Bounds.Load(reader);

        if (contoursCount >= 0)
            return SimpleGlyphLoader.LoadSimpleGlyph(reader, contoursCount, bounds);
        return CompositeGlyphLoader.LoadCompositeGlyph(reader, bounds);
    }
}