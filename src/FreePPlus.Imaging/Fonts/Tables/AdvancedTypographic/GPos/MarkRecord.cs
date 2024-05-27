// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Diagnostics;
using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

/// <summary>
///     Defines a mark record used in a mark array table:
///     <see href="https://docs.microsoft.com/en-us/typography/opentype/spec/gpos#mark-array-table" />
/// </summary>
[DebuggerDisplay("MarkClass: {MarkClass}, AnchorTable: {MarkAnchorTable}")]
internal readonly struct MarkRecord
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MarkRecord" /> struct.
    /// </summary>
    /// <param name="reader">The big endian binary reader.</param>
    /// <param name="offset">Offset to the beginning of MarkArray table.</param>
    public MarkRecord(BigEndianBinaryReader reader, long offset)
    {
        // +--------------+------------------+--------------------------------------------------------------------------------------+
        // | Type         | Name             | Description                                                                          |
        // +==============+==================+======================================================================================+
        // | uint16       | markClass        | Class defined for the associated mark.                                               |
        // +--------------+------------------+--------------------------------------------------------------------------------------+
        // | Offset16     | markAnchorOffset | Offset to Anchor table, from beginning of MarkArray table.                           |
        // +--------------+------------------+--------------------------------------------------------------------------------------+
        MarkClass = reader.ReadUInt16();
        var markAnchorOffset = reader.ReadOffset16();

        // Reset the reader position after reading the anchor table.
        var readerPosition = reader.BaseStream.Position;
        MarkAnchorTable = AnchorTable.Load(reader, offset + markAnchorOffset);
        reader.BaseStream.Seek(readerPosition, SeekOrigin.Begin);
    }

    /// <summary>
    ///     Gets the class defined for the associated mark.
    /// </summary>
    public uint MarkClass { get; }

    /// <summary>
    ///     Gets the mark anchor table.
    /// </summary>
    public AnchorTable MarkAnchorTable { get; }
}