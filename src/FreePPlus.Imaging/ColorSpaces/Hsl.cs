// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.ColorSpaces;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces;

/// <summary>
///     Represents a Hsl (hue, saturation, lightness) color.
/// </summary>
public readonly struct Hsl : IEquatable<Hsl>
{
    private static readonly Vector3 Min = Vector3.Zero;
    private static readonly Vector3 Max = new(360, 1, 1);

    /// <summary>
    ///     Gets the hue component.
    ///     <remarks>A value ranging between 0 and 360.</remarks>
    /// </summary>
    public readonly float H;

    /// <summary>
    ///     Gets the saturation component.
    ///     <remarks>A value ranging between 0 and 1.</remarks>
    /// </summary>
    public readonly float S;

    /// <summary>
    ///     Gets the lightness component.
    ///     <remarks>A value ranging between 0 and 1.</remarks>
    /// </summary>
    public readonly float L;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Hsl" /> struct.
    /// </summary>
    /// <param name="h">The h hue component.</param>
    /// <param name="s">The s saturation component.</param>
    /// <param name="l">The l value (lightness) component.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Hsl(float h, float s, float l)
        : this(new Vector3(h, s, l)) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Hsl" /> struct.
    /// </summary>
    /// <param name="vector">The vector representing the h, s, l components.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Hsl(Vector3 vector)
    {
        vector = Vector3.Clamp(vector, Min, Max);
        H = vector.X;
        S = vector.Y;
        L = vector.Z;
    }

    /// <summary>
    ///     Compares two <see cref="Hsl" /> objects for equality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="Hsl" /> on the left side of the operand.
    /// </param>
    /// <param name="right">The <see cref="Hsl" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is equal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(Hsl left, Hsl right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="Hsl" /> objects for inequality.
    /// </summary>
    /// <param name="left">The <see cref="Hsl" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Hsl" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is unequal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(Hsl left, Hsl right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public override int GetHashCode()
    {
        return HashCode.Combine(H, S, L);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return FormattableString.Invariant($"Hsl({H:#0.##}, {S:#0.##}, {L:#0.##})");
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is Hsl other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool Equals(Hsl other)
    {
        return H.Equals(other.H)
               && S.Equals(other.S)
               && L.Equals(other.L);
    }
}