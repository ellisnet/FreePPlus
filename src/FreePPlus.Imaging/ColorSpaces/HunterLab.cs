﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.ColorSpaces;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces;

/// <summary>
///     Represents a Hunter LAB color.
///     <see href="https://en.wikipedia.org/wiki/Lab_color_space" />.
/// </summary>
public readonly struct HunterLab : IEquatable<HunterLab>
{
    /// <summary>
    ///     D50 standard illuminant.
    ///     Used when reference white is not specified explicitly.
    /// </summary>
    public static readonly CieXyz DefaultWhitePoint = Illuminants.C;

    /// <summary>
    ///     Gets the lightness dimension.
    ///     <remarks>A value usually ranging between 0 (black), 100 (diffuse white) or higher (specular white).</remarks>
    /// </summary>
    public readonly float L;

    /// <summary>
    ///     Gets the 'a' color component.
    ///     <remarks>A value usually ranging from -100 to 100. Negative is green, positive magenta.</remarks>
    /// </summary>
    public readonly float A;

    /// <summary>
    ///     Gets the 'b' color component.
    ///     <remarks>A value usually ranging from -100 to 100. Negative is blue, positive is yellow</remarks>
    /// </summary>
    public readonly float B;

    /// <summary>
    ///     Gets the reference white point of this color.
    /// </summary>
    public readonly CieXyz WhitePoint;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HunterLab" /> struct.
    /// </summary>
    /// <param name="l">The lightness dimension.</param>
    /// <param name="a">The a (green - magenta) component.</param>
    /// <param name="b">The b (blue - yellow) component.</param>
    /// <remarks>Uses <see cref="DefaultWhitePoint" /> as white point.</remarks>
    [MethodImpl(InliningOptions.ShortMethod)]
    public HunterLab(float l, float a, float b)
        : this(new Vector3(l, a, b), DefaultWhitePoint) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HunterLab" /> struct.
    /// </summary>
    /// <param name="l">The lightness dimension.</param>
    /// <param name="a">The a (green - magenta) component.</param>
    /// <param name="b">The b (blue - yellow) component.</param>
    /// <param name="whitePoint">The reference white point. <see cref="Illuminants" /></param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public HunterLab(float l, float a, float b, CieXyz whitePoint)
        : this(new Vector3(l, a, b), whitePoint) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HunterLab" /> struct.
    /// </summary>
    /// <param name="vector">The vector representing the l, a, b components.</param>
    /// <remarks>Uses <see cref="DefaultWhitePoint" /> as white point.</remarks>
    [MethodImpl(InliningOptions.ShortMethod)]
    public HunterLab(Vector3 vector)
        : this(vector, DefaultWhitePoint) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HunterLab" /> struct.
    /// </summary>
    /// <param name="vector">The vector representing the l a b components.</param>
    /// <param name="whitePoint">The reference white point. <see cref="Illuminants" /></param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public HunterLab(Vector3 vector, CieXyz whitePoint)
    {
        // Not clamping as documentation about this space only indicates "usual" ranges
        L = vector.X;
        A = vector.Y;
        B = vector.Z;
        WhitePoint = whitePoint;
    }

    /// <summary>
    ///     Compares two <see cref="HunterLab" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="HunterLab" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="HunterLab" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is equal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    public static bool operator ==(HunterLab left, HunterLab right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="HunterLab" /> objects for inequality
    /// </summary>
    /// <param name="left">The <see cref="HunterLab" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="HunterLab" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is unequal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(HunterLab left, HunterLab right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public override int GetHashCode()
    {
        return HashCode.Combine(L, A, B, WhitePoint);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return FormattableString.Invariant($"HunterLab({L:#0.##}, {A:#0.##}, {B:#0.##})");
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is HunterLab other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool Equals(HunterLab other)
    {
        return L.Equals(other.L)
               && A.Equals(other.A)
               && B.Equals(other.B)
               && WhitePoint.Equals(other.WhitePoint);
    }
}