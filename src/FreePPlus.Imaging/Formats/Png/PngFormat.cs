// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace FreePPlus.Imaging.Formats.Png;

//was previously: namespace SixLabors.ImageSharp.Formats.Png;

/// <summary>
///     Registers the image encoders, decoders and mime type detectors for the png format.
/// </summary>
public sealed class PngFormat : IImageFormat<PngMetadata>
{
    public const string FormatName = "PNG";

    private PngFormat() { }

    /// <summary>
    ///     Gets the current instance.
    /// </summary>
    public static PngFormat Instance { get; } = new();

    /// <inheritdoc />
    public string Name => FormatName;

    /// <inheritdoc />
    public string DefaultMimeType => "image/png";

    /// <inheritdoc />
    public IEnumerable<string> MimeTypes => PngConstants.MimeTypes;

    /// <inheritdoc />
    public IEnumerable<string> FileExtensions => PngConstants.FileExtensions;

    /// <inheritdoc />
    public PngMetadata CreateDefaultFormatMetadata()
    {
        return new PngMetadata();
    }
}