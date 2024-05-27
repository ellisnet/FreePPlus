// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Linq;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Color Lookup Table
/// </summary>
internal sealed class IccClut : IEquatable<IccClut>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccClut" /> class.
    /// </summary>
    /// <param name="values">The CLUT values</param>
    /// <param name="gridPointCount">The gridpoint count</param>
    /// <param name="type">The data type of this CLUT</param>
    public IccClut(float[][] values, byte[] gridPointCount, IccClutDataType type)
    {
        Guard.NotNull(values, nameof(values));
        Guard.NotNull(gridPointCount, nameof(gridPointCount));

        Values = values;
        DataType = type;
        InputChannelCount = gridPointCount.Length;
        OutputChannelCount = values[0].Length;
        GridPointCount = gridPointCount;
        CheckValues();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccClut" /> class.
    /// </summary>
    /// <param name="values">The CLUT values</param>
    /// <param name="gridPointCount">The gridpoint count</param>
    public IccClut(ushort[][] values, byte[] gridPointCount)
    {
        Guard.NotNull(values, nameof(values));
        Guard.NotNull(gridPointCount, nameof(gridPointCount));

        const float Max = ushort.MaxValue;

        Values = new float[values.Length][];
        for (var i = 0; i < values.Length; i++)
        {
            Values[i] = new float[values[i].Length];
            for (var j = 0; j < values[i].Length; j++) Values[i][j] = values[i][j] / Max;
        }

        DataType = IccClutDataType.UInt16;
        InputChannelCount = gridPointCount.Length;
        OutputChannelCount = values[0].Length;
        GridPointCount = gridPointCount;
        CheckValues();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccClut" /> class.
    /// </summary>
    /// <param name="values">The CLUT values</param>
    /// <param name="gridPointCount">The gridpoint count</param>
    public IccClut(byte[][] values, byte[] gridPointCount)
    {
        Guard.NotNull(values, nameof(values));
        Guard.NotNull(gridPointCount, nameof(gridPointCount));

        const float Max = byte.MaxValue;

        Values = new float[values.Length][];
        for (var i = 0; i < values.Length; i++)
        {
            Values[i] = new float[values[i].Length];
            for (var j = 0; j < values[i].Length; j++) Values[i][j] = values[i][j] / Max;
        }

        DataType = IccClutDataType.UInt8;
        InputChannelCount = gridPointCount.Length;
        OutputChannelCount = values[0].Length;
        GridPointCount = gridPointCount;
        CheckValues();
    }

    /// <summary>
    ///     Gets the values that make up this table
    /// </summary>
    public float[][] Values { get; }

    /// <summary>
    ///     Gets the CLUT data type (important when writing a profile)
    /// </summary>
    public IccClutDataType DataType { get; }

    /// <summary>
    ///     Gets the number of input channels
    /// </summary>
    public int InputChannelCount { get; }

    /// <summary>
    ///     Gets the number of output channels
    /// </summary>
    public int OutputChannelCount { get; }

    /// <summary>
    ///     Gets the number of grid points per input channel
    /// </summary>
    public byte[] GridPointCount { get; }

    /// <inheritdoc />
    public bool Equals(IccClut other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return EqualsValuesArray(other)
               && DataType == other.DataType
               && InputChannelCount == other.InputChannelCount
               && OutputChannelCount == other.OutputChannelCount
               && GridPointCount.AsSpan().SequenceEqual(other.GridPointCount);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccClut other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Values,
            DataType,
            InputChannelCount,
            OutputChannelCount,
            GridPointCount);
    }

    private bool EqualsValuesArray(IccClut other)
    {
        if (Values.Length != other.Values.Length) return false;

        for (var i = 0; i < Values.Length; i++)
            if (!Values[i].AsSpan().SequenceEqual(other.Values[i]))
                return false;

        return true;
    }

    private void CheckValues()
    {
        Guard.MustBeBetweenOrEqualTo(InputChannelCount, 1, 15, nameof(InputChannelCount));
        Guard.MustBeBetweenOrEqualTo(OutputChannelCount, 1, 15, nameof(OutputChannelCount));

        var isLengthDifferent = Values.Any(t => t.Length != OutputChannelCount);
        Guard.IsFalse(isLengthDifferent, nameof(Values), "The number of output values varies");

        var length = 0;
        for (var i = 0; i < InputChannelCount; i++) length += (int)Math.Pow(GridPointCount[i], InputChannelCount);

        length /= InputChannelCount;

        Guard.IsTrue(Values.Length == length, nameof(Values), "Length of values array does not match the grid points");
    }
}