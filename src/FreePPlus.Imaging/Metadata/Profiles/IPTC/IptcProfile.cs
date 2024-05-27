// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FreePPlus.Imaging.Metadata.Profiles.Iptc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Iptc;

/// <summary>
///     Represents an IPTC profile providing access to the collection of values.
/// </summary>
public sealed class IptcProfile : IDeepCloneable<IptcProfile>
{
    private Collection<IptcValue> values;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IptcProfile" /> class.
    /// </summary>
    public IptcProfile()
        : this((byte[])null) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IptcProfile" /> class.
    /// </summary>
    /// <param name="data">The byte array to read the iptc profile from.</param>
    public IptcProfile(byte[] data)
    {
        Data = data;
        Initialize();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IptcProfile" /> class
    ///     by making a copy from another IPTC profile.
    /// </summary>
    /// <param name="other">The other IPTC profile, from which the clone should be made from.</param>
    private IptcProfile(IptcProfile other)
    {
        Guard.NotNull(other, nameof(other));

        if (other.values != null)
        {
            values = new Collection<IptcValue>();

            foreach (var value in other.Values) values.Add(value.DeepClone());
        }

        if (other.Data != null)
        {
            Data = new byte[other.Data.Length];
            other.Data.AsSpan().CopyTo(Data);
        }
    }

    /// <summary>
    ///     Gets the byte data of the IPTC profile.
    /// </summary>
    public byte[] Data { get; private set; }

    /// <summary>
    ///     Gets the values of this iptc profile.
    /// </summary>
    public IEnumerable<IptcValue> Values
    {
        get
        {
            Initialize();
            return values;
        }
    }

    /// <inheritdoc />
    public IptcProfile DeepClone()
    {
        return new IptcProfile(this);
    }

    /// <summary>
    ///     Returns all values with the specified tag.
    /// </summary>
    /// <param name="tag">The tag of the iptc value.</param>
    /// <returns>The values found with the specified tag.</returns>
    public List<IptcValue> GetValues(IptcTag tag)
    {
        var iptcValues = new List<IptcValue>();
        foreach (var iptcValue in Values)
            if (iptcValue.Tag == tag)
                iptcValues.Add(iptcValue);

        return iptcValues;
    }

    /// <summary>
    ///     Removes all values with the specified tag.
    /// </summary>
    /// <param name="tag">The tag of the iptc value to remove.</param>
    /// <returns>True when the value was found and removed.</returns>
    public bool RemoveValue(IptcTag tag)
    {
        Initialize();

        var removed = false;
        for (var i = values.Count - 1; i >= 0; i--)
            if (values[i].Tag == tag)
            {
                values.RemoveAt(i);
                removed = true;
            }

        return removed;
    }

    /// <summary>
    ///     Removes values with the specified tag and value.
    /// </summary>
    /// <param name="tag">The tag of the iptc value to remove.</param>
    /// <param name="value">The value of the iptc item to remove.</param>
    /// <returns>True when the value was found and removed.</returns>
    public bool RemoveValue(IptcTag tag, string value)
    {
        Initialize();

        var removed = false;
        for (var i = values.Count - 1; i >= 0; i--)
            if (values[i].Tag == tag && values[i].Value.Equals(value))
            {
                values.RemoveAt(i);
                removed = true;
            }

        return removed;
    }

    /// <summary>
    ///     Changes the encoding for all the values.
    /// </summary>
    /// <param name="encoding">The encoding to use when storing the bytes.</param>
    public void SetEncoding(Encoding encoding)
    {
        Guard.NotNull(encoding, nameof(encoding));

        foreach (var value in Values) value.Encoding = encoding;
    }

    /// <summary>
    ///     Sets the value for the specified tag.
    /// </summary>
    /// <param name="tag">The tag of the iptc value.</param>
    /// <param name="encoding">The encoding to use when storing the bytes.</param>
    /// <param name="value">The value.</param>
    /// <param name="strict">
    ///     Indicates if length restrictions from the specification should be followed strictly.
    ///     Defaults to true.
    /// </param>
    public void SetValue(IptcTag tag, Encoding encoding, string value, bool strict = true)
    {
        Guard.NotNull(encoding, nameof(encoding));
        Guard.NotNull(value, nameof(value));

        if (!tag.IsRepeatable())
            foreach (var iptcValue in Values)
                if (iptcValue.Tag == tag)
                {
                    iptcValue.Strict = strict;
                    iptcValue.Encoding = encoding;
                    iptcValue.Value = value;
                    return;
                }

        values.Add(new IptcValue(tag, encoding, value, strict));
    }

    /// <summary>
    ///     Makes sure the datetime is formatted according to the iptc specification.
    ///     <example>
    ///         A date will be formatted as CCYYMMDD, e.g. "19890317" for 17 March 1989.
    ///         A time value will be formatted as HHMMSS±HHMM, e.g. "090000+0200" for 9 o'clock Berlin time,
    ///         two hours ahead of UTC.
    ///     </example>
    /// </summary>
    /// <param name="tag">The tag of the iptc value.</param>
    /// <param name="dateTimeOffset">The datetime.</param>
    public void SetDateTimeValue(IptcTag tag, DateTimeOffset dateTimeOffset)
    {
        if (!tag.IsDate() && !tag.IsTime()) throw new ArgumentException("iptc tag is not a time or date type");

        var formattedDate = tag.IsDate()
            ? dateTimeOffset.ToString("yyyyMMdd", CultureInfo.InvariantCulture)
            : dateTimeOffset.ToString("HHmmsszzzz", CultureInfo.InvariantCulture)
                .Replace(":", string.Empty);

        SetValue(tag, Encoding.UTF8, formattedDate);
    }

    /// <summary>
    ///     Sets the value of the specified tag.
    /// </summary>
    /// <param name="tag">The tag of the iptc value.</param>
    /// <param name="value">The value.</param>
    /// <param name="strict">
    ///     Indicates if length restrictions from the specification should be followed strictly.
    ///     Defaults to true.
    /// </param>
    public void SetValue(IptcTag tag, string value, bool strict = true)
    {
        SetValue(tag, Encoding.UTF8, value, strict);
    }

    /// <summary>
    ///     Updates the data of the profile.
    /// </summary>
    public void UpdateData()
    {
        var length = 0;
        foreach (var value in Values) length += value.Length + 5;

        Data = new byte[length];

        var i = 0;
        foreach (var value in Values)
        {
            Data[i++] = 28;
            Data[i++] = 2;
            Data[i++] = (byte)value.Tag;
            Data[i++] = (byte)(value.Length >> 8);
            Data[i++] = (byte)value.Length;
            if (value.Length > 0)
            {
                Buffer.BlockCopy(value.ToByteArray(), 0, Data, i, value.Length);
                i += value.Length;
            }
        }
    }

    private void Initialize()
    {
        if (values != null) return;

        values = new Collection<IptcValue>();

        if (Data == null || Data[0] != 0x1c) return;

        var i = 0;
        while (i + 4 < Data.Length)
        {
            if (Data[i++] != 28) continue;

            i++;

            var tag = (IptcTag)Data[i++];

            int count = BinaryPrimitives.ReadInt16BigEndian(Data.AsSpan(i, 2));
            i += 2;

            var iptcData = new byte[count];
            if (count > 0 && i + count <= Data.Length)
            {
                Buffer.BlockCopy(Data, i, iptcData, 0, count);
                values.Add(new IptcValue(tag, iptcData, false));
            }

            i += count;
        }
    }
}