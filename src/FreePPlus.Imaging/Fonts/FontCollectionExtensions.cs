// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Extension methods for <see cref="IFontCollection" />.
/// </summary>
public static class FontCollectionExtensions
{
    /// <summary>
    ///     Adds the fonts from the <see cref="SystemFonts" /> collection to this <see cref="FontCollection" />.
    /// </summary>
    /// <param name="collection">The font collection.</param>
    /// <returns>The <see cref="FontCollection" /> containing the system fonts.</returns>
    public static FontCollection AddSystemFonts(this FontCollection collection)
    {
        // This cast is safe because our underlying SystemFontCollection implements
        // both interfaces separately.
        foreach (var metric in (IReadOnlyFontMetricsCollection)SystemFonts.Collection)
            ((IFontMetricsCollection)collection).AddMetrics(metric);

        collection.AddSearchDirectories(SystemFonts.Collection.SearchDirectories);

        return collection;
    }

    /// <summary>
    ///     Adds the fonts from the <see cref="SystemFonts" /> collection to this <see cref="FontCollection" />.
    /// </summary>
    /// <param name="collection">The font collection.</param>
    /// <param name="match">
    ///     The <see cref="Predicate{T}" /> delegate that defines the conditions of <see cref="FontMetrics" />
    ///     to add into the font collection.
    /// </param>
    /// <returns>The <see cref="FontCollection" /> containing the system fonts.</returns>
    public static FontCollection AddSystemFonts(this FontCollection collection, Predicate<FontMetrics> match)
    {
        var isMatch = false;
        foreach (var metric in (IReadOnlyFontMetricsCollection)SystemFonts.Collection)
        {
            var currentMatch = match(metric);
            isMatch |= currentMatch;
            if (currentMatch) ((IFontMetricsCollection)collection).AddMetrics(metric);
        }

        if (isMatch) collection.AddSearchDirectories(SystemFonts.Collection.SearchDirectories);

        return collection;
    }
}