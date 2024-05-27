// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     Color converter between <see cref="YCbCr" /> and <see cref="Rgb" />
///     See <see href="https://en.wikipedia.org/wiki/YCbCr#JPEG_conversion" /> for formulas.
/// </summary>
internal sealed class YCbCrAndRgbConverter
{
    private static readonly Vector3 MaxBytes = new(255F);

    /// <summary>
    ///     Performs the conversion from the <see cref="YCbCr" /> input to an instance of <see cref="Rgb" /> type.
    /// </summary>
    /// <param name="input">The input color instance.</param>
    /// <returns>The converted result.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgb Convert(in YCbCr input)
    {
        var y = input.Y;
        var cb = input.Cb - 128F;
        var cr = input.Cr - 128F;

        var r = MathF.Round(y + 1.402F * cr, MidpointRounding.AwayFromZero);
        var g = MathF.Round(y - 0.344136F * cb - 0.714136F * cr, MidpointRounding.AwayFromZero);
        var b = MathF.Round(y + 1.772F * cb, MidpointRounding.AwayFromZero);

        return new Rgb(new Vector3(r, g, b) / MaxBytes);
    }

    /// <summary>
    ///     Performs the conversion from the <see cref="Rgb" /> input to an instance of <see cref="YCbCr" /> type.
    /// </summary>
    /// <param name="input">The input color instance.</param>
    /// <returns>The converted result.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public YCbCr Convert(in Rgb input)
    {
        var rgb = input.ToVector3() * MaxBytes;
        var r = rgb.X;
        var g = rgb.Y;
        var b = rgb.Z;

        var y = 0.299F * r + 0.587F * g + 0.114F * b;
        var cb = 128F + (-0.168736F * r - 0.331264F * g + 0.5F * b);
        var cr = 128F + (0.5F * r - 0.418688F * g - 0.081312F * b);

        return new YCbCr(y, cb, cr);
    }
}