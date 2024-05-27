// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Drawing;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Drawing;

/// <summary>
///     Combines two images together by blending the pixels.
/// </summary>
/// <typeparam name="TPixelBg">The pixel format of destination image.</typeparam>
/// <typeparam name="TPixelFg">The pixel format of source image.</typeparam>
internal class DrawImageProcessor<TPixelBg, TPixelFg> : ImageProcessor<TPixelBg>
    where TPixelBg : unmanaged, IPixel<TPixelBg>
    where TPixelFg : unmanaged, IPixel<TPixelFg>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DrawImageProcessor{TPixelBg, TPixelFg}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="image">The foreground <see cref="Image{TPixelFg}" /> to blend with the currently processing image.</param>
    /// <param name="source">The source <see cref="Image{TPixelBg}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    /// <param name="location">The location to draw the blended image.</param>
    /// <param name="colorBlendingMode">The blending mode to use when drawing the image.</param>
    /// <param name="alphaCompositionMode">The Alpha blending mode to use when drawing the image.</param>
    /// <param name="opacity">The opacity of the image to blend. Must be between 0 and 1.</param>
    public DrawImageProcessor(
        Configuration configuration,
        Image<TPixelFg> image,
        Image<TPixelBg> source,
        Rectangle sourceRectangle,
        Point location,
        PixelColorBlendingMode colorBlendingMode,
        PixelAlphaCompositionMode alphaCompositionMode,
        float opacity)
        : base(configuration, source, sourceRectangle)
    {
        Guard.MustBeBetweenOrEqualTo(opacity, 0, 1, nameof(opacity));

        Image = image;
        Opacity = opacity;
        Blender = PixelOperations<TPixelBg>.Instance.GetPixelBlender(colorBlendingMode, alphaCompositionMode);
        Location = location;
    }

    /// <summary>
    ///     Gets the image to blend
    /// </summary>
    public Image<TPixelFg> Image { get; }

    /// <summary>
    ///     Gets the opacity of the image to blend
    /// </summary>
    public float Opacity { get; }

    /// <summary>
    ///     Gets the pixel blender
    /// </summary>
    public PixelBlender<TPixelBg> Blender { get; }

    /// <summary>
    ///     Gets the location to draw the blended image
    /// </summary>
    public Point Location { get; }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixelBg> source)
    {
        var sourceRectangle = SourceRectangle;
        var configuration = Configuration;

        var targetImage = Image;
        var blender = Blender;
        var locationY = Location.Y;

        // Align start/end positions.
        var bounds = targetImage.Bounds();

        var minX = Math.Max(Location.X, sourceRectangle.X);
        var maxX = Math.Min(Location.X + bounds.Width, sourceRectangle.Right);
        var targetX = minX - Location.X;

        var minY = Math.Max(Location.Y, sourceRectangle.Y);
        var maxY = Math.Min(Location.Y + bounds.Height, sourceRectangle.Bottom);

        var width = maxX - minX;

        var workingRect = Rectangle.FromLTRB(minX, minY, maxX, maxY);

        // Not a valid operation because rectangle does not overlap with this image.
        if (workingRect.Width <= 0 || workingRect.Height <= 0)
            throw new ImageProcessingException(
                "Cannot draw image because the source image does not overlap the target image.");

        var operation = new RowOperation(source, targetImage, blender, configuration, minX, width, locationY, targetX,
            Opacity);
        ParallelRowIterator.IterateRows(
            configuration,
            workingRect,
            in operation);
    }

    /// <summary>
    ///     A <see langword="struct" /> implementing the draw logic for <see cref="DrawImageProcessor{TPixelBg,TPixelFg}" />.
    /// </summary>
    private readonly struct RowOperation : IRowOperation
    {
        private readonly ImageFrame<TPixelBg> sourceFrame;
        private readonly Image<TPixelFg> targetImage;
        private readonly PixelBlender<TPixelBg> blender;
        private readonly Configuration configuration;
        private readonly int minX;
        private readonly int width;
        private readonly int locationY;
        private readonly int targetX;
        private readonly float opacity;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowOperation(
            ImageFrame<TPixelBg> sourceFrame,
            Image<TPixelFg> targetImage,
            PixelBlender<TPixelBg> blender,
            Configuration configuration,
            int minX,
            int width,
            int locationY,
            int targetX,
            float opacity)
        {
            this.sourceFrame = sourceFrame;
            this.targetImage = targetImage;
            this.blender = blender;
            this.configuration = configuration;
            this.minX = minX;
            this.width = width;
            this.locationY = locationY;
            this.targetX = targetX;
            this.opacity = opacity;
        }

        /// <inheritdoc />
        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y)
        {
            var background = sourceFrame.GetPixelRowSpan(y).Slice(minX, width);
            var foreground = targetImage.GetPixelRowSpan(y - locationY).Slice(targetX, width);
            blender.Blend<TPixelFg>(configuration, background, background, foreground, opacity);
        }
    }
}