// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <content>
///     Allows conversion to <see cref="Rgb" />.
/// </content>
public partial class ColorSpaceConverter
{
    private static readonly LinearRgbToRgbConverter LinearRgbToRgbConverter = new();

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLab" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLab> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLch" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLch> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLchuv" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLchuv> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLuv" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLuv> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieXyy" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieXyy> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieXyz" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieXyz> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Cmyk" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Cmyk> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Hsv" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Hsv> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Hsl" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Hsl> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="HunterLab" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<HunterLab> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="LinearRgb" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<LinearRgb> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Lms" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Lms> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="YCbCr" /> into <see cref="Rgb" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<YCbCr> source, Span<Rgb> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToRgb(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLab" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in CieLab color)
    {
        var xyzColor = ToCieXyz(color);
        return ToRgb(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="CieLch" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in CieLch color)
    {
        var xyzColor = ToCieXyz(color);
        return ToRgb(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="CieLchuv" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in CieLchuv color)
    {
        var xyzColor = ToCieXyz(color);
        return ToRgb(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="CieLuv" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in CieLuv color)
    {
        var xyzColor = ToCieXyz(color);
        return ToRgb(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="CieXyy" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in CieXyy color)
    {
        var xyzColor = ToCieXyz(color);
        return ToRgb(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="CieXyz" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in CieXyz color)
    {
        // Conversion
        var linear = ToLinearRgb(color);

        // Compand
        return ToRgb(linear);
    }

    /// <summary>
    ///     Converts a <see cref="Cmyk" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in Cmyk color)
    {
        // Conversion
        return CmykAndRgbConverter.Convert(color);
    }

    /// <summary>
    ///     Converts a <see cref="Hsv" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in Hsv color)
    {
        // Conversion
        return HsvAndRgbConverter.Convert(color);
    }

    /// <summary>
    ///     Converts a <see cref="Hsl" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in Hsl color)
    {
        // Conversion
        return HslAndRgbConverter.Convert(color);
    }

    /// <summary>
    ///     Converts a <see cref="HunterLab" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in HunterLab color)
    {
        var xyzColor = ToCieXyz(color);
        return ToRgb(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="LinearRgb" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in LinearRgb color)
    {
        // Conversion
        return LinearRgbToRgbConverter.Convert(color);
    }

    /// <summary>
    ///     Converts a <see cref="Lms" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in Lms color)
    {
        var xyzColor = ToCieXyz(color);
        return ToRgb(xyzColor);
    }

    /// <summary>
    ///     Converts a <see cref="YCbCr" /> into a <see cref="Rgb" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="Rgb" /></returns>
    public Rgb ToRgb(in YCbCr color)
    {
        // Conversion
        var rgb = YCbCrAndRgbConverter.Convert(color);

        // Adaptation
        return Adapt(rgb);
    }
}