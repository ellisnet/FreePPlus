// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing a single 8-bit normalized luminance value.
///     <para>
///         Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
///     </para>
/// </summary>
public partial struct L8 : IPixel<L8>, IPackedVector<byte>
{
    private static readonly Vector4 MaxBytes = new(255F);
    private static readonly Vector4 Half = new(0.5F);

    /// <summary>
    ///     Initializes a new instance of the <see cref="L8" /> struct.
    /// </summary>
    /// <param name="luminance">The luminance component.</param>
    public L8(byte luminance)
    {
        PackedValue = luminance;
    }

    /// <inheritdoc />
    public byte PackedValue { get; set; }

    /// <summary>
    ///     Compares two <see cref="L8" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="L8" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="L8" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(L8 left, L8 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="L8" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="L8" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="L8" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(L8 left, L8 right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public readonly PixelOperations<L8> CreatePixelOperations()
    {
        return new PixelOperations();
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromScaledVector4(Vector4 vector)
    {
        ConvertFromRgbaScaledVector4(vector);
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
        ConvertFromRgbaScaledVector4(vector);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToVector4()
    {
        var rgb = PackedValue / 255F;
        return new Vector4(rgb, rgb, rgb, 1F);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromArgb32(Argb32 source)
    {
        PackedValue = ImageMaths.Get8BitBT709Luminance(source.R, source.G, source.B);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgr24(Bgr24 source)
    {
        PackedValue = ImageMaths.Get8BitBT709Luminance(source.R, source.G, source.B);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra32(Bgra32 source)
    {
        PackedValue = ImageMaths.Get8BitBT709Luminance(source.R, source.G, source.B);
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
        this = source;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL16(L16 source)
    {
        PackedValue = ImageMaths.DownScaleFrom16BitTo8Bit(source.PackedValue);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa16(La16 source)
    {
        PackedValue = source.L;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa32(La32 source)
    {
        PackedValue = ImageMaths.DownScaleFrom16BitTo8Bit(source.L);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb24(Rgb24 source)
    {
        PackedValue = ImageMaths.Get8BitBT709Luminance(source.R, source.G, source.B);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba32(Rgba32 source)
    {
        PackedValue = ImageMaths.Get8BitBT709Luminance(source.R, source.G, source.B);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ToRgba32(ref Rgba32 dest)
    {
        dest.R = PackedValue;
        dest.G = PackedValue;
        dest.B = PackedValue;
        dest.A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb48(Rgb48 source)
    {
        PackedValue = ImageMaths.Get8BitBT709Luminance(
            ImageMaths.DownScaleFrom16BitTo8Bit(source.R),
            ImageMaths.DownScaleFrom16BitTo8Bit(source.G),
            ImageMaths.DownScaleFrom16BitTo8Bit(source.B));
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba64(Rgba64 source)
    {
        PackedValue = ImageMaths.Get8BitBT709Luminance(
            ImageMaths.DownScaleFrom16BitTo8Bit(source.R),
            ImageMaths.DownScaleFrom16BitTo8Bit(source.G),
            ImageMaths.DownScaleFrom16BitTo8Bit(source.B));
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is L8 other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(L8 other)
    {
        return PackedValue.Equals(other.PackedValue);
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return $"L8({PackedValue})";
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly override int GetHashCode()
    {
        return PackedValue.GetHashCode();
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    internal void ConvertFromRgbaScaledVector4(Vector4 vector)
    {
        vector *= MaxBytes;
        vector += Half;
        vector = Vector4Utilities.FastClamp(vector, Vector4.Zero, MaxBytes);
        PackedValue = ImageMaths.Get8BitBT709Luminance((byte)vector.X, (byte)vector.Y, (byte)vector.Z);
    }
}