// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.PixelFormats;
using FreePPlus.Imaging.Processing.Processors.Effects;

namespace FreePPlus.Imaging.Processing;

//was previously: namespace SixLabors.ImageSharp.Processing;

/// <summary>
///     Defines extension methods that allow the application of user defined processing delegate to an <see cref="Image" />
///     .
/// </summary>
public static class PixelRowDelegateExtensions
{
    /// <summary>
    ///     Applies a user defined processing delegate to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rowOperation">The user defined processing delegate to use to modify image rows.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source,
        PixelRowOperation rowOperation)
    {
        return ProcessPixelRowsAsVector4(source, rowOperation, PixelConversionModifiers.None);
    }

    /// <summary>
    ///     Applies a user defined processing delegate to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rowOperation">The user defined processing delegate to use to modify image rows.</param>
    /// <param name="modifiers">The <see cref="PixelConversionModifiers" /> to apply during the pixel conversions.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source,
        PixelRowOperation rowOperation, PixelConversionModifiers modifiers)
    {
        return source.ApplyProcessor(new PixelRowDelegateProcessor(rowOperation, modifiers));
    }

    /// <summary>
    ///     Applies a user defined processing delegate to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rowOperation">The user defined processing delegate to use to modify image rows.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source,
        PixelRowOperation rowOperation, Rectangle rectangle)
    {
        return ProcessPixelRowsAsVector4(source, rowOperation, rectangle, PixelConversionModifiers.None);
    }

    /// <summary>
    ///     Applies a user defined processing delegate to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rowOperation">The user defined processing delegate to use to modify image rows.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <param name="modifiers">The <see cref="PixelConversionModifiers" /> to apply during the pixel conversions.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source,
        PixelRowOperation rowOperation, Rectangle rectangle, PixelConversionModifiers modifiers)
    {
        return source.ApplyProcessor(new PixelRowDelegateProcessor(rowOperation, modifiers), rectangle);
    }

    /// <summary>
    ///     Applies a user defined processing delegate to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rowOperation">The user defined processing delegate to use to modify image rows.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source,
        PixelRowOperation<Point> rowOperation)
    {
        return ProcessPixelRowsAsVector4(source, rowOperation, PixelConversionModifiers.None);
    }

    /// <summary>
    ///     Applies a user defined processing delegate to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rowOperation">The user defined processing delegate to use to modify image rows.</param>
    /// <param name="modifiers">The <see cref="PixelConversionModifiers" /> to apply during the pixel conversions.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source,
        PixelRowOperation<Point> rowOperation, PixelConversionModifiers modifiers)
    {
        return source.ApplyProcessor(new PositionAwarePixelRowDelegateProcessor(rowOperation, modifiers));
    }

    /// <summary>
    ///     Applies a user defined processing delegate to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rowOperation">The user defined processing delegate to use to modify image rows.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source,
        PixelRowOperation<Point> rowOperation, Rectangle rectangle)
    {
        return ProcessPixelRowsAsVector4(source, rowOperation, rectangle, PixelConversionModifiers.None);
    }

    /// <summary>
    ///     Applies a user defined processing delegate to the image.
    /// </summary>
    /// <param name="source">The image this method extends.</param>
    /// <param name="rowOperation">The user defined processing delegate to use to modify image rows.</param>
    /// <param name="rectangle">
    ///     The <see cref="Rectangle" /> structure that specifies the portion of the image object to alter.
    /// </param>
    /// <param name="modifiers">The <see cref="PixelConversionModifiers" /> to apply during the pixel conversions.</param>
    /// <returns>The <see cref="IImageProcessingContext" /> to allow chaining of operations.</returns>
    public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source,
        PixelRowOperation<Point> rowOperation, Rectangle rectangle, PixelConversionModifiers modifiers)
    {
        return source.ApplyProcessor(new PositionAwarePixelRowDelegateProcessor(rowOperation, modifiers), rectangle);
    }
}