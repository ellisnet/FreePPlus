﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Linq;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     The chromaticity tag type provides basic chromaticity data
///     and type of phosphors or colorants of a monitor to applications and utilities.
/// </summary>
internal sealed class IccChromaticityTagDataEntry : IccTagDataEntry, IEquatable<IccChromaticityTagDataEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccChromaticityTagDataEntry" /> class.
    /// </summary>
    /// <param name="colorantType">Colorant Type</param>
    public IccChromaticityTagDataEntry(IccColorantEncoding colorantType)
        : this(colorantType, GetColorantArray(colorantType), IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccChromaticityTagDataEntry" /> class.
    /// </summary>
    /// <param name="channelValues">Values per channel</param>
    public IccChromaticityTagDataEntry(double[][] channelValues)
        : this(IccColorantEncoding.Unknown, channelValues, IccProfileTag.Unknown) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccChromaticityTagDataEntry" /> class.
    /// </summary>
    /// <param name="colorantType">Colorant Type</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccChromaticityTagDataEntry(IccColorantEncoding colorantType, IccProfileTag tagSignature)
        : this(colorantType, GetColorantArray(colorantType), tagSignature) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccChromaticityTagDataEntry" /> class.
    /// </summary>
    /// <param name="channelValues">Values per channel</param>
    /// <param name="tagSignature">Tag Signature</param>
    public IccChromaticityTagDataEntry(double[][] channelValues, IccProfileTag tagSignature)
        : this(IccColorantEncoding.Unknown, channelValues, tagSignature) { }

    private IccChromaticityTagDataEntry(IccColorantEncoding colorantType, double[][] channelValues,
        IccProfileTag tagSignature)
        : base(IccTypeSignature.Chromaticity, tagSignature)
    {
        Guard.NotNull(channelValues, nameof(channelValues));
        Guard.MustBeBetweenOrEqualTo(channelValues.Length, 1, 15, nameof(channelValues));

        ColorantType = colorantType;
        ChannelValues = channelValues;

        var channelLength = channelValues[0].Length;
        var channelsNotSame = channelValues.Any(t => t is null || t.Length != channelLength);
        Guard.IsFalse(channelsNotSame, nameof(channelValues),
            "The number of values per channel is not the same for all channels");
    }

    /// <summary>
    ///     Gets the number of channels
    /// </summary>
    public int ChannelCount => ChannelValues.Length;

    /// <summary>
    ///     Gets the colorant type
    /// </summary>
    public IccColorantEncoding ColorantType { get; }

    /// <summary>
    ///     Gets the values per channel
    /// </summary>
    public double[][] ChannelValues { get; }

    /// <inheritdoc />
    public bool Equals(IccChromaticityTagDataEntry other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return base.Equals(other) && ColorantType == other.ColorantType && EqualsChannelValues(other);
    }

    /// <inheritdoc />
    public override bool Equals(IccTagDataEntry other)
    {
        return other is IccChromaticityTagDataEntry entry && Equals(entry);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is IccChromaticityTagDataEntry other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Signature,
            ColorantType,
            ChannelValues);
    }

    private static double[][] GetColorantArray(IccColorantEncoding colorantType)
    {
        switch (colorantType)
        {
            case IccColorantEncoding.EbuTech3213E:
                return new[]
                {
                    new[] { 0.640, 0.330 },
                    new[] { 0.290, 0.600 },
                    new[] { 0.150, 0.060 }
                };
            case IccColorantEncoding.ItuRBt709_2:
                return new[]
                {
                    new[] { 0.640, 0.330 },
                    new[] { 0.300, 0.600 },
                    new[] { 0.150, 0.060 }
                };
            case IccColorantEncoding.P22:
                return new[]
                {
                    new[] { 0.625, 0.340 },
                    new[] { 0.280, 0.605 },
                    new[] { 0.155, 0.070 }
                };
            case IccColorantEncoding.SmpteRp145:
                return new[]
                {
                    new[] { 0.630, 0.340 },
                    new[] { 0.310, 0.595 },
                    new[] { 0.155, 0.070 }
                };
            default:
                throw new ArgumentException("Unrecognized colorant encoding");
        }
    }

    private bool EqualsChannelValues(IccChromaticityTagDataEntry entry)
    {
        if (ChannelValues.Length != entry.ChannelValues.Length) return false;

        for (var i = 0; i < ChannelValues.Length; i++)
            if (!ChannelValues[i].AsSpan().SequenceEqual(entry.ChannelValues[i]))
                return false;

        return true;
    }
}