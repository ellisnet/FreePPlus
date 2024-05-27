// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedRationalArray : ExifArrayValue<SignedRational>
{
    public ExifSignedRationalArray(ExifTag<SignedRational[]> tag)
        : base(tag) { }

    public ExifSignedRationalArray(ExifTagValue tag)
        : base(tag) { }

    private ExifSignedRationalArray(ExifSignedRationalArray value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.SignedRational;

    public override IExifValue DeepClone()
    {
        return new ExifSignedRationalArray(this);
    }
}