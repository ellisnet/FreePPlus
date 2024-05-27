// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing two 16-bit signed integer values.
///     <para>
///         Ranges from [-32767, -32767, 0, 1] to [32767, 32767, 0, 1] in vector form.
///     </para>
/// </summary>
public struct Short2 : IPixel<Short2>, IPackedVector<uint>
{
    // Largest two byte positive number 0xFFFF >> 1;
    private const float MaxPos = 0x7FFF;

    // Two's complement
    private const float MinNeg = ~(int)MaxPos;

    private static readonly Vector2 Max = new(MaxPos);
    private static readonly Vector2 Min = new(MinNeg);

    /// <summary>
    ///     Initializes a new instance of the <see cref="Short2" /> struct.
    /// </summary>
    /// <param name="x">The x-component.</param>
    /// <param name="y">The y-component.</param>
    public Short2(float x, float y)
        : this(new Vector2(x, y)) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Short2" /> struct.
    /// </summary>
    /// <param name="vector">The vector containing the component values.</param>
    public Short2(Vector2 vector)
    {
        PackedValue = Pack(vector);
    }

    /// <inheritdoc />
    public uint PackedValue { get; set; }

    /// <summary>
    ///     Compares two <see cref="Short2" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Short2" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Short2" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(Short2 left, Short2 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="Short2" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Short2" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Short2" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(Short2 left, Short2 right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public readonly PixelOperations<Short2> CreatePixelOperations()
    {
        return new PixelOperations<Short2>();
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromScaledVector4(Vector4 vector)
    {
        var scaled = new Vector2(vector.X, vector.Y) * 65534F;
        scaled -= new Vector2(32767F);
        PackedValue = Pack(scaled);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToScaledVector4()
    {
        var scaled = ToVector2();
        scaled += new Vector2(32767F);
        scaled /= 65534F;
        return new Vector4(scaled, 0F, 1F);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromVector4(Vector4 vector)
    {
        var vector2 = new Vector2(vector.X, vector.Y);
        PackedValue = Pack(vector2);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToVector4()
    {
        return new Vector4((short)(PackedValue & 0xFFFF), (short)(PackedValue >> 0x10), 0, 1);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromArgb32(Argb32 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgr24(Bgr24 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra32(Bgra32 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra5551(Bgra5551 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL8(L8 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL16(L16 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa16(La16 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa32(La32 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb24(Rgb24 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba32(Rgba32 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ToRgba32(ref Rgba32 dest)
    {
        dest.FromScaledVector4(ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb48(Rgb48 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba64(Rgba64 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <summary>
    ///     Expands the packed representation into a <see cref="Vector2" />.
    ///     The vector components are typically expanded in least to greatest significance order.
    /// </summary>
    /// <returns>The <see cref="Vector2" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector2 ToVector2()
    {
        return new Vector2((short)(PackedValue & 0xFFFF), (short)(PackedValue >> 0x10));
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is Short2 other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(Short2 other)
    {
        return PackedValue.Equals(other.PackedValue);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly override int GetHashCode()
    {
        return PackedValue.GetHashCode();
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        var vector = ToVector2();
        return FormattableString.Invariant($"Short2({vector.X:#0.##}, {vector.Y:#0.##})");
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static uint Pack(Vector2 vector)
    {
        vector = Vector2.Clamp(vector, Min, Max);
        var word2 = (uint)Math.Round(vector.X) & 0xFFFF;
        var word1 = ((uint)Math.Round(vector.Y) & 0xFFFF) << 0x10;

        return word2 | word1;
    }
}