// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     Implements an algorithm to alter the pixels of an image via resampling transforms.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
public interface IResamplingTransformImageProcessor<TPixel> : IImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    /// <summary>
    ///     Applies a resampling transform with the given sampler.
    /// </summary>
    /// <typeparam name="TResampler">The type of sampler.</typeparam>
    /// <param name="sampler">The sampler to use.</param>
    void ApplyTransform<TResampler>(in TResampler sampler)
        where TResampler : struct, IResampler;
}