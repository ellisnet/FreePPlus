// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

// ReSharper disable InconsistentNaming

namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <content>
///     Performs chromatic adaptation on the various color spaces.
/// </content>
public partial class ColorSpaceConverter
{
    /// <summary>
    ///     Performs chromatic adaptation of given <see cref="CieXyz" /> color.
    ///     Target white point is <see cref="ColorSpaceConverterOptions.WhitePoint" />.
    /// </summary>
    /// <param name="color">The color to adapt</param>
    /// <param name="sourceWhitePoint">The source white point.</param>
    /// <returns>The adapted color</returns>
    public CieXyz Adapt(in CieXyz color, in CieXyz sourceWhitePoint)
    {
        return Adapt(color, sourceWhitePoint, _whitePoint);
    }

    /// <summary>
    ///     Performs chromatic adaptation of given <see cref="CieXyz" /> color.
    ///     Target white point is <see cref="ColorSpaceConverterOptions.WhitePoint" />.
    /// </summary>
    /// <param name="color">The color to adapt</param>
    /// <param name="sourceWhitePoint">The source white point.</param>
    /// <param name="targetWhitePoint">The target white point.</param>
    /// <returns>The adapted color</returns>
    public CieXyz Adapt(in CieXyz color, in CieXyz sourceWhitePoint, in CieXyz targetWhitePoint)
    {
        if (!_performChromaticAdaptation || sourceWhitePoint.Equals(targetWhitePoint)) return color;

        return _chromaticAdaptation.Transform(color, sourceWhitePoint, targetWhitePoint);
    }

    /// <summary>
    ///     Adapts <see cref="CieLab" /> color from the source white point to white point set in
    ///     <see cref="ColorSpaceConverterOptions.TargetLabWhitePoint" />.
    /// </summary>
    /// <param name="color">The color to adapt</param>
    /// <returns>The adapted color</returns>
    public CieLab Adapt(in CieLab color)
    {
        if (!_performChromaticAdaptation || color.WhitePoint.Equals(_targetLabWhitePoint)) return color;

        var xyzColor = ToCieXyz(color);
        return ToCieLab(xyzColor);
    }

    /// <summary>
    ///     Adapts <see cref="CieLch" /> color from the source white point to white point set in
    ///     <see cref="ColorSpaceConverterOptions.TargetLabWhitePoint" />.
    /// </summary>
    /// <param name="color">The color to adapt</param>
    /// <returns>The adapted color</returns>
    public CieLch Adapt(in CieLch color)
    {
        if (!_performChromaticAdaptation || color.WhitePoint.Equals(_targetLabWhitePoint)) return color;

        var labColor = ToCieLab(color);
        return ToCieLch(labColor);
    }

    /// <summary>
    ///     Adapts <see cref="CieLchuv" /> color from the source white point to white point set in
    ///     <see cref="ColorSpaceConverterOptions.TargetLabWhitePoint" />.
    /// </summary>
    /// <param name="color">The color to adapt</param>
    /// <returns>The adapted color</returns>
    public CieLchuv Adapt(in CieLchuv color)
    {
        if (!_performChromaticAdaptation || color.WhitePoint.Equals(_targetLabWhitePoint)) return color;

        var luvColor = ToCieLuv(color);
        return ToCieLchuv(luvColor);
    }

    /// <summary>
    ///     Adapts <see cref="CieLuv" /> color from the source white point to white point set in
    ///     <see cref="ColorSpaceConverterOptions.TargetLuvWhitePoint" />.
    /// </summary>
    /// <param name="color">The color to adapt</param>
    /// <returns>The adapted color</returns>
    public CieLuv Adapt(in CieLuv color)
    {
        if (!_performChromaticAdaptation || color.WhitePoint.Equals(_targetLuvWhitePoint)) return color;

        var xyzColor = ToCieXyz(color);
        return ToCieLuv(xyzColor);
    }

    /// <summary>
    ///     Adapts <see cref="HunterLab" /> color from the source white point to white point set in
    ///     <see cref="ColorSpaceConverterOptions.TargetHunterLabWhitePoint" />.
    /// </summary>
    /// <param name="color">The color to adapt</param>
    /// <returns>The adapted color</returns>
    public HunterLab Adapt(in HunterLab color)
    {
        if (!_performChromaticAdaptation || color.WhitePoint.Equals(_targetHunterLabWhitePoint)) return color;

        var xyzColor = ToCieXyz(color);
        return ToHunterLab(xyzColor);
    }

    /// <summary>
    ///     Adapts a <see cref="LinearRgb" /> color from the source working space to working space set in
    ///     <see cref="ColorSpaceConverterOptions.TargetRgbWorkingSpace" />.
    /// </summary>
    /// <param name="color">The color to adapt</param>
    /// <returns>The adapted color</returns>
    public LinearRgb Adapt(in LinearRgb color)
    {
        if (!_performChromaticAdaptation || color.WorkingSpace.Equals(_targetRgbWorkingSpace)) return color;

        // Conversion to XYZ
        var converterToXYZ = GetLinearRgbToCieXyzConverter(color.WorkingSpace);
        var unadapted = converterToXYZ.Convert(color);

        // Adaptation
        var adapted = _chromaticAdaptation.Transform(unadapted, color.WorkingSpace.WhitePoint,
            _targetRgbWorkingSpace.WhitePoint);

        // Conversion back to RGB
        return _cieXyzToLinearRgbConverter.Convert(adapted);
    }

    /// <summary>
    ///     Adapts a <see cref="Rgb" /> color from the source working space to working space set in
    ///     <see cref="ColorSpaceConverterOptions.TargetRgbWorkingSpace" />.
    /// </summary>
    /// <param name="color">The color to adapt</param>
    /// <returns>The adapted color</returns>
    public Rgb Adapt(in Rgb color)
    {
        if (!_performChromaticAdaptation) return color;

        var linearInput = ToLinearRgb(color);
        var linearOutput = Adapt(linearInput);
        return ToRgb(linearOutput);
    }
}