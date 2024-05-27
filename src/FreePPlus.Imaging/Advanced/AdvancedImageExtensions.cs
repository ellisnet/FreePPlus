// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Advanced;

//was previously: namespace SixLabors.ImageSharp.Advanced;

/// <summary>
///     Extension methods over Image{TPixel}
/// </summary>
public static class AdvancedImageExtensions
{
    /// <summary>
    ///     Accepts a <see cref="IImageVisitor" /> to implement a double-dispatch pattern in order to
    ///     apply pixel-specific operations on non-generic <see cref="Image" /> instances
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="visitor">The visitor.</param>
    public static void AcceptVisitor(this Image source, IImageVisitor visitor)
    {
        source.Accept(visitor);
    }

    /// <summary>
    ///     Gets the configuration for the image.
    /// </summary>
    /// <param name="source">The source image.</param>
    /// <returns>Returns the configuration.</returns>
    public static Configuration GetConfiguration(this Image source)
    {
        return GetConfiguration((IConfigurationProvider)source);
    }

    /// <summary>
    ///     Gets the configuration for the image frame.
    /// </summary>
    /// <param name="source">The source image.</param>
    /// <returns>Returns the configuration.</returns>
    public static Configuration GetConfiguration(this ImageFrame source)
    {
        return GetConfiguration((IConfigurationProvider)source);
    }

    /// <summary>
    ///     Gets the configuration.
    /// </summary>
    /// <param name="source">The source image</param>
    /// <returns>Returns the bounds of the image</returns>
    private static Configuration GetConfiguration(IConfigurationProvider source)
    {
        return source?.Configuration ?? Configuration.Default;
    }

    /// <summary>
    ///     Gets the representation of the pixels as a <see cref="IMemoryGroup{T}" /> containing the backing pixel data of the
    ///     image
    ///     stored in row major order, as a list of contiguous <see cref="Memory{T}" /> blocks in the source image's pixel
    ///     format.
    /// </summary>
    /// <param name="source">The source image.</param>
    /// <typeparam name="TPixel">The type of the pixel.</typeparam>
    /// <returns>The <see cref="IMemoryGroup{T}" />.</returns>
    /// <remarks>
    ///     Certain Image Processors may invalidate the returned <see cref="IMemoryGroup{T}" /> and all it's buffers,
    ///     therefore it's not recommended to mutate the image while holding a reference to it's <see cref="IMemoryGroup{T}" />
    ///     .
    /// </remarks>
    public static IMemoryGroup<TPixel> GetPixelMemoryGroup<TPixel>(this ImageFrame<TPixel> source)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return source?.PixelBuffer.FastMemoryGroup.View ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary>
    ///     Gets the representation of the pixels as a <see cref="IMemoryGroup{T}" /> containing the backing pixel data of the
    ///     image
    ///     stored in row major order, as a list of contiguous <see cref="Memory{T}" /> blocks in the source image's pixel
    ///     format.
    /// </summary>
    /// <param name="source">The source image.</param>
    /// <typeparam name="TPixel">The type of the pixel.</typeparam>
    /// <returns>The <see cref="IMemoryGroup{T}" />.</returns>
    /// <remarks>
    ///     Certain Image Processors may invalidate the returned <see cref="IMemoryGroup{T}" /> and all it's buffers,
    ///     therefore it's not recommended to mutate the image while holding a reference to it's <see cref="IMemoryGroup{T}" />
    ///     .
    /// </remarks>
    public static IMemoryGroup<TPixel> GetPixelMemoryGroup<TPixel>(this Image<TPixel> source)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return source?.Frames.RootFrame.GetPixelMemoryGroup() ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary>
    ///     Gets the representation of the pixels as a <see cref="Span{T}" /> of contiguous memory
    ///     at row <paramref name="rowIndex" /> beginning from the the first pixel on that row.
    /// </summary>
    /// <typeparam name="TPixel">The type of the pixel.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="rowIndex">The row.</param>
    /// <returns>The <see cref="Span{TPixel}" /></returns>
    public static Memory<TPixel> GetPixelRowMemory<TPixel>(this ImageFrame<TPixel> source, int rowIndex)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(source, nameof(source));
        Guard.MustBeGreaterThanOrEqualTo(rowIndex, 0, nameof(rowIndex));
        Guard.MustBeLessThan(rowIndex, source.Height, nameof(rowIndex));

        return source.PixelBuffer.GetSafeRowMemory(rowIndex);
    }

    /// <summary>
    ///     Gets the representation of the pixels as <see cref="Span{T}" /> of of contiguous memory
    ///     at row <paramref name="rowIndex" /> beginning from the the first pixel on that row.
    /// </summary>
    /// <typeparam name="TPixel">The type of the pixel.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="rowIndex">The row.</param>
    /// <returns>The <see cref="Span{TPixel}" /></returns>
    public static Memory<TPixel> GetPixelRowMemory<TPixel>(this Image<TPixel> source, int rowIndex)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(source, nameof(source));
        Guard.MustBeGreaterThanOrEqualTo(rowIndex, 0, nameof(rowIndex));
        Guard.MustBeLessThan(rowIndex, source.Height, nameof(rowIndex));

        return source.Frames.RootFrame.PixelBuffer.GetSafeRowMemory(rowIndex);
    }

    /// <summary>
    ///     Gets the <see cref="MemoryAllocator" /> assigned to 'source'.
    /// </summary>
    /// <param name="source">The source image.</param>
    /// <returns>Returns the configuration.</returns>
    internal static MemoryAllocator GetMemoryAllocator(this IConfigurationProvider source)
    {
        return GetConfiguration(source).MemoryAllocator;
    }
}