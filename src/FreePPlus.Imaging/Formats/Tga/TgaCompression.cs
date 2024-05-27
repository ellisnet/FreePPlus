// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Formats.Tga;

//was previously: namespace SixLabors.ImageSharp.Formats.Tga;

/// <summary>
///     Indicates if compression is used.
/// </summary>
public enum TgaCompression
{
    /// <summary>
    ///     No compression is used.
    /// </summary>
    None,

    /// <summary>
    ///     Run length encoding is used.
    /// </summary>
    RunLength
}