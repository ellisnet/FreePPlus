// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

/// <content />
public abstract partial class ExifTag
{
    /// <summary>
    ///     Gets the FreeOffsets exif tag.
    /// </summary>
    public static ExifTag<uint[]> FreeOffsets { get; } = new(ExifTagValue.FreeOffsets);

    /// <summary>
    ///     Gets the FreeByteCounts exif tag.
    /// </summary>
    public static ExifTag<uint[]> FreeByteCounts { get; } = new(ExifTagValue.FreeByteCounts);

    /// <summary>
    ///     Gets the ColorResponseUnit exif tag.
    /// </summary>
    public static ExifTag<uint[]> ColorResponseUnit { get; } = new(ExifTagValue.ColorResponseUnit);

    /// <summary>
    ///     Gets the TileOffsets exif tag.
    /// </summary>
    public static ExifTag<uint[]> TileOffsets { get; } = new(ExifTagValue.TileOffsets);

    /// <summary>
    ///     Gets the SMinSampleValue exif tag.
    /// </summary>
    public static ExifTag<uint[]> SMinSampleValue { get; } = new(ExifTagValue.SMinSampleValue);

    /// <summary>
    ///     Gets the SMaxSampleValue exif tag.
    /// </summary>
    public static ExifTag<uint[]> SMaxSampleValue { get; } = new(ExifTagValue.SMaxSampleValue);

    /// <summary>
    ///     Gets the JPEGQTables exif tag.
    /// </summary>
    public static ExifTag<uint[]> JPEGQTables { get; } = new(ExifTagValue.JPEGQTables);

    /// <summary>
    ///     Gets the JPEGDCTables exif tag.
    /// </summary>
    public static ExifTag<uint[]> JPEGDCTables { get; } = new(ExifTagValue.JPEGDCTables);

    /// <summary>
    ///     Gets the JPEGACTables exif tag.
    /// </summary>
    public static ExifTag<uint[]> JPEGACTables { get; } = new(ExifTagValue.JPEGACTables);

    /// <summary>
    ///     Gets the StripRowCounts exif tag.
    /// </summary>
    public static ExifTag<uint[]> StripRowCounts { get; } = new(ExifTagValue.StripRowCounts);

    /// <summary>
    ///     Gets the IntergraphRegisters exif tag.
    /// </summary>
    public static ExifTag<uint[]> IntergraphRegisters { get; } = new(ExifTagValue.IntergraphRegisters);

    /// <summary>
    ///     Gets the TimeZoneOffset exif tag.
    /// </summary>
    public static ExifTag<uint[]> TimeZoneOffset { get; } = new(ExifTagValue.TimeZoneOffset);
}