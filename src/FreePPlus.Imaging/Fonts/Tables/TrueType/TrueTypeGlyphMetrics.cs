// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Numerics;
using FreePPlus.Imaging.Fonts.Tables.General;
using FreePPlus.Imaging.Fonts.Tables.TrueType.Glyphs;
using FreePPlus.Imaging.Fonts.Unicode;

namespace FreePPlus.Imaging.Fonts.Tables.TrueType;

//was previously: namespace SixLabors.Fonts.Tables.TrueType;

/// <summary>
///     Represents a glyph metric from a particular TrueType font face.
/// </summary>
public class TrueTypeGlyphMetrics : GlyphMetrics
{
    private static readonly Vector2 MirrorScale = new(1, -1);
    private readonly Dictionary<float, GlyphVector> scaledVector = new();
    private readonly GlyphVector vector;

    internal TrueTypeGlyphMetrics(
        StreamFontMetrics font,
        CodePoint codePoint,
        GlyphVector vector,
        ushort advanceWidth,
        ushort advanceHeight,
        short leftSideBearing,
        short topSideBearing,
        ushort unitsPerEM,
        ushort glyphId,
        GlyphType glyphType = GlyphType.Standard,
        GlyphColor? glyphColor = null)
        : base(font, codePoint, vector.GetBounds(), advanceWidth, advanceHeight, leftSideBearing, topSideBearing,
            unitsPerEM, glyphId, glyphType, glyphColor)
    {
        this.vector = vector;
    }

    /// <inheritdoc />
    internal override GlyphMetrics CloneForRendering(TextRun textRun, CodePoint codePoint)
    {
        var fontMetrics = FontMetrics;
        var offset = Offset;
        var scaleFactor = ScaleFactor;
        if (textRun.TextAttributes.HasFlag(TextAttributes.Subscript))
        {
            float units = UnitsPerEm;
            scaleFactor /= new Vector2(fontMetrics.SubscriptXSize / units, fontMetrics.SubscriptYSize / units);
            offset = new Vector2(FontMetrics.SubscriptXOffset, FontMetrics.SubscriptYOffset);
        }
        else if (textRun.TextAttributes.HasFlag(TextAttributes.Superscript))
        {
            float units = UnitsPerEm;
            scaleFactor /= new Vector2(fontMetrics.SuperscriptXSize / units, fontMetrics.SuperscriptYSize / units);
            offset = new Vector2(fontMetrics.SuperscriptXOffset, -fontMetrics.SuperscriptYOffset);
        }

        return new TrueTypeGlyphMetrics(
            fontMetrics,
            codePoint,
            GlyphVector.DeepClone(vector),
            AdvanceWidth,
            AdvanceHeight,
            LeftSideBearing,
            TopSideBearing,
            UnitsPerEm,
            GlyphId,
            GlyphType,
            GlyphColor)
        {
            Offset = offset,
            ScaleFactor = scaleFactor,
            TextRun = textRun
        };
    }

    /// <summary>
    ///     Gets the outline for the current glyph.
    /// </summary>
    /// <returns>The <see cref="GlyphOutline" />.</returns>
    public GlyphOutline GetOutline()
    {
        return vector.GetOutline();
    }

    /// <inheritdoc />
    internal override void RenderTo(IGlyphRenderer renderer, float pointSize, Vector2 location, TextOptions options)
    {
        // https://www.unicode.org/faq/unsup_char.html
        if (ShouldSkipGlyphRendering(CodePoint)) return;

        // TODO: Move to base class
        var dpi = options.Dpi;
        location *= dpi;
        var scaledPPEM = dpi * pointSize;
        var forcePPEMToInt = (FontMetrics.HeadFlags & HeadTable.HeadFlags.ForcePPEMToInt) != 0;

        if (forcePPEMToInt) scaledPPEM = MathF.Round(scaledPPEM);

        var box = GetBoundingBox(location, scaledPPEM);

        // TextRun is never null here as rendering is only accessable via a Glyph which
        // uses the cloned metrics instance.
        var parameters = new GlyphRendererParameters(this, TextRun!, pointSize, dpi);

        if (renderer.BeginGlyph(box, parameters))
        {
            if (!ShouldRenderWhiteSpaceOnly(CodePoint))
            {
                if (GlyphColor.HasValue && renderer is IColorGlyphRenderer colorSurface)
                    colorSurface.SetColor(GlyphColor.Value);

                if (!this.scaledVector.TryGetValue(scaledPPEM, out var scaledVector))
                {
                    // Scale and translate the glyph
                    var scale = new Vector2(scaledPPEM) / ScaleFactor;
                    var transform = Matrix3x2.CreateScale(scale);
                    transform.Translation = Offset * scale * MirrorScale;
                    scaledVector = GlyphVector.Transform(vector, transform);
                    FontMetrics.ApplyTrueTypeHinting(options.HintingMode, this, ref scaledVector, scale, scaledPPEM);
                    this.scaledVector[scaledPPEM] = scaledVector;
                }

                var outline = scaledVector.GetOutline();
                var controlPoints = outline.ControlPoints.Span;
                var endPoints = outline.EndPoints.Span;
                var onCurves = outline.OnCurves.Span;

                var endOfContour = -1;
                for (var i = 0; i < outline.EndPoints.Length; i++)
                {
                    renderer.BeginFigure();
                    var startOfContour = endOfContour + 1;
                    endOfContour = endPoints[i];

                    Vector2 prev;
                    var curr = MirrorScale * controlPoints[endOfContour] + location;
                    var next = MirrorScale * controlPoints[startOfContour] + location;

                    if (onCurves[endOfContour])
                    {
                        renderer.MoveTo(curr);
                    }
                    else
                    {
                        if (onCurves[startOfContour])
                        {
                            renderer.MoveTo(next);
                        }
                        else
                        {
                            // If both first and last points are off-curve, start at their middle.
                            var startPoint = (curr + next) * .5F;
                            renderer.MoveTo(startPoint);
                        }
                    }

                    var length = endOfContour - startOfContour + 1;
                    for (var p = 0; p < length; p++)
                    {
                        prev = curr;
                        curr = next;
                        var currentIndex = startOfContour + p;
                        var nextIndex = startOfContour + (p + 1) % length;
                        var prevIndex = startOfContour + (length + p - 1) % length;
                        next = MirrorScale * controlPoints[nextIndex] + location;

                        if (onCurves[currentIndex])
                        {
                            // This is a straight line.
                            renderer.LineTo(curr);
                        }
                        else
                        {
                            var prev2 = prev;
                            var next2 = next;

                            if (!onCurves[prevIndex])
                            {
                                prev2 = (curr + prev) * .5F;
                                renderer.LineTo(prev2);
                            }

                            if (!onCurves[nextIndex]) next2 = (curr + next) * .5F;

                            renderer.LineTo(prev2);
                            renderer.QuadraticBezierTo(curr, next2);
                        }
                    }

                    renderer.EndFigure();
                }
            }

            RenderDecorationsTo(renderer, location, scaledPPEM);
        }

        renderer.EndGlyph();
    }
}