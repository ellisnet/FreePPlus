// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Quantization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

/// <summary>
///     A pixel sampling strategy that enumerates all pixels.
/// </summary>
public class ExtensivePixelSamplingStrategy : IPixelSamplingStrategy
{
    /// <inheritdoc />
    public IEnumerable<Buffer2DRegion<TPixel>> EnumeratePixelRegions<TPixel>(Image<TPixel> image)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        foreach (ImageFrame<TPixel> frame in image.Frames) yield return frame.PixelBuffer.GetRegion();
    }
}