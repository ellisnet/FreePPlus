// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

internal sealed class MarkGlyphSetsTable
{
    public ushort Format { get; internal set; }

    public ushort[]? CoverageOffset { get; internal set; }

    public static MarkGlyphSetsTable Load(BigEndianBinaryReader reader, long offset)
    {
        reader.Seek(offset, SeekOrigin.Begin);

        var markGlyphSetsTable = new MarkGlyphSetsTable
        {
            Format = reader.ReadUInt16()
        };
        var markSetCount = reader.ReadUInt16();
        markGlyphSetsTable.CoverageOffset = reader.ReadUInt16Array(markSetCount);

        return markGlyphSetsTable;
    }
}