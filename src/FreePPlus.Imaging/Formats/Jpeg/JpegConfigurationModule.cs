﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Formats.Jpeg;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg;

/// <summary>
///     Registers the image encoders, decoders and mime type detectors for the jpeg format.
/// </summary>
public sealed class JpegConfigurationModule : IConfigurationModule
{
    /// <inheritdoc />
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetEncoder(JpegFormat.Instance, new JpegEncoder());
        configuration.ImageFormatsManager.SetDecoder(JpegFormat.Instance, new JpegDecoder());
        configuration.ImageFormatsManager.AddImageFormatDetector(new JpegImageFormatDetector());
    }
}