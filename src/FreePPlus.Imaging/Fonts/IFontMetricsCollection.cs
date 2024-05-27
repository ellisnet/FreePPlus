// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Globalization;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Represents a collection of <see cref="FontMetrics" />
/// </summary>
internal interface IFontMetricsCollection : IReadOnlyFontMetricsCollection
{
    /// <summary>
    ///     Adds the font metrics and culture to the <see cref="IFontMetricsCollection" />.
    /// </summary>
    /// <param name="metrics">The font metrics to add.</param>
    /// <param name="culture">The culture of the font metrics to add.</param>
    /// <returns>The new <see cref="FontFamily" />.</returns>
    FontFamily AddMetrics(FontMetrics metrics, CultureInfo culture);

    /// <summary>
    ///     Adds the font metrics to the <see cref="IFontMetricsCollection" />.
    /// </summary>
    /// <param name="metrics">The font metrics to add.</param>
    void AddMetrics(FontMetrics metrics);
}