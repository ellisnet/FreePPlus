// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Diagnostics;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

[DebuggerDisplay("StartGlyphId: {StartGlyphId}, EndGlyphId: {EndGlyphId}, Class: {Class}")]
internal readonly struct ClassRangeRecord
{
    public ClassRangeRecord(ushort startGlyphId, ushort endGlyphId, ushort glyphClass)
    {
        StartGlyphId = startGlyphId;
        EndGlyphId = endGlyphId;
        Class = glyphClass;
    }

    public ushort StartGlyphId { get; }

    public ushort EndGlyphId { get; }

    public ushort Class { get; }
}