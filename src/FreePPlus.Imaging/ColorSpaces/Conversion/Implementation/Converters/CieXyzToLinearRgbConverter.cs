﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     Color converter between <see cref="CieXyz" /> and <see cref="LinearRgb" />
/// </summary>
internal sealed class CieXyzToLinearRgbConverter : LinearRgbAndCieXyzConverterBase
{
    private readonly Matrix4x4 _conversionMatrix;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CieXyzToLinearRgbConverter" /> class.
    /// </summary>
    public CieXyzToLinearRgbConverter()
        : this(Rgb.DefaultWorkingSpace) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CieXyzToLinearRgbConverter" /> class.
    /// </summary>
    /// <param name="workingSpace">The target working space.</param>
    public CieXyzToLinearRgbConverter(RgbWorkingSpace workingSpace)
    {
        TargetWorkingSpace = workingSpace;

        // Gets the inverted Rgb -> Xyz matrix
        Matrix4x4.Invert(GetRgbToCieXyzMatrix(workingSpace), out var inverted);

        _conversionMatrix = inverted;
    }

    /// <summary>
    ///     Gets the target working space.
    /// </summary>
    public RgbWorkingSpace TargetWorkingSpace { get; }

    /// <summary>
    ///     Performs the conversion from the <see cref="CieXyz" /> input to an instance of <see cref="LinearRgb" /> type.
    /// </summary>
    /// <param name="input">The input color instance.</param>
    /// <returns>The converted result.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public LinearRgb Convert(in CieXyz input)
    {
        var vector = Vector3.Transform(input.ToVector3(), _conversionMatrix);

        return new LinearRgb(vector, TargetWorkingSpace);
    }
}