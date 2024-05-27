// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Lookup Table
/// </summary>
internal readonly struct IccLut : IEquatable<IccLut>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccLut" /> struct.
    /// </summary>
    /// <param name="values">The LUT values</param>
    public IccLut(float[] values)
    {
        Values = values ?? throw new ArgumentNullException(nameof(values));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccLut" /> struct.
    /// </summary>
    /// <param name="values">The LUT values</param>
    public IccLut(ushort[] values)
    {
        Guard.NotNull(values, nameof(values));

        const float max = ushort.MaxValue;

        Values = new float[values.Length];
        for (var i = 0; i < values.Length; i++) Values[i] = values[i] / max;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccLut" /> struct.
    /// </summary>
    /// <param name="values">The LUT values</param>
    public IccLut(byte[] values)
    {
        Guard.NotNull(values, nameof(values));

        const float max = byte.MaxValue;

        Values = new float[values.Length];
        for (var i = 0; i < values.Length; i++) Values[i] = values[i] / max;
    }

    /// <summary>
    ///     Gets the values that make up this table
    /// </summary>
    public float[] Values { get; }

    /// <inheritdoc />
    public bool Equals(IccLut other)
    {
        if (ReferenceEquals(Values, other.Values)) return true;

        return Values.AsSpan().SequenceEqual(other.Values);
    }
}