// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing four 8-bit signed normalized values, ranging from −1 to 1.
///     <para>
///         Ranges from [-1, -1, -1, -1] to [1, 1, 1, 1] in vector form.
///     </para>
/// </summary>
public struct NormalizedByte4 : IPixel<NormalizedByte4>, IPackedVector<uint>
{
    private static readonly Vector4 Half = new(127);
    private static readonly Vector4 MinusOne = new(-1F);

    /// <summary>
    ///     Initializes a new instance of the <see cref="NormalizedByte4" /> struct.
    /// </summary>
    /// <param name="x">The x-component.</param>
    /// <param name="y">The y-component.</param>
    /// <param name="z">The z-component.</param>
    /// <param name="w">The w-component.</param>
    public NormalizedByte4(float x, float y, float z, float w)
        : this(new Vector4(x, y, z, w)) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NormalizedByte4" /> struct.
    /// </summary>
    /// <param name="vector">The vector containing the component values.</param>
    public NormalizedByte4(Vector4 vector)
    {
        PackedValue = Pack(ref vector);
    }

    /// <inheritdoc />
    public uint PackedValue { get; set; }

    /// <summary>
    ///     Compares two <see cref="NormalizedByte4" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="NormalizedByte4" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="NormalizedByte4" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(NormalizedByte4 left, NormalizedByte4 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="NormalizedByte4" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="NormalizedByte4" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="NormalizedByte4" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(NormalizedByte4 left, NormalizedByte4 right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public readonly PixelOperations<NormalizedByte4> CreatePixelOperations()
    {
        return new PixelOperations<NormalizedByte4>();
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromScaledVector4(Vector4 vector)
    {
        vector *= 2F;
        vector -= Vector4.One;
        FromVector4(vector);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToScaledVector4()
    {
        var scaled = ToVector4();
        scaled += Vector4.One;
        scaled /= 2F;
        return scaled;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromVector4(Vector4 vector)
    {
        PackedValue = Pack(ref vector);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToVector4()
    {
        return new Vector4(
            (sbyte)((PackedValue >> 0) & 0xFF) / 127F,
            (sbyte)((PackedValue >> 8) & 0xFF) / 127F,
            (sbyte)((PackedValue >> 16) & 0xFF) / 127F,
            (sbyte)((PackedValue >> 24) & 0xFF) / 127F);
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

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is NormalizedByte4 other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(NormalizedByte4 other)
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
        var vector = ToVector4();
        return FormattableString.Invariant(
            $"NormalizedByte4({vector.X:#0.##}, {vector.Y:#0.##}, {vector.Z:#0.##}, {vector.W:#0.##})");
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static uint Pack(ref Vector4 vector)
    {
        vector = Vector4Utilities.FastClamp(vector, MinusOne, Vector4.One) * Half;

        var byte4 = ((uint)MathF.Round(vector.X) & 0xFF) << 0;
        var byte3 = ((uint)MathF.Round(vector.Y) & 0xFF) << 8;
        var byte2 = ((uint)MathF.Round(vector.Z) & 0xFF) << 16;
        var byte1 = ((uint)MathF.Round(vector.W) & 0xFF) << 24;

        return byte4 | byte3 | byte2 | byte1;
    }
}