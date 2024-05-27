﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     The measurementType information refers only to the internal
///     profile data and is meant to provide profile makers an alternative
///     to the default measurement specifications.
/// </summary>
internal sealed class IccMeasurementTagDataEntry : IccTagDataEntry, IEquatable<IccMeasurementTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccMeasurementTagDataEntry" /> class.
    /// </summary>
    /// <param name="observer">Observer</param>
    /// <param name="xyzBacking">XYZ Backing values</param>
    /// <param name="geometry">Geometry</param>
    /// <param name="flare">Flare</param>
    /// <param name="illuminant">Illuminant</param>
    public IccMeasurementTagDataEntry(IccStandardObserver observer, Vector3 xyzBacking, IccMeasurementGeometry geometry,
        float flare, IccStandardIlluminant illuminant)
        : this(observer, xyzBacking, geometry, flare, illuminant, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccMeasurementTagDataEntry" /> class.
    /// </summary>
    /// <param name="observer">Observer</param>
    /// <param name="xyzBacking">XYZ Backing values</param>
    /// <param name="geometry">Geometry</param>
    /// <param name="flare">Flare</param>
    /// <param name="illuminant">Illuminant</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccMeasurementTagDataEntry(IccStandardObserver observer, Vector3 xyzBacking, IccMeasurementGeometry geometry,
        float flare, IccStandardIlluminant illuminant, IccProfileTag tagSignature)
        : base(IccTypeSignature.Measurement, tagSignature)
    {
        Observer = observer;
        XyzBacking = xyzBacking;
        Geometry = geometry;
        Flare = flare;
        Illuminant = illuminant;
    }

    /// <summary>
    ///     Gets the observer
    /// </summary>
    public IccStandardObserver Observer { get; }

    /// <summary>
    ///     Gets the XYZ Backing values
    /// </summary>
    public Vector3 XyzBacking { get; }

    /// <summary>
    ///     Gets the geometry
    /// </summary>
    public IccMeasurementGeometry Geometry { get; }

    /// <summary>
    ///     Gets the flare
    /// </summary>
    public float Flare { get; }

    /// <summary>
    ///     Gets the illuminant
    /// </summary>
    public IccStandardIlluminant Illuminant { get; }

    /// <inheritdoc />
    public bool Equals(IccMeasurementTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other)
               && Observer == other.Observer
               && XyzBacking.Equals(other.XyzBacking)
               && Geometry == other.Geometry
               && Flare.Equals(other.Flare)
               && Illuminant == other.Illuminant;
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccMeasurementTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccMeasurementTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Signature,
            Observer,
            XyzBacking,
            Geometry,
            Flare,
            Illuminant);
    }
}