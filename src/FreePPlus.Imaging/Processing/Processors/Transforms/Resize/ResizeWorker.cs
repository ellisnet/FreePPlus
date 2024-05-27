// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     Implements the resize algorithm using a sliding window of size
///     maximized by <see cref="Configuration.WorkingBufferSizeHintInBytes" />.
///     The height of the window is a multiple of the vertical kernel's maximum diameter.
///     When sliding the window, the contents of the bottom window band are copied to the new top band.
///     For more details, and visual explanation, see "ResizeWorker.pptx".
/// </summary>
internal sealed class ResizeWorker<TPixel> : IDisposable
    where TPixel : unmanaged, IPixel<TPixel>
{
    private readonly Configuration configuration;

    private readonly PixelConversionModifiers conversionModifiers;

    private readonly int destWidth;

    private readonly ResizeKernelMap horizontalKernelMap;

    private readonly Buffer2DRegion<TPixel> source;

    private readonly Rectangle sourceRectangle;

    private readonly Point targetOrigin;

    private readonly Rectangle targetWorkingRect;

    private readonly IMemoryOwner<Vector4> tempColumnBuffer;

    private readonly IMemoryOwner<Vector4> tempRowBuffer;
    private readonly Buffer2D<Vector4> transposedFirstPassBuffer;

    private readonly ResizeKernelMap verticalKernelMap;

    private readonly int windowBandHeight;

    private readonly int workerHeight;

    private RowInterval currentWindow;

    public ResizeWorker(
        Configuration configuration,
        Buffer2DRegion<TPixel> source,
        PixelConversionModifiers conversionModifiers,
        ResizeKernelMap horizontalKernelMap,
        ResizeKernelMap verticalKernelMap,
        int destWidth,
        Rectangle targetWorkingRect,
        Point targetOrigin)
    {
        this.configuration = configuration;
        this.source = source;
        sourceRectangle = source.Rectangle;
        this.conversionModifiers = conversionModifiers;
        this.horizontalKernelMap = horizontalKernelMap;
        this.verticalKernelMap = verticalKernelMap;
        this.destWidth = destWidth;
        this.targetWorkingRect = targetWorkingRect;
        this.targetOrigin = targetOrigin;

        windowBandHeight = verticalKernelMap.MaxDiameter;

        // We need to make sure the working buffer is contiguous:
        var workingBufferLimitHintInBytes = Math.Min(
            configuration.WorkingBufferSizeHintInBytes,
            configuration.MemoryAllocator.GetBufferCapacityInBytes());

        var numberOfWindowBands = ResizeHelper.CalculateResizeWorkerHeightInWindowBands(
            windowBandHeight,
            destWidth,
            workingBufferLimitHintInBytes);

        workerHeight = Math.Min(sourceRectangle.Height, numberOfWindowBands * windowBandHeight);

        transposedFirstPassBuffer = configuration.MemoryAllocator.Allocate2D<Vector4>(
            workerHeight,
            destWidth,
            AllocationOptions.Clean);

        tempRowBuffer = configuration.MemoryAllocator.Allocate<Vector4>(sourceRectangle.Width);
        tempColumnBuffer = configuration.MemoryAllocator.Allocate<Vector4>(destWidth);

        currentWindow = new RowInterval(0, workerHeight);
    }

    public void Dispose()
    {
        transposedFirstPassBuffer.Dispose();
        tempRowBuffer.Dispose();
        tempColumnBuffer.Dispose();
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public Span<Vector4> GetColumnSpan(int x, int startY)
    {
        return transposedFirstPassBuffer.GetRowSpan(x).Slice(startY - currentWindow.Min);
    }

    public void Initialize()
    {
        CalculateFirstPassValues(currentWindow);
    }

    public void FillDestinationPixels(RowInterval rowInterval, Buffer2D<TPixel> destination)
    {
        var tempColSpan = tempColumnBuffer.GetSpan();

        // When creating transposedFirstPassBuffer, we made sure it's contiguous:
        var transposedFirstPassBufferSpan = transposedFirstPassBuffer.GetSingleSpan();

        for (var y = rowInterval.Min; y < rowInterval.Max; y++)
        {
            // Ensure offsets are normalized for cropping and padding.
            var kernel = verticalKernelMap.GetKernel(y - targetOrigin.Y);

            while (kernel.StartIndex + kernel.Length > currentWindow.Max) Slide();

            ref var tempRowBase = ref MemoryMarshal.GetReference(tempColSpan);

            var top = kernel.StartIndex - currentWindow.Min;
            ref var fpBase = ref transposedFirstPassBufferSpan[top];

            for (var x = 0; x < destWidth; x++)
            {
                ref var firstPassColumnBase = ref Unsafe.Add(ref fpBase, x * workerHeight);

                // Destination color components
                Unsafe.Add(ref tempRowBase, x) = kernel.ConvolveCore(ref firstPassColumnBase);
            }

            var targetRowSpan = destination.GetRowSpan(y);

            PixelOperations<TPixel>.Instance.FromVector4Destructive(configuration, tempColSpan, targetRowSpan,
                conversionModifiers);
        }
    }

    private void Slide()
    {
        var minY = currentWindow.Max - windowBandHeight;
        var maxY = Math.Min(minY + workerHeight, sourceRectangle.Height);

        // Copy previous bottom band to the new top:
        // (rows <--> columns, because the buffer is transposed)
        transposedFirstPassBuffer.CopyColumns(
            workerHeight - windowBandHeight,
            0,
            windowBandHeight);

        currentWindow = new RowInterval(minY, maxY);

        // Calculate the remainder:
        CalculateFirstPassValues(currentWindow.Slice(windowBandHeight));
    }

    private void CalculateFirstPassValues(RowInterval calculationInterval)
    {
        var tempRowSpan = tempRowBuffer.GetSpan();
        var transposedFirstPassBufferSpan = transposedFirstPassBuffer.GetSingleSpan();

        for (var y = calculationInterval.Min; y < calculationInterval.Max; y++)
        {
            var sourceRow = source.GetRowSpan(y);

            PixelOperations<TPixel>.Instance.ToVector4(
                configuration,
                sourceRow,
                tempRowSpan,
                conversionModifiers);

            // optimization for:
            // Span<Vector4> firstPassSpan = transposedFirstPassBufferSpan.Slice(y - this.currentWindow.Min);
            ref var firstPassBaseRef = ref transposedFirstPassBufferSpan[y - currentWindow.Min];

            for (var x = targetWorkingRect.Left; x < targetWorkingRect.Right; x++)
            {
                var kernel = horizontalKernelMap.GetKernel(x - targetOrigin.X);

                // optimization for:
                // firstPassSpan[x * this.workerHeight] = kernel.Convolve(tempRowSpan);
                Unsafe.Add(ref firstPassBaseRef, x * workerHeight) = kernel.Convolve(tempRowSpan);
            }
        }
    }
}