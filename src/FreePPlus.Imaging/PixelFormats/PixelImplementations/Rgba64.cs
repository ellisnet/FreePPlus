// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable IDE0251

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing four 16-bit unsigned normalized values ranging from 0 to 65535.
///     <para>
///         Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
///     </para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct Rgba64 : IPixel<Rgba64>, IPackedVector<ulong>
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
    ///     Gets or sets the alpha component.
    /// </summary>
    public ushort A;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba64" /> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="a">The alpha component.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba64(ushort r, ushort g, ushort b, ushort a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba64" /> struct.
    /// </summary>
    /// <param name="source">A structure of 4 bytes in RGBA byte order.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba64(Rgba32 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
        A = ImageMaths.UpscaleFrom8BitTo16Bit(source.A);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba64" /> struct.
    /// </summary>
    /// <param name="source">A structure of 4 bytes in BGRA byte order.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba64(Bgra32 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
        A = ImageMaths.UpscaleFrom8BitTo16Bit(source.A);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba64" /> struct.
    /// </summary>
    /// <param name="source">A structure of 4 bytes in ARGB byte order.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba64(Argb32 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
        A = ImageMaths.UpscaleFrom8BitTo16Bit(source.A);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba64" /> struct.
    /// </summary>
    /// <param name="source">A structure of 3 bytes in RGB byte order.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba64(Rgb24 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
        A = ushort.MaxValue;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba64" /> struct.
    /// </summary>
    /// <param name="source">A structure of 3 bytes in BGR byte order.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba64(Bgr24 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
        A = ushort.MaxValue;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba64" /> struct.
    /// </summary>
    /// <param name="vector">The <see cref="Vector4" />.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba64(Vector4 vector)
    {
        vector = Vector4Utilities.FastClamp(vector, Vector4.Zero, Vector4.One) * Max;
        R = (ushort)MathF.Round(vector.X);
        G = (ushort)MathF.Round(vector.Y);
        B = (ushort)MathF.Round(vector.Z);
        A = (ushort)MathF.Round(vector.W);
    }

    /// <summary>
    ///     Gets or sets the RGB components of this struct as <see cref="Rgb48" />.
    /// </summary>
    public Rgb48 Rgb
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        readonly get => Unsafe.As<Rgba64, Rgb48>(ref Unsafe.AsRef(in this));

        [MethodImpl(InliningOptions.ShortMethod)]
        set => Unsafe.As<Rgba64, Rgb48>(ref this) = value;
    }

    /// <inheritdoc />
    public ulong PackedValue
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        readonly get => Unsafe.As<Rgba64, ulong>(ref Unsafe.AsRef(in this));

        [MethodImpl(InliningOptions.ShortMethod)]
        set => Unsafe.As<Rgba64, ulong>(ref this) = value;
    }

    /// <summary>
    ///     Converts a <see cref="Rgba64" /> to <see cref="Color" />.
    /// </summary>
    /// <param name="source">The <see cref="Rgba64" />.</param>
    /// <returns>The <see cref="Color" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Color(Rgba64 source)
    {
        return new Color(source);
    }

    /// <summary>
    ///     Converts a <see cref="Color" /> to <see cref="Rgba64" />.
    /// </summary>
    /// <param name="color">The <see cref="Color" />.</param>
    /// <returns>The <see cref="Rgba64" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Rgba64(Color color)
    {
        return color.ToPixel<Rgba64>();
    }

    /// <summary>
    ///     Compares two <see cref="Rgba64" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Rgba64" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rgba64" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(Rgba64 left, Rgba64 right)
    {
        return left.PackedValue == right.PackedValue;
    }

    /// <summary>
    ///     Compares two <see cref="Rgba64" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Rgba64" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rgba64" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(Rgba64 left, Rgba64 right)
    {
        return left.PackedValue != right.PackedValue;
    }

    /// <inheritdoc />
    public readonly PixelOperations<Rgba64> CreatePixelOperations()
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
        A = (ushort)MathF.Round(vector.W);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Vector4 ToVector4()
    {
        return new Vector4(R, G, B, A) / Max;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromArgb32(Argb32 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
        A = ImageMaths.UpscaleFrom8BitTo16Bit(source.A);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgr24(Bgr24 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
        A = ushort.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra32(Bgra32 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
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
    public void FromL8(L8 source)
    {
        var rgb = ImageMaths.UpscaleFrom8BitTo16Bit(source.PackedValue);
        R = rgb;
        G = rgb;
        B = rgb;
        A = ushort.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL16(L16 source)
    {
        R = source.PackedValue;
        G = source.PackedValue;
        B = source.PackedValue;
        A = ushort.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa16(La16 source)
    {
        var rgb = ImageMaths.UpscaleFrom8BitTo16Bit(source.L);
        R = rgb;
        G = rgb;
        B = rgb;
        A = ImageMaths.UpscaleFrom8BitTo16Bit(source.A);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa32(La32 source)
    {
        R = source.L;
        G = source.L;
        B = source.L;
        A = source.A;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb24(Rgb24 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
        A = ushort.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba32(Rgba32 source)
    {
        R = ImageMaths.UpscaleFrom8BitTo16Bit(source.R);
        G = ImageMaths.UpscaleFrom8BitTo16Bit(source.G);
        B = ImageMaths.UpscaleFrom8BitTo16Bit(source.B);
        A = ImageMaths.UpscaleFrom8BitTo16Bit(source.A);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ToRgba32(ref Rgba32 dest)
    {
        dest.R = ImageMaths.DownScaleFrom16BitTo8Bit(R);
        dest.G = ImageMaths.DownScaleFrom16BitTo8Bit(G);
        dest.B = ImageMaths.DownScaleFrom16BitTo8Bit(B);
        dest.A = ImageMaths.DownScaleFrom16BitTo8Bit(A);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb48(Rgb48 source)
    {
        Rgb = source;
        A = ushort.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba64(Rgba64 source)
    {
        this = source;
    }

    /// <summary>
    ///     Convert to <see cref="Rgba32" />.
    /// </summary>
    /// <returns>The <see cref="Rgba32" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Rgba32 ToRgba32()
    {
        var r = ImageMaths.DownScaleFrom16BitTo8Bit(R);
        var g = ImageMaths.DownScaleFrom16BitTo8Bit(G);
        var b = ImageMaths.DownScaleFrom16BitTo8Bit(B);
        var a = ImageMaths.DownScaleFrom16BitTo8Bit(A);
        return new Rgba32(r, g, b, a);
    }

    /// <summary>
    ///     Convert to <see cref="Bgra32" />.
    /// </summary>
    /// <returns>The <see cref="Bgra32" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Bgra32 ToBgra32()
    {
        var r = ImageMaths.DownScaleFrom16BitTo8Bit(R);
        var g = ImageMaths.DownScaleFrom16BitTo8Bit(G);
        var b = ImageMaths.DownScaleFrom16BitTo8Bit(B);
        var a = ImageMaths.DownScaleFrom16BitTo8Bit(A);
        return new Bgra32(r, g, b, a);
    }

    /// <summary>
    ///     Convert to <see cref="Argb32" />.
    /// </summary>
    /// <returns>The <see cref="Argb32" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Argb32 ToArgb32()
    {
        var r = ImageMaths.DownScaleFrom16BitTo8Bit(R);
        var g = ImageMaths.DownScaleFrom16BitTo8Bit(G);
        var b = ImageMaths.DownScaleFrom16BitTo8Bit(B);
        var a = ImageMaths.DownScaleFrom16BitTo8Bit(A);
        return new Argb32(r, g, b, a);
    }

    /// <summary>
    ///     Convert to <see cref="Rgb24" />.
    /// </summary>
    /// <returns>The <see cref="Rgb24" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Rgb24 ToRgb24()
    {
        var r = ImageMaths.DownScaleFrom16BitTo8Bit(R);
        var g = ImageMaths.DownScaleFrom16BitTo8Bit(G);
        var b = ImageMaths.DownScaleFrom16BitTo8Bit(B);
        return new Rgb24(r, g, b);
    }

    /// <summary>
    ///     Convert to <see cref="Bgr24" />.
    /// </summary>
    /// <returns>The <see cref="Bgr24" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly Bgr24 ToBgr24()
    {
        var r = ImageMaths.DownScaleFrom16BitTo8Bit(R);
        var g = ImageMaths.DownScaleFrom16BitTo8Bit(G);
        var b = ImageMaths.DownScaleFrom16BitTo8Bit(B);
        return new Bgr24(r, g, b);
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is Rgba64 rgba64 && Equals(rgba64);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(Rgba64 other)
    {
        return PackedValue.Equals(other.PackedValue);
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return $"Rgba64({R}, {G}, {B}, {A})";
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly override int GetHashCode()
    {
        return PackedValue.GetHashCode();
    }
}