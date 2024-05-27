// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using FreePPlus.Imaging.Metadata;

namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;

/// <summary>
///     Provides information about the JFIF marker segment.
///     TODO: Thumbnail?
/// </summary>
internal readonly struct JFifMarker : IEquatable<JFifMarker>
{
    /// <summary>
    ///     Gets the length of an JFIF marker segment.
    /// </summary>
    public const int Length = 13;

    /// <summary>
    ///     Initializes a new instance of the <see cref="JFifMarker" /> struct.
    /// </summary>
    /// <param name="majorVersion">The major version.</param>
    /// <param name="minorVersion">The minor version.</param>
    /// <param name="densityUnits">The units for the density values.</param>
    /// <param name="xDensity">The horizontal pixel density.</param>
    /// <param name="yDensity">The vertical pixel density.</param>
    private JFifMarker(byte majorVersion, byte minorVersion, byte densityUnits, short xDensity, short yDensity)
    {
        if (xDensity <= 0)
            JpegThrowHelper.ThrowInvalidImageContentException($"X-Density {xDensity} must be greater than 0.");

        if (yDensity <= 0)
            JpegThrowHelper.ThrowInvalidImageContentException($"Y-Density {yDensity} must be greater than 0.");

        MajorVersion = majorVersion;
        MinorVersion = minorVersion;

        // LibJpeg and co will simply cast and not try to enforce a range.
        DensityUnits = (PixelResolutionUnit)densityUnits;
        XDensity = xDensity;
        YDensity = yDensity;
    }

    /// <summary>
    ///     Gets the major version.
    /// </summary>
    public byte MajorVersion { get; }

    /// <summary>
    ///     Gets the minor version.
    /// </summary>
    public byte MinorVersion { get; }

    /// <summary>
    ///     Gets the units for the following pixel density fields
    ///     00 : No units; width:height pixel aspect ratio = Ydensity:Xdensity
    ///     01 : Pixels per inch (2.54 cm)
    ///     02 : Pixels per centimeter
    /// </summary>
    public PixelResolutionUnit DensityUnits { get; }

    /// <summary>
    ///     Gets the horizontal pixel density. Must not be zero.
    /// </summary>
    public short XDensity { get; }

    /// <summary>
    ///     Gets the vertical pixel density. Must not be zero.
    /// </summary>
    public short YDensity { get; }

    /// <summary>
    ///     Converts the specified byte array representation of an JFIF marker to its <see cref="JFifMarker" /> equivalent and
    ///     returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="bytes">The byte array containing metadata to parse.</param>
    /// <param name="marker">The marker to return.</param>
    public static bool TryParse(byte[] bytes, out JFifMarker marker)
    {
        if (ProfileResolver.IsProfile(bytes, ProfileResolver.JFifMarker))
        {
            var majorVersion = bytes[5];
            var minorVersion = bytes[6];
            var densityUnits = bytes[7];
            var xDensity = (short)((bytes[8] << 8) | bytes[9]);
            var yDensity = (short)((bytes[10] << 8) | bytes[11]);

            if (xDensity > 0 && yDensity > 0)
            {
                marker = new JFifMarker(majorVersion, minorVersion, densityUnits, xDensity, yDensity);
                return true;
            }
        }

        marker = default;
        return false;
    }

    /// <inheritdoc />
    public bool Equals(JFifMarker other)
    {
        return MajorVersion == other.MajorVersion
               && MinorVersion == other.MinorVersion
               && DensityUnits == other.DensityUnits
               && XDensity == other.XDensity
               && YDensity == other.YDensity;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is JFifMarker other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            MajorVersion,
            MinorVersion,
            DensityUnits,
            XDensity,
            YDensity);
    }
}