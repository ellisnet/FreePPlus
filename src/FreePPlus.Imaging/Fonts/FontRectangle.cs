// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Stores a set of four single precision floating points that represent the location and size of a rectangle.
/// </summary>
/// <remarks>
///     This struct is fully mutable. This is done (against the guidelines) for the sake of performance,
///     as it avoids the need to create new values for modification operations.
/// </remarks>
public readonly struct FontRectangle : IEquatable<FontRectangle>
{
    /// <summary>
    ///     Represents a <see cref="FontRectangle" /> that has X, Y, Width, and Height values set to zero.
    /// </summary>
    public static readonly FontRectangle Empty = default;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FontRectangle" /> struct.
    /// </summary>
    /// <param name="x">The horizontal position of the rectangle.</param>
    /// <param name="y">The vertical position of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    public FontRectangle(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FontRectangle" /> struct.
    /// </summary>
    /// <param name="point">
    ///     The <see cref="Vector2" /> which specifies the rectangles point in a two-dimensional plane.
    /// </param>
    /// <param name="size">
    ///     The <see cref="Vector2" /> which specifies the rectangles height and width.
    /// </param>
    public FontRectangle(Vector2 point, Vector2 size)
        : this(point.X, point.Y, size.X, size.Y) { }

    /// <summary>
    ///     Gets the x-coordinate of this <see cref="FontRectangle" />.
    /// </summary>
    public float X { get; }

    /// <summary>
    ///     Gets the y-coordinate of this <see cref="FontRectangle" />.
    /// </summary>
    public float Y { get; }

    /// <summary>
    ///     Gets the width of this <see cref="FontRectangle" />.
    /// </summary>
    public float Width { get; }

    /// <summary>
    ///     Gets the height of this <see cref="FontRectangle" />.
    /// </summary>
    public float Height { get; }

    /// <summary>
    ///     Gets the coordinates of the upper-left corner of the rectangular region represented by this
    ///     <see cref="FontRectangle" />.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly Vector2 Location
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(X, Y);
    }

    /// <summary>
    ///     Gets the size of this <see cref="FontRectangle" />.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly Vector2 Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Width, Height);
    }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="FontRectangle" /> is empty.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly bool IsEmpty => Width <= 0 || Height <= 0;

    /// <summary>
    ///     Gets the y-coordinate of the top edge of this <see cref="FontRectangle" />.
    /// </summary>
    public readonly float Top => Y;

    /// <summary>
    ///     Gets the x-coordinate of the right edge of this <see cref="FontRectangle" />.
    /// </summary>
    public float Right
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => X + Width;
    }

    /// <summary>
    ///     Gets the y-coordinate of the bottom edge of this <see cref="FontRectangle" />.
    /// </summary>
    public float Bottom
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Y + Height;
    }

    /// <summary>
    ///     Gets the x-coordinate of the left edge of this <see cref="FontRectangle" />.
    /// </summary>
    public float Left => X;

    /// <summary>
    ///     Compares two <see cref="FontRectangle" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="FontRectangle" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="FontRectangle" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is equal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(FontRectangle left, FontRectangle right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="FontRectangle" /> objects for inequality.
    /// </summary>
    /// <param name="left">The <see cref="FontRectangle" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="FontRectangle" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the current left is unequal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(FontRectangle left, FontRectangle right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    ///     Creates a new <see cref="FontRectangle" /> with the specified location and size.
    /// </summary>
    /// <param name="left">The left coordinate of the rectangle.</param>
    /// <param name="top">The top coordinate of the rectangle.</param>
    /// <param name="right">The right coordinate of the rectangle.</param>
    /// <param name="bottom">The bottom coordinate of the rectangle.</param>
    /// <returns>The <see cref="FontRectangle" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    // ReSharper disable once InconsistentNaming
    public static FontRectangle FromLTRB(float left, float top, float right, float bottom)
    {
        return new FontRectangle(left, top, right - left, bottom - top);
    }

    /// <summary>
    ///     Returns the center point of the given <see cref="FontRectangle" />.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>The <see cref="Vector2" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Center(FontRectangle rectangle)
    {
        return new Vector2(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
    }

    /// <summary>
    ///     Creates a rectangle that represents the intersection between <paramref name="a" /> and
    ///     <paramref name="b" />. If there is no intersection, an empty rectangle is returned.
    /// </summary>
    /// <param name="a">The first rectangle.</param>
    /// <param name="b">The second rectangle.</param>
    /// <returns>The <see cref="FontRectangle" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FontRectangle Intersect(FontRectangle a, FontRectangle b)
    {
        var x1 = MathF.Max(a.X, b.X);
        var x2 = MathF.Min(a.Right, b.Right);
        var y1 = MathF.Max(a.Y, b.Y);
        var y2 = MathF.Min(a.Bottom, b.Bottom);

        if (x2 >= x1 && y2 >= y1) return new FontRectangle(x1, y1, x2 - x1, y2 - y1);

        return Empty;
    }

    /// <summary>
    ///     Creates a new <see cref="FontRectangle" /> from the given <paramref name="rectangle" />
    ///     that is inflated by the specified amount.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <param name="x">The amount to inflate the width by.</param>
    /// <param name="y">The amount to inflate the height by.</param>
    /// <returns>A new <see cref="FontRectangle" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FontRectangle Inflate(FontRectangle rectangle, float x, float y)
    {
        return rectangle.Inflate(x, y);
    }

    /// <summary>
    ///     Creates a new <see cref="FontRectangle" /> by transforming the given rectangle by the given matrix.
    /// </summary>
    /// <param name="rectangle">The source rectangle.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>A transformed <see cref="FontRectangle" />.</returns>
    public static FontRectangle Transform(FontRectangle rectangle, Matrix3x2 matrix)
    {
        var bottomRight = Vector2.Transform(new Vector2(rectangle.Right, rectangle.Bottom), matrix);
        var topLeft = Vector2.Transform(rectangle.Location, matrix);
        var size = bottomRight - topLeft;

        return new FontRectangle(topLeft, size);
    }

    /// <summary>
    ///     Creates a rectangle that represents the union between <paramref name="a" /> and <paramref name="b" />.
    /// </summary>
    /// <param name="a">The first rectangle.</param>
    /// <param name="b">The second rectangle.</param>
    /// <returns>The <see cref="FontRectangle" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FontRectangle Union(FontRectangle a, FontRectangle b)
    {
        var x1 = MathF.Min(a.X, b.X);
        var x2 = MathF.Max(a.Right, b.Right);
        var y1 = MathF.Min(a.Y, b.Y);
        var y2 = MathF.Max(a.Bottom, b.Bottom);

        return new FontRectangle(x1, y1, x2 - x1, y2 - y1);
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
    ///     Creates a FontRectangle that represents the intersection between this FontRectangle and the
    ///     <paramref name="rectangle" />.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>New <see cref="FontRectangle" /> representing the intersections between the two rectangles.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FontRectangle Intersect(FontRectangle rectangle)
    {
        return Intersect(rectangle, this);
    }

    /// <summary>
    ///     Creates a new <see cref="FontRectangle" /> inflated by the specified amount.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>New <see cref="FontRectangle" /> representing the inflated rectangle</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FontRectangle Inflate(float width, float height)
    {
        return new FontRectangle(
            X - width,
            Y - height,
            Width + 2 * width,
            Height + 2 * height);
    }

    /// <summary>
    ///     Creates a new <see cref="FontRectangle" /> inflated by the specified amount.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>New <see cref="FontRectangle" /> representing the inflated rectangle</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FontRectangle Inflate(Vector2 size)
    {
        return Inflate(size.X, size.Y);
    }

    /// <summary>
    ///     Determines if the specified point is contained within the rectangular region defined by
    ///     this <see cref="FontRectangle" />.
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
    ///     <see cref="FontRectangle" /> .
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The <see cref="bool" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(Vector2 point)
    {
        return Contains(point.X, point.Y);
    }

    /// <summary>
    ///     Determines if the rectangular region represented by <paramref name="rectangle" /> is entirely contained
    ///     within the rectangular region represented by this <see cref="FontRectangle" /> .
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>The <see cref="bool" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(FontRectangle rectangle)
    {
        return X <= rectangle.X && rectangle.Right <= Right &&
               Y <= rectangle.Y && rectangle.Bottom <= Bottom;
    }

    /// <summary>
    ///     Determines if the specified <see cref="FontRectangle" /> intersects the rectangular region defined by
    ///     this <see cref="FontRectangle" />.
    /// </summary>
    /// <param name="rectangle">The other rectangle.</param>
    /// <returns>The <see cref="bool" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IntersectsWith(FontRectangle rectangle)
    {
        return rectangle.X < Right && X < rectangle.Right &&
               rectangle.Y < Bottom && Y < rectangle.Bottom;
    }

    /// <summary>
    ///     Adjusts the location of this rectangle by the specified amount.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>New <see cref="FontRectangle" /> representing the offset rectangle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FontRectangle Offset(Vector2 point)
    {
        return Offset(point.X, point.Y);
    }

    /// <summary>
    ///     Adjusts the location of this rectangle by the specified amount.
    /// </summary>
    /// <param name="dx">The amount to offset the x-coordinate.</param>
    /// <param name="dy">The amount to offset the y-coordinate.</param>
    /// <returns>New <see cref="FontRectangle" /> representing the inflated rectangle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FontRectangle Offset(float dx, float dy)
    {
        return new FontRectangle(X + dx, Y + dy, Width, Height);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Width, Height);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"FontRectangle [ X={X}, Y={Y}, Width={Width}, Height={Height} ]";
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is FontRectangle other
               && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(FontRectangle other)
    {
        return X.Equals(other.X)
               && Y.Equals(other.Y)
               && Width.Equals(other.Width)
               && Height.Equals(other.Height);
    }
}