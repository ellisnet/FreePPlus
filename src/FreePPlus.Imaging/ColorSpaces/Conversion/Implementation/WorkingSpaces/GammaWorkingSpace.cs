// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.ColorSpaces.Companding;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     The gamma working space.
/// </summary>
public sealed class GammaWorkingSpace : RgbWorkingSpace
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GammaWorkingSpace" /> class.
    /// </summary>
    /// <param name="gamma">The gamma value.</param>
    /// <param name="referenceWhite">The reference white point.</param>
    /// <param name="chromaticityCoordinates">The chromaticity of the rgb primaries.</param>
    public GammaWorkingSpace(float gamma, CieXyz referenceWhite,
        RgbPrimariesChromaticityCoordinates chromaticityCoordinates)
        : base(referenceWhite, chromaticityCoordinates)
    {
        Gamma = gamma;
    }

    /// <summary>
    ///     Gets the gamma value.
    /// </summary>
    public float Gamma { get; }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public override float Compress(float channel)
    {
        return GammaCompanding.Compress(channel, Gamma);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public override float Expand(float channel)
    {
        return GammaCompanding.Expand(channel, Gamma);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (obj is null) return false;

        if (ReferenceEquals(this, obj)) return true;

        if (obj is GammaWorkingSpace other)
            return Gamma.Equals(other.Gamma)
                   && WhitePoint.Equals(other.WhitePoint)
                   && ChromaticityCoordinates.Equals(other.ChromaticityCoordinates);

        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            WhitePoint,
            ChromaticityCoordinates,
            Gamma);
    }
}