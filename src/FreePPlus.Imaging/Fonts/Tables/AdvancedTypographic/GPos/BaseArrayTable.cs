// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

internal class BaseArrayTable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseArrayTable" /> class.
    /// </summary>
    /// <param name="reader">The big endian binary reader.</param>
    /// <param name="offset">The offset to the beginning of the base array table.</param>
    /// <param name="classCount">The class count.</param>
    public BaseArrayTable(BigEndianBinaryReader reader, long offset, ushort classCount)
    {
        // +--------------+------------------------+--------------------------------------------------------------------------------------+
        // | Type         | Name                   | Description                                                                          |
        // +==============+========================+======================================================================================+
        // | uint16       | baseCount              | Number of BaseRecords                                                                |
        // +--------------+------------------------+--------------------------------------------------------------------------------------+
        // | BaseRecord   | baseRecords[baseCount] | Array of BaseRecords, in order of baseCoverage Index.                                |
        // +--------------+------------------------+--------------------------------------------------------------------------------------+
        reader.Seek(offset, SeekOrigin.Begin);
        var baseCount = reader.ReadUInt16();
        BaseRecords = new BaseRecord[baseCount];
        for (var i = 0; i < baseCount; i++) BaseRecords[i] = new BaseRecord(reader, classCount, offset);
    }

    /// <summary>
    ///     Gets the base records.
    /// </summary>
    public BaseRecord[] BaseRecords { get; }
}