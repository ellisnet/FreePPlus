// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.ColorSpaces;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces;

/// <summary>
///     Represents an CIE XYZ 1931 color
///     <see href="https://en.wikipedia.org/wiki/CIE_1931_color_space#Definition_of_the_CIE_XYZ_color_space" />
/// </summary>
public readonly struct CieXyz : IEquatable<CieXyz>
{
    /// <summary>
    ///     Gets the X component. A mix (a linear combination) of cone response curves chosen to be nonnegative.
    ///     <remarks>A value usually ranging between 0 and 1.</remarks>
    /// </summary>
    public readonly float X;

    /// <summary>
    ///     Gets the Y luminance component.
    ///     <remarks>A value usually ranging between 0 and 1.</remarks>
    /// </summary>
    public readonly float Y;

    /// <summary>
    ///     Gets the Z component. Quasi-equal to blue stimulation, or the S cone response.
    ///     <remarks>A value usually ranging between 0 and 1.</remarks>
    /// </summary>
    public readonly float Z;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CieXyz" /> struct.
    /// </summary>
    /// <param name="x">X is a mix (a linear combination) of cone response curves chosen to be nonnegative</param>
    /// <param name="y">The y luminance component.</param>
    /// <param name="z">Z is quasi-equal to blue stimulation, or the S cone of the human eye.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public CieXyz(float x, float y, float z)
        : this(new Vector3(x, y, z)) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CieXyz" /> struct.
    /// </summary>
    /// <param name="vector">The vector representing the x, y, z components.</param>
    public CieXyz(Vector3 vector)
        : this()
    {
        // Not clamping as documentation about this space only indicates "usual" ranges
        X = vector.X;
        Y = vector.Y;
        Z = vector.Z;
    }

    /// <summary>
    ///     Compares two <see cref="CieXyz" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="CieXyz" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="CieXyz" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is equal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(CieXyz left, CieXyz right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="CieXyz" /> objects for inequality.
    /// </summary>
    /// <param name="left">The <see cref="CieXyz" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="CieXyz" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is unequal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(CieXyz left, CieXyz right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    ///     Returns a new <see cref="Vector3" /> representing this instance.
    /// </summary>
    /// <returns>The <see cref="Vector3" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Vector3 ToVector3()
    {
        return new Vector3(X, Y, Z);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return FormattableString.Invariant($"CieXyz({X:#0.##}, {Y:#0.##}, {Z:#0.##})");
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is CieXyz other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool Equals(CieXyz other)
    {
        return X.Equals(other.X)
               && Y.Equals(other.Y)
               && Z.Equals(other.Z);
    }
}