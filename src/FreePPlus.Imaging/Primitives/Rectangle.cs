// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Stores a set of four integers that represent the location and size of a rectangle.
/// </summary>
/// <remarks>
///     This struct is fully mutable. This is done (against the guidelines) for the sake of performance,
///     as it avoids the need to create new values for modification operations.
/// </remarks>
public struct Rectangle : IEquatable<Rectangle>
{
    /// <summary>
    ///     Represents a <see cref="Rectangle" /> that has X, Y, Width, and Height values set to zero.
    /// </summary>
    public static readonly Rectangle Empty = default;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rectangle" /> struct.
    /// </summary>
    /// <param name="x">The horizontal position of the rectangle.</param>
    /// <param name="y">The vertical position of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    public Rectangle(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rectangle" /> struct.
    /// </summary>
    /// <param name="point">
    ///     The <see cref="Point" /> which specifies the rectangles point in a two-dimensional plane.
    /// </param>
    /// <param name="size">
    ///     The <see cref="Size" /> which specifies the rectangles height and width.
    /// </param>
    public Rectangle(Point point, Size size)
    {
        X = point.X;
        Y = point.Y;
        Width = size.Width;
        Height = size.Height;
    }

    /// <summary>
    ///     Gets or sets the x-coordinate of this <see cref="Rectangle" />.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///     Gets or sets the y-coordinate of this <see cref="Rectangle" />.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    ///     Gets or sets the width of this <see cref="Rectangle" />.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     Gets or sets the height of this <see cref="Rectangle" />.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     Gets or sets the coordinates of the upper-left corner of the rectangular region represented by this
    ///     <see cref="Rectangle" />.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Point Location
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
    ///     Gets or sets the size of this <see cref="Rectangle" />.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Size Size
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
    ///     Gets a value indicating whether this <see cref="Rectangle" /> is empty.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsEmpty => Equals(Empty);

    /// <summary>
    ///     Gets the y-coordinate of the top edge of this <see cref="Rectangle" />.
    /// </summary>
    public int Top => Y;

    /// <summary>
    ///     Gets the x-coordinate of the right edge of this <see cref="Rectangle" />.
    /// </summary>
    public int Right
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => unchecked(X + Width);
    }

    /// <summary>
    ///     Gets the y-coordinate of the bottom edge of this <see cref="Rectangle" />.
    /// </summary>
    public int Bottom
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => unchecked(Y + Height);
    }

    /// <summary>
    ///     Gets the x-coordinate of the left edge of this <see cref="Rectangle" />.
    /// </summary>
    public int Left => X;

    /// <summary>
    ///     Creates a <see cref="RectangleF" /> with the coordinates of the specified <see cref="Rectangle" />.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator RectangleF(Rectangle rectangle)
    {
        return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    }

    /// <summary>
    ///     Creates a <see cref="Vector4" /> with the coordinates of the specified <see cref="Rectangle" />.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector4(Rectangle rectangle)
    {
        return new Vector4(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    }

    /// <summary>
    ///     Compares two <see cref="Rectangle" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Rectangle" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rectangle" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is equal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Rectangle left, Rectangle right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="Rectangle" /> objects for inequality.
    /// </summary>
    /// <param name="left">The <see cref="Rectangle" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rectangle" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is unequal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Rectangle left, Rectangle right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    ///     Creates a new <see cref="Rectangle" /> with the specified location and size.
    /// </summary>
    /// <param name="left">The left coordinate of the rectangle.</param>
    /// <param name="top">The top coordinate of the rectangle.</param>
    /// <param name="right">The right coordinate of the rectangle.</param>
    /// <param name="bottom">The bottom coordinate of the rectangle.</param>
    /// <returns>The <see cref="Rectangle" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    // ReSharper disable once InconsistentNaming
    public static Rectangle FromLTRB(int left, int top, int right, int bottom)
    {
        return new Rectangle(left, top, unchecked(right - left), unchecked(bottom - top));
    }

    /// <summary>
    ///     Returns the center point of the given <see cref="Rectangle" />.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>The <see cref="Point" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point Center(Rectangle rectangle)
    {
        return new Point(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
    }

    /// <summary>
    ///     Creates a rectangle that represents the intersection between <paramref name="a" /> and
    ///     <paramref name="b" />. If there is no intersection, an empty rectangle is returned.
    /// </summary>
    /// <param name="a">The first rectangle.</param>
    /// <param name="b">The second rectangle.</param>
    /// <returns>The <see cref="Rectangle" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle Intersect(Rectangle a, Rectangle b)
    {
        var x1 = Math.Max(a.X, b.X);
        var x2 = Math.Min(a.Right, b.Right);
        var y1 = Math.Max(a.Y, b.Y);
        var y2 = Math.Min(a.Bottom, b.Bottom);

        if (x2 >= x1 && y2 >= y1) return new Rectangle(x1, y1, x2 - x1, y2 - y1);

        return Empty;
    }

    /// <summary>
    ///     Creates a <see cref="Rectangle" /> that is inflated by the specified amount.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <param name="x">The amount to inflate the width by.</param>
    /// <param name="y">The amount to inflate the height by.</param>
    /// <returns>A new <see cref="Rectangle" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle Inflate(Rectangle rectangle, int x, int y)
    {
        var r = rectangle;
        r.Inflate(x, y);
        return r;
    }

    /// <summary>
    ///     Converts a <see cref="RectangleF" /> to a <see cref="Rectangle" /> by performing a ceiling operation on all the
    ///     coordinates.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>The <see cref="Rectangle" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle Ceiling(RectangleF rectangle)
    {
        unchecked
        {
            return new Rectangle(
                (int)MathF.Ceiling(rectangle.X),
                (int)MathF.Ceiling(rectangle.Y),
                (int)MathF.Ceiling(rectangle.Width),
                (int)MathF.Ceiling(rectangle.Height));
        }
    }

    /// <summary>
    ///     Transforms a rectangle by the given matrix.
    /// </summary>
    /// <param name="rectangle">The source rectangle.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>A transformed rectangle.</returns>
    public static RectangleF Transform(Rectangle rectangle, Matrix3x2 matrix)
    {
        PointF bottomRight = Point.Transform(new Point(rectangle.Right, rectangle.Bottom), matrix);
        PointF topLeft = Point.Transform(rectangle.Location, matrix);
        return new RectangleF(topLeft, new SizeF(bottomRight - topLeft));
    }

    /// <summary>
    ///     Converts a <see cref="RectangleF" /> to a <see cref="Rectangle" /> by performing a truncate operation on all the
    ///     coordinates.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>The <see cref="Rectangle" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle Truncate(RectangleF rectangle)
    {
        unchecked
        {
            return new Rectangle(
                (int)rectangle.X,
                (int)rectangle.Y,
                (int)rectangle.Width,
                (int)rectangle.Height);
        }
    }

    /// <summary>
    ///     Converts a <see cref="RectangleF" /> to a <see cref="Rectangle" /> by performing a round operation on all the
    ///     coordinates.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>The <see cref="Rectangle" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle Round(RectangleF rectangle)
    {
        unchecked
        {
            return new Rectangle(
                (int)MathF.Round(rectangle.X),
                (int)MathF.Round(rectangle.Y),
                (int)MathF.Round(rectangle.Width),
                (int)MathF.Round(rectangle.Height));
        }
    }

    /// <summary>
    ///     Creates a rectangle that represents the union between <paramref name="a" /> and <paramref name="b" />.
    /// </summary>
    /// <param name="a">The first rectangle.</param>
    /// <param name="b">The second rectangle.</param>
    /// <returns>The <see cref="Rectangle" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle Union(Rectangle a, Rectangle b)
    {
        var x1 = Math.Min(a.X, b.X);
        var x2 = Math.Max(a.Right, b.Right);
        var y1 = Math.Min(a.Y, b.Y);
        var y2 = Math.Max(a.Bottom, b.Bottom);

        return new Rectangle(x1, y1, x2 - x1, y2 - y1);
    }

    /// <summary>
    ///     Deconstructs this rectangle into four integers.
    /// </summary>
    /// <param name="x">The out value for X.</param>
    /// <param name="y">The out value for Y.</param>
    /// <param name="width">The out value for the width.</param>
    /// <param name="height">The out value for the height.</param>
    public void Deconstruct(out int x, out int y, out int width, out int height)
    {
        x = X;
        y = Y;
        width = Width;
        height = Height;
    }

    /// <summary>
    ///     Creates a Rectangle that represents the intersection between this Rectangle and the <paramref name="rectangle" />.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Intersect(Rectangle rectangle)
    {
        var result = Intersect(rectangle, this);

        X = result.X;
        Y = result.Y;
        Width = result.Width;
        Height = result.Height;
    }

    /// <summary>
    ///     Inflates this <see cref="Rectangle" /> by the specified amount.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Inflate(int width, int height)
    {
        unchecked
        {
            X -= width;
            Y -= height;

            Width += 2 * width;
            Height += 2 * height;
        }
    }

    /// <summary>
    ///     Inflates this <see cref="Rectangle" /> by the specified amount.
    /// </summary>
    /// <param name="size">The size.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Inflate(Size size)
    {
        Inflate(size.Width, size.Height);
    }

    /// <summary>
    ///     Determines if the specfied point is contained within the rectangular region defined by
    ///     this <see cref="Rectangle" />.
    /// </summary>
    /// <param name="x">The x-coordinate of the given point.</param>
    /// <param name="y">The y-coordinate of the given point.</param>
    /// <returns>The <see cref="bool" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(int x, int y)
    {
        return X <= x && x < Right && Y <= y && y < Bottom;
    }

    /// <summary>
    ///     Determines if the specified point is contained within the rectangular region defined by this
    ///     <see cref="Rectangle" /> .
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The <see cref="bool" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Point point)
    {
        return Contains(point.X, point.Y);
    }

    /// <summary>
    ///     Determines if the rectangular region represented by <paramref name="rectangle" /> is entirely contained
    ///     within the rectangular region represented by this <see cref="Rectangle" /> .
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>The <see cref="bool" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Rectangle rectangle)
    {
        return X <= rectangle.X && rectangle.Right <= Right &&
               Y <= rectangle.Y && rectangle.Bottom <= Bottom;
    }

    /// <summary>
    ///     Determines if the specfied <see cref="Rectangle" /> intersects the rectangular region defined by
    ///     this <see cref="Rectangle" />.
    /// </summary>
    /// <param name="rectangle">The other Rectange. </param>
    /// <returns>The <see cref="bool" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IntersectsWith(Rectangle rectangle)
    {
        return rectangle.X < Right && X < rectangle.Right &&
               rectangle.Y < Bottom && Y < rectangle.Bottom;
    }

    /// <summary>
    ///     Adjusts the location of this rectangle by the specified amount.
    /// </summary>
    /// <param name="point">The point.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Offset(Point point)
    {
        Offset(point.X, point.Y);
    }

    /// <summary>
    ///     Adjusts the location of this rectangle by the specified amount.
    /// </summary>
    /// <param name="dx">The amount to offset the x-coordinate.</param>
    /// <param name="dy">The amount to offset the y-coordinate.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Offset(int dx, int dy)
    {
        unchecked
        {
            X += dx;
            Y += dy;
        }
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Width, Height);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Rectangle [ X={X}, Y={Y}, Width={Width}, Height={Height} ]";
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is Rectangle other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Rectangle other)
    {
        return X.Equals(other.X) &&
               Y.Equals(other.Y) &&
               Width.Equals(other.Width) &&
               Height.Equals(other.Height);
    }
}