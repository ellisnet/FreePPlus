// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace FreePPlus.Imaging.ColorSpaces.Conversion;

//was previously: namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

/// <summary>
///     Color converter between HSV and Rgb
///     See <see href="http://www.poynton.com/PDFs/coloureq.pdf" /> for formulas.
/// </summary>
internal sealed class HsvAndRgbConverter
{
    /// <summary>
    ///     Performs the conversion from the <see cref="Hsv" /> input to an instance of <see cref="Rgb" /> type.
    /// </summary>
    /// <param name="input">The input color instance.</param>
    /// <returns>The converted result</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Rgb Convert(in Hsv input)
    {
        var s = input.S;
        var v = input.V;

        if (MathF.Abs(s) < Constants.Epsilon) return new Rgb(v, v, v);

        var h = MathF.Abs(input.H - 360) < Constants.Epsilon ? 0 : input.H / 60;
        var i = (int)Math.Truncate(h);
        var f = h - i;

        var p = v * (1F - s);
        var q = v * (1F - s * f);
        var t = v * (1F - s * (1F - f));

        float r, g, b;
        switch (i)
        {
            case 0:
                r = v;
                g = t;
                b = p;
                break;

            case 1:
                r = q;
                g = v;
                b = p;
                break;

            case 2:
                r = p;
                g = v;
                b = t;
                break;

            case 3:
                r = p;
                g = q;
                b = v;
                break;

            case 4:
                r = t;
                g = p;
                b = v;
                break;

            default:
                r = v;
                g = p;
                b = q;
                break;
        }

        return new Rgb(r, g, b);
    }

    /// <summary>
    ///     Performs the conversion from the <see cref="Rgb" /> input to an instance of <see cref="Hsv" /> type.
    /// </summary>
    /// <param name="input">The input color instance.</param>
    /// <returns>The converted result</returns>
    [MethodImpl(InliningOptions.ShortMethod)]
    public Hsv Convert(in Rgb input)
    {
        var r = input.R;
        var g = input.G;
        var b = input.B;

        var max = MathF.Max(r, MathF.Max(g, b));
        var min = MathF.Min(r, MathF.Min(g, b));
        var chroma = max - min;
        float h = 0;
        float s = 0;
        var v = max;

        if (MathF.Abs(chroma) < Constants.Epsilon) return new Hsv(0, s, v);

        if (MathF.Abs(r - max) < Constants.Epsilon)
            h = (g - b) / chroma;
        else if (MathF.Abs(g - max) < Constants.Epsilon)
            h = 2 + (b - r) / chroma;
        else if (MathF.Abs(b - max) < Constants.Epsilon) h = 4 + (r - g) / chroma;

        h *= 60;
        if (h < 0.0) h += 360;

        s = chroma / v;

        return new Hsv(h, s, v);
    }
}