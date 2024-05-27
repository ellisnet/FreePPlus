// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.General.Colr;

//was previously: namespace SixLabors.Fonts.Tables.General.Colr;

internal sealed class BaseGlyphRecord
{
    public BaseGlyphRecord(ushort glyphId, ushort firstLayerIndex, ushort layerCount)
    {
        GlyphId = glyphId;
        FirstLayerIndex = firstLayerIndex;
        LayerCount = layerCount;
    }

    public ushort GlyphId { get; }

    public ushort FirstLayerIndex { get; }

    public ushort LayerCount { get; }
}

internal sealed class LayerRecord
{
    public LayerRecord(ushort glyphId, ushort paletteIndex)
    {
        GlyphId = glyphId;
        PaletteIndex = paletteIndex;
    }

    public ushort GlyphId { get; }

    public ushort PaletteIndex { get; }
}