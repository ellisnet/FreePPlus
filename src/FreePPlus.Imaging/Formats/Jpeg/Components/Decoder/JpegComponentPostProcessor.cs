// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using FreePPlus.Imaging.Memory;

namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;

/// <summary>
///     Encapsulates postprocessing data for one component for <see cref="JpegImagePostProcessor" />.
/// </summary>
internal class JpegComponentPostProcessor : IDisposable
{
    /// <summary>
    ///     The size of the area in <see cref="ColorBuffer" /> corresponding to one 8x8 Jpeg block
    /// </summary>
    private readonly Size blockAreaSize;

    /// <summary>
    ///     Points to the current row in <see cref="Component" />.
    /// </summary>
    private int currentComponentRowInBlocks;

    /// <summary>
    ///     Initializes a new instance of the <see cref="JpegComponentPostProcessor" /> class.
    /// </summary>
    public JpegComponentPostProcessor(MemoryAllocator memoryAllocator, JpegImagePostProcessor imagePostProcessor,
        IJpegComponent component)
    {
        Component = component;
        ImagePostProcessor = imagePostProcessor;
        blockAreaSize = Component.SubSamplingDivisors * 8;
        ColorBuffer = memoryAllocator.Allocate2DOveraligned<float>(
            imagePostProcessor.PostProcessorBufferSize.Width,
            imagePostProcessor.PostProcessorBufferSize.Height,
            blockAreaSize.Height);

        BlockRowsPerStep = JpegImagePostProcessor.BlockRowsPerStep / Component.SubSamplingDivisors.Height;
    }

    /// <summary>
    ///     Gets the <see cref="JpegImagePostProcessor" />
    /// </summary>
    public JpegImagePostProcessor ImagePostProcessor { get; }

    /// <summary>
    ///     Gets the <see cref="Component" />
    /// </summary>
    public IJpegComponent Component { get; }

    /// <summary>
    ///     Gets the temporary working buffer of color values.
    /// </summary>
    public Buffer2D<float> ColorBuffer { get; }

    /// <summary>
    ///     Gets <see cref="IJpegComponent.SizeInBlocks" />
    /// </summary>
    public Size SizeInBlocks => Component.SizeInBlocks;

    /// <summary>
    ///     Gets the maximal number of block rows being processed in one step.
    /// </summary>
    public int BlockRowsPerStep { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        ColorBuffer.Dispose();
    }

    /// <summary>
    ///     Invoke <see cref="JpegBlockPostProcessor" /> for <see cref="BlockRowsPerStep" /> block rows, copy the result into
    ///     <see cref="ColorBuffer" />.
    /// </summary>
    public void CopyBlocksToColorBuffer()
    {
        var blockPp = new JpegBlockPostProcessor(ImagePostProcessor.RawJpeg, Component);
        var maximumValue = MathF.Pow(2, ImagePostProcessor.RawJpeg.Precision) - 1;

        var destAreaStride = ColorBuffer.Width;

        for (var y = 0; y < BlockRowsPerStep; y++)
        {
            var yBlock = currentComponentRowInBlocks + y;

            if (yBlock >= SizeInBlocks.Height) break;

            var yBuffer = y * blockAreaSize.Height;

            var colorBufferRow = ColorBuffer.GetRowSpan(yBuffer);
            var blockRow = Component.SpectralBlocks.GetRowSpan(yBlock);

            // see: https://github.com/SixLabors/ImageSharp/issues/824
            var widthInBlocks = Math.Min(Component.SpectralBlocks.Width, SizeInBlocks.Width);

            for (var xBlock = 0; xBlock < widthInBlocks; xBlock++)
            {
                ref var block = ref blockRow[xBlock];
                var xBuffer = xBlock * blockAreaSize.Width;
                ref var destAreaOrigin = ref colorBufferRow[xBuffer];

                blockPp.ProcessBlockColorsInto(ref block, ref destAreaOrigin, destAreaStride, maximumValue);
            }
        }

        currentComponentRowInBlocks += BlockRowsPerStep;
    }
}