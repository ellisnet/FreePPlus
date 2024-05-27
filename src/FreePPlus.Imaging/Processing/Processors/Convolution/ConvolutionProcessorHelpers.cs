// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Processing.Processors.Convolution;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal static class ConvolutionProcessorHelpers
{
    /// <summary>
    ///     Kernel radius is calculated using the minimum viable value.
    ///     See <see href="http://chemaguerra.com/gaussian-filter-radius/" />.
    /// </summary>
    internal static int GetDefaultGaussianRadius(float sigma)
    {
        return (int)MathF.Ceiling(sigma * 3);
    }

    /// <summary>
    ///     Create a 1 dimensional Gaussian kernel using the Gaussian G(x) function.
    /// </summary>
    /// <returns>The <see cref="DenseMatrix{T}" />.</returns>
    internal static DenseMatrix<float> CreateGaussianBlurKernel(int size, float weight)
    {
        var kernel = new DenseMatrix<float>(size, 1);

        var sum = 0F;
        var midpoint = (size - 1) / 2F;

        for (var i = 0; i < size; i++)
        {
            var x = i - midpoint;
            var gx = ImageMaths.Gaussian(x, weight);
            sum += gx;
            kernel[0, i] = gx;
        }

        // Normalize kernel so that the sum of all weights equals 1
        for (var i = 0; i < size; i++) kernel[0, i] /= sum;

        return kernel;
    }

    /// <summary>
    ///     Create a 1 dimensional Gaussian kernel using the Gaussian G(x) function
    /// </summary>
    /// <returns>The <see cref="DenseMatrix{T}" />.</returns>
    internal static DenseMatrix<float> CreateGaussianSharpenKernel(int size, float weight)
    {
        var kernel = new DenseMatrix<float>(size, 1);

        float sum = 0;

        var midpoint = (size - 1) / 2F;
        for (var i = 0; i < size; i++)
        {
            var x = i - midpoint;
            var gx = ImageMaths.Gaussian(x, weight);
            sum += gx;
            kernel[0, i] = gx;
        }

        // Invert the kernel for sharpening.
        var midpointRounded = (int)midpoint;
        for (var i = 0; i < size; i++)
            if (i == midpointRounded)
                // Calculate central value
                kernel[0, i] = 2F * sum - kernel[0, i];
            else
                // invert value
                kernel[0, i] = -kernel[0, i];

        // Normalize kernel so that the sum of all weights equals 1
        for (var i = 0; i < size; i++) kernel[0, i] /= sum;

        return kernel;
    }
}