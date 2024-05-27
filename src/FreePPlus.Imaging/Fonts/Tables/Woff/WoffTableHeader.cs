// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using FreePPlus.Imaging.Fonts.IO;

namespace FreePPlus.Imaging.Fonts.Tables.Woff;

//was previously: namespace SixLabors.Fonts.Tables.Woff;

internal sealed class WoffTableHeader : TableHeader
{
    public WoffTableHeader(string tag, uint offset, uint compressedLength, uint origLength, uint checkSum)
        : base(tag, checkSum, offset, origLength)
    {
        CompressedLength = compressedLength;
    }

    public uint CompressedLength { get; }

    public override BigEndianBinaryReader CreateReader(Stream stream)
    {
        // Stream is not compressed.
        if (Length == CompressedLength) return base.CreateReader(stream);

        // Read all data from the compressed stream.
        stream.Seek(Offset, SeekOrigin.Begin);
        using var compressedStream = new ZlibInflateStream(stream);
        var uncompressedBytes = new byte[Length];
        var totalBytesRead = 0;
        var bytesLeftToRead = uncompressedBytes.Length;
        while (totalBytesRead < Length)
        {
            var bytesRead = compressedStream.Read(uncompressedBytes, totalBytesRead, bytesLeftToRead);
            if (bytesRead <= 0)
                throw new InvalidFontFileException(
                    $"Could not read compressed data! Expected bytes: {Length}, bytes read: {totalBytesRead}");

            totalBytesRead += bytesRead;
            bytesLeftToRead -= bytesRead;
        }

        var memoryStream = new MemoryStream(uncompressedBytes);
        return new BigEndianBinaryReader(memoryStream, false);
    }

    // WOFF TableDirectoryEntry
    // UInt32 | tag          | 4-byte sfnt table identifier.
    // UInt32 | offset       | Offset to the data, from beginning of WOFF file.
    // UInt32 | compLength   | Length of the compressed data, excluding padding.
    // UInt32 | origLength   | Length of the uncompressed table, excluding padding.
    // UInt32 | origChecksum | Checksum of the uncompressed table.
    public new static WoffTableHeader Read(BigEndianBinaryReader reader)
    {
        return new WoffTableHeader(
            reader.ReadTag(),
            reader.ReadUInt32(),
            reader.ReadUInt32(),
            reader.ReadUInt32(),
            reader.ReadUInt32());
    }
}