// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing.Processors;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Performs processor application operations on the source image
/// </summary>
/// <typeparam name="TPixel">The pixel format</typeparam>
internal class DefaultImageProcessorContext<TPixel> : IInternalImageProcessingContext<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly bool mutate;
    private readonly Image<TPixel> source;
    private Image<TPixel> destination;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultImageProcessorContext{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="source">The source image.</param>
    /// <param name="mutate">Whether to mutate the image.</param>
    public DefaultImageProcessorContext(Configuration configuration, Image<TPixel> source, bool mutate)
    {
        Configuration = configuration;
        this.mutate = mutate;
        this.source = source;

        // Mutate acts upon the source image only.
        if (this.mutate) destination = source;
    }

    /// <inheritdoc />
    public Configuration Configuration { get; }

    /// <inheritdoc />
    public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

    /// <inheritdoc />
    public Image<TPixel> GetResultImage()
    {
        if (!mutate && destination is null)
            // Ensure we have cloned the source if we are not mutating as we might have failed
            // to register any processors.
            destination = source.Clone();

        return destination;
    }

    /// <inheritdoc />
    public Size GetCurrentSize()
    {
        return GetCurrentBounds().Size;
    }

    /// <inheritdoc />
    public IImageProcessingContext ApplyProcessor(IImageProcessor processor)
    {
        return ApplyProcessor(processor, GetCurrentBounds());
    }

    /// <inheritdoc />
    public IImageProcessingContext ApplyProcessor(IImageProcessor processor, Rectangle rectangle)
    {
        if (!mutate && destination is null)
        {
            // When cloning an image we can optimize the processing pipeline by avoiding an unnecessary
            // interim clone if the first processor in the pipeline is a cloning processor.
            if (processor is ICloningImageProcessor cloningImageProcessor)
                using (var pixelProcessor =
                       cloningImageProcessor.CreatePixelSpecificCloningProcessor(Configuration, source, rectangle))
                {
                    destination = pixelProcessor.CloneAndExecute();
                    return this;
                }

            // Not a cloning processor? We need to create a clone to operate on.
            destination = source.Clone();
        }

        // Standard processing pipeline.
        using (var specificProcessor = processor.CreatePixelSpecificProcessor(Configuration, destination, rectangle))
        {
            specificProcessor.Execute();
        }

        return this;
    }

    private Rectangle GetCurrentBounds()
    {
        return destination?.Bounds() ?? source.Bounds();
    }
}