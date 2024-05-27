// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FreePPlus.Imaging.Fonts.Tables;
using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;
using FreePPlus.Imaging.Fonts.Tables.Cff;
using FreePPlus.Imaging.Fonts.Tables.General;
using FreePPlus.Imaging.Fonts.Tables.TrueType;
using FreePPlus.Imaging.Fonts.Unicode;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     <para>
///         Represents a font face with metrics, which is a set of glyphs with a specific style (regular, italic, bold
///         etc).
///     </para>
///     <para>The font source is a stream.</para>
/// </summary>
internal partial class StreamFontMetrics : FontMetrics
{
    private readonly GlyphMetrics[][]? colorGlyphCache;
    private readonly CompactFontTables? compactFontTables;
    private readonly FontDescription description;

    // https://docs.microsoft.com/en-us/typography/opentype/spec/otff#font-tables
    private readonly GlyphMetrics[][] glyphCache;
    private readonly OutlineType outlineType;
    private readonly TrueTypeFontTables? trueTypeFontTables;
    private short advanceHeightMax;
    private short advanceWidthMax;
    private short ascender;
    private short descender;
    private float italicAngle;
    private short lineGap;
    private short lineHeight;
    private float scaleFactor;
    private short strikeoutPosition;
    private short strikeoutSize;
    private short subscriptXOffset;
    private short subscriptXSize;
    private short subscriptYOffset;
    private short subscriptYSize;
    private short superscriptXOffset;
    private short superscriptXSize;
    private short superscriptYOffset;
    private short superscriptYSize;
    private short underlinePosition;
    private short underlineThickness;
    private ushort unitsPerEm;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StreamFontMetrics" /> class.
    /// </summary>
    /// <param name="tables">The True Type font tables.</param>
    internal StreamFontMetrics(TrueTypeFontTables tables)
    {
        trueTypeFontTables = tables;
        outlineType = OutlineType.TrueType;
        description = new FontDescription(tables.Name, tables.Os2, tables.Head);
        glyphCache = new GlyphMetrics[tables.Glyf.GlyphCount][];
        if (tables.Colr is not null) colorGlyphCache = new GlyphMetrics[tables.Glyf.GlyphCount][];

        Initialize(tables);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="StreamFontMetrics" /> class.
    /// </summary>
    /// <param name="tables">The Compact Font tables.</param>
    internal StreamFontMetrics(CompactFontTables tables)
    {
        compactFontTables = tables;
        outlineType = OutlineType.CFF;
        description = new FontDescription(tables.Name, tables.Os2, tables.Head);
        glyphCache = new GlyphMetrics[tables.Cff.GlyphCount][];
        if (tables.Colr is not null) colorGlyphCache = new GlyphMetrics[tables.Cff.GlyphCount][];

        Initialize(tables);
    }

    public HeadTable.HeadFlags HeadFlags { get; private set; }

    /// <inheritdoc />
    public override FontDescription Description => description;

    /// <inheritdoc />
    public override ushort UnitsPerEm => unitsPerEm;

    /// <inheritdoc />
    public override float ScaleFactor => scaleFactor;

    /// <inheritdoc />
    public override short Ascender => ascender;

    /// <inheritdoc />
    public override short Descender => descender;

    /// <inheritdoc />
    public override short LineGap => lineGap;

    /// <inheritdoc />
    public override short LineHeight => lineHeight;

    /// <inheritdoc />
    public override short AdvanceWidthMax => advanceWidthMax;

    /// <inheritdoc />
    public override short AdvanceHeightMax => advanceHeightMax;

    /// <inheritdoc />
    public override short SubscriptXSize => subscriptXSize;

    /// <inheritdoc />
    public override short SubscriptYSize => subscriptYSize;

    /// <inheritdoc />
    public override short SubscriptXOffset => subscriptXOffset;

    /// <inheritdoc />
    public override short SubscriptYOffset => subscriptYOffset;

    /// <inheritdoc />
    public override short SuperscriptXSize => superscriptXSize;

    /// <inheritdoc />
    public override short SuperscriptYSize => superscriptYSize;

    /// <inheritdoc />
    public override short SuperscriptXOffset => superscriptXOffset;

    /// <inheritdoc />
    public override short SuperscriptYOffset => superscriptYOffset;

    /// <inheritdoc />
    public override short StrikeoutSize => strikeoutSize;

    /// <inheritdoc />
    public override short StrikeoutPosition => strikeoutPosition;

    /// <inheritdoc />
    public override short UnderlinePosition => underlinePosition;

    /// <inheritdoc />
    public override short UnderlineThickness => underlineThickness;

    /// <inheritdoc />
    public override float ItalicAngle => italicAngle;

    /// <inheritdoc />
    internal override bool TryGetGlyphId(CodePoint codePoint, out ushort glyphId)
    {
        return TryGetGlyphId(codePoint, null, out glyphId, out var _);
    }

    /// <inheritdoc />
    internal override bool TryGetGlyphId(CodePoint codePoint, CodePoint? nextCodePoint, out ushort glyphId,
        out bool skipNextCodePoint)
    {
        var cmap = outlineType == OutlineType.TrueType
            ? trueTypeFontTables!.Cmap
            : compactFontTables!.Cmap;

        return cmap.TryGetGlyphId(codePoint, nextCodePoint, out glyphId, out skipNextCodePoint);
    }

    /// <inheritdoc />
    internal override bool TryGetGlyphClass(ushort glyphId, [NotNullWhen(true)] out GlyphClassDef? glyphClass)
    {
        var gdef = outlineType == OutlineType.TrueType
            ? trueTypeFontTables!.Gdef
            : compactFontTables!.Gdef;

        glyphClass = null;
        return gdef is not null && gdef.TryGetGlyphClass(glyphId, out glyphClass);
    }

    /// <inheritdoc />
    internal override bool TryGetMarkAttachmentClass(ushort glyphId,
        [NotNullWhen(true)] out GlyphClassDef? markAttachmentClass)
    {
        var gdef = outlineType == OutlineType.TrueType
            ? trueTypeFontTables!.Gdef
            : compactFontTables!.Gdef;

        markAttachmentClass = null;
        return gdef is not null && gdef.TryGetMarkAttachmentClass(glyphId, out markAttachmentClass);
    }

    /// <inheritdoc />
    public override IEnumerable<GlyphMetrics> GetGlyphMetrics(CodePoint codePoint, ColorFontSupport support)
    {
        TryGetGlyphId(codePoint, out var glyphId);
        return GetGlyphMetrics(codePoint, glyphId, support);
    }

    /// <inheritdoc />
    internal override IEnumerable<GlyphMetrics> GetGlyphMetrics(CodePoint codePoint, ushort glyphId,
        ColorFontSupport support)
    {
        var glyphType = GlyphType.Standard;
        if (glyphId == 0)
            // A glyph was not found in this face for the previously matched
            // codepoint. Set to fallback.
            glyphType = GlyphType.Fallback;

        if (support == ColorFontSupport.MicrosoftColrFormat
            && TryGetColoredMetrics(codePoint, glyphId, out var metrics))
            return metrics;

        // We overwrite the cache entry for this type should the attributes change.
        var cached = glyphCache[glyphId];
        if (cached is null)
            glyphCache[glyphId] = new[]
            {
                CreateGlyphMetrics(
                    codePoint,
                    glyphId,
                    glyphType)
            };

        return glyphCache[glyphId];
    }

    /// <inheritdoc />
    internal override void ApplySubstitution(GlyphSubstitutionCollection collection)
    {
        var gsub = outlineType == OutlineType.TrueType
            ? trueTypeFontTables!.GSub
            : compactFontTables!.GSub;

        gsub?.ApplySubstitution(this, collection);
    }

    /// <inheritdoc />
    internal override void UpdatePositions(GlyphPositioningCollection collection)
    {
        var isTTF = outlineType == OutlineType.TrueType;
        var gpos = isTTF
            ? trueTypeFontTables!.GPos
            : compactFontTables!.GPos;

        var kerned = false;
        var kerningMode = collection.TextOptions.KerningMode;

        gpos?.TryUpdatePositions(this, collection, out kerned);

        if (!kerned && kerningMode != KerningMode.None)
        {
            var kern = isTTF
                ? trueTypeFontTables!.Kern
                : compactFontTables!.Kern;

            if (kern?.Count > 0)
            {
                // Set max constraints to prevent OutOfMemoryException or infinite loops from attacks.
                var maxCount = AdvancedTypographicUtils.GetMaxAllowableShapingCollectionCount(collection.Count);
                for (var index = 1; index < collection.Count; index++)
                {
                    if (index >= maxCount) break;

                    kern.UpdatePositions(this, collection, index - 1, index);
                }
            }
        }
    }

    /// <summary>
    ///     Reads a <see cref="StreamFontMetrics" /> from the specified stream.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>a <see cref="StreamFontMetrics" />.</returns>
    public static StreamFontMetrics LoadFont(string path)
    {
        using var fs = File.OpenRead(path);
        var reader = new FontReader(fs);
        return LoadFont(reader);
    }

    /// <summary>
    ///     Reads a <see cref="StreamFontMetrics" /> from the specified stream.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="offset">Position in the stream to read the font from.</param>
    /// <returns>a <see cref="StreamFontMetrics" />.</returns>
    public static StreamFontMetrics LoadFont(string path, long offset)
    {
        using var fs = File.OpenRead(path);
        fs.Position = offset;
        return LoadFont(fs);
    }

    /// <summary>
    ///     Reads a <see cref="StreamFontMetrics" /> from the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>a <see cref="StreamFontMetrics" />.</returns>
    public static StreamFontMetrics LoadFont(Stream stream)
    {
        var reader = new FontReader(stream);
        return LoadFont(reader);
    }

    internal static StreamFontMetrics LoadFont(FontReader reader)
    {
        if (reader.OutlineType == OutlineType.TrueType)
            return LoadTrueTypeFont(reader);
        return LoadCompactFont(reader);
    }

    private void Initialize<T>(T tables)
        where T : IFontTables
    {
        var head = tables.Head;
        var hhea = tables.Hhea;
        var vhea = tables.Vhea;
        var os2 = tables.Os2;
        var post = tables.Post;

        HeadFlags = head.Flags;

        // https://www.microsoft.com/typography/otspec/recom.htm#tad
        // We use the same approach as FreeType for calculating the the global  ascender, descender,  and
        // height of  OpenType fonts for consistency.
        //
        // 1.If the OS/ 2 table exists and the fsSelection bit 7 is set (USE_TYPO_METRICS), trust the font
        //   and use the Typo* metrics.
        // 2.Otherwise, use the HorizontalHeadTable "hhea" table's metrics.
        // 3.If they are zero and the OS/ 2 table exists,
        //    - Use the OS/ 2 table's sTypo* metrics if they are non-zero.
        //    - Otherwise, use the OS / 2 table's usWin* metrics.
        var useTypoMetrics = os2.FontStyle.HasFlag(OS2Table.FontStyleSelection.USE_TYPO_METRICS);
        if (useTypoMetrics)
        {
            ascender = os2.TypoAscender;
            descender = os2.TypoDescender;
            lineGap = os2.TypoLineGap;
            lineHeight = (short)(ascender - descender + lineGap);
        }
        else
        {
            ascender = hhea.Ascender;
            descender = hhea.Descender;
            lineGap = hhea.LineGap;
            lineHeight = (short)(ascender - descender + lineGap);
        }

        if (ascender == 0 || descender == 0)
        {
            if (os2.TypoAscender != 0 || os2.TypoDescender != 0)
            {
                ascender = os2.TypoAscender;
                descender = os2.TypoDescender;
                lineGap = os2.TypoLineGap;
                lineHeight = (short)(ascender - descender + lineGap);
            }
            else
            {
                ascender = (short)os2.WinAscent;
                descender = (short)-os2.WinDescent;
                lineHeight = (short)(ascender - descender);
            }
        }

        unitsPerEm = head.UnitsPerEm;

        // 72 * UnitsPerEm means 1pt = 1px
        scaleFactor = unitsPerEm * 72F;
        advanceWidthMax = (short)hhea.AdvanceWidthMax;
        advanceHeightMax = vhea == null ? LineHeight : vhea.AdvanceHeightMax;

        subscriptXSize = os2.SubscriptXSize;
        subscriptYSize = os2.SubscriptYSize;
        subscriptXOffset = os2.SubscriptXOffset;
        subscriptYOffset = os2.SubscriptYOffset;
        superscriptXSize = os2.SuperscriptXSize;
        superscriptYSize = os2.SuperscriptYSize;
        superscriptXOffset = os2.SuperscriptXOffset;
        superscriptYOffset = os2.SuperscriptYOffset;
        strikeoutSize = os2.StrikeoutSize;
        strikeoutPosition = os2.StrikeoutPosition;

        underlinePosition = post.UnderlinePosition;
        underlineThickness = post.UnderlineThickness;
        italicAngle = post.ItalicAngle;
    }

    /// <summary>
    ///     Reads a <see cref="StreamFontMetrics" /> from the specified stream.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>a <see cref="StreamFontMetrics" />.</returns>
    public static StreamFontMetrics[] LoadFontCollection(string path)
    {
        using var fs = File.OpenRead(path);
        return LoadFontCollection(fs);
    }

    /// <summary>
    ///     Reads a <see cref="StreamFontMetrics" /> from the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>a <see cref="StreamFontMetrics" />.</returns>
    public static StreamFontMetrics[] LoadFontCollection(Stream stream)
    {
        var startPos = stream.Position;
        var reader = new BigEndianBinaryReader(stream, true);
        var ttcHeader = TtcHeader.Read(reader);
        var fonts = new StreamFontMetrics[(int)ttcHeader.NumFonts];

        for (var i = 0; i < ttcHeader.NumFonts; ++i)
        {
            stream.Position = startPos + ttcHeader.OffsetTable[i];
            fonts[i] = LoadFont(stream);
        }

        return fonts;
    }

    private bool TryGetColoredMetrics(CodePoint codePoint, ushort glyphId,
        [NotNullWhen(true)] out GlyphMetrics[]? metrics)
    {
        var colr = outlineType == OutlineType.TrueType
            ? trueTypeFontTables!.Colr
            : compactFontTables!.Colr;

        if (colr == null || colorGlyphCache == null)
        {
            metrics = null;
            return false;
        }

        // We overwrite the cache entry for this type should the attributes change.
        metrics = colorGlyphCache[glyphId];
        if (metrics is null)
        {
            var indexes = colr.GetLayers(glyphId);
            if (indexes.Length > 0)
            {
                metrics = new GlyphMetrics[indexes.Length];
                for (var i = 0; i < indexes.Length; i++)
                {
                    var layer = indexes[i];

                    metrics[i] = CreateGlyphMetrics(codePoint, layer.GlyphId, GlyphType.ColrLayer, layer.PaletteIndex);
                }
            }

            metrics ??= Array.Empty<GlyphMetrics>();
            colorGlyphCache[glyphId] = metrics;
        }

        return metrics.Length > 0;
    }

    private GlyphMetrics CreateGlyphMetrics(
        CodePoint codePoint,
        ushort glyphId,
        GlyphType glyphType,
        ushort palleteIndex = 0)
    {
        return outlineType switch
        {
            OutlineType.TrueType => CreateTrueTypeGlyphMetrics(codePoint, glyphId, glyphType, palleteIndex),
            OutlineType.CFF => CreateCffGlyphMetrics(codePoint, glyphId, glyphType, palleteIndex),
            _ => throw new NotSupportedException()
        };
    }
}