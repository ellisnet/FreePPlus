// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     Converts from <see cref="CieLab" /> to <see cref="CieLch" />.
/// </summary>
internal sealed class CieLuvToCieLchuvConverter
{
    /// <summary>
    ///     Performs the conversion from the <see cref="CieLuv" /> input to an instance of <see cref="CieLchuv" /> type.
    /// </summary>
    /// <param name="input">The input color instance.</param>
    /// <returns>The converted result</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public CieLchuv Convert(in CieLuv input)
    {
        // Conversion algorithm described here:
        // https://en.wikipedia.org/wiki/CIELUV#Cylindrical_representation_.28CIELCH.29
        float l = input.L, a = input.U, b = input.V;
        var c = MathF.Sqrt(a * a + b * b);
        var hRadians = MathF.Atan2(b, a);
        var hDegrees = GeometryUtilities.RadianToDegree(hRadians);

        // Wrap the angle round at 360.
        hDegrees %= 360;

        // Make sure it's not negative.
        while (hDegrees < 0) hDegrees += 360;

        return new CieLchuv(l, c, hDegrees, input.WhitePoint);
    }
}