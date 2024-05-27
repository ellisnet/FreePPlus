// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using FreePPlus.Imaging.Fonts.Unicode;
using FreePPlus.Imaging.Fonts.WellKnownIds;

namespace FreePPlus.Imaging.Fonts.Tables.General.CMap;

//was previously: namespace SixLabors.Fonts.Tables.General.CMap;

internal sealed class Format4SubTable : CMapSubTable
{
    public Format4SubTable(ushort language, PlatformIDs platform, ushort encoding, Segment[] segments,
        ushort[] glyphIds)
        : base(platform, encoding, 4)
    {
        Language = language;
        Segments = segments;
        GlyphIds = glyphIds;
    }

    public Segment[] Segments { get; }

    public ushort[] GlyphIds { get; }

    public ushort Language { get; }

    public override bool TryGetGlyphId(CodePoint codePoint, out ushort glyphId)
    {
        var charAsInt = codePoint.Value;

        for (var i = 0; i < Segments.Length; i++)
        {
            ref var seg = ref Segments[i];

            if (seg.End >= charAsInt && seg.Start <= charAsInt)
            {
                if (seg.Offset == 0)
                {
                    glyphId = (ushort)((charAsInt + seg.Delta) & ushort.MaxValue);
                    return true;
                }

                long offset = seg.Offset / 2 + (charAsInt - seg.Start);
                glyphId = GlyphIds[offset - Segments.Length + seg.Index];
                return true;
            }
        }

        glyphId = 0;
        return false;
    }

    public static IEnumerable<Format4SubTable> Load(IEnumerable<EncodingRecord> encodings, BigEndianBinaryReader reader)
    {
        // 'cmap' Subtable Format 4:
        // Type   | Name                       | Description
        // -------|----------------------------|------------------------------------------------------------------------
        // uint16 | format                     | Format number is set to 4.
        // uint16 | length                     | This is the length in bytes of the subtable.
        // uint16 | language                   | Please see “Note on the language field in 'cmap' subtables“ in this document.
        // uint16 | segCountX2                 | 2 x segCount.
        // uint16 | searchRange                | 2 x (2**floor(log2(segCount)))
        // uint16 | entrySelector              | log2(searchRange/2)
        // uint16 | rangeShift                 | 2 x segCount - searchRange
        // uint16 | endCount[segCount]         | End characterCode for each segment, last=0xFFFF.
        // uint16 | reservedPad                | Set to 0.
        // uint16 | startCount[segCount]       | Start character code for each segment.
        // int16  | idDelta[segCount]           | Delta for all character codes in segment.
        // uint16 | idRangeOffset[segCount]    | Offsets into glyphIdArray or 0
        // uint16 | glyphIdArray[ ]            | Glyph index array (arbitrary length)
        // format has already been read by this point skip it
        var length = reader.ReadUInt16();
        var language = reader.ReadUInt16();
        var segCountX2 = reader.ReadUInt16();
        var searchRange = reader.ReadUInt16();
        var entrySelector = reader.ReadUInt16();
        var rangeShift = reader.ReadUInt16();
        var segCount = segCountX2 / 2;

        using Buffer<ushort> endCountBuffer = new(segCount);
        var endCounts = endCountBuffer.GetSpan();
        reader.ReadUInt16Array(endCounts);

        var reserved = reader.ReadUInt16();

        using Buffer<ushort> startCountsBuffer = new(segCount);
        var startCounts = startCountsBuffer.GetSpan();
        reader.ReadUInt16Array(startCounts);

        using Buffer<short> idDeltaBuffer = new(segCount);
        var idDelta = idDeltaBuffer.GetSpan();
        reader.ReadInt16Array(idDelta);

        using Buffer<ushort> idRangeOffsetBuffer = new(segCount);
        var idRangeOffset = idRangeOffsetBuffer.GetSpan();
        reader.ReadUInt16Array(idRangeOffset);

        // table length thus far
        var headerLength = 16 + segCount * 8;
        var glyphIdCount = (length - headerLength) / 2;

        var glyphIds = reader.ReadUInt16Array(glyphIdCount);

        var segments = Segment.Create(endCounts, startCounts, idDelta, idRangeOffset);

        List<Format4SubTable> table = new();
        foreach (var encoding in encodings)
            table.Add(new Format4SubTable(language, encoding.PlatformID, encoding.EncodingID, segments, glyphIds));

        return table;
    }

    internal readonly struct Segment
    {
        public Segment(ushort index, ushort end, ushort start, short delta, ushort offset)
        {
            Index = index;
            End = end;
            Start = start;
            Delta = delta;
            Offset = offset;
        }

        public ushort Index { get; }

        public short Delta { get; }

        public ushort End { get; }

        public ushort Offset { get; }

        public ushort Start { get; }

        public static Segment[] Create(ReadOnlySpan<ushort> endCounts, ReadOnlySpan<ushort> startCode,
            ReadOnlySpan<short> idDelta, ReadOnlySpan<ushort> idRangeOffset)
        {
            var count = endCounts.Length;
            var segments = new Segment[count];
            for (ushort i = 0; i < count; i++)
            {
                var start = startCode[i];
                var end = endCounts[i];
                var delta = idDelta[i];
                var offset = idRangeOffset[i];
                segments[i] = new Segment(i, end, start, delta, offset);
            }

            return segments;
        }
    }
}