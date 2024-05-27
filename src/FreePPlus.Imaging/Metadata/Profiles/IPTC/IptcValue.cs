// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Text;

namespace FreePPlus.Imaging.Metadata.Profiles.Iptc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Iptc;

/// <summary>
///     Represents a single value of the IPTC profile.
/// </summary>
public sealed class IptcValue : IDeepCloneable<IptcValue>
{
    private byte[] data = Array.Empty<byte>();
    private Encoding encoding;

    internal IptcValue(IptcValue other)
    {
        if (other.data != null)
        {
            data = new byte[other.data.Length];
            other.data.AsSpan().CopyTo(data);
        }

        if (other.Encoding != null) Encoding = (Encoding)other.Encoding.Clone();

        Tag = other.Tag;
        Strict = other.Strict;
    }

    internal IptcValue(IptcTag tag, byte[] value, bool strict)
    {
        Guard.NotNull(value, nameof(value));

        Strict = strict;
        Tag = tag;
        data = value;
        encoding = Encoding.UTF8;
    }

    internal IptcValue(IptcTag tag, Encoding encoding, string value, bool strict)
    {
        Strict = strict;
        Tag = tag;
        this.encoding = encoding;
        Value = value;
    }

    internal IptcValue(IptcTag tag, string value, bool strict)
    {
        Strict = strict;
        Tag = tag;
        encoding = Encoding.UTF8;
        Value = value;
    }

    /// <summary>
    ///     Gets or sets the encoding to use for the Value.
    /// </summary>
    public Encoding Encoding
    {
        get => encoding;
        set
        {
            if (value != null) encoding = value;
        }
    }

    /// <summary>
    ///     Gets the tag of the iptc value.
    /// </summary>
    public IptcTag Tag { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether to be enforce value length restrictions according
    ///     to the specification.
    /// </summary>
    public bool Strict { get; set; }

    /// <summary>
    ///     Gets or sets the value.
    /// </summary>
    public string Value
    {
        get => encoding.GetString(data);
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                data = Array.Empty<byte>();
            }
            else
            {
                var maxLength = Tag.MaxLength();
                byte[] valueBytes;
                if (Strict && value.Length > maxLength)
                {
                    var cappedValue = value.Substring(0, maxLength);
                    valueBytes = encoding.GetBytes(cappedValue);

                    // It is still possible that the bytes of the string exceed the limit.
                    if (valueBytes.Length > maxLength)
                        throw new ArgumentException(
                            $"The iptc value exceeds the limit of {maxLength} bytes for the tag {Tag}");
                }
                else
                {
                    valueBytes = encoding.GetBytes(value);
                }

                data = valueBytes;
            }
        }
    }

    /// <summary>
    ///     Gets the length of the value.
    /// </summary>
    public int Length => data.Length;

    /// <inheritdoc />
    public IptcValue DeepClone()
    {
        return new IptcValue(this);
    }

    /// <summary>
    ///     Determines whether the specified object is equal to the current <see cref="IptcValue" />.
    /// </summary>
    /// <param name="obj">The object to compare this <see cref="IptcValue" /> with.</param>
    /// <returns>True when the specified object is equal to the current <see cref="IptcValue" />.</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;

        return Equals(obj as IptcValue);
    }

    /// <summary>
    ///     Determines whether the specified iptc value is equal to the current <see cref="IptcValue" />.
    /// </summary>
    /// <param name="other">The iptc value to compare this <see cref="IptcValue" /> with.</param>
    /// <returns>True when the specified iptc value is equal to the current <see cref="IptcValue" />.</returns>
    public bool Equals(IptcValue other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        if (Tag != other.Tag) return false;

        if (data.Length != other.data.Length) return false;

        for (var i = 0; i < data.Length; i++)
            if (data[i] != other.data[i])
                return false;

        return true;
    }

    /// <summary>
    ///     Serves as a hash of this type.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(data, Tag);
    }

    /// <summary>
    ///     Converts this instance to a byte array.
    /// </summary>
    /// <returns>A <see cref="byte" /> array.</returns>
    public byte[] ToByteArray()
    {
        var result = new byte[data.Length];
        data.CopyTo(result, 0);
        return result;
    }

    /// <summary>
    ///     Returns a string that represents the current value.
    /// </summary>
    /// <returns>A string that represents the current value.</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    ///     Returns a string that represents the current value with the specified encoding.
    /// </summary>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>A string that represents the current value with the specified encoding.</returns>
    public string ToString(Encoding encoding)
    {
        Guard.NotNull(encoding, nameof(encoding));

        return encoding.GetString(data);
    }
}