// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Globalization;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifNumber : ExifValue<Number>
{
    public ExifNumber(ExifTag<Number> tag)
        : base(tag) { }

    private ExifNumber(ExifNumber value)
        : base(value) { }

    public override ExifDataType DataType
    {
        get
        {
            if (Value > ushort.MaxValue) return ExifDataType.Long;

            return ExifDataType.Short;
        }
    }

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
            case uint uintValue:
                Value = uintValue;
                return true;
            case short shortValue:
                if (shortValue >= uint.MinValue)
                {
                    Value = (uint)shortValue;
                    return true;
                }

                return false;
            case ushort ushortValue:
                Value = ushortValue;
                return true;
            default:
                return false;
        }
    }

    public override IExifValue DeepClone()
    {
        return new ExifNumber(this);
    }
}