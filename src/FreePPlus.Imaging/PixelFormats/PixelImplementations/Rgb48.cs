// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing three 16-bit unsigned normalized values ranging from 0 to 635535.
///     <para>
///         Ranges from [0, 0, 0, 1] to [1, 1, 1, 1] in vector form.
///     </para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct Rgb48 : IPixel<Rgb48>
{
    private const float Max = ushort.MaxValue;

    /// <summary>
    ///     Gets or sets the red component.
    /// </summary>
    public ushort R;

    /// <summary>
    ///     Gets or sets the green component.
    /// </summary>
    public ushort G;

    /// <summary>
    ///     Gets or sets the blue component.
    /// </summary>
    public ushort B;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgb48" /> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    public Rgb48(ushort r, ushort g, ushort b)
        : this()
    {
        R = r;
        G = g;
        B = b;
    }

    /// <summary>
    ///     Compares two <see cref="Rgb48" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Rgb48" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rgb48" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(Rgb48 left, Rgb48 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="Rgb48" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Rgb48" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rgb48" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(Rgb48 left, Rgb48 right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public readonly PixelOperations<Rgb48> CreatePixelOperations()
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
        vector = Vector4Utilities.FastClamp(vector, Vector4.Zero, Vector4.One) * Max;
        R = (ushort)MathF.Round(vector.X);
        G = (ushort)MathF.Round(vector.Y);
        B = (ushort)MathF.Round(vector.Z);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToVector4()
    {
        return new Vector4(R / Max, G / Max, B / Max, 1F);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromArgb32(Argb32 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgr24(Bgr24 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra32(Bgra32 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba64(Rgba64 source)
    {
        this = source.Rgb;
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
        var rgb = ImageMaths.UpscaleFrom8BitTo16Bit(source.PackedValue);
        R = rgb;
        G = rgb;
        B = rgb;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL16(L16 source)
    {
        R = source.PackedValue;
        G = source.PackedValue;
        B = source.PackedValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa16(La16 source)
    {
        var rgb = ImageMaths.UpscaleFrom8BitTo16Bit(source.L);
        R = rgb;
        G = rgb;
        B = rgb;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa32(La32 source)
    {
        R = source.L;
        G = source.L;
        B = source.L;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb24(Rgb24 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba32(Rgba32 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ToRgba32(ref Rgba32 dest)
    {
        dest.R = ImageMaths.DownScaleFrom16BitTo8Bit(R);
        dest.G = ImageMaths.DownScaleFrom16BitTo8Bit(G);
        dest.B = ImageMaths.DownScaleFrom16BitTo8Bit(B);
        dest.A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb48(Rgb48 source)
    {
        this = source;
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is Rgb48 rgb48 && Equals(rgb48);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(Rgb48 other)
    {
        return R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return $"Rgb48({R}, {G}, {B})";
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly override int GetHashCode()
    {
        return HashCode.Combine(R, G, B);
    }
}