﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     A sampled curve segment
/// </summary>
internal sealed class IccSampledCurveElement : IccCurveSegment, IEquatable<IccSampledCurveElement>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccSampledCurveElement" /> class.
    /// </summary>
    /// <param name="curveEntries">The curve values of this segment</param>
    public IccSampledCurveElement(float[] curveEntries)
        : base(IccCurveSegmentSignature.SampledCurve)
    {
        Guard.NotNull(curveEntries, nameof(curveEntries));
        Guard.IsTrue(curveEntries.Length > 0, nameof(curveEntries), "There must be at least one value");

        CurveEntries = curveEntries;
    }

    /// <summary>
    ///     Gets the curve values of this segment
    /// </summary>
    public float[] CurveEntries { get; }

    /// <inheritdoc />
    public bool Equals(IccSampledCurveElement other)
    {
        return Equals((IccCurveSegment)other);
    }

    /// <inheritdoc />
    public override bool Equals(IccCurveSegment other)
    {
        if (base.Equals(other) && other is IccSampledCurveElement segment)
            return CurveEntries.AsSpan().SequenceEqual(segment.CurveEntries);

        return false;
    }
}