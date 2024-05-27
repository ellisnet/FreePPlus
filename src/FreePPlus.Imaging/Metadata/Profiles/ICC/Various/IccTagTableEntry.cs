// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Entry of ICC tag table
/// </summary>
internal readonly struct IccTagTableEntry : IEquatable<IccTagTableEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccTagTableEntry" /> struct.
    /// </summary>
    /// <param name="signature">Signature of the tag</param>
    /// <param name="offset">Offset of entry in bytes</param>
    /// <param name="dataSize">Size of entry in bytes</param>
    public IccTagTableEntry(IccProfileTag signature, uint offset, uint dataSize)
    {
        Signature = signature;
        Offset = offset;
        DataSize = dataSize;
    }

    /// <summary>
    ///     Gets the signature of the tag.
    /// </summary>
    public IccProfileTag Signature { get; }

    /// <summary>
    ///     Gets the offset of entry in bytes.
    /// </summary>
    public uint Offset { get; }

    /// <summary>
    ///     Gets the size of entry in bytes.
    /// </summary>
    public uint DataSize { get; }

    /// <summary>
    ///     Compares two <see cref="IccTagTableEntry" /> objects for equality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="IccTagTableEntry" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="IccTagTableEntry" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    public static bool operator ==(IccTagTableEntry left, IccTagTableEntry right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="IccTagTableEntry" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="IccTagTableEntry" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="IccTagTableEntry" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    public static bool operator !=(IccTagTableEntry left, IccTagTableEntry right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccTagTableEntry other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(IccTagTableEntry other)
    {
        return Signature.Equals(other.Signature) &&
               Offset.Equals(other.Offset) &&
               DataSize.Equals(other.DataSize);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Signature, Offset, DataSize);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Signature} (Offset: {Offset}; Size: {DataSize})";
    }
}