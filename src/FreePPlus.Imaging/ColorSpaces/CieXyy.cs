// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.ColorSpaces;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces;

/// <summary>
///     Represents an CIE xyY 1931 color
///     <see
///         href="https://en.wikipedia.org/wiki/CIE_1931_color_space#CIE_xy_chromaticity_diagram_and_the_CIE_xyY_color_space" />
/// </summary>
public readonly struct CieXyy : IEquatable<CieXyy>
{
    /// <summary>
    ///     Gets the X chrominance component.
    ///     <remarks>A value usually ranging between 0 and 1.</remarks>
    /// </summary>
    public readonly float X;

    /// <summary>
    ///     Gets the Y chrominance component.
    ///     <remarks>A value usually ranging between 0 and 1.</remarks>
    /// </summary>
    public readonly float Y;

    /// <summary>
    ///     Gets the Y luminance component.
    ///     <remarks>A value usually ranging between 0 and 1.</remarks>
    /// </summary>
    public readonly float Yl;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CieXyy" /> struct.
    /// </summary>
    /// <param name="x">The x chroma component.</param>
    /// <param name="y">The y chroma component.</param>
    /// <param name="yl">The y luminance component.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public CieXyy(float x, float y, float yl)
    {
        // Not clamping as documentation about this space only indicates "usual" ranges
        X = x;
        Y = y;
        Yl = yl;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CieXyy" /> struct.
    /// </summary>
    /// <param name="vector">The vector representing the x, y, Y components.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public CieXyy(Vector3 vector)
        : this()
    {
        // Not clamping as documentation about this space only indicates "usual" ranges
        X = vector.X;
        Y = vector.Y;
        Yl = vector.Z;
    }

    /// <summary>
    ///     Compares two <see cref="CieXyy" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="CieXyy" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="CieXyy" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is equal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(CieXyy left, CieXyy right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="CieXyy" /> objects for inequality.
    /// </summary>
    /// <param name="left">The <see cref="CieXyy" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="CieXyy" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is unequal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(CieXyy left, CieXyy right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Yl);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return FormattableString.Invariant($"CieXyy({X:#0.##}, {Y:#0.##}, {Yl:#0.##})");
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is CieXyy other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool Equals(CieXyy other)
    {
        return X.Equals(other.X)
               && Y.Equals(other.Y)
               && Yl.Equals(other.Yl);
    }
}