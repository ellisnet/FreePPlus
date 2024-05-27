// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <content>
///     Allows conversion to <see cref="YCbCr" />.
/// </content>
public partial class ColorSpaceConverter
{
    private static readonly YCbCrAndRgbConverter YCbCrAndRgbConverter = new();

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLab" /> into <see cref="YCbCr" />.
    /// </summary>
    /// <param name="source">The span to the source colors.</param>
    /// <param name="destination">The span to the destination colors.</param>
    public void Convert(ReadOnlySpan<CieLab> source, Span<YCbCr> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToYCbCr(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLch" /> into <see cref="YCbCr" />.
    /// </summary>
    /// <param name="source">The span to the source colors.</param>
    /// <param name="destination">The span to the destination colors.</param>
    public void Convert(ReadOnlySpan<CieLch> source, Span<YCbCr> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToYCbCr(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLuv" /> into <see cref="YCbCr" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLuv> source, Span<YCbCr> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToYCbCr(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieXyy" /> into <see cref="YCbCr" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieXyy> source, Span<YCbCr> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToYCbCr(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieXyz" /> into <see cref="YCbCr" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieXyz> source, Span<YCbCr> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToYCbCr(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Cmyk" /> into <see cref="YCbCr" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Cmyk> source, Span<YCbCr> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToYCbCr(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Hsl" /> into <see cref="YCbCr" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Hsl> source, Span<YCbCr> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToYCbCr(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Hsv" /> into <see cref="YCbCr" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Hsv> source, Span<YCbCr> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToYCbCr(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="HunterLab" /> into <see cref="YCbCr" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<HunterLab> source, Span<YCbCr> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToYCbCr(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="LinearRgb" /> into <see cref="YCbCr" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<LinearRgb> source, Span<YCbCr> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToYCbCr(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Lms" /> into <see cref="YCbCr" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Lms> source, Span<YCbCr> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToYCbCr(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Rgb" /> into <see cref="YCbCr" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Rgb> source, Span<YCbCr> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToYCbCr(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLab" /> into a <see cref="YCbCr" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="YCbCr" /></returns>
    public YCbCr ToYCbCr(in CieLab color)
    {
        var xyzColor = ToCieXyz(color);

        return ToYCbCr(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="CieLch" /> into a <see cref="YCbCr" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="YCbCr" /></returns>
    public YCbCr ToYCbCr(in CieLch color)
    {
        var xyzColor = ToCieXyz(color);

        return ToYCbCr(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="CieLuv" /> into a <see cref="YCbCr" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="YCbCr" /></returns>
    public YCbCr ToYCbCr(in CieLuv color)
    {
        var xyzColor = ToCieXyz(color);

        return ToYCbCr(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="CieXyy" /> into a <see cref="YCbCr" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="YCbCr" /></returns>
    public YCbCr ToYCbCr(in CieXyy color)
    {
        var xyzColor = ToCieXyz(color);

        return ToYCbCr(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="CieXyz" /> into a <see cref="YCbCr" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="YCbCr" /></returns>
    public YCbCr ToYCbCr(in CieXyz color)
    {
        var rgb = ToRgb(color);

        return YCbCrAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Converts a <see cref="Cmyk" /> into a <see cref="YCbCr" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="YCbCr" /></returns>
    public YCbCr ToYCbCr(in Cmyk color)
    {
        var rgb = ToRgb(color);

        return YCbCrAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Converts a <see cref="Hsl" /> into a <see cref="YCbCr" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="YCbCr" /></returns>
    public YCbCr ToYCbCr(in Hsl color)
    {
        var rgb = ToRgb(color);

        return YCbCrAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Converts a <see cref="Hsv" /> into a <see cref="YCbCr" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="YCbCr" /></returns>
    public YCbCr ToYCbCr(in Hsv color)
    {
        var rgb = ToRgb(color);

        return YCbCrAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Converts a <see cref="HunterLab" /> into a <see cref="YCbCr" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="YCbCr" /></returns>
    public YCbCr ToYCbCr(in HunterLab color)
    {
        var xyzColor = ToCieXyz(color);

        return ToYCbCr(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="LinearRgb" /> into a <see cref="YCbCr" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="YCbCr" /></returns>
    public YCbCr ToYCbCr(in LinearRgb color)
    {
        var rgb = ToRgb(color);

        return YCbCrAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Converts a <see cref="Lms" /> into a <see cref="YCbCr" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="YCbCr" /></returns>
    public YCbCr ToYCbCr(in Lms color)
    {
        var xyzColor = ToCieXyz(color);

        return ToYCbCr(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="Rgb" /> into a <see cref="YCbCr" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="YCbCr" /></returns>
    public YCbCr ToYCbCr(in Rgb color)
    {
        return YCbCrAndRgbConverter.Convert(color);
    }
}