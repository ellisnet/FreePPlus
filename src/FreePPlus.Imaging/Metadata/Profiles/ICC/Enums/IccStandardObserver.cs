﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Standard Observer
/// </summary>
internal enum IccStandardObserver : uint
{
    /// <summary>
    ///     Unknown observer
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     CIE 1931 observer
    /// </summary>
    Cie1931Observer = 1,

    /// <summary>
    ///     CIE 1964 observer
    /// </summary>
    Cie1964Observer = 2
}