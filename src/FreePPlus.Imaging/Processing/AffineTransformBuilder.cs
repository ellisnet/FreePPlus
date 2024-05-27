// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Numerics;
using FreePPlus.Imaging.Processing.Processors.Transforms;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     A helper class for constructing <see cref="Matrix3x2" /> instances for use in affine transforms.
/// </summary>
public class AffineTransformBuilder
{
    private readonly List<Func<Size, Matrix3x2>> matrixFactories = new();

    /// <summary>
    ///     Prepends a rotation matrix using the given rotation angle in degrees
    ///     and the image center point as rotation center.
    /// </summary>
    /// <param name="degrees">The amount of rotation, in degrees.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependRotationDegrees(float degrees)
    {
        return PrependRotationRadians(GeometryUtilities.DegreeToRadian(degrees));
    }

    /// <summary>
    ///     Prepends a rotation matrix using the given rotation angle in radians
    ///     and the image center point as rotation center.
    /// </summary>
    /// <param name="radians">The amount of rotation, in radians.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependRotationRadians(float radians)
    {
        return Prepend(size => TransformUtilities.CreateRotationMatrixRadians(radians, size));
    }

    /// <summary>
    ///     Prepends a rotation matrix using the given rotation in degrees at the given origin.
    /// </summary>
    /// <param name="degrees">The amount of rotation, in degrees.</param>
    /// <param name="origin">The rotation origin point.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependRotationDegrees(float degrees, Vector2 origin)
    {
        return PrependRotationRadians(GeometryUtilities.DegreeToRadian(degrees), origin);
    }

    /// <summary>
    ///     Prepends a rotation matrix using the given rotation in radians at the given origin.
    /// </summary>
    /// <param name="radians">The amount of rotation, in radians.</param>
    /// <param name="origin">The rotation origin point.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependRotationRadians(float radians, Vector2 origin)
    {
        return PrependMatrix(Matrix3x2.CreateRotation(radians, origin));
    }

    /// <summary>
    ///     Appends a rotation matrix using the given rotation angle in degrees
    ///     and the image center point as rotation center.
    /// </summary>
    /// <param name="degrees">The amount of rotation, in degrees.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendRotationDegrees(float degrees)
    {
        return AppendRotationRadians(GeometryUtilities.DegreeToRadian(degrees));
    }

    /// <summary>
    ///     Appends a rotation matrix using the given rotation angle in radians
    ///     and the image center point as rotation center.
    /// </summary>
    /// <param name="radians">The amount of rotation, in radians.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendRotationRadians(float radians)
    {
        return Append(size => TransformUtilities.CreateRotationMatrixRadians(radians, size));
    }

    /// <summary>
    ///     Appends a rotation matrix using the given rotation in degrees at the given origin.
    /// </summary>
    /// <param name="degrees">The amount of rotation, in degrees.</param>
    /// <param name="origin">The rotation origin point.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendRotationDegrees(float degrees, Vector2 origin)
    {
        return AppendRotationRadians(GeometryUtilities.DegreeToRadian(degrees), origin);
    }

    /// <summary>
    ///     Appends a rotation matrix using the given rotation in radians at the given origin.
    /// </summary>
    /// <param name="radians">The amount of rotation, in radians.</param>
    /// <param name="origin">The rotation origin point.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendRotationRadians(float radians, Vector2 origin)
    {
        return AppendMatrix(Matrix3x2.CreateRotation(radians, origin));
    }

    /// <summary>
    ///     Prepends a scale matrix from the given uniform scale.
    /// </summary>
    /// <param name="scale">The uniform scale.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependScale(float scale)
    {
        return PrependMatrix(Matrix3x2.CreateScale(scale));
    }

    /// <summary>
    ///     Prepends a scale matrix from the given vector scale.
    /// </summary>
    /// <param name="scale">The horizontal and vertical scale.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependScale(SizeF scale)
    {
        return PrependScale((Vector2)scale);
    }

    /// <summary>
    ///     Prepends a scale matrix from the given vector scale.
    /// </summary>
    /// <param name="scales">The horizontal and vertical scale.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependScale(Vector2 scales)
    {
        return PrependMatrix(Matrix3x2.CreateScale(scales));
    }

    /// <summary>
    ///     Appends a scale matrix from the given uniform scale.
    /// </summary>
    /// <param name="scale">The uniform scale.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendScale(float scale)
    {
        return AppendMatrix(Matrix3x2.CreateScale(scale));
    }

    /// <summary>
    ///     Appends a scale matrix from the given vector scale.
    /// </summary>
    /// <param name="scales">The horizontal and vertical scale.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendScale(SizeF scales)
    {
        return AppendScale((Vector2)scales);
    }

    /// <summary>
    ///     Appends a scale matrix from the given vector scale.
    /// </summary>
    /// <param name="scales">The horizontal and vertical scale.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendScale(Vector2 scales)
    {
        return AppendMatrix(Matrix3x2.CreateScale(scales));
    }

    /// <summary>
    ///     Prepends a centered skew matrix from the give angles in degrees.
    /// </summary>
    /// <param name="degreesX">The X angle, in degrees.</param>
    /// <param name="degreesY">The Y angle, in degrees.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependSkewDegrees(float degreesX, float degreesY)
    {
        return Prepend(size => TransformUtilities.CreateSkewMatrixDegrees(degreesX, degreesY, size));
    }

    /// <summary>
    ///     Prepends a centered skew matrix from the give angles in radians.
    /// </summary>
    /// <param name="radiansX">The X angle, in radians.</param>
    /// <param name="radiansY">The Y angle, in radians.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependSkewRadians(float radiansX, float radiansY)
    {
        return Prepend(size => TransformUtilities.CreateSkewMatrixRadians(radiansX, radiansY, size));
    }

    /// <summary>
    ///     Prepends a skew matrix using the given angles in degrees at the given origin.
    /// </summary>
    /// <param name="degreesX">The X angle, in degrees.</param>
    /// <param name="degreesY">The Y angle, in degrees.</param>
    /// <param name="origin">The skew origin point.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependSkewDegrees(float degreesX, float degreesY, Vector2 origin)
    {
        return PrependSkewRadians(GeometryUtilities.DegreeToRadian(degreesX),
            GeometryUtilities.DegreeToRadian(degreesY), origin);
    }

    /// <summary>
    ///     Prepends a skew matrix using the given angles in radians at the given origin.
    /// </summary>
    /// <param name="radiansX">The X angle, in radians.</param>
    /// <param name="radiansY">The Y angle, in radians.</param>
    /// <param name="origin">The skew origin point.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependSkewRadians(float radiansX, float radiansY, Vector2 origin)
    {
        return PrependMatrix(Matrix3x2.CreateSkew(radiansX, radiansY, origin));
    }

    /// <summary>
    ///     Appends a centered skew matrix from the give angles in degrees.
    /// </summary>
    /// <param name="degreesX">The X angle, in degrees.</param>
    /// <param name="degreesY">The Y angle, in degrees.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendSkewDegrees(float degreesX, float degreesY)
    {
        return Append(size => TransformUtilities.CreateSkewMatrixDegrees(degreesX, degreesY, size));
    }

    /// <summary>
    ///     Appends a centered skew matrix from the give angles in radians.
    /// </summary>
    /// <param name="radiansX">The X angle, in radians.</param>
    /// <param name="radiansY">The Y angle, in radians.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendSkewRadians(float radiansX, float radiansY)
    {
        return Append(size => TransformUtilities.CreateSkewMatrixRadians(radiansX, radiansY, size));
    }

    /// <summary>
    ///     Appends a skew matrix using the given angles in degrees at the given origin.
    /// </summary>
    /// <param name="degreesX">The X angle, in degrees.</param>
    /// <param name="degreesY">The Y angle, in degrees.</param>
    /// <param name="origin">The skew origin point.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendSkewDegrees(float degreesX, float degreesY, Vector2 origin)
    {
        return AppendSkewRadians(GeometryUtilities.DegreeToRadian(degreesX), GeometryUtilities.DegreeToRadian(degreesY),
            origin);
    }

    /// <summary>
    ///     Appends a skew matrix using the given angles in radians at the given origin.
    /// </summary>
    /// <param name="radiansX">The X angle, in radians.</param>
    /// <param name="radiansY">The Y angle, in radians.</param>
    /// <param name="origin">The skew origin point.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendSkewRadians(float radiansX, float radiansY, Vector2 origin)
    {
        return AppendMatrix(Matrix3x2.CreateSkew(radiansX, radiansY, origin));
    }

    /// <summary>
    ///     Prepends a translation matrix from the given vector.
    /// </summary>
    /// <param name="position">The translation position.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependTranslation(PointF position)
    {
        return PrependTranslation((Vector2)position);
    }

    /// <summary>
    ///     Prepends a translation matrix from the given vector.
    /// </summary>
    /// <param name="position">The translation position.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependTranslation(Vector2 position)
    {
        return PrependMatrix(Matrix3x2.CreateTranslation(position));
    }

    /// <summary>
    ///     Appends a translation matrix from the given vector.
    /// </summary>
    /// <param name="position">The translation position.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendTranslation(PointF position)
    {
        return AppendTranslation((Vector2)position);
    }

    /// <summary>
    ///     Appends a translation matrix from the given vector.
    /// </summary>
    /// <param name="position">The translation position.</param>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendTranslation(Vector2 position)
    {
        return AppendMatrix(Matrix3x2.CreateTranslation(position));
    }

    /// <summary>
    ///     Prepends a raw matrix.
    /// </summary>
    /// <param name="matrix">The matrix to prepend.</param>
    /// <exception cref="DegenerateTransformException">
    ///     The resultant matrix is degenerate containing one or more values equivalent
    ///     to <see cref="float.NaN" /> or a zero determinant and therefore cannot be used
    ///     for linear transforms.
    /// </exception>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder PrependMatrix(Matrix3x2 matrix)
    {
        CheckDegenerate(matrix);
        return Prepend(_ => matrix);
    }

    /// <summary>
    ///     Appends a raw matrix.
    /// </summary>
    /// <param name="matrix">The matrix to append.</param>
    /// <exception cref="DegenerateTransformException">
    ///     The resultant matrix is degenerate containing one or more values equivalent
    ///     to <see cref="float.NaN" /> or a zero determinant and therefore cannot be used
    ///     for linear transforms.
    /// </exception>
    /// <returns>The <see cref="AffineTransformBuilder" />.</returns>
    public AffineTransformBuilder AppendMatrix(Matrix3x2 matrix)
    {
        CheckDegenerate(matrix);
        return Append(_ => matrix);
    }

    /// <summary>
    ///     Returns the combined matrix for a given source size.
    /// </summary>
    /// <param name="sourceSize">The source image size.</param>
    /// <returns>The <see cref="Matrix3x2" />.</returns>
    public Matrix3x2 BuildMatrix(Size sourceSize)
    {
        return BuildMatrix(new Rectangle(Point.Empty, sourceSize));
    }

    /// <summary>
    ///     Returns the combined matrix for a given source rectangle.
    /// </summary>
    /// <param name="sourceRectangle">The rectangle in the source image.</param>
    /// <exception cref="DegenerateTransformException">
    ///     The resultant matrix is degenerate containing one or more values equivalent
    ///     to <see cref="float.NaN" /> or a zero determinant and therefore cannot be used
    ///     for linear transforms.
    /// </exception>
    /// <returns>The <see cref="Matrix3x2" />.</returns>
    public Matrix3x2 BuildMatrix(Rectangle sourceRectangle)
    {
        Guard.MustBeGreaterThan(sourceRectangle.Width, 0, nameof(sourceRectangle));
        Guard.MustBeGreaterThan(sourceRectangle.Height, 0, nameof(sourceRectangle));

        // Translate the origin matrix to cater for source rectangle offsets.
        var matrix = Matrix3x2.CreateTranslation(-sourceRectangle.Location);

        var size = sourceRectangle.Size;

        foreach (var factory in matrixFactories) matrix *= factory(size);

        CheckDegenerate(matrix);

        return matrix;
    }

    private static void CheckDegenerate(Matrix3x2 matrix)
    {
        if (TransformUtilities.IsDegenerate(matrix))
            throw new DegenerateTransformException("Matrix is degenerate. Check input values.");
    }

    private AffineTransformBuilder Prepend(Func<Size, Matrix3x2> factory)
    {
        matrixFactories.Insert(0, factory);
        return this;
    }

    private AffineTransformBuilder Append(Func<Size, Matrix3x2> factory)
    {
        matrixFactories.Add(factory);
        return this;
    }
}