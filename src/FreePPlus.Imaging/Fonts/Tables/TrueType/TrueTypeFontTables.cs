// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;
using FreePPlus.Imaging.Fonts.Tables.General;
using FreePPlus.Imaging.Fonts.Tables.General.Colr;
using FreePPlus.Imaging.Fonts.Tables.General.Kern;
using FreePPlus.Imaging.Fonts.Tables.General.Name;
using FreePPlus.Imaging.Fonts.Tables.General.Post;
using FreePPlus.Imaging.Fonts.Tables.TrueType.Glyphs;
using FreePPlus.Imaging.Fonts.Tables.TrueType.Hinting;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.TrueType;

//was previously: namespace SixLabors.Fonts.Tables.TrueType;

internal sealed class TrueTypeFontTables : IFontTables
{
    public TrueTypeFontTables(
        CMapTable cmap,
        HeadTable head,
        HorizontalHeadTable hhea,
        HorizontalMetricsTable htmx,
        MaximumProfileTable maxp,
        NameTable name,
        OS2Table os2,
        PostTable post,
        GlyphTable glyph,
        IndexLocationTable loca)
    {
        Cmap = cmap;
        Head = head;
        Hhea = hhea;
        Htmx = htmx;
        Maxp = maxp;
        Name = name;
        Os2 = os2;
        Post = post;
        Glyf = glyph;
        Loca = loca;
    }

    // Tables Related to TrueType Outlines
    // +------+-----------------------------------------------+
    // | Tag  | Name                                          |
    // +======+===============================================+
    // | cvt  | Control Value Table (optional table)          |
    // +------+-----------------------------------------------+
    // | fpgm | Font program (optional table)                 |
    // +------+-----------------------------------------------+
    // | glyf | Glyph data                                    |
    // +------+-----------------------------------------------+
    // | loca | Index to location                             |
    // +------+-----------------------------------------------+
    // | prep | CVT Program (optional table)                  |
    // +------+-----------------------------------------------+
    // | gasp | Grid-fitting/Scan-conversion (optional table) |
    // +------+-----------------------------------------------+
    public CvtTable? Cvt { get; set; }

    public FpgmTable? Fpgm { get; set; }

    public GlyphTable Glyf { get; set; }

    public IndexLocationTable Loca { get; set; }

    public PrepTable? Prep { get; set; }

    public CMapTable Cmap { get; set; }

    public HeadTable Head { get; set; }

    public HorizontalHeadTable Hhea { get; set; }

    public HorizontalMetricsTable Htmx { get; set; }

    public MaximumProfileTable Maxp { get; set; }

    public NameTable Name { get; set; }

    public OS2Table Os2 { get; set; }

    public PostTable Post { get; set; }

    public GlyphDefinitionTable? Gdef { get; set; }

    public GSubTable? GSub { get; set; }

    public GPosTable? GPos { get; set; }

    public ColrTable? Colr { get; set; }

    public CpalTable? Cpal { get; set; }

    public KerningTable? Kern { get; set; }

    public VerticalHeadTable? Vhea { get; set; }

    public VerticalMetricsTable? Vmtx { get; set; }
}