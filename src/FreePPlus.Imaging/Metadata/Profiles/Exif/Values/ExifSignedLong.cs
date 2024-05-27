// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Globalization;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedLong : ExifValue<int>
{
    public ExifSignedLong(ExifTagValue tag)
        : base(tag) { }

    private ExifSignedLong(ExifSignedLong value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.SignedLong;

    protected override string StringValue => Value.ToString(CultureInfo.InvariantCulture);

    public override IExifValue DeepClone()
    {
        return new ExifSignedLong(this);
    }
}