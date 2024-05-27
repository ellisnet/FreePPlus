// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     The parametricCurveType describes a one-dimensional curve by
///     specifying one of a predefined set of functions using the parameters.
/// </summary>
internal sealed class IccParametricCurveTagDataEntry : IccTagDataEntry, IEquatable<IccParametricCurveTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccParametricCurveTagDataEntry" /> class.
    /// </summary>
    /// <param name="curve">The Curve</param>
    public IccParametricCurveTagDataEntry(IccParametricCurve curve)
        : this(curve, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccParametricCurveTagDataEntry" /> class.
    /// </summary>
    /// <param name="curve">The Curve</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccParametricCurveTagDataEntry(IccParametricCurve curve, IccProfileTag tagSignature)
        : base(IccTypeSignature.ParametricCurve, tagSignature)
    {
        Curve = curve ?? throw new ArgumentNullException(nameof(curve));
    }

    /// <summary>
    ///     Gets the Curve
    /// </summary>
    public IccParametricCurve Curve { get; }

    /// <inheritdoc />
    public bool Equals(IccParametricCurveTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other) && Curve.Equals(other.Curve);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccParametricCurveTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccParametricCurveTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Signature, Curve);
    }
}