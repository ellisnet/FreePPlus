﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Standard Illuminant
/// </summary>
internal enum IccStandardIlluminant : uint
{
    /// <summary>
    ///     Unknown illuminant
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     D50 illuminant
    /// </summary>
    D50 = 1,

    /// <summary>
    ///     D65 illuminant
    /// </summary>
    D65 = 2,

    /// <summary>
    ///     D93 illuminant
    /// </summary>
    D93 = 3,

    /// <summary>
    ///     F2 illuminant
    /// </summary>
    F2 = 4,

    /// <summary>
    ///     D55 illuminant
    /// </summary>
    D55 = 5,

    /// <summary>
    ///     A illuminant
    /// </summary>
    A = 6,

    /// <summary>
    ///     D50 illuminant
    /// </summary>
    EquiPowerE = 7,

    /// <summary>
    ///     F8 illuminant
    /// </summary>
    F8 = 8
}