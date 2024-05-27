// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Linq;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     The namedColor2Type is a count value and array of structures
///     that provide color coordinates for color names.
/// </summary>
internal sealed class IccNamedColor2TagDataEntry : IccTagDataEntry, IEquatable<IccNamedColor2TagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccNamedColor2TagDataEntry" /> class.
    /// </summary>
    /// <param name="colors">The named colors</param>
    public IccNamedColor2TagDataEntry(IccNamedColor[] colors)
        : this(0, null, null, colors, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccNamedColor2TagDataEntry" /> class.
    /// </summary>
    /// <param name="prefix">Prefix</param>
    /// <param name="suffix">Suffix</param>
    /// ///
    /// <param name="colors">The named colors</param>
    public IccNamedColor2TagDataEntry(string prefix, string suffix, IccNamedColor[] colors)
        : this(0, prefix, suffix, colors, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccNamedColor2TagDataEntry" /> class.
    /// </summary>
    /// <param name="vendorFlags">Vendor specific flags</param>
    /// <param name="prefix">Prefix</param>
    /// <param name="suffix">Suffix</param>
    /// <param name="colors">The named colors</param>
    public IccNamedColor2TagDataEntry(int vendorFlags, string prefix, string suffix, IccNamedColor[] colors)
        : this(vendorFlags, prefix, suffix, colors, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccNamedColor2TagDataEntry" /> class.
    /// </summary>
    /// <param name="colors">The named colors</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccNamedColor2TagDataEntry(IccNamedColor[] colors, IccProfileTag tagSignature)
        : this(0, null, null, colors, tagSignature) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccNamedColor2TagDataEntry" /> class.
    /// </summary>
    /// <param name="prefix">Prefix</param>
    /// <param name="suffix">Suffix</param>
    /// <param name="colors">The named colors</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccNamedColor2TagDataEntry(string prefix, string suffix, IccNamedColor[] colors, IccProfileTag tagSignature)
        : this(0, prefix, suffix, colors, tagSignature) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccNamedColor2TagDataEntry" /> class.
    /// </summary>
    /// <param name="vendorFlags">Vendor specific flags</param>
    /// <param name="prefix">Prefix</param>
    /// <param name="suffix">Suffix</param>
    /// <param name="colors">The named colors</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccNamedColor2TagDataEntry(int vendorFlags, string prefix, string suffix, IccNamedColor[] colors,
        IccProfileTag tagSignature)
        : base(IccTypeSignature.NamedColor2, tagSignature)
    {
        Guard.NotNull(colors, nameof(colors));

        var coordinateCount = 0;
        if (colors.Length > 0)
        {
            coordinateCount = colors[0].DeviceCoordinates?.Length ?? 0;

            Guard.IsFalse(colors.Any(t => (t.DeviceCoordinates?.Length ?? 0) != coordinateCount), nameof(colors),
                "Device coordinate count must be the same for all colors");
        }

        VendorFlags = vendorFlags;
        CoordinateCount = coordinateCount;
        Prefix = prefix;
        Suffix = suffix;
        Colors = colors;
    }

    /// <summary>
    ///     Gets the number of coordinates
    /// </summary>
    public int CoordinateCount { get; }

    /// <summary>
    ///     Gets the prefix
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    ///     Gets the suffix
    /// </summary>
    public string Suffix { get; }

    /// <summary>
    ///     Gets the vendor specific flags
    /// </summary>
    public int VendorFlags { get; }

    /// <summary>
    ///     Gets the named colors
    /// </summary>
    public IccNamedColor[] Colors { get; }

    /// <inheritdoc />
    public bool Equals(IccNamedColor2TagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other)
               && CoordinateCount == other.CoordinateCount
               && string.Equals(Prefix, other.Prefix)
               && string.Equals(Suffix, other.Suffix)
               && VendorFlags == other.VendorFlags
               && Colors.AsSpan().SequenceEqual(other.Colors);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccNamedColor2TagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccNamedColor2TagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Signature,
            CoordinateCount,
            Prefix,
            Suffix,
            VendorFlags,
            Colors);
    }
}