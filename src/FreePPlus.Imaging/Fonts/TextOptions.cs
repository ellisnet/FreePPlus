// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Numerics;
using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Provides configuration options for rendering and shaping text.
/// </summary>
public class TextOptions
{
    private float dpi = 72F;
    private Font? font;
    private float lineSpacing = 1F;
    private float tabWidth = 4F;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TextOptions" /> class.
    /// </summary>
    /// <param name="font">The font.</param>
    public TextOptions(Font font)
    {
        Font = font;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TextOptions" /> class from properties
    ///     copied from the given instance.
    /// </summary>
    /// <param name="options">The options whose properties are copied into this instance.</param>
    public TextOptions(TextOptions options)
    {
        Font = options.Font;
        FallbackFontFamilies = new List<FontFamily>(options.FallbackFontFamilies);
        TabWidth = options.TabWidth;
        HintingMode = options.HintingMode;
        Dpi = options.Dpi;
        LineSpacing = options.LineSpacing;
        Origin = options.Origin;
        WrappingLength = options.WrappingLength;
        WordBreaking = options.WordBreaking;
        TextDirection = options.TextDirection;
        TextAlignment = options.TextAlignment;
        TextJustification = options.TextJustification;
        HorizontalAlignment = options.HorizontalAlignment;
        VerticalAlignment = options.VerticalAlignment;
        LayoutMode = options.LayoutMode;
        KerningMode = options.KerningMode;
        ColorFontSupport = options.ColorFontSupport;
        FeatureTags = new List<Tag>(options.FeatureTags);
        TextRuns = new List<TextRun>(options.TextRuns);
    }

    /// <summary>
    ///     Gets or sets the font.
    /// </summary>
    public Font Font
    {
        get => font!;
        set
        {
            Guard.NotNull(value, nameof(Font));
            font = value;
        }
    }

    /// <summary>
    ///     Gets or sets the collection of fallback font families to use when
    ///     a specific glyph is missing from <see cref="Font" />.
    /// </summary>
    public IReadOnlyList<FontFamily> FallbackFontFamilies { get; set; } = Array.Empty<FontFamily>();

    /// <summary>
    ///     Gets or sets the DPI (Dots Per Inch) to render/measure the text at.
    ///     <para />
    ///     Defaults to 72.
    /// </summary>
    public float Dpi
    {
        get => dpi;

        set
        {
            Guard.MustBeGreaterThanOrEqualTo(value, 0, nameof(Dpi));
            dpi = value;
        }
    }

    /// <summary>
    ///     Gets or sets the width of the tab. Measured as the distance in spaces.
    ///     <para />
    ///     Defaults to 4.
    /// </summary>
    public float TabWidth
    {
        get => tabWidth;

        set
        {
            Guard.MustBeGreaterThanOrEqualTo(value, 0, nameof(TabWidth));
            tabWidth = value;
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether to apply hinting - The use of mathematical instructions
    ///     to adjust the display of an outline font so that it lines up with a rasterized grid.
    /// </summary>
    public HintingMode HintingMode { get; set; }

    /// <summary>
    ///     Gets or sets the line spacing. Applied as a multiple of the line height.
    ///     <para />
    ///     Defaults to 1.
    /// </summary>
    public float LineSpacing
    {
        get => lineSpacing;

        set
        {
            Guard.IsTrue(value != 0, nameof(LineSpacing), "Value must not be equal to 0.");
            lineSpacing = value;
        }
    }

    /// <summary>
    ///     Gets or sets the rendering origin.
    /// </summary>
    public Vector2 Origin { get; set; } = Vector2.Zero;

    /// <summary>
    ///     Gets or sets the length relative to the current DPI at which text will automatically wrap onto a newline.
    /// </summary>
    /// <remarks>
    ///     If value is -1 then wrapping is disabled.
    /// </remarks>
    public float WrappingLength { get; set; } = -1F;

    /// <summary>
    ///     Gets or sets the word breaking mode to use when wrapping text.
    /// </summary>
    public WordBreaking WordBreaking { get; set; }

    /// <summary>
    ///     Gets or sets the text direction.
    /// </summary>
    public TextDirection TextDirection { get; set; } = TextDirection.Auto;

    /// <summary>
    ///     Gets or sets the text alignment of the text within the box.
    /// </summary>
    public TextAlignment TextAlignment { get; set; }

    /// <summary>
    ///     Gets or sets the justification of the text within the box.
    /// </summary>
    public TextJustification TextJustification { get; set; }

    /// <summary>
    ///     Gets or sets the horizontal alignment of the text box.
    /// </summary>
    public HorizontalAlignment HorizontalAlignment { get; set; }

    /// <summary>
    ///     Gets or sets the vertical alignment of the text box.
    /// </summary>
    public VerticalAlignment VerticalAlignment { get; set; }

    /// <summary>
    ///     Gets or sets the layout mode for the text lines.
    /// </summary>
    public LayoutMode LayoutMode { get; set; }

    /// <summary>
    ///     Gets or sets the kerning mode indicating whether to apply kerning (character spacing adjustments)
    ///     to the glyph positions from information found within the font.
    /// </summary>
    public KerningMode KerningMode { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to enable various color font formats.
    /// </summary>
    public ColorFontSupport ColorFontSupport { get; set; } = ColorFontSupport.MicrosoftColrFormat;

    /// <summary>
    ///     Gets or sets the collection of additional feature tags to apply during glyph shaping.
    /// </summary>
    public IReadOnlyList<Tag> FeatureTags { get; set; } = Array.Empty<Tag>();

    /// <summary>
    ///     Gets or sets an optional collection of text runs to apply to the body of text.
    /// </summary>
    public IReadOnlyList<TextRun> TextRuns { get; set; } = Array.Empty<TextRun>();
}