// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Formats;
using FreePPlus.Imaging.Metadata;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Encapsulates properties that describe basic image information including dimensions, pixel type information
///     and additional metadata.
/// </summary>
public interface IImageInfo
{
    /// <summary>
    ///     Gets information about the image pixels.
    /// </summary>
    PixelTypeInfo PixelType { get; }

    /// <summary>
    ///     Gets the width.
    /// </summary>
    int Width { get; }

    /// <summary>
    ///     Gets the height.
    /// </summary>
    int Height { get; }

    /// <summary>
    ///     Gets the metadata of the image.
    /// </summary>
    ImageMetadata Metadata { get; }

    IImageFormat Format { get; }

    double HorizontalResolution { get; }

    double VerticalResolution { get; }
}