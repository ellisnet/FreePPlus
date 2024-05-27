// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <content>
///     Allows conversion to <see cref="CieLuv" />.
/// </content>
public partial class ColorSpaceConverter
{
    private static readonly CieLchuvToCieLuvConverter CieLchuvToCieLuvConverter = new();

    /// <summary>
    ///     Converts a <see cref="CieLab" /> into a <see cref="CieLuv" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLuv" /></returns>
    public CieLuv ToCieLuv(in CieLab color)
    {
        var xyzColor = ToCieXyz(color);
        return ToCieLuv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLab" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLab> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLch" /> into a <see cref="CieLuv" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLuv" /></returns>
    public CieLuv ToCieLuv(in CieLch color)
    {
        var xyzColor = ToCieXyz(color);
        return ToCieLuv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLch" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLch> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLchuv" /> into a <see cref="CieLuv" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLab" /></returns>
    public CieLuv ToCieLuv(in CieLchuv color)
    {
        // Conversion (preserving white point)
        var unadapted = CieLchuvToCieLuvConverter.Convert(color);

        // Adaptation
        return Adapt(unadapted);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLchuv" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLchuv> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieXyy" /> into a <see cref="CieLuv" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLuv" /></returns>
    public CieLuv ToCieLuv(in CieXyy color)
    {
        var xyzColor = ToCieXyz(color);
        return ToCieLuv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieXyy" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieXyy> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieXyz" /> into a <see cref="CieLuv" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLuv" /></returns>
    public CieLuv ToCieLuv(in CieXyz color)
    {
        // Adaptation
        var adapted = Adapt(color, _whitePoint, _targetLuvWhitePoint);

        // Conversion
        return _cieXyzToCieLuvConverter.Convert(adapted);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieXyz" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieXyz> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Cmyk" /> into a <see cref="CieLuv" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLuv" /></returns>
    public CieLuv ToCieLuv(in Cmyk color)
    {
        var xyzColor = ToCieXyz(color);
        return ToCieLuv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Cmyk" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Cmyk> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Hsl" /> into a <see cref="CieLuv" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLuv" /></returns>
    public CieLuv ToCieLuv(in Hsl color)
    {
        var xyzColor = ToCieXyz(color);
        return ToCieLuv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Hsl" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Hsl> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Hsv" /> into a <see cref="CieLuv" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLuv" /></returns>
    public CieLuv ToCieLuv(in Hsv color)
    {
        var xyzColor = ToCieXyz(color);
        return ToCieLuv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Hsv" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Hsv> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="HunterLab" /> into a <see cref="CieLuv" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLuv" /></returns>
    public CieLuv ToCieLuv(in HunterLab color)
    {
        var xyzColor = ToCieXyz(color);
        return ToCieLuv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="HunterLab" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<HunterLab> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Lms" /> into a <see cref="CieLuv" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLuv" /></returns>
    public CieLuv ToCieLuv(in Lms color)
    {
        var xyzColor = ToCieXyz(color);
        return ToCieLuv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Lms" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Lms> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="LinearRgb" /> into a <see cref="CieLuv" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLuv" /></returns>
    public CieLuv ToCieLuv(in LinearRgb color)
    {
        var xyzColor = ToCieXyz(color);
        return ToCieLuv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="LinearRgb" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<LinearRgb> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Rgb" /> into a <see cref="CieLuv" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLuv" /></returns>
    public CieLuv ToCieLuv(in Rgb color)
    {
        var xyzColor = ToCieXyz(color);
        return ToCieLuv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Rgb" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Rgb> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="YCbCr" /> into a <see cref="CieLuv" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieLuv" /></returns>
    public CieLuv ToCieLuv(in YCbCr color)
    {
        var xyzColor = ToCieXyz(color);
        return ToCieLuv(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="YCbCr" /> into <see cref="CieLuv" />.
    /// </summary>
    /// <param name="source">The span to the source colors.</param>
    /// <param name="destination">The span to the destination colors.</param>
    public void Convert(ReadOnlySpan<YCbCr> source, Span<CieLuv> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieLuv(sp);
        }
    }
}