// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

/// <summary>
///     Specifies which parts will be written when the profile is added to an image.
/// </summary>
[Flags]
public enum ExifParts
{
    /// <summary>
    ///     None
    /// </summary>
    None = 0,

    /// <summary>
    ///     IfdTags
    /// </summary>
    IfdTags = 1,

    /// <summary>
    ///     ExifTags
    /// </summary>
    ExifTags = 4,

    /// <summary>
    ///     GPSTags
    /// </summary>
    GpsTags = 8,

    /// <summary>
    ///     All
    /// </summary>
    All = IfdTags | ExifTags | GpsTags
}