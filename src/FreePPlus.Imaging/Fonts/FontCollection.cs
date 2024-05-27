// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using FreePPlus.Imaging.Fonts.Tables;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Represents a collection of font families.
/// </summary>
public sealed class FontCollection : IFontCollection, IFontMetricsCollection
{
    private readonly HashSet<FontMetrics> metricsCollection = new();
    private readonly HashSet<string> searchDirectories = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="FontCollection" /> class.
    /// </summary>
    public FontCollection()
        : this(Array.Empty<string>()) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FontCollection" /> class.
    /// </summary>
    /// <param name="searchDirectories">The collection of directories used to search for font families.</param>
    /// <remarks>
    ///     Use this constructor instead of the parameterless constructor if the fonts added to that collection
    ///     are actually added after searching inside physical file system directories. The message of the
    ///     <see cref="FontFamilyNotFoundException" /> will include the searched directories.
    /// </remarks>
    internal FontCollection(IReadOnlyCollection<string> searchDirectories)
    {
        Guard.NotNull(searchDirectories, nameof(searchDirectories));
        foreach (var dir in searchDirectories) this.searchDirectories.Add(dir);
    }

    /// <inheritdoc />
    public IEnumerable<FontFamily> Families => FamiliesByCultureImpl(CultureInfo.InvariantCulture);

    /// <inheritdoc />
    public FontFamily Add(string path)
    {
        return Add(path, out _);
    }

    /// <inheritdoc />
    public FontFamily Add(string path, out FontDescription description)
    {
        return AddImpl(path, CultureInfo.InvariantCulture, out description);
    }

    /// <inheritdoc />
    public FontFamily Add(Stream stream)
    {
        return Add(stream, out _);
    }

    /// <inheritdoc />
    public FontFamily Add(Stream stream, out FontDescription description)
    {
        return AddImpl(stream, CultureInfo.InvariantCulture, out description);
    }

    /// <inheritdoc />
    public IEnumerable<FontFamily> AddCollection(string path)
    {
        return AddCollection(path, out _);
    }

    /// <inheritdoc />
    public IEnumerable<FontFamily> AddCollection(string path, out IEnumerable<FontDescription> descriptions)
    {
        return AddCollectionImpl(path, CultureInfo.InvariantCulture, out descriptions);
    }

    /// <inheritdoc />
    public IEnumerable<FontFamily> AddCollection(Stream stream)
    {
        return AddCollection(stream, out _);
    }

    /// <inheritdoc />
    public IEnumerable<FontFamily> AddCollection(Stream stream, out IEnumerable<FontDescription> descriptions)
    {
        return AddCollectionImpl(stream, CultureInfo.InvariantCulture, out descriptions);
    }

    /// <inheritdoc />
    public FontFamily Get(string name)
    {
        return GetImpl(name, CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public bool TryGet(string name, out FontFamily family)
    {
        return TryGetImpl(name, CultureInfo.InvariantCulture, out family);
    }

    /// <inheritdoc />
    public FontFamily Add(string path, CultureInfo culture)
    {
        return AddImpl(path, culture, out _);
    }

    /// <inheritdoc />
    public FontFamily Add(string path, CultureInfo culture, out FontDescription description)
    {
        return AddImpl(path, culture, out description);
    }

    /// <inheritdoc />
    public FontFamily Add(Stream stream, CultureInfo culture)
    {
        return AddImpl(stream, culture, out _);
    }

    /// <inheritdoc />
    public FontFamily Add(Stream stream, CultureInfo culture, out FontDescription description)
    {
        return AddImpl(stream, culture, out description);
    }

    /// <inheritdoc />
    public IEnumerable<FontFamily> AddCollection(string path, CultureInfo culture)
    {
        return AddCollection(path, culture, out _);
    }

    /// <inheritdoc />
    public IEnumerable<FontFamily> AddCollection(
        string path,
        CultureInfo culture,
        out IEnumerable<FontDescription> descriptions)
    {
        return AddCollectionImpl(path, culture, out descriptions);
    }

    /// <inheritdoc />
    public IEnumerable<FontFamily> AddCollection(Stream stream, CultureInfo culture)
    {
        return AddCollection(stream, culture, out _);
    }

    /// <inheritdoc />
    public IEnumerable<FontFamily> AddCollection(
        Stream stream,
        CultureInfo culture,
        out IEnumerable<FontDescription> descriptions)
    {
        return AddCollectionImpl(stream, culture, out descriptions);
    }

    /// <inheritdoc />
    public IEnumerable<FontFamily> GetByCulture(CultureInfo culture)
    {
        return FamiliesByCultureImpl(culture);
    }

    /// <inheritdoc />
    public FontFamily Get(string name, CultureInfo culture)
    {
        return GetImpl(name, culture);
    }

    /// <inheritdoc />
    public bool TryGet(string name, CultureInfo culture, out FontFamily family)
    {
        return TryGetImpl(name, culture, out family);
    }

    /// <inheritdoc />
    FontFamily IFontMetricsCollection.AddMetrics(FontMetrics metrics, CultureInfo culture)
    {
        ((IFontMetricsCollection)this).AddMetrics(metrics);
        return new FontFamily(metrics.Description.FontFamily(culture), this, culture);
    }

    /// <inheritdoc />
    void IFontMetricsCollection.AddMetrics(FontMetrics metrics)
    {
        Guard.NotNull(metrics, nameof(metrics));

        if (metrics.Description is null)
            throw new ArgumentException($"{nameof(FontMetrics)} must have a Description.", nameof(metrics));

        lock (metricsCollection)
        {
            metricsCollection.Add(metrics);
        }
    }

    /// <inheritdoc />
    bool IReadOnlyFontMetricsCollection.TryGetMetrics(string name, CultureInfo culture, FontStyle style,
        [NotNullWhen(true)] out FontMetrics? metrics)
    {
        metrics = ((IReadOnlyFontMetricsCollection)this).GetAllMetrics(name, culture)
            .FirstOrDefault(x => x.Description.Style == style);

        return metrics != null;
    }

    /// <inheritdoc />
    IEnumerable<FontMetrics> IReadOnlyFontMetricsCollection.GetAllMetrics(string name, CultureInfo culture)
    {
        Guard.NotNull(name, nameof(name));
        var comparer = StringComparerHelpers.GetCaseInsensitiveStringComparer(culture);

        return metricsCollection
            .Where(x => comparer.Equals(x.Description.FontFamily(culture), name))
            .ToArray();
    }

    /// <inheritdoc />
    IEnumerable<FontStyle> IReadOnlyFontMetricsCollection.GetAllStyles(string name, CultureInfo culture)
    {
        return ((IReadOnlyFontMetricsCollection)this).GetAllMetrics(name, culture).Select(x => x.Description.Style)
            .ToArray();
    }

    /// <inheritdoc />
    IEnumerator<FontMetrics> IReadOnlyFontMetricsCollection.GetEnumerator()
    {
        return metricsCollection.GetEnumerator();
    }

    internal void AddSearchDirectories(IEnumerable<string> directories)
    {
        foreach (var directory in directories) searchDirectories.Add(directory);
    }

    private FontFamily AddImpl(string path, CultureInfo culture, out FontDescription description)
    {
        var instance = new FileFontMetrics(path);
        description = instance.Description;
        return ((IFontMetricsCollection)this).AddMetrics(instance, culture);
    }

    private FontFamily AddImpl(Stream stream, CultureInfo culture, out FontDescription description)
    {
        var metrics = StreamFontMetrics.LoadFont(stream);
        description = metrics.Description;

        return ((IFontMetricsCollection)this).AddMetrics(metrics, culture);
    }

    private IEnumerable<FontFamily> AddCollectionImpl(
        string path,
        CultureInfo culture,
        out IEnumerable<FontDescription> descriptions)
    {
        var fonts = FileFontMetrics.LoadFontCollection(path);

        var description = new FontDescription[fonts.Length];
        var families = new HashSet<FontFamily>();
        for (var i = 0; i < fonts.Length; i++)
        {
            description[i] = fonts[i].Description;
            var family = ((IFontMetricsCollection)this).AddMetrics(fonts[i], culture);
            families.Add(family);
        }

        descriptions = description;
        return families;
    }

    private IEnumerable<FontFamily> AddCollectionImpl(
        Stream stream,
        CultureInfo culture,
        out IEnumerable<FontDescription> descriptions)
    {
        var startPos = stream.Position;
        var reader = new BigEndianBinaryReader(stream, true);
        var ttcHeader = TtcHeader.Read(reader);
        var result = new List<FontDescription>((int)ttcHeader.NumFonts);
        var installedFamilies = new HashSet<FontFamily>();
        for (var i = 0; i < ttcHeader.NumFonts; ++i)
        {
            stream.Position = startPos + ttcHeader.OffsetTable[i];
            var instance = StreamFontMetrics.LoadFont(stream);
            installedFamilies.Add(((IFontMetricsCollection)this).AddMetrics(instance, culture));
            var fontDescription = instance.Description;
            result.Add(fontDescription);
        }

        descriptions = result;
        return installedFamilies;
    }

    private IEnumerable<FontFamily> FamiliesByCultureImpl(CultureInfo culture)
    {
        return metricsCollection
            .Select(x => x.Description.FontFamily(culture))
            .Distinct()
            .Select(x => new FontFamily(x, this, culture))
            .ToArray();
    }

    private bool TryGetImpl(string name, CultureInfo culture, out FontFamily family)
    {
        Guard.NotNull(name, nameof(name));
        var comparer = StringComparerHelpers.GetCaseInsensitiveStringComparer(culture);

        var match = metricsCollection
            .Select(x => x.Description.FontFamily(culture))
            .FirstOrDefault(x => comparer.Equals(name, x));

        if (match != null)
        {
            family = new FontFamily(match, this, culture);
            return true;
        }

        family = default;
        return false;
    }

    private FontFamily GetImpl(string name, CultureInfo culture)
    {
        if (TryGetImpl(name, culture, out var family)) return family;

        throw new FontFamilyNotFoundException(name, searchDirectories);
    }
}