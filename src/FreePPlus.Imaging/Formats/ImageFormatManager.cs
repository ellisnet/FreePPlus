// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FreePPlus.Imaging.Formats;

//was previously: namespace SixLabors.ImageSharp.Formats;

/// <summary>
///     Collection of Image Formats to be used in <see cref="Configuration" /> class.
/// </summary>
public class ImageFormatManager
{
    /// <summary>
    ///     Used for locking against as there is no ConcurrentSet type.
    ///     <see href="https://github.com/dotnet/corefx/issues/6318" />
    /// </summary>
    private static readonly object HashLock = new();

    /// <summary>
    ///     The list of supported <see cref="IImageFormat" />s.
    /// </summary>
    private readonly HashSet<IImageFormat> imageFormats = new();

    /// <summary>
    ///     The list of supported <see cref="IImageEncoder" /> keyed to mime types.
    /// </summary>
    private readonly ConcurrentDictionary<IImageFormat, IImageDecoder> mimeTypeDecoders = new();

    /// <summary>
    ///     The list of supported <see cref="IImageEncoder" /> keyed to mime types.
    /// </summary>
    private readonly ConcurrentDictionary<IImageFormat, IImageEncoder> mimeTypeEncoders = new();

    /// <summary>
    ///     The list of supported <see cref="IImageFormatDetector" />s.
    /// </summary>
    private ConcurrentBag<IImageFormatDetector> imageFormatDetectors = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageFormatManager" /> class.
    /// </summary>
    public ImageFormatManager() { }

    /// <summary>
    ///     Gets the maximum header size of all the formats.
    /// </summary>
    internal int MaxHeaderSize { get; private set; }

    /// <summary>
    ///     Gets the currently registered <see cref="IImageFormat" />s.
    /// </summary>
    public IEnumerable<IImageFormat> ImageFormats => imageFormats;

    /// <summary>
    ///     Gets the currently registered <see cref="IImageFormatDetector" />s.
    /// </summary>
    internal IEnumerable<IImageFormatDetector> FormatDetectors => imageFormatDetectors;

    /// <summary>
    ///     Gets the currently registered <see cref="IImageDecoder" />s.
    /// </summary>
    internal IEnumerable<KeyValuePair<IImageFormat, IImageDecoder>> ImageDecoders => mimeTypeDecoders;

    /// <summary>
    ///     Gets the currently registered <see cref="IImageEncoder" />s.
    /// </summary>
    internal IEnumerable<KeyValuePair<IImageFormat, IImageEncoder>> ImageEncoders => mimeTypeEncoders;

    /// <summary>
    ///     Registers a new format provider.
    /// </summary>
    /// <param name="format">The format to register as a known format.</param>
    public void AddImageFormat(IImageFormat format)
    {
        Guard.NotNull(format, nameof(format));
        Guard.NotNull(format.MimeTypes, nameof(format.MimeTypes));
        Guard.NotNull(format.FileExtensions, nameof(format.FileExtensions));

        lock (HashLock)
        {
            if (!imageFormats.Contains(format)) imageFormats.Add(format);
        }
    }

    /// <summary>
    ///     For the specified file extensions type find the e <see cref="IImageFormat" />.
    /// </summary>
    /// <param name="extension">The extension to discover</param>
    /// <returns>The <see cref="IImageFormat" /> if found otherwise null</returns>
    public IImageFormat FindFormatByFileExtension(string extension)
    {
        Guard.NotNullOrWhiteSpace(extension, nameof(extension));

        if (extension[0] == '.') extension = extension.Substring(1);

        return imageFormats.FirstOrDefault(x => x.FileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    ///     For the specified mime type find the <see cref="IImageFormat" />.
    /// </summary>
    /// <param name="mimeType">The mime-type to discover</param>
    /// <returns>The <see cref="IImageFormat" /> if found; otherwise null</returns>
    public IImageFormat FindFormatByMimeType(string mimeType)
    {
        return imageFormats.FirstOrDefault(x => x.MimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>
    ///     Sets a specific image encoder as the encoder for a specific image format.
    /// </summary>
    /// <param name="imageFormat">The image format to register the encoder for.</param>
    /// <param name="encoder">The encoder to use,</param>
    public void SetEncoder(IImageFormat imageFormat, IImageEncoder encoder)
    {
        Guard.NotNull(imageFormat, nameof(imageFormat));
        Guard.NotNull(encoder, nameof(encoder));
        AddImageFormat(imageFormat);
        mimeTypeEncoders.AddOrUpdate(imageFormat, encoder, (s, e) => encoder);
    }

    /// <summary>
    ///     Sets a specific image decoder as the decoder for a specific image format.
    /// </summary>
    /// <param name="imageFormat">The image format to register the encoder for.</param>
    /// <param name="decoder">The decoder to use,</param>
    public void SetDecoder(IImageFormat imageFormat, IImageDecoder decoder)
    {
        Guard.NotNull(imageFormat, nameof(imageFormat));
        Guard.NotNull(decoder, nameof(decoder));
        AddImageFormat(imageFormat);
        mimeTypeDecoders.AddOrUpdate(imageFormat, decoder, (s, e) => decoder);
    }

    /// <summary>
    ///     Removes all the registered image format detectors.
    /// </summary>
    public void ClearImageFormatDetectors()
    {
        imageFormatDetectors = new ConcurrentBag<IImageFormatDetector>();
    }

    /// <summary>
    ///     Adds a new detector for detecting mime types.
    /// </summary>
    /// <param name="detector">The detector to add</param>
    public void AddImageFormatDetector(IImageFormatDetector detector)
    {
        Guard.NotNull(detector, nameof(detector));
        imageFormatDetectors.Add(detector);
        SetMaxHeaderSize();
    }

    /// <summary>
    ///     For the specified mime type find the decoder.
    /// </summary>
    /// <param name="format">The format to discover</param>
    /// <returns>The <see cref="IImageDecoder" /> if found otherwise null</returns>
    public IImageDecoder FindDecoder(IImageFormat format)
    {
        Guard.NotNull(format, nameof(format));

        return mimeTypeDecoders.TryGetValue(format, out var decoder)
            ? decoder
            : null;
    }

    /// <summary>
    ///     For the specified mime type find the encoder.
    /// </summary>
    /// <param name="format">The format to discover</param>
    /// <returns>The <see cref="IImageEncoder" /> if found otherwise null</returns>
    public IImageEncoder FindEncoder(IImageFormat format)
    {
        Guard.NotNull(format, nameof(format));

        return mimeTypeEncoders.TryGetValue(format, out var encoder)
            ? encoder
            : null;
    }

    /// <summary>
    ///     Sets the max header size.
    /// </summary>
    private void SetMaxHeaderSize()
    {
        MaxHeaderSize = imageFormatDetectors.Max(x => x.HeaderSize);
    }
}