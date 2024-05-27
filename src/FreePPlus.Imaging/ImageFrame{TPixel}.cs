// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.Metadata;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Represents a pixel-specific image frame containing all pixel data and <see cref="ImageFrameMetadata" />.
///     In case of animated formats like gif, it contains the single frame in a animation.
///     In all other cases it is the only frame of the image.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
public sealed class ImageFrame<TPixel> : ImageFrame, IPixelSource<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private bool isDisposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageFrame{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    internal ImageFrame(Configuration configuration, int width, int height)
        : this(configuration, width, height, new ImageFrameMetadata()) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageFrame{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="size">The <see cref="Size" /> of the frame.</param>
    /// <param name="metadata">The metadata.</param>
    internal ImageFrame(Configuration configuration, Size size, ImageFrameMetadata metadata)
        : this(configuration, size.Width, size.Height, metadata) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageFrame{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="metadata">The metadata.</param>
    internal ImageFrame(Configuration configuration, int width, int height, ImageFrameMetadata metadata)
        : base(configuration, width, height, metadata)
    {
        Guard.MustBeGreaterThan(width, 0, nameof(width));
        Guard.MustBeGreaterThan(height, 0, nameof(height));

        PixelBuffer = this.GetConfiguration().MemoryAllocator
            .Allocate2D<TPixel>(width, height, AllocationOptions.Clean);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageFrame{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="backgroundColor">The color to clear the image with.</param>
    internal ImageFrame(Configuration configuration, int width, int height, TPixel backgroundColor)
        : this(configuration, width, height, backgroundColor, new ImageFrameMetadata()) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageFrame{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="backgroundColor">The color to clear the image with.</param>
    /// <param name="metadata">The metadata.</param>
    internal ImageFrame(Configuration configuration, int width, int height, TPixel backgroundColor,
        ImageFrameMetadata metadata)
        : base(configuration, width, height, metadata)
    {
        Guard.MustBeGreaterThan(width, 0, nameof(width));
        Guard.MustBeGreaterThan(height, 0, nameof(height));

        PixelBuffer = this.GetConfiguration().MemoryAllocator.Allocate2D<TPixel>(width, height);
        Clear(backgroundColor);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageFrame{TPixel}" /> class wrapping an existing buffer.
    /// </summary>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="memorySource">The memory source.</param>
    internal ImageFrame(Configuration configuration, int width, int height, MemoryGroup<TPixel> memorySource)
        : this(configuration, width, height, memorySource, new ImageFrameMetadata()) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageFrame{TPixel}" /> class wrapping an existing buffer.
    /// </summary>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="memorySource">The memory source.</param>
    /// <param name="metadata">The metadata.</param>
    internal ImageFrame(Configuration configuration, int width, int height, MemoryGroup<TPixel> memorySource,
        ImageFrameMetadata metadata)
        : base(configuration, width, height, metadata)
    {
        Guard.MustBeGreaterThan(width, 0, nameof(width));
        Guard.MustBeGreaterThan(height, 0, nameof(height));

        PixelBuffer = new Buffer2D<TPixel>(memorySource, width, height);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageFrame{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="source">The source.</param>
    internal ImageFrame(Configuration configuration, ImageFrame<TPixel> source)
        : base(configuration, source.Width, source.Height, source.Metadata.DeepClone())
    {
        Guard.NotNull(configuration, nameof(configuration));
        Guard.NotNull(source, nameof(source));

        PixelBuffer = this.GetConfiguration().MemoryAllocator
            .Allocate2D<TPixel>(source.PixelBuffer.Width, source.PixelBuffer.Height);
        source.PixelBuffer.FastMemoryGroup.CopyTo(PixelBuffer.FastMemoryGroup);
    }

    /// <summary>
    ///     Gets the image pixels. Not private as Buffer2D requires an array in its constructor.
    /// </summary>
    internal Buffer2D<TPixel> PixelBuffer { get; private set; }

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
            return PixelBuffer.GetElementUnsafe(x, y);
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        set
        {
            VerifyCoords(x, y);
            PixelBuffer.GetElementUnsafe(x, y) = value;
        }
    }

    /// <inheritdoc />
    Buffer2D<TPixel> IPixelSource<TPixel>.PixelBuffer => PixelBuffer;

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

        return PixelBuffer.GetRowSpan(rowIndex);
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
    ///     Gets a reference to the pixel at the specified position.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ref TPixel GetPixelReference(int x, int y)
    {
        return ref PixelBuffer[x, y];
    }

    /// <summary>
    ///     Copies the pixels to a <see cref="Buffer2D{TPixel}" /> of the same size.
    /// </summary>
    /// <param name="target">The target pixel buffer accessor.</param>
    internal void CopyTo(Buffer2D<TPixel> target)
    {
        if (Size() != target.Size())
            throw new ArgumentException("ImageFrame<TPixel>.CopyTo(): target must be of the same size!",
                nameof(target));

        PixelBuffer.FastMemoryGroup.CopyTo(target.FastMemoryGroup);
    }

    /// <summary>
    ///     Switches the buffers used by the image and the pixelSource meaning that the Image will "own" the buffer from the
    ///     pixelSource and the pixelSource will now own the Images buffer.
    /// </summary>
    /// <param name="pixelSource">The pixel source.</param>
    internal void SwapOrCopyPixelsBufferFrom(ImageFrame<TPixel> pixelSource)
    {
        Guard.NotNull(pixelSource, nameof(pixelSource));

        Buffer2D<TPixel>.SwapOrCopyContent(PixelBuffer, pixelSource.PixelBuffer);
        UpdateSize(PixelBuffer.Size());
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (isDisposed) return;

        if (disposing)
        {
            PixelBuffer?.Dispose();
            PixelBuffer = null;
        }

        isDisposed = true;
    }

    internal override void CopyPixelsTo<TDestinationPixel>(MemoryGroup<TDestinationPixel> destination)
    {
        if (typeof(TPixel) == typeof(TDestinationPixel))
        {
            PixelBuffer.FastMemoryGroup.TransformTo(destination, (s, d) =>
            {
                var d1 = MemoryMarshal.Cast<TDestinationPixel, TPixel>(d);
                s.CopyTo(d1);
            });
            return;
        }

        PixelBuffer.FastMemoryGroup.TransformTo(destination,
            (s, d) => { PixelOperations<TPixel>.Instance.To(this.GetConfiguration(), s, d); });
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"ImageFrame<{typeof(TPixel).Name}>({Width}x{Height})";
    }

    /// <summary>
    ///     Clones the current instance.
    /// </summary>
    /// <returns>The <see cref="ImageFrame{TPixel}" /></returns>
    internal ImageFrame<TPixel> Clone()
    {
        return Clone(this.GetConfiguration());
    }

    /// <summary>
    ///     Clones the current instance.
    /// </summary>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <returns>The <see cref="ImageFrame{TPixel}" /></returns>
    internal ImageFrame<TPixel> Clone(Configuration configuration)
    {
        return new ImageFrame<TPixel>(configuration, this);
    }

    /// <summary>
    ///     Returns a copy of the image frame in the given pixel format.
    /// </summary>
    /// <typeparam name="TPixel2">The pixel format.</typeparam>
    /// <returns>The <see cref="ImageFrame{TPixel2}" /></returns>
    internal ImageFrame<TPixel2> CloneAs<TPixel2>()
        where TPixel2 : unmanaged, IPixel<TPixel2>
    {
        return CloneAs<TPixel2>(this.GetConfiguration());
    }

    /// <summary>
    ///     Returns a copy of the image frame in the given pixel format.
    /// </summary>
    /// <typeparam name="TPixel2">The pixel format.</typeparam>
    /// <param name="configuration">The configuration providing initialization code which allows extending the library.</param>
    /// <returns>The <see cref="ImageFrame{TPixel2}" /></returns>
    internal ImageFrame<TPixel2> CloneAs<TPixel2>(Configuration configuration)
        where TPixel2 : unmanaged, IPixel<TPixel2>
    {
        if (typeof(TPixel2) == typeof(TPixel)) return Clone(configuration) as ImageFrame<TPixel2>;

        var target = new ImageFrame<TPixel2>(configuration, Width, Height, Metadata.DeepClone());
        var operation = new RowIntervalOperation<TPixel2>(this, target, configuration);

        ParallelRowIterator.IterateRowIntervals(
            configuration,
            Bounds(),
            in operation);

        return target;
    }

    /// <summary>
    ///     Clears the bitmap.
    /// </summary>
    /// <param name="value">The value to initialize the bitmap with.</param>
    internal void Clear(TPixel value)
    {
        var group = PixelBuffer.FastMemoryGroup;

        if (value.Equals(default))
            group.Clear();
        else
            group.Fill(value);
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

    /// <summary>
    ///     A <see langword="struct" /> implementing the clone logic for <see cref="ImageFrame{TPixel}" />.
    /// </summary>
    private readonly struct RowIntervalOperation<TPixel2> : IRowIntervalOperation
        where TPixel2 : unmanaged, IPixel<TPixel2>
    {
        private readonly ImageFrame<TPixel> source;
        private readonly ImageFrame<TPixel2> target;
        private readonly Configuration configuration;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowIntervalOperation(
            ImageFrame<TPixel> source,
            ImageFrame<TPixel2> target,
            Configuration configuration)
        {
            this.source = source;
            this.target = target;
            this.configuration = configuration;
        }

        /// <inheritdoc />
        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(in RowInterval rows)
        {
            for (var y = rows.Min; y < rows.Max; y++)
            {
                var sourceRow = source.GetPixelRowSpan(y);
                var targetRow = target.GetPixelRowSpan(y);
                PixelOperations<TPixel>.Instance.To(configuration, sourceRow, targetRow);
            }
        }
    }
}