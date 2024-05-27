﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Formats;

//was previously: namespace SixLabors.ImageSharp.Formats;

/// <summary>
///     Contains information about the pixels that make up an images visual data.
/// </summary>
public class PixelTypeInfo
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PixelTypeInfo" /> class.
    /// </summary>
    /// <param name="bitsPerPixel">Color depth, in number of bits per pixel.</param>
    internal PixelTypeInfo(int bitsPerPixel)
    {
        BitsPerPixel = bitsPerPixel;
    }

    /// <summary>
    ///     Gets color depth, in number of bits per pixel.
    /// </summary>
    public int BitsPerPixel { get; }

    internal static PixelTypeInfo Create<TPixel>() where TPixel : unmanaged, IPixel<TPixel>
    {
        return new PixelTypeInfo(Unsafe.SizeOf<TPixel>() * 8);
    }
}