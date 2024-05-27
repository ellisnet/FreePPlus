// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.ColorSpaces;

#pragma warning disable IDE0251

namespace FreePPlus.Imaging.PixelFormats;

//was previously: namespace SixLabors.ImageSharp.PixelFormats;

/// <summary>
///     Packed pixel type containing four 8-bit unsigned normalized values ranging from 0 to 255.
///     The color components are stored in red, green, blue, and alpha order (least significant to most significant byte).
///     <para>
///         Ranges from [0, 0, 0, 0] to [1, 1, 1, 1] in vector form.
///     </para>
/// </summary>
/// <remarks>
///     This struct is fully mutable. This is done (against the guidelines) for the sake of performance,
///     as it avoids the need to create new values for modification operations.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public partial struct Rgba32 : IPixel<Rgba32>, IPackedVector<uint>
{
    /// <summary>
    ///     Gets or sets the red component.
    /// </summary>
    public byte R;

    /// <summary>
    ///     Gets or sets the green component.
    /// </summary>
    public byte G;

    /// <summary>
    ///     Gets or sets the blue component.
    /// </summary>
    public byte B;

    /// <summary>
    ///     Gets or sets the alpha component.
    /// </summary>
    public byte A;

    private static readonly Vector4 MaxBytes = new(byte.MaxValue);
    private static readonly Vector4 Half = new(0.5F);

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba32" /> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba32(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
        A = byte.MaxValue;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba32" /> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="a">The alpha component.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba32(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba32" /> struct.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="a">The alpha component.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba32(float r, float g, float b, float a = 1)
        : this()
    {
        Pack(r, g, b, a);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba32" /> struct.
    /// </summary>
    /// <param name="vector">
    ///     The vector containing the components for the packed vector.
    /// </param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba32(Vector3 vector)
        : this()
    {
        Pack(ref vector);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba32" /> struct.
    /// </summary>
    /// <param name="vector">
    ///     The vector containing the components for the packed vector.
    /// </param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba32(Vector4 vector)
        : this()
    {
        this = PackNew(ref vector);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Rgba32" /> struct.
    /// </summary>
    /// <param name="packed">
    ///     The packed value.
    /// </param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgba32(uint packed)
        : this()
    {
        Rgba = packed;
    }

    /// <summary>
    ///     Gets or sets the packed representation of the Rgba32 struct.
    /// </summary>
    public uint Rgba
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        readonly get => Unsafe.As<Rgba32, uint>(ref Unsafe.AsRef(in this));

        [MethodImpl(InliningOptions.ShortMethod)]
        set => Unsafe.As<Rgba32, uint>(ref this) = value;
    }

    /// <summary>
    ///     Gets or sets the RGB components of this struct as <see cref="Rgb24" />
    /// </summary>
    public Rgb24 Rgb
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        readonly get => new(R, G, B);

        [MethodImpl(InliningOptions.ShortMethod)]
        set
        {
            R = value.R;
            G = value.G;
            B = value.B;
        }
    }

    /// <summary>
    ///     Gets or sets the RGB components of this struct as <see cref="Bgr24" /> reverting the component order.
    /// </summary>
    public Bgr24 Bgr
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        readonly get => new(R, G, B);

        [MethodImpl(InliningOptions.ShortMethod)]
        set
        {
            R = value.R;
            G = value.G;
            B = value.B;
        }
    }

    /// <inheritdoc />
    public uint PackedValue
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        readonly get => Rgba;

        [MethodImpl(InliningOptions.ShortMethod)]
        set => Rgba = value;
    }

    /// <summary>
    ///     Converts an <see cref="Rgba32" /> to <see cref="Color" />.
    /// </summary>
    /// <param name="source">The <see cref="Rgba32" />.</param>
    /// <returns>The <see cref="Color" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Color(Rgba32 source)
    {
        return new Color(source);
    }

    /// <summary>
    ///     Converts a <see cref="Color" /> to <see cref="Rgba32" />.
    /// </summary>
    /// <param name="color">The <see cref="Color" />.</param>
    /// <returns>The <see cref="Rgba32" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Rgba32(Color color)
    {
        return color.ToRgba32();
    }

    /// <summary>
    ///     Allows the implicit conversion of an instance of <see cref="ColorSpaces.Rgb" /> to a
    ///     <see cref="Rgba32" />.
    /// </summary>
    /// <param name="color">The instance of <see cref="ColorSpaces.Rgb" /> to convert.</param>
    /// <returns>An instance of <see cref="Rgba32" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static implicit operator Rgba32(Rgb color)
    {
        var vector = new Vector4(color.ToVector3(), 1F);

        Rgba32 rgba = default;
        rgba.FromScaledVector4(vector);
        return rgba;
    }

    /// <summary>
    ///     Compares two <see cref="Rgba32" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Rgba32" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rgba32" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator ==(Rgba32 left, Rgba32 right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="Rgba32" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="Rgba32" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="Rgba32" /> on the right side of the operand.</param>
    /// <returns>
    ///     True if the <paramref name="left" /> parameter is not equal to the <paramref name="right" /> parameter; otherwise,
    ///     false.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool operator !=(Rgba32 left, Rgba32 right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="Rgba32" /> struct
    ///     from the given hexadecimal string.
    /// </summary>
    /// <param name="hex">
    ///     The hexadecimal representation of the combined color components arranged
    ///     in rgb, rgba, rrggbb, or rrggbbaa format to match web syntax.
    /// </param>
    /// <returns>
    ///     The <see cref="Rgba32" />.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static Rgba32 ParseHex(string hex)
    {
        Guard.NotNull(hex, nameof(hex));

        if (!TryParseHex(hex, out var rgba))
            throw new ArgumentException("Hexadecimal string is not in the correct format.", nameof(hex));

        return rgba;
    }

    /// <summary>
    ///     Attempts to creates a new instance of the <see cref="Rgba32" /> struct
    ///     from the given hexadecimal string.
    /// </summary>
    /// <param name="hex">
    ///     The hexadecimal representation of the combined color components arranged
    ///     in rgb, rgba, rrggbb, or rrggbbaa format to match web syntax.
    /// </param>
    /// <param name="result">When this method returns, contains the <see cref="Rgba32" /> equivalent of the hexadecimal input.</param>
    /// <returns>
    ///     The <see cref="bool" />.
    /// </returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static bool TryParseHex(string hex, out Rgba32 result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(hex)) return false;

        hex = ToRgbaHex(hex);

        if (hex is null ||
            !uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var packedValue))
            return false;

        packedValue = BinaryPrimitives.ReverseEndianness(packedValue);
        result = Unsafe.As<uint, Rgba32>(ref packedValue);
        return true;
    }

    /// <inheritdoc />
    public readonly PixelOperations<Rgba32> CreatePixelOperations()
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
        Bgr = source;
        A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromBgra32(Bgra32 source)
    {
        R = source.R;
        G = source.G;
        B = source.B;
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
    public void FromRgb24(Rgb24 source)
    {
        Rgb = source;
        A = byte.MaxValue;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void FromRgba32(Rgba32 source)
    {
        this = source;
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ToRgba32(ref Rgba32 dest)
    {
        dest = this;
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

    /// <summary>
    ///     Converts the value of this instance to a hexadecimal string.
    /// </summary>
    /// <returns>A hexadecimal string representation of the value.</returns>
    public readonly string ToHex()
    {
        var hexOrder = (uint)((A << 0) | (B << 8) | (G << 16) | (R << 24));
        return hexOrder.ToString("X8");
    }

    /// <inheritdoc />
    public readonly override bool Equals(object obj)
    {
        return obj is Rgba32 rgba32 && Equals(rgba32);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly bool Equals(Rgba32 other)
    {
        return Rgba.Equals(other.Rgba);
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return $"Rgba32({R}, {G}, {B}, {A})";
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public readonly override int GetHashCode()
    {
        return Rgba.GetHashCode();
    }

    /// <summary>
    ///     Packs a <see cref="Vector4" /> into a color returning a new instance as a result.
    /// </summary>
    /// <param name="vector">The vector containing the values to pack.</param>
    /// <returns>The <see cref="Rgba32" /></returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    private static Rgba32 PackNew(ref Vector4 vector)
    {
        vector *= MaxBytes;
        vector += Half;
        vector = Vector4Utilities.FastClamp(vector, Vector4.Zero, MaxBytes);

        return new Rgba32((byte)vector.X, (byte)vector.Y, (byte)vector.Z, (byte)vector.W);
    }

    /// <summary>
    ///     Packs the four floats into a color.
    /// </summary>
    /// <param name="x">The x-component</param>
    /// <param name="y">The y-component</param>
    /// <param name="z">The z-component</param>
    /// <param name="w">The w-component</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    private void Pack(float x, float y, float z, float w)
    {
        var value = new Vector4(x, y, z, w);
        Pack(ref value);
    }

    /// <summary>
    ///     Packs a <see cref="Vector3" /> into a uint.
    /// </summary>
    /// <param name="vector">The vector containing the values to pack.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    private void Pack(ref Vector3 vector)
    {
        var value = new Vector4(vector, 1F);
        Pack(ref value);
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

    /// <summary>
    ///     Converts the specified hex value to an rrggbbaa hex value.
    /// </summary>
    /// <param name="hex">The hex value to convert.</param>
    /// <returns>
    ///     A rrggbbaa hex value.
    /// </returns>
    private static string ToRgbaHex(string hex)
    {
        if (hex[0] == '#') hex = hex[1..];

        if (hex.Length == 8) return hex;

        if (hex.Length == 6) return hex + "FF";

        if (hex.Length < 3 || hex.Length > 4) return null;

        var r = hex[0];
        var g = hex[1];
        var b = hex[2];
        var a = hex.Length == 3 ? 'F' : hex[3];

        return new string(new[] { r, r, g, g, b, b, a, a });
    }
}