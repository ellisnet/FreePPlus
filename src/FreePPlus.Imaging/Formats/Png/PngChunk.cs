// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Memory;

namespace FreePPlus.Imaging.Formats.Png;

//was previously: namespace SixLabors.ImageSharp.Formats.Png;

/// <summary>
///     Stores header information about a chunk.
/// </summary>
internal readonly struct PngChunk
{
    public PngChunk(int length, PngChunkType type, IManagedByteBuffer data = null)
    {
        Length = length;
        Type = type;
        Data = data;
    }

    /// <summary>
    ///     Gets the length.
    ///     An unsigned integer giving the number of bytes in the chunk's
    ///     data field. The length counts only the data field, not itself,
    ///     the chunk type code, or the CRC. Zero is a valid length
    /// </summary>
    public int Length { get; }

    /// <summary>
    ///     Gets the chunk type.
    ///     The value is the equal to the UInt32BigEndian encoding of its 4 ASCII characters.
    /// </summary>
    public PngChunkType Type { get; }

    /// <summary>
    ///     Gets the data bytes appropriate to the chunk type, if any.
    ///     This field can be of zero length or null.
    /// </summary>
    public IManagedByteBuffer Data { get; }

    /// <summary>
    ///     Gets a value indicating whether the given chunk is critical to decoding
    /// </summary>
    public bool IsCritical =>
        Type == PngChunkType.Header ||
        Type == PngChunkType.Palette ||
        Type == PngChunkType.Data;
}