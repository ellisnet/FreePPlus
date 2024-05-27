// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     ICC Profile ID
/// </summary>
public readonly struct IccProfileId : IEquatable<IccProfileId>
{
    /// <summary>
    ///     A profile ID with all values set to zero
    /// </summary>
    public static readonly IccProfileId Zero = default;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccProfileId" /> struct.
    /// </summary>
    /// <param name="p1">Part 1 of the ID</param>
    /// <param name="p2">Part 2 of the ID</param>
    /// <param name="p3">Part 3 of the ID</param>
    /// <param name="p4">Part 4 of the ID</param>
    public IccProfileId(uint p1, uint p2, uint p3, uint p4)
    {
        Part1 = p1;
        Part2 = p2;
        Part3 = p3;
        Part4 = p4;
    }

    /// <summary>
    ///     Gets the first part of the ID.
    /// </summary>
    public uint Part1 { get; }

    /// <summary>
    ///     Gets the second part of the ID.
    /// </summary>
    public uint Part2 { get; }

    /// <summary>
    ///     Gets the third part of the ID.
    /// </summary>
    public uint Part3 { get; }

    /// <summary>
    ///     Gets the fourth part of the ID.
    /// </summary>
    public uint Part4 { get; }

    /// <summary>
    ///     Gets a value indicating whether the ID is set or just consists of zeros.
    /// </summary>
    public bool IsSet => !Equals(Zero);

    /// <summary>
    ///     Compares two <see cref="IccProfileId" /> objects for equality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="IccProfileId" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="IccProfileId" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    public static bool operator ==(IccProfileId left, IccProfileId right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="IccProfileId" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="IccProfileId" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="IccProfileId" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    public static bool operator !=(IccProfileId left, IccProfileId right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccProfileId other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(IccProfileId other)
    {
        return Part1 == other.Part1 &&
               Part2 == other.Part2 &&
               Part3 == other.Part3 &&
               Part4 == other.Part4;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Part1,
            Part2,
            Part3,
            Part4);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{ToHex(Part1)}-{ToHex(Part2)}-{ToHex(Part3)}-{ToHex(Part4)}";
    }

    private static string ToHex(uint value)
    {
        return value.ToString("X").PadLeft(8, '0');
    }
}