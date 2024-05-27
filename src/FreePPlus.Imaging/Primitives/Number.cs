// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Represents an integral number.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct Number : IEquatable<Number>, IComparable<Number>
{
    [FieldOffset(0)] private readonly int signedValue;

    [FieldOffset(0)] private readonly uint unsignedValue;

    [FieldOffset(4)] private readonly bool isSigned;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Number" /> struct.
    /// </summary>
    /// <param name="value">The value of the number.</param>
    public Number(int value)
        : this()
    {
        signedValue = value;
        isSigned = true;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Number" /> struct.
    /// </summary>
    /// <param name="value">The value of the number.</param>
    public Number(uint value)
        : this()
    {
        unsignedValue = value;
        isSigned = false;
    }

    /// <summary>
    ///     Converts the specified <see cref="int" /> to an instance of this type.
    /// </summary>
    /// <param name="value">The value.</param>
    public static implicit operator Number(int value)
    {
        return new Number(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="uint" /> to an instance of this type.
    /// </summary>
    /// <param name="value">The value.</param>
    public static implicit operator Number(uint value)
    {
        return new Number(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="ushort" /> to an instance of this type.
    /// </summary>
    /// <param name="value">The value.</param>
    public static implicit operator Number(ushort value)
    {
        return new Number((uint)value);
    }

    /// <summary>
    ///     Converts the specified <see cref="Number" /> to a <see cref="int" />.
    /// </summary>
    /// <param name="number">The <see cref="Number" /> to convert.</param>
    public static explicit operator int(Number number)
    {
        return number.isSigned
            ? number.signedValue
            : (int)number.unsignedValue.Clamp(0, int.MaxValue);
    }

    /// <summary>
    ///     Converts the specified <see cref="Number" /> to a <see cref="uint" />.
    /// </summary>
    /// <param name="number">The <see cref="Number" /> to convert.</param>
    public static explicit operator uint(Number number)
    {
        return number.isSigned
            ? (uint)number.signedValue.Clamp(0, int.MaxValue)
            : number.unsignedValue;
    }

    /// <summary>
    ///     Converts the specified <see cref="Number" /> to a <see cref="ushort" />.
    /// </summary>
    /// <param name="number">The <see cref="Number" /> to convert.</param>
    public static explicit operator ushort(Number number)
    {
        return number.isSigned
            ? (ushort)number.signedValue.Clamp(ushort.MinValue, ushort.MaxValue)
            : (ushort)number.unsignedValue.Clamp(ushort.MinValue, ushort.MaxValue);
    }

    /// <summary>
    ///     Determines whether the specified <see cref="Number" /> instances are considered equal.
    /// </summary>
    /// <param name="left">The first <see cref="Number" /> to compare.</param>
    /// <param name="right"> The second <see cref="Number" /> to compare.</param>
    public static bool operator ==(Number left, Number right)
    {
        return Equals(left, right);
    }

    /// <summary>
    ///     Determines whether the specified <see cref="Number" /> instances are not considered equal.
    /// </summary>
    /// <param name="left">The first <see cref="Number" /> to compare.</param>
    /// <param name="right"> The second <see cref="Number" /> to compare.</param>
    public static bool operator !=(Number left, Number right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    ///     Determines whether the first <see cref="Number" /> is more than the second <see cref="Number" />.
    /// </summary>
    /// <param name="left">The first <see cref="Number" /> to compare.</param>
    /// <param name="right"> The second <see cref="Number" /> to compare.</param>
    public static bool operator >(Number left, Number right)
    {
        return left.CompareTo(right) == 1;
    }

    /// <summary>
    ///     Determines whether the first <see cref="Number" /> is less than the second <see cref="Number" />.
    /// </summary>
    /// <param name="left">The first <see cref="Number" /> to compare.</param>
    /// <param name="right"> The second <see cref="Number" /> to compare.</param>
    public static bool operator <(Number left, Number right)
    {
        return left.CompareTo(right) == -1;
    }

    /// <summary>
    ///     Determines whether the first <see cref="Number" /> is more than or equal to the second <see cref="Number" />.
    /// </summary>
    /// <param name="left">The first <see cref="Number" /> to compare.</param>
    /// <param name="right"> The second <see cref="Number" /> to compare.</param>
    public static bool operator >=(Number left, Number right)
    {
        return left.CompareTo(right) >= 0;
    }

    /// <summary>
    ///     Determines whether the first <see cref="Number" /> is less than or equal to the second <see cref="Number" />.
    /// </summary>
    /// <param name="left">The first <see cref="Number" /> to compare.</param>
    /// <param name="right"> The second <see cref="Number" /> to compare.</param>
    public static bool operator <=(Number left, Number right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <inheritdoc />
    public int CompareTo(Number other)
    {
        return isSigned
            ? signedValue.CompareTo(other.signedValue)
            : unsignedValue.CompareTo(other.unsignedValue);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is Number other && Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(Number other)
    {
        if (isSigned != other.isSigned) return false;

        return isSigned
            ? signedValue.Equals(other.signedValue)
            : unsignedValue.Equals(other.unsignedValue);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return isSigned
            ? signedValue.GetHashCode()
            : unsignedValue.GetHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Converts the numeric value of this instance to its equivalent string representation using the specified
    ///     culture-specific format information.
    /// </summary>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <returns>
    ///     The string representation of the value of this instance, which consists of a sequence of digits ranging from 0
    ///     to 9, without a sign or leading zeros.
    /// </returns>
    public string ToString(IFormatProvider provider)
    {
        return isSigned
            ? signedValue.ToString(provider)
            : unsignedValue.ToString(provider);
    }
}