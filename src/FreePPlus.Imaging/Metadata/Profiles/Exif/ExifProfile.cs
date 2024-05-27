// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.IO;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

/// <summary>
///     Represents an EXIF profile providing access to the collection of values.
/// </summary>
public sealed class ExifProfile : IDeepCloneable<ExifProfile>
{
    /// <summary>
    ///     The byte array to read the EXIF profile from.
    /// </summary>
    private readonly byte[] data;

    /// <summary>
    ///     The thumbnail length in the byte stream
    /// </summary>
    private int thumbnailLength;

    /// <summary>
    ///     The thumbnail offset position in the byte stream
    /// </summary>
    private int thumbnailOffset;

    /// <summary>
    ///     The collection of EXIF values
    /// </summary>
    private List<IExifValue> values;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExifProfile" /> class.
    /// </summary>
    public ExifProfile()
        : this((byte[])null) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExifProfile" /> class.
    /// </summary>
    /// <param name="data">The byte array to read the EXIF profile from.</param>
    public ExifProfile(byte[] data)
    {
        Parts = ExifParts.All;
        this.data = data;
        InvalidTags = Array.Empty<ExifTag>();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExifProfile" /> class
    ///     by making a copy from another EXIF profile.
    /// </summary>
    /// <param name="other">The other EXIF profile, where the clone should be made from.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other" /> is null.</exception>
    /// >
    private ExifProfile(ExifProfile other)
    {
        Guard.NotNull(other, nameof(other));

        Parts = other.Parts;
        thumbnailLength = other.thumbnailLength;
        thumbnailOffset = other.thumbnailOffset;

        InvalidTags = other.InvalidTags.Count > 0
            ? new List<ExifTag>(other.InvalidTags)
            : Array.Empty<ExifTag>();

        if (other.values != null)
        {
            values = new List<IExifValue>(other.Values.Count);

            foreach (var value in other.Values) values.Add(value.DeepClone());
        }

        if (other.data != null)
        {
            data = new byte[other.data.Length];
            other.data.AsSpan().CopyTo(data);
        }
    }

    /// <summary>
    ///     Gets or sets which parts will be written when the profile is added to an image.
    /// </summary>
    public ExifParts Parts { get; set; }

    /// <summary>
    ///     Gets the tags that where found but contained an invalid value.
    /// </summary>
    public IReadOnlyList<ExifTag> InvalidTags { get; private set; }

    /// <summary>
    ///     Gets the values of this EXIF profile.
    /// </summary>
    public IReadOnlyList<IExifValue> Values
    {
        get
        {
            InitializeValues();
            return values;
        }
    }

    /// <inheritdoc />
    public ExifProfile DeepClone()
    {
        return new ExifProfile(this);
    }

    /// <summary>
    ///     Returns the thumbnail in the EXIF profile when available.
    /// </summary>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <returns>
    ///     The <see cref="Image{TPixel}" />.
    /// </returns>
    public Image<TPixel> CreateThumbnail<TPixel>()
        where TPixel : unmanaged, IPixel<TPixel>
    {
        InitializeValues();

        if (thumbnailOffset == 0 || thumbnailLength == 0) return null;

        if (data is null || data.Length < thumbnailOffset + thumbnailLength) return null;

        using (var memStream = new MemoryStream(data, thumbnailOffset, thumbnailLength))
        {
            return Image.Load<TPixel>(memStream);
        }
    }

    /// <summary>
    ///     Returns the value with the specified tag.
    /// </summary>
    /// <param name="tag">The tag of the exif value.</param>
    /// <returns>The value with the specified tag.</returns>
    /// <typeparam name="TValueType">The data type of the tag.</typeparam>
    public IExifValue<TValueType> GetValue<TValueType>(ExifTag<TValueType> tag)
    {
        var value = GetValueInternal(tag);
        return value is null ? null : (IExifValue<TValueType>)value;
    }

    /// <summary>
    ///     Removes the value with the specified tag.
    /// </summary>
    /// <param name="tag">The tag of the EXIF value.</param>
    /// <returns>
    ///     The <see cref="bool" />.
    /// </returns>
    public bool RemoveValue(ExifTag tag)
    {
        InitializeValues();

        for (var i = 0; i < values.Count; i++)
            if (values[i].Tag == tag)
            {
                values.RemoveAt(i);
                return true;
            }

        return false;
    }

    /// <summary>
    ///     Sets the value of the specified tag.
    /// </summary>
    /// <param name="tag">The tag of the exif value.</param>
    /// <param name="value">The value.</param>
    /// <typeparam name="TValueType">The data type of the tag.</typeparam>
    public void SetValue<TValueType>(ExifTag<TValueType> tag, TValueType value)
    {
        SetValueInternal(tag, value);
    }

    /// <summary>
    ///     Converts this instance to a byte array.
    /// </summary>
    /// <returns>The <see cref="T:byte[]" /></returns>
    public byte[] ToByteArray()
    {
        if (values is null) return data;

        if (values.Count == 0) return Array.Empty<byte>();

        var writer = new ExifWriter(values, Parts);
        return writer.GetData();
    }

    /// <summary>
    ///     Returns the value with the specified tag.
    /// </summary>
    /// <param name="tag">The tag of the exif value.</param>
    /// <returns>The value with the specified tag.</returns>
    internal IExifValue GetValueInternal(ExifTag tag)
    {
        foreach (var exifValue in Values)
            if (exifValue.Tag == tag)
                return exifValue;

        return null;
    }

    /// <summary>
    ///     Sets the value of the specified tag.
    /// </summary>
    /// <param name="tag">The tag of the exif value.</param>
    /// <param name="value">The value.</param>
    internal void SetValueInternal(ExifTag tag, object value)
    {
        foreach (var exifValue in Values)
            if (exifValue.Tag == tag)
            {
                exifValue.TrySetValue(value);
                return;
            }

        var newExifValue = ExifValues.Create(tag);
        if (newExifValue is null) throw new NotSupportedException();

        newExifValue.TrySetValue(value);
        values.Add(newExifValue);
    }

    /// <summary>
    ///     Synchronizes the profiles with the specified metadata.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    internal void Sync(ImageMetadata metadata)
    {
        SyncResolution(ExifTag.XResolution, metadata.HorizontalResolution);
        SyncResolution(ExifTag.YResolution, metadata.VerticalResolution);
    }

    private void SyncResolution(ExifTag<Rational> tag, double resolution)
    {
        var value = GetValue(tag);

        if (value is null) return;

        if (value.IsArray || value.DataType != ExifDataType.Rational) RemoveValue(value.Tag);

        var newResolution = new Rational(resolution, false);
        SetValue(tag, newResolution);
    }

    private void InitializeValues()
    {
        if (values != null) return;

        if (data is null)
        {
            values = new List<IExifValue>();
            return;
        }

        var reader = new ExifReader(data);

        values = reader.ReadValues();

        InvalidTags = reader.InvalidTags.Count > 0
            ? new List<ExifTag>(reader.InvalidTags)
            : Array.Empty<ExifTag>();

        thumbnailOffset = (int)reader.ThumbnailOffset;
        thumbnailLength = (int)reader.ThumbnailLength;
    }
}