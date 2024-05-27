// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Pixel type containing three 8-bit unsigned normalized values ranging from 0 to 255.
///     The color components are stored in blue, green, red order (least significant to most significant byte).
///     <para>
///         Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
///     </para>
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public partial struct Bgr24 : IPixel<Bgr24>
{
    /// <summary>
    ///     The blue component.
    /// </summary>
    [FieldOffset(0)] public byte B;

    /// <summary>
    ///     The green component.
    /// </summary>
    [FieldOffset(1)] public byte G;

    /// <summary>
    ///     The red component.
    /// </summary>
    [FieldOffset(2)] public byte R;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Bgr24" /> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Bgr24(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    /// <summary>
    ///     Converts an <see cref="Bgr24" /> to <see cref="Color" />.
    /// </summary>
    /// <param name="source">The <see cref="Bgr24" />.</param>
    /// <returns>The <see cref="Color" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Color(Bgr24 source)
    {
        return new Color(source);
    }

    /// <summary>
    ///     Converts a <see cref="Color" /> to <see cref="Bgr24" />.
    /// </summary>
    /// <param name="color">The <see cref="Color" />.</param>
    /// <returns>The <see cref="Bgr24" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Bgr24(Color color)
    {
        return color.ToBgr24();
    }

    /// <summary>
    ///     Compares two <see cref="Bgr24" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Bgr24" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Bgr24" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(Bgr24 left, Bgr24 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="Bgr24" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Bgr24" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Bgr24" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(Bgr24 left, Bgr24 right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public readonly PixelOperations<Bgr24> CreatePixelOperations()
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
        Rgba32 rgba = default;
        rgba.FromVector4(vector);
        FromRgba32(rgba);
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
        this = source;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra5551(Bgra5551 source)
    {
        FromScaledVector4(source.ToScaledVector4());
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
    public void FromRgb24(Rgb24 source)
    {
        R = source.R;
        G = source.G;
        B = source.B;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba32(Rgba32 source)
    {
        this = source.Bgr;
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
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(Bgr24 other)
    {
        return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is Bgr24 other && Equals(other);
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return $"Bgra({B}, {G}, {R})";
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly override int GetHashCode()
    {
        return HashCode.Combine(R, B, G);
    }
}