﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

// ReSharper disable UnusedMember.Global

namespace FreePPlus.Imaging.ColorSpaces;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces;

/// <summary>
///     The well known standard illuminants.
///     Standard illuminants provide a basis for comparing images or colors recorded under different lighting
/// </summary>
/// <remarks>
///     Coefficients taken from: http://www.brucelindbloom.com/index.html?Eqn_ChromAdapt.html
///     <br />
///     Descriptions taken from: http://en.wikipedia.org/wiki/Standard_illuminant
/// </remarks>
public static class Illuminants
{
    /// <summary>
    ///     Incandescent / Tungsten
    /// </summary>
    public static readonly CieXyz A = new(1.09850F, 1F, 0.35585F);

    /// <summary>
    ///     Direct sunlight at noon (obsoleteF)
    /// </summary>
    public static readonly CieXyz B = new(0.99072F, 1F, 0.85223F);

    /// <summary>
    ///     Average / North sky Daylight (obsoleteF)
    /// </summary>
    public static readonly CieXyz C = new(0.98074F, 1F, 1.18232F);

    /// <summary>
    ///     Horizon Light. ICC profile PCS
    /// </summary>
    public static readonly CieXyz D50 = new(0.96422F, 1F, 0.82521F);

    /// <summary>
    ///     Mid-morning / Mid-afternoon Daylight
    /// </summary>
    public static readonly CieXyz D55 = new(0.95682F, 1F, 0.92149F);

    /// <summary>
    ///     Noon Daylight: TelevisionF, sRGB color space
    /// </summary>
    public static readonly CieXyz D65 = new(0.95047F, 1F, 1.08883F);

    /// <summary>
    ///     North sky Daylight
    /// </summary>
    public static readonly CieXyz D75 = new(0.94972F, 1F, 1.22638F);

    /// <summary>
    ///     Equal energy
    /// </summary>
    public static readonly CieXyz E = new(1F, 1F, 1F);

    /// <summary>
    ///     Cool White Fluorescent
    /// </summary>
    public static readonly CieXyz F2 = new(0.99186F, 1F, 0.67393F);

    /// <summary>
    ///     D65 simulatorF, Daylight simulator
    /// </summary>
    public static readonly CieXyz F7 = new(0.95041F, 1F, 1.08747F);

    /// <summary>
    ///     Philips TL84F, Ultralume 40
    /// </summary>
    public static readonly CieXyz F11 = new(1.00962F, 1F, 0.64350F);
}