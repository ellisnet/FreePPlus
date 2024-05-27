// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Provides methods to read ICC data types
/// </summary>
internal sealed partial class IccDataReader
{
    /// <summary>
    ///     Reads a <see cref="IccMultiProcessElement" />
    /// </summary>
    /// <returns>The read <see cref="IccMultiProcessElement" /></returns>
    public IccMultiProcessElement ReadMultiProcessElement()
    {
        var signature = (IccMultiProcessElementSignature)ReadUInt32();
        var inChannelCount = ReadUInt16();
        var outChannelCount = ReadUInt16();

        switch (signature)
        {
            case IccMultiProcessElementSignature.CurveSet:
                return ReadCurveSetProcessElement(inChannelCount, outChannelCount);
            case IccMultiProcessElementSignature.Matrix:
                return ReadMatrixProcessElement(inChannelCount, outChannelCount);
            case IccMultiProcessElementSignature.Clut:
                return ReadClutProcessElement(inChannelCount, outChannelCount);

            // Currently just placeholders for future ICC expansion
            case IccMultiProcessElementSignature.BAcs:
                AddIndex(8);
                return new IccBAcsProcessElement(inChannelCount, outChannelCount);
            case IccMultiProcessElementSignature.EAcs:
                AddIndex(8);
                return new IccEAcsProcessElement(inChannelCount, outChannelCount);

            default:
                throw new InvalidIccProfileException($"Invalid MultiProcessElement type of {signature}");
        }
    }

    /// <summary>
    ///     Reads a CurveSet <see cref="IccMultiProcessElement" />
    /// </summary>
    /// <param name="inChannelCount">Number of input channels</param>
    /// <param name="outChannelCount">Number of output channels</param>
    /// <returns>The read <see cref="IccCurveSetProcessElement" /></returns>
    public IccCurveSetProcessElement ReadCurveSetProcessElement(int inChannelCount, int outChannelCount)
    {
        var curves = new IccOneDimensionalCurve[inChannelCount];
        for (var i = 0; i < inChannelCount; i++)
        {
            curves[i] = ReadOneDimensionalCurve();
            AddPadding();
        }

        return new IccCurveSetProcessElement(curves);
    }

    /// <summary>
    ///     Reads a Matrix <see cref="IccMultiProcessElement" />
    /// </summary>
    /// <param name="inChannelCount">Number of input channels</param>
    /// <param name="outChannelCount">Number of output channels</param>
    /// <returns>The read <see cref="IccMatrixProcessElement" /></returns>
    public IccMatrixProcessElement ReadMatrixProcessElement(int inChannelCount, int outChannelCount)
    {
        return new IccMatrixProcessElement(
            ReadMatrix(inChannelCount, outChannelCount, true),
            ReadMatrix(outChannelCount, true));
    }

    /// <summary>
    ///     Reads a CLUT <see cref="IccMultiProcessElement" />
    /// </summary>
    /// <param name="inChannelCount">Number of input channels</param>
    /// <param name="outChannelCount">Number of output channels</param>
    /// <returns>The read <see cref="IccClutProcessElement" /></returns>
    public IccClutProcessElement ReadClutProcessElement(int inChannelCount, int outChannelCount)
    {
        return new IccClutProcessElement(ReadClut(inChannelCount, outChannelCount, true));
    }
}