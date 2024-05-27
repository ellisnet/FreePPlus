// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

/// <content />
public abstract partial class ExifTag
{
    /// <summary>
    ///     Gets the FaxProfile exif tag.
    /// </summary>
    public static ExifTag<byte> FaxProfile { get; } = new(ExifTagValue.FaxProfile);

    /// <summary>
    ///     Gets the ModeNumber exif tag.
    /// </summary>
    public static ExifTag<byte> ModeNumber { get; } = new(ExifTagValue.ModeNumber);

    /// <summary>
    ///     Gets the GPSAltitudeRef exif tag.
    /// </summary>
    public static ExifTag<byte> GPSAltitudeRef { get; } = new(ExifTagValue.GPSAltitudeRef);
}