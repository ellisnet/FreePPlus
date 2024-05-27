// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Quantization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

/// <summary>
///     Enables the quantization of images to reduce the number of colors used in the image palette.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class QuantizeProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly IQuantizer quantizer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="QuantizeProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="quantizer">The quantizer used to reduce the color palette.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public QuantizeProcessor(Configuration configuration, IQuantizer quantizer, Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle)
    {
        Guard.NotNull(quantizer, nameof(quantizer));
        this.quantizer = quantizer;
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        var interest = Rectangle.Intersect(source.Bounds(), SourceRectangle);

        var configuration = Configuration;
        using var frameQuantizer = quantizer.CreatePixelSpecificQuantizer<TPixel>(configuration);
        using var quantized = frameQuantizer.BuildPaletteAndQuantizeFrame(source, interest);

        var operation = new RowIntervalOperation(SourceRectangle, source, quantized);
        ParallelRowIterator.IterateRowIntervals(
            configuration,
            interest,
            in operation);
    }

    private readonly struct RowIntervalOperation : IRowIntervalOperation
    {
        private readonly Rectangle bounds;
        private readonly ImageFrame<TPixel> source;
        private readonly IndexedImageFrame<TPixel> quantized;

        [MethodImpl(InliningOptions.ShortMethod)]
        public RowIntervalOperation(
            Rectangle bounds,
            ImageFrame<TPixel> source,
            IndexedImageFrame<TPixel> quantized)
        {
            this.bounds = bounds;
            this.source = source;
            this.quantized = quantized;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(in RowInterval rows)
        {
            var paletteSpan = quantized.Palette.Span;
            var offsetY = bounds.Top;
            var offsetX = bounds.Left;

            for (var y = rows.Min; y < rows.Max; y++)
            {
                var row = source.GetPixelRowSpan(y);
                var quantizedRow = quantized.GetPixelRowSpan(y - offsetY);

                for (var x = bounds.Left; x < bounds.Right; x++) row[x] = paletteSpan[quantizedRow[x - offsetX]];
            }
        }
    }
}