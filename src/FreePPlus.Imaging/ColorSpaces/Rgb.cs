// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.ColorSpaces.Conversion;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.ColorSpaces;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces;

/// <summary>
///     Represents an RGB color with specified <see cref="RgbWorkingSpace" /> working space.
/// </summary>
public readonly struct Rgb : IEquatable<Rgb>
{
    /// <summary>
    ///     The default rgb working space.
    /// </summary>
    public static readonly RgbWorkingSpace DefaultWorkingSpace = RgbWorkingSpaces.SRgb;

    private static readonly Vector3 Min = Vector3.Zero;
    private static readonly Vector3 Max = Vector3.One;

    /// <summary>
    ///     Gets the red component.
    ///     <remarks>A value usually ranging between 0 and 1.</remarks>
    /// </summary>
    public readonly float R;

    /// <summary>
    ///     Gets the green component.
    ///     <remarks>A value usually ranging between 0 and 1.</remarks>
    /// </summary>
    public readonly float G;

    /// <summary>
    ///     Gets the blue component.
    ///     <remarks>A value usually ranging between 0 and 1.</remarks>
    /// </summary>
    public readonly float B;

    /// <summary>
    ///     Gets the Rgb color space <seealso cref="RgbWorkingSpaces" />
    /// </summary>
    public readonly RgbWorkingSpace WorkingSpace;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgb" /> struct.
    /// </summary>
    /// <param name="r">The red component ranging between 0 and 1.</param>
    /// <param name="g">The green component ranging between 0 and 1.</param>
    /// <param name="b">The blue component ranging between 0 and 1.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgb(float r, float g, float b)
        : this(r, g, b, DefaultWorkingSpace) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgb" /> struct.
    /// </summary>
    /// <param name="r">The red component ranging between 0 and 1.</param>
    /// <param name="g">The green component ranging between 0 and 1.</param>
    /// <param name="b">The blue component ranging between 0 and 1.</param>
    /// <param name="workingSpace">The rgb working space.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgb(float r, float g, float b, RgbWorkingSpace workingSpace)
        : this(new Vector3(r, g, b), workingSpace) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgb" /> struct.
    /// </summary>
    /// <param name="vector">The vector representing the r, g, b components.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgb(Vector3 vector)
        : this(vector, DefaultWorkingSpace) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgb" /> struct.
    /// </summary>
    /// <param name="vector">The vector representing the r, g, b components.</param>
    /// <param name="workingSpace">The rgb working space.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgb(Vector3 vector, RgbWorkingSpace workingSpace)
    {
        vector = Vector3.Clamp(vector, Min, Max);
        R = vector.X;
        G = vector.Y;
        B = vector.Z;
        WorkingSpace = workingSpace;
    }

    /// <summary>
    ///     Allows the implicit conversion of an instance of <see cref="Rgb24" /> to a
    ///     <see cref="Rgb" />.
    /// </summary>
    /// <param name="color">The instance of <see cref="Rgba32" /> to convert.</param>
    /// <returns>An instance of <see cref="Rgb" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Rgb(Rgb24 color)
    {
        return new Rgb(color.R / 255F, color.G / 255F, color.B / 255F);
    }

    /// <summary>
    ///     Allows the implicit conversion of an instance of <see cref="Rgba32" /> to a
    ///     <see cref="Rgb" />.
    /// </summary>
    /// <param name="color">The instance of <see cref="Rgba32" /> to convert.</param>
    /// <returns>An instance of <see cref="Rgb" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Rgb(Rgba32 color)
    {
        return new Rgb(color.R / 255F, color.G / 255F, color.B / 255F);
    }

    /// <summary>
    ///     Compares two <see cref="Rgb" /> objects for equality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="Rgb" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="Rgb" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the current left is equal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(Rgb left, Rgb right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="Rgb" /> objects for inequality.
    /// </summary>
    /// <param name="left">The <see cref="Rgb" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rgb" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is unequal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(Rgb left, Rgb right)
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
        return new Vector3(R, G, B);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return FormattableString.Invariant($"Rgb({R:#0.##}, {G:#0.##}, {B:#0.##})");
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is Rgb other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool Equals(Rgb other)
    {
        return R.Equals(other.R)
               && G.Equals(other.G)
               && B.Equals(other.B);
    }
}