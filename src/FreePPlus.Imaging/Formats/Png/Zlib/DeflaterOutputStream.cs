// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using FreePPlus.Imaging.Memory;

namespace FreePPlus.Imaging.Formats.Png.Zlib;

//was previously: namespace SixLabors.ImageSharp.Formats.Png.Zlib;

/// <summary>
///     A special stream deflating or compressing the bytes that are
///     written to it.  It uses a Deflater to perform actual deflating.
/// </summary>
internal sealed class DeflaterOutputStream : Stream
{
    private const int BufferLength = 512;
    private readonly byte[] buffer;
    private readonly Stream rawStream;
    private Deflater deflater;
    private bool isDisposed;
    private IManagedByteBuffer memoryOwner;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DeflaterOutputStream" /> class.
    /// </summary>
    /// <param name="memoryAllocator">The memory allocator to use for buffer allocations.</param>
    /// <param name="rawStream">The output stream where deflated output is written.</param>
    /// <param name="compressionLevel">The compression level.</param>
    public DeflaterOutputStream(MemoryAllocator memoryAllocator, Stream rawStream, int compressionLevel)
    {
        this.rawStream = rawStream;
        memoryOwner = memoryAllocator.AllocateManagedByteBuffer(BufferLength);
        buffer = memoryOwner.Array;
        deflater = new Deflater(memoryAllocator, compressionLevel);
    }

    /// <inheritdoc />
    public override bool CanRead => false;

    /// <inheritdoc />
    public override bool CanSeek => false;

    /// <inheritdoc />
    public override bool CanWrite => rawStream.CanWrite;

    /// <inheritdoc />
    public override long Length => rawStream.Length;

    /// <inheritdoc />
    public override long Position
    {
        get => rawStream.Position;

        set => throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override int ReadByte()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override void Flush()
    {
        deflater.Flush();
        Deflate(true);
        rawStream.Flush();
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        deflater.SetInput(buffer, offset, count);
        Deflate();
    }

    private void Deflate()
    {
        Deflate(false);
    }

    private void Deflate(bool flushing)
    {
        while (flushing || !deflater.IsNeedingInput)
        {
            var deflateCount = deflater.Deflate(buffer, 0, BufferLength);

            if (deflateCount <= 0) break;

            rawStream.Write(buffer, 0, deflateCount);
        }

        if (!deflater.IsNeedingInput) DeflateThrowHelper.ThrowNoDeflate();
    }

    private void Finish()
    {
        deflater.Finish();
        while (!deflater.IsFinished)
        {
            var len = deflater.Deflate(buffer, 0, BufferLength);
            if (len <= 0) break;

            rawStream.Write(buffer, 0, len);
        }

        if (!deflater.IsFinished) DeflateThrowHelper.ThrowNoDeflate();

        rawStream.Flush();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                Finish();
                deflater.Dispose();
                memoryOwner.Dispose();
            }

            deflater = null;
            memoryOwner = null;
            isDisposed = true;
            base.Dispose(disposing);
        }
    }
}