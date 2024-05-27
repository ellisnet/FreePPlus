// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

#pragma warning disable SA1117 // Parameters should be on same line or separate lines
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     A structure encapsulating a 5x4 matrix used for transforming the color and alpha components of an image.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ColorMatrix : IEquatable<ColorMatrix>
{
    /// <summary>
    ///     Value at row 1, column 1 of the matrix.
    /// </summary>
    public float M11;

    /// <summary>
    ///     Value at row 1, column 2 of the matrix.
    /// </summary>
    public float M12;

    /// <summary>
    ///     Value at row 1, column 3 of the matrix.
    /// </summary>
    public float M13;

    /// <summary>
    ///     Value at row 1, column 4 of the matrix.
    /// </summary>
    public float M14;

    /// <summary>
    ///     Value at row 2, column 1 of the matrix.
    /// </summary>
    public float M21;

    /// <summary>
    ///     Value at row 2, column 2 of the matrix.
    /// </summary>
    public float M22;

    /// <summary>
    ///     Value at row 2, column 3 of the matrix.
    /// </summary>
    public float M23;

    /// <summary>
    ///     Value at row 2, column 4 of the matrix.
    /// </summary>
    public float M24;

    /// <summary>
    ///     Value at row 3, column 1 of the matrix.
    /// </summary>
    public float M31;

    /// <summary>
    ///     Value at row 3, column 2 of the matrix.
    /// </summary>
    public float M32;

    /// <summary>
    ///     Value at row 3, column 3 of the matrix.
    /// </summary>
    public float M33;

    /// <summary>
    ///     Value at row 3, column 4 of the matrix.
    /// </summary>
    public float M34;

    /// <summary>
    ///     Value at row 4, column 1 of the matrix.
    /// </summary>
    public float M41;

    /// <summary>
    ///     Value at row 4, column 2 of the matrix.
    /// </summary>
    public float M42;

    /// <summary>
    ///     Value at row 4, column 3 of the matrix.
    /// </summary>
    public float M43;

    /// <summary>
    ///     Value at row 4, column 4 of the matrix.
    /// </summary>
    public float M44;

    /// <summary>
    ///     Value at row 5, column 1 of the matrix.
    /// </summary>
    public float M51;

    /// <summary>
    ///     Value at row 5, column 2 of the matrix.
    /// </summary>
    public float M52;

    /// <summary>
    ///     Value at row 5, column 3 of the matrix.
    /// </summary>
    public float M53;

    /// <summary>
    ///     Value at row 5, column 4 of the matrix.
    /// </summary>
    public float M54;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ColorMatrix" /> struct.
    /// </summary>
    /// <param name="m11">The value at row 1, column 1 of the matrix.</param>
    /// <param name="m12">The value at row 1, column 2 of the matrix.</param>
    /// <param name="m13">The value at row 1, column 3 of the matrix.</param>
    /// <param name="m14">The value at row 1, column 4 of the matrix.</param>
    /// <param name="m21">The value at row 2, column 1 of the matrix.</param>
    /// <param name="m22">The value at row 2, column 2 of the matrix.</param>
    /// <param name="m23">The value at row 2, column 3 of the matrix.</param>
    /// <param name="m24">The value at row 2, column 4 of the matrix.</param>
    /// <param name="m31">The value at row 3, column 1 of the matrix.</param>
    /// <param name="m32">The value at row 3, column 2 of the matrix.</param>
    /// <param name="m33">The value at row 3, column 3 of the matrix.</param>
    /// <param name="m34">The value at row 3, column 4 of the matrix.</param>
    /// <param name="m41">The value at row 4, column 1 of the matrix.</param>
    /// <param name="m42">The value at row 4, column 2 of the matrix.</param>
    /// <param name="m43">The value at row 4, column 3 of the matrix.</param>
    /// <param name="m44">The value at row 4, column 4 of the matrix.</param>
    /// <param name="m51">The value at row 5, column 1 of the matrix.</param>
    /// <param name="m52">The value at row 5, column 2 of the matrix.</param>
    /// <param name="m53">The value at row 5, column 3 of the matrix.</param>
    /// <param name="m54">The value at row 5, column 4 of the matrix.</param>
    public ColorMatrix(float m11, float m12, float m13, float m14,
        float m21, float m22, float m23, float m24,
        float m31, float m32, float m33, float m34,
        float m41, float m42, float m43, float m44,
        float m51, float m52, float m53, float m54)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M14 = m14;

        M21 = m21;
        M22 = m22;
        M23 = m23;
        M24 = m24;

        M31 = m31;
        M32 = m32;
        M33 = m33;
        M34 = m34;

        M41 = m41;
        M42 = m42;
        M43 = m43;
        M44 = m44;

        M51 = m51;
        M52 = m52;
        M53 = m53;
        M54 = m54;
    }

    /// <summary>
    ///     Gets the multiplicative identity matrix.
    /// </summary>
    public static ColorMatrix Identity { get; } =
        new(1F, 0F, 0F, 0F,
            0F, 1F, 0F, 0F,
            0F, 0F, 1F, 0F,
            0F, 0F, 0F, 1F,
            0F, 0F, 0F, 0F);

    /// <summary>
    ///     Gets a value indicating whether the matrix is the identity matrix.
    /// </summary>
    public bool IsIdentity =>
        // Check diagonal element first for early out.
        M11 == 1F && M22 == 1F && M33 == 1F && M44 == 1F
        && M12 == 0F && M13 == 0F && M14 == 0F
        && M21 == 0F && M23 == 0F && M24 == 0F
        && M31 == 0F && M32 == 0F && M34 == 0F
        && M41 == 0F && M42 == 0F && M43 == 0F
        && M51 == 0F && M52 == 0F && M53 == 0F && M54 == 0F;

    /// <summary>
    ///     Adds two matrices together.
    /// </summary>
    /// <param name="value1">The first source matrix.</param>
    /// <param name="value2">The second source matrix.</param>
    /// <returns>The resulting matrix.</returns>
    public static ColorMatrix operator +(ColorMatrix value1, ColorMatrix value2)
    {
        var m = default(ColorMatrix);

        m.M11 = value1.M11 + value2.M11;
        m.M12 = value1.M12 + value2.M12;
        m.M13 = value1.M13 + value2.M13;
        m.M14 = value1.M14 + value2.M14;
        m.M21 = value1.M21 + value2.M21;
        m.M22 = value1.M22 + value2.M22;
        m.M23 = value1.M23 + value2.M23;
        m.M24 = value1.M24 + value2.M24;
        m.M31 = value1.M31 + value2.M31;
        m.M32 = value1.M32 + value2.M32;
        m.M33 = value1.M33 + value2.M33;
        m.M34 = value1.M34 + value2.M34;
        m.M41 = value1.M41 + value2.M41;
        m.M42 = value1.M42 + value2.M42;
        m.M43 = value1.M43 + value2.M43;
        m.M44 = value1.M44 + value2.M44;
        m.M51 = value1.M51 + value2.M51;
        m.M52 = value1.M52 + value2.M52;
        m.M53 = value1.M53 + value2.M53;
        m.M54 = value1.M54 + value2.M54;

        return m;
    }

    /// <summary>
    ///     Subtracts the second matrix from the first.
    /// </summary>
    /// <param name="value1">The first source matrix.</param>
    /// <param name="value2">The second source matrix.</param>
    /// <returns>The result of the subtraction.</returns>
    public static ColorMatrix operator -(ColorMatrix value1, ColorMatrix value2)
    {
        var m = default(ColorMatrix);

        m.M11 = value1.M11 - value2.M11;
        m.M12 = value1.M12 - value2.M12;
        m.M13 = value1.M13 - value2.M13;
        m.M14 = value1.M14 - value2.M14;
        m.M21 = value1.M21 - value2.M21;
        m.M22 = value1.M22 - value2.M22;
        m.M23 = value1.M23 - value2.M23;
        m.M24 = value1.M24 - value2.M24;
        m.M31 = value1.M31 - value2.M31;
        m.M32 = value1.M32 - value2.M32;
        m.M33 = value1.M33 - value2.M33;
        m.M34 = value1.M34 - value2.M34;
        m.M41 = value1.M41 - value2.M41;
        m.M42 = value1.M42 - value2.M42;
        m.M43 = value1.M43 - value2.M43;
        m.M44 = value1.M44 - value2.M44;
        m.M51 = value1.M51 - value2.M51;
        m.M52 = value1.M52 - value2.M52;
        m.M53 = value1.M53 - value2.M53;
        m.M54 = value1.M54 - value2.M54;

        return m;
    }

    /// <summary>
    ///     Returns a new matrix with the negated elements of the given matrix.
    /// </summary>
    /// <param name="value">The source matrix.</param>
    /// <returns>The negated matrix.</returns>
    public static ColorMatrix operator -(ColorMatrix value)
    {
        var m = default(ColorMatrix);

        m.M11 = -value.M11;
        m.M12 = -value.M12;
        m.M13 = -value.M13;
        m.M14 = -value.M14;
        m.M21 = -value.M21;
        m.M22 = -value.M22;
        m.M23 = -value.M23;
        m.M24 = -value.M24;
        m.M31 = -value.M31;
        m.M32 = -value.M32;
        m.M33 = -value.M33;
        m.M34 = -value.M34;
        m.M41 = -value.M41;
        m.M42 = -value.M42;
        m.M43 = -value.M43;
        m.M44 = -value.M44;
        m.M51 = -value.M51;
        m.M52 = -value.M52;
        m.M53 = -value.M53;
        m.M54 = -value.M54;

        return m;
    }

    /// <summary>
    ///     Multiplies a matrix by another matrix.
    /// </summary>
    /// <param name="value1">The first source matrix.</param>
    /// <param name="value2">The second source matrix.</param>
    /// <returns>The result of the multiplication.</returns>
    public static ColorMatrix operator *(ColorMatrix value1, ColorMatrix value2)
    {
        var m = default(ColorMatrix);

        // First row
        m.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31 + value1.M14 * value2.M41;
        m.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32 + value1.M14 * value2.M42;
        m.M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33 + value1.M14 * value2.M43;
        m.M14 = value1.M11 * value2.M14 + value1.M12 * value2.M24 + value1.M13 * value2.M34 + value1.M14 * value2.M44;

        // Second row
        m.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31 + value1.M24 * value2.M41;
        m.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32 + value1.M24 * value2.M42;
        m.M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33 + value1.M24 * value2.M43;
        m.M24 = value1.M21 * value2.M14 + value1.M22 * value2.M24 + value1.M23 * value2.M34 + value1.M24 * value2.M44;

        // Third row
        m.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31 + value1.M34 * value2.M41;
        m.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32 + value1.M34 * value2.M42;
        m.M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33 + value1.M34 * value2.M43;
        m.M34 = value1.M31 * value2.M14 + value1.M32 * value2.M24 + value1.M33 * value2.M34 + value1.M34 * value2.M44;

        // Fourth row
        m.M41 = value1.M41 * value2.M11 + value1.M42 * value2.M21 + value1.M43 * value2.M31 + value1.M44 * value2.M41;
        m.M42 = value1.M41 * value2.M12 + value1.M42 * value2.M22 + value1.M43 * value2.M32 + value1.M44 * value2.M42;
        m.M43 = value1.M41 * value2.M13 + value1.M42 * value2.M23 + value1.M43 * value2.M33 + value1.M44 * value2.M43;
        m.M44 = value1.M41 * value2.M14 + value1.M42 * value2.M24 + value1.M43 * value2.M34 + value1.M44 * value2.M44;

        // Fifth row
        m.M51 = value1.M51 * value2.M11 + value1.M52 * value2.M21 + value1.M53 * value2.M31 + value1.M54 * value2.M41 +
                value2.M51;
        m.M52 = value1.M51 * value2.M12 + value1.M52 * value2.M22 + value1.M53 * value2.M32 + value1.M54 * value2.M52 +
                value2.M52;
        m.M53 = value1.M51 * value2.M13 + value1.M52 * value2.M23 + value1.M53 * value2.M33 + value1.M54 * value2.M53 +
                value2.M53;
        m.M54 = value1.M51 * value2.M14 + value1.M52 * value2.M24 + value1.M53 * value2.M34 + value1.M54 * value2.M54 +
                value2.M54;

        return m;
    }

    /// <summary>
    ///     Multiplies a matrix by a scalar value.
    /// </summary>
    /// <param name="value1">The source matrix.</param>
    /// <param name="value2">The scaling factor.</param>
    /// <returns>The scaled matrix.</returns>
    public static ColorMatrix operator *(ColorMatrix value1, float value2)
    {
        var m = default(ColorMatrix);

        m.M11 = value1.M11 * value2;
        m.M12 = value1.M12 * value2;
        m.M13 = value1.M13 * value2;
        m.M14 = value1.M14 * value2;
        m.M21 = value1.M21 * value2;
        m.M22 = value1.M22 * value2;
        m.M23 = value1.M23 * value2;
        m.M24 = value1.M24 * value2;
        m.M31 = value1.M31 * value2;
        m.M32 = value1.M32 * value2;
        m.M33 = value1.M33 * value2;
        m.M34 = value1.M34 * value2;
        m.M41 = value1.M41 * value2;
        m.M42 = value1.M42 * value2;
        m.M43 = value1.M43 * value2;
        m.M44 = value1.M44 * value2;
        m.M51 = value1.M51 * value2;
        m.M52 = value1.M52 * value2;
        m.M53 = value1.M53 * value2;
        m.M54 = value1.M54 * value2;

        return m;
    }

    /// <summary>
    ///     Returns a boolean indicating whether the given two matrices are equal.
    /// </summary>
    /// <param name="value1">The first matrix to compare.</param>
    /// <param name="value2">The second matrix to compare.</param>
    /// <returns>True if the given matrices are equal; False otherwise.</returns>
    public static bool operator ==(ColorMatrix value1, ColorMatrix value2)
    {
        return value1.Equals(value2);
    }

    /// <summary>
    ///     Returns a boolean indicating whether the given two matrices are not equal.
    /// </summary>
    /// <param name="value1">The first matrix to compare.</param>
    /// <param name="value2">The second matrix to compare.</param>
    /// <returns>True if the given matrices are equal; False otherwise.</returns>
    public static bool operator !=(ColorMatrix value1, ColorMatrix value2)
    {
        return !value1.Equals(value2);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is ColorMatrix matrix && Equals(matrix);
    }

    /// <inheritdoc />
    public bool Equals(ColorMatrix other)
    {
        return M11 == other.M11
               && M12 == other.M12
               && M13 == other.M13
               && M14 == other.M14
               && M21 == other.M21
               && M22 == other.M22
               && M23 == other.M23
               && M24 == other.M24
               && M31 == other.M31
               && M32 == other.M32
               && M33 == other.M33
               && M34 == other.M34
               && M41 == other.M41
               && M42 == other.M42
               && M43 == other.M43
               && M44 == other.M44
               && M51 == other.M51
               && M52 == other.M52
               && M53 == other.M53
               && M54 == other.M54;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        HashCode hash = default;
        hash.Add(M11);
        hash.Add(M12);
        hash.Add(M13);
        hash.Add(M14);
        hash.Add(M21);
        hash.Add(M22);
        hash.Add(M23);
        hash.Add(M24);
        hash.Add(M31);
        hash.Add(M32);
        hash.Add(M33);
        hash.Add(M34);
        hash.Add(M41);
        hash.Add(M42);
        hash.Add(M43);
        hash.Add(M44);
        hash.Add(M51);
        hash.Add(M52);
        hash.Add(M53);
        hash.Add(M54);
        return hash.ToHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var ci = CultureInfo.CurrentCulture;

        return string.Format(ci,
            "{{ {{M11:{0} M12:{1} M13:{2} M14:{3}}} {{M21:{4} M22:{5} M23:{6} M24:{7}}} {{M31:{8} M32:{9} M33:{10} M34:{11}}} {{M41:{12} M42:{13} M43:{14} M44:{15}}} {{M51:{16} M52:{17} M53:{18} M54:{19}}} }}",
            M11.ToString(ci), M12.ToString(ci), M13.ToString(ci), M14.ToString(ci),
            M21.ToString(ci), M22.ToString(ci), M23.ToString(ci), M24.ToString(ci),
            M31.ToString(ci), M32.ToString(ci), M33.ToString(ci), M34.ToString(ci),
            M41.ToString(ci), M42.ToString(ci), M43.ToString(ci), M44.ToString(ci),
            M51.ToString(ci), M52.ToString(ci), M53.ToString(ci), M54.ToString(ci));
    }
}