// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace FreePPlus.Imaging.Formats.Tga;

//was previously: namespace SixLabors.ImageSharp.Formats.Tga;

/// <summary>
///     Registers the image encoders, decoders and mime type detectors for the tga format.
/// </summary>
public sealed class TgaFormat : IImageFormat<TgaMetadata>
{
    public const string FormatName = "TGA";

    private TgaFormat() { }

    /// <summary>
    ///     Gets the current instance.
    /// </summary>
    public static TgaFormat Instance { get; } = new();

    /// <inheritdoc />
    public string Name => FormatName;

    /// <inheritdoc />
    public string DefaultMimeType => "image/tga";

    /// <inheritdoc />
    public IEnumerable<string> MimeTypes => TgaConstants.MimeTypes;

    /// <inheritdoc />
    public IEnumerable<string> FileExtensions => TgaConstants.FileExtensions;

    /// <inheritdoc />
    public TgaMetadata CreateDefaultFormatMetadata()
    {
        return new TgaMetadata();
    }
}