// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.ColorSpaces;

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Pixel type containing three 8-bit unsigned normalized values ranging from 0 to 255.
///     The color components are stored in red, green, blue order (least significant to most significant byte).
///     <para>
///         Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
///     </para>
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public partial struct Rgb24 : IPixel<Rgb24>
{
    /// <summary>
    ///     The red component.
    /// </summary>
    [FieldOffset(0)] public byte R;

    /// <summary>
    ///     The green component.
    /// </summary>
    [FieldOffset(1)] public byte G;

    /// <summary>
    ///     The blue component.
    /// </summary>
    [FieldOffset(2)] public byte B;

    private static readonly Vector4 MaxBytes = new(byte.MaxValue);
    private static readonly Vector4 Half = new(0.5F);

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgb24" /> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgb24(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    /// <summary>
    ///     Converts an <see cref="Rgb24" /> to <see cref="Color" />.
    /// </summary>
    /// <param name="source">The <see cref="Rgb24" />.</param>
    /// <returns>The <see cref="Color" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Color(Rgb24 source)
    {
        return new Color(source);
    }

    /// <summary>
    ///     Converts a <see cref="Color" /> to <see cref="Rgb24" />.
    /// </summary>
    /// <param name="color">The <see cref="Color" />.</param>
    /// <returns>The <see cref="Rgb24" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Rgb24(Color color)
    {
        return color.ToRgb24();
    }

    /// <summary>
    ///     Allows the implicit conversion of an instance of <see cref="ColorSpaces.Rgb" /> to a
    ///     <see cref="Rgb24" />.
    /// </summary>
    /// <param name="color">The instance of <see cref="ColorSpaces.Rgb" /> to convert.</param>
    /// <returns>An instance of <see cref="Rgb24" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Rgb24(Rgb color)
    {
        var vector = new Vector4(color.ToVector3(), 1F);

        Rgb24 rgb = default;
        rgb.FromScaledVector4(vector);
        return rgb;
    }

    /// <summary>
    ///     Compares two <see cref="Rgb24" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Rgb24" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rgb24" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(Rgb24 left, Rgb24 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="Rgb24" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Rgb24" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rgb24" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(Rgb24 left, Rgb24 right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public readonly PixelOperations<Rgb24> CreatePixelOperations()
    {
        return new PixelOperations();
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
        Pack(ref vector);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToVector4()
    {
        return new Rgba32(R, G, B, byte.MaxValue).ToVector4();
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromArgb32(Argb32 source)
    {
        R = source.R;
        G = source.G;
        B = source.B;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgr24(Bgr24 source)
    {
        R = source.R;
        G = source.G;
        B = source.B;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra32(Bgra32 source)
    {
        R = source.R;
        G = source.G;
        B = source.B;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL8(L8 source)
    {
        R = source.PackedValue;
        G = source.PackedValue;
        B = source.PackedValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL16(L16 source)
    {
        var rgb = ImageMaths.DownScaleFrom16BitTo8Bit(source.PackedValue);
        R = rgb;
        G = rgb;
        B = rgb;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa16(La16 source)
    {
        R = source.L;
        G = source.L;
        B = source.L;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa32(La32 source)
    {
        var rgb = ImageMaths.DownScaleFrom16BitTo8Bit(source.L);
        R = rgb;
        G = rgb;
        B = rgb;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra5551(Bgra5551 source)
    {
        FromScaledVector4(source.ToScaledVector4());
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb24(Rgb24 source)
    {
        this = source;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba32(Rgba32 source)
    {
        this = source.Rgb;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ToRgba32(ref Rgba32 dest)
    {
        dest.R = R;
        dest.G = G;
        dest.B = B;
        dest.A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb48(Rgb48 source)
    {
        R = ImageMaths.DownScaleFrom16BitTo8Bit(source.R);
        G = ImageMaths.DownScaleFrom16BitTo8Bit(source.G);
        B = ImageMaths.DownScaleFrom16BitTo8Bit(source.B);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba64(Rgba64 source)
    {
        R = ImageMaths.DownScaleFrom16BitTo8Bit(source.R);
        G = ImageMaths.DownScaleFrom16BitTo8Bit(source.G);
        B = ImageMaths.DownScaleFrom16BitTo8Bit(source.B);
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is Rgb24 other && Equals(other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(Rgb24 other)
    {
        return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly override int GetHashCode()
    {
        return HashCode.Combine(R, B, G);
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return $"Rgb24({R}, {G}, {B})";
    }

    /// <summary>
    ///     Packs a <see cref="Vector4" /> into a color.
    /// </summary>
    /// <param name="vector">The vector containing the values to pack.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    private void Pack(ref Vector4 vector)
    {
        vector *= MaxBytes;
        vector += Half;
        vector = Vector4Utilities.FastClamp(vector, Vector4.Zero, MaxBytes);

        R = (byte)vector.X;
        G = (byte)vector.Y;
        B = (byte)vector.Z;
    }
}