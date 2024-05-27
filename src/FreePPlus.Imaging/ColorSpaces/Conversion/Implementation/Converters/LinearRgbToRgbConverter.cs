// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     Color converter between <see cref="LinearRgb" /> and <see cref="Rgb" />.
/// </summary>
internal sealed class LinearRgbToRgbConverter
{
    /// <summary>
    ///     Performs the conversion from the <see cref="LinearRgb" /> input to an instance of <see cref="Rgb" /> type.
    /// </summary>
    /// <param name="input">The input color instance.</param>
    /// <returns>The converted result.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgb Convert(in LinearRgb input)
    {
        return new Rgb(
            input.WorkingSpace.Compress(input.R),
            input.WorkingSpace.Compress(input.G),
            input.WorkingSpace.Compress(input.B),
            input.WorkingSpace);
    }
}