// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using FreePPlus.Imaging.Formats;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <content>
///     Adds static methods allowing the creation of new image from a given file.
/// </content>
public abstract partial class Image
{
    /// <summary>
    ///     By reading the header on the provided file this calculates the images mime type.
    /// </summary>
    /// <param name="filePath">The image file to open and to read the header from.</param>
    /// <returns>The mime type or null if none found.</returns>
    public static IImageFormat DetectFormat(string filePath)
    {
        return DetectFormat(Configuration.Default, filePath);
    }

    /// <summary>
    ///     By reading the header on the provided file this calculates the images mime type.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="filePath">The image file to open and to read the header from.</param>
    /// <exception cref="ArgumentNullException">The configuration is null.</exception>
    /// <returns>The mime type or null if none found.</returns>
    public static IImageFormat DetectFormat(Configuration configuration, string filePath)
    {
        Guard.NotNull(configuration, nameof(configuration));

        using (var file = configuration.FileSystem.OpenRead(filePath))
        {
            return DetectFormat(configuration, file);
        }
    }

    /// <summary>
    ///     Reads the raw image information from the specified stream without fully decoding it.
    /// </summary>
    /// <param name="filePath">The image file to open and to read the header from.</param>
    /// <returns>
    ///     The <see cref="IImageInfo" /> or null if suitable info detector not found.
    /// </returns>
    public static IImageInfo Identify(string filePath)
    {
        return Identify(filePath, out var _);
    }

    /// <summary>
    ///     Reads the raw image information from the specified stream without fully decoding it.
    /// </summary>
    /// <param name="filePath">The image file to open and to read the header from.</param>
    /// <param name="format">The format type of the decoded image.</param>
    /// <returns>
    ///     The <see cref="IImageInfo" /> or null if suitable info detector not found.
    /// </returns>
    public static IImageInfo Identify(string filePath, out IImageFormat format)
    {
        return Identify(Configuration.Default, filePath, out format);
    }

    /// <summary>
    ///     Reads the raw image information from the specified stream without fully decoding it.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="filePath">The image file to open and to read the header from.</param>
    /// <param name="format">The format type of the decoded image.</param>
    /// <exception cref="ArgumentNullException">The configuration is null.</exception>
    /// <returns>
    ///     The <see cref="IImageInfo" /> or null if suitable info detector is not found.
    /// </returns>
    public static IImageInfo Identify(Configuration configuration, string filePath, out IImageFormat format)
    {
        Guard.NotNull(configuration, nameof(configuration));
        using (var file = configuration.FileSystem.OpenRead(filePath))
        {
            return Identify(configuration, file, out format);
        }
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Image" /> class from the given file.
    /// </summary>
    /// <param name="path">The file path to the image.</param>
    /// <exception cref="NotSupportedException">
    ///     Thrown if the stream is not readable nor seekable.
    /// </exception>
    /// <returns>The <see cref="Image" />.</returns>
    public static Image Load(string path)
    {
        return Load(Configuration.Default, path);
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Image" /> class from the given file.
    /// </summary>
    /// <param name="path">The file path to the image.</param>
    /// <param name="format">The mime type of the decoded image.</param>
    /// <exception cref="NotSupportedException">
    ///     Thrown if the stream is not readable nor seekable.
    /// </exception>
    /// <returns>A new <see cref="Image{Rgba32}" />.</returns>
    public static Image Load(string path, out IImageFormat format)
    {
        return Load(Configuration.Default, path, out format);
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Image" /> class from the given file.
    /// </summary>
    /// <param name="configuration">The configuration for the decoder.</param>
    /// <param name="path">The file path to the image.</param>
    /// <exception cref="ArgumentNullException">The configuration is null.</exception>
    /// <exception cref="ArgumentNullException">The path is null.</exception>
    /// <exception cref="UnknownImageFormatException">Image format not recognised.</exception>
    /// <exception cref="InvalidImageContentException">Image contains invalid content.</exception>
    /// <returns>The <see cref="Image" />.</returns>
    public static Image Load(Configuration configuration, string path)
    {
        return Load(configuration, path, out _);
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Image" /> class from the given file.
    /// </summary>
    /// <param name="configuration">The Configuration.</param>
    /// <param name="path">The file path to the image.</param>
    /// <param name="decoder">The decoder.</param>
    /// <exception cref="ArgumentNullException">The configuration is null.</exception>
    /// <exception cref="ArgumentNullException">The path is null.</exception>
    /// <exception cref="ArgumentNullException">The decoder is null.</exception>
    /// <exception cref="UnknownImageFormatException">Image format not recognised.</exception>
    /// <exception cref="InvalidImageContentException">Image contains invalid content.</exception>
    /// <returns>The <see cref="Image" />.</returns>
    public static Image Load(Configuration configuration, string path, IImageDecoder decoder)
    {
        Guard.NotNull(configuration, nameof(configuration));
        Guard.NotNull(path, nameof(path));

        using (var stream = configuration.FileSystem.OpenRead(path))
        {
            return Load(configuration, stream, decoder);
        }
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Image" /> class from the given file.
    /// </summary>
    /// <param name="path">The file path to the image.</param>
    /// <param name="decoder">The decoder.</param>
    /// <exception cref="ArgumentNullException">The path is null.</exception>
    /// <exception cref="ArgumentNullException">The decoder is null.</exception>
    /// <exception cref="UnknownImageFormatException">Image format not recognised.</exception>
    /// <exception cref="InvalidImageContentException">Image contains invalid content.</exception>
    /// <returns>The <see cref="Image" />.</returns>
    public static Image Load(string path, IImageDecoder decoder)
    {
        return Load(Configuration.Default, path, decoder);
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Image{TPixel}" /> class from the given file.
    /// </summary>
    /// <param name="path">The file path to the image.</param>
    /// <exception cref="ArgumentNullException">The path is null.</exception>
    /// <exception cref="UnknownImageFormatException">Image format not recognised.</exception>
    /// <exception cref="InvalidImageContentException">Image contains invalid content.</exception>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <returns>A new <see cref="Image{TPixel}" />.</returns>
    public static Image<TPixel> Load<TPixel>(string path)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return Load<TPixel>(Configuration.Default, path);
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Image{TPixel}" /> class from the given file.
    /// </summary>
    /// <param name="path">The file path to the image.</param>
    /// <param name="format">The mime type of the decoded image.</param>
    /// <exception cref="ArgumentNullException">The path is null.</exception>
    /// <exception cref="UnknownImageFormatException">Image format not recognised.</exception>
    /// <exception cref="InvalidImageContentException">Image contains invalid content.</exception>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <returns>A new <see cref="Image{TPixel}" />.</returns>
    public static Image<TPixel> Load<TPixel>(string path, out IImageFormat format)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return Load<TPixel>(Configuration.Default, path, out format);
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Image{TPixel}" /> class from the given file.
    /// </summary>
    /// <param name="configuration">The configuration options.</param>
    /// <param name="path">The file path to the image.</param>
    /// <exception cref="ArgumentNullException">The configuration is null.</exception>
    /// <exception cref="ArgumentNullException">The path is null.</exception>
    /// <exception cref="UnknownImageFormatException">Image format not recognised.</exception>
    /// <exception cref="InvalidImageContentException">Image contains invalid content.</exception>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <returns>A new <see cref="Image{TPixel}" />.</returns>
    public static Image<TPixel> Load<TPixel>(Configuration configuration, string path)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(configuration, nameof(configuration));
        Guard.NotNull(path, nameof(path));

        using (var stream = configuration.FileSystem.OpenRead(path))
        {
            return Load<TPixel>(configuration, stream);
        }
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Image{TPixel}" /> class from the given file.
    /// </summary>
    /// <param name="configuration">The configuration options.</param>
    /// <param name="path">The file path to the image.</param>
    /// <param name="format">The mime type of the decoded image.</param>
    /// <exception cref="ArgumentNullException">The configuration is null.</exception>
    /// <exception cref="ArgumentNullException">The path is null.</exception>
    /// <exception cref="UnknownImageFormatException">Image format not recognised.</exception>
    /// <exception cref="InvalidImageContentException">Image contains invalid content.</exception>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <returns>A new <see cref="Image{TPixel}" />.</returns>
    public static Image<TPixel> Load<TPixel>(Configuration configuration, string path, out IImageFormat format)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(configuration, nameof(configuration));
        Guard.NotNull(path, nameof(path));

        using (var stream = configuration.FileSystem.OpenRead(path))
        {
            return Load<TPixel>(configuration, stream, out format);
        }
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Image" /> class from the given file.
    ///     The pixel type is selected by the decoder.
    /// </summary>
    /// <param name="configuration">The configuration options.</param>
    /// <param name="path">The file path to the image.</param>
    /// <param name="format">The mime type of the decoded image.</param>
    /// <exception cref="ArgumentNullException">The configuration is null.</exception>
    /// <exception cref="ArgumentNullException">The path is null.</exception>
    /// <exception cref="UnknownImageFormatException">Image format not recognised.</exception>
    /// <exception cref="InvalidImageContentException">Image contains invalid content.</exception>
    /// <returns>A new <see cref="Image{TPixel}" />.</returns>
    public static Image Load(Configuration configuration, string path, out IImageFormat format)
    {
        Guard.NotNull(configuration, nameof(configuration));
        Guard.NotNull(path, nameof(path));

        using (var stream = configuration.FileSystem.OpenRead(path))
        {
            return Load(configuration, stream, out format);
        }
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Image{TPixel}" /> class from the given file.
    /// </summary>
    /// <param name="path">The file path to the image.</param>
    /// <param name="decoder">The decoder.</param>
    /// <exception cref="ArgumentNullException">The path is null.</exception>
    /// <exception cref="UnknownImageFormatException">Image format not recognised.</exception>
    /// <exception cref="InvalidImageContentException">Image contains invalid content.</exception>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <returns>A new <see cref="Image{TPixel}" />.</returns>
    public static Image<TPixel> Load<TPixel>(string path, IImageDecoder decoder)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return Load<TPixel>(Configuration.Default, path, decoder);
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Image{TPixel}" /> class from the given file.
    /// </summary>
    /// <param name="configuration">The Configuration.</param>
    /// <param name="path">The file path to the image.</param>
    /// <param name="decoder">The decoder.</param>
    /// <exception cref="ArgumentNullException">The configuration is null.</exception>
    /// <exception cref="ArgumentNullException">The path is null.</exception>
    /// <exception cref="ArgumentNullException">The decoder is null.</exception>
    /// <exception cref="UnknownImageFormatException">Image format not recognised.</exception>
    /// <exception cref="InvalidImageContentException">Image contains invalid content.</exception>
    /// <typeparam name="TPixel">The pixel format.</typeparam>
    /// <returns>A new <see cref="Image{TPixel}" />.</returns>
    public static Image<TPixel> Load<TPixel>(Configuration configuration, string path, IImageDecoder decoder)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(configuration, nameof(configuration));
        Guard.NotNull(path, nameof(path));

        using (var stream = configuration.FileSystem.OpenRead(path))
        {
            return Load<TPixel>(configuration, stream, decoder);
        }
    }
}