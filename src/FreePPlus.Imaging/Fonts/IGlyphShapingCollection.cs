// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Defines the contract for glyph shaping collections.
/// </summary>
internal interface IGlyphShapingCollection
{
    /// <summary>
    ///     Gets the collection count.
    /// </summary>
    int Count { get; }

    /// <summary>
    ///     Gets a value indicating whether the text layout mode is vertical.
    /// </summary>
    bool IsVerticalLayoutMode { get; }

    /// <summary>
    ///     Gets the text options used by this collection.
    /// </summary>
    TextOptions TextOptions { get; }

    /// <summary>
    ///     Gets the glyph id at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the elements to get.</param>
    /// <returns>The <see cref="ushort" />.</returns>
    ushort this[int index] { get; }

    /// <summary>
    ///     Gets the shaping data at the specified position.
    /// </summary>
    /// <param name="index">The zero-based index of the elements to get.</param>
    /// <returns>The <see cref="GlyphShapingData" />.</returns>
    GlyphShapingData GetGlyphShapingData(int index);

    /// <summary>
    ///     Adds the shaping feature to the collection which should be applied to the glyph at a specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <param name="feature">The feature to apply.</param>
    void AddShapingFeature(int index, TagEntry feature);

    /// <summary>
    ///     Enables a previously added shaping feature.
    /// </summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <param name="feature">The feature to enable.</param>
    void EnableShapingFeature(int index, Tag feature);

    /// <summary>
    ///     Disables a previously added shaping feature.
    /// </summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <param name="feature">The feature to disable.</param>
    void DisableShapingFeature(int index, Tag feature);
}