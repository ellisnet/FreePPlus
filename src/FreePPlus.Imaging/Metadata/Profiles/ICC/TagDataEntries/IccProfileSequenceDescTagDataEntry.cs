﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     This type is an array of structures, each of which contains information
///     from the header fields and tags from the original profiles which were
///     combined to create the final profile.
/// </summary>
internal sealed class IccProfileSequenceDescTagDataEntry : IccTagDataEntry,
    IEquatable<IccProfileSequenceDescTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccProfileSequenceDescTagDataEntry" /> class.
    /// </summary>
    /// <param name="descriptions">Profile Descriptions</param>
    public IccProfileSequenceDescTagDataEntry(IccProfileDescription[] descriptions)
        : this(descriptions, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccProfileSequenceDescTagDataEntry" /> class.
    /// </summary>
    /// <param name="descriptions">Profile Descriptions</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccProfileSequenceDescTagDataEntry(IccProfileDescription[] descriptions, IccProfileTag tagSignature)
        : base(IccTypeSignature.ProfileSequenceDesc, tagSignature)
    {
        Descriptions = descriptions ?? throw new ArgumentNullException(nameof(descriptions));
    }

    /// <summary>
    ///     Gets the profile descriptions
    /// </summary>
    public IccProfileDescription[] Descriptions { get; }

    /// <inheritdoc />
    public bool Equals(IccProfileSequenceDescTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other) && Descriptions.AsSpan().SequenceEqual(other.Descriptions);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccProfileSequenceDescTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccProfileSequenceDescTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Signature, Descriptions);
    }
}