// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers.Binary;

namespace FreePPlus.Imaging.Formats.Bmp;

//was previously: namespace SixLabors.ImageSharp.Formats.Bmp;

/// <summary>
///     Detects bmp file headers.
/// </summary>
public sealed class BmpImageFormatDetector : IImageFormatDetector
{
    /// <inheritdoc />
    public int HeaderSize => 2;

    /// <inheritdoc />
    public IImageFormat DetectFormat(ReadOnlySpan<byte> header)
    {
        return IsSupportedFileFormat(header) ? BmpFormat.Instance : null;
    }

    private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length >= HeaderSize)
        {
            var fileTypeMarker = BinaryPrimitives.ReadInt16LittleEndian(header);
            return fileTypeMarker == BmpConstants.TypeMarkers.Bitmap ||
                   fileTypeMarker == BmpConstants.TypeMarkers.BitmapArray;
        }

        return false;
    }
}