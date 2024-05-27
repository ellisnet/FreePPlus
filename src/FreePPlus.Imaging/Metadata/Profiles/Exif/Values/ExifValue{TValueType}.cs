// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal abstract class ExifValue<TValueType> : ExifValue, IExifValue<TValueType>
{
    protected ExifValue(ExifTag<TValueType> tag)
        : base(tag) { }

    protected ExifValue(ExifTagValue tag)
        : base(tag) { }

    internal ExifValue(ExifValue value)
        : base(value) { }

    /// <summary>
    ///     Gets the value of the current instance as a string.
    /// </summary>
    protected abstract string StringValue { get; }

    public TValueType Value { get; set; }

    public override object GetValue()
    {
        return Value;
    }

    public override bool TrySetValue(object value)
    {
        if (value is null)
        {
            Value = default;
            return true;
        }

        // We use type comparison here over "is" to avoid compiler optimizations
        // that equate short with ushort, and sbyte with byte.
        if (value.GetType() == typeof(TValueType))
        {
            Value = (TValueType)value;
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        if (Value == null) return null;

        var description = ExifTagDescriptionAttribute.GetDescription(Tag, Value);
        return description ?? StringValue;
    }
}