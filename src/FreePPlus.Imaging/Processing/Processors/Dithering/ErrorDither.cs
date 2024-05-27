// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing.Processors.Quantization;

#pragma warning disable IDE0290

namespace FreePPlus.Imaging.Processing.Processors.Dithering;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Dithering;

/// <summary>
///     An error diffusion dithering implementation.
///     <see href="http://www.efg2.com/Lab/Library/ImageProcessing/DHALF.TXT" />
/// </summary>
public readonly partial struct ErrorDither : IDither, IEquatable<ErrorDither>, IEquatable<IDither>
{
    private readonly int offset;
    private readonly DenseMatrix<float> matrix;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ErrorDither" /> struct.
    /// </summary>
    /// <param name="matrix">The diffusion matrix.</param>
    /// <param name="offset">The starting offset within the matrix.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public ErrorDither(in DenseMatrix<float> matrix, int offset)
    {
        this.matrix = matrix;
        this.offset = offset;
    }

    /// <summary>
    ///     Compares the two <see cref="ErrorDither" /> instances to determine whether they are equal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator ==(IDither left, ErrorDither right)
    {
        return right == left;
    }

    /// <summary>
    ///     Compares the two <see cref="ErrorDither" /> instances to determine whether they are unequal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator !=(IDither left, ErrorDither right)
    {
        return !(right == left);
    }

    /// <summary>
    ///     Compares the two <see cref="ErrorDither" /> instances to determine whether they are equal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator ==(ErrorDither left, IDither right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares the two <see cref="ErrorDither" /> instances to determine whether they are unequal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator !=(ErrorDither left, IDither right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Compares the two <see cref="ErrorDither" /> instances to determine whether they are equal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator ==(ErrorDither left, ErrorDither right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares the two <see cref="ErrorDither" /> instances to determine whether they are unequal.
    /// </summary>
    /// <param name="left">The first source instance.</param>
    /// <param name="right">The second source instance.</param>
    /// <returns>The <see cref="bool" />.</returns>
    public static bool operator !=(ErrorDither left, ErrorDither right)
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
        var offsetY = bounds.Top;
        var offsetX = bounds.Left;
        var scale = quantizer.Options.DitherScale;

        for (var y = bounds.Top; y < bounds.Bottom; y++)
        {
            ref var sourceRowRef = ref MemoryMarshal.GetReference(source.GetPixelRowSpan(y));
            ref var destinationRowRef =
                ref MemoryMarshal.GetReference(destination.GetWritablePixelRowSpanUnsafe(y - offsetY));

            for (var x = bounds.Left; x < bounds.Right; x++)
            {
                var sourcePixel = Unsafe.Add(ref sourceRowRef, x);
                Unsafe.Add(ref destinationRowRef, x - offsetX) =
                    quantizer.GetQuantizedColor(sourcePixel, out var transformed);
                Dither(source, bounds, sourcePixel, transformed, x, y, scale);
            }
        }
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
        var scale = processor.DitherScale;
        for (var y = bounds.Top; y < bounds.Bottom; y++)
        {
            ref var sourceRowRef = ref MemoryMarshal.GetReference(source.GetPixelRowSpan(y));
            for (var x = bounds.Left; x < bounds.Right; x++)
            {
                ref var sourcePixel = ref Unsafe.Add(ref sourceRowRef, x);
                var transformed = Unsafe.AsRef(in processor).GetPaletteColor(sourcePixel);
                Dither(source, bounds, sourcePixel, transformed, x, y, scale);
                sourcePixel = transformed;
            }
        }
    }

    // Internal for AOT
    [MethodImpl(InliningOptions.ShortMethod)]
    internal TPixel Dither<TPixel>(
        ImageFrame<TPixel> image,
        Rectangle bounds,
        TPixel source,
        TPixel transformed,
        int x,
        int y,
        float scale)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        // Equal? Break out as there's no error to pass.
        if (source.Equals(transformed)) return transformed;

        // Calculate the error
        var error = (source.ToVector4() - transformed.ToVector4()) * scale;

        var offset = this.offset;
        var matrix = this.matrix;

        // Loop through and distribute the error amongst neighboring pixels.
        for (int row = 0, targetY = y; row < matrix.Rows; row++, targetY++)
        {
            if (targetY >= bounds.Bottom) continue;

            var rowSpan = image.GetPixelRowSpan(targetY);

            for (var col = 0; col < matrix.Columns; col++)
            {
                var targetX = x + (col - offset);
                if (targetX < bounds.Left || targetX >= bounds.Right) continue;

                var coefficient = matrix[row, col];
                if (coefficient == 0) continue;

                ref var pixel = ref rowSpan[targetX];
                var result = pixel.ToVector4();

                result += error * coefficient;
                pixel.FromVector4(result);
            }
        }

        return transformed;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is ErrorDither dither && Equals(dither);
    }

    /// <inheritdoc />
    public bool Equals(ErrorDither other)
    {
        return offset == other.offset && matrix.Equals(other.matrix);
    }

    /// <inheritdoc />
    public bool Equals(IDither other)
    {
        return Equals((object)other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(offset, matrix);
    }
}