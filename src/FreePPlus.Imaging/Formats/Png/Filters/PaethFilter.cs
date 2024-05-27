// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.Formats.Png.Filters;

//was previously: namespace SixLabors.ImageSharp.Formats.Png.Filters;

/// <summary>
///     The Paeth filter computes a simple linear function of the three neighboring pixels (left, above, upper left),
///     then chooses as predictor the neighboring pixel closest to the computed value.
///     This technique is due to Alan W. Paeth.
///     <see href="https://www.w3.org/TR/PNG-Filters.html" />
/// </summary>
internal static class PaethFilter
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

        // Paeth(x) + PaethPredictor(Raw(x-bpp), Prior(x), Prior(x-bpp))
        var offset = bytesPerPixel + 1; // Add one because x starts at one.
        var x = 1;
        for (; x < offset; x++)
        {
            ref var scan = ref Unsafe.Add(ref scanBaseRef, x);
            var above = Unsafe.Add(ref prevBaseRef, x);
            scan = (byte)(scan + above);
        }

        for (; x < scanline.Length; x++)
        {
            ref var scan = ref Unsafe.Add(ref scanBaseRef, x);
            var left = Unsafe.Add(ref scanBaseRef, x - bytesPerPixel);
            var above = Unsafe.Add(ref prevBaseRef, x);
            var upperLeft = Unsafe.Add(ref prevBaseRef, x - bytesPerPixel);
            scan = (byte)(scan + PaethPredictor(left, above, upperLeft));
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

        // Paeth(x) = Raw(x) - PaethPredictor(Raw(x-bpp), Prior(x), Prior(x - bpp))
        resultBaseRef = 4;

        var x = 0;
        for (; x < bytesPerPixel; /* Note: ++x happens in the body to avoid one add operation */)
        {
            var scan = Unsafe.Add(ref scanBaseRef, x);
            var above = Unsafe.Add(ref prevBaseRef, x);
            ++x;
            ref var res = ref Unsafe.Add(ref resultBaseRef, x);
            res = (byte)(scan - PaethPredictor(0, above, 0));
            sum += ImageMaths.FastAbs(unchecked((sbyte)res));
        }

        for (var xLeft = x - bytesPerPixel;
             x < scanline.Length;
             ++xLeft /* Note: ++x happens in the body to avoid one add operation */)
        {
            var scan = Unsafe.Add(ref scanBaseRef, x);
            var left = Unsafe.Add(ref scanBaseRef, xLeft);
            var above = Unsafe.Add(ref prevBaseRef, x);
            var upperLeft = Unsafe.Add(ref prevBaseRef, xLeft);
            ++x;
            ref var res = ref Unsafe.Add(ref resultBaseRef, x);
            res = (byte)(scan - PaethPredictor(left, above, upperLeft));
            sum += ImageMaths.FastAbs(unchecked((sbyte)res));
        }

        sum -= 4;
    }

    /// <summary>
    ///     Computes a simple linear function of the three neighboring pixels (left, above, upper left), then chooses
    ///     as predictor the neighboring pixel closest to the computed value.
    /// </summary>
    /// <param name="left">The left neighbor pixel.</param>
    /// <param name="above">The above neighbor pixel.</param>
    /// <param name="upperLeft">The upper left neighbor pixel.</param>
    /// <returns>
    ///     The <see cref="byte" />.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte PaethPredictor(byte left, byte above, byte upperLeft)
    {
        var p = left + above - upperLeft;
        var pa = ImageMaths.FastAbs(p - left);
        var pb = ImageMaths.FastAbs(p - above);
        var pc = ImageMaths.FastAbs(p - upperLeft);

        if (pa <= pb && pa <= pc) return left;

        if (pb <= pc) return above;

        return upperLeft;
    }
}