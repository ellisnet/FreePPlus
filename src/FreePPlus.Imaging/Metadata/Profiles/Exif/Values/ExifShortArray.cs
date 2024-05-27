// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifShortArray : ExifArrayValue<ushort>
{
    public ExifShortArray(ExifTag<ushort[]> tag)
        : base(tag) { }

    public ExifShortArray(ExifTagValue tag)
        : base(tag) { }

    private ExifShortArray(ExifShortArray value)
        : base(value) { }

    public override ExifDataType DataType => ExifDataType.Short;

    public override bool TrySetValue(object value)
    {
        if (base.TrySetValue(value)) return true;

        if (value is int[] signedIntArray) return TrySetSignedIntArray(signedIntArray);

        if (value is short[] signedShortArray) return TrySetSignedShortArray(signedShortArray);

        if (value is int signedInt)
        {
            if (signedInt >= ushort.MinValue && signedInt <= ushort.MaxValue) Value = new[] { (ushort)signedInt };

            return true;
        }

        if (value is short signedShort)
        {
            if (signedShort >= ushort.MinValue) Value = new[] { (ushort)signedShort };

            return true;
        }

        return false;
    }

    public override IExifValue DeepClone()
    {
        return new ExifShortArray(this);
    }

    private bool TrySetSignedIntArray(int[] signed)
    {
        if (Array.FindIndex(signed, x => x < ushort.MinValue || x > ushort.MaxValue) > -1) return false;

        var unsigned = new ushort[signed.Length];
        for (var i = 0; i < signed.Length; i++)
        {
            var s = signed[i];
            unsigned[i] = (ushort)s;
        }

        Value = unsigned;
        return true;
    }

    private bool TrySetSignedShortArray(short[] signed)
    {
        if (Array.FindIndex(signed, x => x < ushort.MinValue) > -1) return false;

        var unsigned = new ushort[signed.Length];
        for (var i = 0; i < signed.Length; i++)
        {
            var s = signed[i];
            unsigned[i] = (ushort)s;
        }

        Value = unsigned;
        return true;
    }
}