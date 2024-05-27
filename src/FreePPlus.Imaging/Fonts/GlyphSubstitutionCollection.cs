// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;
using FreePPlus.Imaging.Fonts.Unicode;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Represents a collection of glyph indices that are mapped to input codepoints.
/// </summary>
internal sealed class GlyphSubstitutionCollection : IGlyphShapingCollection
{
    /// <summary>
    ///     Contains a map the index of a map within the collection, non-sequential codepoint offsets, and their glyph ids.
    /// </summary>
    private readonly List<OffsetGlyphDataPair> glyphs = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="GlyphSubstitutionCollection" /> class.
    /// </summary>
    /// <param name="textOptions">The text options.</param>
    public GlyphSubstitutionCollection(TextOptions textOptions)
    {
        TextOptions = textOptions;
        IsVerticalLayoutMode = textOptions.LayoutMode.IsVertical();
    }

    /// <summary>
    ///     Gets or sets the running id of any ligature glyphs contained withing this collection are a member of.
    /// </summary>
    public int LigatureId { get; set; } = 1;

    /// <summary>
    ///     Gets the number of glyphs ids contained in the collection.
    ///     This may be more or less than original input codepoint count (due to substitution process).
    /// </summary>
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
    ///     Gets the shaping data at the specified position.
    /// </summary>
    /// <param name="index">The zero-based index of the elements to get.</param>
    /// <param name="offset">The zero-based index within the input codepoint collection.</param>
    /// <returns>The <see cref="GlyphShapingData" />.</returns>
    internal GlyphShapingData GetGlyphShapingData(int index, out int offset)
    {
        var pair = glyphs[index];
        offset = pair.Offset;
        return pair.Data;
    }

    /// <summary>
    ///     Adds the glyph id and the codepoint it represents to the collection.
    /// </summary>
    /// <param name="glyphId">The id of the glyph to add.</param>
    /// <param name="codePoint">The codepoint the glyph represents.</param>
    /// <param name="direction">The resolved text direction for the codepoint.</param>
    /// <param name="textRun">The text run this glyph belongs to.</param>
    /// <param name="offset">The zero-based index within the input codepoint collection.</param>
    public void AddGlyph(ushort glyphId, CodePoint codePoint, TextDirection direction, TextRun textRun, int offset)
    {
        glyphs.Add(new OffsetGlyphDataPair(offset, new GlyphShapingData(textRun)
        {
            CodePoint = codePoint,
            Direction = direction,
            GlyphId = glyphId
        }));
    }

    /// <summary>
    ///     Moves the specified glyph to the specified position.
    /// </summary>
    /// <param name="fromIndex">The index to move from.</param>
    /// <param name="toIndex">The index to move to.</param>
    public void MoveGlyph(int fromIndex, int toIndex)
    {
        var data = GetGlyphShapingData(fromIndex);
        if (fromIndex > toIndex)
        {
            var idx = fromIndex;
            while (idx > toIndex)
            {
                glyphs[idx].Data = glyphs[idx - 1].Data;
                idx--;
            }
        }
        else
        {
            var idx = toIndex;
            while (idx > fromIndex)
            {
                glyphs[idx - 1].Data = glyphs[idx].Data;
                idx--;
            }
        }

        glyphs[toIndex].Data = data;
    }

    /// <summary>
    ///     Removes all elements from the collection.
    /// </summary>
    public void Clear()
    {
        glyphs.Clear();
        LigatureId = 1;
    }

    /// <summary>
    ///     Gets the specified glyph ids matching the given codepoint offset.
    /// </summary>
    /// <param name="offset">The zero-based index within the input codepoint collection.</param>
    /// <param name="data">
    ///     When this method returns, contains the shaping data associated with the specified offset,
    ///     if the value is found; otherwise, the default value for the type of the data parameter.
    ///     This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if the <see cref="GlyphSubstitutionCollection" /> contains glyph ids
    ///     for the specified offset; otherwise, <see langword="false" />.
    /// </returns>
    public bool TryGetGlyphShapingDataAtOffset(int offset,
        [NotNullWhen(true)] out IReadOnlyList<GlyphShapingData>? data)
    {
        List<GlyphShapingData> match = new();
        for (var i = 0; i < glyphs.Count; i++)
            if (glyphs[i].Offset == offset)
                match.Add(glyphs[i].Data);
            else if (match.Count > 0)
                // Offsets, though non-sequential, are sorted, so we can stop searching.
                break;

        data = match;
        return match.Count > 0;
    }

    /// <summary>
    ///     Performs a 1:1 replacement of a glyph id at the given position.
    /// </summary>
    /// <param name="index">The zero-based index of the element to replace.</param>
    /// <param name="glyphId">The replacement glyph id.</param>
    public void Replace(int index, ushort glyphId)
    {
        glyphs[index].Data.GlyphId = glyphId;
    }

    /// <summary>
    ///     Performs a 1:1 replacement of a glyph id at the given position while removing a series of glyph ids at the given
    ///     positions within the sequence.
    /// </summary>
    /// <param name="index">The zero-based index of the element to replace.</param>
    /// <param name="removalIndices">The indices at which to remove elements.</param>
    /// <param name="glyphId">The replacement glyph id.</param>
    /// <param name="ligatureId">The ligature id.</param>
    public void Replace(int index, ReadOnlySpan<int> removalIndices, ushort glyphId, int ligatureId)
    {
        // Remove the glyphs at each index.
        var codePointCount = 0;
        for (var i = removalIndices.Length - 1; i >= 0; i--)
        {
            var match = removalIndices[i];
            codePointCount += glyphs[match].Data.CodePointCount;
            glyphs.RemoveAt(match);
        }

        // Assign our new id at the index.
        var current = glyphs[index].Data;
        current.CodePointCount += codePointCount;
        current.GlyphId = glyphId;
        current.LigatureId = ligatureId;
        current.LigatureComponent = -1;
        current.MarkAttachment = -1;
        current.CursiveAttachment = -1;
    }

    /// <summary>
    ///     Performs a 1:1 replacement of a glyph id at the given position while removing a series of glyph ids.
    /// </summary>
    /// <param name="index">The zero-based index of the element to replace.</param>
    /// <param name="count">The number of glyphs to remove.</param>
    /// <param name="glyphId">The replacement glyph id.</param>
    public void Replace(int index, int count, ushort glyphId)
    {
        // Remove the glyphs at each index.
        var codePointCount = 0;
        for (var i = count; i > 0; i--)
        {
            var match = index + i;
            codePointCount += glyphs[match].Data.CodePointCount;
            glyphs.RemoveAt(match);
        }

        // Assign our new id at the index.
        var current = glyphs[index].Data;
        current.CodePointCount += codePointCount;
        current.GlyphId = glyphId;
        current.LigatureId = 0;
        current.LigatureComponent = -1;
        current.MarkAttachment = -1;
        current.CursiveAttachment = -1;
    }

    /// <summary>
    ///     Replaces a single glyph id with a collection of glyph ids.
    /// </summary>
    /// <param name="index">The zero-based index of the element to replace.</param>
    /// <param name="glyphIds">The collection of replacement glyph ids.</param>
    public void Replace(int index, ReadOnlySpan<ushort> glyphIds)
    {
        if (glyphIds.Length > 0)
        {
            var pair = glyphs[index];
            var current = pair.Data;
            current.GlyphId = glyphIds[0];
            current.LigatureComponent = 0;
            current.MarkAttachment = -1;
            current.CursiveAttachment = -1;
            current.IsDecomposed = true;

            // Add additional glyphs from the rest of the sequence.
            if (glyphIds.Length > 1)
            {
                glyphIds = glyphIds[1..];
                for (var i = 0; i < glyphIds.Length; i++)
                {
                    GlyphShapingData data = new(current)
                    {
                        GlyphId = glyphIds[i],
                        LigatureComponent = i + 1
                    };

                    glyphs.Insert(++index, new OffsetGlyphDataPair(pair.Offset, data));
                }
            }
        }
        else
        {
            // Spec disallows removal of glyphs in this manner but it's common enough practice to allow it.
            // https://github.com/MicrosoftDocs/typography-issues/issues/673
            glyphs.RemoveAt(index);
        }
    }

    private class OffsetGlyphDataPair
    {
        public OffsetGlyphDataPair(int offset, GlyphShapingData data)
        {
            Offset = offset;
            Data = data;
        }

        public int Offset { get; }

        public GlyphShapingData Data { get; set; }
    }
}