// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Text;
using FreePPlus.Imaging.Fonts.WellKnownIds;

namespace FreePPlus.Imaging.Fonts.Utilities;

//was previously: namespace SixLabors.Fonts.Utilities;

/// <summary>
///     Converts encoding ID to TextEncoding
/// </summary>
internal static class EncodingIDExtensions
{
    /// <summary>
    ///     Converts encoding ID to TextEncoding
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>the encoding for this encoding ID</returns>
    public static Encoding AsEncoding(this EncodingIDs id)
    {
        switch (id)
        {
            case EncodingIDs.Unicode11:
            case EncodingIDs.Unicode2:
                return Encoding.BigEndianUnicode;
            default:
                return Encoding.UTF8;
        }
    }
}