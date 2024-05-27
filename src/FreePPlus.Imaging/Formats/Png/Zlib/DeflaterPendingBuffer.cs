// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Memory;

namespace FreePPlus.Imaging.Formats.Png.Zlib;

//was previously: namespace SixLabors.ImageSharp.Formats.Png.Zlib;

/// <summary>
///     Stores pending data for writing data to the Deflater.
/// </summary>
internal sealed unsafe class DeflaterPendingBuffer : IDisposable
{
    private readonly byte[] buffer;
    private readonly byte* pinnedBuffer;
    private uint bits;
    private MemoryHandle bufferMemoryHandle;
    private IManagedByteBuffer bufferMemoryOwner;
    private int end;
    private bool isDisposed;

    private int start;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeflaterPendingBuffer" /> class.
    /// </summary>
    /// <param name="memoryAllocator">The memory allocator to use for buffer allocations.</param>
    public DeflaterPendingBuffer(MemoryAllocator memoryAllocator)
    {
        bufferMemoryOwner = memoryAllocator.AllocateManagedByteBuffer(DeflaterConstants.PENDING_BUF_SIZE);
        buffer = bufferMemoryOwner.Array;
        bufferMemoryHandle = bufferMemoryOwner.Memory.Pin();
        pinnedBuffer = (byte*)bufferMemoryHandle.Pointer;
    }

    /// <summary>
    ///     Gets the number of bits written to the buffer.
    /// </summary>
    public int BitCount { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether indicates the buffer has been flushed.
    /// </summary>
    public bool IsFlushed => end == 0;

    /// <inheritdoc />
    public void Dispose()
    {
        if (!isDisposed)
        {
            bufferMemoryHandle.Dispose();
            bufferMemoryOwner.Dispose();
            bufferMemoryOwner = null;
            isDisposed = true;
        }
    }

    /// <summary>
    ///     Clear internal state/buffers.
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void Reset()
    {
        start = end = BitCount = 0;
    }

    /// <summary>
    ///     Write a short value to buffer LSB first.
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void WriteShort(int value)
    {
        var pinned = pinnedBuffer;
        pinned[end++] = unchecked((byte)value);
        pinned[end++] = unchecked((byte)(value >> 8));
    }

    /// <summary>
    ///     Write a block of data to the internal buffer.
    /// </summary>
    /// <param name="block">The data to write.</param>
    /// <param name="offset">The offset of first byte to write.</param>
    /// <param name="length">The number of bytes to write.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void WriteBlock(byte[] block, int offset, int length)
    {
        Unsafe.CopyBlockUnaligned(ref buffer[end], ref block[offset], unchecked((uint)length));
        end += length;
    }

    /// <summary>
    ///     Aligns internal buffer on a byte boundary.
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void AlignToByte()
    {
        if (BitCount > 0)
        {
            var pinned = pinnedBuffer;
            pinned[end++] = unchecked((byte)bits);
            if (BitCount > 8) pinned[end++] = unchecked((byte)(bits >> 8));
        }

        bits = 0;
        BitCount = 0;
    }

    /// <summary>
    ///     Write bits to internal buffer
    /// </summary>
    /// <param name="b">source of bits</param>
    /// <param name="count">number of bits to write</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void WriteBits(int b, int count)
    {
        bits |= (uint)(b << BitCount);
        BitCount += count;
        if (BitCount >= 16)
        {
            var pinned = pinnedBuffer;
            pinned[end++] = unchecked((byte)bits);
            pinned[end++] = unchecked((byte)(bits >> 8));
            bits >>= 16;
            BitCount -= 16;
        }
    }

    /// <summary>
    ///     Write a short value to internal buffer most significant byte first
    /// </summary>
    /// <param name="value">The value to write</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void WriteShortMSB(int value)
    {
        var pinned = pinnedBuffer;
        pinned[end++] = unchecked((byte)(value >> 8));
        pinned[end++] = unchecked((byte)value);
    }

    /// <summary>
    ///     Flushes the pending buffer into the given output array.
    ///     If the output array is to small, only a partial flush is done.
    /// </summary>
    /// <param name="output">The output array.</param>
    /// <param name="offset">The offset into output array.</param>
    /// <param name="length">The maximum number of bytes to store.</param>
    /// <returns>The number of bytes flushed.</returns>
    public int Flush(byte[] output, int offset, int length)
    {
        if (BitCount >= 8)
        {
            pinnedBuffer[end++] = unchecked((byte)bits);
            bits >>= 8;
            BitCount -= 8;
        }

        if (length > end - start)
        {
            length = end - start;

            Unsafe.CopyBlockUnaligned(ref output[offset], ref buffer[start], unchecked((uint)length));
            start = 0;
            end = 0;
        }
        else
        {
            Unsafe.CopyBlockUnaligned(ref output[offset], ref buffer[start], unchecked((uint)length));
            start += length;
        }

        return length;
    }
}