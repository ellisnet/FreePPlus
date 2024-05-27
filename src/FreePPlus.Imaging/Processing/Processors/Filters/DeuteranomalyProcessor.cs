﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing.Processors.Filters;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Filters;

/// <summary>
///     Converts the colors of the image recreating Deuteranomaly (Green-Weak) color blindness.
/// </summary>
public sealed class DeuteranomalyProcessor : FilterProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeuteranomalyProcessor" /> class.
    /// </summary>
    public DeuteranomalyProcessor()
        : base(KnownFilterMatrices.DeuteranomalyFilter) { }
}