// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;
using FreePPlus.Imaging.Fonts.Tables.Cff;
using FreePPlus.Imaging.Fonts.Tables.General;
using FreePPlus.Imaging.Fonts.Tables.General.Colr;
using FreePPlus.Imaging.Fonts.Tables.General.Kern;
using FreePPlus.Imaging.Fonts.Tables.General.Name;
using FreePPlus.Imaging.Fonts.Tables.General.Post;
using FreePPlus.Imaging.Fonts.Unicode;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <content>
///     Contains CFF specific methods.
/// </content>
internal partial class StreamFontMetrics
{
    private static StreamFontMetrics LoadCompactFont(FontReader reader)
    {
        // Load using recommended order for best performance.
        // https://www.microsoft.com/typography/otspec/recom.htm#TableOrdering
        // 'head', 'hhea', 'maxp', OS/2, 'name', 'cmap', 'post', 'CFF '
        var head = reader.GetTable<HeadTable>();
        var hhea = reader.GetTable<HorizontalHeadTable>();
        var maxp = reader.GetTable<MaximumProfileTable>();
        var os2 = reader.GetTable<OS2Table>();
        var name = reader.GetTable<NameTable>();
        var cmap = reader.GetTable<CMapTable>();
        var post = reader.GetTable<PostTable>();

        var cff = reader.TryGetTable<Cff1Table>() ?? (ICffTable?)reader.TryGetTable<Cff2Table>();

        // TODO: VORG
        var htmx = reader.GetTable<HorizontalMetricsTable>();
        var vhea = reader.TryGetTable<VerticalHeadTable>();
        VerticalMetricsTable? vmtx = null;
        if (vhea is not null) vmtx = reader.TryGetTable<VerticalMetricsTable>();

        var kern = reader.TryGetTable<KerningTable>();

        var gdef = reader.TryGetTable<GlyphDefinitionTable>();
        var gSub = reader.TryGetTable<GSubTable>();
        var gPos = reader.TryGetTable<GPosTable>();

        var colr = reader.TryGetTable<ColrTable>();
        var cpal = reader.TryGetTable<CpalTable>();

        CompactFontTables tables = new(cmap, head, hhea, htmx, maxp, name, os2, post, cff!)
        {
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

    private GlyphMetrics CreateCffGlyphMetrics(
        CodePoint codePoint,
        ushort glyphId,
        GlyphType glyphType,
        ushort palleteIndex = 0)
    {
        var tables = compactFontTables!;
        var cff = tables.Cff;
        var htmx = tables.Htmx;
        var vtmx = tables.Vmtx;

        var vector = cff.GetGlyph(glyphId);
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

        return new CffGlyphMetrics(
            this,
            codePoint,
            vector,
            bounds,
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