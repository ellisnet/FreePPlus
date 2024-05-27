// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifNumberArray : ExifArrayValue<Number>
{
    public ExifNumberArray(ExifTag<Number[]> tag)
        : base(tag) { }

    private ExifNumberArray(ExifNumberArray value)
        : base(value) { }

    public override ExifDataType DataType
    {
        get
        {
            if (Value is null) return ExifDataType.Short;

            for (var i = 0; i < Value.Length; i++)
                if (Value[i] > ushort.MaxValue)
                    return ExifDataType.Long;

            return ExifDataType.Short;
        }
    }

    public override IExifValue DeepClone()
    {
        return new ExifNumberArray(this);
    }
}