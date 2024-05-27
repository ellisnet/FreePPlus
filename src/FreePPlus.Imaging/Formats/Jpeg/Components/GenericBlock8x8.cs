// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

// ReSharper disable InconsistentNaming
namespace FreePPlus.Imaging.Formats.Jpeg.Components;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components;

/// <summary>
///     A generic 8x8 block implementation, useful for manipulating custom 8x8 pixel data.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe partial struct GenericBlock8x8<T>
    where T : struct
{
    public const int Size = 64;

    /// <summary>
    ///     FOR TESTING ONLY!
    ///     Gets or sets a <see cref="Rgb24" /> value at the given index
    /// </summary>
    /// <param name="idx">The index</param>
    /// <returns>The value</returns>
    public T this[int idx]
    {
        get
        {
            ref var selfRef = ref Unsafe.As<GenericBlock8x8<T>, T>(ref this);
            return Unsafe.Add(ref selfRef, idx);
        }

        set
        {
            ref var selfRef = ref Unsafe.As<GenericBlock8x8<T>, T>(ref this);
            Unsafe.Add(ref selfRef, idx) = value;
        }
    }

    /// <summary>
    ///     FOR TESTING ONLY!
    ///     Gets or sets a value in a row+column of the 8x8 block
    /// </summary>
    /// <param name="x">The x position index in the row</param>
    /// <param name="y">The column index</param>
    /// <returns>The value</returns>
    public T this[int x, int y]
    {
        get => this[y * 8 + x];
        set => this[y * 8 + x] = value;
    }

    /// <summary>
    ///     Load a 8x8 region of an image into the block.
    ///     The "outlying" area of the block will be stretched out with pixels on the right and bottom edge of the image.
    /// </summary>
    public void LoadAndStretchEdges(Buffer2D<T> source, int sourceX, int sourceY, in RowOctet<T> currentRows)
    {
        var width = Math.Min(8, source.Width - sourceX);
        var height = Math.Min(8, source.Height - sourceY);

        if (width <= 0 || height <= 0) return;

        var byteWidth = (uint)width * (uint)Unsafe.SizeOf<T>();
        var remainderXCount = 8 - width;

        ref var blockStart = ref Unsafe.As<GenericBlock8x8<T>, byte>(ref this);
        var blockRowSizeInBytes = 8 * Unsafe.SizeOf<T>();

        for (var y = 0; y < height; y++)
        {
            var row = currentRows[y];

            ref var s = ref Unsafe.As<T, byte>(ref row[sourceX]);
            ref var d = ref Unsafe.Add(ref blockStart, y * blockRowSizeInBytes);

            Unsafe.CopyBlock(ref d, ref s, byteWidth);

            ref var last = ref Unsafe.Add(ref Unsafe.As<byte, T>(ref d), width - 1);

            for (var x = 1; x <= remainderXCount; x++) Unsafe.Add(ref last, x) = last;
        }

        var remainderYCount = 8 - height;

        if (remainderYCount == 0) return;

        ref var lastRowStart = ref Unsafe.Add(ref blockStart, (height - 1) * blockRowSizeInBytes);

        for (var y = 1; y <= remainderYCount; y++)
        {
            ref var remStart = ref Unsafe.Add(ref lastRowStart, blockRowSizeInBytes * y);
            Unsafe.CopyBlock(ref remStart, ref lastRowStart, (uint)blockRowSizeInBytes);
        }
    }

    /// <summary>
    ///     Only for on-stack instances!
    /// </summary>
    public Span<T> AsSpanUnsafe()
    {
        return new Span<T>(Unsafe.AsPointer(ref this), Size);
    }
}