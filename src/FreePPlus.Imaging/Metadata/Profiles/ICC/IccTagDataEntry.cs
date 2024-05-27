﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     The data of an ICC tag entry
/// </summary>
public abstract class IccTagDataEntry : IEquatable<IccTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccTagDataEntry" /> class.
    ///     TagSignature will be <see cref="IccProfileTag.Unknown" />
    /// </summary>
    /// <param name="signature">Type Signature</param>
    protected IccTagDataEntry(IccTypeSignature signature)
        : this(signature, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccTagDataEntry" /> class.
    /// </summary>
    /// <param name="signature">Type Signature</param>
    /// <param name="tagSignature">Tag Signature</param>
    protected IccTagDataEntry(IccTypeSignature signature, IccProfileTag tagSignature)
    {
        Signature = signature;
        TagSignature = tagSignature;
    }

    /// <summary>
    ///     Gets the type Signature
    /// </summary>
    public IccTypeSignature Signature { get; }

    /// <summary>
    ///     Gets or sets the tag Signature
    /// </summary>
    public IccProfileTag TagSignature { get; set; }

    /// <inheritdoc />
    public virtual bool Equals(IccTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return Signature == other.Signature;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Signature.GetHashCode();
    }
}