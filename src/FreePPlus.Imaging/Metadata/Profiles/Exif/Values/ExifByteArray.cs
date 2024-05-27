// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifByteArray : ExifArrayValue<byte>
{
    public ExifByteArray(ExifTag<byte[]> tag, ExifDataType dataType)
        : base(tag)
    {
        DataType = dataType;
    }

    public ExifByteArray(ExifTagValue tag, ExifDataType dataType)
        : base(tag)
    {
        DataType = dataType;
    }

    private ExifByteArray(ExifByteArray value)
        : base(value)
    {
        DataType = value.DataType;
    }

    public override ExifDataType DataType { get; }

    public override bool TrySetValue(object value)
    {
        if (base.TrySetValue(value)) return true;

        if (value is int[] intArrayValue) return TrySetSignedIntArray(intArrayValue);

        if (value is int intValue)
        {
            if (intValue >= byte.MinValue && intValue <= byte.MaxValue) Value = new[] { (byte)intValue };

            return true;
        }

        return false;
    }

    public override IExifValue DeepClone()
    {
        return new ExifByteArray(this);
    }

    private bool TrySetSignedIntArray(int[] intArrayValue)
    {
        if (Array.FindIndex(intArrayValue, x => x < byte.MinValue || x > byte.MaxValue) > -1) return false;

        var value = new byte[intArrayValue.Length];
        for (var i = 0; i < intArrayValue.Length; i++)
        {
            var s = intArrayValue[i];
            value[i] = (byte)s;
        }

        Value = value;
        return true;
    }
}