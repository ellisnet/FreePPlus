// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using FreePPlus.Imaging.Memory;

namespace FreePPlus.Imaging.Formats.Png.Zlib;

//was previously: namespace SixLabors.ImageSharp.Formats.Png.Zlib;

/// <summary>
///     Provides methods and properties for compressing streams by using the Zlib Deflate algorithm.
/// </summary>
internal sealed class ZlibDeflateStream : Stream
{
    /// <summary>
    ///     Computes the checksum for the data stream.
    /// </summary>
    private readonly Adler32 adler32 = new();

    /// <summary>
    ///     The raw stream containing the uncompressed image data.
    /// </summary>
    private readonly Stream rawStream;

    /// <summary>
    ///     The stream responsible for compressing the input stream.
    /// </summary>
    // private DeflateStream deflateStream;
    private DeflaterOutputStream deflateStream;

    /// <summary>
    ///     A value indicating whether this instance of the given entity has been disposed.
    /// </summary>
    /// <value><see langword="true" /> if this instance has been disposed; otherwise, <see langword="false" />.</value>
    /// <remarks>
    ///     If the entity is disposed, it must not be disposed a second
    ///     time. The isDisposed field is set the first time the entity
    ///     is disposed. If the isDisposed field is true, then the Dispose()
    ///     method will not dispose again. This help not to prolong the entity's
    ///     life in the Garbage Collector.
    /// </remarks>
    private bool isDisposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ZlibDeflateStream" /> class.
    /// </summary>
    /// <param name="memoryAllocator">The memory allocator to use for buffer allocations.</param>
    /// <param name="stream">The stream to compress.</param>
    /// <param name="level">The compression level.</param>
    public ZlibDeflateStream(MemoryAllocator memoryAllocator, Stream stream, PngCompressionLevel level)
    {
        var compressionLevel = (int)level;
        rawStream = stream;

        // Write the zlib header : http://tools.ietf.org/html/rfc1950
        // CMF(Compression Method and flags)
        // This byte is divided into a 4 - bit compression method and a
        // 4-bit information field depending on the compression method.
        // bits 0 to 3  CM Compression method
        // bits 4 to 7  CINFO Compression info
        //
        //   0   1
        // +---+---+
        // |CMF|FLG|
        // +---+---+
        const int Cmf = 0x78;
        var flg = 218;

        // http://stackoverflow.com/a/2331025/277304
        if (compressionLevel >= 5 && compressionLevel <= 6)
            flg = 156;
        else if (compressionLevel >= 3 && compressionLevel <= 4)
            flg = 94;
        else if (compressionLevel <= 2) flg = 1;

        // Just in case
        flg -= (Cmf * 256 + flg) % 31;

        if (flg < 0) flg += 31;

        rawStream.WriteByte(Cmf);
        rawStream.WriteByte((byte)flg);

        deflateStream = new DeflaterOutputStream(memoryAllocator, rawStream, compressionLevel);
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
    public override void Flush()
    {
        deflateStream.Flush();
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
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
    public override void Write(byte[] buffer, int offset, int count)
    {
        deflateStream.Write(buffer, offset, count);
        adler32.Update(buffer.AsSpan(offset, count));
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (isDisposed) return;

        if (disposing)
        {
            // dispose managed resources
            deflateStream.Dispose();

            // Add the crc
            var crc = (uint)adler32.Value;
            rawStream.WriteByte((byte)((crc >> 24) & 0xFF));
            rawStream.WriteByte((byte)((crc >> 16) & 0xFF));
            rawStream.WriteByte((byte)((crc >> 8) & 0xFF));
            rawStream.WriteByte((byte)(crc & 0xFF));
        }

        deflateStream = null;

        base.Dispose(disposing);
        isDisposed = true;
    }
}