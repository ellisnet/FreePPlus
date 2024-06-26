// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Diagnostics;
using System.Text;

namespace FreePPlus.Imaging.Fonts.Utilities;

//was previously: namespace SixLabors.Fonts.Utilities;

[DebuggerDisplay("Offset: {Offset}, Length: {Length}, Value: {Value}")]
internal class StringLoader
{
    public StringLoader(ushort length, ushort offset, Encoding encoding)
    {
        Length = length;
        Offset = offset;
        Encoding = encoding;
        Value = string.Empty;
    }

    public ushort Length { get; }

    public ushort Offset { get; }

    public string Value { get; private set; }

    public Encoding Encoding { get; }

    public static StringLoader Create(BigEndianBinaryReader reader)
    {
        return Create(reader, Encoding.BigEndianUnicode);
    }

    public static StringLoader Create(BigEndianBinaryReader reader, Encoding encoding)
    {
        return new StringLoader(reader.ReadUInt16(), reader.ReadUInt16(), encoding);
    }

    public void LoadValue(BigEndianBinaryReader reader)
    {
        Value = reader.ReadString(Length, Encoding).Replace("\0", string.Empty);
    }
}