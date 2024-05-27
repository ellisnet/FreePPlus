// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.Memory;

//was previously: namespace SixLabors.ImageSharp.Memory;

/// <summary>
///     Represents a rectangular region inside a 2D memory buffer (<see cref="Buffer2D{T}" />).
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public readonly struct Buffer2DRegion<T>
    where T : unmanaged
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Buffer2DRegion{T}" /> struct.
    /// </summary>
    /// <param name="buffer">The <see cref="Buffer2D{T}" />.</param>
    /// <param name="rectangle">The <see cref="Rectangle" /> defining a rectangular area within the buffer.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Buffer2DRegion(Buffer2D<T> buffer, Rectangle rectangle)
    {
        DebugGuard.MustBeGreaterThanOrEqualTo(rectangle.X, 0, nameof(rectangle));
        DebugGuard.MustBeGreaterThanOrEqualTo(rectangle.Y, 0, nameof(rectangle));
        DebugGuard.MustBeLessThanOrEqualTo(rectangle.Width, buffer.Width, nameof(rectangle));
        DebugGuard.MustBeLessThanOrEqualTo(rectangle.Height, buffer.Height, nameof(rectangle));

        Buffer = buffer;
        Rectangle = rectangle;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Buffer2DRegion{T}" /> struct.
    /// </summary>
    /// <param name="buffer">The <see cref="Buffer2D{T}" />.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Buffer2DRegion(Buffer2D<T> buffer)
        : this(buffer, buffer.FullRectangle()) { }

    /// <summary>
    ///     Gets the rectangle specifying the boundaries of the area in <see cref="Buffer" />.
    /// </summary>
    public Rectangle Rectangle { get; }

    /// <summary>
    ///     Gets the <see cref="Buffer2D{T}" /> being pointed by this instance.
    /// </summary>
    public Buffer2D<T> Buffer { get; }

    /// <summary>
    ///     Gets the width
    /// </summary>
    public int Width => Rectangle.Width;

    /// <summary>
    ///     Gets the height
    /// </summary>
    public int Height => Rectangle.Height;

    /// <summary>
    ///     Gets the pixel stride which is equal to the width of <see cref="Buffer" />.
    /// </summary>
    public int Stride => Buffer.Width;

    /// <summary>
    ///     Gets the size of the area.
    /// </summary>
    internal Size Size => Rectangle.Size;

    /// <summary>
    ///     Gets a value indicating whether the area refers to the entire <see cref="Buffer" />
    /// </summary>
    internal bool IsFullBufferArea => Size == Buffer.Size();

    /// <summary>
    ///     Gets or sets a value at the given index.
    /// </summary>
    /// <param name="x">The position inside a row</param>
    /// <param name="y">The row index</param>
    /// <returns>The reference to the value</returns>
    internal ref T this[int x, int y] => ref Buffer[x + Rectangle.X, y + Rectangle.Y];

    /// <summary>
    ///     Gets a span to row 'y' inside this area.
    /// </summary>
    /// <param name="y">The row index</param>
    /// <returns>The span</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> GetRowSpan(int y)
    {
        var yy = Rectangle.Y + y;
        var xx = Rectangle.X;
        var width = Rectangle.Width;

        return Buffer.GetRowSpan(yy).Slice(xx, width);
    }

    /// <summary>
    ///     Returns a subregion as <see cref="Buffer2DRegion{T}" />. (Similar to <see cref="Span{T}.Slice(int, int)" />.)
    /// </summary>
    /// <param name="x">The x index at the subregion origin.</param>
    /// <param name="y">The y index at the subregion origin.</param>
    /// <param name="width">The desired width of the subregion.</param>
    /// <param name="height">The desired height of the subregion.</param>
    /// <returns>The subregion</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Buffer2DRegion<T> GetSubRegion(int x, int y, int width, int height)
    {
        var rectangle = new Rectangle(x, y, width, height);
        return GetSubRegion(rectangle);
    }

    /// <summary>
    ///     Returns a subregion as <see cref="Buffer2DRegion{T}" />. (Similar to <see cref="Span{T}.Slice(int, int)" />.)
    /// </summary>
    /// <param name="rectangle">The <see cref="Rectangle" /> specifying the boundaries of the subregion</param>
    /// <returns>The subregion</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Buffer2DRegion<T> GetSubRegion(Rectangle rectangle)
    {
        DebugGuard.MustBeLessThanOrEqualTo(rectangle.Width, Rectangle.Width, nameof(rectangle));
        DebugGuard.MustBeLessThanOrEqualTo(rectangle.Height, Rectangle.Height, nameof(rectangle));

        var x = Rectangle.X + rectangle.X;
        var y = Rectangle.Y + rectangle.Y;
        rectangle = new Rectangle(x, y, rectangle.Width, rectangle.Height);
        return new Buffer2DRegion<T>(Buffer, rectangle);
    }

    /// <summary>
    ///     Gets a reference to the [0,0] element.
    /// </summary>
    /// <returns>The reference to the [0,0] element</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref T GetReferenceToOrigin()
    {
        var y = Rectangle.Y;
        var x = Rectangle.X;
        return ref Buffer.GetRowSpan(y)[x];
    }

    internal void Clear()
    {
        // Optimization for when the size of the area is the same as the buffer size.
        if (IsFullBufferArea)
        {
            Buffer.FastMemoryGroup.Clear();
            return;
        }

        for (var y = 0; y < Rectangle.Height; y++)
        {
            var row = GetRowSpan(y);
            row.Clear();
        }
    }
}