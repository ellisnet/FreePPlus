// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal ref struct SimpleBinaryReader
{
    private readonly ReadOnlySpan<byte> buffer;

    public SimpleBinaryReader(ReadOnlySpan<byte> buffer)
    {
        this.buffer = buffer;
        Position = 0;
    }

    public int Length => buffer.Length;

    // TODO: Bounds checks.
    public int Position { get; set; }

    public bool CanRead()
    {
        return (uint)Position < buffer.Length;
    }

    public byte ReadByte()
    {
        return buffer[Position++];
    }

    public int ReadInt16BE()
    {
        var b1 = buffer[Position + 1];
        var b0 = buffer[Position];
        Position += 2;

        return (short)((b0 << 8) | b1);
    }

    public float ReadFloatFixed1616()
    {
        // Read a BE int, we parse it later.
        var b3 = buffer[Position + 3];
        var b2 = buffer[Position + 2];
        var b1 = buffer[Position + 1];
        var b0 = buffer[Position];
        Position += 4;

        // This number is interpreted as a Fixed; that is, a signed number with 16 bits of fraction
        float number = (short)((b0 << 8) | b1);
        var fraction = (short)((b2 << 8) | b3) / 65536F;
        return number + fraction;
    }
}