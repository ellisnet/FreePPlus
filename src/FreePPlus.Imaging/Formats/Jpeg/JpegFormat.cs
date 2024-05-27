// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace FreePPlus.Imaging.Formats.Jpeg;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg;

/// <summary>
///     Registers the image encoders, decoders and mime type detectors for the jpeg format.
/// </summary>
public sealed class JpegFormat : IImageFormat<JpegMetadata>
{
    public const string FormatName = "JPEG";

    private JpegFormat() { }

    /// <summary>
    ///     Gets the current instance.
    /// </summary>
    public static JpegFormat Instance { get; } = new();

    /// <inheritdoc />
    public string Name => FormatName;

    /// <inheritdoc />
    public string DefaultMimeType => "image/jpeg";

    /// <inheritdoc />
    public IEnumerable<string> MimeTypes => JpegConstants.MimeTypes;

    /// <inheritdoc />
    public IEnumerable<string> FileExtensions => JpegConstants.FileExtensions;

    /// <inheritdoc />
    public JpegMetadata CreateDefaultFormatMetadata()
    {
        return new JpegMetadata();
    }
}