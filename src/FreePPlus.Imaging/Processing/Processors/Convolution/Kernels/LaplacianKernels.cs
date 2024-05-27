﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing.Processors.Convolution;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

/// <summary>
///     Contains Laplacian kernels of different sizes
/// </summary>
internal static class LaplacianKernels
{
    /// <summary>
    ///     Gets the 3x3 Laplacian kernel
    /// </summary>
    public static DenseMatrix<float> Laplacian3x3 => LaplacianKernelFactory.CreateKernel(3);

    /// <summary>
    ///     Gets the 5x5 Laplacian kernel
    /// </summary>
    public static DenseMatrix<float> Laplacian5x5 => LaplacianKernelFactory.CreateKernel(5);

    /// <summary>
    ///     Gets the Laplacian of Gaussian kernel.
    /// </summary>
    public static DenseMatrix<float> LaplacianOfGaussianXY =>
        new float[,]
        {
            { 0, 0, -1, 0, 0 },
            { 0, -1, -2, -1, 0 },
            { -1, -2, 16, -2, -1 },
            { 0, -1, -2, -1, 0 },
            { 0, 0, -1, 0, 0 }
        };
}