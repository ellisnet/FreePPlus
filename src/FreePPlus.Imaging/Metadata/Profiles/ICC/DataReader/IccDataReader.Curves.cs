// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Provides methods to read ICC data types
/// </summary>
internal sealed partial class IccDataReader
{
    /// <summary>
    ///     Reads a <see cref="IccOneDimensionalCurve" />
    /// </summary>
    /// <returns>The read curve</returns>
    public IccOneDimensionalCurve ReadOneDimensionalCurve()
    {
        var segmentCount = ReadUInt16();
        AddIndex(2); // 2 bytes reserved
        var breakPoints = new float[segmentCount - 1];
        for (var i = 0; i < breakPoints.Length; i++) breakPoints[i] = ReadSingle();

        var segments = new IccCurveSegment[segmentCount];
        for (var i = 0; i < segmentCount; i++) segments[i] = ReadCurveSegment();

        return new IccOneDimensionalCurve(breakPoints, segments);
    }

    /// <summary>
    ///     Reads a <see cref="IccResponseCurve" />
    /// </summary>
    /// <param name="channelCount">The number of channels</param>
    /// <returns>The read curve</returns>
    public IccResponseCurve ReadResponseCurve(int channelCount)
    {
        var type = (IccCurveMeasurementEncodings)ReadUInt32();
        var measurement = new uint[channelCount];
        for (var i = 0; i < channelCount; i++) measurement[i] = ReadUInt32();

        var xyzValues = new Vector3[channelCount];
        for (var i = 0; i < channelCount; i++) xyzValues[i] = ReadXyzNumber();

        var response = new IccResponseNumber[channelCount][];
        for (var i = 0; i < channelCount; i++)
        {
            response[i] = new IccResponseNumber[measurement[i]];
            for (uint j = 0; j < measurement[i]; j++) response[i][j] = ReadResponseNumber();
        }

        return new IccResponseCurve(type, xyzValues, response);
    }

    /// <summary>
    ///     Reads a <see cref="IccParametricCurve" />
    /// </summary>
    /// <returns>The read curve</returns>
    public IccParametricCurve ReadParametricCurve()
    {
        var type = ReadUInt16();
        AddIndex(2); // 2 bytes reserved
        float gamma, a, b, c, d, e, f;
        gamma = a = b = c = d = e = f = 0;

        if (type <= 4) gamma = ReadFix16();

        if (type > 0 && type <= 4)
        {
            a = ReadFix16();
            b = ReadFix16();
        }

        if (type > 1 && type <= 4) c = ReadFix16();

        if (type > 2 && type <= 4) d = ReadFix16();

        if (type == 4)
        {
            e = ReadFix16();
            f = ReadFix16();
        }

        switch (type)
        {
            case 0: return new IccParametricCurve(gamma);
            case 1: return new IccParametricCurve(gamma, a, b);
            case 2: return new IccParametricCurve(gamma, a, b, c);
            case 3: return new IccParametricCurve(gamma, a, b, c, d);
            case 4: return new IccParametricCurve(gamma, a, b, c, d, e, f);
            default: throw new InvalidIccProfileException($"Invalid parametric curve type of {type}");
        }
    }

    /// <summary>
    ///     Reads a <see cref="IccCurveSegment" />
    /// </summary>
    /// <returns>The read segment</returns>
    public IccCurveSegment ReadCurveSegment()
    {
        var signature = (IccCurveSegmentSignature)ReadUInt32();
        AddIndex(4); // 4 bytes reserved

        switch (signature)
        {
            case IccCurveSegmentSignature.FormulaCurve:
                return ReadFormulaCurveElement();
            case IccCurveSegmentSignature.SampledCurve:
                return ReadSampledCurveElement();
            default:
                throw new InvalidIccProfileException($"Invalid curve segment type of {signature}");
        }
    }

    /// <summary>
    ///     Reads a <see cref="IccFormulaCurveElement" />
    /// </summary>
    /// <returns>The read segment</returns>
    public IccFormulaCurveElement ReadFormulaCurveElement()
    {
        var type = (IccFormulaCurveType)ReadUInt16();
        AddIndex(2); // 2 bytes reserved
        float gamma, a, b, c, d, e;
        gamma = d = e = 0;

        if (type == IccFormulaCurveType.Type1 || type == IccFormulaCurveType.Type2) gamma = ReadSingle();

        a = ReadSingle();
        b = ReadSingle();
        c = ReadSingle();

        if (type == IccFormulaCurveType.Type2 || type == IccFormulaCurveType.Type3) d = ReadSingle();

        if (type == IccFormulaCurveType.Type3) e = ReadSingle();

        return new IccFormulaCurveElement(type, gamma, a, b, c, d, e);
    }

    /// <summary>
    ///     Reads a <see cref="IccSampledCurveElement" />
    /// </summary>
    /// <returns>The read segment</returns>
    public IccSampledCurveElement ReadSampledCurveElement()
    {
        var count = ReadUInt32();
        var entries = new float[count];
        for (var i = 0; i < count; i++) entries[i] = ReadSingle();

        return new IccSampledCurveElement(entries);
    }

    /// <summary>
    ///     Reads curve data
    /// </summary>
    /// <param name="count">Number of input channels</param>
    /// <returns>The curve data</returns>
    private IccTagDataEntry[] ReadCurves(int count)
    {
        var tdata = new IccTagDataEntry[count];
        for (var i = 0; i < count; i++)
        {
            var type = ReadTagDataEntryHeader();
            if (type != IccTypeSignature.Curve && type != IccTypeSignature.ParametricCurve)
                throw new InvalidIccProfileException(
                    $"Curve has to be either \"{nameof(IccTypeSignature)}.{nameof(IccTypeSignature.Curve)}\" or" +
                    $" \"{nameof(IccTypeSignature)}.{nameof(IccTypeSignature.ParametricCurve)}\" for LutAToB- and LutBToA-TagDataEntries");

            if (type == IccTypeSignature.Curve)
                tdata[i] = ReadCurveTagDataEntry();
            else if (type == IccTypeSignature.ParametricCurve) tdata[i] = ReadParametricCurveTagDataEntry();

            AddPadding();
        }

        return tdata;
    }
}