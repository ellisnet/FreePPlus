// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal class CffTopDictionary
{
    public CffTopDictionary()
    {
        CidFontInfo = new CidFontInfo();
    }

    public string? Version { get; set; }

    public string? Notice { get; set; }

    public string? CopyRight { get; set; }

    public string? FullName { get; set; }

    public string? FamilyName { get; set; }

    public string? Weight { get; set; }

    public double UnderlinePosition { get; set; }

    public double UnderlineThickness { get; set; }

    public double[] FontBBox { get; set; } = Array.Empty<double>();

    public CidFontInfo CidFontInfo { get; set; }
}