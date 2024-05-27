// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

internal sealed class ChainedSequenceRuleSetTable
{
    private ChainedSequenceRuleSetTable(ChainedSequenceRuleTable[] subRules)
    {
        SequenceRuleTables = subRules;
    }

    public ChainedSequenceRuleTable[] SequenceRuleTables { get; }

    public static ChainedSequenceRuleSetTable Load(BigEndianBinaryReader reader, long offset)
    {
        // ChainedSequenceRuleSet
        // +----------+--------------------------------------------+-----------------------------------------+
        // | Type     | Name                                       | Description                             |
        // +==========+============================================+=========================================+
        // | uint16   | chainedSeqRuleCount                        | Number of ChainedSequenceRule tables    |
        // +----------+--------------------------------------------+-----------------------------------------+
        // | Offset16 | chainedSeqRuleOffsets[chainedSeqRuleCount] | Array of offsets to ChainedSequenceRule |
        // |          |                                            | tables, from beginning of               |
        // |          |                                            | ChainedSequenceRuleSet table            |
        // +----------+--------------------------------------------+-----------------------------------------+
        reader.Seek(offset, SeekOrigin.Begin);
        var chainedSeqRuleCount = reader.ReadUInt16();

        using Buffer<ushort> chainedSeqRuleOffsetsBuffer = new(chainedSeqRuleCount);
        var chainedSeqRuleOffsets = chainedSeqRuleOffsetsBuffer.GetSpan();
        reader.ReadUInt16Array(chainedSeqRuleOffsets);

        var chainedSequenceRules = new ChainedSequenceRuleTable[chainedSeqRuleCount];
        for (var i = 0; i < chainedSequenceRules.Length; i++)
            chainedSequenceRules[i] = ChainedSequenceRuleTable.Load(reader, offset + chainedSeqRuleOffsets[i]);

        return new ChainedSequenceRuleSetTable(chainedSequenceRules);
    }
}