// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using FreePPlus.Imaging.Formats;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.Metadata;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <content>
///     Adds static methods allowing wrapping an existing memory area as an image.
/// </content>
public abstract partial class Image
{
    /// <summary>
    ///     Wraps an existing contiguous memory area of 'width' x 'height' pixels,
    ///     allowing to view/manipulate it as an ImageSharp <see cref="Image{TPixel}" /> instance.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type</typeparam>
    /// <param name="configuration">The <see cref="Configuration" /></param>
    /// <param name="pixelMemory">The pixel memory.</param>
    /// <param name="width">The width of the memory image.</param>
    /// <param name="height">The height of the memory image.</param>
    /// <param name="metadata">The <see cref="ImageMetadata" />.</param>
    /// <exception cref="ArgumentNullException">The configuration is null.</exception>
    /// <exception cref="ArgumentNullException">The metadata is null.</exception>
    /// <returns>An <see cref="Image{TPixel}" /> instance</returns>
    public static Image<TPixel> WrapMemory<TPixel>(
        Configuration configuration,
        Memory<TPixel> pixelMemory,
        int width,
        int height,
        ImageMetadata metadata,
        IImageFormat format)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(configuration, nameof(configuration));
        Guard.NotNull(metadata, nameof(metadata));

        var memorySource = MemoryGroup<TPixel>.Wrap(pixelMemory);
        return new Image<TPixel>(configuration, memorySource, width, height, metadata, format);
    }

    /// <summary>
    ///     Wraps an existing contiguous memory area of 'width' x 'height' pixels,
    ///     allowing to view/manipulate it as an ImageSharp <see cref="Image{TPixel}" /> instance.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type</typeparam>
    /// <param name="configuration">The <see cref="Configuration" /></param>
    /// <param name="pixelMemory">The pixel memory.</param>
    /// <param name="width">The width of the memory image.</param>
    /// <param name="height">The height of the memory image.</param>
    /// <exception cref="ArgumentNullException">The configuration is null.</exception>
    /// <returns>An <see cref="Image{TPixel}" /> instance.</returns>
    public static Image<TPixel> WrapMemory<TPixel>(
        Configuration configuration,
        Memory<TPixel> pixelMemory,
        int width,
        int height,
        IImageFormat format)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return WrapMemory(configuration, pixelMemory, width, height, new ImageMetadata(), format);
    }

    /// <summary>
    ///     Wraps an existing contiguous memory area of 'width' x 'height' pixels,
    ///     allowing to view/manipulate it as an ImageSharp <see cref="Image{TPixel}" /> instance.
    ///     The memory is being observed, the caller remains responsible for managing it's lifecycle.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type.</typeparam>
    /// <param name="pixelMemory">The pixel memory.</param>
    /// <param name="width">The width of the memory image.</param>
    /// <param name="height">The height of the memory image.</param>
    /// <returns>An <see cref="Image{TPixel}" /> instance.</returns>
    public static Image<TPixel> WrapMemory<TPixel>(
        Memory<TPixel> pixelMemory,
        int width,
        int height,
        IImageFormat format)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return WrapMemory(Configuration.Default, pixelMemory, width, height, format);
    }

    /// <summary>
    ///     Wraps an existing contiguous memory area of 'width' x 'height' pixels,
    ///     allowing to view/manipulate it as an ImageSharp <see cref="Image{TPixel}" /> instance.
    ///     The ownership of the <paramref name="pixelMemoryOwner" /> is being transferred to the new
    ///     <see cref="Image{TPixel}" /> instance,
    ///     meaning that the caller is not allowed to dispose <paramref name="pixelMemoryOwner" />.
    ///     It will be disposed together with the result image.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type</typeparam>
    /// <param name="configuration">The <see cref="Configuration" /></param>
    /// <param name="pixelMemoryOwner">The <see cref="IMemoryOwner{T}" /> that is being transferred to the image</param>
    /// <param name="width">The width of the memory image.</param>
    /// <param name="height">The height of the memory image.</param>
    /// <param name="metadata">The <see cref="ImageMetadata" /></param>
    /// <exception cref="ArgumentNullException">The configuration is null.</exception>
    /// <exception cref="ArgumentNullException">The metadata is null.</exception>
    /// <returns>An <see cref="Image{TPixel}" /> instance</returns>
    public static Image<TPixel> WrapMemory<TPixel>(
        Configuration configuration,
        IMemoryOwner<TPixel> pixelMemoryOwner,
        int width,
        int height,
        ImageMetadata metadata,
        IImageFormat format)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Guard.NotNull(configuration, nameof(configuration));
        Guard.NotNull(metadata, nameof(metadata));

        var memorySource = MemoryGroup<TPixel>.Wrap(pixelMemoryOwner);
        return new Image<TPixel>(configuration, memorySource, width, height, metadata, format);
    }

    /// <summary>
    ///     Wraps an existing contiguous memory area of 'width' x 'height' pixels,
    ///     allowing to view/manipulate it as an ImageSharp <see cref="Image{TPixel}" /> instance.
    ///     The ownership of the <paramref name="pixelMemoryOwner" /> is being transferred to the new
    ///     <see cref="Image{TPixel}" /> instance,
    ///     meaning that the caller is not allowed to dispose <paramref name="pixelMemoryOwner" />.
    ///     It will be disposed together with the result image.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type.</typeparam>
    /// <param name="configuration">The <see cref="Configuration" /></param>
    /// <param name="pixelMemoryOwner">The <see cref="IMemoryOwner{T}" /> that is being transferred to the image.</param>
    /// <param name="width">The width of the memory image.</param>
    /// <param name="height">The height of the memory image.</param>
    /// <exception cref="ArgumentNullException">The configuration is null.</exception>
    /// <returns>An <see cref="Image{TPixel}" /> instance</returns>
    public static Image<TPixel> WrapMemory<TPixel>(
        Configuration configuration,
        IMemoryOwner<TPixel> pixelMemoryOwner,
        int width,
        int height,
        IImageFormat format)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return WrapMemory(configuration, pixelMemoryOwner, width, height, new ImageMetadata(), format);
    }

    /// <summary>
    ///     Wraps an existing contiguous memory area of 'width' x 'height' pixels,
    ///     allowing to view/manipulate it as an ImageSharp <see cref="Image{TPixel}" /> instance.
    ///     The ownership of the <paramref name="pixelMemoryOwner" /> is being transferred to the new
    ///     <see cref="Image{TPixel}" /> instance,
    ///     meaning that the caller is not allowed to dispose <paramref name="pixelMemoryOwner" />.
    ///     It will be disposed together with the result image.
    /// </summary>
    /// <typeparam name="TPixel">The pixel type</typeparam>
    /// <param name="pixelMemoryOwner">The <see cref="IMemoryOwner{T}" /> that is being transferred to the image.</param>
    /// <param name="width">The width of the memory image.</param>
    /// <param name="height">The height of the memory image.</param>
    /// <returns>An <see cref="Image{TPixel}" /> instance.</returns>
    public static Image<TPixel> WrapMemory<TPixel>(
        IMemoryOwner<TPixel> pixelMemoryOwner,
        int width,
        int height,
        IImageFormat format)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return WrapMemory(Configuration.Default, pixelMemoryOwner, width, height, format);
    }
}