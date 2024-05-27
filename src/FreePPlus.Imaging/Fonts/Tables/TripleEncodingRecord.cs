// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables;

//was previously: namespace SixLabors.Fonts.Tables;

internal readonly struct TripleEncodingRecord
{
    public readonly byte ByteCount;
    public readonly byte XBits;
    public readonly byte YBits;
    public readonly ushort DeltaX;
    public readonly ushort DeltaY;
    public readonly sbyte Xsign;
    public readonly sbyte Ysign;

    public TripleEncodingRecord(
        byte byteCount,
        byte xbits,
        byte ybits,
        ushort deltaX,
        ushort deltaY,
        sbyte xsign,
        sbyte ysign)
    {
        ByteCount = byteCount;
        XBits = xbits;
        YBits = ybits;
        DeltaX = deltaX;
        DeltaY = deltaY;
        Xsign = xsign;
        Ysign = ysign;
    }

    public int Tx(int orgX)
    {
        return (orgX + DeltaX) * Xsign;
    }

    public int Ty(int orgY)
    {
        return (orgY + DeltaY) * Ysign;
    }
}