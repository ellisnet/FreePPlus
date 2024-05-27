// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Formats.Jpeg;
using FreePPlus.Imaging.Metadata;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Extension methods for the <see cref="ImageMetadata" /> type.
/// </summary>
public static partial class MetadataExtensions
{
    /// <summary>
    ///     Gets the jpeg format specific metadata for the image.
    /// </summary>
    /// <param name="metadata">The metadata this method extends.</param>
    /// <returns>The <see cref="JpegMetadata" />.</returns>
    public static JpegMetadata GetJpegMetadata(this ImageMetadata metadata)
    {
        return metadata.GetFormatMetadata(JpegFormat.Instance);
    }
}