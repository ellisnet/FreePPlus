// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using FreePPlus.Imaging.Metadata.Profiles.Exif;
using FreePPlus.Imaging.PixelFormats;

namespace FreePPlus.Imaging.Processing.Processors.Transforms;

//was previously: namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

/// <summary>
///     Adjusts an image so that its orientation is suitable for viewing. Adjustments are based on EXIF metadata embedded
///     in the image.
/// </summary>
/// <typeparam name="TPixel">The pixel format.</typeparam>
internal class AutoOrientProcessor<TPixel> : ImageProcessor<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AutoOrientProcessor{TPixel}" /> class.
    /// </summary>
    /// <param name="configuration">The configuration which allows altering default behaviour or extending the library.</param>
    /// <param name="source">The source <see cref="Image{TPixel}" /> for the current processor instance.</param>
    /// <param name="sourceRectangle">The source area to process for the current processor instance.</param>
    public AutoOrientProcessor(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle)
        : base(configuration, source, sourceRectangle) { }

    /// <inheritdoc />
    protected override void BeforeImageApply()
    {
        var orientation = GetExifOrientation(Source);
        var size = SourceRectangle.Size;
        switch (orientation)
        {
            case OrientationMode.TopRight:
                new FlipProcessor(FlipMode.Horizontal).Execute(Configuration, Source, SourceRectangle);
                break;

            case OrientationMode.BottomRight:
                new RotateProcessor((int)RotateMode.Rotate180, size).Execute(Configuration, Source, SourceRectangle);
                break;

            case OrientationMode.BottomLeft:
                new FlipProcessor(FlipMode.Vertical).Execute(Configuration, Source, SourceRectangle);
                break;

            case OrientationMode.LeftTop:
                new RotateProcessor((int)RotateMode.Rotate90, size).Execute(Configuration, Source, SourceRectangle);
                new FlipProcessor(FlipMode.Horizontal).Execute(Configuration, Source, SourceRectangle);
                break;

            case OrientationMode.RightTop:
                new RotateProcessor((int)RotateMode.Rotate90, size).Execute(Configuration, Source, SourceRectangle);
                break;

            case OrientationMode.RightBottom:
                new FlipProcessor(FlipMode.Vertical).Execute(Configuration, Source, SourceRectangle);
                new RotateProcessor((int)RotateMode.Rotate270, size).Execute(Configuration, Source, SourceRectangle);
                break;

            case OrientationMode.LeftBottom:
                new RotateProcessor((int)RotateMode.Rotate270, size).Execute(Configuration, Source, SourceRectangle);
                break;

            case OrientationMode.Unknown:
            case OrientationMode.TopLeft:
            default:
                break;
        }

        base.BeforeImageApply();
    }

    /// <inheritdoc />
    protected override void OnFrameApply(ImageFrame<TPixel> sourceBase)
    {
        // All processing happens at the image level within BeforeImageApply();
    }

    /// <summary>
    ///     Returns the current EXIF orientation
    /// </summary>
    /// <param name="source">The image to auto rotate.</param>
    /// <returns>The <see cref="OrientationMode" /></returns>
    private static OrientationMode GetExifOrientation(Image<TPixel> source)
    {
        if (source.Metadata.ExifProfile is null) return OrientationMode.Unknown;

        var value = source.Metadata.ExifProfile.GetValue(ExifTag.Orientation);
        if (value is null) return OrientationMode.Unknown;

        OrientationMode orientation;
        if (value.DataType == ExifDataType.Short)
        {
            orientation = (OrientationMode)value.Value;
        }
        else
        {
            orientation = (OrientationMode)Convert.ToUInt16(value.Value);
            source.Metadata.ExifProfile.RemoveValue(ExifTag.Orientation);
        }

        source.Metadata.ExifProfile.SetValue(ExifTag.Orientation, (ushort)OrientationMode.TopLeft);

        return orientation;
    }
}