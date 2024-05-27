// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Fonts.Unicode;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Defines a particular format for text, including font face, size, and style attributes.
///     This class cannot be inherited.
/// </summary>
public sealed class Font
{
    private readonly Lazy<string> _fontName;
    private readonly Lazy<FontMetrics?> _metrics;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Font" /> class.
    /// </summary>
    /// <param name="family">The font family.</param>
    /// <param name="size">The size of the font in PT units.</param>
    public Font(FontFamily family, float size)
        : this(family, size, FontStyle.Regular) { }

    public Font(string name, float size, FontStyle style)
        : this(FontHelper.GetFontByName(name), size, style) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Font" /> class.
    /// </summary>
    /// <param name="family">The font family.</param>
    /// <param name="size">The size of the font in PT units.</param>
    /// <param name="style">The font style.</param>
    public Font(FontFamily family, float size, FontStyle style)
    {
        if (family == default)
            throw new ArgumentException("Cannot use the default value type instance to create a font.", nameof(family));

        Family = family;
        RequestedStyle = style;
        Size = size;
        _metrics = new Lazy<FontMetrics?>(LoadInstanceInternal);
        _fontName = new Lazy<string>(LoadFontName);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Font" /> class.
    /// </summary>
    /// <param name="prototype">The prototype.</param>
    /// <param name="style">The font style.</param>
    public Font(Font prototype, FontStyle style)
        : this(prototype?.Family ?? throw new ArgumentNullException(nameof(prototype)), prototype.Size, style) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Font" /> class.
    /// </summary>
    /// <param name="prototype">The prototype.</param>
    /// <param name="size">The size of the font in PT units.</param>
    /// <param name="style">The font style.</param>
    public Font(Font prototype, float size, FontStyle style)
        : this(prototype?.Family ?? throw new ArgumentNullException(nameof(prototype)), size, style) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Font" /> class.
    /// </summary>
    /// <param name="prototype">The prototype.</param>
    /// <param name="size">The size of the font in PT units.</param>
    public Font(Font prototype, float size)
        : this(prototype.Family, size, prototype.RequestedStyle) { }

    /// <summary>
    ///     Gets the family.
    /// </summary>
    public FontFamily Family { get; }

    /// <summary>
    ///     Gets the name.
    /// </summary>
    public string Name => _fontName.Value;

    /// <summary>
    ///     Gets the size of the font in PT units.
    /// </summary>
    public float Size { get; }

    /// <summary>
    ///     Gets the font metrics.
    /// </summary>
    public FontMetrics FontMetrics => _metrics.Value ?? throw new FontException("Font instance not found.");

    /// <summary>
    ///     Gets a value indicating whether this <see cref="Font" /> is bold.
    /// </summary>
    public bool IsBold => (FontMetrics.Description.Style & FontStyle.Bold) == FontStyle.Bold;

    /// <summary>
    ///     Gets a value indicating whether this <see cref="Font" /> is italic.
    /// </summary>
    public bool IsItalic => (FontMetrics.Description.Style & FontStyle.Italic) == FontStyle.Italic;

    /// <summary>
    ///     Gets a value indicating whether this <see cref="Font" /> is underline.
    /// </summary>
    public bool IsUnderline =>
        false; //FontMetrics.Description.Style == FontStyle.Underline; //TODO: Underline not actually supported at this time

    /// <summary>
    ///     Gets a value indicating whether this <see cref="Font" /> is strikeout.
    /// </summary>
    public bool IsStrikeout =>
        false; //FontMetrics.Description.Style == FontStyle.Strikeout; //TODO: Strikeout not actually supported at this time

    /// <summary>
    ///     Gets the requested style.
    /// </summary>
    internal FontStyle RequestedStyle { get; }

    /// <summary>
    ///     Gets the filesystem path to the font family source.
    /// </summary>
    /// <param name="path">
    ///     When this method returns, contains the filesystem path to the font family source,
    ///     if the path exists; otherwise, the default value for the type of the path parameter.
    ///     This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if the <see cref="Font" /> was created via a filesystem path; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    public bool TryGetPath([NotNullWhen(true)] out string? path)
    {
        if (this == default) FontsThrowHelper.ThrowDefaultInstance();

        if (FontMetrics is FileFontMetrics fileMetrics)
        {
            path = fileMetrics.Path;
            return true;
        }

        path = null;
        return false;
    }

    /// <summary>
    ///     Gets the glyphs for the given codepoint.
    /// </summary>
    /// <param name="codePoint">The code point of the character.</param>
    /// <param name="support">Options for enabling color font support during layout and rendering.</param>
    /// <returns>Returns the glyph</returns>
    public IEnumerable<Glyph> GetGlyphs(CodePoint codePoint, ColorFontSupport support)
    {
        return GetGlyphs(codePoint, TextAttributes.None, support);
    }

    /// <summary>
    ///     Gets the glyphs for the given codepoint.
    /// </summary>
    /// <param name="codePoint">The code point of the character.</param>
    /// <param name="textAttributes">The text attributes to apply to the glyphs.</param>
    /// <param name="support">Options for enabling color font support during layout and rendering.</param>
    /// <returns>Returns the glyph</returns>
    public IEnumerable<Glyph> GetGlyphs(CodePoint codePoint, TextAttributes textAttributes, ColorFontSupport support)
    {
        TextRun textRun = new() { Start = 0, End = 1, Font = this, TextAttributes = textAttributes };
        foreach (var metrics in FontMetrics.GetGlyphMetrics(codePoint, support))
            yield return new Glyph(metrics.CloneForRendering(textRun, codePoint), Size);
    }

    private string LoadFontName()
    {
        return _metrics.Value?.Description.FontName(Family.Culture) ?? string.Empty;
    }

    private FontMetrics? LoadInstanceInternal()
    {
        if (Family.TryGetMetrics(RequestedStyle, out var metrics)) return metrics;

        if (RequestedStyle.HasFlag(FontStyle.Italic))
            // Can't find style requested and they want one that's at least partial italic.
            // Try the regular italic.
            if (Family.TryGetMetrics(FontStyle.Italic, out metrics))
                return metrics;

        if (RequestedStyle.HasFlag(FontStyle.Bold))
            // Can't find style requested and they want one that's at least partial bold.
            // Try the regular bold.
            if (Family.TryGetMetrics(FontStyle.Bold, out metrics))
                return metrics;

        // Can't find style requested so let's just try returning the default.
        var styles = Family.GetAvailableStyles();
        var defaultStyle = styles.Contains(FontStyle.Regular)
            ? FontStyle.Regular
            : styles.First();

        Family.TryGetMetrics(defaultStyle, out metrics);
        return metrics;
    }
}