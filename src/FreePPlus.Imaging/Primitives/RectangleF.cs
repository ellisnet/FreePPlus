// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Stores a set of four single precision floating points that represent the location and size of a rectangle.
/// </summary>
/// <remarks>
///     This struct is fully mutable. This is done (against the guidelines) for the sake of performance,
///     as it avoids the need to create new values for modification operations.
/// </remarks>
public struct RectangleF : IEquatable<RectangleF>
{
    /// <summary>
    ///     Represents a <see cref="RectangleF" /> that has X, Y, Width, and Height values set to zero.
    /// </summary>
    public static readonly RectangleF Empty = default;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RectangleF" /> struct.
    /// </summary>
    /// <param name="x">The horizontal position of the rectangle.</param>
    /// <param name="y">The vertical position of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    public RectangleF(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RectangleF" /> struct.
    /// </summary>
    /// <param name="point">
    ///     The <see cref="Point" /> which specifies the rectangles point in a two-dimensional plane.
    /// </param>
    /// <param name="size">
    ///     The <see cref="Size" /> which specifies the rectangles height and width.
    /// </param>
    public RectangleF(PointF point, SizeF size)
    {
        X = point.X;
        Y = point.Y;
        Width = size.Width;
        Height = size.Height;
    }

    /// <summary>
    ///     Gets or sets the x-coordinate of this <see cref="RectangleF" />.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    ///     Gets or sets the y-coordinate of this <see cref="RectangleF" />.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    ///     Gets or sets the width of this <see cref="RectangleF" />.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    ///     Gets or sets the height of this <see cref="RectangleF" />.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    ///     Gets or sets the coordinates of the upper-left corner of the rectangular region represented by this
    ///     <see cref="RectangleF" />.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public PointF Location
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(X, Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    /// <summary>
    ///     Gets or sets the size of this <see cref="RectangleF" />.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public SizeF Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Width, Height);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            Width = value.Width;
            Height = value.Height;
        }
    }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="RectangleF" /> is empty.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsEmpty => Width <= 0 || Height <= 0;

    /// <summary>
    ///     Gets the y-coordinate of the top edge of this <see cref="RectangleF" />.
    /// </summary>
    public float Top => Y;

    /// <summary>
    ///     Gets the x-coordinate of the right edge of this <see cref="RectangleF" />.
    /// </summary>
    public float Right
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => X + Width;
    }

    /// <summary>
    ///     Gets the y-coordinate of the bottom edge of this <see cref="RectangleF" />.
    /// </summary>
    public float Bottom
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Y + Height;
    }

    /// <summary>
    ///     Gets the x-coordinate of the left edge of this <see cref="RectangleF" />.
    /// </summary>
    public float Left => X;

    /// <summary>
    ///     Creates a <see cref="Rectangle" /> with the coordinates of the specified <see cref="RectangleF" /> by truncating
    ///     each coordinate.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Rectangle(RectangleF rectangle)
    {
        return Rectangle.Truncate(rectangle);
    }

    /// <summary>
    ///     Compares two <see cref="RectangleF" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="RectangleF" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="RectangleF" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is equal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(RectangleF left, RectangleF right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="RectangleF" /> objects for inequality.
    /// </summary>
    /// <param name="left">The <see cref="RectangleF" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="RectangleF" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is unequal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(RectangleF left, RectangleF right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    ///     Creates a new <see cref="RectangleF" /> with the specified location and size.
    /// </summary>
    /// <param name="left">The left coordinate of the rectangle.</param>
    /// <param name="top">The top coordinate of the rectangle.</param>
    /// <param name="right">The right coordinate of the rectangle.</param>
    /// <param name="bottom">The bottom coordinate of the rectangle.</param>
    /// <returns>The <see cref="RectangleF" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    // ReSharper disable once InconsistentNaming
    public static RectangleF FromLTRB(float left, float top, float right, float bottom)
    {
        return new RectangleF(left, top, right - left, bottom - top);
    }

    /// <summary>
    ///     Returns the center point of the given <see cref="RectangleF" />.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>The <see cref="Point" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PointF Center(RectangleF rectangle)
    {
        return new PointF(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
    }

    /// <summary>
    ///     Creates a rectangle that represents the intersection between <paramref name="a" /> and
    ///     <paramref name="b" />. If there is no intersection, an empty rectangle is returned.
    /// </summary>
    /// <param name="a">The first rectangle.</param>
    /// <param name="b">The second rectangle.</param>
    /// <returns>The <see cref="RectangleF" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectangleF Intersect(RectangleF a, RectangleF b)
    {
        var x1 = MathF.Max(a.X, b.X);
        var x2 = MathF.Min(a.Right, b.Right);
        var y1 = MathF.Max(a.Y, b.Y);
        var y2 = MathF.Min(a.Bottom, b.Bottom);

        if (x2 >= x1 && y2 >= y1) return new RectangleF(x1, y1, x2 - x1, y2 - y1);

        return Empty;
    }

    /// <summary>
    ///     Creates a <see cref="RectangleF" /> that is inflated by the specified amount.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <param name="x">The amount to inflate the width by.</param>
    /// <param name="y">The amount to inflate the height by.</param>
    /// <returns>A new <see cref="RectangleF" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectangleF Inflate(RectangleF rectangle, float x, float y)
    {
        var r = rectangle;
        r.Inflate(x, y);
        return r;
    }

    /// <summary>
    ///     Transforms a rectangle by the given matrix.
    /// </summary>
    /// <param name="rectangle">The source rectangle.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>A transformed <see cref="RectangleF" />.</returns>
    public static RectangleF Transform(RectangleF rectangle, Matrix3x2 matrix)
    {
        var bottomRight = PointF.Transform(new PointF(rectangle.Right, rectangle.Bottom), matrix);
        var topLeft = PointF.Transform(rectangle.Location, matrix);
        return new RectangleF(topLeft, new SizeF(bottomRight - topLeft));
    }

    /// <summary>
    ///     Creates a rectangle that represents the union between <paramref name="a" /> and <paramref name="b" />.
    /// </summary>
    /// <param name="a">The first rectangle.</param>
    /// <param name="b">The second rectangle.</param>
    /// <returns>The <see cref="RectangleF" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RectangleF Union(RectangleF a, RectangleF b)
    {
        var x1 = MathF.Min(a.X, b.X);
        var x2 = MathF.Max(a.Right, b.Right);
        var y1 = MathF.Min(a.Y, b.Y);
        var y2 = MathF.Max(a.Bottom, b.Bottom);

        return new RectangleF(x1, y1, x2 - x1, y2 - y1);
    }

    /// <summary>
    ///     Deconstructs this rectangle into four floats.
    /// </summary>
    /// <param name="x">The out value for X.</param>
    /// <param name="y">The out value for Y.</param>
    /// <param name="width">The out value for the width.</param>
    /// <param name="height">The out value for the height.</param>
    public void Deconstruct(out float x, out float y, out float width, out float height)
    {
        x = X;
        y = Y;
        width = Width;
        height = Height;
    }

    /// <summary>
    ///     Creates a RectangleF that represents the intersection between this RectangleF and the <paramref name="rectangle" />
    ///     .
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Intersect(RectangleF rectangle)
    {
        var result = Intersect(rectangle, this);

        X = result.X;
        Y = result.Y;
        Width = result.Width;
        Height = result.Height;
    }

    /// <summary>
    ///     Inflates this <see cref="RectangleF" /> by the specified amount.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Inflate(float width, float height)
    {
        X -= width;
        Y -= height;

        Width += 2 * width;
        Height += 2 * height;
    }

    /// <summary>
    ///     Inflates this <see cref="RectangleF" /> by the specified amount.
    /// </summary>
    /// <param name="size">The size.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Inflate(SizeF size)
    {
        Inflate(size.Width, size.Height);
    }

    /// <summary>
    ///     Determines if the specfied point is contained within the rectangular region defined by
    ///     this <see cref="RectangleF" />.
    /// </summary>
    /// <param name="x">The x-coordinate of the given point.</param>
    /// <param name="y">The y-coordinate of the given point.</param>
    /// <returns>The <see cref="bool" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(float x, float y)
    {
        return X <= x && x < Right && Y <= y && y < Bottom;
    }

    /// <summary>
    ///     Determines if the specified point is contained within the rectangular region defined by this
    ///     <see cref="RectangleF" /> .
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The <see cref="bool" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(PointF point)
    {
        return Contains(point.X, point.Y);
    }

    /// <summary>
    ///     Determines if the rectangular region represented by <paramref name="rectangle" /> is entirely contained
    ///     within the rectangular region represented by this <see cref="RectangleF" /> .
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>The <see cref="bool" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(RectangleF rectangle)
    {
        return X <= rectangle.X && rectangle.Right <= Right &&
               Y <= rectangle.Y && rectangle.Bottom <= Bottom;
    }

    /// <summary>
    ///     Determines if the specfied <see cref="RectangleF" /> intersects the rectangular region defined by
    ///     this <see cref="RectangleF" />.
    /// </summary>
    /// <param name="rectangle">The other Rectange. </param>
    /// <returns>The <see cref="bool" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IntersectsWith(RectangleF rectangle)
    {
        return rectangle.X < Right && X < rectangle.Right &&
               rectangle.Y < Bottom && Y < rectangle.Bottom;
    }

    /// <summary>
    ///     Adjusts the location of this rectangle by the specified amount.
    /// </summary>
    /// <param name="point">The point.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Offset(PointF point)
    {
        Offset(point.X, point.Y);
    }

    /// <summary>
    ///     Adjusts the location of this rectangle by the specified amount.
    /// </summary>
    /// <param name="dx">The amount to offset the x-coordinate.</param>
    /// <param name="dy">The amount to offset the y-coordinate.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Offset(float dx, float dy)
    {
        X += dx;
        Y += dy;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Width, Height);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"RectangleF [ X={X}, Y={Y}, Width={Width}, Height={Height} ]";
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is RectangleF other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(RectangleF other)
    {
        return X.Equals(other.X) &&
               Y.Equals(other.Y) &&
               Width.Equals(other.Width) &&
               Height.Equals(other.Height);
    }
}