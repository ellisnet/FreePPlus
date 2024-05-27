// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;

namespace FreePPlus.Imaging.Fonts.Tables.TrueType.Glyphs;

//was previously: namespace SixLabors.Fonts.Tables.TrueType.Glyphs;

internal class SimpleGlyphLoader : GlyphLoader
{
    private readonly Bounds bounds;
    private readonly ushort[] endPoints;
    private readonly byte[] instructions;
    private readonly bool[] onCurves;
    private readonly short[] xs;
    private readonly short[] ys;

    public SimpleGlyphLoader(short[] xs, short[] ys, bool[] onCurves, ushort[] endPoints, Bounds bounds,
        byte[] instructions)
    {
        this.xs = xs;
        this.ys = ys;
        this.onCurves = onCurves;
        this.endPoints = endPoints;
        this.bounds = bounds;
        this.instructions = instructions;
    }

    public SimpleGlyphLoader(Bounds bounds)
    {
        ys = xs = Array.Empty<short>();
        onCurves = Array.Empty<bool>();
        endPoints = Array.Empty<ushort>();
        instructions = Array.Empty<byte>();
        this.bounds = bounds;
    }

    public override GlyphVector CreateGlyph(GlyphTable table)
    {
        return new GlyphVector(Convert(xs, ys), onCurves, endPoints, bounds, instructions);
    }

    private static Vector2[] Convert(short[] xs, short[] ys)
    {
        var vectors = new Vector2[xs.Length];
        for (var i = 0; i < xs.Length; i++) vectors[i] = new Vector2(xs[i], ys[i]);

        return vectors;
    }

    public static GlyphLoader LoadSimpleGlyph(BigEndianBinaryReader reader, short count, in Bounds bounds)
    {
        if (count == 0) return new SimpleGlyphLoader(bounds);

        // uint16         | endPtsOfContours[n] | Array of last points of each contour; n is the number of contours.
        // uint16         | instructionLength   | Total number of bytes for instructions.
        // uint8          | instructions[n]     | Array of instructions for each glyph; n is the number of instructions.
        // uint8          | flags[n]            | Array of flags for each coordinate in outline; n is the number of flags.
        // uint8 or int16 | xCoordinates[ ]     | First coordinates relative to(0, 0); others are relative to previous point.
        // uint8 or int16 | yCoordinates[]      | First coordinates relative to (0, 0); others are relative to previous point.
        var endPoints = reader.ReadUInt16Array(count);

        var instructionSize = reader.ReadUInt16();
        var instructions = reader.ReadUInt8Array(instructionSize);

        // TODO: should this take the max points rather?
        var pointCount = 0;
        if (count > 0) pointCount = endPoints[count - 1] + 1;

        var flags = ReadFlags(reader, pointCount);
        var xs = ReadCoordinates(reader, pointCount, flags, Flags.XByte, Flags.XSignOrSame);
        var ys = ReadCoordinates(reader, pointCount, flags, Flags.YByte, Flags.YSignOrSame);

        var onCurves = new bool[flags.Length];
        for (var i = flags.Length - 1; i >= 0; --i) onCurves[i] = flags[i].HasFlag(Flags.OnCurve);

        return new SimpleGlyphLoader(xs, ys, onCurves, endPoints, bounds, instructions);
    }

    private static Flags[] ReadFlags(BigEndianBinaryReader reader, int flagCount)
    {
        var result = new Flags[flagCount];
        var c = 0;
        var repeatCount = 0;
        Flags flag = default;
        while (c < flagCount)
        {
            if (repeatCount > 0)
            {
                repeatCount--;
            }
            else
            {
                flag = (Flags)reader.ReadUInt8();
                if (flag.HasFlag(Flags.Repeat)) repeatCount = reader.ReadByte();
            }

            result[c++] = flag;
        }

        return result;
    }

    private static short[] ReadCoordinates(BigEndianBinaryReader reader, int pointCount, Flags[] flags, Flags isByte,
        Flags signOrSame)
    {
        var xs = new short[pointCount];
        var x = 0;
        for (var i = 0; i < pointCount; i++)
        {
            int dx;
            if (flags[i].HasFlag(isByte))
            {
                var b = reader.ReadByte();
                dx = flags[i].HasFlag(signOrSame) ? b : -b;
            }
            else
            {
                if (flags[i].HasFlag(signOrSame))
                    dx = 0;
                else
                    dx = reader.ReadInt16();
            }

            x += dx;
            xs[i] = (short)x; // TODO: overflow?
        }

        return xs;
    }

    [Flags]
    private enum Flags : byte
    {
        ControlPoint = 0,
        OnCurve = 1,
        XByte = 2,
        YByte = 4,
        Repeat = 8,
        XSignOrSame = 16,
        YSignOrSame = 32
    }
}