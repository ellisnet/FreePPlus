// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     A surface that can have a glyph rendered to it as a series of actions, where the engine support colored glyphs
///     (emoji).
/// </summary>
public interface IColorGlyphRenderer : IGlyphRenderer
{
    /// <summary>
    ///     Sets the color to use for the current glyph.
    /// </summary>
    /// <param name="color">The color to override the renders brush with.</param>
    void SetColor(GlyphColor color);
}

/// <summary>
///     Provides access to the color details for the current glyph.
/// </summary>
public readonly struct GlyphColor
{
    internal GlyphColor(byte blue, byte green, byte red, byte alpha)
    {
        Blue = blue;
        Green = green;
        Red = red;
        Alpha = alpha;
    }

    /// <summary>
    ///     Gets the blue component
    /// </summary>
    public readonly byte Blue { get; }

    /// <summary>
    ///     Gets the green component
    /// </summary>
    public readonly byte Green { get; }

    /// <summary>
    ///     Gets the red component
    /// </summary>
    public readonly byte Red { get; }

    /// <summary>
    ///     Gets the alpha component
    /// </summary>
    public readonly byte Alpha { get; }

    /// <summary>
    ///     Compares two <see cref="GlyphColor" /> objects for equality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="GlyphColor" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="GlyphColor" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the current left is equal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    public static bool operator ==(GlyphColor left, GlyphColor right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="GlyphColor" /> objects for inequality.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="GlyphColor" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="GlyphColor" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the current left is unequal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    public static bool operator !=(GlyphColor left, GlyphColor right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is GlyphColor p && Equals(p);
    }

    /// <summary>
    ///     Compares the <see cref="GlyphColor" /> for equality to this color.
    /// </summary>
    /// <param name="other">
    ///     The other <see cref="GlyphColor" /> to compare to.
    /// </param>
    /// <returns>
    ///     True if the current color is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    public bool Equals(GlyphColor other)
    {
        return other.Red == Red
               && other.Green == Green
               && other.Blue == Blue
               && other.Alpha == Alpha;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Red,
            Green,
            Blue,
            Alpha);
    }
}