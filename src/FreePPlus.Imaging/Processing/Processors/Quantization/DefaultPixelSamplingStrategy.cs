// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Quantization;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

/// <summary>
///     A pixel sampling strategy that enumerates a limited amount of rows from different frames,
///     if the total number of pixels is over a threshold.
/// </summary>
public class DefaultPixelSamplingStrategy : IPixelSamplingStrategy
{
    // TODO: This value shall be determined by benchmarking.
    // A smaller value should likely work well, providing better perf.
    private const int DefaultMaximumPixels = 4096 * 4096;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultPixelSamplingStrategy" /> class.
    /// </summary>
    public DefaultPixelSamplingStrategy()
        : this(DefaultMaximumPixels, 0.1) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultPixelSamplingStrategy" /> class.
    /// </summary>
    /// <param name="maximumPixels">The maximum number of pixels to process.</param>
    /// <param name="minimumScanRatio">always scan at least this portion of total pixels within the image.</param>
    public DefaultPixelSamplingStrategy(int maximumPixels, double minimumScanRatio)
    {
        Guard.MustBeGreaterThan(maximumPixels, 0, nameof(maximumPixels));
        MaximumPixels = maximumPixels;
        MinimumScanRatio = minimumScanRatio;
    }

    /// <summary>
    ///     Gets the maximum number of pixels to process. (The threshold.)
    /// </summary>
    public long MaximumPixels { get; }

    /// <summary>
    ///     Gets a value indicating: always scan at least this portion of total pixels within the image.
    ///     The default is 0.1 (10%).
    /// </summary>
    public double MinimumScanRatio { get; }

    /// <inheritdoc />
    public IEnumerable<Buffer2DRegion<TPixel>> EnumeratePixelRegions<TPixel>(Image<TPixel> image)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var maximumPixels = Math.Min(MaximumPixels, (long)image.Width * image.Height * image.Frames.Count);
        var maxNumberOfRows = maximumPixels / image.Width;
        var totalNumberOfRows = (long)image.Height * image.Frames.Count;

        if (totalNumberOfRows <= maxNumberOfRows)
        {
            // Enumerate all pixels
            foreach (ImageFrame<TPixel> frame in image.Frames) yield return frame.PixelBuffer.GetRegion();
        }
        else
        {
            var r = maxNumberOfRows / (double)totalNumberOfRows;

            // Use a rough approximation to make sure we don't leave out large contiguous regions:
            if (maxNumberOfRows > 200)
                r = Math.Round(r, 2);
            else
                r = Math.Round(r, 1);

            r = Math.Max(MinimumScanRatio, r); // always visit the minimum defined portion of the image.

            var ratio = new Rational(r);

            var denom = (int)ratio.Denominator;
            var num = (int)ratio.Numerator;

            for (var pos = 0; pos < totalNumberOfRows; pos++)
            {
                var subPos = pos % denom;
                if (subPos < num) yield return GetRow(pos);
            }

            Buffer2DRegion<TPixel> GetRow(int pos)
            {
                var frameIdx = pos / image.Height;
                var y = pos % image.Height;
                return image.Frames[frameIdx].PixelBuffer.GetRegion(0, y, image.Width, 1);
            }
        }
    }
}