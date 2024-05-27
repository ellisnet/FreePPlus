// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable IDE0251
#pragma warning disable IDE0290

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing two 16-bit normalized values representing luminance and alpha.
///     <para>
///         Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
///     </para>
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public partial struct La32 : IPixel<La32>, IPackedVector<uint>
{
    private const float Max = ushort.MaxValue;

    /// <summary>
    ///     Gets or sets the luminance component.
    /// </summary>
    [FieldOffset(0)] public ushort L;

    /// <summary>
    ///     Gets or sets the alpha component.
    /// </summary>
    [FieldOffset(2)] public ushort A;

    /// <summary>
    ///     Initializes a new instance of the <see cref="La32" /> struct.
    /// </summary>
    /// <param name="l">The luminance component.</param>
    /// <param name="a">The alpha componant.</param>
    public La32(ushort l, ushort a)
    {
        L = l;
        A = a;
    }

    /// <inheritdoc />
    public uint PackedValue
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        readonly get => Unsafe.As<La32, uint>(ref Unsafe.AsRef(in this));

        [MethodImpl(InliningOptions.ShortMethod)]
        set => Unsafe.As<La32, uint>(ref this) = value;
    }

    /// <summary>
    ///     Compares two <see cref="La32" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="La32" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="La32" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(La32 left, La32 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="La32" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="La32" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="La32" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(La32 left, La32 right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public readonly PixelOperations<La32> CreatePixelOperations()
    {
        return new PixelOperations();
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(La32 other)
    {
        return PackedValue.Equals(other.PackedValue);
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is La32 other && Equals(other);
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return $"La32({L}, {A})";
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
        L = ImageMaths.Get16BitBT709Luminance(
            ImageMaths.UpscaleFrom8BitTo16Bit(source.R),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.G),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.B));

        A = ImageMaths.UpscaleFrom8BitTo16Bit(source.A);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgr24(Bgr24 source)
    {
        L = ImageMaths.Get16BitBT709Luminance(
            ImageMaths.UpscaleFrom8BitTo16Bit(source.R),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.G),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.B));

        A = ushort.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra32(Bgra32 source)
    {
        L = ImageMaths.Get16BitBT709Luminance(
            ImageMaths.UpscaleFrom8BitTo16Bit(source.R),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.G),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.B));

        A = ImageMaths.UpscaleFrom8BitTo16Bit(source.A);
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
        L = source.PackedValue;
        A = ushort.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL8(L8 source)
    {
        L = ImageMaths.UpscaleFrom8BitTo16Bit(source.PackedValue);
        A = ushort.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa16(La16 source)
    {
        L = ImageMaths.UpscaleFrom8BitTo16Bit(source.L);
        A = ImageMaths.UpscaleFrom8BitTo16Bit(source.A);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa32(La32 source)
    {
        this = source;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb24(Rgb24 source)
    {
        L = ImageMaths.Get16BitBT709Luminance(
            ImageMaths.UpscaleFrom8BitTo16Bit(source.R),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.G),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.B));

        A = ushort.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb48(Rgb48 source)
    {
        L = ImageMaths.Get16BitBT709Luminance(source.R, source.G, source.B);
        A = ushort.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba32(Rgba32 source)
    {
        L = ImageMaths.Get16BitBT709Luminance(
            ImageMaths.UpscaleFrom8BitTo16Bit(source.R),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.G),
            ImageMaths.UpscaleFrom8BitTo16Bit(source.B));

        A = ImageMaths.UpscaleFrom8BitTo16Bit(source.A);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba64(Rgba64 source)
    {
        L = ImageMaths.Get16BitBT709Luminance(source.R, source.G, source.B);
        A = source.A;
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
        var rgb = ImageMaths.DownScaleFrom16BitTo8Bit(L);
        dest.R = rgb;
        dest.G = rgb;
        dest.B = rgb;
        dest.A = ImageMaths.DownScaleFrom16BitTo8Bit(A);
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
        var scaled = L / Max;
        return new Vector4(scaled, scaled, scaled, A / Max);
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    internal void ConvertFromRgbaScaledVector4(Vector4 vector)
    {
        vector = Vector4Utilities.FastClamp(vector, Vector4.Zero, Vector4.One) * Max;
        L = ImageMaths.Get16BitBT709Luminance(
            vector.X,
            vector.Y,
            vector.Z);

        A = (ushort)MathF.Round(vector.W);
    }
}