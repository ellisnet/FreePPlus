// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Processing.Processors.Filters;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Filters;

/// <summary>
///     Converts the colors of the image recreating an old Lomograph effect.
/// </summary>
public sealed class LomographProcessor : FilterProcessor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LomographProcessor" /> class.
    /// </summary>
    /// <param name="graphicsOptions">Graphics options to use within the processor.</param>
    public LomographProcessor(GraphicsOptions graphicsOptions)
        : base(KnownFilterMatrices.LomographFilter)
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
        return new LomographProcessor<TPixel>(configuration, this, source, sourceRectangle);
    }
}