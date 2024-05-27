// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal class FontDict
{
    public FontDict(int name, int dictSize, int dictOffset)
    {
        FontName = name;
        PrivateDicSize = dictSize;
        PrivateDicOffset = dictOffset;
    }

    public int FontName { get; set; }

    public int PrivateDicSize { get; }

    public int PrivateDicOffset { get; }

    public byte[][]? LocalSubr { get; set; }
}