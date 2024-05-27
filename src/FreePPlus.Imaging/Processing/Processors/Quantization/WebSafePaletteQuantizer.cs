// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing.Processors.Quantization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

/// <summary>
///     A palette quantizer consisting of web safe colors as defined in the CSS Color Module Level 4.
/// </summary>
public class WebSafePaletteQuantizer : PaletteQuantizer
{
    private static readonly QuantizerOptions DefaultOptions = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="WebSafePaletteQuantizer" /> class.
    /// </summary>
    public WebSafePaletteQuantizer()
        : this(DefaultOptions) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="WebSafePaletteQuantizer" /> class.
    /// </summary>
    /// <param name="options">The quantizer options defining quantization rules.</param>
    public WebSafePaletteQuantizer(QuantizerOptions options)
        : base(Color.WebSafePalette, options) { }
}