// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Quantization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

/// <summary>
///     Allows the quantization of images pixels using Octrees.
///     <see href="http://msdn.microsoft.com/en-us/library/aa479306.aspx" />
/// </summary>
public class OctreeQuantizer : IQuantizer
{
    private static readonly QuantizerOptions DefaultOptions = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="OctreeQuantizer" /> class
    ///     using the default <see cref="QuantizerOptions" />.
    /// </summary>
    public OctreeQuantizer()
        : this(DefaultOptions) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OctreeQuantizer" /> class.
    /// </summary>
    /// <param name="options">The quantizer options defining quantization rules.</param>
    public OctreeQuantizer(QuantizerOptions options)
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
        return new OctreeQuantizer<TPixel>(configuration, options);
    }
}