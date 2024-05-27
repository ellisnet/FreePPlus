// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Fonts.WellKnownIds;

namespace FreePPlus.Imaging.Fonts.Tables.General.CMap;

//was previously: namespace SixLabors.Fonts.Tables.General.CMap;

internal readonly struct EncodingRecord
{
    public EncodingRecord(PlatformIDs platformID, ushort encodingID, uint offset)
    {
        PlatformID = platformID;
        EncodingID = encodingID;
        Offset = offset;
    }

    public PlatformIDs PlatformID { get; }

    public ushort EncodingID { get; }

    public uint Offset { get; }

    public static EncodingRecord Read(BigEndianBinaryReader reader)
    {
        var platform = (PlatformIDs)reader.ReadUInt16();
        var encoding = reader.ReadUInt16();
        var offset = reader.ReadOffset32();

        return new EncodingRecord(platform, encoding, offset);
    }
}