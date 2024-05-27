// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Globalization;
using System.Text;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Represents a number that can be expressed as a fraction.
/// </summary>
/// <remarks>
///     This is a very simplified implementation of a rational number designed for use with metadata only.
/// </remarks>
internal readonly struct LongRational : IEquatable<LongRational>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LongRational" /> struct.
    /// </summary>
    /// <param name="numerator">
    ///     The number above the line in a vulgar fraction showing how many of the parts
    ///     indicated by the denominator are taken.
    /// </param>
    /// <param name="denominator">
    ///     The number below the line in a vulgar fraction; a divisor.
    /// </param>
    public LongRational(long numerator, long denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
    }

    /// <summary>
    ///     Gets the numerator of a number.
    /// </summary>
    public long Numerator { get; }

    /// <summary>
    ///     Gets the denominator of a number.
    /// </summary>
    public long Denominator { get; }

    /// <summary>
    ///     Gets a value indicating whether this instance is indeterminate.
    /// </summary>
    public bool IsIndeterminate => Denominator == 0 && Numerator == 0;

    /// <summary>
    ///     Gets a value indicating whether this instance is an integer (n, 1)
    /// </summary>
    public bool IsInteger => Denominator == 1;

    /// <summary>
    ///     Gets a value indicating whether this instance is equal to negative infinity (-1, 0)
    /// </summary>
    public bool IsNegativeInfinity => Denominator == 0 && Numerator == -1;

    /// <summary>
    ///     Gets a value indicating whether this instance is equal to positive infinity (1, 0)
    /// </summary>
    public bool IsPositiveInfinity => Denominator == 0 && Numerator == 1;

    /// <summary>
    ///     Gets a value indicating whether this instance is equal to 0 (0, 1)
    /// </summary>
    public bool IsZero => Denominator == 1 && Numerator == 0;

    /// <inheritdoc />
    public bool Equals(LongRational other)
    {
        return Numerator == other.Numerator && Denominator == other.Denominator;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return ((Numerator * 397) ^ Denominator).GetHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Converts the numeric value of this instance to its equivalent string representation using
    ///     the specified culture-specific format information.
    /// </summary>
    /// <param name="provider">
    ///     An object that supplies culture-specific formatting information.
    /// </param>
    /// <returns>The <see cref="string" /></returns>
    public string ToString(IFormatProvider provider)
    {
        if (IsIndeterminate) return "[ Indeterminate ]";

        if (IsPositiveInfinity) return "[ PositiveInfinity ]";

        if (IsNegativeInfinity) return "[ NegativeInfinity ]";

        if (IsZero) return "0";

        if (IsInteger) return Numerator.ToString(provider);

        var sb = new StringBuilder();
        sb.Append(Numerator.ToString(provider));
        sb.Append('/');
        sb.Append(Denominator.ToString(provider));
        return sb.ToString();
    }

    /// <summary>
    ///     Create a new instance of the <see cref="LongRational" /> struct from a double value.
    /// </summary>
    /// <param name="value">The <see cref="double" /> to create the instance from.</param>
    /// <param name="bestPrecision">Whether to use the best possible precision when parsing the value.</param>
    public static LongRational FromDouble(double value, bool bestPrecision)
    {
        if (double.IsNaN(value)) return new LongRational(0, 0);

        if (double.IsPositiveInfinity(value)) return new LongRational(1, 0);

        if (double.IsNegativeInfinity(value)) return new LongRational(-1, 0);

        long numerator = 1;
        long denominator = 1;

        var val = Math.Abs(value);
        var df = numerator / (double)denominator;
        var epsilon = bestPrecision ? double.Epsilon : .000001;

        while (Math.Abs(df - val) > epsilon)
        {
            if (df < val)
            {
                numerator++;
            }
            else
            {
                denominator++;
                numerator = (int)(val * denominator);
            }

            df = numerator / (double)denominator;
        }

        if (value < 0.0) numerator *= -1;

        return new LongRational(numerator, denominator).Simplify();
    }

    /// <summary>
    ///     Finds the greatest common divisor of two <see cref="long" /> values.
    /// </summary>
    /// <param name="left">The first value</param>
    /// <param name="right">The second value</param>
    /// <returns>The <see cref="long" /></returns>
    private static long GreatestCommonDivisor(long left, long right)
    {
        return right == 0 ? left : GreatestCommonDivisor(right, left % right);
    }

    /// <summary>
    ///     Simplifies the <see cref="LongRational" />
    /// </summary>
    public LongRational Simplify()
    {
        if (IsIndeterminate ||
            IsNegativeInfinity ||
            IsPositiveInfinity ||
            IsInteger ||
            IsZero)
            return this;

        if (Numerator == 0) return new LongRational(0, 0);

        if (Numerator == Denominator) return new LongRational(1, 1);

        var gcd = GreatestCommonDivisor(Math.Abs(Numerator), Math.Abs(Denominator));

        if (gcd > 1) return new LongRational(Numerator / gcd, Denominator / gcd);

        return this;
    }
}