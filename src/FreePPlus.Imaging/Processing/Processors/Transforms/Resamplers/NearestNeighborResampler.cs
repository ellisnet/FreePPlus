// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     The function implements the nearest neighbor algorithm. This uses an unscaled filter
///     which will select the closest pixel to the new pixels position.
/// </summary>
public readonly struct NearestNeighborResampler : IResampler
{
    /// <inheritdoc />
    public float Radius => 1;

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public float GetValue(float x)
    {
        return x;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ApplyTransform<TPixel>(IResamplingTransformImageProcessor<TPixel> processor)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        processor.ApplyTransform(in this);
    }
}