// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifFloatArray : ExifArrayValue<float>
{
    public ExifFloatArray(ExifTagValue tag)
        : base(tag) { }

    private ExifFloatArray(ExifFloatArray value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.SingleFloat;

    public override IExifValue DeepClone()
    {
        return new ExifFloatArray(this);
    }
}