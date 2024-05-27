// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing.Processors.Binarization;
using FreePPlus.Imaging.Processing.Processors.Convolution;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     Provides methods to allow the cropping of an image to preserve areas of highest entropy.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class EntropyCropProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly EntropyCropProcessor definition;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EntropyCropProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="definition">The <see cref="EntropyCropProcessor" />.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public EntropyCropProcessor(Configuration configuration, EntropyCropProcessor definition, Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        this.definition = definition;
    }

    /// <inheritdoc />
    protected override void BeforeImageApply()
    {
        Rectangle rectangle;

        // TODO: This is clunky. We should add behavior enum to ExtractFrame.
        // All frames have be the same size so we only need to calculate the correct dimensions for the first frame
        using (var temp = new Image<TPixel>(Configuration, Source.Metadata.DeepClone(),
                   new[] { Source.Frames.RootFrame.Clone() }, Source.Format))
        {
            var configuration = Source.GetConfiguration();

            // Detect the edges.
            new SobelProcessor(false).Execute(Configuration, temp, SourceRectangle);

            // Apply threshold binarization filter.
            new BinaryThresholdProcessor(definition.Threshold).Execute(Configuration, temp, SourceRectangle);

            // Search for the first white pixels
            rectangle = ImageMaths.GetFilteredBoundingRectangle(temp.Frames.RootFrame, 0);
        }

        new CropProcessor(rectangle, Source.Size()).Execute(Configuration, Source, SourceRectangle);

        base.BeforeImageApply();
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        // All processing happens at the image level within BeforeImageApply();
    }
}