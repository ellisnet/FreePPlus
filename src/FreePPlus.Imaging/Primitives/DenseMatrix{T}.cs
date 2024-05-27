// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Represents a dense matrix with arbitrary elements.
///     Components that are adjacent in a column of the matrix are adjacent in the storage array.
///     The components are said to be stored in column major order.
/// </summary>
/// <typeparam name="T">The type of elements in the matrix.</typeparam>
public readonly struct DenseMatrix<T> : IEquatable<DenseMatrix<T>>
    where T : struct, IEquatable<T>
{
    /// <summary>
    ///     The 1D representation of the dense matrix.
    /// </summary>
    public readonly T[] Data;

    /// <summary>
    ///     Gets the number of columns in the dense matrix.
    /// </summary>
    public readonly int Columns;

    /// <summary>
    ///     Gets the number of rows in the dense matrix.
    /// </summary>
    public readonly int Rows;

    /// <summary>
    ///     Gets the size of the dense matrix.
    /// </summary>
    public readonly Size Size;

    /// <summary>
    ///     Gets the number of items in the array.
    /// </summary>
    public readonly int Count;

    /// <summary>
    ///     Initializes a new instance of the <see cref=" DenseMatrix{T}" /> struct.
    /// </summary>
    /// <param name="length">The length of each side in the matrix.</param>
    public DenseMatrix(int length)
        : this(length, length) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref=" DenseMatrix{T}" /> struct.
    /// </summary>
    /// <param name="columns">The number of columns.</param>
    /// <param name="rows">The number of rows.</param>
    public DenseMatrix(int columns, int rows)
    {
        Guard.MustBeGreaterThan(columns, 0, nameof(columns));
        Guard.MustBeGreaterThan(rows, 0, nameof(rows));

        Rows = rows;
        Columns = columns;
        Size = new Size(columns, rows);
        Count = columns * rows;
        Data = new T[Columns * Rows];
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref=" DenseMatrix{T}" /> struct.
    /// </summary>
    /// <param name="data">The 2D array to provide access to.</param>
    public DenseMatrix(T[,] data)
    {
        Guard.NotNull(data, nameof(data));
        var rows = data.GetLength(0);
        var columns = data.GetLength(1);

        Guard.MustBeGreaterThan(rows, 0, nameof(Rows));
        Guard.MustBeGreaterThan(columns, 0, nameof(Columns));

        Rows = rows;
        Columns = columns;
        Size = new Size(columns, rows);
        Count = Columns * Rows;
        Data = new T[Columns * Rows];

        for (var y = 0; y < Rows; y++)
        for (var x = 0; x < Columns; x++)
        {
            ref var value = ref this[y, x];
            value = data[y, x];
        }
    }

    /// <summary>
    ///     Gets a span wrapping the <see cref="Data" />.
    /// </summary>
    public Span<T> Span => new(Data);

    /// <summary>
    ///     Gets or sets the item at the specified position.
    /// </summary>
    /// <param name="row">
    ///     The row-coordinate of the item. Must be greater than or equal to zero and less than the height of the
    ///     array.
    /// </param>
    /// <param name="column">
    ///     The column-coordinate of the item. Must be greater than or equal to zero and less than the width
    ///     of the array.
    /// </param>
    /// <returns>The <see typeparam="T" /> at the specified position.</returns>
    public ref T this[int row, int column]
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        get
        {
            CheckCoordinates(row, column);
            return ref Data[row * Columns + column];
        }
    }

    /// <summary>
    ///     Performs an implicit conversion from a <see cref="T:T[,]" /> to a <see cref=" DenseMatrix{T}" />.
    /// </summary>
    /// <param name="data">The source array.</param>
    /// <returns>
    ///     The <see cref="DenseMatrix{T}" /> representation on the source data.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator DenseMatrix<T>(T[,] data)
    {
        return new DenseMatrix<T>(data);
    }

    /// <summary>
    ///     Performs an implicit conversion from a <see cref="DenseMatrix{T}" /> to a <see cref="T:T[,]" />.
    /// </summary>
    /// <param name="data">The source array.</param>
    /// <returns>
    ///     The <see cref="T:T[,]" /> representation on the source data.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
#pragma warning disable SA1008 // Opening parenthesis should be spaced correctly
    public static implicit operator T[,](in DenseMatrix<T> data)
#pragma warning restore SA1008 // Opening parenthesis should be spaced correctly
    {
        var result = new T[data.Rows, data.Columns];

        for (var y = 0; y < data.Rows; y++)
        for (var x = 0; x < data.Columns; x++)
        {
            ref var value = ref result[y, x];
            value = data[y, x];
        }

        return result;
    }

    /// <summary>
    ///     Compares the two <see cref="DenseMatrix{T}" /> instances to determine whether they are unequal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator ==(DenseMatrix<T> left, DenseMatrix<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares the two <see cref="DenseMatrix{T}" /> instances to determine whether they are equal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator !=(DenseMatrix<T> left, DenseMatrix<T> right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Transposes the rows and columns of the dense matrix.
    /// </summary>
    /// <returns>The <see cref="DenseMatrix{T}" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public DenseMatrix<T> Transpose()
    {
        var result = new DenseMatrix<T>(Rows, Columns);

        for (var y = 0; y < Rows; y++)
        for (var x = 0; x < Columns; x++)
        {
            ref var value = ref result[x, y];
            value = this[y, x];
        }

        return result;
    }

    /// <summary>
    ///     Fills the matrix with the given value
    /// </summary>
    /// <param name="value">The value to fill each item with</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void Fill(T value)
    {
        Span.Fill(value);
    }

    /// <summary>
    ///     Clears the matrix setting each value to the default value for the element type
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void Clear()
    {
        Span.Clear();
    }

    /// <summary>
    ///     Checks the coordinates to ensure they are within bounds.
    /// </summary>
    /// <param name="row">The y-coordinate of the item. Must be greater than zero and smaller than the height of the matrix.</param>
    /// <param name="column">The x-coordinate of the item. Must be greater than zero and smaller than the width of the matrix.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the coordinates are not within the bounds of the array.
    /// </exception>
    [Conditional("DEBUG")]
    private void CheckCoordinates(int row, int column)
    {
        if (row < 0 || row >= Rows)
            throw new ArgumentOutOfRangeException(nameof(row), row, $"{row} is outwith the matrix bounds.");

        if (column < 0 || column >= Columns)
            throw new ArgumentOutOfRangeException(nameof(column), column, $"{column} is outwith the matrix bounds.");
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is DenseMatrix<T> other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool Equals(DenseMatrix<T> other)
    {
        return Columns == other.Columns
               && Rows == other.Rows
               && Span.SequenceEqual(other.Span);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public override int GetHashCode()
    {
        HashCode code = default;

        code.Add(Columns);
        code.Add(Rows);

        var span = Span;
        for (var i = 0; i < span.Length; i++) code.Add(span[i]);

        return code.ToHashCode();
    }
}