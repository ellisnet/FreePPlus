// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     Represents the chromaticity coordinates of RGB primaries.
///     One of the specifiers of <see cref="RgbWorkingSpace" />.
/// </summary>
public readonly struct RgbPrimariesChromaticityCoordinates : IEquatable<RgbPrimariesChromaticityCoordinates>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RgbPrimariesChromaticityCoordinates" /> struct.
    /// </summary>
    /// <param name="r">The chromaticity coordinates of the red channel.</param>
    /// <param name="g">The chromaticity coordinates of the green channel.</param>
    /// <param name="b">The chromaticity coordinates of the blue channel.</param>
    public RgbPrimariesChromaticityCoordinates(CieXyChromaticityCoordinates r, CieXyChromaticityCoordinates g,
        CieXyChromaticityCoordinates b)
    {
        R = r;
        G = g;
        B = b;
    }

    /// <summary>
    ///     Gets the chromaticity coordinates of the red channel.
    /// </summary>
    public CieXyChromaticityCoordinates R { get; }

    /// <summary>
    ///     Gets the chromaticity coordinates of the green channel.
    /// </summary>
    public CieXyChromaticityCoordinates G { get; }

    /// <summary>
    ///     Gets the chromaticity coordinates of the blue channel.
    /// </summary>
    public CieXyChromaticityCoordinates B { get; }

    /// <summary>
    ///     Compares two <see cref="RgbPrimariesChromaticityCoordinates" /> objects for equality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="RgbPrimariesChromaticityCoordinates" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="RgbPrimariesChromaticityCoordinates" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the current left is equal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    public static bool operator ==(RgbPrimariesChromaticityCoordinates left, RgbPrimariesChromaticityCoordinates right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="RgbPrimariesChromaticityCoordinates" /> objects for inequality
    /// </summary>
    /// <param name="left">
    ///     The <see cref="RgbPrimariesChromaticityCoordinates" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="RgbPrimariesChromaticityCoordinates" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the current left is unequal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    public static bool operator !=(RgbPrimariesChromaticityCoordinates left, RgbPrimariesChromaticityCoordinates right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is RgbPrimariesChromaticityCoordinates other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(RgbPrimariesChromaticityCoordinates other)
    {
        return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B);
    }
}