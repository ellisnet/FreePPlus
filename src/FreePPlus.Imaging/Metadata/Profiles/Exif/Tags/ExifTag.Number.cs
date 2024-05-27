// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

/// <content />
public abstract partial class ExifTag
{
    /// <summary>
    ///     Gets the ImageWidth exif tag.
    /// </summary>
    public static ExifTag<Number> ImageWidth { get; } = new(ExifTagValue.ImageWidth);

    /// <summary>
    ///     Gets the ImageLength exif tag.
    /// </summary>
    public static ExifTag<Number> ImageLength { get; } = new(ExifTagValue.ImageLength);

    /// <summary>
    ///     Gets the TileWidth exif tag.
    /// </summary>
    public static ExifTag<Number> TileWidth { get; } = new(ExifTagValue.TileWidth);

    /// <summary>
    ///     Gets the TileLength exif tag.
    /// </summary>
    public static ExifTag<Number> TileLength { get; } = new(ExifTagValue.TileLength);

    /// <summary>
    ///     Gets the BadFaxLines exif tag.
    /// </summary>
    public static ExifTag<Number> BadFaxLines { get; } = new(ExifTagValue.BadFaxLines);

    /// <summary>
    ///     Gets the ConsecutiveBadFaxLines exif tag.
    /// </summary>
    public static ExifTag<Number> ConsecutiveBadFaxLines { get; } = new(ExifTagValue.ConsecutiveBadFaxLines);

    /// <summary>
    ///     Gets the PixelXDimension exif tag.
    /// </summary>
    public static ExifTag<Number> PixelXDimension { get; } = new(ExifTagValue.PixelXDimension);

    /// <summary>
    ///     Gets the PixelYDimension exif tag.
    /// </summary>
    public static ExifTag<Number> PixelYDimension { get; } = new(ExifTagValue.PixelYDimension);
}