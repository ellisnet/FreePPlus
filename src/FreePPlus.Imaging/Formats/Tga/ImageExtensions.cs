// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.IO;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Formats.Tga;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Extension methods for the <see cref="Image" /> type.
/// </summary>
public static partial class ImageExtensions
{
    /// <summary>
    ///     Saves the image to the given stream with the tga format.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="stream">The stream to save the image to.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if the stream is null.</exception>
    public static void SaveAsTga(this Image source, Stream stream)
    {
        SaveAsTga(source, stream, null);
    }

    /// <summary>
    ///     Saves the image to the given stream with the tga format.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="stream">The stream to save the image to.</param>
    /// <param name="encoder">The options for the encoder.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if the stream is null.</exception>
    public static void SaveAsTga(this Image source, Stream stream, TgaEncoder encoder)
    {
        source.Save(
            stream,
            encoder ?? source.GetConfiguration().ImageFormatsManager.FindEncoder(TgaFormat.Instance));
    }
}