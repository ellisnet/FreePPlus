// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable IDE0251

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing four 8-bit unsigned normalized values ranging from 0 to 255.
///     The color components are stored in blue, green, red, and alpha order (least significant to most significant byte).
///     The format is binary compatible with System.Drawing.Imaging.PixelFormat.Format32bppArgb
///     <para>
///         Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
///     </para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct Bgra32 : IPixel<Bgra32>, IPackedVector<uint>
{
    /// <summary>
    ///     Gets or sets the blue component.
    /// </summary>
    public byte B;

    /// <summary>
    ///     Gets or sets the green component.
    /// </summary>
    public byte G;

    /// <summary>
    ///     Gets or sets the red component.
    /// </summary>
    public byte R;

    /// <summary>
    ///     Gets or sets the alpha component.
    /// </summary>
    public byte A;

    /// <summary>
    ///     The maximum byte value.
    /// </summary>
    private static readonly Vector4 MaxBytes = new(255);

    /// <summary>
    ///     The half vector value.
    /// </summary>
    private static readonly Vector4 Half = new(0.5F);

    /// <summary>
    ///     Initializes a new instance of the <see cref="Bgra32" /> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Bgra32(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
        A = byte.MaxValue;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Bgra32" /> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="a">The alpha component.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Bgra32(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    ///     Gets or sets the packed representation of the Bgra32 struct.
    /// </summary>
    public uint Bgra
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        readonly get => Unsafe.As<Bgra32, uint>(ref Unsafe.AsRef(in this));

        [MethodImpl(InliningOptions.ShortMethod)]
        set => Unsafe.As<Bgra32, uint>(ref this) = value;
    }

    /// <inheritdoc />
    public uint PackedValue
    {
        readonly get => Bgra;
        set => Bgra = value;
    }

    /// <summary>
    ///     Converts an <see cref="Bgra32" /> to <see cref="Color" />.
    /// </summary>
    /// <param name="source">The <see cref="Bgra32" />.</param>
    /// <returns>The <see cref="Color" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Color(Bgra32 source)
    {
        return new Color(source);
    }

    /// <summary>
    ///     Converts a <see cref="Color" /> to <see cref="Bgra32" />.
    /// </summary>
    /// <param name="color">The <see cref="Color" />.</param>
    /// <returns>The <see cref="Bgra32" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Bgra32(Color color)
    {
        return color.ToBgra32();
    }

    /// <summary>
    ///     Compares two <see cref="Bgra32" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Bgra32" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Bgra32" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(Bgra32 left, Bgra32 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="Bgra32" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Bgra32" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Bgra32" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(Bgra32 left, Bgra32 right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public readonly PixelOperations<Bgra32> CreatePixelOperations()
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
        return new Vector4(R, G, B, A) / MaxBytes;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromArgb32(Argb32 source)
    {
        R = source.R;
        G = source.G;
        B = source.B;
        A = source.A;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgr24(Bgr24 source)
    {
        R = source.R;
        G = source.G;
        B = source.B;
        A = byte.MaxValue;
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
        this = source;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL8(L8 source)
    {
        R = source.PackedValue;
        G = source.PackedValue;
        B = source.PackedValue;
        A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromL16(L16 source)
    {
        var rgb = ImageMaths.DownScaleFrom16BitTo8Bit(source.PackedValue);
        R = rgb;
        G = rgb;
        B = rgb;
        A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa16(La16 source)
    {
        R = source.L;
        G = source.L;
        B = source.L;
        A = source.A;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromLa32(La32 source)
    {
        var rgb = ImageMaths.DownScaleFrom16BitTo8Bit(source.L);
        R = rgb;
        G = rgb;
        B = rgb;
        A = ImageMaths.DownScaleFrom16BitTo8Bit(source.A);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba32(Rgba32 source)
    {
        R = source.R;
        G = source.G;
        B = source.B;
        A = source.A;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb24(Rgb24 source)
    {
        R = source.R;
        G = source.G;
        B = source.B;
        A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ToRgba32(ref Rgba32 dest)
    {
        dest.R = R;
        dest.G = G;
        dest.B = B;
        dest.A = A;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgb48(Rgb48 source)
    {
        R = ImageMaths.DownScaleFrom16BitTo8Bit(source.R);
        G = ImageMaths.DownScaleFrom16BitTo8Bit(source.G);
        B = ImageMaths.DownScaleFrom16BitTo8Bit(source.B);
        A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba64(Rgba64 source)
    {
        R = ImageMaths.DownScaleFrom16BitTo8Bit(source.R);
        G = ImageMaths.DownScaleFrom16BitTo8Bit(source.G);
        B = ImageMaths.DownScaleFrom16BitTo8Bit(source.B);
        A = ImageMaths.DownScaleFrom16BitTo8Bit(source.A);
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is Bgra32 other && Equals(other);
    }

    /// <inheritdoc />
    public readonly bool Equals(Bgra32 other)
    {
        return Bgra.Equals(other.Bgra);
    }

    /// <inheritdoc />
    public readonly override int GetHashCode()
    {
        return Bgra.GetHashCode();
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return $"Bgra32({B}, {G}, {R}, {A})";
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
        A = (byte)vector.W;
    }
}