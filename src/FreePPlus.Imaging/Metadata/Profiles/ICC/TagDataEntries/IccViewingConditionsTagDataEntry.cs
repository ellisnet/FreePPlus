// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     This type represents a set of viewing condition parameters.
/// </summary>
internal sealed class IccViewingConditionsTagDataEntry : IccTagDataEntry, IEquatable<IccViewingConditionsTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccViewingConditionsTagDataEntry" /> class.
    /// </summary>
    /// <param name="illuminantXyz">XYZ values of Illuminant</param>
    /// <param name="surroundXyz">XYZ values of Surrounding</param>
    /// <param name="illuminant">Illuminant</param>
    public IccViewingConditionsTagDataEntry(Vector3 illuminantXyz, Vector3 surroundXyz,
        IccStandardIlluminant illuminant)
        : this(illuminantXyz, surroundXyz, illuminant, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccViewingConditionsTagDataEntry" /> class.
    /// </summary>
    /// <param name="illuminantXyz">XYZ values of Illuminant</param>
    /// <param name="surroundXyz">XYZ values of Surrounding</param>
    /// <param name="illuminant">Illuminant</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccViewingConditionsTagDataEntry(Vector3 illuminantXyz, Vector3 surroundXyz,
        IccStandardIlluminant illuminant, IccProfileTag tagSignature)
        : base(IccTypeSignature.ViewingConditions, tagSignature)
    {
        IlluminantXyz = illuminantXyz;
        SurroundXyz = surroundXyz;
        Illuminant = illuminant;
    }

    /// <summary>
    ///     Gets the XYZ values of illuminant.
    /// </summary>
    public Vector3 IlluminantXyz { get; }

    /// <summary>
    ///     Gets the XYZ values of Surrounding
    /// </summary>
    public Vector3 SurroundXyz { get; }

    /// <summary>
    ///     Gets the illuminant.
    /// </summary>
    public IccStandardIlluminant Illuminant { get; }

    /// <inheritdoc />
    public bool Equals(IccViewingConditionsTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other)
               && IlluminantXyz.Equals(other.IlluminantXyz)
               && SurroundXyz.Equals(other.SurroundXyz)
               && Illuminant == other.Illuminant;
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccViewingConditionsTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccViewingConditionsTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Signature,
            IlluminantXyz,
            SurroundXyz,
            Illuminant);
    }
}