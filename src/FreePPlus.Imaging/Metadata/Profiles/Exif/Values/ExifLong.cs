// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Globalization;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifLong : ExifValue<uint>
{
    public ExifLong(ExifTag<uint> tag)
        : base(tag) { }

    public ExifLong(ExifTagValue tag)
        : base(tag) { }

    private ExifLong(ExifLong value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.Long;

    protected override string StringValue => Value.ToString(CultureInfo.InvariantCulture);

    public override bool TrySetValue(object value)
    {
        if (base.TrySetValue(value)) return true;

        switch (value)
        {
            case int intValue:
                if (intValue >= uint.MinValue)
                {
                    Value = (uint)intValue;
                    return true;
                }

                return false;
            default:
                return false;
        }
    }

    public override IExifValue DeepClone()
    {
        return new ExifLong(this);
    }
}