// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Encapsulated logic for laying out and then rendering text to a <see cref="IGlyphRenderer" /> surface.
/// </summary>
public class TextRenderer
{
    private readonly IGlyphRenderer renderer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TextRenderer" /> class.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    public TextRenderer(IGlyphRenderer renderer)
    {
        this.renderer = renderer;
    }

    /// <summary>
    ///     Renders the text to the <paramref name="renderer" />.
    /// </summary>
    /// <param name="renderer">The target renderer.</param>
    /// <param name="text">The text.</param>
    /// <param name="options">The style.</param>
    public static void RenderTextTo(IGlyphRenderer renderer, ReadOnlySpan<char> text, TextOptions options)
    {
        new TextRenderer(renderer).RenderText(text, options);
    }

    /// <summary>
    ///     Renders the text to the <paramref name="renderer" />.
    /// </summary>
    /// <param name="renderer">The target renderer.</param>
    /// <param name="text">The text.</param>
    /// <param name="options">The style.</param>
    public static void RenderTextTo(IGlyphRenderer renderer, string text, TextOptions options)
    {
        new TextRenderer(renderer).RenderText(text, options);
    }

    /// <summary>
    ///     Renders the text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="options">The style.</param>
    public void RenderText(string text, TextOptions options)
    {
        RenderText(text.AsSpan(), options);
    }

    /// <summary>
    ///     Renders the text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="options">The style.</param>
    public void RenderText(ReadOnlySpan<char> text, TextOptions options)
    {
        var glyphsToRender = TextLayout.GenerateLayout(text, options);
        var rect = TextMeasurer.GetBounds(glyphsToRender, options.Dpi);

        renderer.BeginText(rect);

        foreach (var g in glyphsToRender) g.Glyph.RenderTo(renderer, g.Location, options);

        renderer.EndText();
    }
}