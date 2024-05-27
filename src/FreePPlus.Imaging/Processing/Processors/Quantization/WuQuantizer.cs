// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Quantization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

/// <summary>
///     Allows the quantization of images pixels using Xiaolin Wu's Color Quantizer
///     <see href="http://www.ece.mcmaster.ca/~xwu/cq.c" />
/// </summary>
public class WuQuantizer : IQuantizer
{
    private static readonly QuantizerOptions DefaultOptions = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="WuQuantizer" /> class
    ///     using the default <see cref="QuantizerOptions" />.
    /// </summary>
    public WuQuantizer()
        : this(DefaultOptions) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="WuQuantizer" /> class.
    /// </summary>
    /// <param name="options">The quantizer options defining quantization rules.</param>
    public WuQuantizer(QuantizerOptions options)
    {
        Guard.NotNull(options, nameof(options));
        Options = options;
    }

    /// <inheritdoc />
    public QuantizerOptions Options { get; }

    /// <inheritdoc />
    public IQuantizer<TPixel> CreatePixelSpecificQuantizer<TPixel>(Configuration configuration)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return CreatePixelSpecificQuantizer<TPixel>(configuration, Options);
    }

    /// <inheritdoc />
    public IQuantizer<TPixel> CreatePixelSpecificQuantizer<TPixel>(Configuration configuration,
        QuantizerOptions options)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return new WuQuantizer<TPixel>(configuration, options);
    }
}