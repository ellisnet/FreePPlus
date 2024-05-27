// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;

#pragma warning disable IDE0251
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal struct CffGlyphData
{
    private readonly byte[][] globalSubrBuffers;
    private readonly byte[][] localSubrBuffers;
    private readonly byte[] charStrings;
    private readonly int nominalWidthX;

    public CffGlyphData(
        ushort glyphIndex,
        byte[][] globalSubrBuffers,
        byte[][] localSubrBuffers,
        int nominalWidthX,
        byte[] charStrings)
    {
        GlyphIndex = glyphIndex;
        this.globalSubrBuffers = globalSubrBuffers;
        this.localSubrBuffers = localSubrBuffers;
        this.nominalWidthX = nominalWidthX;
        this.charStrings = charStrings;

        GlyphName = null;
    }

    public readonly ushort GlyphIndex { get; }

    public string? GlyphName { get; set; }

    public Bounds GetBounds()
    {
        using var engine = new CffEvaluationEngine(
            charStrings,
            globalSubrBuffers,
            localSubrBuffers,
            nominalWidthX);

        return engine.GetBounds();
    }

    public void RenderTo(IGlyphRenderer renderer, Vector2 scale, Vector2 offset)
    {
        using var engine = new CffEvaluationEngine(
            charStrings,
            globalSubrBuffers,
            localSubrBuffers,
            nominalWidthX);

        engine.RenderTo(renderer, scale, offset);
    }
}