// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;
using FreePPlus.Imaging.Fonts.Tables.General;
using FreePPlus.Imaging.Fonts.Tables.General.Colr;
using FreePPlus.Imaging.Fonts.Tables.General.Kern;
using FreePPlus.Imaging.Fonts.Tables.General.Name;
using FreePPlus.Imaging.Fonts.Tables.General.Post;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal sealed class CompactFontTables : IFontTables
{
    public CompactFontTables(
        CMapTable cmap,
        HeadTable head,
        HorizontalHeadTable hhea,
        HorizontalMetricsTable htmx,
        MaximumProfileTable maxp,
        NameTable name,
        OS2Table os2,
        PostTable post,
        ICffTable cff)
    {
        Cmap = cmap;
        Head = head;
        Hhea = hhea;
        Htmx = htmx;
        Maxp = maxp;
        Name = name;
        Os2 = os2;
        Post = post;
        Cff = cff;
    }

    // Tables Related to CFF Outlines
    // +------+----------------------------------+
    // | Tag  | Name                             |
    // +======+==================================+
    // | CFF  | Compact Font Format 1.0          |
    // +------+----------------------------------+
    // | CFF2 | Compact Font Format 2.0          |
    // +------+----------------------------------+
    // | VORG | Vertical Origin (optional table) |
    // +------+----------------------------------+
    public ICffTable Cff { get; set; }

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