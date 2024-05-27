// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Extension methods for <see cref="DenseMatrix{T}" />.
///     TODO: One day rewrite all this to use SIMD intrinsics. There's a lot of scope for improvement.
/// </summary>
internal static class DenseMatrixUtils
{
    /// <summary>
    ///     Computes the sum of vectors in the span referenced by <paramref name="targetRowRef" /> weighted by the two kernel
    ///     weight values.
    ///     Using this method the convolution filter is not applied to alpha in addition to the color channels.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="matrixY">The vertical dense matrix.</param>
    /// <param name="matrixX">The horizontal dense matrix.</param>
    /// <param name="sourcePixels">The source frame.</param>
    /// <param name="targetRowRef">The target row base reference.</param>
    /// <param name="row">The current row.</param>
    /// <param name="column">The current column.</param>
    /// <param name="minRow">The minimum working area row.</param>
    /// <param name="maxRow">The maximum working area row.</param>
    /// <param name="minColumn">The minimum working area column.</param>
    /// <param name="maxColumn">The maximum working area column.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void Convolve2D3<TPixel>(
        in DenseMatrix<float> matrixY,
        in DenseMatrix<float> matrixX,
        Buffer2D<TPixel> sourcePixels,
        ref Vector4 targetRowRef,
        int row,
        int column,
        int minRow,
        int maxRow,
        int minColumn,
        int maxColumn)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Convolve2DImpl(
            in matrixY,
            in matrixX,
            sourcePixels,
            row,
            column,
            minRow,
            maxRow,
            minColumn,
            maxColumn,
            out var vector);

        ref var target = ref Unsafe.Add(ref targetRowRef, column);
        vector.W = target.W;

        Vector4Utilities.UnPremultiply(ref vector);
        target = vector;
    }

    /// <summary>
    ///     Computes the sum of vectors in the span referenced by <paramref name="targetRowRef" /> weighted by the two kernel
    ///     weight values.
    ///     Using this method the convolution filter is applied to alpha in addition to the color channels.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="matrixY">The vertical dense matrix.</param>
    /// <param name="matrixX">The horizontal dense matrix.</param>
    /// <param name="sourcePixels">The source frame.</param>
    /// <param name="targetRowRef">The target row base reference.</param>
    /// <param name="row">The current row.</param>
    /// <param name="column">The current column.</param>
    /// <param name="minRow">The minimum working area row.</param>
    /// <param name="maxRow">The maximum working area row.</param>
    /// <param name="minColumn">The minimum working area column.</param>
    /// <param name="maxColumn">The maximum working area column.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void Convolve2D4<TPixel>(
        in DenseMatrix<float> matrixY,
        in DenseMatrix<float> matrixX,
        Buffer2D<TPixel> sourcePixels,
        ref Vector4 targetRowRef,
        int row,
        int column,
        int minRow,
        int maxRow,
        int minColumn,
        int maxColumn)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Convolve2DImpl(
            in matrixY,
            in matrixX,
            sourcePixels,
            row,
            column,
            minRow,
            maxRow,
            minColumn,
            maxColumn,
            out var vector);

        ref var target = ref Unsafe.Add(ref targetRowRef, column);
        Vector4Utilities.UnPremultiply(ref vector);
        target = vector;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public static void Convolve2DImpl<TPixel>(
        in DenseMatrix<float> matrixY,
        in DenseMatrix<float> matrixX,
        Buffer2D<TPixel> sourcePixels,
        int row,
        int column,
        int minRow,
        int maxRow,
        int minColumn,
        int maxColumn,
        out Vector4 vector)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Vector4 vectorY = default;
        Vector4 vectorX = default;
        var matrixHeight = matrixY.Rows;
        var matrixWidth = matrixY.Columns;
        var radiusY = matrixHeight >> 1;
        var radiusX = matrixWidth >> 1;
        var sourceOffsetColumnBase = column + minColumn;

        for (var y = 0; y < matrixHeight; y++)
        {
            var offsetY = (row + y - radiusY).Clamp(minRow, maxRow);
            var sourceRowSpan = sourcePixels.GetRowSpan(offsetY);

            for (var x = 0; x < matrixWidth; x++)
            {
                var offsetX = (sourceOffsetColumnBase + x - radiusX).Clamp(minColumn, maxColumn);
                var currentColor = sourceRowSpan[offsetX].ToVector4();
                Vector4Utilities.Premultiply(ref currentColor);

                vectorX += matrixX[y, x] * currentColor;
                vectorY += matrixY[y, x] * currentColor;
            }
        }

        vector = Vector4.SquareRoot(vectorX * vectorX + vectorY * vectorY);
    }

    /// <summary>
    ///     Computes the sum of vectors in the span referenced by <paramref name="targetRowRef" /> weighted by the kernel
    ///     weight values.
    ///     Using this method the convolution filter is not applied to alpha in addition to the color channels.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="matrix">The dense matrix.</param>
    /// <param name="sourcePixels">The source frame.</param>
    /// <param name="targetRowRef">The target row base reference.</param>
    /// <param name="row">The current row.</param>
    /// <param name="column">The current column.</param>
    /// <param name="minRow">The minimum working area row.</param>
    /// <param name="maxRow">The maximum working area row.</param>
    /// <param name="minColumn">The minimum working area column.</param>
    /// <param name="maxColumn">The maximum working area column.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void Convolve3<TPixel>(
        in DenseMatrix<float> matrix,
        Buffer2D<TPixel> sourcePixels,
        ref Vector4 targetRowRef,
        int row,
        int column,
        int minRow,
        int maxRow,
        int minColumn,
        int maxColumn)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Vector4 vector = default;

        ConvolveImpl(
            in matrix,
            sourcePixels,
            row,
            column,
            minRow,
            maxRow,
            minColumn,
            maxColumn,
            ref vector);

        ref var target = ref Unsafe.Add(ref targetRowRef, column);
        vector.W = target.W;

        Vector4Utilities.UnPremultiply(ref vector);
        target = vector;
    }

    /// <summary>
    ///     Computes the sum of vectors in the span referenced by <paramref name="targetRowRef" /> weighted by the kernel
    ///     weight values.
    ///     Using this method the convolution filter is applied to alpha in addition to the color channels.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <param name="matrix">The dense matrix.</param>
    /// <param name="sourcePixels">The source frame.</param>
    /// <param name="targetRowRef">The target row base reference.</param>
    /// <param name="row">The current row.</param>
    /// <param name="column">The current column.</param>
    /// <param name="minRow">The minimum working area row.</param>
    /// <param name="maxRow">The maximum working area row.</param>
    /// <param name="minColumn">The minimum working area column.</param>
    /// <param name="maxColumn">The maximum working area column.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void Convolve4<TPixel>(
        in DenseMatrix<float> matrix,
        Buffer2D<TPixel> sourcePixels,
        ref Vector4 targetRowRef,
        int row,
        int column,
        int minRow,
        int maxRow,
        int minColumn,
        int maxColumn)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Vector4 vector = default;

        ConvolveImpl(
            in matrix,
            sourcePixels,
            row,
            column,
            minRow,
            maxRow,
            minColumn,
            maxColumn,
            ref vector);

        ref var target = ref Unsafe.Add(ref targetRowRef, column);
        Vector4Utilities.UnPremultiply(ref vector);
        target = vector;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static void ConvolveImpl<TPixel>(
        in DenseMatrix<float> matrix,
        Buffer2D<TPixel> sourcePixels,
        int row,
        int column,
        int minRow,
        int maxRow,
        int minColumn,
        int maxColumn,
        ref Vector4 vector)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var matrixHeight = matrix.Rows;
        var matrixWidth = matrix.Columns;
        var radiusY = matrixHeight >> 1;
        var radiusX = matrixWidth >> 1;
        var sourceOffsetColumnBase = column + minColumn;

        for (var y = 0; y < matrixHeight; y++)
        {
            var offsetY = (row + y - radiusY).Clamp(minRow, maxRow);
            var sourceRowSpan = sourcePixels.GetRowSpan(offsetY);

            for (var x = 0; x < matrixWidth; x++)
            {
                var offsetX = (sourceOffsetColumnBase + x - radiusX).Clamp(minColumn, maxColumn);
                var currentColor = sourceRowSpan[offsetX].ToVector4();
                Vector4Utilities.Premultiply(ref currentColor);
                vector += matrix[y, x] * currentColor;
            }
        }
    }
}