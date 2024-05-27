// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Formats;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.Metadata;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Encapsulates an image, which consists of the pixel data for a graphics image and its attributes.
///     For generic <see cref="Image{TPixel}" />-s the pixel type is known at compile time.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
public sealed class Image<TPixel> : Image
    where TPixel : unmanaged, IPixel<TPixel>
{
    private bool isDisposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Image{TPixel}" /> class
    ///     with the height and the width of the image.
    /// </summary>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    public Image(Configuration configuration, int width, int height, IImageFormat expectedFormat)
        : this(configuration, width, height, new ImageMetadata(), expectedFormat) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Image{TPixel}" /> class
    ///     with the height and the width of the image.
    /// </summary>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="backgroundColor">The color to initialize the pixels with.</param>
    public Image(Configuration configuration, int width, int height, TPixel backgroundColor,
        IImageFormat expectedFormat)
        : this(configuration, width, height, backgroundColor, new ImageMetadata(), expectedFormat) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Image{TPixel}" /> class
    ///     with the height and the width of the image.
    /// </summary>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="backgroundColor">The color to initialize the pixels with.</param>
    public Image(int width, int height, TPixel backgroundColor, IImageFormat expectedFormat)
        : this(Configuration.Default, width, height, backgroundColor, new ImageMetadata(), expectedFormat) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Image{TPixel}" /> class
    ///     with the height and the width of the image.
    /// </summary>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    public Image(int width, int height, IImageFormat expectedFormat)
        : this(Configuration.Default, width, height, expectedFormat) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Image{TPixel}" /> class
    ///     with the height and the width of the image.
    /// </summary>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="metadata">The images metadata.</param>
    internal Image(Configuration configuration, int width, int height, ImageMetadata metadata,
        IImageFormat expectedFormat)
        : base(configuration, PixelTypeInfo.Create<TPixel>(), metadata, width, height, expectedFormat)
    {
        Frames = new ImageFrameCollection<TPixel>(this, width, height, default(TPixel), expectedFormat);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Image{TPixel}" /> class
    ///     wrapping an external <see cref="MemoryGroup{T}" />.
    /// </summary>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <param name="memoryGroup">The memory source.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="metadata">The images metadata.</param>
    internal Image(
        Configuration configuration,
        MemoryGroup<TPixel> memoryGroup,
        int width,
        int height,
        ImageMetadata metadata,
        IImageFormat expectedFormat)
        : base(configuration, PixelTypeInfo.Create<TPixel>(), metadata, width, height, expectedFormat)
    {
        Frames = new ImageFrameCollection<TPixel>(this, width, height, memoryGroup, expectedFormat);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Image{TPixel}" /> class
    ///     with the height and the width of the image.
    /// </summary>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="backgroundColor">The color to initialize the pixels with.</param>
    /// <param name="metadata">The images metadata.</param>
    internal Image(
        Configuration configuration,
        int width,
        int height,
        TPixel backgroundColor,
        ImageMetadata metadata,
        IImageFormat expectedFormat)
        : base(configuration, PixelTypeInfo.Create<TPixel>(), metadata, width, height, expectedFormat)
    {
        Frames = new ImageFrameCollection<TPixel>(this, width, height, backgroundColor, expectedFormat);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Image{TPixel}" /> class
    ///     with the height and the width of the image.
    /// </summary>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <param name="metadata">The images metadata.</param>
    /// <param name="frames">The frames that will be owned by this image instance.</param>
    internal Image(Configuration configuration, ImageMetadata metadata, IEnumerable<ImageFrame<TPixel>> frames,
        IImageFormat expectedFormat)
        : base(configuration, PixelTypeInfo.Create<TPixel>(), metadata, ValidateFramesAndGetSize(frames),
            expectedFormat)
    {
        Frames = new ImageFrameCollection<TPixel>(this, frames, expectedFormat);
    }

    /// <inheritdoc />
    protected override ImageFrameCollection NonGenericFrameCollection => Frames;

    /// <summary>
    ///     Gets the collection of image frames.
    /// </summary>
    public new ImageFrameCollection<TPixel> Frames { get; }

    /// <summary>
    ///     Gets the root frame.
    /// </summary>
    private IPixelSource<TPixel> PixelSource =>
        Frames?.RootFrame ?? throw new ObjectDisposedException(nameof(Image<TPixel>));

    /// <summary>
    ///     Gets or sets the pixel at the specified position.
    /// </summary>
    /// <param name="x">
    ///     The x-coordinate of the pixel. Must be greater than or equal to zero and less than the width of the
    ///     image.
    /// </param>
    /// <param name="y">
    ///     The y-coordinate of the pixel. Must be greater than or equal to zero and less than the height of the
    ///     image.
    /// </param>
    /// <returns>The <see typeparam="TPixel" /> at the specified position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the provided (x,y) coordinates are outside the image
    ///     boundary.
    /// </exception>
    public TPixel this[int x, int y]
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        get
        {
            VerifyCoords(x, y);
            return PixelSource.PixelBuffer.GetElementUnsafe(x, y);
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        set
        {
            VerifyCoords(x, y);
            PixelSource.PixelBuffer.GetElementUnsafe(x, y) = value;
        }
    }

    /// <summary>
    ///     Gets the representation of the pixels as a <see cref="Span{T}" /> of contiguous memory
    ///     at row <paramref name="rowIndex" /> beginning from the first pixel on that row.
    /// </summary>
    /// <param name="rowIndex">The row.</param>
    /// <returns>The <see cref="Span{TPixel}" /></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when row index is out of range.</exception>
    public Span<TPixel> GetPixelRowSpan(int rowIndex)
    {
        Guard.MustBeGreaterThanOrEqualTo(rowIndex, 0, nameof(rowIndex));
        Guard.MustBeLessThan(rowIndex, Height, nameof(rowIndex));

        return PixelSource.PixelBuffer.GetRowSpan(rowIndex);
    }

    /// <summary>
    ///     Gets the representation of the pixels as a <see cref="Span{T}" /> in the source image's pixel format
    ///     stored in row major order, if the backing buffer is contiguous.
    /// </summary>
    /// <param name="span">The <see cref="Span{T}" />.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public bool TryGetSinglePixelSpan(out Span<TPixel> span)
    {
        var mg = this.GetPixelMemoryGroup();
        if (mg.Count > 1)
        {
            span = default;
            return false;
        }

        span = mg.Single().Span;
        return true;
    }

    /// <summary>
    ///     Clones the current image
    /// </summary>
    /// <returns>Returns a new image with all the same metadata as the original.</returns>
    public Image<TPixel> Clone()
    {
        return Clone(this.GetConfiguration());
    }

    /// <summary>
    ///     Clones the current image with the given configuration.
    /// </summary>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <returns>Returns a new <see cref="Image{TPixel}" /> with all the same pixel data as the original.</returns>
    public Image<TPixel> Clone(Configuration configuration)
    {
        EnsureNotDisposed();

        var clonedFrames = new ImageFrame<TPixel>[Frames.Count];
        for (var i = 0; i < clonedFrames.Length; i++) clonedFrames[i] = Frames[i].Clone(configuration);

        return new Image<TPixel>(configuration, Metadata.DeepClone(), clonedFrames, Format);
    }

    /// <summary>
    ///     Returns a copy of the image in the given pixel format.
    /// </summary>
    /// <typeparam name="TPixel2">The pixel format.</typeparam>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <returns>The <see cref="Image{TPixel2}" />.</returns>
    public override Image<TPixel2> CloneAs<TPixel2>(Configuration configuration)
    {
        EnsureNotDisposed();

        var clonedFrames = new ImageFrame<TPixel2>[Frames.Count];
        for (var i = 0; i < clonedFrames.Length; i++) clonedFrames[i] = Frames[i].CloneAs<TPixel2>(configuration);

        return new Image<TPixel2>(configuration, Metadata.DeepClone(), clonedFrames, Format);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (isDisposed) return;

        if (disposing) Frames.Dispose();

        isDisposed = true;
    }

    /// <inheritdoc />
    internal override void EnsureNotDisposed()
    {
        if (isDisposed) throw new ObjectDisposedException("Trying to execute an operation on a disposed image.");
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Image<{typeof(TPixel).Name}>: {Width}x{Height}";
    }

    /// <inheritdoc />
    internal override void Accept(IImageVisitor visitor)
    {
        EnsureNotDisposed();

        visitor.Visit(this);
    }

    /// <summary>
    ///     Switches the buffers used by the image and the pixelSource meaning that the Image will "own" the buffer from the
    ///     pixelSource and the pixelSource will now own the Images buffer.
    /// </summary>
    /// <param name="pixelSource">The pixel source.</param>
    internal void SwapOrCopyPixelsBuffersFrom(Image<TPixel> pixelSource)
    {
        Guard.NotNull(pixelSource, nameof(pixelSource));

        for (var i = 0; i < Frames.Count; i++) Frames[i].SwapOrCopyPixelsBufferFrom(pixelSource.Frames[i]);

        UpdateSize(pixelSource.Size());
    }

    private static Size ValidateFramesAndGetSize(IEnumerable<ImageFrame<TPixel>> frames)
    {
        Guard.NotNull(frames, nameof(frames));

        var rootFrame = frames.FirstOrDefault();

        if (rootFrame == null) throw new ArgumentException("Must not be empty.", nameof(frames));

        var rootSize = rootFrame.Size();

        if (frames.Any(f => f.Size() != rootSize))
            throw new ArgumentException("The provided frames must be of the same size.", nameof(frames));

        return rootSize;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private void VerifyCoords(int x, int y)
    {
        if (x < 0 || x >= Width) ThrowArgumentOutOfRangeException(nameof(x));

        if (y < 0 || y >= Height) ThrowArgumentOutOfRangeException(nameof(y));
    }

    [MethodImpl(InliningOptions.ColdPath)]
    private static void ThrowArgumentOutOfRangeException(string paramName)
    {
        throw new ArgumentOutOfRangeException(paramName);
    }
}