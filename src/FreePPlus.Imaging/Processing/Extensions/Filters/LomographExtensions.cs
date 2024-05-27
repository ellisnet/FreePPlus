// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Processing.Processors.Filters;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines extensions that allow the recreation of an old Lomograph camera effect on an <see cref="Image" />
///     using Mutate/Clone.
/// </summary>
public static class LomographExtensions
{
    /// <summary>
    ///     Alters the colors of the image recreating an old Lomograph camera effect.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Lomograph(this IImageProcessingContext source)
    {
        return source.ApplyProcessor(new LomographProcessor(source.GetGraphicsOptions()));
    }

    /// <summary>
    ///     Alters the colors of the image recreating an old Lomograph camera effect.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext Lomograph(this IImageProcessingContext source, Rectangle rectangle)
    {
        return source.ApplyProcessor(new LomographProcessor(source.GetGraphicsOptions()), rectangle);
    }
}