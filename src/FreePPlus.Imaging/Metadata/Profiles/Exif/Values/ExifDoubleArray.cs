// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifDoubleArray : ExifArrayValue<double>
{
    public ExifDoubleArray(ExifTag<double[]> tag)
        : base(tag) { }

    public ExifDoubleArray(ExifTagValue tag)
        : base(tag) { }

    private ExifDoubleArray(ExifDoubleArray value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.DoubleFloat;

    public override IExifValue DeepClone()
    {
        return new ExifDoubleArray(this);
    }
}