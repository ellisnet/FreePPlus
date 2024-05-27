// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;
using FreePPlus.Imaging.Fonts.Unicode;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Contains supplementary data that allows the shaping of glyphs.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal class GlyphShapingData
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GlyphShapingData" /> class.
    /// </summary>
    public GlyphShapingData(TextRun textRun)
    {
        TextRun = textRun;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GlyphShapingData" /> class.
    /// </summary>
    /// <param name="data">The data to copy properties from.</param>
    /// <param name="clearFeatures">Whether to clear features.</param>
    public GlyphShapingData(GlyphShapingData data, bool clearFeatures = false)
    {
        GlyphId = data.GlyphId;
        CodePoint = data.CodePoint;
        CodePointCount = data.CodePointCount;
        Direction = data.Direction;
        TextRun = data.TextRun;
        LigatureId = data.LigatureId;
        LigatureComponent = data.LigatureComponent;
        MarkAttachment = data.MarkAttachment;
        CursiveAttachment = data.CursiveAttachment;
        IsDecomposed = data.IsDecomposed;

        if (!clearFeatures) Features = new List<TagEntry>(data.Features);

        Bounds = data.Bounds;
    }

    /// <summary>
    ///     Gets or sets the glyph id.
    /// </summary>
    public ushort GlyphId { get; set; }

    /// <summary>
    ///     Gets or sets the leading codepoint.
    /// </summary>
    public CodePoint CodePoint { get; set; }

    /// <summary>
    ///     Gets or sets the codepoint count represented by this glyph.
    /// </summary>
    public int CodePointCount { get; set; } = 1;

    /// <summary>
    ///     Gets or sets the text direction.
    /// </summary>
    public TextDirection Direction { get; set; }

    /// <summary>
    ///     Gets or sets the text run this glyph belongs to.
    /// </summary>
    public TextRun TextRun { get; set; }

    /// <summary>
    ///     Gets or sets the id of any ligature this glyph is a member of.
    /// </summary>
    public int LigatureId { get; set; }

    /// <summary>
    ///     Gets or sets the ligature component index of the glyph.
    /// </summary>
    public int LigatureComponent { get; set; } = -1;

    /// <summary>
    ///     Gets or sets the index of any mark attachment.
    /// </summary>
    public int MarkAttachment { get; set; } = -1;

    /// <summary>
    ///     Gets or sets the index of any cursive attachment.
    /// </summary>
    public int CursiveAttachment { get; set; } = -1;

    /// <summary>
    ///     Gets or sets the collection of features.
    /// </summary>
    public List<TagEntry> Features { get; set; } = new();

    /// <summary>
    ///     Gets or sets the shaping bounds.
    /// </summary>
    public GlyphShapingBounds Bounds { get; set; } = new(0, 0, 0, 0);

    /// <summary>
    ///     Gets or sets a value indicating whether this glyph is the result of a decomposition substitution
    /// </summary>
    public bool IsDecomposed { get; set; }

    private string DebuggerDisplay
        => FormattableString
            .Invariant(
                $" {GlyphId} : {CodePoint.ToDebuggerDisplay()} : {CodePoint.GetScriptClass(CodePoint)} : {Direction} : {TextRun.TextAttributes} : {LigatureId} : {LigatureComponent} : {IsDecomposed}");
}