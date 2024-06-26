﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     An element to process data
/// </summary>
internal abstract class IccMultiProcessElement : IEquatable<IccMultiProcessElement>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccMultiProcessElement" /> class.
    /// </summary>
    /// <param name="signature">The signature of this element</param>
    /// <param name="inChannelCount">Number of input channels</param>
    /// <param name="outChannelCount">Number of output channels</param>
    protected IccMultiProcessElement(IccMultiProcessElementSignature signature, int inChannelCount, int outChannelCount)
    {
        Guard.MustBeBetweenOrEqualTo(inChannelCount, 1, 15, nameof(inChannelCount));
        Guard.MustBeBetweenOrEqualTo(outChannelCount, 1, 15, nameof(outChannelCount));

        Signature = signature;
        InputChannelCount = inChannelCount;
        OutputChannelCount = outChannelCount;
    }

    /// <summary>
    ///     Gets the signature of this element,
    /// </summary>
    public IccMultiProcessElementSignature Signature { get; }

    /// <summary>
    ///     Gets the number of input channels
    /// </summary>
    public int InputChannelCount { get; }

    /// <summary>
    ///     Gets the number of output channels.
    /// </summary>
    public int OutputChannelCount { get; }

    /// <inheritdoc />
    public virtual bool Equals(IccMultiProcessElement other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return Signature == other.Signature
               && InputChannelCount == other.InputChannelCount
               && OutputChannelCount == other.OutputChannelCount;
    }
}