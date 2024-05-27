// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     Utility methods for affine and projective transforms.
/// </summary>
internal static class LinearTransformUtilities
{
    [MethodImpl(InliningOptions.ShortMethod)]
    internal static int GetSamplingRadius<TResampler>(in TResampler sampler, int sourceSize, int destinationSize)
        where TResampler : struct, IResampler
    {
        double scale = sourceSize / destinationSize;
        if (scale < 1) scale = 1;

        return (int)Math.Ceiling(scale * sampler.Radius);
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    internal static void Convolve<TResampler, TPixel>(
        in TResampler sampler,
        Vector2 transformedPoint,
        Buffer2D<TPixel> sourcePixels,
        Span<Vector4> targetRow,
        int column,
        ref float yKernelSpanRef,
        ref float xKernelSpanRef,
        Vector2 radialExtents,
        Vector4 maxSourceExtents)
        where TResampler : struct, IResampler
        where TPixel : unmanaged, IPixel<TPixel>
    {
        // Clamp sampling pixel radial extents to the source image edges
        var minXY = transformedPoint - radialExtents;
        var maxXY = transformedPoint + radialExtents;

        // left, top, right, bottom
        var sourceExtents = new Vector4(
            MathF.Ceiling(minXY.X),
            MathF.Ceiling(minXY.Y),
            MathF.Floor(maxXY.X),
            MathF.Floor(maxXY.Y));

        sourceExtents = Vector4Utilities.FastClamp(sourceExtents, Vector4.Zero, maxSourceExtents);

        var left = (int)sourceExtents.X;
        var top = (int)sourceExtents.Y;
        var right = (int)sourceExtents.Z;
        var bottom = (int)sourceExtents.W;

        if (left == right || top == bottom) return;

        CalculateWeights(in sampler, top, bottom, transformedPoint.Y, ref yKernelSpanRef);
        CalculateWeights(in sampler, left, right, transformedPoint.X, ref xKernelSpanRef);

        var sum = Vector4.Zero;
        for (int kernelY = 0, y = top; y <= bottom; y++, kernelY++)
        {
            var yWeight = Unsafe.Add(ref yKernelSpanRef, kernelY);

            for (int kernelX = 0, x = left; x <= right; x++, kernelX++)
            {
                var xWeight = Unsafe.Add(ref xKernelSpanRef, kernelX);

                // Values are first premultiplied to prevent darkening of edge pixels.
                var current = sourcePixels[x, y].ToVector4();
                Vector4Utilities.Premultiply(ref current);
                sum += current * xWeight * yWeight;
            }
        }

        // Reverse the premultiplication
        Vector4Utilities.UnPremultiply(ref sum);
        targetRow[column] = sum;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static void CalculateWeights<TResampler>(in TResampler sampler, int min, int max, float point,
        ref float weightsRef)
        where TResampler : struct, IResampler
    {
        float sum = 0;
        for (int x = 0, i = min; i <= max; i++, x++)
        {
            var weight = sampler.GetValue(i - point);
            sum += weight;
            Unsafe.Add(ref weightsRef, x) = weight;
        }
    }
}