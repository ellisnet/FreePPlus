﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing.Processors.Normalization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Normalization;

/// <summary>
///     Enumerates the different types of defined histogram equalization methods.
/// </summary>
public enum HistogramEqualizationMethod
{
    /// <summary>
    ///     A global histogram equalization.
    /// </summary>
    Global,

    /// <summary>
    ///     Adaptive histogram equalization using a tile interpolation approach.
    /// </summary>
    AdaptiveTileInterpolation,

    /// <summary>
    ///     Adaptive histogram equalization using sliding window. Slower then the tile interpolation mode, but can yield to
    ///     better results.
    /// </summary>
    AdaptiveSlidingWindow
}