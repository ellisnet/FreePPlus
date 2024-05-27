﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using FreePPlus.Imaging.Formats;

namespace FreePPlus.Imaging.Metadata;

//was previously: namespace SixLabors.ImageSharp.Metadata;

/// <summary>
///     Encapsulates the metadata of an image frame.
/// </summary>
public sealed class ImageFrameMetadata : IDeepCloneable<ImageFrameMetadata>
{
    private readonly Dictionary<IImageFormat, IDeepCloneable> formatMetadata = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageFrameMetadata" /> class.
    /// </summary>
    internal ImageFrameMetadata() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageFrameMetadata" /> class
    ///     by making a copy from other metadata.
    /// </summary>
    /// <param name="other">
    ///     The other <see cref="ImageFrameMetadata" /> to create this instance from.
    /// </param>
    internal ImageFrameMetadata(ImageFrameMetadata other)
    {
        DebugGuard.NotNull(other, nameof(other));

        foreach (var meta in other.formatMetadata) formatMetadata.Add(meta.Key, meta.Value.DeepClone());
    }

    /// <inheritdoc />
    public ImageFrameMetadata DeepClone()
    {
        return new ImageFrameMetadata(this);
    }

    /// <summary>
    ///     Gets the metadata value associated with the specified key.
    /// </summary>
    /// <typeparam name="TFormatMetadata">The type of format metadata.</typeparam>
    /// <typeparam name="TFormatFrameMetadata">The type of format frame metadata.</typeparam>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>
    ///     The <typeparamref name="TFormatFrameMetadata" />.
    /// </returns>
    public TFormatFrameMetadata GetFormatMetadata<TFormatMetadata, TFormatFrameMetadata>(
        IImageFormat<TFormatMetadata, TFormatFrameMetadata> key)
        where TFormatMetadata : class
        where TFormatFrameMetadata : class, IDeepCloneable
    {
        if (formatMetadata.TryGetValue(key, out var meta)) return (TFormatFrameMetadata)meta;

        var newMeta = key.CreateDefaultFormatFrameMetadata();
        formatMetadata[key] = newMeta;
        return newMeta;
    }
}