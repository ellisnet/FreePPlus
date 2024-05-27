﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing.Processors.Filters;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Filters;

/// <summary>
///     Converts the colors of the image recreating Deuteranopia (Green-Blind) color blindness.
/// </summary>
public sealed class DeuteranopiaProcessor : FilterProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeuteranopiaProcessor" /> class.
    /// </summary>
    public DeuteranopiaProcessor()
        : base(KnownFilterMatrices.DeuteranopiaFilter) { }
}