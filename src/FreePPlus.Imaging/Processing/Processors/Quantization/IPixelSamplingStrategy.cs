// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Quantization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

/// <summary>
///     Provides an abstraction to enumerate pixel regions within a multi-framed <see cref="Image{TPixel}" />.
/// </summary>
public interface IPixelSamplingStrategy
{
    /// <summary>
    ///     Enumerates pixel regions within the image as <see cref="Buffer2DRegion{T}" />.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <typeparam name="TPixel">The pixel type.</typeparam>
    /// <returns>An enumeration of pixel regions.</returns>
    IEnumerable<Buffer2DRegion<TPixel>> EnumeratePixelRegions<TPixel>(Image<TPixel> image)
        where TPixel : unmanaged, IPixel<TPixel>;
}