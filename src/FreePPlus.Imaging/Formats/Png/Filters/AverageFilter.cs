﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.Formats.Png.Filters;

//was previously: namespace SixLabors.ImageSharp.Formats.Png.Filters;

/// <summary>
///     The Average filter uses the average of the two neighboring pixels (left and above) to predict
///     the value of a pixel.
///     <see href="https://www.w3.org/TR/PNG-Filters.html" />
/// </summary>
internal static class AverageFilter
{
    /// <summary>
    ///     Decodes the scanline
    /// </summary>
    /// <param name="scanline">The scanline to decode</param>
    /// <param name="previousScanline">The previous scanline.</param>
    /// <param name="bytesPerPixel">The bytes per pixel.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Decode(Span<byte> scanline, Span<byte> previousScanline, int bytesPerPixel)
    {
        DebugGuard.MustBeSameSized(scanline, previousScanline, nameof(scanline));

        ref var scanBaseRef = ref MemoryMarshal.GetReference(scanline);
        ref var prevBaseRef = ref MemoryMarshal.GetReference(previousScanline);

        // Average(x) + floor((Raw(x-bpp)+Prior(x))/2)
        var x = 1;
        for (; x <= bytesPerPixel /* Note the <= because x starts at 1 */; ++x)
        {
            ref var scan = ref Unsafe.Add(ref scanBaseRef, x);
            var above = Unsafe.Add(ref prevBaseRef, x);
            scan = (byte)(scan + (above >> 1));
        }

        for (; x < scanline.Length; ++x)
        {
            ref var scan = ref Unsafe.Add(ref scanBaseRef, x);
            var left = Unsafe.Add(ref scanBaseRef, x - bytesPerPixel);
            var above = Unsafe.Add(ref prevBaseRef, x);
            scan = (byte)(scan + Average(left, above));
        }
    }

    /// <summary>
    ///     Encodes the scanline
    /// </summary>
    /// <param name="scanline">The scanline to encode</param>
    /// <param name="previousScanline">The previous scanline.</param>
    /// <param name="result">The filtered scanline result.</param>
    /// <param name="bytesPerPixel">The bytes per pixel.</param>
    /// <param name="sum">The sum of the total variance of the filtered row</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Encode(Span<byte> scanline, Span<byte> previousScanline, Span<byte> result, int bytesPerPixel,
        out int sum)
    {
        DebugGuard.MustBeSameSized(scanline, previousScanline, nameof(scanline));
        DebugGuard.MustBeSizedAtLeast(result, scanline, nameof(result));

        ref var scanBaseRef = ref MemoryMarshal.GetReference(scanline);
        ref var prevBaseRef = ref MemoryMarshal.GetReference(previousScanline);
        ref var resultBaseRef = ref MemoryMarshal.GetReference(result);
        sum = 0;

        // Average(x) = Raw(x) - floor((Raw(x-bpp)+Prior(x))/2)
        resultBaseRef = 3;

        var x = 0;
        for (; x < bytesPerPixel; /* Note: ++x happens in the body to avoid one add operation */)
        {
            var scan = Unsafe.Add(ref scanBaseRef, x);
            var above = Unsafe.Add(ref prevBaseRef, x);
            ++x;
            ref var res = ref Unsafe.Add(ref resultBaseRef, x);
            res = (byte)(scan - (above >> 1));
            sum += ImageMaths.FastAbs(unchecked((sbyte)res));
        }

        for (var xLeft = x - bytesPerPixel;
             x < scanline.Length;
             ++xLeft /* Note: ++x happens in the body to avoid one add operation */)
        {
            var scan = Unsafe.Add(ref scanBaseRef, x);
            var left = Unsafe.Add(ref scanBaseRef, xLeft);
            var above = Unsafe.Add(ref prevBaseRef, x);
            ++x;
            ref var res = ref Unsafe.Add(ref resultBaseRef, x);
            res = (byte)(scan - Average(left, above));
            sum += ImageMaths.FastAbs(unchecked((sbyte)res));
        }

        sum -= 3;
    }

    /// <summary>
    ///     Calculates the average value of two bytes
    /// </summary>
    /// <param name="left">The left byte</param>
    /// <param name="above">The above byte</param>
    /// <returns>The <see cref="int" /></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Average(byte left, byte above)
    {
        return (left + above) >> 1;
    }
}