﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Formats.Bmp;

//was previously: namespace SixLabors.ImageSharp.Formats.Bmp;

/// <summary>
///     Image decoder options for decoding Windows bitmap streams.
/// </summary>
internal interface IBmpDecoderOptions
{
    /// <summary>
    ///     Gets the value indicating how to deal with skipped pixels, which can occur during decoding run length encoded
    ///     bitmaps.
    /// </summary>
    RleSkippedPixelHandling RleSkippedPixelHandling { get; }
}