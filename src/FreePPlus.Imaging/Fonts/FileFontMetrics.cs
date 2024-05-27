// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FreePPlus.Imaging.Fonts.Tables;
using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;
using FreePPlus.Imaging.Fonts.Unicode;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     <para>
///         Represents a font face with metrics, which is a set of glyphs with a specific style (regular, italic, bold
///         etc).
///     </para>
///     <para>The font source is a filesystem path.</para>
/// </summary>
internal sealed class FileFontMetrics : FontMetrics
{
    private readonly Lazy<StreamFontMetrics> metrics;

    public FileFontMetrics(string path)
        : this(path, 0) { }

    public FileFontMetrics(string path, long offset)
        : this(FontDescription.LoadDescription(path), path, offset) { }

    internal FileFontMetrics(FontDescription description, string path, long offset)
    {
        Description = description;
        Path = path;
        metrics = new Lazy<StreamFontMetrics>(() => StreamFontMetrics.LoadFont(path, offset));
    }

    /// <inheritdoc cref="FontMetrics.Description" />
    public override FontDescription Description { get; }

    /// <summary>
    ///     Gets the filesystem path to the font face source.
    /// </summary>
    public string Path { get; }

    /// <inheritdoc />
    public override ushort UnitsPerEm => metrics.Value.UnitsPerEm;

    /// <inheritdoc />
    public override float ScaleFactor => metrics.Value.ScaleFactor;

    /// <inheritdoc />
    public override short Ascender => metrics.Value.Ascender;

    /// <inheritdoc />
    public override short Descender => metrics.Value.Descender;

    /// <inheritdoc />
    public override short LineGap => metrics.Value.LineGap;

    /// <inheritdoc />
    public override short LineHeight => metrics.Value.LineHeight;

    /// <inheritdoc />
    public override short AdvanceWidthMax => metrics.Value.AdvanceWidthMax;

    /// <inheritdoc />
    public override short AdvanceHeightMax => metrics.Value.AdvanceHeightMax;

    /// <inheritdoc />
    public override short SubscriptXSize => metrics.Value.SubscriptXSize;

    /// <inheritdoc />
    public override short SubscriptYSize => metrics.Value.SubscriptYSize;

    /// <inheritdoc />
    public override short SubscriptXOffset => metrics.Value.SubscriptXOffset;

    /// <inheritdoc />
    public override short SubscriptYOffset => metrics.Value.SubscriptYOffset;

    /// <inheritdoc />
    public override short SuperscriptXSize => metrics.Value.SuperscriptXSize;

    /// <inheritdoc />
    public override short SuperscriptYSize => metrics.Value.SuperscriptYSize;

    /// <inheritdoc />
    public override short SuperscriptXOffset => metrics.Value.SuperscriptXOffset;

    /// <inheritdoc />
    public override short SuperscriptYOffset => metrics.Value.SuperscriptYOffset;

    /// <inheritdoc />
    public override short StrikeoutSize => metrics.Value.StrikeoutSize;

    /// <inheritdoc />
    public override short StrikeoutPosition => metrics.Value.StrikeoutPosition;

    /// <inheritdoc />
    public override short UnderlinePosition => metrics.Value.UnderlinePosition;

    /// <inheritdoc />
    public override short UnderlineThickness => metrics.Value.UnderlineThickness;

    /// <inheritdoc />
    public override float ItalicAngle => metrics.Value.ItalicAngle;

    /// <inheritdoc />
    internal override bool TryGetGlyphId(CodePoint codePoint, out ushort glyphId)
    {
        return metrics.Value.TryGetGlyphId(codePoint, out glyphId);
    }

    /// <inheritdoc />
    internal override bool TryGetGlyphId(
        CodePoint codePoint,
        CodePoint? nextCodePoint,
        out ushort glyphId,
        out bool skipNextCodePoint)
    {
        return metrics.Value.TryGetGlyphId(codePoint, nextCodePoint, out glyphId, out skipNextCodePoint);
    }

    /// <inheritdoc />
    internal override bool TryGetGlyphClass(ushort glyphId, [NotNullWhen(true)] out GlyphClassDef? glyphClass)
    {
        return metrics.Value.TryGetGlyphClass(glyphId, out glyphClass);
    }

    /// <inheritdoc />
    internal override bool TryGetMarkAttachmentClass(ushort glyphId,
        [NotNullWhen(true)] out GlyphClassDef? markAttachmentClass)
    {
        return metrics.Value.TryGetMarkAttachmentClass(glyphId, out markAttachmentClass);
    }

    /// <inheritdoc />
    public override IEnumerable<GlyphMetrics> GetGlyphMetrics(CodePoint codePoint, ColorFontSupport support)
    {
        return metrics.Value.GetGlyphMetrics(codePoint, support);
    }

    /// <inheritdoc />
    internal override IEnumerable<GlyphMetrics> GetGlyphMetrics(CodePoint codePoint, ushort glyphId,
        ColorFontSupport support)
    {
        return metrics.Value.GetGlyphMetrics(codePoint, glyphId, support);
    }

    /// <inheritdoc />
    internal override void ApplySubstitution(GlyphSubstitutionCollection collection)
    {
        metrics.Value.ApplySubstitution(collection);
    }

    /// <inheritdoc />
    internal override void UpdatePositions(GlyphPositioningCollection collection)
    {
        metrics.Value.UpdatePositions(collection);
    }

    /// <summary>
    ///     Reads a <see cref="StreamFontMetrics" /> from the specified stream.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>a <see cref="StreamFontMetrics" />.</returns>
    public static FileFontMetrics[] LoadFontCollection(string path)
    {
        using var fs = File.OpenRead(path);
        var startPos = fs.Position;
        var reader = new BigEndianBinaryReader(fs, true);
        var ttcHeader = TtcHeader.Read(reader);
        var fonts = new FileFontMetrics[(int)ttcHeader.NumFonts];

        for (var i = 0; i < ttcHeader.NumFonts; ++i)
        {
            fs.Position = startPos + ttcHeader.OffsetTable[i];
            var description = FontDescription.LoadDescription(fs);
            fonts[i] = new FileFontMetrics(description, path, ttcHeader.OffsetTable[i]);
        }

        return fonts;
    }
}