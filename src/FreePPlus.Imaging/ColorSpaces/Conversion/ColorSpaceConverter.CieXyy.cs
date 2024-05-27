// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <content>
///     Allows conversion to <see cref="CieXyy" />.
/// </content>
public partial class ColorSpaceConverter
{
    private static readonly CieXyzAndCieXyyConverter CieXyzAndCieXyyConverter = new();

    /// <summary>
    ///     Converts a <see cref="CieLab" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(in CieLab color)
    {
        var xyzColor = ToCieXyz(color);

        return ToCieXyy(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLab" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLab> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLch" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(in CieLch color)
    {
        var xyzColor = ToCieXyz(color);

        return ToCieXyy(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLch" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLch> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLchuv" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(in CieLchuv color)
    {
        var xyzColor = ToCieXyz(color);

        return ToCieXyy(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLchuv" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLchuv> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLuv" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(in CieLuv color)
    {
        var xyzColor = ToCieXyz(color);

        return ToCieXyy(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLuv" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLuv> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieXyz" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(in CieXyz color)
    {
        return CieXyzAndCieXyyConverter.Convert(color);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieXyz" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieXyz> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Cmyk" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(in Cmyk color)
    {
        var xyzColor = ToCieXyz(color);

        return ToCieXyy(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Cmyk" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Cmyk> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Hsl" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(Hsl color)
    {
        var xyzColor = ToCieXyz(color);

        return ToCieXyy(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Hsl" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Hsl> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Hsv" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(in Hsv color)
    {
        var xyzColor = ToCieXyz(color);

        return ToCieXyy(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Hsv" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Hsv> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="HunterLab" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(in HunterLab color)
    {
        var xyzColor = ToCieXyz(color);

        return ToCieXyy(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="HunterLab" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<HunterLab> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="LinearRgb" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(in LinearRgb color)
    {
        var xyzColor = ToCieXyz(color);

        return ToCieXyy(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="LinearRgb" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<LinearRgb> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Lms" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(in Lms color)
    {
        var xyzColor = ToCieXyz(color);

        return ToCieXyy(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Lms" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Lms> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Rgb" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(in Rgb color)
    {
        var xyzColor = ToCieXyz(color);

        return ToCieXyy(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Rgb" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Rgb> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="YCbCr" /> into a <see cref="CieXyy" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyy" /></returns>
    public CieXyy ToCieXyy(in YCbCr color)
    {
        var xyzColor = ToCieXyz(color);

        return ToCieXyy(xyzColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="YCbCr" /> into <see cref="CieXyy" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<YCbCr> source, Span<CieXyy> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyy(sp);
        }
    }
}