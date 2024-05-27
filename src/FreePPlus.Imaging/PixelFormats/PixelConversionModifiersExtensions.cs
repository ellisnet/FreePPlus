﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Extension and utility methods for <see cref="PixelConversionModifiers" />.
/// </summary>
internal static class PixelConversionModifiersExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined(this PixelConversionModifiers modifiers, PixelConversionModifiers expected)
    {
        return (modifiers & expected) == expected;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PixelConversionModifiers Remove(
        this PixelConversionModifiers modifiers,
        PixelConversionModifiers removeThis)
    {
        return modifiers & ~removeThis;
    }

    /// <summary>
    ///     Applies the union of <see cref="PixelConversionModifiers.Scale" /> and
    ///     <see cref="PixelConversionModifiers.SRgbCompand" />,
    ///     if <paramref name="compand" /> is true, returns unmodified <paramref name="originalModifiers" /> otherwise.
    /// </summary>
    /// <remarks>
    ///     <see cref="PixelConversionModifiers.Scale" /> and <see cref="PixelConversionModifiers.SRgbCompand" />
    ///     should be always used together!
    /// </remarks>
    public static PixelConversionModifiers ApplyCompanding(
        this PixelConversionModifiers originalModifiers,
        bool compand)
    {
        return compand
            ? originalModifiers | PixelConversionModifiers.Scale | PixelConversionModifiers.SRgbCompand
            : originalModifiers;
    }
}