// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

/// <content />
public abstract partial class ExifTag
{
    /// <summary>
    ///     Gets the StripOffsets exif tag.
    /// </summary>
    public static ExifTag<Number[]> StripOffsets { get; } = new(ExifTagValue.StripOffsets);

    /// <summary>
    ///     Gets the TileByteCounts exif tag.
    /// </summary>
    public static ExifTag<Number[]> TileByteCounts { get; } = new(ExifTagValue.TileByteCounts);

    /// <summary>
    ///     Gets the ImageLayer exif tag.
    /// </summary>
    public static ExifTag<Number[]> ImageLayer { get; } = new(ExifTagValue.ImageLayer);
}