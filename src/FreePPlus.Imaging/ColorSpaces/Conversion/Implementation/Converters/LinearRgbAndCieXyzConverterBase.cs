// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     Provides base methods for converting between <see cref="LinearRgb" /> and <see cref="CieXyz" /> color spaces.
/// </summary>
internal abstract class LinearRgbAndCieXyzConverterBase
{
    /// <summary>
    ///     Returns the correct matrix to convert between the Rgb and CieXyz color space.
    /// </summary>
    /// <param name="workingSpace">The Rgb working space.</param>
    /// <returns>The <see cref="Matrix4x4" /> based on the chromaticity and working space.</returns>
    public static Matrix4x4 GetRgbToCieXyzMatrix(RgbWorkingSpace workingSpace)
    {
        DebugGuard.NotNull(workingSpace, nameof(workingSpace));
        var chromaticity = workingSpace.ChromaticityCoordinates;

        var xr = chromaticity.R.X;
        var xg = chromaticity.G.X;
        var xb = chromaticity.B.X;
        var yr = chromaticity.R.Y;
        var yg = chromaticity.G.Y;
        var yb = chromaticity.B.Y;

        var mXr = xr / yr;
        const float Yr = 1;
        var mZr = (1 - xr - yr) / yr;

        var mXg = xg / yg;
        const float Yg = 1;
        var mZg = (1 - xg - yg) / yg;

        var mXb = xb / yb;
        const float Yb = 1;
        var mZb = (1 - xb - yb) / yb;

        var xyzMatrix = new Matrix4x4
        {
            M11 = mXr,
            M21 = mXg,
            M31 = mXb,
            M12 = Yr,
            M22 = Yg,
            M32 = Yb,
            M13 = mZr,
            M23 = mZg,
            M33 = mZb,
            M44 = 1F
        };

        Matrix4x4.Invert(xyzMatrix, out var inverseXyzMatrix);

        var vector = Vector3.Transform(workingSpace.WhitePoint.ToVector3(), inverseXyzMatrix);

        // Use transposed Rows/Columns
        // T-O-D-O: Is there a built-in method for this multiplication?
        return new Matrix4x4
        {
            M11 = vector.X * mXr,
            M21 = vector.Y * mXg,
            M31 = vector.Z * mXb,
            M12 = vector.X * Yr,
            M22 = vector.Y * Yg,
            M32 = vector.Z * Yb,
            M13 = vector.X * mZr,
            M23 = vector.Y * mZg,
            M33 = vector.Z * mZb,
            M44 = 1F
        };
    }
}