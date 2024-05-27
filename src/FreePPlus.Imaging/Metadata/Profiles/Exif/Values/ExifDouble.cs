// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Globalization;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifDouble : ExifValue<double>
{
    public ExifDouble(ExifTag<double> tag)
        : base(tag) { }

    public ExifDouble(ExifTagValue tag)
        : base(tag) { }

    private ExifDouble(ExifDouble value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.DoubleFloat;

    protected override string StringValue => Value.ToString(CultureInfo.InvariantCulture);

    public override bool TrySetValue(object value)
    {
        if (base.TrySetValue(value)) return true;

        switch (value)
        {
            case int intValue:
                Value = intValue;
                return true;
            default:
                return false;
        }
    }

    public override IExifValue DeepClone()
    {
        return new ExifDouble(this);
    }
}