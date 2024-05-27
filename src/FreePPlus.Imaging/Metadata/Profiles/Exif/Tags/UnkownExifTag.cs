// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class UnkownExifTag : ExifTag
{
    internal UnkownExifTag(ExifTagValue value)
        : base((ushort)value) { }
}