﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     This type represents an array of unsigned 64bit integers.
/// </summary>
internal sealed class IccUInt64ArrayTagDataEntry : IccTagDataEntry, IEquatable<IccUInt64ArrayTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccUInt64ArrayTagDataEntry" /> class.
    /// </summary>
    /// <param name="data">The array data</param>
    public IccUInt64ArrayTagDataEntry(ulong[] data)
        : this(data, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccUInt64ArrayTagDataEntry" /> class.
    /// </summary>
    /// <param name="data">The array data</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccUInt64ArrayTagDataEntry(ulong[] data, IccProfileTag tagSignature)
        : base(IccTypeSignature.UInt64Array, tagSignature)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    /// <summary>
    ///     Gets the array data
    /// </summary>
    public ulong[] Data { get; }

    /// <inheritdoc />
    public bool Equals(IccUInt64ArrayTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other) && Data.AsSpan().SequenceEqual(other.Data);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccUInt64ArrayTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccUInt64ArrayTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Signature, Data);
    }
}