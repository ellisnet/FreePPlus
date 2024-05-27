// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.TrueType.Glyphs;

//was previously: namespace SixLabors.Fonts.Tables.TrueType.Glyphs;

internal sealed class TransformedGlyphLoader : GlyphLoader
{
    private readonly GlyphVector glyphVector;

    public TransformedGlyphLoader(GlyphVector glyphVector)
    {
        this.glyphVector = glyphVector;
    }

    public override GlyphVector CreateGlyph(GlyphTable table)
    {
        return glyphVector;
    }
}