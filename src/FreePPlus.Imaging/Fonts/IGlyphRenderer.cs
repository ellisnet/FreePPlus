// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     A surface that can have a glyph rendered to it as a series of actions.
/// </summary>
public interface IGlyphRenderer
{
    /// <summary>
    ///     Begins the figure.
    /// </summary>
    void BeginFigure();

    /// <summary>
    ///     Sets a new start point to draw lines from.
    /// </summary>
    /// <param name="point">The point.</param>
    void MoveTo(Vector2 point);

    /// <summary>
    ///     Draw a quadratic bezier curve connecting the previous point to <paramref name="point" />.
    /// </summary>
    /// <param name="secondControlPoint">The second control point.</param>
    /// <param name="point">The point.</param>
    void QuadraticBezierTo(Vector2 secondControlPoint, Vector2 point);

    /// <summary>
    ///     Draw a cubic bezier curve connecting the previous point to <paramref name="point" />.
    /// </summary>
    /// <param name="secondControlPoint">The second control point.</param>
    /// <param name="thirdControlPoint">The third control point.</param>
    /// <param name="point">The point.</param>
    void CubicBezierTo(Vector2 secondControlPoint, Vector2 thirdControlPoint, Vector2 point);

    /// <summary>
    ///     Draw a straight line connecting the previous point to <paramref name="point" />.
    /// </summary>
    /// <param name="point">The point.</param>
    void LineTo(Vector2 point);

    /// <summary>
    ///     Ends the figure.
    /// </summary>
    void EndFigure();

    /// <summary>
    ///     Ends the glyph.
    /// </summary>
    void EndGlyph();

    /// <summary>
    ///     Begins the glyph.
    /// </summary>
    /// <param name="bounds">The bounds the glyph will be rendered at and at what size.</param>
    /// <param name="parameters">
    ///     The set of parameters that uniquely represents a version of a glyph in at particular font
    ///     size, font family, font style and DPI.
    /// </param>
    /// <returns>Returns true if the glyph should be rendered otherwise it returns false.</returns>
    bool BeginGlyph(FontRectangle bounds, GlyphRendererParameters parameters);

    /// <summary>
    ///     Called once all glyphs have completed rendering.
    /// </summary>
    void EndText();

    /// <summary>
    ///     Called before any glyphs have been rendered.
    /// </summary>
    /// <param name="bounds">The bounds the text will be rendered at and at whats size.</param>
    void BeginText(FontRectangle bounds);
}