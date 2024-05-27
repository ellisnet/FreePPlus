// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing a single 16-bit normalized luminance value.
///     <para>
///         Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
///     </para>
/// </summary>
public partial struct L16 : IPixel<L16>, IPackedVector<ushort>
{
    private const float Max = ushort.MaxValue;

    /// <summary>
    ///     Initializes a new instance of the <see cref="L16" /> struct.
    /// </summary>
    /// <param name="luminance">The luminance component</param>
    public L16(ushort luminance)
    {
        PackedValue = luminance;
    }

    /// <inheritdoc />
    public ushort PackedValue { get; set; }

    /// <summary>
    ///     Compares two <see cref="L16" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="L16" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="L16" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(L16 left, L16 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="L16" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="L16" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="L16" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(L16 left, L16 right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public readonly PixelOperations<L16> CreatePixelOperations()
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
        var scaled = PackedValue / Max;
        return new Vector4(scaled, scaled, scaled, 1F);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromArgb32(Argb32 source)
    {
        PackedValue = ImageMaths.Get16BitBT709Luminance(
            ImageMaths.UpscaleFrom8BitTo16Bit(source.R),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.G),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.B));
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgr24(Bgr24 source)
    {
        PackedValue = ImageMaths.Get16BitBT709Luminance(
            ImageMaths.UpscaleFrom8BitTo16Bit(source.R),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.G),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.B));
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra32(Bgra32 source)
    {
        PackedValue = ImageMaths.Get16BitBT709Luminance(
            ImageMaths.UpscaleFrom8BitTo16Bit(source.R),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.G),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.B));
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
        PackedValue = ImageMaths.UpscaleFrom8BitTo16Bit(source.PackedValue);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL16(L16 source)
    {
        this = source;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa16(La16 source)
    {
        PackedValue = ImageMaths.UpscaleFrom8BitTo16Bit(source.L);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa32(La32 source)
    {
        PackedValue = source.L;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb24(Rgb24 source)
    {
        PackedValue = ImageMaths.Get16BitBT709Luminance(
            ImageMaths.UpscaleFrom8BitTo16Bit(source.R),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.G),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.B));
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba32(Rgba32 source)
    {
        PackedValue = ImageMaths.Get16BitBT709Luminance(
            ImageMaths.UpscaleFrom8BitTo16Bit(source.R),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.G),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.B));
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ToRgba32(ref Rgba32 dest)
    {
        var rgb = ImageMaths.DownScaleFrom16BitTo8Bit(PackedValue);
        dest.R = rgb;
        dest.G = rgb;
        dest.B = rgb;
        dest.A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb48(Rgb48 source)
    {
        PackedValue = ImageMaths.Get16BitBT709Luminance(source.R, source.G, source.B);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba64(Rgba64 source)
    {
        PackedValue = ImageMaths.Get16BitBT709Luminance(source.R, source.G, source.B);
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is L16 other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(L16 other)
    {
        return PackedValue.Equals(other.PackedValue);
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return $"L16({PackedValue})";
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
        vector = Vector4Utilities.FastClamp(vector, Vector4.Zero, Vector4.One) * Max;
        PackedValue = ImageMaths.Get16BitBT709Luminance(
            vector.X,
            vector.Y,
            vector.Z);
    }
}