// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <content>
///     Provides methods to write ICC data types
/// </content>
internal sealed partial class IccDataWriter
{
    /// <summary>
    ///     Writes a <see cref="IccOneDimensionalCurve" />
    /// </summary>
    /// <param name="value">The curve to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteOneDimensionalCurve(IccOneDimensionalCurve value)
    {
        var count = WriteUInt16((ushort)value.Segments.Length);
        count += WriteEmpty(2);

        foreach (var point in value.BreakPoints) count += WriteSingle(point);

        foreach (var segment in value.Segments) count += WriteCurveSegment(segment);

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccResponseCurve" />
    /// </summary>
    /// <param name="value">The curve to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteResponseCurve(IccResponseCurve value)
    {
        var count = WriteUInt32((uint)value.CurveType);

        foreach (var responseArray in value.ResponseArrays) count += WriteUInt32((uint)responseArray.Length);

        foreach (var xyz in value.XyzValues) count += WriteXyzNumber(xyz);

        foreach (var responseArray in value.ResponseArrays)
        foreach (var response in responseArray)
            count += WriteResponseNumber(response);

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccParametricCurve" />
    /// </summary>
    /// <param name="value">The curve to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteParametricCurve(IccParametricCurve value)
    {
        var typeValue = (ushort)value.Type;
        var count = WriteUInt16(typeValue);
        count += WriteEmpty(2);

        if (typeValue <= 4) count += WriteFix16(value.G);

        if (typeValue > 0 && typeValue <= 4)
        {
            count += WriteFix16(value.A);
            count += WriteFix16(value.B);
        }

        if (typeValue > 1 && typeValue <= 4) count += WriteFix16(value.C);

        if (typeValue > 2 && typeValue <= 4) count += WriteFix16(value.D);

        if (typeValue == 4)
        {
            count += WriteFix16(value.E);
            count += WriteFix16(value.F);
        }

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccCurveSegment" />
    /// </summary>
    /// <param name="value">The curve to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteCurveSegment(IccCurveSegment value)
    {
        var count = WriteUInt32((uint)value.Signature);
        count += WriteEmpty(4);

        switch (value.Signature)
        {
            case IccCurveSegmentSignature.FormulaCurve:
                return count + WriteFormulaCurveElement((IccFormulaCurveElement)value);
            case IccCurveSegmentSignature.SampledCurve:
                return count + WriteSampledCurveElement((IccSampledCurveElement)value);
            default:
                throw new InvalidIccProfileException($"Invalid CurveSegment type of {value.Signature}");
        }
    }

    /// <summary>
    ///     Writes a <see cref="IccFormulaCurveElement" />
    /// </summary>
    /// <param name="value">The curve to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteFormulaCurveElement(IccFormulaCurveElement value)
    {
        var count = WriteUInt16((ushort)value.Type);
        count += WriteEmpty(2);

        if (value.Type == IccFormulaCurveType.Type1 || value.Type == IccFormulaCurveType.Type2)
            count += WriteSingle(value.Gamma);

        count += WriteSingle(value.A);
        count += WriteSingle(value.B);
        count += WriteSingle(value.C);

        if (value.Type == IccFormulaCurveType.Type2 || value.Type == IccFormulaCurveType.Type3)
            count += WriteSingle(value.D);

        if (value.Type == IccFormulaCurveType.Type3) count += WriteSingle(value.E);

        return count;
    }

    /// <summary>
    ///     Writes a <see cref="IccSampledCurveElement" />
    /// </summary>
    /// <param name="value">The curve to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteSampledCurveElement(IccSampledCurveElement value)
    {
        var count = WriteUInt32((uint)value.CurveEntries.Length);
        foreach (var entry in value.CurveEntries) count += WriteSingle(entry);

        return count;
    }
}