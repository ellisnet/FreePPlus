// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

/// <summary>
///     A ref struct stack implementation that uses a pooled span to store the data.
/// </summary>
/// <typeparam name="T">The type of elements in the stack.</typeparam>
internal ref struct RefStack<T>
    where T : struct
{
    private const int MaxLength = 0X7FFFFFC7;
    private Buffer<T> buffer;
    private Span<T> stack;
    private bool isDisposed;

    public RefStack(int capacity)
    {
        if (capacity < 1) capacity = 4;

        buffer = new Buffer<T>(capacity);
        stack = buffer.GetSpan();
        isDisposed = false;
        Length = 0;
    }

    public int Length { get; private set; }

    public T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)Length) ThrowForOutOfRange();

            return stack[index];
        }

        set
        {
            if ((uint)index >= (uint)Length)
            {
                Push(value);
                return;
            }

            stack[index] = value;
        }
    }

    /// <summary>
    ///     Adds an item to the stack.
    /// </summary>
    /// <param name="value">The item to add.</param>
    public void Push(T value)
    {
        if ((uint)Length < (uint)stack.Length)
        {
            stack[Length++] = value;
        }
        else
        {
            var capacity = stack.Length * 2;
            if ((uint)capacity > MaxLength) capacity = MaxLength;

            var newBuffer = new Buffer<T>(capacity);
            var newStack = newBuffer.GetSpan();

            stack.CopyTo(newStack);
            buffer.Dispose();

            buffer = newBuffer;
            stack = newStack;

            stack[Length++] = value;
        }
    }

    /// <summary>
    ///     Removes the first element of the stack.
    /// </summary>
    /// <returns>The <typeparamref name="T" /> element.</returns>
    public T Shift()
    {
        var newSize = Length - 1;
        if (newSize < 0) ThrowForEmptyStack();

        var item = stack[0];
        stack = stack.Slice(1);
        Length = newSize;
        return item;
    }

    /// <summary>
    ///     Removes the last element of the stack.
    /// </summary>
    /// <returns>The <typeparamref name="T" /> element.</returns>
    public T Pop()
    {
        var newSize = Length - 1;
        if (newSize < 0) ThrowForEmptyStack();

        Length = newSize;
        return stack[newSize];
    }

    /// <summary>
    ///     Clears the current stack.
    /// </summary>
    public void Clear()
    {
        Length = 0;
        stack = buffer.GetSpan();
    }

    public void Dispose()
    {
        if (isDisposed) return;

        buffer.Dispose();
        isDisposed = true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowForOutOfRange()
    {
        throw new InvalidOperationException("Index must be greater or equal to zero or less than the stack length.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowForEmptyStack()
    {
        throw new InvalidOperationException("Empty stack!");
    }
}