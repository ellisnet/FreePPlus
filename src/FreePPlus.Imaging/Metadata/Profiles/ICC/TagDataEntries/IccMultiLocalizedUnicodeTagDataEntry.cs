// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     This tag structure contains a set of records each referencing
///     a multilingual string associated with a profile.
/// </summary>
internal sealed class IccMultiLocalizedUnicodeTagDataEntry : IccTagDataEntry,
    IEquatable<IccMultiLocalizedUnicodeTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccMultiLocalizedUnicodeTagDataEntry" /> class.
    /// </summary>
    /// <param name="texts">Localized Text</param>
    public IccMultiLocalizedUnicodeTagDataEntry(IccLocalizedString[] texts)
        : this(texts, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccMultiLocalizedUnicodeTagDataEntry" /> class.
    /// </summary>
    /// <param name="texts">Localized Text</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccMultiLocalizedUnicodeTagDataEntry(IccLocalizedString[] texts, IccProfileTag tagSignature)
        : base(IccTypeSignature.MultiLocalizedUnicode, tagSignature)
    {
        Texts = texts ?? throw new ArgumentNullException(nameof(texts));
    }

    /// <summary>
    ///     Gets the localized texts
    /// </summary>
    public IccLocalizedString[] Texts { get; }

    /// <inheritdoc />
    public bool Equals(IccMultiLocalizedUnicodeTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other) && Texts.AsSpan().SequenceEqual(other.Texts);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccMultiLocalizedUnicodeTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccMultiLocalizedUnicodeTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Signature, Texts);
    }
}