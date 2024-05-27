// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Advanced;
using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing.Processors.Quantization;

#pragma warning disable IDE0290

namespace FreePPlus.Imaging.Processing.Processors.Dithering;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Dithering;

/// <summary>
///     An ordered dithering matrix with equal sides of arbitrary length
/// </summary>
public readonly partial struct OrderedDither : IDither, IEquatable<OrderedDither>, IEquatable<IDither>
{
    private readonly DenseMatrix<float> thresholdMatrix;
    private readonly int modulusX;
    private readonly int modulusY;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OrderedDither" /> struct.
    /// </summary>
    /// <param name="length">The length of the matrix sides</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public OrderedDither(uint length)
    {
        var ditherMatrix = OrderedDitherFactory.CreateDitherMatrix(length);

        // Create a new matrix to run against, that pre-thresholds the values.
        // We don't want to adjust the original matrix generation code as that
        // creates known, easy to test values.
        // https://en.wikipedia.org/wiki/Ordered_dithering#Algorithm
        var thresholdMatrix = new DenseMatrix<float>((int)length);
        float m2 = length * length;
        for (var y = 0; y < length; y++)
        for (var x = 0; x < length; x++)
            thresholdMatrix[y, x] = (ditherMatrix[y, x] + 1) / m2 - .5F;

        modulusX = ditherMatrix.Columns;
        modulusY = ditherMatrix.Rows;
        this.thresholdMatrix = thresholdMatrix;
    }

    /// <summary>
    ///     Compares the two <see cref="OrderedDither" /> instances to determine whether they are equal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator ==(IDither left, OrderedDither right)
    {
        return right == left;
    }

    /// <summary>
    ///     Compares the two <see cref="OrderedDither" /> instances to determine whether they are unequal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator !=(IDither left, OrderedDither right)
    {
        return !(right == left);
    }

    /// <summary>
    ///     Compares the two <see cref="OrderedDither" /> instances to determine whether they are equal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator ==(OrderedDither left, IDither right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares the two <see cref="OrderedDither" /> instances to determine whether they are unequal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator !=(OrderedDither left, IDither right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Compares the two <see cref="OrderedDither" /> instances to determine whether they are equal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator ==(OrderedDither left, OrderedDither right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares the two <see cref="OrderedDither" /> instances to determine whether they are unequal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator !=(OrderedDither left, OrderedDither right)
    {
        return !(left == right);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ApplyQuantizationDither<TFrameQuantizer, TPixel>(
        ref TFrameQuantizer quantizer,
        ImageFrame<TPixel> source,
        IndexedImageFrame<TPixel> destination,
        Rectangle bounds)
        where TFrameQuantizer : struct, IQuantizer<TPixel>
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var ditherOperation = new QuantizeDitherRowOperation<TFrameQuantizer, TPixel>(
            ref quantizer,
            in Unsafe.AsRef(in this),
            source,
            destination,
            bounds);

        ParallelRowIterator.IterateRows(
            quantizer.Configuration,
            bounds,
            in ditherOperation);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ApplyPaletteDither<TPaletteDitherImageProcessor, TPixel>(
        in TPaletteDitherImageProcessor processor,
        ImageFrame<TPixel> source,
        Rectangle bounds)
        where TPaletteDitherImageProcessor : struct, IPaletteDitherImageProcessor<TPixel>
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var ditherOperation = new PaletteDitherRowOperation<TPaletteDitherImageProcessor, TPixel>(
            in processor,
            in Unsafe.AsRef(in this),
            source,
            bounds);

        ParallelRowIterator.IterateRows(
            processor.Configuration,
            bounds,
            in ditherOperation);
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    internal TPixel Dither<TPixel>(
        TPixel source,
        int x,
        int y,
        int bitDepth,
        float scale)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Rgba32 rgba = default;
        source.ToRgba32(ref rgba);
        Rgba32 attempt;

        // Spread assumes an even colorspace distribution and precision.
        // Calculated as 0-255/component count. 256 / bitDepth
        // https://bisqwit.iki.fi/story/howto/dither/jy/
        // https://en.wikipedia.org/wiki/Ordered_dithering#Algorithm
        var spread = 256 / bitDepth;
        var factor = spread * thresholdMatrix[y % modulusY, x % modulusX] * scale;

        attempt.R = (byte)(rgba.R + factor).Clamp(byte.MinValue, byte.MaxValue);
        attempt.G = (byte)(rgba.G + factor).Clamp(byte.MinValue, byte.MaxValue);
        attempt.B = (byte)(rgba.B + factor).Clamp(byte.MinValue, byte.MaxValue);
        attempt.A = (byte)(rgba.A + factor).Clamp(byte.MinValue, byte.MaxValue);

        TPixel result = default;
        result.FromRgba32(attempt);

        return result;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is OrderedDither dither && Equals(dither);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool Equals(OrderedDither other)
    {
        return thresholdMatrix.Equals(other.thresholdMatrix) && modulusX == other.modulusX &&
               modulusY == other.modulusY;
    }

    /// <inheritdoc />
    public bool Equals(IDither other)
    {
        return Equals((object)other);
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public override int GetHashCode()
    {
        return HashCode.Combine(thresholdMatrix, modulusX, modulusY);
    }

    private readonly struct QuantizeDitherRowOperation<TFrameQuantizer, TPixel> : IRowOperation
        where TFrameQuantizer : struct, IQuantizer<TPixel>
        where TPixel : unmanaged, IPixel<TPixel>
    {
        private readonly TFrameQuantizer quantizer;
        private readonly OrderedDither dither;
        private readonly ImageFrame<TPixel> source;
        private readonly IndexedImageFrame<TPixel> destination;
        private readonly Rectangle bounds;
        private readonly int bitDepth;

        [MethodImpl(InliningOptions.ShortMethod)]
        public QuantizeDitherRowOperation(
            ref TFrameQuantizer quantizer,
            in OrderedDither dither,
            ImageFrame<TPixel> source,
            IndexedImageFrame<TPixel> destination,
            Rectangle bounds)
        {
            this.quantizer = quantizer;
            this.dither = dither;
            this.source = source;
            this.destination = destination;
            this.bounds = bounds;
            bitDepth = ImageMaths.GetBitsNeededForColorDepth(destination.Palette.Length);
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y)
        {
            var offsetY = bounds.Top;
            var offsetX = bounds.Left;
            var scale = quantizer.Options.DitherScale;

            ref var sourceRowRef = ref MemoryMarshal.GetReference(source.GetPixelRowSpan(y));
            ref var destinationRowRef =
                ref MemoryMarshal.GetReference(destination.GetWritablePixelRowSpanUnsafe(y - offsetY));

            for (var x = bounds.Left; x < bounds.Right; x++)
            {
                var dithered = dither.Dither(Unsafe.Add(ref sourceRowRef, x), x, y, bitDepth, scale);
                Unsafe.Add(ref destinationRowRef, x - offsetX) =
                    Unsafe.AsRef(in quantizer).GetQuantizedColor(dithered, out var _);
            }
        }
    }

    private readonly struct PaletteDitherRowOperation<TPaletteDitherImageProcessor, TPixel> : IRowOperation
        where TPaletteDitherImageProcessor : struct, IPaletteDitherImageProcessor<TPixel>
        where TPixel : unmanaged, IPixel<TPixel>
    {
        private readonly TPaletteDitherImageProcessor processor;
        private readonly OrderedDither dither;
        private readonly ImageFrame<TPixel> source;
        private readonly Rectangle bounds;
        private readonly float scale;
        private readonly int bitDepth;

        [MethodImpl(InliningOptions.ShortMethod)]
        public PaletteDitherRowOperation(
            in TPaletteDitherImageProcessor processor,
            in OrderedDither dither,
            ImageFrame<TPixel> source,
            Rectangle bounds)
        {
            this.processor = processor;
            this.dither = dither;
            this.source = source;
            this.bounds = bounds;
            scale = processor.DitherScale;
            bitDepth = ImageMaths.GetBitsNeededForColorDepth(processor.Palette.Span.Length);
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        public void Invoke(int y)
        {
            ref var sourceRowRef = ref MemoryMarshal.GetReference(source.GetPixelRowSpan(y));

            for (var x = bounds.Left; x < bounds.Right; x++)
            {
                ref var sourcePixel = ref Unsafe.Add(ref sourceRowRef, x);
                var dithered = dither.Dither(sourcePixel, x, y, bitDepth, scale);
                sourcePixel = Unsafe.AsRef(in processor).GetPaletteColor(dithered);
            }
        }
    }
}