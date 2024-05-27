// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace FreePPlus.Imaging.Formats.Gif;

//was previously: namespace SixLabors.ImageSharp.Formats.Gif;

/// <summary>
///     Registers the image encoders, decoders and mime type detectors for the gif format.
/// </summary>
public sealed class GifFormat : IImageFormat<GifMetadata, GifFrameMetadata>
{
    public const string FormatName = "GIF";

    private GifFormat() { }

    /// <summary>
    ///     Gets the current instance.
    /// </summary>
    public static GifFormat Instance { get; } = new();

    /// <inheritdoc />
    public string Name => FormatName;

    /// <inheritdoc />
    public string DefaultMimeType => "image/gif";

    /// <inheritdoc />
    public IEnumerable<string> MimeTypes => GifConstants.MimeTypes;

    /// <inheritdoc />
    public IEnumerable<string> FileExtensions => GifConstants.FileExtensions;

    /// <inheritdoc />
    public GifMetadata CreateDefaultFormatMetadata()
    {
        return new GifMetadata();
    }

    /// <inheritdoc />
    public GifFrameMetadata CreateDefaultFormatFrameMetadata()
    {
        return new GifFrameMetadata();
    }
}