// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Utility class for common geometric functions.
/// </summary>
public static class GeometryUtilities
{
    /// <summary>
    ///     Converts a degree (360-periodic) angle to a radian (2*Pi-periodic) angle.
    /// </summary>
    /// <param name="degree">The angle in degrees.</param>
    /// <returns>
    ///     The <see cref="float" /> representing the degree as radians.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DegreeToRadian(float degree)
    {
        return degree * (MathF.PI / 180F);
    }

    /// <summary>
    ///     Converts a radian (2*Pi-periodic) angle to a degree (360-periodic) angle.
    /// </summary>
    /// <param name="radian">The angle in radians.</param>
    /// <returns>
    ///     The <see cref="float" /> representing the degree as radians.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RadianToDegree(float radian)
    {
        return radian / (MathF.PI / 180F);
    }
}