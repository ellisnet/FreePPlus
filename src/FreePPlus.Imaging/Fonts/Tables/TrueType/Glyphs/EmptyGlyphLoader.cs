// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.TrueType.Glyphs;

//was previously: namespace SixLabors.Fonts.Tables.TrueType.Glyphs;

internal class EmptyGlyphLoader : GlyphLoader
{
    private readonly Bounds fallbackEmptyBounds;
    private GlyphVector? glyph;
    private bool loop;

    public EmptyGlyphLoader(Bounds fallbackEmptyBounds)
    {
        this.fallbackEmptyBounds = fallbackEmptyBounds;
    }

    public override GlyphVector CreateGlyph(GlyphTable table)
    {
        if (loop)
        {
            if (glyph is null) glyph = GlyphVector.Empty(fallbackEmptyBounds);

            return glyph.Value;
        }

        loop = true;
        return table.GetGlyph(0);
    }
}