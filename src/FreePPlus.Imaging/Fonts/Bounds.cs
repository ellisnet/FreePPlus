// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

internal readonly struct Bounds : IEquatable<Bounds>
{
    public Bounds(Vector2 min, Vector2 max)
    {
        Min = Vector2.Min(min, max);
        Max = Vector2.Max(min, max);
    }

    public Bounds(float minX, float minY, float maxX, float maxY)
        : this(new Vector2(minX, minY), new Vector2(maxX, maxY)) { }

    public Vector2 Min { get; }

    public Vector2 Max { get; }

    public static bool operator ==(Bounds left, Bounds right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Bounds left, Bounds right)
    {
        return !(left == right);
    }

    public Vector2 Size()
    {
        return new Vector2(Max.X - Min.X, Max.Y - Min.Y);
    }

    public static Bounds Load(BigEndianBinaryReader reader)
    {
        var minX = reader.ReadInt16();
        var minY = reader.ReadInt16();
        var maxX = reader.ReadInt16();
        var maxY = reader.ReadInt16();

        return new Bounds(minX, minY, maxX, maxY);
    }

    public static Bounds Transform(in Bounds bounds, Matrix3x2 matrix)
    {
        return new Bounds(Vector2.Transform(bounds.Min, matrix), Vector2.Transform(bounds.Max, matrix));
    }

    public override bool Equals(object? obj)
    {
        return obj is Bounds bounds && Equals(bounds);
    }

    public bool Equals(Bounds other)
    {
        return Min.Equals(other.Min) && Max.Equals(other.Max);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max);
    }
}