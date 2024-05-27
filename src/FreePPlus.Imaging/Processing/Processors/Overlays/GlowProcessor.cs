// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Overlays;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Overlays;

/// <summary>
///     Defines a radial glow effect applicable to an <see cref="Image" />.
/// </summary>
public sealed class GlowProcessor : IImageProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GlowProcessor" /> class.
    /// </summary>
    /// <param name="options">The options effecting blending and composition.</param>
    /// <param name="color">The color or the glow.</param>
    public GlowProcessor(GraphicsOptions options, Color color)
        : this(options, color, 0) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GlowProcessor" /> class.
    /// </summary>
    /// <param name="options">The options effecting blending and composition.</param>
    /// <param name="color">The color or the glow.</param>
    /// <param name="radius">The radius of the glow.</param>
    internal GlowProcessor(GraphicsOptions options, Color color, ValueSize radius)
    {
        GlowColor = color;
        Radius = radius;
        GraphicsOptions = options;
    }

    /// <summary>
    ///     Gets the options effecting blending and composition.
    /// </summary>
    public GraphicsOptions GraphicsOptions { get; }

    /// <summary>
    ///     Gets the glow color to apply.
    /// </summary>
    public Color GlowColor { get; }

    /// <summary>
    ///     Gets the the radius.
    /// </summary>
    internal ValueSize Radius { get; }

    /// <inheritdoc />
    public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration,
        Image<TPixel> source, Rectangle sourceRectangle)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return new GlowProcessor<TPixel>(configuration, this, source, sourceRectangle);
    }
}