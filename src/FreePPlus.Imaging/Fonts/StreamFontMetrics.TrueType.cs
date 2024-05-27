// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

//using System;
using System.Numerics;
using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;
using FreePPlus.Imaging.Fonts.Tables.General;
using FreePPlus.Imaging.Fonts.Tables.General.Colr;
using FreePPlus.Imaging.Fonts.Tables.General.Kern;
using FreePPlus.Imaging.Fonts.Tables.General.Name;
using FreePPlus.Imaging.Fonts.Tables.General.Post;
using FreePPlus.Imaging.Fonts.Tables.TrueType;
using FreePPlus.Imaging.Fonts.Tables.TrueType.Glyphs;
using FreePPlus.Imaging.Fonts.Tables.TrueType.Hinting;
using FreePPlus.Imaging.Fonts.Unicode;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <content>
///     Contains TrueType specific methods.
/// </content>
internal partial class StreamFontMetrics
{
    //Was previously: [ThreadStatic] private TrueTypeInterpreter? interpreter;
    private TrueTypeInterpreter? interpreter;

    internal void ApplyTrueTypeHinting(HintingMode hintingMode, GlyphMetrics metrics, ref GlyphVector glyphVector,
        Vector2 scaleXY, float scaledPPEM)
    {
        if (hintingMode == HintingMode.None || outlineType != OutlineType.TrueType) return;

        var tables = trueTypeFontTables!;
        if (interpreter == null)
        {
            var maxp = tables.Maxp;
            interpreter = new TrueTypeInterpreter(
                maxp.MaxStackElements,
                maxp.MaxStorage,
                maxp.MaxFunctionDefs,
                maxp.MaxInstructionDefs,
                maxp.MaxTwilightPoints);

            var fpgm = tables.Fpgm;
            if (fpgm is not null) interpreter.InitializeFunctionDefs(fpgm.Instructions);
        }

        var cvt = tables.Cvt;
        var prep = tables.Prep;
        var scaleFactor = scaledPPEM / UnitsPerEm;
        interpreter.SetControlValueTable(cvt?.ControlValues, scaleFactor, scaledPPEM, prep?.Instructions);

        var bounds = glyphVector.GetBounds();

        var pp1 = new Vector2(bounds.Min.X - metrics.LeftSideBearing * scaleXY.X, 0);
        var pp2 = new Vector2(pp1.X + metrics.AdvanceWidth * scaleXY.X, 0);
        var pp3 = new Vector2(0, bounds.Max.Y + metrics.TopSideBearing * scaleXY.Y);
        var pp4 = new Vector2(0, pp3.Y - metrics.AdvanceHeight * scaleXY.Y);

        GlyphVector.Hint(hintingMode, ref glyphVector, interpreter, pp1, pp2, pp3, pp4);
    }

    private static StreamFontMetrics LoadTrueTypeFont(FontReader reader)
    {
        // Load using recommended order for best performance.
        // https://www.microsoft.com/typography/otspec/recom.htm#TableOrdering
        // 'head', 'hhea', 'maxp', OS/2, 'hmtx', LTSH, VDMX, 'hdmx', 'cmap', 'fpgm', 'prep', 'cvt ', 'loca', 'glyf', 'kern', 'name', 'post', 'gasp', PCLT, DSIG
        var head = reader.GetTable<HeadTable>();
        var hhea = reader.GetTable<HorizontalHeadTable>();
        var maxp = reader.GetTable<MaximumProfileTable>();
        var os2 = reader.GetTable<OS2Table>();
        var htmx = reader.GetTable<HorizontalMetricsTable>();
        var cmap = reader.GetTable<CMapTable>();
        var fpgm = reader.TryGetTable<FpgmTable>();
        var prep = reader.TryGetTable<PrepTable>();
        var cvt = reader.TryGetTable<CvtTable>();
        var loca = reader.GetTable<IndexLocationTable>();
        var glyf = reader.GetTable<GlyphTable>();
        var kern = reader.TryGetTable<KerningTable>();
        var name = reader.GetTable<NameTable>();
        var post = reader.GetTable<PostTable>();

        var vhea = reader.TryGetTable<VerticalHeadTable>();
        VerticalMetricsTable? vmtx = null;
        if (vhea is not null) vmtx = reader.TryGetTable<VerticalMetricsTable>();

        var gdef = reader.TryGetTable<GlyphDefinitionTable>();
        var gSub = reader.TryGetTable<GSubTable>();
        var gPos = reader.TryGetTable<GPosTable>();

        var colr = reader.TryGetTable<ColrTable>();
        var cpal = reader.TryGetTable<CpalTable>();

        TrueTypeFontTables tables = new(cmap, head, hhea, htmx, maxp, name, os2, post, glyf, loca)
        {
            Fpgm = fpgm,
            Prep = prep,
            Cvt = cvt,
            Kern = kern,
            Vhea = vhea,
            Vmtx = vmtx,
            Gdef = gdef,
            GSub = gSub,
            GPos = gPos,
            Colr = colr,
            Cpal = cpal
        };

        return new StreamFontMetrics(tables);
    }

    //Was previously:
    //private GlyphMetrics CreateTrueTypeGlyphMetrics(
    //    CodePoint codePoint,
    //    ushort glyphId,
    //    GlyphType glyphType,
    //    ushort palleteIndex = 0)

    private TrueTypeGlyphMetrics CreateTrueTypeGlyphMetrics(
        CodePoint codePoint,
        ushort glyphId,
        GlyphType glyphType,
        ushort palleteIndex = 0)
    {
        var tables = trueTypeFontTables!;
        var glyf = tables.Glyf;
        var htmx = tables.Htmx;
        var vtmx = tables.Vmtx;

        var vector = glyf.GetGlyph(glyphId);
        var bounds = vector.GetBounds();
        var advanceWidth = htmx.GetAdvancedWidth(glyphId);
        var lsb = htmx.GetLeftSideBearing(glyphId);

        // Provide a default for the advance height. This is overwritten for vertical fonts.
        var advancedHeight = (ushort)(Ascender - Descender);
        var tsb = (short)(Ascender - bounds.Max.Y);
        if (vtmx != null)
        {
            advancedHeight = vtmx.GetAdvancedHeight(glyphId);
            tsb = vtmx.GetTopSideBearing(glyphId);
        }

        GlyphColor? color = null;
        if (glyphType == GlyphType.ColrLayer)
            // 0xFFFF is special index meaning use foreground color and thus leave unset
            if (palleteIndex != 0xFFFF)
            {
                var cpal = tables.Cpal;
                color = cpal?.GetGlyphColor(0, palleteIndex);
            }

        return new TrueTypeGlyphMetrics(
            this,
            codePoint,
            vector,
            advanceWidth,
            advancedHeight,
            lsb,
            tsb,
            UnitsPerEm,
            glyphId,
            glyphType,
            color);
    }
}