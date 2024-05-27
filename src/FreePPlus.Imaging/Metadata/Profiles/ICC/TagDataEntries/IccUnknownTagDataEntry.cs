// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     This tag stores data of an unknown tag data entry
/// </summary>
internal sealed class IccUnknownTagDataEntry : IccTagDataEntry, IEquatable<IccUnknownTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccUnknownTagDataEntry" /> class.
    /// </summary>
    /// <param name="data">The raw data of the entry</param>
    public IccUnknownTagDataEntry(byte[] data)
        : this(data, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccUnknownTagDataEntry" /> class.
    /// </summary>
    /// <param name="data">The raw data of the entry</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccUnknownTagDataEntry(byte[] data, IccProfileTag tagSignature)
        : base(IccTypeSignature.Unknown, tagSignature)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    /// <summary>
    ///     Gets the raw data of the entry.
    /// </summary>
    public byte[] Data { get; }

    /// <inheritdoc />
    public bool Equals(IccUnknownTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other) && Data.AsSpan().SequenceEqual(other.Data);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccUnknownTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccUnknownTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Signature, Data);
    }
}