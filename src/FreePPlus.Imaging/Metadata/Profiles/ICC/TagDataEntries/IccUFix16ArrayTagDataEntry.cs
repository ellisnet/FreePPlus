// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     This type represents an array of doubles (from 32bit values).
/// </summary>
internal sealed class IccUFix16ArrayTagDataEntry : IccTagDataEntry, IEquatable<IccUFix16ArrayTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccUFix16ArrayTagDataEntry" /> class.
    /// </summary>
    /// <param name="data">The array data</param>
    public IccUFix16ArrayTagDataEntry(float[] data)
        : this(data, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccUFix16ArrayTagDataEntry" /> class.
    /// </summary>
    /// <param name="data">The array data</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccUFix16ArrayTagDataEntry(float[] data, IccProfileTag tagSignature)
        : base(IccTypeSignature.U16Fixed16Array, tagSignature)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    /// <summary>
    ///     Gets the array data.
    /// </summary>
    public float[] Data { get; }

    /// <inheritdoc />
    public bool Equals(IccUFix16ArrayTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other) && Data.AsSpan().SequenceEqual(other.Data);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccUFix16ArrayTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccUFix16ArrayTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Signature, Data);
    }
}