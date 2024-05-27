// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     The type contains a one-dimensional table of double values.
/// </summary>
internal sealed class IccCurveTagDataEntry : IccTagDataEntry, IEquatable<IccCurveTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccCurveTagDataEntry" /> class.
    /// </summary>
    public IccCurveTagDataEntry()
        : this(Array.Empty<float>(), IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccCurveTagDataEntry" /> class.
    /// </summary>
    /// <param name="gamma">Gamma value</param>
    public IccCurveTagDataEntry(float gamma)
        : this(new[] { gamma }, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccCurveTagDataEntry" /> class.
    /// </summary>
    /// <param name="curveData">Curve Data</param>
    public IccCurveTagDataEntry(float[] curveData)
        : this(curveData, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccCurveTagDataEntry" /> class.
    /// </summary>
    /// <param name="tagSignature">Tag Signature</param>
    public IccCurveTagDataEntry(IccProfileTag tagSignature)
        : this(Array.Empty<float>(), tagSignature) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccCurveTagDataEntry" /> class.
    /// </summary>
    /// <param name="gamma">Gamma value</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccCurveTagDataEntry(float gamma, IccProfileTag tagSignature)
        : this(new[] { gamma }, tagSignature) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccCurveTagDataEntry" /> class.
    /// </summary>
    /// <param name="curveData">Curve Data</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccCurveTagDataEntry(float[] curveData, IccProfileTag tagSignature)
        : base(IccTypeSignature.Curve, tagSignature)
    {
        CurveData = curveData ?? Array.Empty<float>();
    }

    /// <summary>
    ///     Gets the curve data
    /// </summary>
    public float[] CurveData { get; }

    /// <summary>
    ///     Gets the gamma value.
    ///     Only valid if <see cref="IsGamma" /> is true
    /// </summary>
    public float Gamma => IsGamma ? CurveData[0] : 0;

    /// <summary>
    ///     Gets a value indicating whether the curve maps input directly to output.
    /// </summary>
    public bool IsIdentityResponse => CurveData.Length == 0;

    /// <summary>
    ///     Gets a value indicating whether the curve is a gamma curve.
    /// </summary>
    public bool IsGamma => CurveData.Length == 1;

    /// <inheritdoc />
    public bool Equals(IccCurveTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other) && CurveData.AsSpan().SequenceEqual(other.CurveData);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccCurveTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccCurveTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Signature, CurveData);
    }
}