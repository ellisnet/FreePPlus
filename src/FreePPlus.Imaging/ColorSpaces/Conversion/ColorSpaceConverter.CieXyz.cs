// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <content>
///     Allows conversion to <see cref="CieXyz" />.
/// </content>
public partial class ColorSpaceConverter
{
    private static readonly CieLabToCieXyzConverter CieLabToCieXyzConverter = new();

    private static readonly CieLuvToCieXyzConverter CieLuvToCieXyzConverter = new();

    private static readonly HunterLabToCieXyzConverter
        HunterLabToCieXyzConverter = new();

    private LinearRgbToCieXyzConverter _linearRgbToCieXyzConverter;

    /// <summary>
    ///     Converts a <see cref="CieLab" /> into a <see cref="CieXyz" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in CieLab color)
    {
        // Conversion
        var unadapted = CieLabToCieXyzConverter.Convert(color);

        // Adaptation
        return Adapt(unadapted, color.WhitePoint);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLab" /> into <see cref="CieXyz" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLab> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLch" /> into a <see cref="CieXyz" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in CieLch color)
    {
        // Conversion to Lab
        var labColor = CieLchToCieLabConverter.Convert(color);

        // Conversion to XYZ (incl. adaptation)
        return ToCieXyz(labColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLch" /> into <see cref="CieXyz" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLch> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLuv" /> into a <see cref="CieXyz" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in CieLchuv color)
    {
        // Conversion to Luv
        var luvColor = CieLchuvToCieLuvConverter.Convert(color);

        // Conversion to XYZ (incl. adaptation)
        return ToCieXyz(luvColor);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLchuv" /> into <see cref="CieXyz" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLchuv> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieLuv" /> into a <see cref="CieXyz" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in CieLuv color)
    {
        // Conversion
        var unadapted = CieLuvToCieXyzConverter.Convert(color);

        // Adaptation
        return Adapt(unadapted, color.WhitePoint);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieLuv" /> into <see cref="CieXyz" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieLuv> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="CieXyy" /> into a <see cref="CieXyz" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in CieXyy color)
    {
        // Conversion
        return CieXyzAndCieXyyConverter.Convert(color);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="CieXyy" /> into <see cref="CieXyz" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<CieXyy> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Cmyk" /> into a <see cref="CieXyz" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in Cmyk color)
    {
        var rgb = ToRgb(color);

        return ToCieXyz(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Cmyk" /> into <see cref="CieXyz" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Cmyk> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Hsl" /> into a <see cref="CieXyz" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in Hsl color)
    {
        var rgb = ToRgb(color);

        return ToCieXyz(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Hsl" /> into <see cref="CieXyz" />.
    /// </summary>
    /// <param name="source">The span to the source colors.</param>
    /// <param name="destination">The span to the destination colors.</param>
    public void Convert(ReadOnlySpan<Hsl> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Hsv" /> into a <see cref="CieXyz" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in Hsv color)
    {
        // Conversion
        var rgb = ToRgb(color);

        return ToCieXyz(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Hsv" /> into <see cref="CieXyz" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Hsv> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="HunterLab" /> into a <see cref="CieXyz" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in HunterLab color)
    {
        var unadapted = HunterLabToCieXyzConverter.Convert(color);

        return Adapt(unadapted, color.WhitePoint);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="HunterLab" /> into <see cref="CieXyz" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<HunterLab> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="LinearRgb" /> into a <see cref="CieXyz" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in LinearRgb color)
    {
        // Conversion
        var converter = GetLinearRgbToCieXyzConverter(color.WorkingSpace);
        var unadapted = converter.Convert(color);

        return Adapt(unadapted, color.WorkingSpace.WhitePoint);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="LinearRgb" /> into <see cref="CieXyz" />.
    /// </summary>
    /// <param name="source">The span to the source colors.</param>
    /// <param name="destination">The span to the destination colors.</param>
    public void Convert(ReadOnlySpan<LinearRgb> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Lms" /> into a <see cref="CieXyz" />.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in Lms color)
    {
        return _cieXyzAndLmsConverter.Convert(color);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Lms" /> into <see cref="CieXyz" />.
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Lms> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="Rgb" /> into a <see cref="CieXyz" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in Rgb color)
    {
        // Conversion
        var linear = RgbToLinearRgbConverter.Convert(color);
        return ToCieXyz(linear);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="Rgb" /> into <see cref="CieXyz" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<Rgb> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Converts a <see cref="YCbCr" /> into a <see cref="CieXyz" />
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The <see cref="CieXyz" /></returns>
    public CieXyz ToCieXyz(in YCbCr color)
    {
        var rgb = ToRgb(color);

        return ToCieXyz(rgb);
    }

    /// <summary>
    ///     Performs the bulk conversion from <see cref="YCbCr" /> into <see cref="CieXyz" />
    /// </summary>
    /// <param name="source">The span to the source colors</param>
    /// <param name="destination">The span to the destination colors</param>
    public void Convert(ReadOnlySpan<YCbCr> source, Span<CieXyz> destination)
    {
        Guard.DestinationShouldNotBeTooShort(source, destination, nameof(destination));
        var count = source.Length;

        ref var sourceRef = ref MemoryMarshal.GetReference(source);
        ref var destRef = ref MemoryMarshal.GetReference(destination);

        for (var i = 0; i < count; i++)
        {
            ref var sp = ref Unsafe.Add(ref sourceRef, i);
            ref var dp = ref Unsafe.Add(ref destRef, i);
            dp = ToCieXyz(sp);
        }
    }

    /// <summary>
    ///     Gets the correct converter for the given rgb working space.
    /// </summary>
    /// <param name="workingSpace">The source working space</param>
    /// <returns>The <see cref="LinearRgbToCieXyzConverter" /></returns>
    private LinearRgbToCieXyzConverter GetLinearRgbToCieXyzConverter(RgbWorkingSpace workingSpace)
    {
        if (_linearRgbToCieXyzConverter?.SourceWorkingSpace.Equals(workingSpace) == true)
            return _linearRgbToCieXyzConverter;

        return _linearRgbToCieXyzConverter = new LinearRgbToCieXyzConverter(workingSpace);
    }
}