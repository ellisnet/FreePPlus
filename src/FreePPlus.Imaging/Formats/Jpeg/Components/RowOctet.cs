// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Memory;

namespace FreePPlus.Imaging.Formats.Jpeg.Components;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components;

/// <summary>
///     Cache 8 pixel rows on the stack, which may originate from different buffers of a <see cref="MemoryGroup{T}" />.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly ref struct RowOctet<T>
    where T : struct
{
    private readonly Span<T> row0;
    private readonly Span<T> row1;
    private readonly Span<T> row2;
    private readonly Span<T> row3;
    private readonly Span<T> row4;
    private readonly Span<T> row5;
    private readonly Span<T> row6;
    private readonly Span<T> row7;

    public RowOctet(Buffer2D<T> buffer, int startY)
    {
        var y = startY;
        var height = buffer.Height;
        row0 = y < height ? buffer.GetRowSpan(y++) : default;
        row1 = y < height ? buffer.GetRowSpan(y++) : default;
        row2 = y < height ? buffer.GetRowSpan(y++) : default;
        row3 = y < height ? buffer.GetRowSpan(y++) : default;
        row4 = y < height ? buffer.GetRowSpan(y++) : default;
        row5 = y < height ? buffer.GetRowSpan(y++) : default;
        row6 = y < height ? buffer.GetRowSpan(y++) : default;
        row7 = y < height ? buffer.GetRowSpan(y) : default;
    }

    public Span<T> this[int y]
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        get
        {
            // No unsafe tricks, since Span<T> can't be used as a generic argument
            return y switch
            {
                0 => row0,
                1 => row1,
                2 => row2,
                3 => row3,
                4 => row4,
                5 => row5,
                6 => row6,
                7 => row7,
                _ => ThrowIndexOutOfRangeException()
            };
        }
    }

    [MethodImpl(InliningOptions.ColdPath)]
    private static Span<T> ThrowIndexOutOfRangeException()
    {
        throw new IndexOutOfRangeException();
    }
}