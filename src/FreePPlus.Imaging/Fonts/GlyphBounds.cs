// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Fonts.Unicode;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Represents the bounds of a <see cref="Glyph" /> for a given <see cref="CodePoint" />.
/// </summary>
public readonly struct GlyphBounds
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GlyphBounds" /> struct.
    /// </summary>
    /// <param name="codePoint">The Unicode codepoint for the glyph.</param>
    /// <param name="bounds">The glyph bounds.</param>
    public GlyphBounds(CodePoint codePoint, FontRectangle bounds)
    {
        Codepoint = codePoint;
        Bounds = bounds;
    }

    /// <summary>
    ///     Gets the Unicode codepoint of the glyph.
    /// </summary>
    public CodePoint Codepoint { get; }

    /// <summary>
    ///     Gets the glyph bounds.
    /// </summary>
    public FontRectangle Bounds { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Codepoint: {Codepoint}, Bounds: {Bounds}.";
    }
}