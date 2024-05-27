// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     This type is a representation of the time and date.
/// </summary>
internal sealed class IccDateTimeTagDataEntry : IccTagDataEntry, IEquatable<IccDateTimeTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccDateTimeTagDataEntry" /> class.
    /// </summary>
    /// <param name="value">The DateTime value</param>
    public IccDateTimeTagDataEntry(DateTime value)
        : this(value, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccDateTimeTagDataEntry" /> class.
    /// </summary>
    /// <param name="value">The DateTime value</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccDateTimeTagDataEntry(DateTime value, IccProfileTag tagSignature)
        : base(IccTypeSignature.DateTime, tagSignature)
    {
        Value = value;
    }

    /// <summary>
    ///     Gets the date and time value
    /// </summary>
    public DateTime Value { get; }

    /// <inheritdoc />
    public bool Equals(IccDateTimeTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other) && Value.Equals(other.Value);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccDateTimeTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccDateTimeTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Signature, Value);
    }
}