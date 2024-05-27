// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Globalization;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifByte : ExifValue<byte>
{
    public ExifByte(ExifTag<byte> tag, ExifDataType dataType)
        : base(tag)
    {
        DataType = dataType;
    }

    public ExifByte(ExifTagValue tag, ExifDataType dataType)
        : base(tag)
    {
        DataType = dataType;
    }

    private ExifByte(ExifByte value)
        : base(value)
    {
        DataType = value.DataType;
    }

    public override ExifDataType DataType { get; }

    protected override string StringValue => Value.ToString("X2", CultureInfo.InvariantCulture);

    public override bool TrySetValue(object value)
    {
        if (base.TrySetValue(value)) return true;

        switch (value)
        {
            case int intValue:
                if (intValue >= byte.MinValue && intValue <= byte.MaxValue)
                {
                    Value = (byte)intValue;
                    return true;
                }

                return false;
            default:
                return base.TrySetValue(value);
        }
    }

    public override IExifValue DeepClone()
    {
        return new ExifByte(this);
    }
}