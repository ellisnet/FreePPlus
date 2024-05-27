// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using FreePPlus.Imaging.Fonts.Unicode;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     A glyphs layout and location
/// </summary>
internal readonly struct GlyphLayout
{
    internal GlyphLayout(
        Glyph glyph,
        Vector2 location,
        float ascender,
        float descender,
        float linegap,
        float lineHeight,
        float width,
        float height,
        bool isStartOfLine)
    {
        Glyph = glyph;
        CodePoint = glyph.GlyphMetrics.CodePoint;
        Location = location;
        Ascender = ascender;
        Descender = descender;
        LineGap = linegap;
        LineHeight = lineHeight;
        Width = width;
        Height = height;
        IsStartOfLine = isStartOfLine;
    }

    /// <summary>
    ///     Gets the glyph.
    /// </summary>
    public Glyph Glyph { get; }

    /// <summary>
    ///     Gets the codepoint represented by this glyph.
    /// </summary>
    public CodePoint CodePoint { get; }

    /// <summary>
    ///     Gets the location.
    /// </summary>
    public Vector2 Location { get; }

    /// <summary>
    ///     Gets the ascender
    /// </summary>
    public float Ascender { get; }

    /// <summary>
    ///     Gets the ascender
    /// </summary>
    public float Descender { get; }

    /// <summary>
    ///     Gets the lie gap
    /// </summary>
    public float LineGap { get; }

    /// <summary>
    ///     Gets the line height of the glyph.
    /// </summary>
    public float LineHeight { get; }

    /// <summary>
    ///     Gets the width.
    /// </summary>
    public float Width { get; }

    /// <summary>
    ///     Gets the height.
    /// </summary>
    public float Height { get; }

    /// <summary>
    ///     Gets a value indicating whether this glyph is the first glyph on a new line.
    /// </summary>
    public bool IsStartOfLine { get; }

    /// <summary>
    ///     Gets a value indicating whether the glyph represents a whitespace character.
    /// </summary>
    /// <returns>The <see cref="bool" />.</returns>
    public bool IsWhiteSpace()
    {
        return CodePoint.IsWhiteSpace(CodePoint);
    }

    internal FontRectangle BoundingBox(float dpi)
    {
        var box = Glyph.BoundingBox(Location * dpi, dpi);

        // TODO: Should this be in GlyphMetrics? We likely need to check more than whitespace.
        if (IsWhiteSpace()) box = new FontRectangle(box.X, box.Y, Width * dpi, box.Height);

        return box;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var s = IsStartOfLine ? "@ " : string.Empty;
        var ws = IsWhiteSpace() ? "!" : string.Empty;
        var l = Location;
        return $"{s}{ws}{CodePoint.ToDebuggerDisplay()} {l.X},{l.Y} {Width}x{Height}";
    }
}