// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables;

//was previously: namespace SixLabors.Fonts.Tables;

internal class TableHeader
{
    public TableHeader(string tag, uint checkSum, uint offset, uint len)
    {
        Tag = tag;
        CheckSum = checkSum;
        Offset = offset;
        Length = len;
    }

    public string Tag { get; }

    public uint Offset { get; }

    public uint CheckSum { get; }

    public uint Length { get; }

    public static TableHeader Read(BigEndianBinaryReader reader)
    {
        return new TableHeader(
            reader.ReadTag(),
            reader.ReadUInt32(),
            reader.ReadOffset32(),
            reader.ReadUInt32());
    }

    public virtual BigEndianBinaryReader CreateReader(Stream stream)
    {
        stream.Seek(Offset, SeekOrigin.Begin);

        return new BigEndianBinaryReader(stream, true);
    }
}