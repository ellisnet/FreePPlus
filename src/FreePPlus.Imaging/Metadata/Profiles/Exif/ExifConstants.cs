// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Exif;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal static class ExifConstants
{
    public static ReadOnlySpan<byte> LittleEndianByteOrderMarker => new byte[]
    {
        (byte)'I',
        (byte)'I',
        0x2A,
        0x00
    };

    public static ReadOnlySpan<byte> BigEndianByteOrderMarker => new byte[]
    {
        (byte)'M',
        (byte)'M',
        0x00,
        0x2A
    };
}