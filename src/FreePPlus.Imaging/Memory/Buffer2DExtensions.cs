// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.Memory;

//was previously: namespace SixLabors.ImageSharp.Memory;

/// <summary>
///     Defines extension methods for <see cref="Buffer2D{T}" />.
/// </summary>
public static class Buffer2DExtensions
{
    /// <summary>
    ///     Gets the backing <see cref="IMemoryGroup{T}" />.
    /// </summary>
    /// <param name="buffer">The buffer.</param>
    /// <typeparam name="T">The element type.</typeparam>
    /// <returns>The MemoryGroup.</returns>
    public static IMemoryGroup<T> GetMemoryGroup<T>(this Buffer2D<T> buffer)
        where T : struct
    {
        Guard.NotNull(buffer, nameof(buffer));
        return buffer.FastMemoryGroup.View;
    }

    /// <summary>
    ///     TODO: Does not work with multi-buffer groups, should be specific to Resize.
    ///     Copy <paramref name="columnCount" /> columns of <paramref name="buffer" /> inplace,
    ///     from positions starting at <paramref name="sourceIndex" /> to positions at <paramref name="destIndex" />.
    /// </summary>
    internal static unsafe void CopyColumns<T>(
        this Buffer2D<T> buffer,
        int sourceIndex,
        int destIndex,
        int columnCount)
        where T : struct
    {
        DebugGuard.NotNull(buffer, nameof(buffer));
        DebugGuard.MustBeGreaterThanOrEqualTo(sourceIndex, 0, nameof(sourceIndex));
        DebugGuard.MustBeGreaterThanOrEqualTo(destIndex, 0, nameof(sourceIndex));
        CheckColumnRegionsDoNotOverlap(buffer, sourceIndex, destIndex, columnCount);

        var elementSize = Unsafe.SizeOf<T>();
        var width = buffer.Width * elementSize;
        var sOffset = sourceIndex * elementSize;
        var dOffset = destIndex * elementSize;
        long count = columnCount * elementSize;

        var span = MemoryMarshal.AsBytes(buffer.GetSingleMemory().Span);

        fixed (byte* ptr = span)
        {
            var basePtr = ptr;
            for (var y = 0; y < buffer.Height; y++)
            {
                var sPtr = basePtr + sOffset;
                var dPtr = basePtr + dOffset;

                Buffer.MemoryCopy(sPtr, dPtr, count, count);

                basePtr += width;
            }
        }
    }

    /// <summary>
    ///     Returns a <see cref="Rectangle" /> representing the full area of the buffer.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="buffer">The <see cref="Buffer2D{T}" /></param>
    /// <returns>The <see cref="Rectangle" /></returns>
    internal static Rectangle FullRectangle<T>(this Buffer2D<T> buffer)
        where T : struct
    {
        return new Rectangle(0, 0, buffer.Width, buffer.Height);
    }

    /// <summary>
    ///     Return a <see cref="Buffer2DRegion{T}" /> to the subregion represented by 'rectangle'
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="buffer">The <see cref="Buffer2D{T}" /></param>
    /// <param name="rectangle">The rectangle subregion</param>
    /// <returns>The <see cref="Buffer2DRegion{T}" /></returns>
    internal static Buffer2DRegion<T> GetRegion<T>(this Buffer2D<T> buffer, Rectangle rectangle)
        where T : unmanaged
    {
        return new Buffer2DRegion<T>(buffer, rectangle);
    }

    internal static Buffer2DRegion<T> GetRegion<T>(this Buffer2D<T> buffer, int x, int y, int width, int height)
        where T : unmanaged
    {
        return new Buffer2DRegion<T>(buffer, new Rectangle(x, y, width, height));
    }

    /// <summary>
    ///     Return a <see cref="Buffer2DRegion{T}" /> to the whole area of 'buffer'
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="buffer">The <see cref="Buffer2D{T}" /></param>
    /// <returns>The <see cref="Buffer2DRegion{T}" /></returns>
    internal static Buffer2DRegion<T> GetRegion<T>(this Buffer2D<T> buffer)
        where T : unmanaged
    {
        return new Buffer2DRegion<T>(buffer);
    }

    /// <summary>
    ///     Returns the size of the buffer.
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    /// <param name="buffer">The <see cref="Buffer2D{T}" /></param>
    /// <returns>The <see cref="Size{T}" /> of the buffer</returns>
    internal static Size Size<T>(this Buffer2D<T> buffer)
        where T : struct
    {
        return new Size(buffer.Width, buffer.Height);
    }

    [Conditional("DEBUG")]
    private static void CheckColumnRegionsDoNotOverlap<T>(
        Buffer2D<T> buffer,
        int sourceIndex,
        int destIndex,
        int columnCount)
        where T : struct
    {
        var minIndex = Math.Min(sourceIndex, destIndex);
        var maxIndex = Math.Max(sourceIndex, destIndex);
        if (maxIndex < minIndex + columnCount || maxIndex > buffer.Width - columnCount)
            throw new InvalidOperationException("Column regions should not overlap!");
    }
}