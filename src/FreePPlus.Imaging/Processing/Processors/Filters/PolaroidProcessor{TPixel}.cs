// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing.Processors.Overlays;

namespace FreePPlus.Imaging.Processing.Processors.Filters;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Filters;

/// <summary>
///     Converts the colors of the image recreating an old Polaroid effect.
/// </summary>
internal class PolaroidProcessor<TPixel> : FilterProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private static readonly Color LightOrange = Color.FromRgba(255, 153, 102, 128);
    private static readonly Color VeryDarkOrange = Color.FromRgb(102, 34, 0);
    private readonly PolaroidProcessor definition;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PolaroidProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="PolaroidProcessor" /> defining the parameters.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public PolaroidProcessor(Configuration configuration, PolaroidProcessor definition, Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, definition, source, sourceRectangle)
    {
        this.definition = definition;
    }

    /// <inheritdoc />
    protected override void AfterImageApply()
    {
        new VignetteProcessor(definition.GraphicsOptions, VeryDarkOrange).Execute(Configuration, Source,
            SourceRectangle);
        new GlowProcessor(definition.GraphicsOptions, LightOrange, Source.Width / 4F).Execute(Configuration, Source,
            SourceRectangle);
        base.AfterImageApply();
    }
}