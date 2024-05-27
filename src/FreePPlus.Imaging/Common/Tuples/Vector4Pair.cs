// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.Tuples;

//was previously: namespace SixLabors.ImageSharp.Tuples;

/// <summary>
///     Its faster to process multiple Vector4-s together, so let's pair them!
///     On AVX2 this pair should be convertible to <see cref="Vector{T}" /> of <see cref="float" />!
///     TODO: Investigate defining this as union with an Octet.OfSingle type.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct Vector4Pair
{
    public Vector4 A;

    public Vector4 B;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MultiplyInplace(float value)
    {
        A *= value;
        B *= value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddInplace(Vector4 value)
    {
        A += value;
        B += value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddInplace(ref Vector4Pair other)
    {
        A += other.A;
        B += other.B;
    }

    /// <summary>
    ///     .
    ///     Downscale method, specific to Jpeg color conversion. Works only if Vector{float}.Count == 4!        /// TODO: Move
    ///     it somewhere else.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void RoundAndDownscalePreVector8(float downscaleFactor)
    {
        ref var a = ref Unsafe.As<Vector4, Vector<float>>(ref A);
        a = a.FastRound();

        ref var b = ref Unsafe.As<Vector4, Vector<float>>(ref B);
        b = b.FastRound();

        // Downscale by 1/factor
        var scale = new Vector4(1 / downscaleFactor);
        A *= scale;
        B *= scale;
    }

    /// <summary>
    ///     AVX2-only Downscale method, specific to Jpeg color conversion.
    ///     TODO: Move it somewhere else.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void RoundAndDownscaleVector8(float downscaleFactor)
    {
        ref var self = ref Unsafe.As<Vector4Pair, Vector<float>>(ref this);
        var v = self;
        v = v.FastRound();

        // Downscale by 1/factor
        v *= new Vector<float>(1 / downscaleFactor);
        self = v;
    }

    public override string ToString()
    {
        return $"{nameof(Vector4Pair)}({A}, {B})";
    }
}