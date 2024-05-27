// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Fonts.Unicode;
using FreePPlus.Imaging.Fonts.WellKnownIds;

namespace FreePPlus.Imaging.Fonts.Tables.General.CMap;

//was previously: namespace SixLabors.Fonts.Tables.General.CMap;

internal abstract class CMapSubTable
{
    public CMapSubTable() { }

    public CMapSubTable(PlatformIDs platform, ushort encoding, ushort format)
    {
        Platform = platform;
        Encoding = encoding;
        Format = format;
    }

    public ushort Format { get; }

    public PlatformIDs Platform { get; }

    public ushort Encoding { get; }

    public abstract bool TryGetGlyphId(CodePoint codePoint, out ushort glyphId);
}