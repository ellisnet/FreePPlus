// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Formats.Bmp;
using System.IO;
using System.Threading.Tasks;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Extension methods for the <see cref="Image" /> type.
/// </summary>
public static partial class ImageExtensions
{
    /// <summary>
    ///     Saves the image to the given stream with the bmp format.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="stream">The stream to save the image to.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if the stream is null.</exception>
    public static void SaveAsBmp(this Image source, Stream stream)
    {
        source.SaveAsBmp(stream, null);
    }

    /// <summary>
    ///     Saves the image to the given stream with the bmp format.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="stream">The stream to save the image to.</param>
    /// <param name="encoder">The encoder to save the image with.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if the stream is null.</exception>
    public static void SaveAsBmp(this Image source, Stream stream, BmpEncoder encoder)
    {
        source.Save(
            stream,
            encoder ?? source.GetConfiguration().ImageFormatsManager.FindEncoder(BmpFormat.Instance));
    }

    /// <summary>
    ///     Saves the image to the given stream with the bmp format.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="stream">The stream to save the image to.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if the stream is null.</exception>
    public static async Task SaveAsBmpAsync(this Image source, Stream stream)
    {
        await Task.Run(() =>
        {
            source.SaveAsBmp(stream, null);
        });
    }

    /// <summary>
    ///     Saves the image to the given stream with the bmp format.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="stream">The stream to save the image to.</param>
    /// <param name="encoder">The encoder to save the image with.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if the stream is null.</exception>
    public static async Task SaveAsBmpAsync(this Image source, Stream stream, BmpEncoder encoder)
    {
        await Task.Run(() =>
        {
            source.Save(
                stream,
                encoder ?? source.GetConfiguration().ImageFormatsManager.FindEncoder(BmpFormat.Instance));
        });
    }
}
