// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors;

internal static class ImageProcessorExtensions
{
    /// <summary>
    ///     Executes the processor against the given source image and rectangle bounds.
    /// </summary>
    /// <param name="processor">The processor.</param>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="source">The source image.</param>
    /// <param name="sourceRectangle">The source bounds.</param>
    public static void Execute(this IImageProcessor processor, Configuration configuration, Image source,
        Rectangle sourceRectangle)
    {
        source.AcceptVisitor(new ExecuteVisitor(configuration, processor, sourceRectangle));
    }

    private class ExecuteVisitor : IImageVisitor
    {
        private readonly Configuration configuration;
        private readonly IImageProcessor processor;
        private readonly Rectangle sourceRectangle;

        public ExecuteVisitor(Configuration configuration, IImageProcessor processor, Rectangle sourceRectangle)
        {
            this.configuration = configuration;
            this.processor = processor;
            this.sourceRectangle = sourceRectangle;
        }

        public void Visit<TPixel>(Image<TPixel> image)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using (var processorImpl = processor.CreatePixelSpecificProcessor(configuration, image, sourceRectangle))
            {
                processorImpl.Execute();
            }
        }
    }
}