// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     This is a simple text structure that contains a text string.
/// </summary>
internal sealed class IccTextTagDataEntry : IccTagDataEntry, IEquatable<IccTextTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccTextTagDataEntry" /> class.
    /// </summary>
    /// <param name="text">The Text</param>
    public IccTextTagDataEntry(string text)
        : this(text, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccTextTagDataEntry" /> class.
    /// </summary>
    /// <param name="text">The Text</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccTextTagDataEntry(string text, IccProfileTag tagSignature)
        : base(IccTypeSignature.Text, tagSignature)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    /// <summary>
    ///     Gets the Text
    /// </summary>
    public string Text { get; }

    /// <inheritdoc />
    public bool Equals(IccTextTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other) && string.Equals(Text, other.Text);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccTextTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccTextTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Signature, Text);
    }
}