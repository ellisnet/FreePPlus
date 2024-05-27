// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <content>
///     Allows conversion to <see cref="Hsl" />.
/// </content>
public partial class ColorSpaceConverter
{
    private static readonly HslAndRgbConverter HslAndRgbConverter = new();

    /// <summary>
    ///     Converts a <see cref="CieLab" /> into a <see cref="Hsl" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(in CieLab color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsl(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLab" /> into <see cref="Hsl" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLab> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLch" /> into a <see cref="Hsl" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(in CieLch color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsl(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLch" /> into <see cref="Hsl" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLch> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLchuv" /> into a <see cref="Hsl" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(in CieLchuv color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsl(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLchuv" /> into <see cref="Hsl" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLchuv> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLuv" /> into a <see cref="Hsl" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(in CieLuv color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsl(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLuv" /> into <see cref="Hsl" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLuv> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieXyy" /> into a <see cref="Hsl" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(in CieXyy color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsl(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieXyy" /> into <see cref="Hsl" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieXyy> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieXyz" /> into a <see cref="Hsl" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(in CieXyz color)
    {
        var rgb = ToRgb(color);

        return HslAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieXyz" /> into <see cref="Hsl" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieXyz> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Cmyk" /> into a <see cref="Hsl" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(in Cmyk color)
    {
        var rgb = ToRgb(color);

        return HslAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Cmyk" /> into <see cref="Hsl" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Cmyk> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Hsv" /> into a <see cref="Hsl" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(in Hsv color)
    {
        var rgb = ToRgb(color);

        return HslAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Hsv" /> into <see cref="Hsl" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Hsv> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="HunterLab" /> into a <see cref="Hsl" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(in HunterLab color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsl(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="HunterLab" /> into <see cref="Hsl" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<HunterLab> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="LinearRgb" /> into a <see cref="Hsl" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(in LinearRgb color)
    {
        var rgb = ToRgb(color);

        return HslAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="LinearRgb" /> into <see cref="Hsl" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<LinearRgb> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Lms" /> into a <see cref="Hsl" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(Lms color)
    {
        var xyzColor = ToCieXyz(color);

        return ToHsl(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Lms" /> into <see cref="Hsl" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Lms> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Rgb" /> into a <see cref="Hsl" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(in Rgb color)
    {
        return HslAndRgbConverter.Convert(color);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Rgb" /> into <see cref="Hsl" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Rgb> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="YCbCr" /> into a <see cref="Hsl" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Hsl" /></returns>
    public Hsl ToHsl(in YCbCr color)
    {
        var rgb = ToRgb(color);

        return HslAndRgbConverter.Convert(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="YCbCr" /> into <see cref="Hsl" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<YCbCr> source, Span<Hsl> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToHsl(sp);
        }
    }
}