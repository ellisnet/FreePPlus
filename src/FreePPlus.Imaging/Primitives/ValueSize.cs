﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Represents a value in relation to a value on the image.
/// </summary>
internal readonly struct ValueSize : IEquatable<ValueSize>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ValueSize" /> struct.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="type">The type.</param>
    public ValueSize(float value, ValueSizeType type)
    {
        if (type != ValueSizeType.Absolute) Guard.MustBeBetweenOrEqualTo(value, 0, 1, nameof(value));

        Value = value;
        Type = type;
    }

    /// <summary>
    ///     Enumerates the different value types.
    /// </summary>
    public enum ValueSizeType
    {
        /// <summary>
        ///     The value is the final return value.
        /// </summary>
        Absolute,

        /// <summary>
        ///     The value is a percentage of the image width.
        /// </summary>
        PercentageOfWidth,

        /// <summary>
        ///     The value is a percentage of the images height.
        /// </summary>
        PercentageOfHeight
    }

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public float Value { get; }

    /// <summary>
    ///     Gets the type.
    /// </summary>
    public ValueSizeType Type { get; }

    /// <summary>
    ///     Implicitly converts a float into an absolute value.
    /// </summary>
    /// <param name="f">the value to use as the absolute figure.</param>
    public static implicit operator ValueSize(float f)
    {
        return Absolute(f);
    }

    /// <summary>
    ///     Create a new ValueSize with as a PercentageOfWidth type with value set to percentage.
    /// </summary>
    /// <param name="percentage">The percentage.</param>
    /// <returns>a Values size with type PercentageOfWidth</returns>
    public static ValueSize PercentageOfWidth(float percentage)
    {
        return new ValueSize(percentage, ValueSizeType.PercentageOfWidth);
    }

    /// <summary>
    ///     Create a new ValueSize with as a PercentageOfHeight type with value set to percentage.
    /// </summary>
    /// <param name="percentage">The percentage.</param>
    /// <returns>a Values size with type PercentageOfHeight</returns>
    public static ValueSize PercentageOfHeight(float percentage)
    {
        return new ValueSize(percentage, ValueSizeType.PercentageOfHeight);
    }

    /// <summary>
    ///     Create a new ValueSize with as a Absolute type with value set to value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>a Values size with type Absolute.</returns>
    public static ValueSize Absolute(float value)
    {
        return new ValueSize(value, ValueSizeType.Absolute);
    }

    /// <summary>
    ///     Calculates the specified size.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>The calculated value.</returns>
    public float Calculate(Size size)
    {
        switch (Type)
        {
            case ValueSizeType.PercentageOfWidth:
                return Value * size.Width;
            case ValueSizeType.PercentageOfHeight:
                return Value * size.Height;
            case ValueSizeType.Absolute:
            default:
                return Value;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Value} - {Type}";
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is ValueSize size && Equals(size);
    }

    /// <inheritdoc />
    public bool Equals(ValueSize other)
    {
        return Type == other.Type && Value.Equals(other.Value);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Type);
    }
}