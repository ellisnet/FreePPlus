﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <content>
///     Contains internal extensions for <see cref="Image{TPixel}" />
/// </content>
public static partial class ImageExtensions
{
    /// <summary>
    ///     Locks the image providing access to the pixels.
    ///     <remarks>
    ///         It is imperative that the accessor is correctly disposed off after use.
    ///     </remarks>
    /// </summary>
    /// <typeparam name="TPixel">The type of the pixel.</typeparam>
    /// <param name="image">The image.</param>
    /// <returns>
    ///     The <see cref="Buffer2D{TPixel}" />
    /// </returns>
    internal static Buffer2D<TPixel> GetRootFramePixelBuffer<TPixel>(this Image<TPixel> image)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return image.Frames.RootFrame.PixelBuffer;
    }
}