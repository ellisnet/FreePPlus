// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Extension methods for classes that implement <see cref="IComparable{T}" />.
/// </summary>
internal static class ComparableExtensions
{
    /// <summary>
    ///     Restricts a <see cref="byte" /> to be within a specified range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
    /// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
    /// <returns>
    ///     The <see cref="byte" /> representing the clamped value.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static byte Clamp(this byte value, byte min, byte max)
    {
        // Order is important here as someone might set min to higher than max.
        if (value >= max) return max;

        if (value <= min) return min;

        return value;
    }

    /// <summary>
    ///     Restricts a <see cref="uint" /> to be within a specified range.
    /// </summary>
    /// <param name="value">The The value to clamp.</param>
    /// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
    /// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
    /// <returns>
    ///     The <see cref="int" /> representing the clamped value.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static uint Clamp(this uint value, uint min, uint max)
    {
        if (value >= max) return max;

        if (value <= min) return min;

        return value;
    }

    /// <summary>
    ///     Restricts a <see cref="int" /> to be within a specified range.
    /// </summary>
    /// <param name="value">The The value to clamp.</param>
    /// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
    /// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
    /// <returns>
    ///     The <see cref="int" /> representing the clamped value.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static int Clamp(this int value, int min, int max)
    {
        if (value >= max) return max;

        if (value <= min) return min;

        return value;
    }

    /// <summary>
    ///     Restricts a <see cref="float" /> to be within a specified range.
    /// </summary>
    /// <param name="value">The The value to clamp.</param>
    /// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
    /// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
    /// <returns>
    ///     The <see cref="float" /> representing the clamped value.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static float Clamp(this float value, float min, float max)
    {
        if (value >= max) return max;

        if (value <= min) return min;

        return value;
    }

    /// <summary>
    ///     Restricts a <see cref="double" /> to be within a specified range.
    /// </summary>
    /// <param name="value">The The value to clamp.</param>
    /// <param name="min">The minimum value. If value is less than min, min will be returned.</param>
    /// <param name="max">The maximum value. If value is greater than max, max will be returned.</param>
    /// <returns>
    ///     The <see cref="double" /> representing the clamped value.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static double Clamp(this double value, double min, double max)
    {
        if (value >= max) return max;

        if (value <= min) return min;

        return value;
    }
}