// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Formats.Jpeg;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg;

/// <summary>
///     Provides Jpeg specific metadata information for the image.
/// </summary>
public class JpegMetadata : IDeepCloneable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JpegMetadata" /> class.
    /// </summary>
    public JpegMetadata() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="JpegMetadata" /> class.
    /// </summary>
    /// <param name="other">The metadata to create an instance from.</param>
    private JpegMetadata(JpegMetadata other)
    {
        Quality = other.Quality;
    }

    /// <summary>
    ///     Gets or sets the encoded quality.
    /// </summary>
    public int Quality { get; set; } = 75;

    /// <inheritdoc />
    public IDeepCloneable DeepClone()
    {
        return new JpegMetadata(this);
    }
}