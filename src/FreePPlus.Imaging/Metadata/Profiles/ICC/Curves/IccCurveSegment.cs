// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     A segment of a curve
/// </summary>
internal abstract class IccCurveSegment : IEquatable<IccCurveSegment>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccCurveSegment" /> class.
    /// </summary>
    /// <param name="signature">The signature of this segment</param>
    protected IccCurveSegment(IccCurveSegmentSignature signature)
    {
        Signature = signature;
    }

    /// <summary>
    ///     Gets the signature of this segment
    /// </summary>
    public IccCurveSegmentSignature Signature { get; }

    /// <inheritdoc />
    public virtual bool Equals(IccCurveSegment other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return Signature == other.Signature;
    }
}