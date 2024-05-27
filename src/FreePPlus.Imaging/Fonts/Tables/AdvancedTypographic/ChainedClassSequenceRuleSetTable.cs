// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

internal sealed class ChainedClassSequenceRuleSetTable
{
    private ChainedClassSequenceRuleSetTable(ChainedClassSequenceRuleTable[] subRules)
    {
        SubRules = subRules;
    }

    public ChainedClassSequenceRuleTable[] SubRules { get; }

    public static ChainedClassSequenceRuleSetTable Load(BigEndianBinaryReader reader, long offset)
    {
        // ClassSequenceRuleSet
        // +----------+----------------------------------------+---------------------------------------+
        // | Type     | Name                                   | Description                           |
        // +==========+========================================+=======================================+
        // | uint16   | classSeqRuleCount                      | Number of ClassSequenceRule tables    |
        // +----------+----------------------------------------+---------------------------------------+
        // | Offset16 | classSeqRuleOffsets[classSeqRuleCount] | Array of offsets to ClassSequenceRule |
        // |          |                                        | tables, from beginning of             |
        // |          |                                        | ClassSequenceRuleSet table            |
        // +----------+----------------------------------------+---------------------------------------+
        reader.Seek(offset, SeekOrigin.Begin);
        var seqRuleCount = reader.ReadUInt16();

        using Buffer<ushort> seqRuleOffsetsBuffer = new(seqRuleCount);
        var seqRuleOffsets = seqRuleOffsetsBuffer.GetSpan();
        reader.ReadUInt16Array(seqRuleOffsets);

        var subRules = new ChainedClassSequenceRuleTable[seqRuleCount];
        for (var i = 0; i < subRules.Length; i++)
            subRules[i] = ChainedClassSequenceRuleTable.Load(reader, offset + seqRuleOffsets[i]);

        return new ChainedClassSequenceRuleSetTable(subRules);
    }
}