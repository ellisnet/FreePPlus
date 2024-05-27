// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

/// <content />
public abstract partial class ExifTag
{
    /// <summary>
    ///     Gets the PixelScale exif tag.
    /// </summary>
    public static ExifTag<double[]> PixelScale { get; } = new(ExifTagValue.PixelScale);

    /// <summary>
    ///     Gets the IntergraphMatrix exif tag.
    /// </summary>
    public static ExifTag<double[]> IntergraphMatrix { get; } = new(ExifTagValue.IntergraphMatrix);

    /// <summary>
    ///     Gets the ModelTiePoint exif tag.
    /// </summary>
    public static ExifTag<double[]> ModelTiePoint { get; } = new(ExifTagValue.ModelTiePoint);

    /// <summary>
    ///     Gets the ModelTransform exif tag.
    /// </summary>
    public static ExifTag<double[]> ModelTransform { get; } = new(ExifTagValue.ModelTransform);
}