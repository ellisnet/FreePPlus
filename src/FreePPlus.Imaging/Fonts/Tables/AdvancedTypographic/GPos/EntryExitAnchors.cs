// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

internal sealed class EntryExitAnchors
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EntryExitAnchors" /> class.
    /// </summary>
    /// <param name="reader">The big endian binary reader.</param>
    /// <param name="offset">The offset to exitAnchor table, from beginning of CursivePos subtable.</param>
    /// <param name="entryExitRecord">Offsets to entry and exit Anchor table, from beginning of CursivePos subtable.</param>
    public EntryExitAnchors(BigEndianBinaryReader reader, long offset, EntryExitRecord entryExitRecord)
    {
        EntryAnchor = entryExitRecord.EntryAnchorOffset != 0
            ? AnchorTable.Load(reader, offset + entryExitRecord.EntryAnchorOffset)
            : null;
        ExitAnchor = entryExitRecord.ExitAnchorOffset != 0
            ? AnchorTable.Load(reader, offset + entryExitRecord.ExitAnchorOffset)
            : null;
    }

    /// <summary>
    ///     Gets the entry anchor table.
    /// </summary>
    public AnchorTable? EntryAnchor { get; }

    /// <summary>
    ///     Gets the exit anchor table.
    /// </summary>
    public AnchorTable? ExitAnchor { get; }
}