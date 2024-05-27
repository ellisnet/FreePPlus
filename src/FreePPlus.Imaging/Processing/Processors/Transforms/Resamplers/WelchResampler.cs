// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     The function implements the welch algorithm.
///     <see href="http://www.imagemagick.org/Usage/filter/" />
/// </summary>
public readonly struct WelchResampler : IResampler
{
    /// <inheritdoc />
    public float Radius => 3;

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public float GetValue(float x)
    {
        if (x < 0F) x = -x;

        if (x < 3F) return ImageMaths.SinC(x) * (1F - x * x / 9F);

        return 0F;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ApplyTransform<TPixel>(IResamplingTransformImageProcessor<TPixel> processor)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        processor.ApplyTransform(in this);
    }
}