// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.Formats.Png.Filters;

//was previously: namespace SixLabors.ImageSharp.Formats.Png.Filters;

/// <summary>
///     The Up filter is just like the Sub filter except that the pixel immediately above the current pixel,
///     rather than just to its left, is used as the predictor.
///     <see href="https://www.w3.org/TR/PNG-Filters.html" />
/// </summary>
internal static class UpFilter
{
    /// <summary>
    ///     Decodes the scanline
    /// </summary>
    /// <param name="scanline">The scanline to decode</param>
    /// <param name="previousScanline">The previous scanline.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Decode(Span<byte> scanline, Span<byte> previousScanline)
    {
        DebugGuard.MustBeSameSized(scanline, previousScanline, nameof(scanline));

        ref var scanBaseRef = ref MemoryMarshal.GetReference(scanline);
        ref var prevBaseRef = ref MemoryMarshal.GetReference(previousScanline);

        // Up(x) + Prior(x)
        for (var x = 1; x < scanline.Length; x++)
        {
            ref var scan = ref Unsafe.Add(ref scanBaseRef, x);
            var above = Unsafe.Add(ref prevBaseRef, x);
            scan = (byte)(scan + above);
        }
    }

    /// <summary>
    ///     Encodes the scanline
    /// </summary>
    /// <param name="scanline">The scanline to encode</param>
    /// <param name="previousScanline">The previous scanline.</param>
    /// <param name="result">The filtered scanline result.</param>
    /// <param name="sum">The sum of the total variance of the filtered row</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Encode(Span<byte> scanline, Span<byte> previousScanline, Span<byte> result, out int sum)
    {
        DebugGuard.MustBeSameSized(scanline, previousScanline, nameof(scanline));
        DebugGuard.MustBeSizedAtLeast(result, scanline, nameof(result));

        ref var scanBaseRef = ref MemoryMarshal.GetReference(scanline);
        ref var prevBaseRef = ref MemoryMarshal.GetReference(previousScanline);
        ref var resultBaseRef = ref MemoryMarshal.GetReference(result);
        sum = 0;

        // Up(x) = Raw(x) - Prior(x)
        resultBaseRef = 2;

        for (var x = 0; x < scanline.Length; /* Note: ++x happens in the body to avoid one add operation */)
        {
            var scan = Unsafe.Add(ref scanBaseRef, x);
            var above = Unsafe.Add(ref prevBaseRef, x);
            ++x;
            ref var res = ref Unsafe.Add(ref resultBaseRef, x);
            res = (byte)(scan - above);
            sum += ImageMaths.FastAbs(unchecked((sbyte)res));
        }

        sum -= 2;
    }
}