// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Extension methods for <see cref="Buffer2D{T}" />.
///     TODO: One day rewrite all this to use SIMD intrinsics. There's a lot of scope for improvement.
/// </summary>
internal static class Buffer2DUtils
{
    /// <summary>
    ///     Computes the sum of vectors in <paramref name="targetRow" /> weighted by the kernel weight values.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="kernel">The 1D convolution kernel.</param>
    /// <param name="sourcePixels">The source frame.</param>
    /// <param name="targetRow">The target row.</param>
    /// <param name="row">The current row.</param>
    /// <param name="column">The current column.</param>
    /// <param name="minRow">The minimum working area row.</param>
    /// <param name="maxRow">The maximum working area row.</param>
    /// <param name="minColumn">The minimum working area column.</param>
    /// <param name="maxColumn">The maximum working area column.</param>
    public static void Convolve4<TPixel>(
        Span<Complex64> kernel,
        Buffer2D<TPixel> sourcePixels,
        Span<ComplexVector4> targetRow,
        int row,
        int column,
        int minRow,
        int maxRow,
        int minColumn,
        int maxColumn)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        ComplexVector4 vector = default;
        var kernelLength = kernel.Length;
        var radiusY = kernelLength >> 1;
        var sourceOffsetColumnBase = column + minColumn;
        ref var baseRef = ref MemoryMarshal.GetReference(kernel);

        for (var i = 0; i < kernelLength; i++)
        {
            var offsetY = (row + i - radiusY).Clamp(minRow, maxRow);
            var offsetX = sourceOffsetColumnBase.Clamp(minColumn, maxColumn);
            var sourceRowSpan = sourcePixels.GetRowSpan(offsetY);
            var currentColor = sourceRowSpan[offsetX].ToVector4();

            vector.Sum(Unsafe.Add(ref baseRef, i) * currentColor);
        }

        targetRow[column] = vector;
    }

    /// <summary>
    ///     Computes the sum of vectors in <paramref name="targetRow" /> weighted by the kernel weight values and accumulates
    ///     the partial results.
    /// </summary>
    /// <param name="kernel">The 1D convolution kernel.</param>
    /// <param name="sourceValues">The source frame.</param>
    /// <param name="targetRow">The target row.</param>
    /// <param name="row">The current row.</param>
    /// <param name="column">The current column.</param>
    /// <param name="minRow">The minimum working area row.</param>
    /// <param name="maxRow">The maximum working area row.</param>
    /// <param name="minColumn">The minimum working area column.</param>
    /// <param name="maxColumn">The maximum working area column.</param>
    /// <param name="z">The weight factor for the real component of the complex pixel values.</param>
    /// <param name="w">The weight factor for the imaginary component of the complex pixel values.</param>
    public static void Convolve4AndAccumulatePartials(
        Span<Complex64> kernel,
        Buffer2D<ComplexVector4> sourceValues,
        Span<Vector4> targetRow,
        int row,
        int column,
        int minRow,
        int maxRow,
        int minColumn,
        int maxColumn,
        float z,
        float w)
    {
        ComplexVector4 vector = default;
        var kernelLength = kernel.Length;
        var radiusX = kernelLength >> 1;
        var sourceOffsetColumnBase = column + minColumn;

        var offsetY = row.Clamp(minRow, maxRow);
        ref var sourceRef = ref MemoryMarshal.GetReference(sourceValues.GetRowSpan(offsetY));
        ref var baseRef = ref MemoryMarshal.GetReference(kernel);

        for (var x = 0; x < kernelLength; x++)
        {
            var offsetX = (sourceOffsetColumnBase + x - radiusX).Clamp(minColumn, maxColumn);
            vector.Sum(Unsafe.Add(ref baseRef, x) * Unsafe.Add(ref sourceRef, offsetX));
        }

        targetRow[column] += vector.WeightedSum(z, w);
    }
}