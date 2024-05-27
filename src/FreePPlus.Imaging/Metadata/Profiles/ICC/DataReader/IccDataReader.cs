// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Provides methods to read ICC data types
/// </summary>
internal sealed partial class IccDataReader
{
    /// <summary>
    ///     The data that is read
    /// </summary>
    private readonly byte[] data;

    /// <summary>
    ///     The current reading position
    /// </summary>
    private int currentIndex;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccDataReader" /> class.
    /// </summary>
    /// <param name="data">The data to read</param>
    public IccDataReader(byte[] data)
    {
        this.data = data ?? throw new ArgumentNullException(nameof(data));
    }

    /// <summary>
    ///     Gets the length in bytes of the raw data
    /// </summary>
    public int DataLength => data.Length;

    /// <summary>
    ///     Sets the reading position to the given value
    /// </summary>
    /// <param name="index">The new index position</param>
    public void SetIndex(int index)
    {
        currentIndex = index.Clamp(0, data.Length);
    }

    /// <summary>
    ///     Returns the current <see cref="currentIndex" /> without increment and adds the given increment
    /// </summary>
    /// <param name="increment">The value to increment <see cref="currentIndex" /></param>
    /// <returns>The current <see cref="currentIndex" /> without the increment</returns>
    private int AddIndex(int increment)
    {
        var tmp = currentIndex;
        currentIndex += increment;
        return tmp;
    }

    /// <summary>
    ///     Calculates the 4 byte padding and adds it to the <see cref="currentIndex" /> variable
    /// </summary>
    private void AddPadding()
    {
        currentIndex += CalcPadding();
    }

    /// <summary>
    ///     Calculates the 4 byte padding
    /// </summary>
    /// <returns>the number of bytes to pad</returns>
    private int CalcPadding()
    {
        var p = 4 - currentIndex % 4;
        return p >= 4 ? 0 : p;
    }

    /// <summary>
    ///     Gets the bit value at a specified position
    /// </summary>
    /// <param name="value">The value from where the bit will be extracted</param>
    /// <param name="position">Position of the bit. Zero based index from left to right.</param>
    /// <returns>The bit value at specified position</returns>
    private bool GetBit(byte value, int position)
    {
        return ((value >> (7 - position)) & 1) == 1;
    }
}