// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

internal sealed class ClassSequenceRuleSetTable
{
    private ClassSequenceRuleSetTable(ClassSequenceRuleTable[] sequenceRuleTables)
    {
        SequenceRuleTables = sequenceRuleTables;
    }

    public ClassSequenceRuleTable[] SequenceRuleTables { get; }

    /// <summary>
    ///     Loads the class sequence rule set table.
    /// </summary>
    /// <param name="reader">The big endian binary reader.</param>
    /// <param name="offset">Offset from beginning of the ClassSequenceRuleSet table.</param>
    /// <returns>A class sequence rule set table.</returns>
    public static ClassSequenceRuleSetTable Load(BigEndianBinaryReader reader, long offset)
    {
        // ClassSequenceRuleSet
        // +----------+----------------------------------------+---------------------------------------+
        // | Type     | Name                                   | Description                           |
        // +==========+========================================+=======================================+
        // | uint16   | classSeqRuleCount                      | Number of ClassSequenceRule tables.   |
        // +----------+----------------------------------------+---------------------------------------+
        // | Offset16 | classSeqRuleOffsets[classSeqRuleCount] | Array of offsets to ClassSequenceRule |
        // |          |                                        | tables, from beginning of             |
        // |          |                                        | ClassSequenceRuleSet table.           |
        // +----------+----------------------------------------+---------------------------------------+
        reader.Seek(offset, SeekOrigin.Begin);
        var seqRuleCount = reader.ReadUInt16();

        using Buffer<ushort> seqRuleOffsetsBuffer = new(seqRuleCount);
        var seqRuleOffsets = seqRuleOffsetsBuffer.GetSpan();
        reader.ReadUInt16Array(seqRuleOffsets);

        var subRules = new ClassSequenceRuleTable[seqRuleCount];
        for (var i = 0; i < subRules.Length; i++)
            subRules[i] = ClassSequenceRuleTable.Load(reader, offset + seqRuleOffsets[i]);

        return new ClassSequenceRuleSetTable(subRules);
    }
}