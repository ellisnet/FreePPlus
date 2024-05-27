// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing.Processors.Overlays;

namespace FreePPlus.Imaging.Processing.Processors.Filters;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Filters;

/// <summary>
///     Converts the colors of the image recreating an old Lomograph effect.
/// </summary>
internal class LomographProcessor<TPixel> : FilterProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private static readonly Color VeryDarkGreen = Color.FromRgba(0, 10, 0, 255);
    private readonly LomographProcessor definition;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LomographProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="LomographProcessor" /> defining the parameters.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public LomographProcessor(Configuration configuration, LomographProcessor definition, Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, definition, source, sourceRectangle)
    {
        this.definition = definition;
    }

    /// <inheritdoc />
    protected override void AfterImageApply()
    {
        new VignetteProcessor(definition.GraphicsOptions, VeryDarkGreen).Execute(Configuration, Source,
            SourceRectangle);
        base.AfterImageApply();
    }
}