// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Formats.Tga;

//was previously: namespace SixLabors.ImageSharp.Formats.Tga;

/// <summary>
///     Detects tga file headers.
/// </summary>
public sealed class TgaImageFormatDetector : IImageFormatDetector
{
    /// <inheritdoc />
    public int HeaderSize => TgaConstants.FileHeaderLength;

    /// <inheritdoc />
    public IImageFormat DetectFormat(ReadOnlySpan<byte> header)
    {
        return IsSupportedFileFormat(header) ? TgaFormat.Instance : null;
    }

    private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length >= HeaderSize)
        {
            // There are no magic bytes in a tga file, so at least the image type
            // and the colormap type in the header will be checked for a valid value.
            if (header[1] != 0 && header[1] != 1) return false;

            var imageType = (TgaImageType)header[2];
            return imageType.IsValid();
        }

        return false;
    }
}