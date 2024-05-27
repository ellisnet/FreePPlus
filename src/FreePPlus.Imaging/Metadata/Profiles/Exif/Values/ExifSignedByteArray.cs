// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedByteArray : ExifArrayValue<sbyte>
{
    public ExifSignedByteArray(ExifTagValue tag)
        : base(tag) { }

    private ExifSignedByteArray(ExifSignedByteArray value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.SignedByte;

    public override IExifValue DeepClone()
    {
        return new ExifSignedByteArray(this);
    }
}