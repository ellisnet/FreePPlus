// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Globalization;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifString : ExifValue<string>
{
    public ExifString(ExifTag<string> tag)
        : base(tag) { }

    public ExifString(ExifTagValue tag)
        : base(tag) { }

    private ExifString(ExifString value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.Ascii;

    protected override string StringValue => Value;

    public override bool TrySetValue(object value)
    {
        if (base.TrySetValue(value)) return true;

        switch (value)
        {
            case int intValue:
                Value = intValue.ToString(CultureInfo.InvariantCulture);
                return true;
            default:
                return false;
        }
    }

    public override IExifValue DeepClone()
    {
        return new ExifString(this);
    }
}