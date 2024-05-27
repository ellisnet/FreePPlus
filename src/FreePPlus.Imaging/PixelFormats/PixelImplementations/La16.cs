// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable IDE0251
#pragma warning disable IDE0290

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing two 8-bit normalized values representing luminance and alpha.
///     <para>
///         Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
///     </para>
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public partial struct La16 : IPixel<La16>, IPackedVector<ushort>
{
    private static readonly Vector4 MaxBytes = new(255F);
    private static readonly Vector4 Half = new(0.5F);

    /// <summary>
    ///     Gets or sets the luminance component.
    /// </summary>
    [FieldOffset(0)] public byte L;

    /// <summary>
    ///     Gets or sets the alpha component.
    /// </summary>
    [FieldOffset(1)] public byte A;

    /// <summary>
    ///     Initializes a new instance of the <see cref="La16" /> struct.
    /// </summary>
    /// <param name="l">The luminance component.</param>
    /// <param name="a">The alpha componant.</param>
    public La16(byte l, byte a)
    {
        L = l;
        A = a;
    }

    /// <inheritdoc />
    public ushort PackedValue
    {
        readonly get => Unsafe.As<La16, ushort>(ref Unsafe.AsRef(in this));
        set => Unsafe.As<La16, ushort>(ref this) = value;
    }

    /// <summary>
    ///     Compares two <see cref="La16" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="La16" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="La16" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(La16 left, La16 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="La16" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="La16" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="La16" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(La16 left, La16 right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public readonly PixelOperations<La16> CreatePixelOperations()
    {
        return new PixelOperations();
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(La16 other)
    {
        return PackedValue.Equals(other.PackedValue);
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is La16 other && Equals(other);
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return $"La16({L}, {A})";
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly override int GetHashCode()
    {
        return PackedValue.GetHashCode();
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromArgb32(Argb32 source)
    {
        L = ImageMaths.Get8BitBT709Luminance(source.R, source.G, source.B);
        A = source.A;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgr24(Bgr24 source)
    {
        L = ImageMaths.Get8BitBT709Luminance(source.R, source.G, source.B);
        A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra32(Bgra32 source)
    {
        L = ImageMaths.Get8BitBT709Luminance(source.R, source.G, source.B);
        A = source.A;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra5551(Bgra5551 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL16(L16 source)
    {
        L = ImageMaths.DownScaleFrom16BitTo8Bit(source.PackedValue);
        A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL8(L8 source)
    {
        L = source.PackedValue;
        A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa16(La16 source)
    {
        this = source;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa32(La32 source)
    {
        L = ImageMaths.DownScaleFrom16BitTo8Bit(source.L);
        A = ImageMaths.DownScaleFrom16BitTo8Bit(source.A);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb24(Rgb24 source)
    {
        L = ImageMaths.Get8BitBT709Luminance(source.R, source.G, source.B);
        A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb48(Rgb48 source)
    {
        L = ImageMaths.Get8BitBT709Luminance(
            ImageMaths.DownScaleFrom16BitTo8Bit(source.R),
            ImageMaths.DownScaleFrom16BitTo8Bit(source.G),
            ImageMaths.DownScaleFrom16BitTo8Bit(source.B));

        A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba32(Rgba32 source)
    {
        L = ImageMaths.Get8BitBT709Luminance(source.R, source.G, source.B);
        A = source.A;
    }

    /// <inheritdoc />
    public void FromRgba64(Rgba64 source)
    {
        L = ImageMaths.Get8BitBT709Luminance(
            ImageMaths.DownScaleFrom16BitTo8Bit(source.R),
            ImageMaths.DownScaleFrom16BitTo8Bit(source.G),
            ImageMaths.DownScaleFrom16BitTo8Bit(source.B));

        A = ImageMaths.DownScaleFrom16BitTo8Bit(source.A);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromScaledVector4(Vector4 vector)
    {
        ConvertFromRgbaScaledVector4(vector);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromVector4(Vector4 vector)
    {
        ConvertFromRgbaScaledVector4(vector);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ToRgba32(ref Rgba32 dest)
    {
        dest.R = L;
        dest.G = L;
        dest.B = L;
        dest.A = A;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToScaledVector4()
    {
        return ToVector4();
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToVector4()
    {
        const float Max = 255F;
        var rgb = L / Max;
        return new Vector4(rgb, rgb, rgb, A / Max);
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    internal void ConvertFromRgbaScaledVector4(Vector4 vector)
    {
        vector *= MaxBytes;
        vector += Half;
        vector = Vector4Utilities.FastClamp(vector, Vector4.Zero, MaxBytes);
        L = ImageMaths.Get8BitBT709Luminance((byte)vector.X, (byte)vector.Y, (byte)vector.Z);
        A = (byte)vector.W;
    }
}