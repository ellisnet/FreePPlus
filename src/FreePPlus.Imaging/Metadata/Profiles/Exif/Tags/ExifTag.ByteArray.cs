// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

/// <content />
public abstract partial class ExifTag
{
    /// <summary>
    ///     Gets the ClipPath exif tag.
    /// </summary>
    public static ExifTag<byte[]> ClipPath => new(ExifTagValue.ClipPath);

    /// <summary>
    ///     Gets the VersionYear exif tag.
    /// </summary>
    public static ExifTag<byte[]> VersionYear => new(ExifTagValue.VersionYear);

    /// <summary>
    ///     Gets the XMP exif tag.
    /// </summary>
    public static ExifTag<byte[]> XMP => new(ExifTagValue.XMP);

    /// <summary>
    ///     Gets the CFAPattern2 exif tag.
    /// </summary>
    public static ExifTag<byte[]> CFAPattern2 => new(ExifTagValue.CFAPattern2);

    /// <summary>
    ///     Gets the TIFFEPStandardID exif tag.
    /// </summary>
    public static ExifTag<byte[]> TIFFEPStandardID => new(ExifTagValue.TIFFEPStandardID);

    /// <summary>
    ///     Gets the XPTitle exif tag.
    /// </summary>
    public static ExifTag<byte[]> XPTitle => new(ExifTagValue.XPTitle);

    /// <summary>
    ///     Gets the XPComment exif tag.
    /// </summary>
    public static ExifTag<byte[]> XPComment => new(ExifTagValue.XPComment);

    /// <summary>
    ///     Gets the XPAuthor exif tag.
    /// </summary>
    public static ExifTag<byte[]> XPAuthor => new(ExifTagValue.XPAuthor);

    /// <summary>
    ///     Gets the XPKeywords exif tag.
    /// </summary>
    public static ExifTag<byte[]> XPKeywords => new(ExifTagValue.XPKeywords);

    /// <summary>
    ///     Gets the XPSubject exif tag.
    /// </summary>
    public static ExifTag<byte[]> XPSubject => new(ExifTagValue.XPSubject);

    /// <summary>
    ///     Gets the GPSVersionID exif tag.
    /// </summary>
    public static ExifTag<byte[]> GPSVersionID => new(ExifTagValue.GPSVersionID);
}