// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Provides methods to write ICC data types
/// </summary>
internal sealed partial class IccDataWriter
{
    /// <summary>
    ///     Writes a <see cref="IccMultiProcessElement" />
    /// </summary>
    /// <param name="value">The element to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteMultiProcessElement(IccMultiProcessElement value)
    {
        var count = WriteUInt32((uint)value.Signature);
        count += WriteUInt16((ushort)value.InputChannelCount);
        count += WriteUInt16((ushort)value.OutputChannelCount);

        switch (value.Signature)
        {
            case IccMultiProcessElementSignature.CurveSet:
                return count + WriteCurveSetProcessElement((IccCurveSetProcessElement)value);
            case IccMultiProcessElementSignature.Matrix:
                return count + WriteMatrixProcessElement((IccMatrixProcessElement)value);
            case IccMultiProcessElementSignature.Clut:
                return count + WriteClutProcessElement((IccClutProcessElement)value);

            case IccMultiProcessElementSignature.BAcs:
            case IccMultiProcessElementSignature.EAcs:
                return count + WriteEmpty(8);

            default:
                throw new InvalidIccProfileException($"Invalid MultiProcessElement type of {value.Signature}");
        }
    }

    /// <summary>
    ///     Writes a CurveSet <see cref="IccMultiProcessElement" />
    /// </summary>
    /// <param name="value">The element to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteCurveSetProcessElement(IccCurveSetProcessElement value)
    {
        var count = 0;
        foreach (var curve in value.Curves)
        {
            count += WriteOneDimensionalCurve(curve);
            count += WritePadding();
        }

        return count;
    }

    /// <summary>
    ///     Writes a Matrix <see cref="IccMultiProcessElement" />
    /// </summary>
    /// <param name="value">The element to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteMatrixProcessElement(IccMatrixProcessElement value)
    {
        return WriteMatrix(value.MatrixIxO, true)
               + WriteMatrix(value.MatrixOx1, true);
    }

    /// <summary>
    ///     Writes a CLUT <see cref="IccMultiProcessElement" />
    /// </summary>
    /// <param name="value">The element to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteClutProcessElement(IccClutProcessElement value)
    {
        return WriteClut(value.ClutValue);
    }
}