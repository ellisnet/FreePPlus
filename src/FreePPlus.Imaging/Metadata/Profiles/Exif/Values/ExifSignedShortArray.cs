// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedShortArray : ExifArrayValue<short>
{
    public ExifSignedShortArray(ExifTagValue tag)
        : base(tag) { }

    private ExifSignedShortArray(ExifSignedShortArray value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.SignedShort;

    public override bool TrySetValue(object value)
    {
        if (base.TrySetValue(value)) return true;

        if (value is int[] intArray) return TrySetSignedArray(intArray);

        if (value is int intValue)
        {
            if (intValue >= short.MinValue && intValue <= short.MaxValue) Value = new[] { (short)intValue };

            return true;
        }

        return false;
    }

    public override IExifValue DeepClone()
    {
        return new ExifSignedShortArray(this);
    }

    private bool TrySetSignedArray(int[] intArray)
    {
        if (Array.FindIndex(intArray, x => x < short.MinValue || x > short.MaxValue) > -1) return false;

        var value = new short[intArray.Length];
        for (var i = 0; i < intArray.Length; i++)
        {
            var s = intArray[i];
            value[i] = (short)s;
        }

        Value = value;
        return true;
    }
}