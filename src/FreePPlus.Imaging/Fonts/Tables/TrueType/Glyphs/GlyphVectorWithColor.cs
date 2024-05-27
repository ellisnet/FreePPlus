// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.TrueType.Glyphs;

//was previously: namespace SixLabors.Fonts.Tables.TrueType.Glyphs;

internal readonly struct GlyphVectorWithColor
{
    internal GlyphVectorWithColor(GlyphVector vector, GlyphColor color)
    {
        Vector = vector;
        Color = color;
    }

    public GlyphVector Vector { get; }

    public GlyphColor Color { get; }
}