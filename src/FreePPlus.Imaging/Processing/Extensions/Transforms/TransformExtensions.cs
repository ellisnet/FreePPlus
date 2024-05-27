﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using FreePPlus.Imaging.Processing.Processors.Transforms;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines extensions that allow the application of composable transform operations on an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class TransformExtensions
{
    /// <summary>
    ///     Performs an affine transform of an image.
    /// </summary>
    /// <param name="source">The image to transform.</param>
    /// <param name="builder">The affine transform builder.</param>
    /// <returns>The <see cref="Image{TPixel}" /></returns>
    public static IImageProcessingContext Transform(
        this IImageProcessingContext source,
        AffineTransformBuilder builder)
    {
        return Transform(source, builder, KnownResamplers.Bicubic);
    }

    /// <summary>
    ///     Performs an affine transform of an image using the specified sampling algorithm.
    /// </summary>
    /// <param name="ctx">The <see cref="IImageProcessingContext" />.</param>
    /// <param name="builder">The affine transform builder.</param>
    /// <param name="sampler">The <see cref="IResampler" /> to perform the resampling.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Transform(
        this IImageProcessingContext ctx,
        AffineTransformBuilder builder,
        IResampler sampler)
    {
        return ctx.Transform(new Rectangle(Point.Empty, ctx.GetCurrentSize()), builder, sampler);
    }

    /// <summary>
    ///     Performs an affine transform of an image using the specified sampling algorithm.
    /// </summary>
    /// <param name="ctx">The <see cref="IImageProcessingContext" />.</param>
    /// <param name="sourceRectangle">The source rectangle</param>
    /// <param name="builder">The affine transform builder.</param>
    /// <param name="sampler">The <see cref="IResampler" /> to perform the resampling.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Transform(
        this IImageProcessingContext ctx,
        Rectangle sourceRectangle,
        AffineTransformBuilder builder,
        IResampler sampler)
    {
        var transform = builder.BuildMatrix(sourceRectangle);
        var targetDimensions = TransformUtilities.GetTransformedSize(sourceRectangle.Size, transform);
        return ctx.Transform(sourceRectangle, transform, targetDimensions, sampler);
    }

    /// <summary>
    ///     Performs an affine transform of an image using the specified sampling algorithm.
    /// </summary>
    /// <param name="ctx">The <see cref="IImageProcessingContext" />.</param>
    /// <param name="sourceRectangle">The source rectangle</param>
    /// <param name="transform">The transformation matrix.</param>
    /// <param name="targetDimensions">The size of the result image.</param>
    /// <param name="sampler">The <see cref="IResampler" /> to perform the resampling.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Transform(
        this IImageProcessingContext ctx,
        Rectangle sourceRectangle,
        Matrix3x2 transform,
        Size targetDimensions,
        IResampler sampler)
    {
        return ctx.ApplyProcessor(
            new AffineTransformProcessor(transform, sampler, targetDimensions),
            sourceRectangle);
    }

    /// <summary>
    ///     Performs a projective transform of an image.
    /// </summary>
    /// <param name="source">The image to transform.</param>
    /// <param name="builder">The affine transform builder.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Transform(
        this IImageProcessingContext source,
        ProjectiveTransformBuilder builder)
    {
        return Transform(source, builder, KnownResamplers.Bicubic);
    }

    /// <summary>
    ///     Performs a projective transform of an image using the specified sampling algorithm.
    /// </summary>
    /// <param name="ctx">The <see cref="IImageProcessingContext" />.</param>
    /// <param name="builder">The projective transform builder.</param>
    /// <param name="sampler">The <see cref="IResampler" /> to perform the resampling.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Transform(
        this IImageProcessingContext ctx,
        ProjectiveTransformBuilder builder,
        IResampler sampler)
    {
        return ctx.Transform(new Rectangle(Point.Empty, ctx.GetCurrentSize()), builder, sampler);
    }

    /// <summary>
    ///     Performs a projective transform of an image using the specified sampling algorithm.
    /// </summary>
    /// <param name="ctx">The <see cref="IImageProcessingContext" />.</param>
    /// <param name="sourceRectangle">The source rectangle</param>
    /// <param name="builder">The projective transform builder.</param>
    /// <param name="sampler">The <see cref="IResampler" /> to perform the resampling.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Transform(
        this IImageProcessingContext ctx,
        Rectangle sourceRectangle,
        ProjectiveTransformBuilder builder,
        IResampler sampler)
    {
        var transform = builder.BuildMatrix(sourceRectangle);
        var targetDimensions = TransformUtilities.GetTransformedSize(sourceRectangle.Size, transform);
        return ctx.Transform(sourceRectangle, transform, targetDimensions, sampler);
    }

    /// <summary>
    ///     Performs a projective transform of an image using the specified sampling algorithm.
    /// </summary>
    /// <param name="ctx">The <see cref="IImageProcessingContext" />.</param>
    /// <param name="sourceRectangle">The source rectangle</param>
    /// <param name="transform">The transformation matrix.</param>
    /// <param name="targetDimensions">The size of the result image.</param>
    /// <param name="sampler">The <see cref="IResampler" /> to perform the resampling.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Transform(
        this IImageProcessingContext ctx,
        Rectangle sourceRectangle,
        Matrix4x4 transform,
        Size targetDimensions,
        IResampler sampler)
    {
        return ctx.ApplyProcessor(
            new ProjectiveTransformProcessor(transform, sampler, targetDimensions),
            sourceRectangle);
    }
}