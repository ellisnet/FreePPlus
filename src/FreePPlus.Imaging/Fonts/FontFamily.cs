// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#pragma warning disable IDE0251
#pragma warning disable IDE0250
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Defines a group of type faces having a similar basic design and certain
///     variations in styles.
/// </summary>
public struct FontFamily : IEquatable<FontFamily>
{
    private readonly IReadOnlyFontMetricsCollection collection;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FontFamily" /> struct.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="collection">The collection.</param>
    /// <param name="culture">The culture the family was extracted against</param>
    internal FontFamily(string name, IReadOnlyFontMetricsCollection collection, CultureInfo culture)
    {
        Guard.NotNull(collection, nameof(collection));

        this.collection = collection;
        Name = name;
        Culture = culture;
    }

    /// <summary>
    ///     Gets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the culture this instance was extracted against.
    /// </summary>
    public CultureInfo Culture { get; }

    /// <summary>
    ///     Compares two <see cref="FontFamily" /> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="FontFamily" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="FontFamily" /> on the right side of the operand.</param>
    /// <returns>
    ///     <see langword="true" /> if the current left is equal to the <paramref name="right" />
    ///     parameter; otherwise, <see langword="false" />.
    /// </returns>
    public static bool operator ==(FontFamily left, FontFamily right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="FontFamily" /> objects for inequality.
    /// </summary>
    /// <param name="left">The <see cref="FontFamily" /> on the left side of the operand.</param>
    /// <param name="right">The <see cref="FontFamily" /> on the right side of the operand.</param>
    /// <returns>
    ///     <see langword="true" /> if the current left is unequal to the <paramref name="right" />
    ///     parameter; otherwise, <see langword="false" />.
    /// </returns>
    public static bool operator !=(FontFamily left, FontFamily right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Font" /> for the named font family with regular styling.
    /// </summary>
    /// <param name="size">The size of the font in PT units.</param>
    /// <returns>The new <see cref="Font" />.</returns>
    public Font CreateFont(float size)
    {
        if (this == default) FontsThrowHelper.ThrowDefaultInstance();

        return new Font(this, size);
    }

    /// <summary>
    ///     Create a new instance of the <see cref="Font" /> for the named font family.
    /// </summary>
    /// <param name="size">The size of the font in PT units.</param>
    /// <param name="style">The font style.</param>
    /// <returns>The new <see cref="Font" />.</returns>
    public Font CreateFont(float size, FontStyle style)
    {
        if (this == default) FontsThrowHelper.ThrowDefaultInstance();

        return new Font(this, size, style);
    }

    /// <summary>
    ///     Gets the collection of <see cref="FontStyle" /> that are currently available.
    /// </summary>
    /// <returns>The <see cref="IEnumerable{T}" />.</returns>
    public IEnumerable<FontStyle> GetAvailableStyles()
    {
        if (this == default) FontsThrowHelper.ThrowDefaultInstance();

        return collection.GetAllStyles(Name, Culture);
    }

    /// <summary>
    ///     Gets the collection of filesystem paths to the font family sources.
    /// </summary>
    /// <param name="paths">
    ///     When this method returns, contains the filesystem paths to the font family sources,
    ///     if the path exists; otherwise, an empty value for the type of the paths parameter.
    ///     This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if the <see cref="FontFamily" /> was created via filesystem paths; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    public bool TryGetPaths(out IEnumerable<string> paths)
    {
        if (this == default) FontsThrowHelper.ThrowDefaultInstance();

        var filePaths = new List<string>();
        foreach (var style in GetAvailableStyles())
            if (collection.TryGetMetrics(Name, Culture, style, out var metrics)
                && metrics is FileFontMetrics fileMetrics)
                filePaths.Add(fileMetrics.Path);

        paths = filePaths;
        return filePaths.Count > 0;
    }

    /// <summary>
    ///     Gets the specified font metrics matching the given font style.
    /// </summary>
    /// <param name="style">The font style to use when searching for a match.</param>
    /// <param name="metrics">
    ///     When this method returns, contains the metrics associated with the specified name,
    ///     if the name is found; otherwise, the default value for the type of the metrics parameter.
    ///     This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if the <see cref="FontFamily" /> contains font metrics
    ///     with the specified name; otherwise, <see langword="false" />.
    /// </returns>
    internal bool TryGetMetrics(FontStyle style, [NotNullWhen(true)] out FontMetrics? metrics)
    {
        if (this == default) FontsThrowHelper.ThrowDefaultInstance();

        return collection.TryGetMetrics(Name, Culture, style, out metrics);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is FontFamily family && Equals(family);
    }

    /// <inheritdoc />
    public bool Equals(FontFamily other)
    {
        var comparer = StringComparerHelpers.GetCaseInsensitiveStringComparer(Culture);
        return comparer.Equals(Name, other.Name)
               && EqualityComparer<CultureInfo>.Default.Equals(Culture, other.Culture)
               && EqualityComparer<IReadOnlyFontMetricsCollection>.Default.Equals(collection, other.collection);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(collection, Name, Culture);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}

public static class FontHelper
{
    public static FontFamily GetFontByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or empty.", nameof(name));

        return SystemFonts.Get(name);
    }
}