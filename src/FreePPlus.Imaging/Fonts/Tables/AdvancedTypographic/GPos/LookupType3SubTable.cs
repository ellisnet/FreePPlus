// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

/// <summary>
///     Cursive Attachment Positioning Subtable.
///     Some cursive fonts are designed so that adjacent glyphs join when rendered with their default positioning.
///     However, if positioning adjustments are needed to join the glyphs, a cursive attachment positioning (CursivePos)
///     subtable can describe
///     how to connect the glyphs by aligning two anchor points: the designated exit point of a glyph, and the designated
///     entry point of the following glyph.
///     <see
///         href="https://docs.microsoft.com/en-us/typography/opentype/spec/gpos#cursive-attachment-positioning-format1-cursive-attachment" />
/// </summary>
internal static class LookupType3SubTable
{
    public static LookupSubTable Load(BigEndianBinaryReader reader, long offset, LookupFlags lookupFlags)
    {
        reader.Seek(offset, SeekOrigin.Begin);
        var posFormat = reader.ReadUInt16();

        return posFormat switch
        {
            1 => LookupType3Format1SubTable.Load(reader, offset, lookupFlags),
            _ => new NotImplementedSubTable()
        };
    }

    internal sealed class LookupType3Format1SubTable : LookupSubTable
    {
        private readonly CoverageTable coverageTable;
        private readonly EntryExitAnchors[] entryExitAnchors;

        public LookupType3Format1SubTable(CoverageTable coverageTable, EntryExitAnchors[] entryExitAnchors,
            LookupFlags lookupFlags)
            : base(lookupFlags)
        {
            this.coverageTable = coverageTable;
            this.entryExitAnchors = entryExitAnchors;
        }

        public static LookupType3Format1SubTable Load(BigEndianBinaryReader reader, long offset,
            LookupFlags lookupFlags)
        {
            // Cursive Attachment Positioning Format1.
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | Type               |  Name                           | Description                                          |
            // +====================+=================================+======================================================+
            // | uint16             | posFormat                       | Format identifier: format = 1                        |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | Offset16           | coverageOffset                  | Offset to Coverage table,                            |
            // |                    |                                 | from beginning of CursivePos subtable.               |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | uint16             | entryExitCount                  | Number of EntryExit records.                         |
            // +--------------------+---------------------------------+------------------------------------------------------+
            // | EntryExitRecord    | entryExitRecord[entryExitCount] | Array of EntryExit records, in Coverage index order. |
            // +--------------------+---------------------------------+------------------------------------------------------+
            var coverageOffset = reader.ReadOffset16();
            var entryExitCount = reader.ReadUInt16();
            var entryExitRecords = new EntryExitRecord[entryExitCount];
            for (var i = 0; i < entryExitCount; i++) entryExitRecords[i] = new EntryExitRecord(reader, offset);

            var entryExitAnchors = new EntryExitAnchors[entryExitCount];
            for (var i = 0; i < entryExitCount; i++)
                entryExitAnchors[i] = new EntryExitAnchors(reader, offset, entryExitRecords[i]);

            var coverageTable = CoverageTable.Load(reader, offset + coverageOffset);

            return new LookupType3Format1SubTable(coverageTable, entryExitAnchors, lookupFlags);
        }

        public override bool TryUpdatePosition(
            FontMetrics fontMetrics,
            GPosTable table,
            GlyphPositioningCollection collection,
            Tag feature,
            int index,
            int count)
        {
            if (count <= 1) return false;

            // Implements Cursive Attachment Positioning Subtable:
            // https://docs.microsoft.com/en-us/typography/opentype/spec/gpos#lookup-type-3-cursive-attachment-positioning-subtable
            var glyphId = collection[index];
            if (glyphId == 0) return false;

            var nextIndex = index + 1;
            var nextGlyphId = collection[nextIndex];
            if (nextGlyphId == 0) return false;

            var coverageNext = coverageTable.CoverageIndexOf(nextGlyphId);
            if (coverageNext < 0) return false;

            var nextRecord = entryExitAnchors[coverageNext];
            var entry = nextRecord.EntryAnchor;
            if (entry is null) return false;

            var coverage = coverageTable.CoverageIndexOf(glyphId);
            if (coverage < 0) return false;

            var curRecord = entryExitAnchors[coverage];
            var exit = curRecord.ExitAnchor;
            if (exit is null) return false;

            var current = collection.GetGlyphShapingData(index);
            var next = collection.GetGlyphShapingData(nextIndex);

            var exitXY = exit.GetAnchor(fontMetrics, current, collection);
            var entryXY = entry.GetAnchor(fontMetrics, next, collection);

            if (!collection.IsVerticalLayoutMode)
            {
                // Horizontal
                if (current.Direction == TextDirection.LeftToRight)
                {
                    current.Bounds.Width = exitXY.XCoordinate + current.Bounds.X;

                    var delta = entryXY.XCoordinate + next.Bounds.X;
                    next.Bounds.Width -= delta;
                    next.Bounds.X -= delta;
                }
                else
                {
                    var delta = exitXY.XCoordinate + current.Bounds.X;
                    current.Bounds.Width -= delta;
                    current.Bounds.X -= delta;

                    next.Bounds.Width = entryXY.XCoordinate + next.Bounds.X;
                }
            }
            else
            {
                // Vertical : Top to bottom
                if (current.Direction == TextDirection.LeftToRight)
                {
                    current.Bounds.Height = exitXY.YCoordinate + current.Bounds.Y;

                    var delta = entryXY.YCoordinate + next.Bounds.Y;
                    next.Bounds.Height -= delta;
                    next.Bounds.Y -= delta;
                }
                else
                {
                    var delta = exitXY.YCoordinate + current.Bounds.Y;
                    current.Bounds.Height -= delta;
                    current.Bounds.Y -= delta;

                    next.Bounds.Height = entryXY.YCoordinate + next.Bounds.Y;
                }
            }

            var child = index;
            var parent = nextIndex;
            var xOffset = entryXY.XCoordinate - exitXY.XCoordinate;
            var yOffset = entryXY.YCoordinate - exitXY.YCoordinate;
            if (LookupFlags.HasFlag(LookupFlags.RightToLeft))
            {
                (parent, child) = (child, parent);

                xOffset = -xOffset;
                yOffset = -yOffset;
            }

            // If child was already connected to someone else, walk through its old
            // chain and reverse the link direction, such that the whole tree of its
            // previous connection now attaches to new parent.Watch out for case
            // where new parent is on the path from old chain...
            var horizontal = !collection.IsVerticalLayoutMode;
            ReverseCursiveMinorOffset(collection, index, child, horizontal, parent);

            var c = collection.GetGlyphShapingData(child);
            c.CursiveAttachment = parent - child;
            if (horizontal)
                c.Bounds.Y = yOffset;
            else
                c.Bounds.X = xOffset;

            // If parent was attached to child, separate them.
            var p = collection.GetGlyphShapingData(parent);
            if (p.CursiveAttachment == -c.CursiveAttachment) p.CursiveAttachment = 0;

            return true;
        }

        private static void ReverseCursiveMinorOffset(
            GlyphPositioningCollection collection,
            int position,
            int i,
            bool horizontal,
            int parent)
        {
            var c = collection.GetGlyphShapingData(i);
            var chain = c.CursiveAttachment;
            if (chain <= 0) return;

            c.CursiveAttachment = 0;

            var j = i + chain;

            // Stop if we see new parent in the chain.
            if (j == parent) return;

            ReverseCursiveMinorOffset(collection, position, j, horizontal, parent);

            var p = collection.GetGlyphShapingData(j);
            if (horizontal)
                p.Bounds.Y = -c.Bounds.Y;
            else
                p.Bounds.X = -c.Bounds.X;

            p.CursiveAttachment = -chain;
        }
    }
}