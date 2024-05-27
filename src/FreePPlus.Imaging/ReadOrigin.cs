﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Specifies the position in a stream to use for reading.
/// </summary>
public enum ReadOrigin
{
    /// <summary>
    ///     Specifies the beginning of a stream.
    /// </summary>
    Begin,

    /// <summary>
    ///     Specifies the current position within a stream.
    /// </summary>
    Current
}