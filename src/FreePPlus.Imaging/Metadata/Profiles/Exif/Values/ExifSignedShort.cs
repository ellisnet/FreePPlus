// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Globalization;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedShort : ExifValue<short>
{
    public ExifSignedShort(ExifTagValue tag)
        : base(tag) { }

    private ExifSignedShort(ExifSignedShort value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.SignedShort;

    protected override string StringValue => Value.ToString(CultureInfo.InvariantCulture);

    public override bool TrySetValue(object value)
    {
        if (base.TrySetValue(value)) return true;

        switch (value)
        {
            case int intValue:
                if (intValue >= short.MinValue && intValue <= short.MaxValue)
                {
                    Value = (short)intValue;
                    return true;
                }

                return false;
            default:
                return false;
        }
    }

    public override IExifValue DeepClone()
    {
        return new ExifSignedShort(this);
    }
}