// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

/// <summary>
///     Used to apply a transform against any glyphs rendered by the engine.
/// </summary>
internal struct TransformingGlyphRenderer : IGlyphRenderer
{
    private readonly Vector2 scale;
    private readonly Vector2 offset;
    private readonly IGlyphRenderer renderer;

    public TransformingGlyphRenderer(Vector2 scale, Vector2 offset, IGlyphRenderer renderer)
    {
        this.scale = scale;
        this.offset = offset;
        this.renderer = renderer;
        IsOpen = false;
    }

    public bool IsOpen { get; set; }

    public void BeginFigure()
    {
        IsOpen = false;
        renderer.BeginFigure();
    }

    public bool BeginGlyph(FontRectangle bounds, GlyphRendererParameters parameters)
    {
        IsOpen = false;
        return renderer.BeginGlyph(bounds, parameters);
    }

    public void BeginText(FontRectangle bounds)
    {
        IsOpen = false;
        renderer.BeginText(bounds);
    }

    public void EndFigure()
    {
        IsOpen = false;
        renderer.EndFigure();
    }

    public void EndGlyph()
    {
        IsOpen = false;
        renderer.EndGlyph();
    }

    public void EndText()
    {
        IsOpen = false;
        renderer.EndText();
    }

    public void LineTo(Vector2 point)
    {
        IsOpen = true;
        renderer.LineTo(Transform(point));
    }

    public void MoveTo(Vector2 point)
    {
        if (IsOpen) EndFigure();

        renderer.MoveTo(Transform(point));
    }

    public void CubicBezierTo(Vector2 secondControlPoint, Vector2 thirdControlPoint, Vector2 point)
    {
        IsOpen = true;
        renderer.CubicBezierTo(Transform(secondControlPoint), Transform(thirdControlPoint), Transform(point));
    }

    public void QuadraticBezierTo(Vector2 secondControlPoint, Vector2 point)
    {
        IsOpen = true;
        renderer.QuadraticBezierTo(Transform(secondControlPoint), Transform(point));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Vector2 Transform(Vector2 point)
    {
        return point * scale + offset;
    }
}