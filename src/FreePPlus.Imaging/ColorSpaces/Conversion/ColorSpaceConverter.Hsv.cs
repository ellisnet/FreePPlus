// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <content>
///     Allows conversion to <see cref="Hsv" />.
/// </content>
public partial class ColorSpaceConverter
{
    private static readonly HsvAndRgbConverter HsvAndRgbConverter = new();

    /// <summary>
    ///     Converts a <see cref="CieLab" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(in CieLab color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLab" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLab> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLch" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(in CieLch color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLch" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLch> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLchuv" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(in CieLchuv color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLchuv" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLchuv> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLuv" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(in CieLuv color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLuv" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLuv> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieXyy" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(in CieXyy color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieXyy" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieXyy> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieXyz" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(in CieXyz color)
    {
        var rgb = ToRgb(color);

        return HsvAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieXyz" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieXyz> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Cmyk" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(in Cmyk color)
    {
        var rgb = ToRgb(color);

        return HsvAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Cmyk" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Cmyk> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Hsl" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(in Hsl color)
    {
        var rgb = ToRgb(color);

        return HsvAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Hsl" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors.</param>
    public void Convert(ReadOnlySpan<Hsl> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="HunterLab" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(in HunterLab color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="HunterLab" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<HunterLab> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="LinearRgb" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(in LinearRgb color)
    {
        var rgb = ToRgb(color);

        return HsvAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="LinearRgb" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<LinearRgb> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Lms" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(Lms color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Lms" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Lms> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Rgb" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(in Rgb color)
    {
        return HsvAndRgbConverter.Convert(color);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Rgb" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Rgb> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="YCbCr" /> into a <see cref="Hsv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsv" /></returns>
    public Hsv ToHsv(in YCbCr color)
    {
        var rgb = ToRgb(color);

        return HsvAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="YCbCr" /> into <see cref="Hsv" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<YCbCr> source, Span<Hsv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsv(sp);
        }
    }
}