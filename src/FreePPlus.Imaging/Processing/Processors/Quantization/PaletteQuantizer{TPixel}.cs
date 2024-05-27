// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

#pragma warning disable IDE0250
#pragma warning disable IDE0251

namespace FreePPlus.Imaging.Processing.Processors.Quantization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

/// <summary>
///     Encapsulates methods to create a quantized image based upon the given palette.
///     <see href="http://msdn.microsoft.com/en-us/library/aa479306.aspx" />
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal struct PaletteQuantizer<TPixel> : IQuantizer<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly EuclideanPixelMap<TPixel> pixelMap;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PaletteQuantizer{TPixel}" /> struct.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="options">The quantizer options defining quantization rules.</param>
    /// <param name="pixelMap">The pixel map for looking up color matches from a predefined palette.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public PaletteQuantizer(
        Configuration configuration,
        QuantizerOptions options,
        EuclideanPixelMap<TPixel> pixelMap)
    {
        Guard.NotNull(configuration, nameof(configuration));
        Guard.NotNull(options, nameof(options));

        Configuration = configuration;
        Options = options;
        this.pixelMap = pixelMap;
    }

    /// <inheritdoc />
    public Configuration Configuration { get; }

    /// <inheritdoc />
    public QuantizerOptions Options { get; }

    /// <inheritdoc />
    public ReadOnlyMemory<TPixel> Palette => pixelMap.Palette;

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly IndexedImageFrame<TPixel> QuantizeFrame(ImageFrame<TPixel> source, Rectangle bounds)
    {
        return QuantizerUtilities.QuantizeFrame(ref Unsafe.AsRef(in this), source, bounds);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void AddPaletteColors(Buffer2DRegion<TPixel> pixelRegion) { }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly byte GetQuantizedColor(TPixel color, out TPixel match)
    {
        return (byte)pixelMap.GetClosestColor(color, out match);
    }

    /// <inheritdoc />
    public void Dispose() { }
}