// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Entry of ICC colorant table
/// </summary>
internal readonly struct IccColorantTableEntry : IEquatable<IccColorantTableEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccColorantTableEntry" /> struct.
    /// </summary>
    /// <param name="name">Name of the colorant</param>
    public IccColorantTableEntry(string name)
        : this(name, 0, 0, 0) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccColorantTableEntry" /> struct.
    /// </summary>
    /// <param name="name">Name of the colorant</param>
    /// <param name="pcs1">First PCS value</param>
    /// <param name="pcs2">Second PCS value</param>
    /// <param name="pcs3">Third PCS value</param>
    public IccColorantTableEntry(string name, ushort pcs1, ushort pcs2, ushort pcs3)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Pcs1 = pcs1;
        Pcs2 = pcs2;
        Pcs3 = pcs3;
    }

    /// <summary>
    ///     Gets the colorant name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the first PCS value.
    /// </summary>
    public ushort Pcs1 { get; }

    /// <summary>
    ///     Gets the second PCS value.
    /// </summary>
    public ushort Pcs2 { get; }

    /// <summary>
    ///     Gets the third PCS value.
    /// </summary>
    public ushort Pcs3 { get; }

    /// <summary>
    ///     Compares two <see cref="IccColorantTableEntry" /> objects for equality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="IccColorantTableEntry" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="IccColorantTableEntry" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    public static bool operator ==(IccColorantTableEntry left, IccColorantTableEntry right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="IccColorantTableEntry" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="IccColorantTableEntry" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="IccColorantTableEntry" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    public static bool operator !=(IccColorantTableEntry left, IccColorantTableEntry right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccColorantTableEntry other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(IccColorantTableEntry other)
    {
        return Name == other.Name
               && Pcs1 == other.Pcs1
               && Pcs2 == other.Pcs2
               && Pcs3 == other.Pcs3;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Name,
            Pcs1,
            Pcs2,
            Pcs3);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Name}: {Pcs1}; {Pcs2}; {Pcs3}";
    }
}