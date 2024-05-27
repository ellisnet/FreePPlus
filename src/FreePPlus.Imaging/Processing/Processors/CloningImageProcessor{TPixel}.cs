// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors;

/// <summary>
///     The base class for all pixel specific cloning image processors.
///     Allows the application of processing algorithms to the image.
///     The image is cloned before operating upon and the buffers swapped upon completion.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
public abstract class CloningImageProcessor<TPixel> : ICloningImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CloningImageProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    protected CloningImageProcessor(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
    {
        Configuration = configuration;
        Source = source;
        SourceRectangle = sourceRectangle;
    }

    /// <summary>
    ///     Gets The source <see cref="Image{TPixel}" /> for the current processor instance.
    /// </summary>
    protected Image<TPixel> Source { get; }

    /// <summary>
    ///     Gets The source area to process for the current processor instance.
    /// </summary>
    protected Rectangle SourceRectangle { get; }

    /// <summary>
    ///     Gets the <see cref="Configuration" /> instance to use when performing operations.
    /// </summary>
    protected Configuration Configuration { get; }

    /// <inheritdoc />
    Image<TPixel> ICloningImageProcessor<TPixel>.CloneAndExecute()
    {
        var clone = CreateTarget();
        CheckFrameCount(Source, clone);

        var configuration = Configuration;
        BeforeImageApply(clone);

        for (var i = 0; i < Source.Frames.Count; i++)
        {
            var sourceFrame = Source.Frames[i];
            var clonedFrame = clone.Frames[i];

            BeforeFrameApply(sourceFrame, clonedFrame);
            OnFrameApply(sourceFrame, clonedFrame);
            AfterFrameApply(sourceFrame, clonedFrame);
        }

        AfterImageApply(clone);

        return clone;
    }

    /// <inheritdoc />
    void IImageProcessor<TPixel>.Execute()
    {
        // Create an interim clone of the source image to operate on.
        // Doing this allows for the application of transforms that will alter
        // the dimensions of the image.
        Image<TPixel> clone = default;
        try
        {
            clone = ((ICloningImageProcessor<TPixel>)this).CloneAndExecute();

            // We now need to move the pixel data/size data from the clone to the source.
            CheckFrameCount(Source, clone);
            Source.SwapOrCopyPixelsBuffersFrom(clone);
        }
        finally
        {
            // Dispose of the clone now that we have swapped the pixel/size data.
            clone?.Dispose();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Gets the size of the destination image.
    /// </summary>
    /// <returns>The <see cref="Size" />.</returns>
    protected abstract Size GetDestinationSize();

    /// <summary>
    ///     This method is called before the process is applied to prepare the processor.
    /// </summary>
    /// <param name="destination">The cloned/destination image. Cannot be null.</param>
    protected virtual void BeforeImageApply(Image<TPixel> destination) { }

    /// <summary>
    ///     This method is called before the process is applied to prepare the processor.
    /// </summary>
    /// <param name="source">The source image. Cannot be null.</param>
    /// <param name="destination">The cloned/destination image. Cannot be null.</param>
    protected virtual void BeforeFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination) { }

    /// <summary>
    ///     Applies the process to the specified portion of the specified <see cref="ImageFrame{TPixel}" /> at the specified
    ///     location
    ///     and with the specified size.
    /// </summary>
    /// <param name="source">The source image. Cannot be null.</param>
    /// <param name="destination">The cloned/destination image. Cannot be null.</param>
    protected abstract void OnFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination);

    /// <summary>
    ///     This method is called after the process is applied to prepare the processor.
    /// </summary>
    /// <param name="source">The source image. Cannot be null.</param>
    /// <param name="destination">The cloned/destination image. Cannot be null.</param>
    protected virtual void AfterFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination) { }

    /// <summary>
    ///     This method is called after the process is applied to prepare the processor.
    /// </summary>
    /// <param name="destination">The cloned/destination image. Cannot be null.</param>
    protected virtual void AfterImageApply(Image<TPixel> destination) { }

    /// <summary>
    ///     Disposes the object and frees resources for the Garbage Collector.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed and unmanaged objects.</param>
    protected virtual void Dispose(bool disposing) { }

    private Image<TPixel> CreateTarget()
    {
        var source = Source;
        var destinationSize = GetDestinationSize();

        // We will always be creating the clone even for mutate because we may need to resize the canvas.
        var destinationFrames = new ImageFrame<TPixel>[source.Frames.Count];
        for (var i = 0; i < destinationFrames.Length; i++)
            destinationFrames[i] = new ImageFrame<TPixel>(
                Configuration,
                destinationSize.Width,
                destinationSize.Height,
                source.Frames[i].Metadata.DeepClone());

        // Use the overload to prevent an extra frame being added.
        return new Image<TPixel>(Configuration, source.Metadata.DeepClone(), destinationFrames, source.Format);
    }

    private void CheckFrameCount(Image<TPixel> a, Image<TPixel> b)
    {
        if (a.Frames.Count != b.Frames.Count)
            throw new ImageProcessingException(
                $"An error occurred when processing the image using {GetType().Name}. The processor changed the number of frames.");
    }
}