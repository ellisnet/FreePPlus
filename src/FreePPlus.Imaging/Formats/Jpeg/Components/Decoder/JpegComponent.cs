// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using FreePPlus.Imaging.Memory;

namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;

/// <summary>
///     Represents a single frame component.
/// </summary>
internal sealed class JpegComponent : IDisposable, IJpegComponent
{
    private readonly MemoryAllocator memoryAllocator;

    public JpegComponent(MemoryAllocator memoryAllocator, JpegFrame frame, byte id, int horizontalFactor,
        int verticalFactor, byte quantizationTableIndex, int index)
    {
        this.memoryAllocator = memoryAllocator;
        Frame = frame;
        Id = id;

        // Validate sampling factors.
        if (horizontalFactor == 0 || verticalFactor == 0) JpegThrowHelper.ThrowBadSampling();

        HorizontalSamplingFactor = horizontalFactor;
        VerticalSamplingFactor = verticalFactor;
        SamplingFactors = new Size(HorizontalSamplingFactor, VerticalSamplingFactor);

        if (quantizationTableIndex > 3) JpegThrowHelper.ThrowBadQuantizationTable();

        QuantizationTableIndex = quantizationTableIndex;
        Index = index;
    }

    /// <summary>
    ///     Gets the component id.
    /// </summary>
    public byte Id { get; }

    /// <summary>
    ///     Gets or sets DC coefficient predictor.
    /// </summary>
    public int DcPredictor { get; set; }

    /// <summary>
    ///     Gets the horizontal sampling factor.
    /// </summary>
    public int HorizontalSamplingFactor { get; }

    /// <summary>
    ///     Gets the vertical sampling factor.
    /// </summary>
    public int VerticalSamplingFactor { get; }

    /// <summary>
    ///     Gets the number of blocks per line.
    /// </summary>
    public int WidthInBlocks { get; private set; }

    /// <summary>
    ///     Gets the number of blocks per column.
    /// </summary>
    public int HeightInBlocks { get; private set; }

    /// <summary>
    ///     Gets or sets the index for the DC Huffman table.
    /// </summary>
    public int DCHuffmanTableId { get; set; }

    /// <summary>
    ///     Gets or sets the index for the AC Huffman table.
    /// </summary>
    public int ACHuffmanTableId { get; set; }

    public JpegFrame Frame { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        SpectralBlocks?.Dispose();
        SpectralBlocks = null;
    }

    /// <inheritdoc />
    public Buffer2D<Block8x8> SpectralBlocks { get; private set; }

    /// <inheritdoc />
    public Size SubSamplingDivisors { get; private set; }

    /// <inheritdoc />
    public int QuantizationTableIndex { get; }

    /// <inheritdoc />
    public int Index { get; }

    /// <inheritdoc />
    public Size SizeInBlocks { get; private set; }

    /// <inheritdoc />
    public Size SamplingFactors { get; set; }

    public void Init()
    {
        WidthInBlocks = (int)MathF.Ceiling(
            MathF.Ceiling(Frame.SamplesPerLine / 8F) * HorizontalSamplingFactor / Frame.MaxHorizontalFactor);

        HeightInBlocks = (int)MathF.Ceiling(
            MathF.Ceiling(Frame.Scanlines / 8F) * VerticalSamplingFactor / Frame.MaxVerticalFactor);

        var blocksPerLineForMcu = Frame.McusPerLine * HorizontalSamplingFactor;
        var blocksPerColumnForMcu = Frame.McusPerColumn * VerticalSamplingFactor;
        SizeInBlocks = new Size(blocksPerLineForMcu, blocksPerColumnForMcu);

        var c0 = Frame.Components[0];
        SubSamplingDivisors = c0.SamplingFactors.DivideBy(SamplingFactors);

        if (SubSamplingDivisors.Width == 0 || SubSamplingDivisors.Height == 0) JpegThrowHelper.ThrowBadSampling();

        var totalNumberOfBlocks = blocksPerColumnForMcu * (blocksPerLineForMcu + 1);
        var width = WidthInBlocks + 1;
        var height = totalNumberOfBlocks / width;

        SpectralBlocks = memoryAllocator.Allocate2D<Block8x8>(width, height, AllocationOptions.Clean);
    }
}