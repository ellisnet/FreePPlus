// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Globalization;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedByte : ExifValue<sbyte>
{
    public ExifSignedByte(ExifTagValue tag)
        : base(tag) { }

    private ExifSignedByte(ExifSignedByte value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.SignedByte;

    protected override string StringValue => Value.ToString("X2", CultureInfo.InvariantCulture);

    public override bool TrySetValue(object value)
    {
        if (base.TrySetValue(value)) return true;

        switch (value)
        {
            case int intValue:
                if (intValue >= sbyte.MinValue && intValue <= sbyte.MaxValue)
                {
                    Value = (sbyte)intValue;
                    return true;
                }

                return false;
            default:
                return false;
        }
    }

    public override IExifValue DeepClone()
    {
        return new ExifSignedByte(this);
    }
}