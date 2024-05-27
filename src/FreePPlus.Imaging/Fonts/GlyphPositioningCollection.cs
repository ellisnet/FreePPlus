// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;
using FreePPlus.Imaging.Fonts.Unicode;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Represents a collection of glyph metrics that are mapped to input codepoints.
/// </summary>
internal sealed class GlyphPositioningCollection : IGlyphShapingCollection
{
    /// <summary>
    ///     Contains a map the index of a map within the collection, non-sequential codepoint offsets, and their glyph ids,
    ///     point size, and mtrics.
    /// </summary>
    private readonly List<GlyphPositioningData> glyphs = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="GlyphPositioningCollection" /> class.
    /// </summary>
    /// <param name="textOptions">The text options.</param>
    public GlyphPositioningCollection(TextOptions textOptions)
    {
        TextOptions = textOptions;
        IsVerticalLayoutMode = textOptions.LayoutMode.IsVertical();
    }

    /// <inheritdoc />
    public int Count => glyphs.Count;

    /// <inheritdoc />
    public bool IsVerticalLayoutMode { get; }

    /// <inheritdoc />
    public TextOptions TextOptions { get; }

    /// <inheritdoc />
    public ushort this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => glyphs[index].Data.GlyphId;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GlyphShapingData GetGlyphShapingData(int index)
    {
        return glyphs[index].Data;
    }

    /// <inheritdoc />
    public void AddShapingFeature(int index, TagEntry feature)
    {
        glyphs[index].Data.Features.Add(feature);
    }

    /// <inheritdoc />
    public void EnableShapingFeature(int index, Tag feature)
    {
        var features = glyphs[index].Data.Features;
        for (var i = 0; i < features.Count; i++)
        {
            var tagEntry = features[i];
            if (tagEntry.Tag == feature)
            {
                tagEntry.Enabled = true;
                features[i] = tagEntry;
                break;
            }
        }
    }

    /// <inheritdoc />
    public void DisableShapingFeature(int index, Tag feature)
    {
        var features = glyphs[index].Data.Features;
        for (var i = 0; i < features.Count; i++)
        {
            var tagEntry = features[i];
            if (tagEntry.Tag == feature)
            {
                tagEntry.Enabled = false;
                features[i] = tagEntry;
                break;
            }
        }
    }

    /// <summary>
    ///     Gets the glyph metrics at the given codepoint offset.
    /// </summary>
    /// <param name="offset">The zero-based index within the input codepoint collection.</param>
    /// <param name="pointSize">The font size in PT units of the font containing this glyph.</param>
    /// <param name="isDecomposed">Whether the glyph is the result of a decomposition substitution.</param>
    /// <param name="metrics">
    ///     When this method returns, contains the glyph metrics associated with the specified offset,
    ///     if the value is found; otherwise, the default value for the type of the metrics parameter.
    ///     This parameter is passed uninitialized.
    /// </param>
    /// <returns>The metrics.</returns>
    public bool TryGetGlyphMetricsAtOffset(int offset, out float pointSize, out bool isDecomposed,
        [NotNullWhen(true)] out IReadOnlyList<GlyphMetrics>? metrics)
    {
        List<GlyphMetrics> match = new();
        pointSize = 0;
        isDecomposed = false;
        for (var i = 0; i < glyphs.Count; i++)
            if (glyphs[i].Offset == offset)
            {
                var glyph = glyphs[i];
                isDecomposed = glyph.Data.IsDecomposed;
                pointSize = glyph.PointSize;
                match.AddRange(glyph.Metrics);
            }
            else if (match.Count > 0)
            {
                // Offsets, though non-sequential, are sorted, so we can stop searching.
                break;
            }

        metrics = match;
        return match.Count > 0;
    }

    /// <summary>
    ///     Updates the collection of glyph ids to the metrics collection to overwrite any glyphs that have been previously
    ///     identified as fallbacks.
    /// </summary>
    /// <param name="font">The font face with metrics.</param>
    /// <param name="collection">The glyph substitution collection.</param>
    /// <returns>
    ///     <see langword="true" /> if the metrics collection does not contain any fallbacks; otherwise
    ///     <see langword="false" />.
    /// </returns>
    public bool TryUpdate(Font font, GlyphSubstitutionCollection collection)
    {
        var fontMetrics = font.FontMetrics;
        var colorFontSupport = TextOptions.ColorFontSupport;
        var hasFallBacks = false;
        List<int> orphans = new();
        for (var i = 0; i < glyphs.Count; i++)
        {
            var current = glyphs[i];
            if (current.Metrics[0].GlyphType != GlyphType.Fallback)
                // We've already got the correct glyph.
                continue;

            var offset = current.Offset;
            var pointSize = current.PointSize;
            if (collection.TryGetGlyphShapingDataAtOffset(offset, out var data))
            {
                ushort shiftXY = 0;
                var replacementCount = 0;
                for (var j = 0; j < data.Count; j++)
                {
                    var shape = data[j];
                    var id = shape.GlyphId;
                    var codePoint = shape.CodePoint;
                    var isDecomposed = shape.IsDecomposed;

                    // Perform a semi-deep clone (FontMetrics is not cloned) so we can continue to
                    // cache the original in the font metrics and only update our collection.
                    var metrics = new List<GlyphMetrics>(data.Count);
                    foreach (var gm in fontMetrics.GetGlyphMetrics(codePoint, id, colorFontSupport))
                    {
                        if (gm.GlyphType == GlyphType.Fallback && !CodePoint.IsControl(codePoint))
                        {
                            // If the glyphs are fallbacks we don't want them as
                            // we've already captured them on the first run.
                            hasFallBacks = true;
                            break;
                        }

                        // Clone and offset the glyph for rendering.
                        // If the glyph is the result of a decomposition substitution we need to offset it.
                        // We slip the text run in here while we clone so we have it available to the renderer.
                        var clone = gm.CloneForRendering(shape.TextRun, codePoint);
                        if (isDecomposed)
                        {
                            if (!IsVerticalLayoutMode)
                            {
                                clone.ApplyOffset((short)shiftXY, 0);
                                shiftXY += clone.AdvanceWidth;
                            }
                            else
                            {
                                clone.ApplyOffset(0, (short)shiftXY);
                                shiftXY += clone.AdvanceHeight;
                            }
                        }

                        metrics.Add(clone);
                    }

                    if (metrics.Count > 0)
                    {
                        if (j == 0)
                            // There should only be a single fallback glyph at this position from the previous collection.
                            glyphs.RemoveAt(i);

                        // Track the number of inserted glyphs at the offset so we can correctly increment our position.
                        glyphs.Insert(i += replacementCount,
                            new GlyphPositioningData(offset,
                                new GlyphShapingData(shape, true)
                                {
                                    Bounds = new GlyphShapingBounds(0, 0, metrics[0].AdvanceWidth,
                                        metrics[0].AdvanceHeight)
                                }, pointSize, metrics.ToArray()));
                        replacementCount++;
                    }
                }
            }
            else
            {
                // If a font had glyphs but a follow up font also has them and can substitute. e.g ligatures
                // then we end up with orphaned fallbacks. We need to remove them.
                orphans.Add(i);
            }
        }

        // Remove any orphans.
        for (var i = orphans.Count - 1; i >= 0; i--) glyphs.RemoveAt(orphans[i]);

        return !hasFallBacks;
    }

    /// <summary>
    ///     Adds the collection of glyph ids to the metrics collection.
    ///     identified as fallbacks.
    /// </summary>
    /// <param name="font">The font face with metrics.</param>
    /// <param name="collection">The glyph substitution collection.</param>
    /// <returns>
    ///     <see langword="true" /> if the metrics collection does not contain any fallbacks; otherwise
    ///     <see langword="false" />.
    /// </returns>
    public bool TryAdd(Font font, GlyphSubstitutionCollection collection)
    {
        var hasFallBacks = false;
        var fontMetrics = font.FontMetrics;
        var colorFontSupport = TextOptions.ColorFontSupport;
        ushort shiftXY = 0;
        for (var i = 0; i < collection.Count; i++)
        {
            var data = collection.GetGlyphShapingData(i, out var offset);
            var codePoint = data.CodePoint;
            var id = data.GlyphId;
            List<GlyphMetrics> metrics = new();

            var isDecomposed = data.IsDecomposed;
            if (!isDecomposed) shiftXY = 0;

            // Perform a semi-deep clone (FontMetrics is not cloned) so we can continue to
            // cache the original in the font metrics and only update our collection.
            foreach (var gm in fontMetrics.GetGlyphMetrics(codePoint, id, colorFontSupport))
            {
                if (gm.GlyphType == GlyphType.Fallback && !CodePoint.IsControl(codePoint)) hasFallBacks = true;

                // Clone and offset the glyph for rendering.
                // If the glyph is the result of a decomposition substitution we need to offset it.
                // We slip the text run in here while we clone so we have it available to the renderer.
                var clone = gm.CloneForRendering(data.TextRun, codePoint);
                if (isDecomposed)
                {
                    if (!IsVerticalLayoutMode)
                    {
                        clone.ApplyOffset((short)shiftXY, 0);
                        shiftXY += clone.AdvanceWidth;
                    }
                    else
                    {
                        clone.ApplyOffset(0, (short)shiftXY);
                        shiftXY += clone.AdvanceHeight;
                    }
                }

                metrics.Add(clone);
            }

            if (metrics.Count > 0)
            {
                var gm = metrics.ToArray();
                if (IsVerticalLayoutMode)
                    glyphs.Add(new GlyphPositioningData(offset,
                        new GlyphShapingData(data, true)
                            { Bounds = new GlyphShapingBounds(0, 0, 0, gm[0].AdvanceHeight) }, font.Size, gm));
                else
                    glyphs.Add(new GlyphPositioningData(offset,
                        new GlyphShapingData(data, true)
                            { Bounds = new GlyphShapingBounds(0, 0, gm[0].AdvanceWidth, 0) }, font.Size, gm));
            }
        }

        return !hasFallBacks;
    }

    /// <summary>
    ///     Updates the position of the glyph at the specified index.
    /// </summary>
    /// <param name="fontMetrics">The font metrics.</param>
    /// <param name="index">The zero-based index of the element.</param>
    public void UpdatePosition(FontMetrics fontMetrics, int index)
    {
        var data = GetGlyphShapingData(index);
        var isDirtyXY = data.Bounds.IsDirtyXY;
        var isDirtyWH = data.Bounds.IsDirtyWH;
        if (!isDirtyXY && !isDirtyWH) return;

        var glyphId = data.GlyphId;
        foreach (var m in glyphs[index].Metrics)
            if (m.GlyphId == glyphId && fontMetrics == m.FontMetrics)
            {
                if (isDirtyXY) m.ApplyOffset((short)data.Bounds.X, (short)data.Bounds.Y);

                if (isDirtyWH)
                {
                    m.SetAdvanceWidth((ushort)data.Bounds.Width);
                    m.SetAdvanceHeight((ushort)data.Bounds.Height);
                }
            }
    }

    /// <summary>
    ///     Updates the advanced metrics of the glyphs at the given index and id,
    ///     adding dx and dy to the current advance.
    /// </summary>
    /// <param name="fontMetrics">The font face with metrics.</param>
    /// <param name="index">The zero-based index of the element.</param>
    /// <param name="glyphId">The id of the glyph to offset.</param>
    /// <param name="dx">The delta x-advance.</param>
    /// <param name="dy">The delta y-advance.</param>
    public void Advance(FontMetrics fontMetrics, int index, ushort glyphId, short dx, short dy)
    {
        foreach (var m in glyphs[index].Metrics)
            if (m.GlyphId == glyphId && fontMetrics == m.FontMetrics)
                m.ApplyAdvance(dx, IsVerticalLayoutMode ? dy : (short)0);
    }

    /// <summary>
    ///     Returns a value indicating whether the element at the given index should be processed.
    /// </summary>
    /// <param name="fontMetrics">The font face with metrics.</param>
    /// <param name="index">The zero-based index of the elements to position.</param>
    /// <returns><see langword="true" /> if the element should be processed; otherwise, <see langword="false" />.</returns>
    public bool ShouldProcess(FontMetrics fontMetrics, int index)
    {
        return glyphs[index].Metrics[0].FontMetrics == fontMetrics;
    }

    private class GlyphPositioningData
    {
        public GlyphPositioningData(int offset, GlyphShapingData data, float pointSize, GlyphMetrics[] metrics)
        {
            Offset = offset;
            Data = data;
            PointSize = pointSize;
            Metrics = metrics;
        }

        public int Offset { get; }

        public GlyphShapingData Data { get; }

        public float PointSize { get; }

        public GlyphMetrics[] Metrics { get; }
    }
}