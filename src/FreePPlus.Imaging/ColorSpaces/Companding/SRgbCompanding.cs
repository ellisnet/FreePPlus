﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.ColorSpaces.Companding;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Companding;

/// <summary>
///     Implements sRGB companding.
/// </summary>
/// <remarks>
///     For more info see:
///     <see href="http://www.brucelindbloom.com/index.html?Eqn_RGB_to_XYZ.html" />
///     <see href="http://www.brucelindbloom.com/index.html?Eqn_XYZ_to_RGB.html" />
/// </remarks>
public static class SRgbCompanding
{
    /// <summary>
    ///     Expands the companded vectors to their linear equivalents with respect to the energy.
    /// </summary>
    /// <param name="vectors">The span of vectors.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void Expand(Span<Vector4> vectors)
    {
        ref var baseRef = ref MemoryMarshal.GetReference(vectors);

        for (var i = 0; i < vectors.Length; i++)
        {
            ref var v = ref Unsafe.Add(ref baseRef, i);
            Expand(ref v);
        }
    }

    /// <summary>
    ///     Compresses the uncompanded vectors to their nonlinear equivalents with respect to the energy.
    /// </summary>
    /// <param name="vectors">The span of vectors.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void Compress(Span<Vector4> vectors)
    {
        ref var baseRef = ref MemoryMarshal.GetReference(vectors);

        for (var i = 0; i < vectors.Length; i++)
        {
            ref var v = ref Unsafe.Add(ref baseRef, i);
            Compress(ref v);
        }
    }

    /// <summary>
    ///     Expands a companded vector to its linear equivalent with respect to the energy.
    /// </summary>
    /// <param name="vector">The vector.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void Expand(ref Vector4 vector)
    {
        vector.X = Expand(vector.X);
        vector.Y = Expand(vector.Y);
        vector.Z = Expand(vector.Z);
    }

    /// <summary>
    ///     Compresses an uncompanded vector (linear) to its nonlinear equivalent.
    /// </summary>
    /// <param name="vector">The vector.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void Compress(ref Vector4 vector)
    {
        vector.X = Compress(vector.X);
        vector.Y = Compress(vector.Y);
        vector.Z = Compress(vector.Z);
    }

    /// <summary>
    ///     Expands a companded channel to its linear equivalent with respect to the energy.
    /// </summary>
    /// <param name="channel">The channel value.</param>
    /// <returns>The <see cref="float" /> representing the linear channel value.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static float Expand(float channel)
    {
        return channel <= 0.04045F ? channel / 12.92F : MathF.Pow((channel + 0.055F) / 1.055F, 2.4F);
    }

    /// <summary>
    ///     Compresses an uncompanded channel (linear) to its nonlinear equivalent.
    /// </summary>
    /// <param name="channel">The channel value.</param>
    /// <returns>The <see cref="float" /> representing the nonlinear channel value.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static float Compress(float channel)
    {
        return channel <= 0.0031308F ? 12.92F * channel : 1.055F * MathF.Pow(channel, 0.416666666666667F) - 0.055F;
    }
}