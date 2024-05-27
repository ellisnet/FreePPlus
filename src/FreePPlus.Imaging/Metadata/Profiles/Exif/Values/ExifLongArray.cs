// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifLongArray : ExifArrayValue<uint>
{
    public ExifLongArray(ExifTag<uint[]> tag)
        : base(tag) { }

    public ExifLongArray(ExifTagValue tag)
        : base(tag) { }

    private ExifLongArray(ExifLongArray value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.Long;

    public override IExifValue DeepClone()
    {
        return new ExifLongArray(this);
    }
}