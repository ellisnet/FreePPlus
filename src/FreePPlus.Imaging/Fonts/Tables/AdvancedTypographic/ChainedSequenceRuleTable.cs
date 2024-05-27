// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

internal sealed class ChainedSequenceRuleTable
{
    private ChainedSequenceRuleTable(
        ushort[] backtrackSequence,
        ushort[] inputSequence,
        ushort[] lookaheadSequence,
        SequenceLookupRecord[] seqLookupRecords)
    {
        BacktrackSequence = backtrackSequence;
        InputSequence = inputSequence;
        LookaheadSequence = lookaheadSequence;
        SequenceLookupRecords = seqLookupRecords;
    }

    public ushort[] BacktrackSequence { get; }

    public ushort[] InputSequence { get; }

    public ushort[] LookaheadSequence { get; }

    /// <summary>
    ///     Gets the sequence lookup records.
    ///     The seqLookupRecords array lists the sequence lookup records that specify actions to be taken on glyphs at various
    ///     positions within the input sequence.
    ///     These do not have to be ordered in sequence position order; they are ordered according to the desired result.
    ///     All of the sequence lookup records are processed in order, and each applies to the results of the actions indicated
    ///     by the preceding record.
    /// </summary>
    public SequenceLookupRecord[] SequenceLookupRecords { get; }

    public static ChainedSequenceRuleTable Load(BigEndianBinaryReader reader, long offset)
    {
        // ChainedSequenceRule
        // +----------------------+----------------------------------------+--------------------------------------------+
        // | Type                 | Name                                   | Description                                |
        // +======================+========================================+============================================+
        // | uint16               | backtrackGlyphCount                    | Number of glyphs in the backtrack sequence |
        // +----------------------+----------------------------------------+--------------------------------------------+
        // | uint16               | backtrackSequence[backtrackGlyphCount] | Array of backtrack glyph IDs               |
        // +----------------------+----------------------------------------+--------------------------------------------+
        // | uint16               | inputGlyphCount                        | Number of glyphs in the input sequence     |
        // +----------------------+----------------------------------------+--------------------------------------------+
        // | uint16               | inputSequence[inputGlyphCount - 1]     | Array of input glyph IDs—start with        |
        // |                      |                                        | second glyph                               |
        // +----------------------+----------------------------------------+--------------------------------------------+
        // | uint16               | lookaheadGlyphCount                    | Number of glyphs in the lookahead sequence |
        // +----------------------+----------------------------------------+--------------------------------------------+
        // | uint16               | lookaheadSequence[lookaheadGlyphCount] | Array of lookahead glyph IDs               |
        // +----------------------+----------------------------------------+--------------------------------------------+
        // | uint16               | seqLookupCount                         | Number of SequenceLookupRecords            |
        // +----------------------+----------------------------------------+--------------------------------------------+
        // | SequenceLookupRecord | seqLookupRecords[seqLookupCount]       | Array of SequenceLookupRecords             |
        // +----------------------+----------------------------------------+--------------------------------------------+
        reader.Seek(offset, SeekOrigin.Begin);
        var backtrackGlyphCount = reader.ReadUInt16();
        var backtrackSequence = reader.ReadUInt16Array(backtrackGlyphCount);

        var inputGlyphCount = reader.ReadUInt16();
        var inputSequence = reader.ReadUInt16Array(inputGlyphCount - 1);

        var lookaheadGlyphCount = reader.ReadUInt16();
        var lookaheadSequence = reader.ReadUInt16Array(lookaheadGlyphCount);

        var seqLookupCount = reader.ReadUInt16();
        var seqLookupRecords = SequenceLookupRecord.LoadArray(reader, seqLookupCount);

        return new ChainedSequenceRuleTable(backtrackSequence, inputSequence, lookaheadSequence, seqLookupRecords);
    }
}