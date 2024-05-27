// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using FreePPlus.Imaging.ColorSpaces.Companding;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     The sRgb working space.
/// </summary>
public sealed class SRgbWorkingSpace : RgbWorkingSpace
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SRgbWorkingSpace" /> class.
    /// </summary>
    /// <param name="referenceWhite">The reference white point.</param>
    /// <param name="chromaticityCoordinates">The chromaticity of the rgb primaries.</param>
    public SRgbWorkingSpace(CieXyz referenceWhite, RgbPrimariesChromaticityCoordinates chromaticityCoordinates)
        : base(referenceWhite, chromaticityCoordinates) { }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public override float Compress(float channel)
    {
        return SRgbCompanding.Compress(channel);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public override float Expand(float channel)
    {
        return SRgbCompanding.Expand(channel);
    }
}