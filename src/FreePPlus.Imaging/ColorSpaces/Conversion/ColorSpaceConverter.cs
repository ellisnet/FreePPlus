// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     Provides methods to allow the conversion of color values between different color spaces.
/// </summary>
public partial class ColorSpaceConverter
{
    // Options.
    private static readonly ColorSpaceConverterOptions DefaultOptions = new();
    private readonly IChromaticAdaptation _chromaticAdaptation;
    private readonly CieXyzAndLmsConverter _cieXyzAndLmsConverter;
    private readonly CieXyzToCieLabConverter _cieXyzToCieLabConverter;
    private readonly CieXyzToCieLuvConverter _cieXyzToCieLuvConverter;
    private readonly CieXyzToHunterLabConverter _cieXyzToHunterLabConverter;
    private readonly CieXyzToLinearRgbConverter _cieXyzToLinearRgbConverter;
    private readonly Matrix4x4 _lmsAdaptationMatrix;
    private readonly bool _performChromaticAdaptation;
    private readonly CieXyz _targetHunterLabWhitePoint;
    private readonly CieXyz _targetLabWhitePoint;
    private readonly CieXyz _targetLuvWhitePoint;
    private readonly RgbWorkingSpace _targetRgbWorkingSpace;
    private readonly CieXyz _whitePoint;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ColorSpaceConverter" /> class.
    /// </summary>
    public ColorSpaceConverter()
        : this(DefaultOptions) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ColorSpaceConverter" /> class.
    /// </summary>
    /// <param name="options">The configuration options.</param>
    public ColorSpaceConverter(ColorSpaceConverterOptions options)
    {
        Guard.NotNull(options, nameof(options));
        _whitePoint = options.WhitePoint;
        _targetLuvWhitePoint = options.TargetLuvWhitePoint;
        _targetLabWhitePoint = options.TargetLabWhitePoint;
        _targetHunterLabWhitePoint = options.TargetHunterLabWhitePoint;
        _targetRgbWorkingSpace = options.TargetRgbWorkingSpace;
        _chromaticAdaptation = options.ChromaticAdaptation;
        _performChromaticAdaptation = _chromaticAdaptation != null;
        _lmsAdaptationMatrix = options.LmsAdaptationMatrix;

        _cieXyzAndLmsConverter = new CieXyzAndLmsConverter(_lmsAdaptationMatrix);
        _cieXyzToCieLabConverter = new CieXyzToCieLabConverter(_targetLabWhitePoint);
        _cieXyzToCieLuvConverter = new CieXyzToCieLuvConverter(_targetLuvWhitePoint);
        _cieXyzToHunterLabConverter = new CieXyzToHunterLabConverter(_targetHunterLabWhitePoint);
        _cieXyzToLinearRgbConverter = new CieXyzToLinearRgbConverter(_targetRgbWorkingSpace);
    }
}