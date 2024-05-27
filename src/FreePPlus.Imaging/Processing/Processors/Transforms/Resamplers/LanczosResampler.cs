// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     The function implements the Lanczos kernel algorithm as described on
///     <see href="https://en.wikipedia.org/wiki/Lanczos_resampling#Algorithm">Wikipedia</see>.
/// </summary>
public readonly struct LanczosResampler : IResampler
{
    /// <summary>
    ///     Implements the Lanczos kernel algorithm with a radius of 2.
    /// </summary>
    public static LanczosResampler Lanczos2 = new(2);

    /// <summary>
    ///     Implements the Lanczos kernel algorithm with a radius of 3.
    /// </summary>
    public static LanczosResampler Lanczos3 = new(3);

    /// <summary>
    ///     Implements the Lanczos kernel algorithm with a radius of 5.
    /// </summary>
    public static LanczosResampler Lanczos5 = new(5);

    /// <summary>
    ///     Implements the Lanczos kernel algorithm with a radius of 8.
    /// </summary>
    public static LanczosResampler Lanczos8 = new(8);

    /// <summary>
    ///     Initializes a new instance of the <see cref="LanczosResampler" /> struct.
    /// </summary>
    /// <param name="radius">The sampling radius.</param>
    public LanczosResampler(float radius)
    {
        Radius = radius;
    }

    /// <inheritdoc />
    public float Radius { get; }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public float GetValue(float x)
    {
        if (x < 0F) x = -x;

        var radius = Radius;
        if (x < radius) return ImageMaths.SinC(x) * ImageMaths.SinC(x / radius);

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