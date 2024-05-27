// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using FreePPlus.Imaging.Formats;
using FreePPlus.Imaging.Metadata;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Contains information about the image including dimensions, pixel type information and additional metadata
/// </summary>
internal sealed class ImageInfo : IImageInfo
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageInfo" /> class.
    /// </summary>
    /// <param name="pixelType">The image pixel type information.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="metadata">The images metadata.</param>
    /// <param name="format">Expected format of the image.</param>
    public ImageInfo(PixelTypeInfo pixelType, int width, int height, ImageMetadata metadata, IImageFormat format)
    {
        PixelType = pixelType;
        Width = width;
        Height = height;
        Metadata = metadata;
        Format = format;
    }

    /// <inheritdoc />
    public PixelTypeInfo PixelType { get; }

    /// <inheritdoc />
    public int Width { get; }

    /// <inheritdoc />
    public int Height { get; }

    /// <inheritdoc />
    public ImageMetadata Metadata { get; }

    /// <inheritdoc />
    public IImageFormat Format { get; }

    /// <inheritdoc />
    public double HorizontalResolution => Metadata?.HorizontalResolution
                                          ?? throw new InvalidOperationException(
                                              $"{nameof(HorizontalResolution)} value not available for this image.");

    /// <inheritdoc />
    public double VerticalResolution => Metadata?.VerticalResolution
                                        ?? throw new InvalidOperationException(
                                            $"{nameof(VerticalResolution)} value not available for this image.");
}