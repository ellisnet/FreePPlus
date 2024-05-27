// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal class CffPrivateDictionary
{
    public CffPrivateDictionary(byte[][] localSubrRawBuffers, int defaultWidthX, int nominalWidthX)
    {
        LocalSubrRawBuffers = localSubrRawBuffers;
        DefaultWidthX = defaultWidthX;
        NominalWidthX = nominalWidthX;
    }

    public byte[][] LocalSubrRawBuffers { get; set; }

    public int DefaultWidthX { get; set; }

    public int NominalWidthX { get; set; }
}