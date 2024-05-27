// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Memory;

namespace FreePPlus.Imaging.Advanced;

//was previously: namespace SixLabors.ImageSharp.Advanced;

/// <summary>
///     Defines the contract for an action that operates on a row interval.
/// </summary>
public interface IRowIntervalOperation
{
    /// <summary>
    ///     Invokes the method passing the row interval.
    /// </summary>
    /// <param name="rows">The row interval.</param>
    void Invoke(in RowInterval rows);
}