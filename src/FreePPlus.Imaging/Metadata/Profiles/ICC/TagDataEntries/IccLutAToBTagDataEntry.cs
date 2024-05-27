// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Linq;
using System.Numerics;

// T-O-D-O: Review the use of base IccTagDataEntry comparison.
namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     This structure represents a color transform.
/// </summary>
internal sealed class IccLutAToBTagDataEntry : IccTagDataEntry, IEquatable<IccLutAToBTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccLutAToBTagDataEntry" /> class.
    /// </summary>
    /// <param name="curveB">B Curve</param>
    /// <param name="matrix3x3">Two dimensional conversion matrix (3x3)</param>
    /// <param name="matrix3x1">One dimensional conversion matrix (3x1)</param>
    /// <param name="curveM">M Curve</param>
    /// <param name="clutValues">CLUT</param>
    /// <param name="curveA">A Curve</param>
    public IccLutAToBTagDataEntry(
        IccTagDataEntry[] curveB,
        float[,] matrix3x3,
        float[] matrix3x1,
        IccTagDataEntry[] curveM,
        IccClut clutValues,
        IccTagDataEntry[] curveA)
        : this(curveB, matrix3x3, matrix3x1, curveM, clutValues, curveA, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccLutAToBTagDataEntry" /> class.
    /// </summary>
    /// <param name="curveB">B Curve</param>
    /// <param name="matrix3x3">Two dimensional conversion matrix (3x3)</param>
    /// <param name="matrix3x1">One dimensional conversion matrix (3x1)</param>
    /// <param name="curveM">M Curve</param>
    /// <param name="clutValues">CLUT</param>
    /// <param name="curveA">A Curve</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccLutAToBTagDataEntry(
        IccTagDataEntry[] curveB,
        float[,] matrix3x3,
        float[] matrix3x1,
        IccTagDataEntry[] curveM,
        IccClut clutValues,
        IccTagDataEntry[] curveA,
        IccProfileTag tagSignature)
        : base(IccTypeSignature.LutAToB, tagSignature)
    {
        VerifyMatrix(matrix3x3, matrix3x1);
        VerifyCurve(curveA, nameof(curveA));
        VerifyCurve(curveB, nameof(curveB));
        VerifyCurve(curveM, nameof(curveM));

        Matrix3x3 = CreateMatrix3x3(matrix3x3);
        Matrix3x1 = CreateMatrix3x1(matrix3x1);
        CurveA = curveA;
        CurveB = curveB;
        CurveM = curveM;
        ClutValues = clutValues;

        if (IsAClutMMatrixB())
        {
            Guard.IsTrue(CurveB.Length == 3, nameof(CurveB), $"{nameof(CurveB)} must have a length of three");
            Guard.IsTrue(CurveM.Length == 3, nameof(CurveM), $"{nameof(CurveM)} must have a length of three");
            Guard.MustBeBetweenOrEqualTo(CurveA.Length, 1, 15, nameof(CurveA));

            InputChannelCount = curveA.Length;
            OutputChannelCount = 3;

            Guard.IsTrue(InputChannelCount == clutValues.InputChannelCount, nameof(clutValues),
                "Input channel count does not match the CLUT size");
            Guard.IsTrue(OutputChannelCount == clutValues.OutputChannelCount, nameof(clutValues),
                "Output channel count does not match the CLUT size");
        }
        else if (IsMMatrixB())
        {
            Guard.IsTrue(CurveB.Length == 3, nameof(CurveB), $"{nameof(CurveB)} must have a length of three");
            Guard.IsTrue(CurveM.Length == 3, nameof(CurveM), $"{nameof(CurveM)} must have a length of three");

            InputChannelCount = OutputChannelCount = 3;
        }
        else if (IsAClutB())
        {
            Guard.MustBeBetweenOrEqualTo(CurveA.Length, 1, 15, nameof(CurveA));
            Guard.MustBeBetweenOrEqualTo(CurveB.Length, 1, 15, nameof(CurveB));

            InputChannelCount = curveA.Length;
            OutputChannelCount = curveB.Length;

            Guard.IsTrue(InputChannelCount == clutValues.InputChannelCount, nameof(clutValues),
                "Input channel count does not match the CLUT size");
            Guard.IsTrue(OutputChannelCount == clutValues.OutputChannelCount, nameof(clutValues),
                "Output channel count does not match the CLUT size");
        }
        else if (IsB())
        {
            InputChannelCount = OutputChannelCount = CurveB.Length;
        }
        else
        {
            throw new ArgumentException("Invalid combination of values given");
        }
    }

    /// <summary>
    ///     Gets the number of input channels
    /// </summary>
    public int InputChannelCount { get; }

    /// <summary>
    ///     Gets the number of output channels
    /// </summary>
    public int OutputChannelCount { get; }

    /// <summary>
    ///     Gets the two dimensional conversion matrix (3x3)
    /// </summary>
    public Matrix4x4? Matrix3x3 { get; }

    /// <summary>
    ///     Gets the one dimensional conversion matrix (3x1)
    /// </summary>
    public Vector3? Matrix3x1 { get; }

    /// <summary>
    ///     Gets the color lookup table
    /// </summary>
    public IccClut ClutValues { get; }

    /// <summary>
    ///     Gets the B Curve
    /// </summary>
    public IccTagDataEntry[] CurveB { get; }

    /// <summary>
    ///     Gets the M Curve
    /// </summary>
    public IccTagDataEntry[] CurveM { get; }

    /// <summary>
    ///     Gets the A Curve
    /// </summary>
    public IccTagDataEntry[] CurveA { get; }

    /// <inheritdoc />
    public bool Equals(IccLutAToBTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other)
               && InputChannelCount == other.InputChannelCount
               && OutputChannelCount == other.OutputChannelCount
               && Matrix3x3.Equals(other.Matrix3x3)
               && Matrix3x1.Equals(other.Matrix3x1)
               && ClutValues.Equals(other.ClutValues)
               && EqualsCurve(CurveB, other.CurveB)
               && EqualsCurve(CurveM, other.CurveM)
               && EqualsCurve(CurveA, other.CurveA);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccLutAToBTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccLutAToBTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        HashCode hashCode = default;

        hashCode.Add(Signature);
        hashCode.Add(InputChannelCount);
        hashCode.Add(OutputChannelCount);
        hashCode.Add(Matrix3x3);
        hashCode.Add(Matrix3x1);
        hashCode.Add(ClutValues);
        hashCode.Add(CurveB);
        hashCode.Add(CurveM);
        hashCode.Add(CurveA);

        return hashCode.ToHashCode();
    }

    private static bool EqualsCurve(IccTagDataEntry[] thisCurves, IccTagDataEntry[] entryCurves)
    {
        var thisNull = thisCurves is null;
        var entryNull = entryCurves is null;

        if (thisNull && entryNull) return true;

        if (entryNull) return false;

        return thisCurves.SequenceEqual(entryCurves);
    }

    private bool IsAClutMMatrixB()
    {
        return CurveB != null
               && Matrix3x3 != null
               && Matrix3x1 != null
               && CurveM != null
               && ClutValues != null
               && CurveA != null;
    }

    private bool IsMMatrixB()
    {
        return CurveB != null
               && Matrix3x3 != null
               && Matrix3x1 != null
               && CurveM != null;
    }

    private bool IsAClutB()
    {
        return CurveB != null
               && ClutValues != null
               && CurveA != null;
    }

    private bool IsB()
    {
        return CurveB != null;
    }

    private void VerifyCurve(IccTagDataEntry[] curves, string name)
    {
        if (curves != null)
        {
            var isNotCurve = curves.Any(t => !(t is IccParametricCurveTagDataEntry) && !(t is IccCurveTagDataEntry));
            Guard.IsFalse(isNotCurve, nameof(name),
                $"{nameof(name)} must be of type {nameof(IccParametricCurveTagDataEntry)} or {nameof(IccCurveTagDataEntry)}");
        }
    }

    private void VerifyMatrix(float[,] matrix3x3, float[] matrix3x1)
    {
        if (matrix3x1 != null)
            Guard.IsTrue(matrix3x1.Length == 3, nameof(matrix3x1), "Matrix must have a size of three");

        if (matrix3x3 != null)
        {
            var is3By3 = matrix3x3.GetLength(0) == 3 && matrix3x3.GetLength(1) == 3;
            Guard.IsTrue(is3By3, nameof(matrix3x3), "Matrix must have a size of three by three");
        }
    }

    private Vector3? CreateMatrix3x1(float[] matrix)
    {
        if (matrix is null) return null;

        return new Vector3(matrix[0], matrix[1], matrix[2]);
    }

    private Matrix4x4? CreateMatrix3x3(float[,] matrix)
    {
        if (matrix is null) return null;

        return new Matrix4x4(
            matrix[0, 0],
            matrix[0, 1],
            matrix[0, 2],
            0,
            matrix[1, 0],
            matrix[1, 1],
            matrix[1, 2],
            0,
            matrix[2, 0],
            matrix[2, 1],
            matrix[2, 2],
            0,
            0,
            0,
            0,
            1);
    }
}