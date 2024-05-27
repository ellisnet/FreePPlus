// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Globalization;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifFloat : ExifValue<float>
{
    public ExifFloat(ExifTagValue tag)
        : base(tag) { }

    private ExifFloat(ExifFloat value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.SingleFloat;

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
        return new ExifFloat(this);
    }
}