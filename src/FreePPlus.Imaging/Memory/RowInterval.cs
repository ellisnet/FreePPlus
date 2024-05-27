// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Memory;

//was previously: namespace SixLabors.ImageSharp.Memory;

/// <summary>
///     Represents an interval of rows in a <see cref="Rectangle" /> and/or <see cref="Buffer2D{T}" />
/// </summary>
/// <remarks>
///     Before RC1, this class might be target of API changes, use it on your own risk!
/// </remarks>
public readonly struct RowInterval : IEquatable<RowInterval>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RowInterval" /> struct.
    /// </summary>
    /// <param name="min">The inclusive minimum row.</param>
    /// <param name="max">The exclusive maximum row.</param>
    public RowInterval(int min, int max)
    {
        Guard.MustBeLessThan(min, max, nameof(min));

        Min = min;
        Max = max;
    }

    /// <summary>
    ///     Gets the inclusive minimum row.
    /// </summary>
    public int Min { get; }

    /// <summary>
    ///     Gets the exclusive maximum row.
    /// </summary>
    public int Max { get; }

    /// <summary>
    ///     Gets the difference (<see cref="Max" /> - <see cref="Min" />).
    /// </summary>
    public int Height => Max - Min;

    /// <summary>
    ///     Returns a boolean indicating whether the given two <see cref="RowInterval" />-s are equal.
    /// </summary>
    /// <param name="left">The first <see cref="RowInterval" /> to compare.</param>
    /// <param name="right">The second <see cref="RowInterval" /> to compare.</param>
    /// <returns>True if the given <see cref="RowInterval" />-s are equal; False otherwise.</returns>
    public static bool operator ==(RowInterval left, RowInterval right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Returns a boolean indicating whether the given two <see cref="RowInterval" />-s are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="RowInterval" /> to compare.</param>
    /// <param name="right">The second <see cref="RowInterval" /> to compare.</param>
    /// <returns>True if the given <see cref="RowInterval" />-s are not equal; False otherwise.</returns>
    public static bool operator !=(RowInterval left, RowInterval right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public bool Equals(RowInterval other)
    {
        return Min == other.Min && Max == other.Max;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return !ReferenceEquals(null, obj) && obj is RowInterval other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"RowInterval [{Min}->{Max}]";
    }

    internal RowInterval Slice(int start)
    {
        return new RowInterval(Min + start, Max);
    }

    internal RowInterval Slice(int start, int length)
    {
        return new RowInterval(Min + start, Min + start + length);
    }
}