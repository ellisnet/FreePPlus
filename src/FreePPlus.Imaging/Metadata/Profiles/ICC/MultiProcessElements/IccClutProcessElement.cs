// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     A CLUT (color lookup table) element to process data
/// </summary>
internal sealed class IccClutProcessElement : IccMultiProcessElement, IEquatable<IccClutProcessElement>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccClutProcessElement" /> class.
    /// </summary>
    /// <param name="clutValue">The color lookup table of this element</param>
    public IccClutProcessElement(IccClut clutValue)
        : base(IccMultiProcessElementSignature.Clut, clutValue?.InputChannelCount ?? 1,
            clutValue?.OutputChannelCount ?? 1)
    {
        ClutValue = clutValue ?? throw new ArgumentNullException(nameof(clutValue));
    }

    /// <summary>
    ///     Gets the color lookup table of this element
    /// </summary>
    public IccClut ClutValue { get; }

    /// <inheritdoc />
    public bool Equals(IccClutProcessElement other)
    {
        return Equals((IccMultiProcessElement)other);
    }

    /// <inheritdoc />
    public override bool Equals(IccMultiProcessElement other)
    {
        if (base.Equals(other) && other is IccClutProcessElement element) return ClutValue.Equals(element.ClutValue);

        return false;
    }
}