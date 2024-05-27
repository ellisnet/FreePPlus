// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Provides methods to write ICC data types
/// </summary>
internal sealed partial class IccDataWriter
{
    /// <summary>
    ///     Writes a two dimensional matrix
    /// </summary>
    /// <param name="value">The matrix to write</param>
    /// <param name="isSingle">True if the values are encoded as Single; false if encoded as Fix16</param>
    /// <returns>The number of bytes written</returns>
    public int WriteMatrix(Matrix4x4 value, bool isSingle)
    {
        var count = 0;

        if (isSingle)
        {
            count += WriteSingle(value.M11);
            count += WriteSingle(value.M21);
            count += WriteSingle(value.M31);

            count += WriteSingle(value.M12);
            count += WriteSingle(value.M22);
            count += WriteSingle(value.M32);

            count += WriteSingle(value.M13);
            count += WriteSingle(value.M23);
            count += WriteSingle(value.M33);
        }
        else
        {
            count += WriteFix16(value.M11);
            count += WriteFix16(value.M21);
            count += WriteFix16(value.M31);

            count += WriteFix16(value.M12);
            count += WriteFix16(value.M22);
            count += WriteFix16(value.M32);

            count += WriteFix16(value.M13);
            count += WriteFix16(value.M23);
            count += WriteFix16(value.M33);
        }

        return count;
    }

    /// <summary>
    ///     Writes a two dimensional matrix
    /// </summary>
    /// <param name="value">The matrix to write</param>
    /// <param name="isSingle">True if the values are encoded as Single; false if encoded as Fix16</param>
    /// <returns>The number of bytes written</returns>
    public int WriteMatrix(in DenseMatrix<float> value, bool isSingle)
    {
        var count = 0;
        for (var y = 0; y < value.Rows; y++)
        for (var x = 0; x < value.Columns; x++)
            if (isSingle)
                count += WriteSingle(value[x, y]);
            else
                count += WriteFix16(value[x, y]);

        return count;
    }

    /// <summary>
    ///     Writes a two dimensional matrix
    /// </summary>
    /// <param name="value">The matrix to write</param>
    /// <param name="isSingle">True if the values are encoded as Single; false if encoded as Fix16</param>
    /// <returns>The number of bytes written</returns>
    public int WriteMatrix(float[,] value, bool isSingle)
    {
        var count = 0;
        for (var y = 0; y < value.GetLength(1); y++)
        for (var x = 0; x < value.GetLength(0); x++)
            if (isSingle)
                count += WriteSingle(value[x, y]);
            else
                count += WriteFix16(value[x, y]);

        return count;
    }

    /// <summary>
    ///     Writes a one dimensional matrix
    /// </summary>
    /// <param name="value">The matrix to write</param>
    /// <param name="isSingle">True if the values are encoded as Single; false if encoded as Fix16</param>
    /// <returns>The number of bytes written</returns>
    public int WriteMatrix(Vector3 value, bool isSingle)
    {
        var count = 0;
        if (isSingle)
        {
            count += WriteSingle(value.X);
            count += WriteSingle(value.Y);
            count += WriteSingle(value.Z);
        }
        else
        {
            count += WriteFix16(value.X);
            count += WriteFix16(value.Y);
            count += WriteFix16(value.Z);
        }

        return count;
    }

    /// <summary>
    ///     Writes a one dimensional matrix
    /// </summary>
    /// <param name="value">The matrix to write</param>
    /// <param name="isSingle">True if the values are encoded as Single; false if encoded as Fix16</param>
    /// <returns>The number of bytes written</returns>
    public int WriteMatrix(float[] value, bool isSingle)
    {
        var count = 0;
        for (var i = 0; i < value.Length; i++)
            if (isSingle)
                count += WriteSingle(value[i]);
            else
                count += WriteFix16(value[i]);

        return count;
    }
}