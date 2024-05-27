// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Metadata;
using FreePPlus.Imaging.Metadata.Profiles.Exif;

namespace FreePPlus.Imaging.Common.Helpers;

//was previously: namespace SixLabors.ImageSharp.Common.Helpers;

/// <summary>
///     Contains methods for converting values between unit scales.
/// </summary>
internal static class UnitConverter
{
    /// <summary>
    ///     The number of centimeters in a meter.
    ///     1 cm is equal to exactly 0.01 meters.
    /// </summary>
    private const double CmsInMeter = 1 / 0.01D;

    /// <summary>
    ///     The number of centimeters in an inch.
    ///     1 inch is equal to exactly 2.54 centimeters.
    /// </summary>
    private const double CmsInInch = 2.54D;

    /// <summary>
    ///     The number of inches in a meter.
    ///     1 inch is equal to exactly 0.0254 meters.
    /// </summary>
    private const double InchesInMeter = 1 / 0.0254D;

    /// <summary>
    ///     Scales the value from centimeters to meters.
    /// </summary>
    /// <param name="x">The value to scale.</param>
    /// <returns>The <see cref="double" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static double CmToMeter(double x)
    {
        return x * CmsInMeter;
    }

    /// <summary>
    ///     Scales the value from meters to centimeters.
    /// </summary>
    /// <param name="x">The value to scale.</param>
    /// <returns>The <see cref="double" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static double MeterToCm(double x)
    {
        return x / CmsInMeter;
    }

    /// <summary>
    ///     Scales the value from meters to inches.
    /// </summary>
    /// <param name="x">The value to scale.</param>
    /// <returns>The <see cref="double" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static double MeterToInch(double x)
    {
        return x / InchesInMeter;
    }

    /// <summary>
    ///     Scales the value from inches to meters.
    /// </summary>
    /// <param name="x">The value to scale.</param>
    /// <returns>The <see cref="double" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static double InchToMeter(double x)
    {
        return x * InchesInMeter;
    }

    /// <summary>
    ///     Scales the value from centimeters to inches.
    /// </summary>
    /// <param name="x">The value to scale.</param>
    /// <returns>The <see cref="double" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static double CmToInch(double x)
    {
        return x / CmsInInch;
    }

    /// <summary>
    ///     Scales the value from inches to centimeters.
    /// </summary>
    /// <param name="x">The value to scale.</param>
    /// <returns>The <see cref="double" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static double InchToCm(double x)
    {
        return x * CmsInInch;
    }

    /// <summary>
    ///     Converts an <see cref="ExifTag.ResolutionUnit" /> to a <see cref="PixelResolutionUnit" />.
    /// </summary>
    /// <param name="profile">The EXIF profile containing the value.</param>
    /// <returns>The <see cref="PixelResolutionUnit" /></returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static PixelResolutionUnit ExifProfileToResolutionUnit(ExifProfile profile)
    {
        var resolution = profile.GetValue(ExifTag.ResolutionUnit);

        // EXIF is 1, 2, 3 so we minus "1" off the result.
        return resolution is null ? default : (PixelResolutionUnit)(byte)(resolution.Value - 1);
    }
}