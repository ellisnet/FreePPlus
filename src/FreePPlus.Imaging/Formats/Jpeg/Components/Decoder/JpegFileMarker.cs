﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;

/// <summary>
///     Represents a jpeg file marker.
/// </summary>
internal readonly struct JpegFileMarker
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JpegFileMarker" /> struct.
    /// </summary>
    /// <param name="marker">The marker</param>
    /// <param name="position">The position within the stream</param>
    public JpegFileMarker(byte marker, long position)
        : this(marker, position, false) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="JpegFileMarker" /> struct.
    /// </summary>
    /// <param name="marker">The marker</param>
    /// <param name="position">The position within the stream</param>
    /// <param name="invalid">Whether the current marker is invalid</param>
    public JpegFileMarker(byte marker, long position, bool invalid)
    {
        Marker = marker;
        Position = position;
        Invalid = invalid;
    }

    /// <summary>
    ///     Gets a value indicating whether the current marker is invalid
    /// </summary>
    public bool Invalid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    /// <summary>
    ///     Gets the position of the marker within a stream
    /// </summary>
    public byte Marker
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    /// <summary>
    ///     Gets the position of the marker within a stream
    /// </summary>
    public long Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Marker.ToString("X");
    }
}