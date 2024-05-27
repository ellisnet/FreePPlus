// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Globalization;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     A string with a specific locale.
/// </summary>
internal readonly struct IccLocalizedString : IEquatable<IccLocalizedString>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccLocalizedString" /> struct.
    ///     The culture will be <see cref="CultureInfo.CurrentCulture" />
    /// </summary>
    /// <param name="text">The text value of this string</param>
    public IccLocalizedString(string text)
        : this(CultureInfo.CurrentCulture, text) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccLocalizedString" /> struct.
    ///     The culture will be <see cref="CultureInfo.CurrentCulture" />
    /// </summary>
    /// <param name="culture">The culture of this string</param>
    /// <param name="text">The text value of this string</param>
    public IccLocalizedString(CultureInfo culture, string text)
    {
        Culture = culture ?? throw new ArgumentNullException(nameof(culture));
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    /// <summary>
    ///     Gets the text value.
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     Gets the culture of text.
    /// </summary>
    public CultureInfo Culture { get; }

    /// <inheritdoc />
    public bool Equals(IccLocalizedString other)
    {
        return Culture.Equals(other.Culture) &&
               Text == other.Text;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Culture.Name}: {Text}";
    }
}