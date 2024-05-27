﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     A one dimensional ICC curve.
/// </summary>
internal sealed class IccOneDimensionalCurve : IEquatable<IccOneDimensionalCurve>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccOneDimensionalCurve" /> class.
    /// </summary>
    /// <param name="breakPoints">The break points of this curve</param>
    /// <param name="segments">The segments of this curve</param>
    public IccOneDimensionalCurve(float[] breakPoints, IccCurveSegment[] segments)
    {
        Guard.NotNull(breakPoints, nameof(breakPoints));
        Guard.NotNull(segments, nameof(segments));

        var isSizeCorrect = breakPoints.Length == segments.Length - 1;
        Guard.IsTrue(isSizeCorrect, $"{nameof(breakPoints)},{nameof(segments)}",
            "Number of BreakPoints must be one less than number of Segments");

        BreakPoints = breakPoints;
        Segments = segments;
    }

    /// <summary>
    ///     Gets the breakpoints that separate two curve segments
    /// </summary>
    public float[] BreakPoints { get; }

    /// <summary>
    ///     Gets an array of curve segments
    /// </summary>
    public IccCurveSegment[] Segments { get; }

    /// <inheritdoc />
    public bool Equals(IccOneDimensionalCurve other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return BreakPoints.AsSpan().SequenceEqual(other.BreakPoints)
               && Segments.AsSpan().SequenceEqual(other.Segments);
    }
}