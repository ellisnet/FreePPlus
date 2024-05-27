// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using FreePPlus.Imaging.Processing.Processors.Dithering;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines dithering extensions to apply on an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class DitherExtensions
{
    /// <summary>
    ///     Dithers the image reducing it to a web-safe palette using <see cref="KnownDitherings.Bayer8x8" />.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Dither(this IImageProcessingContext source)
    {
        return Dither(source, KnownDitherings.Bayer8x8);
    }

    /// <summary>
    ///     Dithers the image reducing it to a web-safe palette using ordered dithering.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="dither">The ordered ditherer.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Dither(
        this IImageProcessingContext source,
        IDither dither)
    {
        return source.ApplyProcessor(new PaletteDitherProcessor(dither));
    }

    /// <summary>
    ///     Dithers the image reducing it to a web-safe palette using ordered dithering.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="dither">The ordered ditherer.</param>
    /// <param name="ditherScale">The dithering scale used to adjust the amount of dither.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Dither(
        this IImageProcessingContext source,
        IDither dither,
        float ditherScale)
    {
        return source.ApplyProcessor(new PaletteDitherProcessor(dither, ditherScale));
    }

    /// <summary>
    ///     Dithers the image reducing it to the given palette using ordered dithering.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="dither">The ordered ditherer.</param>
    /// <param name="palette">The palette to select substitute colors from.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Dither(
        this IImageProcessingContext source,
        IDither dither,
        ReadOnlyMemory<Color> palette)
    {
        return source.ApplyProcessor(new PaletteDitherProcessor(dither, palette));
    }

    /// <summary>
    ///     Dithers the image reducing it to the given palette using ordered dithering.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="dither">The ordered ditherer.</param>
    /// <param name="ditherScale">The dithering scale used to adjust the amount of dither.</param>
    /// <param name="palette">The palette to select substitute colors from.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Dither(
        this IImageProcessingContext source,
        IDither dither,
        float ditherScale,
        ReadOnlyMemory<Color> palette)
    {
        return source.ApplyProcessor(new PaletteDitherProcessor(dither, ditherScale, palette));
    }

    /// <summary>
    ///     Dithers the image reducing it to a web-safe palette using <see cref="KnownDitherings.Bayer8x8" />.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Dither(this IImageProcessingContext source, Rectangle rectangle)
    {
        return Dither(source, KnownDitherings.Bayer8x8, rectangle);
    }

    /// <summary>
    ///     Dithers the image reducing it to a web-safe palette using ordered dithering.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="dither">The ordered ditherer.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Dither(
        this IImageProcessingContext source,
        IDither dither,
        Rectangle rectangle)
    {
        return source.ApplyProcessor(new PaletteDitherProcessor(dither), rectangle);
    }

    /// <summary>
    ///     Dithers the image reducing it to a web-safe palette using ordered dithering.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="dither">The ordered ditherer.</param>
    /// <param name="ditherScale">The dithering scale used to adjust the amount of dither.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Dither(
        this IImageProcessingContext source,
        IDither dither,
        float ditherScale,
        Rectangle rectangle)
    {
        return source.ApplyProcessor(new PaletteDitherProcessor(dither, ditherScale), rectangle);
    }

    /// <summary>
    ///     Dithers the image reducing it to the given palette using ordered dithering.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="dither">The ordered ditherer.</param>
    /// <param name="palette">The palette to select substitute colors from.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Dither(
        this IImageProcessingContext source,
        IDither dither,
        ReadOnlyMemory<Color> palette,
        Rectangle rectangle)
    {
        return source.ApplyProcessor(new PaletteDitherProcessor(dither, palette), rectangle);
    }

    /// <summary>
    ///     Dithers the image reducing it to the given palette using ordered dithering.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="dither">The ordered ditherer.</param>
    /// <param name="ditherScale">The dithering scale used to adjust the amount of dither.</param>
    /// <param name="palette">The palette to select substitute colors from.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Dither(
        this IImageProcessingContext source,
        IDither dither,
        float ditherScale,
        ReadOnlyMemory<Color> palette,
        Rectangle rectangle)
    {
        return source.ApplyProcessor(new PaletteDitherProcessor(dither, ditherScale, palette), rectangle);
    }
}