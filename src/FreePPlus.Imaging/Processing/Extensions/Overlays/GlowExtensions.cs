// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Overlays;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines extensions that allow the application of a radial glow on an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class GlowExtensions
{
    /// <summary>
    ///     Applies a radial glow effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Glow(this IImageProcessingContext source)
    {
        return Glow(source, source.GetGraphicsOptions());
    }

    /// <summary>
    ///     Applies a radial glow effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="color">The color to set as the glow.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Glow(this IImageProcessingContext source, Color color)
    {
        return Glow(source, source.GetGraphicsOptions(), color);
    }

    /// <summary>
    ///     Applies a radial glow effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="radius">The the radius.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Glow(this IImageProcessingContext source, float radius)
    {
        return Glow(source, source.GetGraphicsOptions(), radius);
    }

    /// <summary>
    ///     Applies a radial glow effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Glow(this IImageProcessingContext source, Rectangle rectangle)
    {
        return source.Glow(source.GetGraphicsOptions(), rectangle);
    }

    /// <summary>
    ///     Applies a radial glow effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="color">The color to set as the glow.</param>
    /// <param name="radius">The the radius.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Glow(
        this IImageProcessingContext source,
        Color color,
        float radius,
        Rectangle rectangle)
    {
        return source.Glow(source.GetGraphicsOptions(), color, ValueSize.Absolute(radius), rectangle);
    }

    /// <summary>
    ///     Applies a radial glow effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="options">The options effecting things like blending.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Glow(this IImageProcessingContext source, GraphicsOptions options)
    {
        return source.Glow(options, Color.Black, ValueSize.PercentageOfWidth(0.5f));
    }

    /// <summary>
    ///     Applies a radial glow effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="options">The options effecting things like blending.</param>
    /// <param name="color">The color to set as the glow.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Glow(
        this IImageProcessingContext source,
        GraphicsOptions options,
        Color color)
    {
        return source.Glow(options, color, ValueSize.PercentageOfWidth(0.5f));
    }

    /// <summary>
    ///     Applies a radial glow effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="options">The options effecting things like blending.</param>
    /// <param name="radius">The the radius.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Glow(
        this IImageProcessingContext source,
        GraphicsOptions options,
        float radius)
    {
        return source.Glow(options, Color.Black, ValueSize.Absolute(radius));
    }

    /// <summary>
    ///     Applies a radial glow effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="options">The options effecting things like blending.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Glow(
        this IImageProcessingContext source,
        GraphicsOptions options,
        Rectangle rectangle)
    {
        return source.Glow(options, Color.Black, ValueSize.PercentageOfWidth(0.5f), rectangle);
    }

    /// <summary>
    ///     Applies a radial glow effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="options">The options effecting things like blending.</param>
    /// <param name="color">The color to set as the glow.</param>
    /// <param name="radius">The the radius.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Glow(
        this IImageProcessingContext source,
        GraphicsOptions options,
        Color color,
        float radius,
        Rectangle rectangle)
    {
        return source.Glow(options, color, ValueSize.Absolute(radius), rectangle);
    }

    /// <summary>
    ///     Applies a radial glow effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="options">The options effecting things like blending.</param>
    /// <param name="color">The color to set as the glow.</param>
    /// <param name="radius">The the radius.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    private static IImageProcessingContext Glow(
        this IImageProcessingContext source,
        GraphicsOptions options,
        Color color,
        ValueSize radius,
        Rectangle rectangle)
    {
        return source.ApplyProcessor(new GlowProcessor(options, color, radius), rectangle);
    }

    /// <summary>
    ///     Applies a radial glow effect to an image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="options">The options effecting things like blending.</param>
    /// <param name="color">The color to set as the glow.</param>
    /// <param name="radius">The the radius.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    private static IImageProcessingContext Glow(
        this IImageProcessingContext source,
        GraphicsOptions options,
        Color color,
        ValueSize radius)
    {
        return source.ApplyProcessor(new GlowProcessor(options, color, radius));
    }
}