// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using FreePPlus.Imaging.Formats;
using FreePPlus.Imaging.Metadata.Profiles.Exif;
using FreePPlus.Imaging.Metadata.Profiles.Icc;
using FreePPlus.Imaging.Metadata.Profiles.Iptc;

namespace FreePPlus.Imaging.Metadata;

//was previously: namespace SixLabors.ImageSharp.Metadata;

/// <summary>
///     Encapsulates the metadata of an image.
/// </summary>
public sealed class ImageMetadata : IDeepCloneable<ImageMetadata>
{
    /// <summary>
    ///     The default horizontal resolution value (dots per inch) in x direction.
    ///     <remarks>The default value is 96 <see cref="PixelResolutionUnit.PixelsPerInch" />.</remarks>
    /// </summary>
    public const double DefaultHorizontalResolution = 96;

    /// <summary>
    ///     The default vertical resolution value (dots per inch) in y direction.
    ///     <remarks>The default value is 96 <see cref="PixelResolutionUnit.PixelsPerInch" />.</remarks>
    /// </summary>
    public const double DefaultVerticalResolution = 96;

    /// <summary>
    ///     The default pixel resolution units.
    ///     <remarks>The default value is <see cref="PixelResolutionUnit.PixelsPerInch" />.</remarks>
    /// </summary>
    public const PixelResolutionUnit DefaultPixelResolutionUnits = PixelResolutionUnit.PixelsPerInch;

    private readonly Dictionary<IImageFormat, IDeepCloneable> formatMetadata = new();
    private double horizontalResolution;
    private double verticalResolution;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageMetadata" /> class.
    /// </summary>
    internal ImageMetadata()
    {
        horizontalResolution = DefaultHorizontalResolution;
        verticalResolution = DefaultVerticalResolution;
        ResolutionUnits = DefaultPixelResolutionUnits;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageMetadata" /> class
    ///     by making a copy from other metadata.
    /// </summary>
    /// <param name="other">
    ///     The other <see cref="ImageMetadata" /> to create this instance from.
    /// </param>
    private ImageMetadata(ImageMetadata other)
    {
        HorizontalResolution = other.HorizontalResolution;
        VerticalResolution = other.VerticalResolution;
        ResolutionUnits = other.ResolutionUnits;

        foreach (var meta in other.formatMetadata) formatMetadata.Add(meta.Key, meta.Value.DeepClone());

        ExifProfile = other.ExifProfile?.DeepClone();
        IccProfile = other.IccProfile?.DeepClone();
        IptcProfile = other.IptcProfile?.DeepClone();
    }

    /// <summary>
    ///     Gets or sets the resolution of the image in x- direction.
    ///     It is defined as the number of dots per inch and should be an positive value.
    /// </summary>
    /// <value>The density of the image in x- direction.</value>
    public double HorizontalResolution
    {
        get => horizontalResolution;

        set
        {
            if (value > 0) horizontalResolution = value;
        }
    }

    /// <summary>
    ///     Gets or sets the resolution of the image in y- direction.
    ///     It is defined as the number of dots per inch and should be an positive value.
    /// </summary>
    /// <value>The density of the image in y- direction.</value>
    public double VerticalResolution
    {
        get => verticalResolution;

        set
        {
            if (value > 0) verticalResolution = value;
        }
    }

    /// <summary>
    ///     Gets or sets unit of measure used when reporting resolution.
    ///     00 : No units; width:height pixel aspect ratio = Ydensity:Xdensity
    ///     01 : Pixels per inch (2.54 cm)
    ///     02 : Pixels per centimeter
    ///     03 : Pixels per meter
    /// </summary>
    public PixelResolutionUnit ResolutionUnits { get; set; }

    /// <summary>
    ///     Gets or sets the Exif profile.
    /// </summary>
    public ExifProfile ExifProfile { get; set; }

    /// <summary>
    ///     Gets or sets the list of ICC profiles.
    /// </summary>
    public IccProfile IccProfile { get; set; }

    /// <summary>
    ///     Gets or sets the iptc profile.
    /// </summary>
    public IptcProfile IptcProfile { get; set; }

    /// <inheritdoc />
    public ImageMetadata DeepClone()
    {
        return new ImageMetadata(this);
    }

    /// <summary>
    ///     Gets the metadata value associated with the specified key.
    /// </summary>
    /// <typeparam name="TFormatMetadata">The type of metadata.</typeparam>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>
    ///     The <typeparamref name="TFormatMetadata" />.
    /// </returns>
    public TFormatMetadata GetFormatMetadata<TFormatMetadata>(IImageFormat<TFormatMetadata> key)
        where TFormatMetadata : class, IDeepCloneable
    {
        if (formatMetadata.TryGetValue(key, out var meta)) return (TFormatMetadata)meta;

        var newMeta = key.CreateDefaultFormatMetadata();
        formatMetadata[key] = newMeta;
        return newMeta;
    }

    /// <summary>
    ///     Synchronizes the profiles with the current metadata.
    /// </summary>
    internal void SyncProfiles()
    {
        ExifProfile?.Sync(this);
    }
}