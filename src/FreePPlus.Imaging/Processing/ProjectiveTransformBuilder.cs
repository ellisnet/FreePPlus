// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Numerics;
using FreePPlus.Imaging.Processing.Processors.Transforms;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     A helper class for constructing <see cref="Matrix4x4" /> instances for use in projective transforms.
/// </summary>
public class ProjectiveTransformBuilder
{
    private readonly List<Func<Size, Matrix4x4>> matrixFactories = new();

    /// <summary>
    ///     Prepends a matrix that performs a tapering projective transform.
    /// </summary>
    /// <param name="side">An enumeration that indicates the side of the rectangle that tapers.</param>
    /// <param name="corner">An enumeration that indicates on which corners to taper the rectangle.</param>
    /// <param name="fraction">The amount to taper.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder PrependTaper(TaperSide side, TaperCorner corner, float fraction)
    {
        return Prepend(size => TransformUtilities.CreateTaperMatrix(size, side, corner, fraction));
    }

    /// <summary>
    ///     Appends a matrix that performs a tapering projective transform.
    /// </summary>
    /// <param name="side">An enumeration that indicates the side of the rectangle that tapers.</param>
    /// <param name="corner">An enumeration that indicates on which corners to taper the rectangle.</param>
    /// <param name="fraction">The amount to taper.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder AppendTaper(TaperSide side, TaperCorner corner, float fraction)
    {
        return Append(size => TransformUtilities.CreateTaperMatrix(size, side, corner, fraction));
    }

    /// <summary>
    ///     Prepends a centered rotation matrix using the given rotation in degrees.
    /// </summary>
    /// <param name="degrees">The amount of rotation, in degrees.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder PrependRotationDegrees(float degrees)
    {
        return PrependRotationRadians(GeometryUtilities.DegreeToRadian(degrees));
    }

    /// <summary>
    ///     Prepends a centered rotation matrix using the given rotation in radians.
    /// </summary>
    /// <param name="radians">The amount of rotation, in radians.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder PrependRotationRadians(float radians)
    {
        return Prepend(size => new Matrix4x4(TransformUtilities.CreateRotationMatrixRadians(radians, size)));
    }

    /// <summary>
    ///     Prepends a centered rotation matrix using the given rotation in degrees at the given origin.
    /// </summary>
    /// <param name="degrees">The amount of rotation, in radians.</param>
    /// <param name="origin">The rotation origin point.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    internal ProjectiveTransformBuilder PrependRotationDegrees(float degrees, Vector2 origin)
    {
        return PrependRotationRadians(GeometryUtilities.DegreeToRadian(degrees), origin);
    }

    /// <summary>
    ///     Prepends a centered rotation matrix using the given rotation in radians at the given origin.
    /// </summary>
    /// <param name="radians">The amount of rotation, in radians.</param>
    /// <param name="origin">The rotation origin point.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    internal ProjectiveTransformBuilder PrependRotationRadians(float radians, Vector2 origin)
    {
        return PrependMatrix(Matrix4x4.CreateRotationZ(radians, new Vector3(origin, 0)));
    }

    /// <summary>
    ///     Appends a centered rotation matrix using the given rotation in degrees.
    /// </summary>
    /// <param name="degrees">The amount of rotation, in degrees.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder AppendRotationDegrees(float degrees)
    {
        return AppendRotationRadians(GeometryUtilities.DegreeToRadian(degrees));
    }

    /// <summary>
    ///     Appends a centered rotation matrix using the given rotation in radians.
    /// </summary>
    /// <param name="radians">The amount of rotation, in radians.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder AppendRotationRadians(float radians)
    {
        return Append(size => new Matrix4x4(TransformUtilities.CreateRotationMatrixRadians(radians, size)));
    }

    /// <summary>
    ///     Appends a centered rotation matrix using the given rotation in degrees at the given origin.
    /// </summary>
    /// <param name="degrees">The amount of rotation, in radians.</param>
    /// <param name="origin">The rotation origin point.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    internal ProjectiveTransformBuilder AppendRotationDegrees(float degrees, Vector2 origin)
    {
        return AppendRotationRadians(GeometryUtilities.DegreeToRadian(degrees), origin);
    }

    /// <summary>
    ///     Appends a centered rotation matrix using the given rotation in radians at the given origin.
    /// </summary>
    /// <param name="radians">The amount of rotation, in radians.</param>
    /// <param name="origin">The rotation origin point.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    internal ProjectiveTransformBuilder AppendRotationRadians(float radians, Vector2 origin)
    {
        return AppendMatrix(Matrix4x4.CreateRotationZ(radians, new Vector3(origin, 0)));
    }

    /// <summary>
    ///     Prepends a scale matrix from the given uniform scale.
    /// </summary>
    /// <param name="scale">The uniform scale.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder PrependScale(float scale)
    {
        return PrependMatrix(Matrix4x4.CreateScale(scale));
    }

    /// <summary>
    ///     Prepends a scale matrix from the given vector scale.
    /// </summary>
    /// <param name="scale">The horizontal and vertical scale.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder PrependScale(SizeF scale)
    {
        return PrependScale((Vector2)scale);
    }

    /// <summary>
    ///     Prepends a scale matrix from the given vector scale.
    /// </summary>
    /// <param name="scales">The horizontal and vertical scale.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder PrependScale(Vector2 scales)
    {
        return PrependMatrix(Matrix4x4.CreateScale(new Vector3(scales, 1F)));
    }

    /// <summary>
    ///     Appends a scale matrix from the given uniform scale.
    /// </summary>
    /// <param name="scale">The uniform scale.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder AppendScale(float scale)
    {
        return AppendMatrix(Matrix4x4.CreateScale(scale));
    }

    /// <summary>
    ///     Appends a scale matrix from the given vector scale.
    /// </summary>
    /// <param name="scales">The horizontal and vertical scale.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder AppendScale(SizeF scales)
    {
        return AppendScale((Vector2)scales);
    }

    /// <summary>
    ///     Appends a scale matrix from the given vector scale.
    /// </summary>
    /// <param name="scales">The horizontal and vertical scale.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder AppendScale(Vector2 scales)
    {
        return AppendMatrix(Matrix4x4.CreateScale(new Vector3(scales, 1F)));
    }

    /// <summary>
    ///     Prepends a centered skew matrix from the give angles in degrees.
    /// </summary>
    /// <param name="degreesX">The X angle, in degrees.</param>
    /// <param name="degreesY">The Y angle, in degrees.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    internal ProjectiveTransformBuilder PrependSkewDegrees(float degreesX, float degreesY)
    {
        return PrependSkewRadians(GeometryUtilities.DegreeToRadian(degreesX),
            GeometryUtilities.DegreeToRadian(degreesY));
    }

    /// <summary>
    ///     Prepends a centered skew matrix from the give angles in radians.
    /// </summary>
    /// <param name="radiansX">The X angle, in radians.</param>
    /// <param name="radiansY">The Y angle, in radians.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder PrependSkewRadians(float radiansX, float radiansY)
    {
        return Prepend(size => new Matrix4x4(TransformUtilities.CreateSkewMatrixRadians(radiansX, radiansY, size)));
    }

    /// <summary>
    ///     Prepends a skew matrix using the given angles in degrees at the given origin.
    /// </summary>
    /// <param name="degreesX">The X angle, in degrees.</param>
    /// <param name="degreesY">The Y angle, in degrees.</param>
    /// <param name="origin">The skew origin point.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder PrependSkewDegrees(float degreesX, float degreesY, Vector2 origin)
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
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder PrependSkewRadians(float radiansX, float radiansY, Vector2 origin)
    {
        return PrependMatrix(new Matrix4x4(Matrix3x2.CreateSkew(radiansX, radiansY, origin)));
    }

    /// <summary>
    ///     Appends a centered skew matrix from the give angles in degrees.
    /// </summary>
    /// <param name="degreesX">The X angle, in degrees.</param>
    /// <param name="degreesY">The Y angle, in degrees.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    internal ProjectiveTransformBuilder AppendSkewDegrees(float degreesX, float degreesY)
    {
        return AppendSkewRadians(GeometryUtilities.DegreeToRadian(degreesX),
            GeometryUtilities.DegreeToRadian(degreesY));
    }

    /// <summary>
    ///     Appends a centered skew matrix from the give angles in radians.
    /// </summary>
    /// <param name="radiansX">The X angle, in radians.</param>
    /// <param name="radiansY">The Y angle, in radians.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder AppendSkewRadians(float radiansX, float radiansY)
    {
        return Append(size => new Matrix4x4(TransformUtilities.CreateSkewMatrixRadians(radiansX, radiansY, size)));
    }

    /// <summary>
    ///     Appends a skew matrix using the given angles in degrees at the given origin.
    /// </summary>
    /// <param name="degreesX">The X angle, in degrees.</param>
    /// <param name="degreesY">The Y angle, in degrees.</param>
    /// <param name="origin">The skew origin point.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder AppendSkewDegrees(float degreesX, float degreesY, Vector2 origin)
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
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder AppendSkewRadians(float radiansX, float radiansY, Vector2 origin)
    {
        return AppendMatrix(new Matrix4x4(Matrix3x2.CreateSkew(radiansX, radiansY, origin)));
    }

    /// <summary>
    ///     Prepends a translation matrix from the given vector.
    /// </summary>
    /// <param name="position">The translation position.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder PrependTranslation(PointF position)
    {
        return PrependTranslation((Vector2)position);
    }

    /// <summary>
    ///     Prepends a translation matrix from the given vector.
    /// </summary>
    /// <param name="position">The translation position.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder PrependTranslation(Vector2 position)
    {
        return PrependMatrix(Matrix4x4.CreateTranslation(new Vector3(position, 0)));
    }

    /// <summary>
    ///     Appends a translation matrix from the given vector.
    /// </summary>
    /// <param name="position">The translation position.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder AppendTranslation(PointF position)
    {
        return AppendTranslation((Vector2)position);
    }

    /// <summary>
    ///     Appends a translation matrix from the given vector.
    /// </summary>
    /// <param name="position">The translation position.</param>
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder AppendTranslation(Vector2 position)
    {
        return AppendMatrix(Matrix4x4.CreateTranslation(new Vector3(position, 0)));
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
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder PrependMatrix(Matrix4x4 matrix)
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
    /// <returns>The <see cref="ProjectiveTransformBuilder" />.</returns>
    public ProjectiveTransformBuilder AppendMatrix(Matrix4x4 matrix)
    {
        CheckDegenerate(matrix);
        return Append(_ => matrix);
    }

    /// <summary>
    ///     Returns the combined matrix for a given source size.
    /// </summary>
    /// <param name="sourceSize">The source image size.</param>
    /// <returns>The <see cref="Matrix4x4" />.</returns>
    public Matrix4x4 BuildMatrix(Size sourceSize)
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
    /// <returns>The <see cref="Matrix4x4" />.</returns>
    public Matrix4x4 BuildMatrix(Rectangle sourceRectangle)
    {
        Guard.MustBeGreaterThan(sourceRectangle.Width, 0, nameof(sourceRectangle));
        Guard.MustBeGreaterThan(sourceRectangle.Height, 0, nameof(sourceRectangle));

        // Translate the origin matrix to cater for source rectangle offsets.
        var matrix = Matrix4x4.CreateTranslation(new Vector3(-sourceRectangle.Location, 0));

        var size = sourceRectangle.Size;

        foreach (var factory in matrixFactories) matrix *= factory(size);

        CheckDegenerate(matrix);

        return matrix;
    }

    private static void CheckDegenerate(Matrix4x4 matrix)
    {
        if (TransformUtilities.IsDegenerate(matrix))
            throw new DegenerateTransformException("Matrix is degenerate. Check input values.");
    }

    private ProjectiveTransformBuilder Prepend(Func<Size, Matrix4x4> factory)
    {
        matrixFactories.Insert(0, factory);
        return this;
    }

    private ProjectiveTransformBuilder Append(Func<Size, Matrix4x4> factory)
    {
        matrixFactories.Add(factory);
        return this;
    }
}