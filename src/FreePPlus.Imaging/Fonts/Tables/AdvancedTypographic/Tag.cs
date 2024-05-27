// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

/// <summary>
///     Data type for tag identifiers. Tags are four byte integers, each byte representing a character.
///     Tags are used to identify tables, design-variation axes, scripts, languages, font features, and baselines with
///     human-readable names.
/// </summary>
public readonly struct Tag : IEquatable<Tag>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Tag" /> struct.
    /// </summary>
    /// <param name="value">The tag value.</param>
    public Tag(uint value)
    {
        Value = value;
    }

    /// <summary>
    ///     Gets the Tag value as 32 bit unsigned integer.
    /// </summary>
    public uint Value { get; }

    public static implicit operator Tag(uint value)
    {
        return new Tag(value);
    }

    public static implicit operator Tag(FeatureTags value)
    {
        return new Tag((uint)value);
    }

    public static bool operator ==(Tag left, Tag right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Tag left, Tag right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Converts the string representation of a number to its Tag equivalent.
    /// </summary>
    /// <param name="value">A string containing a tag to convert.</param>
    /// <returns>The <see cref="Tag" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tag Parse(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length != 4) return default;

        var b3 = GetByte(value[3]);
        var b2 = GetByte(value[2]);
        var b1 = GetByte(value[1]);
        var b0 = GetByte(value[0]);

        return (uint)((b0 << 24) | (b1 << 16) | (b2 << 8) | b3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte GetByte(char c)
    {
        if (c is >= (char)0 and <= (char)255) return (byte)c;

        return 0;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Tag tag && Equals(tag);
    }

    /// <inheritdoc />
    public bool Equals(Tag other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var chars = new char[4];
        chars[3] = (char)(Value & 0xFF);
        chars[2] = (char)((Value >> 8) & 0xFF);
        chars[1] = (char)((Value >> 16) & 0xFF);
        chars[0] = (char)((Value >> 24) & 0xFF);

        return new string(chars);
    }
}