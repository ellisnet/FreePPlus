// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing.Processors.Filters;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Filters;

/// <summary>
///     Converts the colors of the image recreating an old Polaroid effect.
/// </summary>
public sealed class PolaroidProcessor : FilterProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PolaroidProcessor" /> class.
    /// </summary>
    /// <param name="graphicsOptions">Graphics options to use within the processor.</param>
    public PolaroidProcessor(GraphicsOptions graphicsOptions)
        : base(KnownFilterMatrices.PolaroidFilter)
    {
        GraphicsOptions = graphicsOptions;
    }

    /// <summary>
    ///     Gets the options effecting blending and composition
    /// </summary>
    public GraphicsOptions GraphicsOptions { get; }

    /// <inheritdoc />
    public override IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration,
        Image<TPixel> source, Rectangle sourceRectangle)
    {
        return new PolaroidProcessor<TPixel>(configuration, this, source, sourceRectangle);
    }
}