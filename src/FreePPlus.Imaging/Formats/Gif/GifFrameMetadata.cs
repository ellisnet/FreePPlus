// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Formats.Gif;

//was previously: namespace SixLabors.ImageSharp.Formats.Gif;

/// <summary>
///     Provides Gif specific metadata information for the image frame.
/// </summary>
public class GifFrameMetadata : IDeepCloneable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GifFrameMetadata" /> class.
    /// </summary>
    public GifFrameMetadata() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GifFrameMetadata" /> class.
    /// </summary>
    /// <param name="other">The metadata to create an instance from.</param>
    private GifFrameMetadata(GifFrameMetadata other)
    {
        ColorTableLength = other.ColorTableLength;
        FrameDelay = other.FrameDelay;
        DisposalMethod = other.DisposalMethod;
    }

    /// <summary>
    ///     Gets or sets the length of the color table for paletted images.
    ///     If not 0, then this field indicates the maximum number of colors to use when quantizing the
    ///     image frame.
    /// </summary>
    public int ColorTableLength { get; set; }

    /// <summary>
    ///     Gets or sets the frame delay for animated images.
    ///     If not 0, when utilized in Gif animation, this field specifies the number of hundredths (1/100) of a second to
    ///     wait before continuing with the processing of the Data Stream.
    ///     The clock starts ticking immediately after the graphic is rendered.
    /// </summary>
    public int FrameDelay { get; set; }

    /// <summary>
    ///     Gets or sets the disposal method for animated images.
    ///     Primarily used in Gif animation, this field indicates the way in which the graphic is to
    ///     be treated after being displayed.
    /// </summary>
    public GifDisposalMethod DisposalMethod { get; set; }

    /// <inheritdoc />
    public IDeepCloneable DeepClone()
    {
        return new GifFrameMetadata(this);
    }
}