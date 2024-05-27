// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal abstract class ExifArrayValue<TValueType> : ExifValue, IExifValue<TValueType[]>
{
    protected ExifArrayValue(ExifTag<TValueType[]> tag)
        : base(tag) { }

    protected ExifArrayValue(ExifTagValue tag)
        : base(tag) { }

    internal ExifArrayValue(ExifArrayValue<TValueType> value)
        : base(value) { }

    public override bool IsArray => true;

    public TValueType[] Value { get; set; }

    public override object GetValue()
    {
        return Value;
    }

    public override bool TrySetValue(object value)
    {
        if (value is null)
        {
            Value = null;
            return true;
        }

        var type = value.GetType();
        if (value.GetType() == typeof(TValueType[]))
        {
            Value = (TValueType[])value;
            return true;
        }

        if (type == typeof(TValueType))
        {
            Value = new[] { (TValueType)value };
            return true;
        }

        return false;
    }
}