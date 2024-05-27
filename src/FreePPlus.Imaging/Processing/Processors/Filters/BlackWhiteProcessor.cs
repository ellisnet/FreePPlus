﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing.Processors.Filters;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Filters;

/// <summary>
///     Applies a black and white filter matrix to the image.
/// </summary>
public sealed class BlackWhiteProcessor : FilterProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BlackWhiteProcessor" /> class.
    /// </summary>
    public BlackWhiteProcessor()
        : base(KnownFilterMatrices.BlackWhiteFilter) { }
}