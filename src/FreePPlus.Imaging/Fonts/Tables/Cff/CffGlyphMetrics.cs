// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using FreePPlus.Imaging.Fonts.Tables.General;
using FreePPlus.Imaging.Fonts.Unicode;

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

/// <summary>
///     Represents a glyph metric from a particular Compact Font Face.
/// </summary>
internal class CffGlyphMetrics : GlyphMetrics
{
    private static readonly Vector2 MirrorScale = new(1, -1);
    private CffGlyphData glyphData;

    public CffGlyphMetrics(
        StreamFontMetrics font,
        CodePoint codePoint,
        CffGlyphData glyphData,
        Bounds bounds,
        ushort advanceWidth,
        ushort advanceHeight,
        short leftSideBearing,
        short topSideBearing,
        ushort unitsPerEM,
        ushort glyphId,
        GlyphType glyphType = GlyphType.Standard,
        GlyphColor? glyphColor = null)
        : base(font, codePoint, bounds, advanceWidth, advanceHeight, leftSideBearing, topSideBearing, unitsPerEM,
            glyphId, glyphType, glyphColor)
    {
        this.glyphData = glyphData;
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

        return new CffGlyphMetrics(
            fontMetrics,
            codePoint,
            glyphData,
            Bounds,
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

    /// <inheritdoc />
    internal override void RenderTo(IGlyphRenderer renderer, float pointSize, Vector2 location, TextOptions options)
    {
        // https://www.unicode.org/faq/unsup_char.html
        if (ShouldSkipGlyphRendering(CodePoint)) return;

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

                var scale = new Vector2(scaledPPEM) / ScaleFactor * MirrorScale;
                var offset = location + Offset * scale * MirrorScale;
                glyphData.RenderTo(renderer, scale, offset);
            }

            RenderDecorationsTo(renderer, location, scaledPPEM);
        }

        renderer.EndGlyph();
    }
}