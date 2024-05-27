// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Formats;
using FreePPlus.Imaging.Metadata;
using FreePPlus.Imaging.PixelFormats;

[assembly: InternalsVisibleTo("FreePPlus.OfficeOpenXml.Tests")]

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Encapsulates an image, which consists of the pixel data for a graphics image and its attributes.
///     For the non-generic <see cref="Image" /> type, the pixel type is only known at runtime.
///     <see cref="Image" /> is always implemented by a pixel-specific <see cref="Image{TPixel}" /> instance.
/// </summary>
public abstract partial class Image : IImageJ, IConfigurationProvider
{
    private readonly Configuration _configuration;
    private Size _size;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Image" /> class.
    /// </summary>
    /// <param name="configuration">
    ///     The configuration which allows altering default behaviour or extending the library.
    /// </param>
    /// <param name="pixelType">The <see cref="PixelTypeInfo" />.</param>
    /// <param name="metadata">The <see cref="ImageMetadata" />.</param>
    /// <param name="size">The <see cref="_size" />.</param>
    /// <param name="format">The <see cref="Format" />.</param>
    protected Image(Configuration configuration, PixelTypeInfo pixelType, ImageMetadata metadata, Size size,
        IImageFormat format)
    {
        _configuration = configuration ?? Configuration.Default;
        PixelType = pixelType;
        _size = size;
        Metadata = metadata ?? new ImageMetadata();
        Format = format;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Image" /> class.
    /// </summary>
    internal Image(
        Configuration configuration,
        PixelTypeInfo pixelType,
        ImageMetadata metadata,
        int width,
        int height,
        IImageFormat format)
        : this(configuration, pixelType, metadata, new Size(width, height), format) { }

    /// <summary>
    ///     Gets the <see cref="ImageFrameCollection" /> implementing the public <see cref="Frames" /> property.
    /// </summary>
    protected abstract ImageFrameCollection NonGenericFrameCollection { get; }

    /// <summary>
    ///     Gets the frames of the image as (non-generic) <see cref="ImageFrameCollection" />.
    /// </summary>
    public ImageFrameCollection Frames => NonGenericFrameCollection;

    /// <inheritdoc />
    Configuration IConfigurationProvider.Configuration => _configuration;

    /// <inheritdoc />
    public PixelTypeInfo PixelType { get; }

    /// <inheritdoc />
    public int Width => _size.Width;

    /// <inheritdoc />
    public int Height => _size.Height;

    /// <inheritdoc />
    public ImageMetadata Metadata { get; }

    /// <inheritdoc />
    public IImageFormat Format { get; }

    /// <inheritdoc />
    public double HorizontalResolution => Metadata?.HorizontalResolution
                                          ?? throw new InvalidOperationException(
                                              $"{nameof(HorizontalResolution)} value not available for this image.");

    /// <inheritdoc />
    public double VerticalResolution => Metadata?.VerticalResolution
                                        ?? throw new InvalidOperationException(
                                            $"{nameof(VerticalResolution)} value not available for this image.");

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Saves the image to the given stream using the given image encoder.
    /// </summary>
    /// <param name="stream">The stream to save the image to.</param>
    /// <param name="encoder">The encoder to save the image with.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if the stream or encoder is null.</exception>
    public void Save(Stream stream, IImageEncoder encoder)
    {
        Guard.NotNull(stream, nameof(stream));
        Guard.NotNull(encoder, nameof(encoder));
        EnsureNotDisposed();

        this.AcceptVisitor(new EncodeVisitor(encoder, stream));
    }

    /// <summary>
    ///     Returns a copy of the image in the given pixel format.
    /// </summary>
    /// <typeparam name="TPixel2">The pixel format.</typeparam>
    /// <returns>The <see cref="Image{TPixel2}" /></returns>
    public Image<TPixel2> CloneAs<TPixel2>()
        where TPixel2 : unmanaged, IPixel<TPixel2>
    {
        return CloneAs<TPixel2>(this.GetConfiguration());
    }

    /// <summary>
    ///     Returns a copy of the image in the given pixel format.
    /// </summary>
    /// <typeparam name="TPixel2">The pixel format.</typeparam>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <returns>The <see cref="Image{TPixel2}" />.</returns>
    public abstract Image<TPixel2> CloneAs<TPixel2>(Configuration configuration)
        where TPixel2 : unmanaged, IPixel<TPixel2>;

    /// <summary>
    ///     Update the size of the image after mutation.
    /// </summary>
    /// <param name="size">The <see cref="Size" />.</param>
    protected void UpdateSize(Size size)
    {
        _size = size;
    }

    /// <summary>
    ///     Disposes the object and frees resources for the Garbage Collector.
    /// </summary>
    /// <param name="disposing">Whether to dispose of managed and unmanaged objects.</param>
    protected abstract void Dispose(bool disposing);

    /// <summary>
    ///     Throws <see cref="ObjectDisposedException" /> if the image is disposed.
    /// </summary>
    internal abstract void EnsureNotDisposed();

    /// <summary>
    ///     Accepts a <see cref="IImageVisitor" />.
    ///     Implemented by <see cref="Image{TPixel}" /> invoking <see cref="IImageVisitor.Visit{TPixel}" />
    ///     with the pixel type of the image.
    /// </summary>
    /// <param name="visitor">The visitor.</param>
    internal abstract void Accept(IImageVisitor visitor);

    private class EncodeVisitor : IImageVisitor
    {
        private readonly IImageEncoder _encoder;

        private readonly Stream _stream;

        public EncodeVisitor(IImageEncoder encoder, Stream stream)
        {
            _encoder = encoder;
            _stream = stream;
        }

        public void Visit<TPixel>(Image<TPixel> image)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            _encoder.Encode(image, _stream);
        }
    }
}