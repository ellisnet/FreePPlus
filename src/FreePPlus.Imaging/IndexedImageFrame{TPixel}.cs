// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing.Processors.Quantization;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     A pixel-specific image frame where each pixel buffer value represents an index in a color palette.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
public sealed class IndexedImageFrame<TPixel> : IPixelSource, IDisposable
    where TPixel : unmanaged, IPixel<TPixel>
{
    private bool isDisposed;
    private IMemoryOwner<TPixel> paletteOwner;
    private Buffer2D<byte> pixelBuffer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IndexedImageFrame{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">
    ///     The configuration which allows altering default behaviour or extending the library.
    /// </param>
    /// <param name="width">The frame width.</param>
    /// <param name="height">The frame height.</param>
    /// <param name="palette">The color palette.</param>
    internal IndexedImageFrame(Configuration configuration, int width, int height, ReadOnlyMemory<TPixel> palette)
    {
        Guard.NotNull(configuration, nameof(configuration));
        Guard.MustBeLessThanOrEqualTo(palette.Length, QuantizerConstants.MaxColors, nameof(palette));
        Guard.MustBeGreaterThan(width, 0, nameof(width));
        Guard.MustBeGreaterThan(height, 0, nameof(height));

        Configuration = configuration;
        Width = width;
        Height = height;
        pixelBuffer = configuration.MemoryAllocator.Allocate2D<byte>(width, height);

        // Copy the palette over. We want the lifetime of this frame to be independant of any palette source.
        paletteOwner = configuration.MemoryAllocator.Allocate<TPixel>(palette.Length);
        palette.Span.CopyTo(paletteOwner.GetSpan());
        Palette = paletteOwner.Memory.Slice(0, palette.Length);
    }

    /// <summary>
    ///     Gets the configuration which allows altering default behaviour or extending the library.
    /// </summary>
    public Configuration Configuration { get; }

    /// <summary>
    ///     Gets the width of this <see cref="IndexedImageFrame{TPixel}" />.
    /// </summary>
    public int Width { get; }

    /// <summary>
    ///     Gets the height of this <see cref="IndexedImageFrame{TPixel}" />.
    /// </summary>
    public int Height { get; }

    /// <summary>
    ///     Gets the color palette of this <see cref="IndexedImageFrame{TPixel}" />.
    /// </summary>
    public ReadOnlyMemory<TPixel> Palette { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!isDisposed)
        {
            isDisposed = true;
            pixelBuffer.Dispose();
            paletteOwner.Dispose();
            pixelBuffer = null;
            paletteOwner = null;
        }
    }

    /// <inheritdoc />
    Buffer2D<byte> IPixelSource.PixelBuffer => pixelBuffer;

    /// <summary>
    ///     Gets the representation of the pixels as a <see cref="ReadOnlySpan{T}" /> of contiguous memory
    ///     at row <paramref name="rowIndex" /> beginning from the first pixel on that row.
    /// </summary>
    /// <param name="rowIndex">The row index in the pixel buffer.</param>
    /// <returns>The pixel row as a <see cref="ReadOnlySpan{T}" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public ReadOnlySpan<byte> GetPixelRowSpan(int rowIndex)
    {
        return GetWritablePixelRowSpanUnsafe(rowIndex);
    }

    /// <summary>
    ///     <para>
    ///         Gets the representation of the pixels as a <see cref="Span{T}" /> of contiguous memory
    ///         at row <paramref name="rowIndex" /> beginning from the first pixel on that row.
    ///     </para>
    ///     <para>
    ///         Note: Values written to this span are not sanitized against the palette length.
    ///         Care should be taken during assignment to prevent out-of-bounds errors.
    ///     </para>
    /// </summary>
    /// <param name="rowIndex">The row index in the pixel buffer.</param>
    /// <returns>The pixel row as a <see cref="Span{T}" />.</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Span<byte> GetWritablePixelRowSpanUnsafe(int rowIndex)
    {
        return pixelBuffer.GetRowSpan(rowIndex);
    }
}