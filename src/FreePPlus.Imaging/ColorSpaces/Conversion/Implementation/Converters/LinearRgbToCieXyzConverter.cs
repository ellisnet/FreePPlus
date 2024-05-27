// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     Color converter between <see cref="LinearRgb" /> and <see cref="CieXyz" />
/// </summary>
internal sealed class LinearRgbToCieXyzConverter : LinearRgbAndCieXyzConverterBase
{
    private readonly Matrix4x4 _conversionMatrix;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LinearRgbToCieXyzConverter" /> class.
    /// </summary>
    public LinearRgbToCieXyzConverter()
        : this(Rgb.DefaultWorkingSpace) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LinearRgbToCieXyzConverter" /> class.
    /// </summary>
    /// <param name="workingSpace">The target working space.</param>
    public LinearRgbToCieXyzConverter(RgbWorkingSpace workingSpace)
    {
        SourceWorkingSpace = workingSpace;
        _conversionMatrix = GetRgbToCieXyzMatrix(workingSpace);
    }

    /// <summary>
    ///     Gets the source working space
    /// </summary>
    public RgbWorkingSpace SourceWorkingSpace { get; }

    /// <summary>
    ///     Performs the conversion from the <see cref="LinearRgb" /> input to an instance of <see cref="CieXyz" /> type.
    /// </summary>
    /// <param name="input">The input color instance.</param>
    /// <returns>The converted result</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public CieXyz Convert(in LinearRgb input)
    {
        DebugGuard.IsTrue(input.WorkingSpace.Equals(SourceWorkingSpace), nameof(input.WorkingSpace),
            "Input and source working spaces must be equal.");

        var vector = Vector3.Transform(input.ToVector3(), _conversionMatrix);
        return new CieXyz(vector);
    }
}