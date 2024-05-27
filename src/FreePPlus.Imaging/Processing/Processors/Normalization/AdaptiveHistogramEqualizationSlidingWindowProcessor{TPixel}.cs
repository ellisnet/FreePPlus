// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Normalization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Normalization;

/// <summary>
///     Applies an adaptive histogram equalization to the image using an sliding window approach.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel> : HistogramEqualizationProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AdaptiveHistogramEqualizationSlidingWindowProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="luminanceLevels">
    ///     The number of different luminance levels. Typical values are 256 for 8-bit grayscale images
    ///     or 65536 for 16-bit grayscale images.
    /// </param>
    /// <param name="clipHistogram">Indicating whether to clip the histogram bins at a specific value.</param>
    /// <param name="clipLimit">The histogram clip limit. Histogram bins which exceed this limit, will be capped at this value.</param>
    /// <param name="tiles">
    ///     The number of tiles the image is split into (horizontal and vertically). Minimum value is 2.
    ///     Maximum value is 100.
    /// </param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public AdaptiveHistogramEqualizationSlidingWindowProcessor(
        Configuration configuration,
        int luminanceLevels,
        bool clipHistogram,
        int clipLimit,
        int tiles,
        Image<TPixel> source,
        Rectangle sourceRectangle)
        : base(configuration, luminanceLevels, clipHistogram, clipLimit, source, sourceRectangle)
    {
        Guard.MustBeGreaterThanOrEqualTo(tiles, 2, nameof(tiles));
        Guard.MustBeLessThanOrEqualTo(tiles, 100, nameof(tiles));

        Tiles = tiles;
    }

    /// <summary>
    ///     Gets the number of tiles the image is split into (horizontal and vertically) for the adaptive histogram
    ///     equalization.
    /// </summary>
    private int Tiles { get; }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> source)
    {
        var memoryAllocator = Configuration.MemoryAllocator;

        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Configuration.MaxDegreeOfParallelism };
        var tileWidth = source.Width / Tiles;
        var tileHeight = tileWidth;
        var pixelInTile = tileWidth * tileHeight;
        var halfTileHeight = tileHeight / 2;
        var halfTileWidth = halfTileHeight;
        var slidingWindowInfos =
            new SlidingWindowInfos(tileWidth, tileHeight, halfTileWidth, halfTileHeight, pixelInTile);

        // TODO: If the process was able to be switched to operate in parallel rows instead of columns
        // then we could take advantage of batching and allocate per-row buffers only once per batch.
        using var targetPixels = Configuration.MemoryAllocator.Allocate2D<TPixel>(source.Width, source.Height);

        // Process the inner tiles, which do not require to check the borders.
        var innerOperation = new SlidingWindowOperation(
            Configuration,
            this,
            source,
            memoryAllocator,
            targetPixels,
            slidingWindowInfos,
            halfTileHeight,
            source.Height - halfTileHeight,
            true);

        Parallel.For(
            halfTileWidth,
            source.Width - halfTileWidth,
            parallelOptions,
            innerOperation.Invoke);

        // Process the left border of the image.
        var leftBorderOperation = new SlidingWindowOperation(
            Configuration,
            this,
            source,
            memoryAllocator,
            targetPixels,
            slidingWindowInfos,
            0,
            source.Height,
            false);

        Parallel.For(
            0,
            halfTileWidth,
            parallelOptions,
            leftBorderOperation.Invoke);

        // Process the right border of the image.
        var rightBorderOperation = new SlidingWindowOperation(
            Configuration,
            this,
            source,
            memoryAllocator,
            targetPixels,
            slidingWindowInfos,
            0,
            source.Height,
            false);

        Parallel.For(
            source.Width - halfTileWidth,
            source.Width,
            parallelOptions,
            rightBorderOperation.Invoke);

        // Process the top border of the image.
        var topBorderOperation = new SlidingWindowOperation(
            Configuration,
            this,
            source,
            memoryAllocator,
            targetPixels,
            slidingWindowInfos,
            0,
            halfTileHeight,
            false);

        Parallel.For(
            halfTileWidth,
            source.Width - halfTileWidth,
            parallelOptions,
            topBorderOperation.Invoke);

        // Process the bottom border of the image.
        var bottomBorderOperation = new SlidingWindowOperation(
            Configuration,
            this,
            source,
            memoryAllocator,
            targetPixels,
            slidingWindowInfos,
            source.Height - halfTileHeight,
            source.Height,
            false);

        Parallel.For(
            halfTileWidth,
            source.Width - halfTileWidth,
            parallelOptions,
            bottomBorderOperation.Invoke);

        Buffer2D<TPixel>.SwapOrCopyContent(source.PixelBuffer, targetPixels);
    }

    /// <summary>
    ///     Get the a pixel row at a given position with a length of the tile width. Mirrors pixels which exceeds the edges.
    /// </summary>
    /// <param name="source">The source image.</param>
    /// <param name="rowPixels">Pre-allocated pixel row span of the size of a the tile width.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="tileWidth">The width in pixels of a tile.</param>
    /// <param name="configuration">The configuration.</param>
    private void CopyPixelRow(
        ImageFrame<TPixel> source,
        Span<Vector4> rowPixels,
        int x,
        int y,
        int tileWidth,
        Configuration configuration)
    {
        if (y < 0)
        {
            y = ImageMaths.FastAbs(y);
        }
        else if (y >= source.Height)
        {
            var diff = y - source.Height;
            y = source.Height - diff - 1;
        }

        // Special cases for the left and the right border where GetPixelRowSpan can not be used.
        if (x < 0)
        {
            rowPixels.Clear();
            var idx = 0;
            for (var dx = x; dx < x + tileWidth; dx++)
            {
                rowPixels[idx] = source[ImageMaths.FastAbs(dx), y].ToVector4();
                idx++;
            }

            return;
        }

        if (x + tileWidth > source.Width)
        {
            rowPixels.Clear();
            var idx = 0;
            for (var dx = x; dx < x + tileWidth; dx++)
            {
                if (dx >= source.Width)
                {
                    var diff = dx - source.Width;
                    rowPixels[idx] = source[dx - diff - 1, y].ToVector4();
                }
                else
                {
                    rowPixels[idx] = source[dx, y].ToVector4();
                }

                idx++;
            }

            return;
        }

        CopyPixelRowFast(source, rowPixels, x, y, tileWidth, configuration);
    }

    /// <summary>
    ///     Get the a pixel row at a given position with a length of the tile width.
    /// </summary>
    /// <param name="source">The source image.</param>
    /// <param name="rowPixels">Pre-allocated pixel row span of the size of a the tile width.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="tileWidth">The width in pixels of a tile.</param>
    /// <param name="configuration">The configuration.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    private void CopyPixelRowFast(
        ImageFrame<TPixel> source,
        Span<Vector4> rowPixels,
        int x,
        int y,
        int tileWidth,
        Configuration configuration)
    {
        PixelOperations<TPixel>.Instance.ToVector4(configuration, source.GetPixelRowSpan(y).Slice(x, tileWidth),
            rowPixels);
    }

    /// <summary>
    ///     Adds a column of grey values to the histogram.
    /// </summary>
    /// <param name="greyValuesBase">The reference to the span of grey values to add.</param>
    /// <param name="histogramBase">The reference to the histogram span.</param>
    /// <param name="luminanceLevels">The number of different luminance levels.</param>
    /// <param name="length">The grey values span length.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    private void AddPixelsToHistogram(ref Vector4 greyValuesBase, ref int histogramBase, int luminanceLevels,
        int length)
    {
        for (var idx = 0; idx < length; idx++)
        {
            var luminance = ImageMaths.GetBT709Luminance(ref Unsafe.Add(ref greyValuesBase, idx), luminanceLevels);
            Unsafe.Add(ref histogramBase, luminance)++;
        }
    }

    /// <summary>
    ///     Removes a column of grey values from the histogram.
    /// </summary>
    /// <param name="greyValuesBase">The reference to the span of grey values to remove.</param>
    /// <param name="histogramBase">The reference to the histogram span.</param>
    /// <param name="luminanceLevels">The number of different luminance levels.</param>
    /// <param name="length">The grey values span length.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    private void RemovePixelsFromHistogram(ref Vector4 greyValuesBase, ref int histogramBase, int luminanceLevels,
        int length)
    {
        for (var idx = 0; idx < length; idx++)
        {
            var luminance = ImageMaths.GetBT709Luminance(ref Unsafe.Add(ref greyValuesBase, idx), luminanceLevels);
            Unsafe.Add(ref histogramBase, luminance)--;
        }
    }

    /// <summary>
    ///     Applies the sliding window equalization to one column of the image. The window is moved from top to bottom.
    ///     Moving the window one pixel down requires to remove one row from the top of the window from the histogram and
    ///     adding a new row at the bottom.
    /// </summary>
    private readonly struct SlidingWindowOperation
    {
        private readonly Configuration configuration;
        private readonly AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel> processor;
        private readonly ImageFrame<TPixel> source;
        private readonly MemoryAllocator memoryAllocator;
        private readonly Buffer2D<TPixel> targetPixels;
        private readonly SlidingWindowInfos swInfos;
        private readonly int yStart;
        private readonly int yEnd;
        private readonly bool useFastPath;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SlidingWindowOperation" /> struct.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="processor">The histogram processor.</param>
        /// <param name="source">The source image.</param>
        /// <param name="memoryAllocator">The memory allocator.</param>
        /// <param name="targetPixels">The target pixels.</param>
        /// <param name="swInfos"><see cref="SlidingWindowInfos" /> about the sliding window dimensions.</param>
        /// <param name="yStart">The y start position.</param>
        /// <param name="yEnd">The y end position.</param>
        /// <param name="useFastPath">if set to true the borders of the image will not be checked.</param>
        [MethodImpl(InliningOptions.ShortMethod)]
        public SlidingWindowOperation(
            Configuration configuration,
            AdaptiveHistogramEqualizationSlidingWindowProcessor<TPixel> processor,
            ImageFrame<TPixel> source,
            MemoryAllocator memoryAllocator,
            Buffer2D<TPixel> targetPixels,
            SlidingWindowInfos swInfos,
            int yStart,
            int yEnd,
            bool useFastPath)
        {
            this.configuration = configuration;
            this.processor = processor;
            this.source = source;
            this.memoryAllocator = memoryAllocator;
            this.targetPixels = targetPixels;
            this.swInfos = swInfos;
            this.yStart = yStart;
            this.yEnd = yEnd;
            this.useFastPath = useFastPath;
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int x)
        {
            using (var histogramBuffer =
                   memoryAllocator.Allocate<int>(processor.LuminanceLevels, AllocationOptions.Clean))
            using (var histogramBufferCopy =
                   memoryAllocator.Allocate<int>(processor.LuminanceLevels, AllocationOptions.Clean))
            using (var cdfBuffer = memoryAllocator.Allocate<int>(processor.LuminanceLevels, AllocationOptions.Clean))
            using (var pixelRowBuffer = memoryAllocator.Allocate<Vector4>(swInfos.TileWidth, AllocationOptions.Clean))
            {
                var histogram = histogramBuffer.GetSpan();
                ref var histogramBase = ref MemoryMarshal.GetReference(histogram);

                var histogramCopy = histogramBufferCopy.GetSpan();
                ref var histogramCopyBase = ref MemoryMarshal.GetReference(histogramCopy);

                ref var cdfBase = ref MemoryMarshal.GetReference(cdfBuffer.GetSpan());

                var pixelRow = pixelRowBuffer.GetSpan();
                ref var pixelRowBase = ref MemoryMarshal.GetReference(pixelRow);

                // Build the initial histogram of grayscale values.
                for (var dy = yStart - swInfos.HalfTileHeight; dy < yStart + swInfos.HalfTileHeight; dy++)
                {
                    if (useFastPath)
                        processor.CopyPixelRowFast(source, pixelRow, x - swInfos.HalfTileWidth, dy, swInfos.TileWidth,
                            configuration);
                    else
                        processor.CopyPixelRow(source, pixelRow, x - swInfos.HalfTileWidth, dy, swInfos.TileWidth,
                            configuration);

                    processor.AddPixelsToHistogram(ref pixelRowBase, ref histogramBase, processor.LuminanceLevels,
                        pixelRow.Length);
                }

                for (var y = yStart; y < yEnd; y++)
                {
                    if (processor.ClipHistogramEnabled)
                    {
                        // Clipping the histogram, but doing it on a copy to keep the original un-clipped values for the next iteration.
                        histogram.CopyTo(histogramCopy);
                        processor.ClipHistogram(histogramCopy, processor.ClipLimit);
                    }

                    // Calculate the cumulative distribution function, which will map each input pixel in the current tile to a new value.
                    var cdfMin = processor.ClipHistogramEnabled
                        ? processor.CalculateCdf(ref cdfBase, ref histogramCopyBase, histogram.Length - 1)
                        : processor.CalculateCdf(ref cdfBase, ref histogramBase, histogram.Length - 1);

                    float numberOfPixelsMinusCdfMin = swInfos.PixelInTile - cdfMin;

                    // Map the current pixel to the new equalized value.
                    var luminance = GetLuminance(source[x, y], processor.LuminanceLevels);
                    var luminanceEqualized = Unsafe.Add(ref cdfBase, luminance) / numberOfPixelsMinusCdfMin;
                    targetPixels[x, y].FromVector4(new Vector4(luminanceEqualized, luminanceEqualized,
                        luminanceEqualized, source[x, y].ToVector4().W));

                    // Remove top most row from the histogram, mirroring rows which exceeds the borders.
                    if (useFastPath)
                        processor.CopyPixelRowFast(source, pixelRow, x - swInfos.HalfTileWidth,
                            y - swInfos.HalfTileWidth, swInfos.TileWidth, configuration);
                    else
                        processor.CopyPixelRow(source, pixelRow, x - swInfos.HalfTileWidth, y - swInfos.HalfTileWidth,
                            swInfos.TileWidth, configuration);

                    processor.RemovePixelsFromHistogram(ref pixelRowBase, ref histogramBase, processor.LuminanceLevels,
                        pixelRow.Length);

                    // Add new bottom row to the histogram, mirroring rows which exceeds the borders.
                    if (useFastPath)
                        processor.CopyPixelRowFast(source, pixelRow, x - swInfos.HalfTileWidth,
                            y + swInfos.HalfTileWidth, swInfos.TileWidth, configuration);
                    else
                        processor.CopyPixelRow(source, pixelRow, x - swInfos.HalfTileWidth, y + swInfos.HalfTileWidth,
                            swInfos.TileWidth, configuration);

                    processor.AddPixelsToHistogram(ref pixelRowBase, ref histogramBase, processor.LuminanceLevels,
                        pixelRow.Length);
                }
            }
        }
    }

    private class SlidingWindowInfos
    {
        public SlidingWindowInfos(int tileWidth, int tileHeight, int halfTileWidth, int halfTileHeight, int pixelInTile)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            HalfTileWidth = halfTileWidth;
            HalfTileHeight = halfTileHeight;
            PixelInTile = pixelInTile;
        }

        public int TileWidth { get; }

        public int TileHeight { get; }

        public int PixelInTile { get; }

        public int HalfTileWidth { get; }

        public int HalfTileHeight { get; }
    }
}