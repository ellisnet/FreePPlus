// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal abstract class ExifValue : IExifValue, IEquatable<ExifTag>
{
    protected ExifValue(ExifTag tag)
    {
        Tag = tag;
    }

    protected ExifValue(ExifTagValue tag)
    {
        Tag = new UnkownExifTag(tag);
    }

    internal ExifValue(ExifValue other)
    {
        Guard.NotNull(other, nameof(other));

        DataType = other.DataType;
        IsArray = other.IsArray;
        Tag = other.Tag;

        if (!other.IsArray)
        {
            // All types are value types except for string which is immutable so safe to simply assign.
            TrySetValue(other.GetValue());
        }
        else
        {
            // All array types are value types so Clone() is sufficient here.
            var array = (Array)other.GetValue();
            TrySetValue(array.Clone());
        }
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public bool Equals(ExifTag other)
    {
        return Tag.Equals(other);
    }

    public virtual ExifDataType DataType { get; }

    public virtual bool IsArray { get; }

    public ExifTag Tag { get; }

    public abstract object GetValue();

    public abstract bool TrySetValue(object value);

    public abstract IExifValue DeepClone();

    public static bool operator ==(ExifValue left, ExifTag right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ExifValue left, ExifTag right)
    {
        return !Equals(left, right);
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;

        if (ReferenceEquals(this, obj)) return true;

        if (obj is ExifTag tag) return Equals(tag);

        if (obj is ExifValue value) return Tag.Equals(value.Tag) && Equals(GetValue(), value.GetValue());

        return false;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public override int GetHashCode()
    {
        return HashCode.Combine(Tag, GetValue());
    }
}