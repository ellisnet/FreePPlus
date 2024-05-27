﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Formats.Bmp;

//was previously: namespace SixLabors.ImageSharp.Formats.Bmp;

/// <summary>
///     Enumerates the available bits per pixel the bitmap encoder supports.
/// </summary>
public enum BmpBitsPerPixel : short
{
    /// <summary>
    ///     8 bits per pixel. Each pixel consists of 1 byte.
    /// </summary>
    Pixel8 = 8,

    /// <summary>
    ///     16 bits per pixel. Each pixel consists of 2 bytes.
    /// </summary>
    Pixel16 = 16,

    /// <summary>
    ///     24 bits per pixel. Each pixel consists of 3 bytes.
    /// </summary>
    Pixel24 = 24,

    /// <summary>
    ///     32 bits per pixel. Each pixel consists of 4 bytes.
    /// </summary>
    Pixel32 = 32
}