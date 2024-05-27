// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing.Processors.Filters;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Filters;

/// <summary>
///     Converts the colors of the image recreating Protanomaly (Red-Weak) color blindness.
/// </summary>
public sealed class ProtanomalyProcessor : FilterProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProtanomalyProcessor" /> class.
    /// </summary>
    public ProtanomalyProcessor()
        : base(KnownFilterMatrices.ProtanomalyFilter) { }
}