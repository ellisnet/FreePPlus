// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing unsigned normalized values ranging from 0 to 1.
///     The x and z components use 5 bits, and the y component uses 6 bits.
///     <para>
///         Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
///     </para>
/// </summary>
public struct Bgr565 : IPixel<Bgr565>, IPackedVector<ushort>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Bgr565" /> struct.
    /// </summary>
    /// <param name="x">The x-component</param>
    /// <param name="y">The y-component</param>
    /// <param name="z">The z-component</param>
    public Bgr565(float x, float y, float z)
        : this(new Vector3(x, y, z)) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Bgr565" /> struct.
    /// </summary>
    /// <param name="vector">
    ///     The vector containing the components for the packed value.
    /// </param>
    public Bgr565(Vector3 vector)
    {
        PackedValue = Pack(ref vector);
    }

    /// <inheritdoc />
    public ushort PackedValue { get; set; }

    /// <summary>
    ///     Compares two <see cref="Bgr565" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Bgr565" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Bgr565" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(Bgr565 left, Bgr565 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="Bgr565" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Bgr565" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Bgr565" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(Bgr565 left, Bgr565 right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public readonly PixelOperations<Bgr565> CreatePixelOperations()
    {
        return new PixelOperations<Bgr565>();
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromScaledVector4(Vector4 vector)
    {
        FromVector4(vector);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToScaledVector4()
    {
        return ToVector4();
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromVector4(Vector4 vector)
    {
        var vector3 = new Vector3(vector.X, vector.Y, vector.Z);
        PackedValue = Pack(ref vector3);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToVector4()
    {
        return new Vector4(ToVector3(), 1F);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromArgb32(Argb32 source)
    {
        FromVector4(source.ToVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra5551(Bgra5551 source)
    {
        FromVector4(source.ToVector4());
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
        FromVector4(source.ToVector4());
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
        FromVector4(source.ToVector4());
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
    ///     Expands the packed representation into a <see cref="Vector3" />.
    ///     The vector components are typically expanded in least to greatest significance order.
    /// </summary>
    /// <returns>The <see cref="Vector3" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector3 ToVector3()
    {
        return new Vector3(
            ((PackedValue >> 11) & 0x1F) * (1F / 31F),
            ((PackedValue >> 5) & 0x3F) * (1F / 63F),
            (PackedValue & 0x1F) * (1F / 31F));
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is Bgr565 other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(Bgr565 other)
    {
        return PackedValue.Equals(other.PackedValue);
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        var vector = ToVector3();
        return FormattableString.Invariant($"Bgr565({vector.Z:#0.##}, {vector.Y:#0.##}, {vector.X:#0.##})");
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly override int GetHashCode()
    {
        return PackedValue.GetHashCode();
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static ushort Pack(ref Vector3 vector)
    {
        vector = Vector3.Clamp(vector, Vector3.Zero, Vector3.One);

        return (ushort)((((int)Math.Round(vector.X * 31F) & 0x1F) << 11)
                        | (((int)Math.Round(vector.Y * 63F) & 0x3F) << 5)
                        | ((int)Math.Round(vector.Z * 31F) & 0x1F));
    }
}