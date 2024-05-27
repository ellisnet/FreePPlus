// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

/// <summary>
///     This lookup provides a mechanism whereby any other lookup type’s subtables are stored at a 32-bit offset location
///     in the GPOS table.
///     This is needed if the total size of the subtables exceeds the 16-bit limits of the various other offsets in the
///     GPOS table.
///     In this specification, the subtable stored at the 32-bit offset location is termed the “extension” subtable.
///     <see href="https://docs.microsoft.com/en-us/typography/opentype/spec/gpos#lookuptype-9-extension-positioning" />
/// </summary>
internal static class LookupType9SubTable
{
    public static LookupSubTable Load(
        BigEndianBinaryReader reader,
        long offset,
        LookupFlags lookupFlags,
        Func<ushort, LookupFlags, BigEndianBinaryReader, long, LookupSubTable> subTableLoader)
    {
        reader.Seek(offset, SeekOrigin.Begin);
        var substFormat = reader.ReadUInt16();

        return substFormat switch
        {
            1 => LookupType9Format1SubTable.Load(reader, offset, lookupFlags, subTableLoader),
            _ => new NotImplementedSubTable()
        };
    }
}

internal static class LookupType9Format1SubTable
{
    public static LookupSubTable Load(
        BigEndianBinaryReader reader,
        long offset,
        LookupFlags lookupFlags,
        Func<ushort, LookupFlags, BigEndianBinaryReader, long, LookupSubTable> subTableLoader)
    {
        // +----------+---------------------+------------------------------------------------------------------------------------------------------------------------------------+
        // | Type     | Name                | Description                                                                                                                        |
        // +==========+=====================+====================================================================================================================================+
        // | uint16   | substFormat         | Format identifier. Set to 1.                                                                                                       |
        // +----------+---------------------+------------------------------------------------------------------------------------------------------------------------------------+
        // | uint16   | extensionLookupType | Lookup type of subtable referenced by extensionOffset (that is, the extension subtable).                                           |
        // +----------+---------------------+------------------------------------------------------------------------------------------------------------------------------------+
        // | Offset32 | extensionOffset     | Offset to the extension subtable, of lookup type extensionLookupType, relative to the start of the ExtensionSubstFormat1 subtable. |
        // +----------+---------------------+------------------------------------------------------------------------------------------------------------------------------------+
        var extensionLookupType = reader.ReadUInt16();
        var extensionOffset = reader.ReadOffset32();

        // The extensionLookupType field must be set to any lookup type other than 9.
        // All subtables in a LookupType 9 lookup must have the same extensionLookupType.
        if (extensionLookupType == 9)
            // Don't throw, we'll just ignore.
            return new NotImplementedSubTable();

        // Read the lookup table again with the updated offset.
        return subTableLoader(extensionLookupType, lookupFlags, reader, offset + extensionOffset);
    }
}