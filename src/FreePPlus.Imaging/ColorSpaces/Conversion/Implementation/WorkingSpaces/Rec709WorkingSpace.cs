// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using FreePPlus.Imaging.ColorSpaces.Companding;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     Rec. 709 (ITU-R Recommendation BT.709) working space.
/// </summary>
public sealed class Rec709WorkingSpace : RgbWorkingSpace
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Rec709WorkingSpace" /> class.
    /// </summary>
    /// <param name="referenceWhite">The reference white point.</param>
    /// <param name="chromaticityCoordinates">The chromaticity of the rgb primaries.</param>
    public Rec709WorkingSpace(CieXyz referenceWhite, RgbPrimariesChromaticityCoordinates chromaticityCoordinates)
        : base(referenceWhite, chromaticityCoordinates) { }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public override float Compress(float channel)
    {
        return Rec709Companding.Compress(channel);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public override float Expand(float channel)
    {
        return Rec709Companding.Expand(channel);
    }
}