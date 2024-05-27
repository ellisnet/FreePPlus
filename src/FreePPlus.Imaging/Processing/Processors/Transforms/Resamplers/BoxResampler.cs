// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     The function implements the box algorithm. Similar to nearest neighbor when upscaling.
///     When downscaling the pixels will average, merging together.
/// </summary>
public readonly struct BoxResampler : IResampler
{
    /// <inheritdoc />
    public float Radius => 0.5F;

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public float GetValue(float x)
    {
        if (x > -0.5F && x <= 0.5F) return 1;

        return 0;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ApplyTransform<TPixel>(IResamplingTransformImageProcessor<TPixel> processor)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        processor.ApplyTransform(in this);
    }
}