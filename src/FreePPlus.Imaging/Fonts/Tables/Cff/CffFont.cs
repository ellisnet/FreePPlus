// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal class CffFont
{
    public CffFont(string name, CffTopDictionary metrics, CffGlyphData[] glyphs)
    {
        FontName = name;
        Metrics = metrics;
        Glyphs = glyphs;
    }

    public string FontName { get; set; }

    public CffTopDictionary Metrics { get; set; }

    public CffGlyphData[] Glyphs { get; }
}