﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Enumerates the various options which determine which side to taper
/// </summary>
public enum TaperSide
{
    /// <summary>
    ///     Taper the left side
    /// </summary>
    Left,

    /// <summary>
    ///     Taper the top side
    /// </summary>
    Top,

    /// <summary>
    ///     Taper the right side
    /// </summary>
    Right,

    /// <summary>
    ///     Taper the bottom side
    /// </summary>
    Bottom
}