// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Fonts.Unicode;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Represents a glyph metric from a particular font face.
/// </summary>
public abstract class GlyphMetrics
{
    private static readonly Vector2 MirrorScale = new(1, -1);

    internal GlyphMetrics(
        StreamFontMetrics font,
        CodePoint codePoint,
        Bounds bounds,
        ushort advanceWidth,
        ushort advanceHeight,
        short leftSideBearing,
        short topSideBearing,
        ushort unitsPerEM,
        ushort glyphId,
        GlyphType glyphType = GlyphType.Standard,
        GlyphColor? glyphColor = null)
    {
        FontMetrics = font;
        CodePoint = codePoint;
        UnitsPerEm = unitsPerEM;
        AdvanceWidth = advanceWidth;
        AdvanceHeight = advanceHeight;
        GlyphId = glyphId;
        Bounds = bounds;
        Width = bounds.Max.X - bounds.Min.X;
        Height = bounds.Max.Y - bounds.Min.Y;
        GlyphType = glyphType;
        LeftSideBearing = leftSideBearing;
        RightSideBearing = (short)(AdvanceWidth - LeftSideBearing - Width);
        TopSideBearing = topSideBearing;
        BottomSideBearing = (short)(AdvanceHeight - TopSideBearing - Height);
        ScaleFactor = new Vector2(unitsPerEM * 72F);
        GlyphColor = glyphColor;
    }

    /// <summary>
    ///     Gets the font metrics.
    /// </summary>
    internal StreamFontMetrics FontMetrics { get; }

    /// <summary>
    ///     Gets the Unicode codepoint of the glyph.
    /// </summary>
    public CodePoint CodePoint { get; }

    /// <summary>
    ///     Gets the advance width for horizontal layout, expressed in font units.
    /// </summary>
    public ushort AdvanceWidth { get; private set; }

    /// <summary>
    ///     Gets the advance height for vertical layout, expressed in font units.
    /// </summary>
    public ushort AdvanceHeight { get; private set; }

    /// <summary>
    ///     Gets the left side bearing for horizontal layout, expressed in font units.
    /// </summary>
    public short LeftSideBearing { get; }

    /// <summary>
    ///     Gets the right side bearing for horizontal layout, expressed in font units.
    /// </summary>
    public short RightSideBearing { get; }

    /// <summary>
    ///     Gets the top side bearing for vertical layout, expressed in font units.
    /// </summary>
    public short TopSideBearing { get; }

    /// <summary>
    ///     Gets the bottom side bearing for vertical layout, expressed in font units.
    /// </summary>
    public short BottomSideBearing { get; }

    /// <summary>
    ///     Gets the bounds, expressed in font units.
    /// </summary>
    internal Bounds Bounds { get; }

    /// <summary>
    ///     Gets the width, expressed in font units.
    /// </summary>
    public float Width { get; }

    /// <summary>
    ///     Gets the height, expressed in font units.
    /// </summary>
    public float Height { get; }

    /// <summary>
    ///     Gets the glyph type.
    /// </summary>
    public GlyphType GlyphType { get; }

    /// <summary>
    ///     Gets the color of this glyph when the <see cref="GlyphType" /> is <see cref="GlyphType.ColrLayer" />
    /// </summary>
    public GlyphColor? GlyphColor { get; }

    /// <inheritdoc cref="FontMetrics.UnitsPerEm" />
    public ushort UnitsPerEm { get; }

    /// <inheritdoc cref="FontMetrics.ScaleFactor" />
    public Vector2 ScaleFactor { get; protected set; }

    internal Vector2 Offset { get; set; }

    internal TextRun? TextRun { get; set; }

    /// <summary>
    ///     Gets the glyph Id.
    /// </summary>
    internal ushort GlyphId { get; }

    /// <summary>
    ///     Performs a semi-deep clone (FontMetrics are not cloned) for rendering
    ///     This allows caching the original in the font metrics.
    /// </summary>
    /// <param name="textRun">The text run this glyph is a member of.</param>
    /// <param name="codePoint">The codepoint for this glyph.</param>
    /// <returns>The new <see cref="GlyphMetrics" />.</returns>
    internal abstract GlyphMetrics CloneForRendering(TextRun textRun, CodePoint codePoint);

    /// <summary>
    ///     Apply an offset to the glyph.
    /// </summary>
    /// <param name="x">The x-offset.</param>
    /// <param name="y">The y-offset.</param>
    internal void ApplyOffset(short x, short y)
    {
        Offset = Vector2.Transform(Offset, Matrix3x2.CreateTranslation(x, y));
    }

    /// <summary>
    ///     Applies an advance to the glyph.
    /// </summary>
    /// <param name="x">The x-advance.</param>
    /// <param name="y">The y-advance.</param>
    internal void ApplyAdvance(short x, short y)
    {
        AdvanceWidth = (ushort)(AdvanceWidth + x);

        // AdvanceHeight values grow downward but font-space grows upward, hence negation
        AdvanceHeight = (ushort)(AdvanceHeight - y);
    }

    /// <summary>
    ///     Sets a new advance width.
    /// </summary>
    /// <param name="x">The x-advance.</param>
    internal void SetAdvanceWidth(ushort x)
    {
        AdvanceWidth = x;
    }

    /// <summary>
    ///     Sets a new advance height.
    /// </summary>
    /// <param name="y">The y-advance.</param>
    internal void SetAdvanceHeight(ushort y)
    {
        AdvanceHeight = y;
    }

    internal FontRectangle GetBoundingBox(Vector2 origin, float scaledPointSize)
    {
        var scale = new Vector2(scaledPointSize) / ScaleFactor;
        var bounds = Bounds;
        var size = bounds.Size() * scale;
        var loc = (new Vector2(bounds.Min.X, bounds.Max.Y) + Offset) * scale * MirrorScale;
        loc = origin + loc;

        return new FontRectangle(loc.X, loc.Y, size.X, size.Y);
    }

    /// <summary>
    ///     Renders the glyph to the render surface in font units relative to a bottom left origin at (0,0)
    /// </summary>
    /// <param name="renderer">The surface renderer.</param>
    /// <param name="pointSize">Size of the point.</param>
    /// <param name="location">The location.</param>
    /// <param name="options">The options used to influence the rendering of this glyph.</param>
    internal abstract void RenderTo(IGlyphRenderer renderer, float pointSize, Vector2 location, TextOptions options);

    internal void RenderDecorationsTo(IGlyphRenderer renderer, Vector2 location, float scaledPPEM)
    {
        (Vector2 Start, Vector2 End, float Thickness) GetEnds(float thickness, float position)
        {
            var scale = new Vector2(scaledPPEM) / ScaleFactor * MirrorScale;
            var offset = location + Offset * scale * MirrorScale;

            // Calculate the correct advance for the line.
            float width = AdvanceWidth;
            if (width == 0)
                // For zero advance glyphs we must calculate our advance width from bearing + width;
                width = LeftSideBearing + Width;

            var tl = new Vector2(0, position) * scale + offset;
            var tr = new Vector2(width, position) * scale + offset;
            var bl = new Vector2(0, position + thickness) * scale + offset;

            return (tl, tr, tl.Y - bl.Y);
        }

        void DrawLine(float thickness, float position)
        {
            renderer.BeginFigure();

            (var start, var end, var finalThickness) = GetEnds(thickness, position);
            var halfHeight = new Vector2(0, -finalThickness * .5F);

            var tl = start - halfHeight;
            var tr = end - halfHeight;
            var bl = start + halfHeight;
            var br = end + halfHeight;

            // Clamp the horizontal components to a whole pixel.
            tl.Y = MathF.Ceiling(tl.Y);
            tr.Y = MathF.Ceiling(tr.Y);
            br.Y = MathF.Floor(br.Y);
            bl.Y = MathF.Floor(bl.Y);

            // Do the same for vertical components.
            tl.X = MathF.Floor(tl.X);
            tr.X = MathF.Floor(tr.X);
            br.X = MathF.Floor(br.X);
            bl.X = MathF.Floor(bl.X);

            renderer.MoveTo(tl);
            renderer.LineTo(bl);
            renderer.LineTo(br);
            renderer.LineTo(tr);

            renderer.EndFigure();
        }

        void SetDecoration(TextDecorations decorationType, float thickness, float position)
        {
            (var start, var end, var calcThickness) = GetEnds(thickness, position);
            ((IGlyphDecorationRenderer)renderer).SetDecoration(decorationType, start, end, calcThickness);
        }

        // There's no built in metrics for these values so we will need to infer them from the other metrics.
        // Offset to avoid clipping.
        float overlineThickness = FontMetrics.UnderlineThickness;

        // TODO: Check this. Segoe UI glyphs live outside the metrics so the overline covers the glyph.
        var overlinePosition = FontMetrics.Ascender - overlineThickness * .5F;
        if (renderer is IGlyphDecorationRenderer decorationRenderer)
        {
            // Allow the rendered to override the decorations to attach
            var decorations = decorationRenderer.EnabledDecorations();
            if ((decorations & TextDecorations.Underline) == TextDecorations.Underline)
                SetDecoration(TextDecorations.Underline, FontMetrics.UnderlineThickness, FontMetrics.UnderlinePosition);

            if ((decorations & TextDecorations.Strikeout) == TextDecorations.Strikeout)
                SetDecoration(TextDecorations.Strikeout, FontMetrics.StrikeoutSize, FontMetrics.StrikeoutPosition);

            if ((decorations & TextDecorations.Overline) == TextDecorations.Overline)
                SetDecoration(TextDecorations.Overline, overlineThickness, overlinePosition);
        }
        else
        {
            // TextRun is never null here as rendering is only accessable via a Glyph which
            // uses the cloned metrics instance.
            if ((TextRun!.TextDecorations & TextDecorations.Underline) == TextDecorations.Underline)
                DrawLine(FontMetrics.UnderlineThickness, FontMetrics.UnderlinePosition);

            if ((TextRun!.TextDecorations & TextDecorations.Strikeout) == TextDecorations.Strikeout)
                DrawLine(FontMetrics.StrikeoutSize, FontMetrics.StrikeoutPosition);

            if ((TextRun!.TextDecorations & TextDecorations.Overline) == TextDecorations.Overline)
                DrawLine(overlineThickness, overlinePosition);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ShouldSkipGlyphRendering(CodePoint codePoint)
    {
        return UnicodeUtility.IsDefaultIgnorableCodePoint((uint)codePoint.Value) &&
               !ShouldRenderWhiteSpaceOnly(codePoint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ShouldRenderWhiteSpaceOnly(CodePoint codePoint)
    {
        if (CodePoint.IsWhiteSpace(codePoint)) return true;

        // Note: While U+115F, U+1160, U+3164 and U+FFA0 are Default_Ignorable,
        // we do NOT want to hide them, as the way Uniscribe has implemented them
        // is with regular spacing glyphs, and that's the way fonts are made to work.
        // As such, we make exceptions for those four.
        // Also ignoring U+1BCA0..1BCA3. https://github.com/harfbuzz/harfbuzz/issues/503
        var value = (uint)codePoint.Value;
        if (value is 0x115F or 0x1160 or 0x3164 or 0xFFA0) return true;

        if (UnicodeUtility.IsInRangeInclusive(value, 0x1BCA0, 0x1BCA3)) return true;

        return false;
    }
}