// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     Color converter between Rgb and LinearRgb.
/// </summary>
internal class RgbToLinearRgbConverter
{
    /// <summary>
    ///     Performs the conversion from the <see cref="Rgb" /> input to an instance of <see cref="LinearRgb" /> type.
    /// </summary>
    /// <param name="input">The input color instance.</param>
    /// <returns>The converted result.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public LinearRgb Convert(in Rgb input)
    {
        return new LinearRgb(
            input.WorkingSpace.Expand(input.R),
            input.WorkingSpace.Expand(input.G),
            input.WorkingSpace.Expand(input.B),
            input.WorkingSpace);
    }
}