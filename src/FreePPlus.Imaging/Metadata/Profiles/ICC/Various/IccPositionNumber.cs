﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Position of an object within an ICC profile
/// </summary>
internal readonly struct IccPositionNumber : IEquatable<IccPositionNumber>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccPositionNumber" /> struct.
    /// </summary>
    /// <param name="offset">Offset in bytes</param>
    /// <param name="size">Size in bytes</param>
    public IccPositionNumber(uint offset, uint size)
    {
        Offset = offset;
        Size = size;
    }

    /// <summary>
    ///     Gets the offset in bytes
    /// </summary>
    public uint Offset { get; }

    /// <summary>
    ///     Gets the size in bytes
    /// </summary>
    public uint Size { get; }

    /// <summary>
    ///     Compares two <see cref="IccPositionNumber" /> objects for equality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="IccPositionNumber" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="IccPositionNumber" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    public static bool operator ==(IccPositionNumber left, IccPositionNumber right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="IccPositionNumber" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="IccPositionNumber" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="IccPositionNumber" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    public static bool operator !=(IccPositionNumber left, IccPositionNumber right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccPositionNumber other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(IccPositionNumber other)
    {
        return Offset == other.Offset &&
               Size == other.Size;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return unchecked((int)(Offset ^ Size));
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Offset}; {Size}";
    }
}