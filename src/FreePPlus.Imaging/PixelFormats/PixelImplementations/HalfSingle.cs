// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing a single 16 bit floating point value.
///     <para>
///         Ranges from [-1, 0, 0, 1] to [1, 0, 0, 1] in vector form.
///     </para>
/// </summary>
public struct HalfSingle : IPixel<HalfSingle>, IPackedVector<ushort>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="HalfSingle" /> struct.
    /// </summary>
    /// <param name="single">The single component.</param>
    public HalfSingle(float single)
    {
        PackedValue = HalfTypeHelper.Pack(single);
    }

    /// <inheritdoc />
    public ushort PackedValue { get; set; }

    /// <summary>
    ///     Compares two <see cref="HalfSingle" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="HalfSingle" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="HalfSingle" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(HalfSingle left, HalfSingle right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="HalfSingle" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="HalfSingle" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="HalfSingle" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(HalfSingle left, HalfSingle right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public PixelOperations<HalfSingle> CreatePixelOperations()
    {
        return new PixelOperations<HalfSingle>();
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromScaledVector4(Vector4 vector)
    {
        var scaled = vector.X;
        scaled *= 2F;
        scaled--;
        PackedValue = HalfTypeHelper.Pack(scaled);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToScaledVector4()
    {
        var single = ToSingle() + 1F;
        single /= 2F;
        return new Vector4(single, 0, 0, 1F);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromVector4(Vector4 vector)
    {
        PackedValue = HalfTypeHelper.Pack(vector.X);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToVector4()
    {
        return new Vector4(ToSingle(), 0, 0, 1F);
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
    ///     Expands the packed representation into a <see cref="float" />.
    /// </summary>
    /// <returns>The <see cref="float" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly float ToSingle()
    {
        return HalfTypeHelper.Unpack(PackedValue);
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is HalfSingle other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(HalfSingle other)
    {
        return PackedValue.Equals(other.PackedValue);
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return FormattableString.Invariant($"HalfSingle({ToSingle():#0.##})");
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly override int GetHashCode()
    {
        return PackedValue.GetHashCode();
    }
}