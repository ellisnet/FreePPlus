// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Overlays;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines extensions that allow the application of a radial glow to an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class VignetteExtensions
{
    /// <summary>
    ///     Applies a radial vignette effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Vignette(this IImageProcessingContext source)
    {
        return Vignette(source, source.GetGraphicsOptions());
    }

    /// <summary>
    ///     Applies a radial vignette effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="color">The color to set as the vignette.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Vignette(this IImageProcessingContext source, Color color)
    {
        return Vignette(source, source.GetGraphicsOptions(), color);
    }

    /// <summary>
    ///     Applies a radial vignette effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="radiusX">The the x-radius.</param>
    /// <param name="radiusY">The the y-radius.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Vignette(
        this IImageProcessingContext source,
        float radiusX,
        float radiusY)
    {
        return Vignette(source, source.GetGraphicsOptions(), radiusX, radiusY);
    }

    /// <summary>
    ///     Applies a radial vignette effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Vignette(this IImageProcessingContext source, Rectangle rectangle)
    {
        return Vignette(source, source.GetGraphicsOptions(), rectangle);
    }

    /// <summary>
    ///     Applies a radial vignette effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="color">The color to set as the vignette.</param>
    /// <param name="radiusX">The the x-radius.</param>
    /// <param name="radiusY">The the y-radius.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Vignette(
        this IImageProcessingContext source,
        Color color,
        float radiusX,
        float radiusY,
        Rectangle rectangle)
    {
        return source.Vignette(source.GetGraphicsOptions(), color, radiusX, radiusY, rectangle);
    }

    /// <summary>
    ///     Applies a radial vignette effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="options">The options effecting pixel blending.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Vignette(this IImageProcessingContext source, GraphicsOptions options)
    {
        return source.VignetteInternal(
            options,
            Color.Black,
            ValueSize.PercentageOfWidth(.5f),
            ValueSize.PercentageOfHeight(.5f));
    }

    /// <summary>
    ///     Applies a radial vignette effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="options">The options effecting pixel blending.</param>
    /// <param name="color">The color to set as the vignette.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Vignette(
        this IImageProcessingContext source,
        GraphicsOptions options,
        Color color)
    {
        return source.VignetteInternal(
            options,
            color,
            ValueSize.PercentageOfWidth(.5f),
            ValueSize.PercentageOfHeight(.5f));
    }

    /// <summary>
    ///     Applies a radial vignette effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="options">The options effecting pixel blending.</param>
    /// <param name="radiusX">The the x-radius.</param>
    /// <param name="radiusY">The the y-radius.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Vignette(
        this IImageProcessingContext source,
        GraphicsOptions options,
        float radiusX,
        float radiusY)
    {
        return source.VignetteInternal(options, Color.Black, radiusX, radiusY);
    }

    /// <summary>
    ///     Applies a radial vignette effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="options">The options effecting pixel blending.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Vignette(
        this IImageProcessingContext source,
        GraphicsOptions options,
        Rectangle rectangle)
    {
        return source.VignetteInternal(
            options,
            Color.Black,
            ValueSize.PercentageOfWidth(.5f),
            ValueSize.PercentageOfHeight(.5f),
            rectangle);
    }

    /// <summary>
    ///     Applies a radial vignette effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="options">The options effecting pixel blending.</param>
    /// <param name="color">The color to set as the vignette.</param>
    /// <param name="radiusX">The the x-radius.</param>
    /// <param name="radiusY">The the y-radius.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Vignette(
        this IImageProcessingContext source,
        GraphicsOptions options,
        Color color,
        float radiusX,
        float radiusY,
        Rectangle rectangle)
    {
        return source.VignetteInternal(options, color, radiusX, radiusY, rectangle);
    }

    private static IImageProcessingContext VignetteInternal(
        this IImageProcessingContext source,
        GraphicsOptions options,
        Color color,
        ValueSize radiusX,
        ValueSize radiusY,
        Rectangle rectangle)
    {
        return source.ApplyProcessor(new VignetteProcessor(options, color, radiusX, radiusY), rectangle);
    }

    private static IImageProcessingContext VignetteInternal(
        this IImageProcessingContext source,
        GraphicsOptions options,
        Color color,
        ValueSize radiusX,
        ValueSize radiusY)
    {
        return source.ApplyProcessor(new VignetteProcessor(options, color, radiusX, radiusY));
    }
}