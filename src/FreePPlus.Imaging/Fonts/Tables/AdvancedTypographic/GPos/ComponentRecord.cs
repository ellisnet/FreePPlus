// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

/// <summary>
///     In a ComponentRecord, the zero-based ligatureAnchorOffsets array lists offsets to Anchor tables by mark class.
///     If a component does not define an attachment point for a particular class of marks, then the offset to the
///     corresponding Anchor table will be NULL.
///     Example 8 at the end of this chapter shows a MarkLigPosFormat1 subtable used to attach mark accents to a ligature
///     glyph in the Arabic script.
/// </summary>
internal class ComponentRecord
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ComponentRecord" /> class.
    /// </summary>
    /// <param name="reader">The big endian binary reader.</param>
    /// <param name="markClassCount">Number of defined mark classes.</param>
    /// <param name="offset">Offset from beginning of LigatureAttach table.</param>
    public ComponentRecord(BigEndianBinaryReader reader, ushort markClassCount, long offset)
    {
        // +--------------+---------------------------------------+----------------------------------------------------------------------------------------+
        // | Type         | Name                                  | Description                                                                            |
        // +==============+=======================================+========================================================================================+
        // | Offset16     | ligatureAnchorOffsets[markClassCount] | Array of offsets (one per class) to Anchor tables. Offsets are from                    |
        // |              |                                       | beginning of LigatureAttach table, ordered by class (offsets may be NULL).             |
        // +--------------+---------------------------------------+----------------------------------------------------------------------------------------+
        LigatureAnchorTables = new AnchorTable[markClassCount];
        var ligatureAnchorOffsets = new ushort[markClassCount];
        for (var i = 0; i < markClassCount; i++) ligatureAnchorOffsets[i] = reader.ReadOffset16();

        var position = reader.BaseStream.Position;
        for (var i = 0; i < markClassCount; i++)
            if (ligatureAnchorOffsets[i] is not 0)
                LigatureAnchorTables[i] = AnchorTable.Load(reader, offset + ligatureAnchorOffsets[i]);

        reader.BaseStream.Position = position;
    }

    public AnchorTable[] LigatureAnchorTables { get; }
}