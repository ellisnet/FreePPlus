// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     Color converter between CIE XYZ and CIE xyY.
///     <see href="http://www.brucelindbloom.com/" /> for formulas.
/// </summary>
internal sealed class CieXyzAndCieXyyConverter
{
    /// <summary>
    ///     Performs the conversion from the <see cref="CieXyz" /> input to an instance of <see cref="CieXyy" /> type.
    /// </summary>
    /// <param name="input">The input color instance.</param>
    /// <returns>The converted result</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public CieXyy Convert(in CieXyz input)
    {
        var x = input.X / (input.X + input.Y + input.Z);
        var y = input.Y / (input.X + input.Y + input.Z);

        if (float.IsNaN(x) || float.IsNaN(y)) return new CieXyy(0, 0, input.Y);

        return new CieXyy(x, y, input.Y);
    }

    /// <summary>
    ///     Performs the conversion from the <see cref="CieXyy" /> input to an instance of <see cref="CieXyz" /> type.
    /// </summary>
    /// <param name="input">The input color instance.</param>
    /// <returns>The converted result</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public CieXyz Convert(in CieXyy input)
    {
        if (MathF.Abs(input.Y) < Constants.Epsilon) return new CieXyz(0, 0, input.Yl);

        var x = input.X * input.Yl / input.Y;
        var y = input.Yl;
        var z = (1 - input.X - input.Y) * y / input.Y;

        return new CieXyz(x, y, z);
    }
}