﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Color lookup table data type
/// </summary>
internal enum IccClutDataType
{
    /// <summary>
    ///     32bit floating point
    /// </summary>
    Float,

    /// <summary>
    ///     8bit unsigned integer (byte)
    /// </summary>
    UInt8,

    /// <summary>
    ///     16bit unsigned integer (ushort)
    /// </summary>
    UInt16
}