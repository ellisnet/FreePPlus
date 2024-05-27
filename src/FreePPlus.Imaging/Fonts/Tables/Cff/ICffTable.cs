// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

/// <summary>
///     Defines a common interface for CFF1 and CFF2 tables.
/// </summary>
internal interface ICffTable
{
    /// <summary>
    ///     Gets the number of glyphs in the table.
    /// </summary>
    int GlyphCount { get; }

    /// <summary>
    ///     Gets the glyph data at the given index.
    /// </summary>
    /// <param name="index">The glyph index.</param>
    /// <returns>The <see cref="CffGlyphData" />.</returns>
    CffGlyphData GetGlyph(int index);
}