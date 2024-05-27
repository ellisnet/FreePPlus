// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

internal readonly struct BaseRecord
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseRecord" /> struct.
    /// </summary>
    /// <param name="reader">The big endian binary reader.</param>
    /// <param name="classCount">The class count.</param>
    /// <param name="offset">Offset to the from beginning of BaseArray table.</param>
    public BaseRecord(BigEndianBinaryReader reader, ushort classCount, long offset)
    {
        // +--------------+-----------------------------------+----------------------------------------------------------------------------------------+
        // | Type         | Name                              | Description                                                                            |
        // +==============+===================================+========================================================================================+
        // | Offset16     | baseAnchorOffsets[markClassCount] | Array of offsets (one per mark class) to Anchor tables.                                |
        // |              |                                   | Offsets are from beginning of BaseArray table, ordered by class (offsets may be NULL). |
        // +--------------+-----------------------------------+----------------------------------------------------------------------------------------+
        BaseAnchorTables = new AnchorTable[classCount];
        var baseAnchorOffsets = new ushort[classCount];
        for (var i = 0; i < classCount; i++) baseAnchorOffsets[i] = reader.ReadOffset16();

        var position = reader.BaseStream.Position;
        for (var i = 0; i < classCount; i++)
            if (baseAnchorOffsets[i] is not 0)
                BaseAnchorTables[i] = AnchorTable.Load(reader, offset + baseAnchorOffsets[i]);

        reader.BaseStream.Position = position;
    }

    /// <summary>
    ///     Gets the base anchor tables.
    /// </summary>
    public AnchorTable[] BaseAnchorTables { get; }
}