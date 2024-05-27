// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Convolution;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

/// <summary>
///     Defines Gaussian blur by a (Sigma, Radius) pair.
/// </summary>
public sealed class GaussianBlurProcessor : IImageProcessor
{
    /// <summary>
    ///     The default value for <see cref="Sigma" />.
    /// </summary>
    public const float DefaultSigma = 3f;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GaussianBlurProcessor" /> class.
    /// </summary>
    public GaussianBlurProcessor()
        : this(DefaultSigma, ConvolutionProcessorHelpers.GetDefaultGaussianRadius(DefaultSigma)) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GaussianBlurProcessor" /> class.
    /// </summary>
    /// <param name="sigma">The 'sigma' value representing the weight of the blur.</param>
    public GaussianBlurProcessor(float sigma)
        : this(sigma, ConvolutionProcessorHelpers.GetDefaultGaussianRadius(sigma)) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GaussianBlurProcessor" /> class.
    /// </summary>
    /// <param name="radius">
    ///     The 'radius' value representing the size of the area to sample.
    /// </param>
    public GaussianBlurProcessor(int radius)
        : this(radius / 3F, radius) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GaussianBlurProcessor" /> class.
    /// </summary>
    /// <param name="sigma">
    ///     The 'sigma' value representing the weight of the blur.
    /// </param>
    /// <param name="radius">
    ///     The 'radius' value representing the size of the area to sample.
    ///     This should be at least twice the sigma value.
    /// </param>
    public GaussianBlurProcessor(float sigma, int radius)
    {
        Sigma = sigma;
        Radius = radius;
    }

    /// <summary>
    ///     Gets the sigma value representing the weight of the blur
    /// </summary>
    public float Sigma { get; }

    /// <summary>
    ///     Gets the radius defining the size of the area to sample.
    /// </summary>
    public int Radius { get; }

    /// <inheritdoc />
    public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration,
        Image<TPixel> source, Rectangle sourceRectangle)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return new GaussianBlurProcessor<TPixel>(configuration, this, source, sourceRectangle);
    }
}