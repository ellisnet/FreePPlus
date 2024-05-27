﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Linq;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     The purpose of this tag type is to provide a mechanism to relate physical
///     colorant amounts with the normalized device codes produced by lut8Type, lut16Type,
///     lutAToBType, lutBToAType or multiProcessElementsType tags so that corrections can
///     be made for variation in the device without having to produce a new profile.
/// </summary>
internal sealed class IccResponseCurveSet16TagDataEntry : IccTagDataEntry, IEquatable<IccResponseCurveSet16TagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccResponseCurveSet16TagDataEntry" /> class.
    /// </summary>
    /// <param name="curves">The Curves</param>
    public IccResponseCurveSet16TagDataEntry(IccResponseCurve[] curves)
        : this(curves, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccResponseCurveSet16TagDataEntry" /> class.
    /// </summary>
    /// <param name="curves">The Curves</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccResponseCurveSet16TagDataEntry(IccResponseCurve[] curves, IccProfileTag tagSignature)
        : base(IccTypeSignature.ResponseCurveSet16, tagSignature)
    {
        Guard.NotNull(curves, nameof(curves));
        Guard.IsTrue(curves.Length > 0, nameof(curves), $"{nameof(curves)} needs at least one element");

        Curves = curves;
        ChannelCount = (ushort)curves[0].ResponseArrays.Length;

        Guard.IsFalse(curves.Any(t => t.ResponseArrays.Length != ChannelCount), nameof(curves),
            "All curves need to have the same number of channels");
    }

    /// <summary>
    ///     Gets the number of channels
    /// </summary>
    public ushort ChannelCount { get; }

    /// <summary>
    ///     Gets the curves
    /// </summary>
    public IccResponseCurve[] Curves { get; }

    /// <inheritdoc />
    public bool Equals(IccResponseCurveSet16TagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other)
               && ChannelCount == other.ChannelCount
               && Curves.AsSpan().SequenceEqual(other.Curves);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccResponseCurveSet16TagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccResponseCurveSet16TagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Signature,
            ChannelCount,
            Curves);
    }
}