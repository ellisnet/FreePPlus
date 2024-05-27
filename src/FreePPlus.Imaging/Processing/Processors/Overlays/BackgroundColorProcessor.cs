// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Overlays;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Overlays;

/// <summary>
///     Defines a processing operation to replace the background color of an <see cref="Image" />.
/// </summary>
public sealed class BackgroundColorProcessor : IImageProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BackgroundColorProcessor" /> class.
    /// </summary>
    /// <param name="options">The options defining blending algorithm and amount.</param>
    /// <param name="color">The <see cref="Color" /> to set the background color to.</param>
    public BackgroundColorProcessor(GraphicsOptions options, Color color)
    {
        Color = color;
        GraphicsOptions = options;
    }

    /// <summary>
    ///     Gets the Graphics options to alter how processor is applied.
    /// </summary>
    public GraphicsOptions GraphicsOptions { get; }

    /// <summary>
    ///     Gets the background color value.
    /// </summary>
    public Color Color { get; }

    /// <inheritdoc />
    public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration,
        Image<TPixel> source, Rectangle sourceRectangle)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        return new BackgroundColorProcessor<TPixel>(configuration, this, source, sourceRectangle);
    }
}