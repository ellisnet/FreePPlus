// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Diagnostics;
using FreePPlus.Imaging.Fonts.Unicode;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     The combined set of properties that uniquely identify the glyph that is to be rendered
///     at a particular size and dpi.
/// </summary>
[DebuggerDisplay("GlyphIndex = {GlyphIndex}, PointSize = {PointSize}, DpiX = {DpiX}, DpiY = {DpiY}")]
public readonly struct GlyphRendererParameters : IEquatable<GlyphRendererParameters>
{
    internal GlyphRendererParameters(GlyphMetrics metrics, TextRun textRun, float pointSize, float dpi)
    {
        Font = metrics.FontMetrics.Description.FontNameInvariantCulture?.ToUpper() ?? string.Empty;
        FontStyle = metrics.FontMetrics.Description.Style;
        GlyphIndex = metrics.GlyphId;
        PointSize = pointSize;
        Dpi = dpi;
        GlyphType = metrics.GlyphType;
        GlyphColor = metrics.GlyphColor ?? default;
        TextRun = textRun;
        CodePoint = metrics.CodePoint;
    }

    /// <summary>
    ///     Gets the name of the Font this glyph belongs to.
    /// </summary>
    public string Font { get; }

    /// <summary>
    ///     Gets the color details of this glyph.
    /// </summary>
    public GlyphColor GlyphColor { get; }

    /// <summary>
    ///     Gets the type of this glyph.
    /// </summary>
    public GlyphType GlyphType { get; }

    /// <summary>
    ///     Gets the style of the font this glyph belongs to.
    /// </summary>
    public FontStyle FontStyle { get; }

    /// <summary>
    ///     Gets the index of the glyph within the font tables.
    /// </summary>
    public ushort GlyphIndex { get; }

    /// <summary>
    ///     Gets the codepoint represented by this glyph.
    /// </summary>
    public CodePoint CodePoint { get; }

    /// <summary>
    ///     Gets the rendered point size.
    /// </summary>
    public float PointSize { get; }

    /// <summary>
    ///     Gets the dots-per-inch the glyph is to be rendered at.
    /// </summary>
    public float Dpi { get; }

    /// <summary>
    ///     Gets the text run this glyph belongs to.
    /// </summary>
    public TextRun TextRun { get; }

    /// <summary>
    ///     Compares two <see cref="GlyphRendererParameters" /> objects for equality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="GlyphRendererParameters" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="GlyphRendererParameters" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the current left is equal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    public static bool operator ==(GlyphRendererParameters left, GlyphRendererParameters right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="GlyphRendererParameters" /> objects for inequality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="GlyphRendererParameters" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="GlyphRendererParameters" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the current left is unequal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    public static bool operator !=(GlyphRendererParameters left, GlyphRendererParameters right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public bool Equals(GlyphRendererParameters other)
    {
        return other.PointSize == PointSize
               && other.FontStyle == FontStyle
               && other.Dpi == Dpi
               && other.GlyphIndex == GlyphIndex
               && other.GlyphType == GlyphType
               && other.TextRun == TextRun
               && other.GlyphColor.Equals(GlyphColor)
               && ((other.Font is null && Font is null)
                   || other.Font?.Equals(Font, StringComparison.OrdinalIgnoreCase) == true);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is GlyphRendererParameters p && Equals(p);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Font,
            PointSize,
            GlyphIndex,
            GlyphType,
            GlyphColor,
            FontStyle,
            Dpi,
            TextRun);
    }
}