// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing two 8-bit signed normalized values, ranging from −1 to 1.
///     <para>
///         Ranges from [-1, -1, 0, 1] to [1, 1, 0, 1] in vector form.
///     </para>
/// </summary>
public struct NormalizedByte2 : IPixel<NormalizedByte2>, IPackedVector<ushort>
{
    private static readonly Vector2 Half = new(127);
    private static readonly Vector2 MinusOne = new(-1F);

    /// <summary>
    ///     Initializes a new instance of the <see cref="NormalizedByte2" /> struct.
    /// </summary>
    /// <param name="x">The x-component.</param>
    /// <param name="y">The y-component.</param>
    public NormalizedByte2(float x, float y)
        : this(new Vector2(x, y)) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NormalizedByte2" /> struct.
    /// </summary>
    /// <param name="vector">The vector containing the component values.</param>
    public NormalizedByte2(Vector2 vector)
    {
        PackedValue = Pack(vector);
    }

    /// <inheritdoc />
    public ushort PackedValue { get; set; }

    /// <summary>
    ///     Compares two <see cref="NormalizedByte2" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="NormalizedByte2" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="NormalizedByte2" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(NormalizedByte2 left, NormalizedByte2 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="NormalizedByte2" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="NormalizedByte2" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="NormalizedByte2" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(NormalizedByte2 left, NormalizedByte2 right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public readonly PixelOperations<NormalizedByte2> CreatePixelOperations()
    {
        return new PixelOperations<NormalizedByte2>();
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromScaledVector4(Vector4 vector)
    {
        var scaled = new Vector2(vector.X, vector.Y) * 2F;
        scaled -= Vector2.One;
        PackedValue = Pack(scaled);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToScaledVector4()
    {
        var scaled = ToVector2();
        scaled += Vector2.One;
        scaled /= 2F;
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
        return new Vector4(ToVector2(), 0F, 1F);
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
    public void ToRgba32(ref Rgba32 dest)
    {
        dest.FromScaledVector4(ToScaledVector4());
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
        return new Vector2(
            (sbyte)((PackedValue >> 0) & 0xFF) / 127F,
            (sbyte)((PackedValue >> 8) & 0xFF) / 127F);
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is NormalizedByte2 other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(NormalizedByte2 other)
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
        return FormattableString.Invariant($"NormalizedByte2({vector.X:#0.##}, {vector.Y:#0.##})");
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static ushort Pack(Vector2 vector)
    {
        vector = Vector2.Clamp(vector, MinusOne, Vector2.One) * Half;

        var byte2 = ((ushort)Math.Round(vector.X) & 0xFF) << 0;
        var byte1 = ((ushort)Math.Round(vector.Y) & 0xFF) << 8;

        return (ushort)(byte2 | byte1);
    }
}