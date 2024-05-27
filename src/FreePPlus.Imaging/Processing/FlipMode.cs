﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Provides enumeration over how a image should be flipped.
/// </summary>
public enum FlipMode
{
    /// <summary>
    ///     Don't flip the image.
    /// </summary>
    None,

    /// <summary>
    ///     Flip the image horizontally.
    /// </summary>
    Horizontal,

    /// <summary>
    ///     Flip the image vertically.
    /// </summary>
    Vertical
}