// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Text;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     The dataType is a simple data structure that contains
///     either 7-bit ASCII or binary data, i.e. textType data or transparent bytes.
/// </summary>
internal sealed class IccDataTagDataEntry : IccTagDataEntry, IEquatable<IccDataTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccDataTagDataEntry" /> class.
    /// </summary>
    /// <param name="data">The raw data</param>
    public IccDataTagDataEntry(byte[] data)
        : this(data, false, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccDataTagDataEntry" /> class.
    /// </summary>
    /// <param name="data">The raw data</param>
    /// <param name="isAscii">True if the given data is 7bit ASCII encoded text</param>
    public IccDataTagDataEntry(byte[] data, bool isAscii)
        : this(data, isAscii, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccDataTagDataEntry" /> class.
    /// </summary>
    /// <param name="data">The raw data</param>
    /// <param name="isAscii">True if the given data is 7bit ASCII encoded text</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccDataTagDataEntry(byte[] data, bool isAscii, IccProfileTag tagSignature)
        : base(IccTypeSignature.Data, tagSignature)
    {
        Data = data ?? throw new ArgumentException(nameof(data));
        IsAscii = isAscii;
    }

    /// <summary>
    ///     Gets the raw Data
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    ///     Gets a value indicating whether the <see cref="Data" /> represents 7bit ASCII encoded text
    /// </summary>
    public bool IsAscii { get; }

    /// <summary>
    ///     Gets the <see cref="Data" /> decoded as 7bit ASCII.
    ///     If <see cref="IsAscii" /> is false, returns null
    /// </summary>
    public string AsciiString => IsAscii ? Encoding.ASCII.GetString(Data, 0, Data.Length) : null;

    /// <inheritdoc />
    public bool Equals(IccDataTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other) && Data.AsSpan().SequenceEqual(other.Data) && IsAscii == other.IsAscii;
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccDataTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccDataTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Signature,
            Data,
            IsAscii);
    }
}