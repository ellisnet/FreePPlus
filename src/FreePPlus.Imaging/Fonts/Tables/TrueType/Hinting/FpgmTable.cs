// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.TrueType.Hinting;

//was previously: namespace SixLabors.Fonts.Tables.TrueType.Hinting;

internal class FpgmTable : Table
{
    internal const string TableName = "fpgm";

    public FpgmTable(byte[] instructions)
    {
        Instructions = instructions;
    }

    public byte[] Instructions { get; }

    public static FpgmTable? Load(FontReader fontReader)
    {
        if (!fontReader.TryGetReaderAtTablePosition(TableName, out var binaryReader, out var header)) return null;

        using (binaryReader)
        {
            return Load(binaryReader, header.Length);
        }
    }

    public static FpgmTable Load(BigEndianBinaryReader reader, uint tableLength)
    {
        // HEADER

        // Type     | Description
        // ---------| ------------
        // uint8[n] | Instructions. n is the number of uint8 items that fit in the size of the table.
        var instructions = reader.ReadUInt8Array((int)tableLength);

        return new FpgmTable(instructions);
    }
}