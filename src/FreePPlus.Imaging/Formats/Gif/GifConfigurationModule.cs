﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Formats.Gif;

//was previously: namespace SixLabors.ImageSharp.Formats.Gif;

/// <summary>
///     Registers the image encoders, decoders and mime type detectors for the gif format.
/// </summary>
public sealed class GifConfigurationModule : IConfigurationModule
{
    /// <inheritdoc />
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetEncoder(GifFormat.Instance, new GifEncoder());
        configuration.ImageFormatsManager.SetDecoder(GifFormat.Instance, new GifDecoder());
        configuration.ImageFormatsManager.AddImageFormatDetector(new GifImageFormatDetector());
    }
}