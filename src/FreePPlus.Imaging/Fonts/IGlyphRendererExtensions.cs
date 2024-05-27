// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     A surface that can have a glyph rendered to it as a series of actions.
/// </summary>
public static class IGlyphRendererExtensions
{
    /// <summary>
    ///     Renders the text.
    /// </summary>
    /// <param name="renderer">The target renderer surface.</param>
    /// <param name="text">The text.</param>
    /// <param name="options">The options.</param>
    /// <returns>Returns the original <paramref name="renderer" /></returns>
    public static IGlyphRenderer Render(this IGlyphRenderer renderer, ReadOnlySpan<char> text, TextOptions options)
    {
        new TextRenderer(renderer).RenderText(text, options);
        return renderer;
    }
}