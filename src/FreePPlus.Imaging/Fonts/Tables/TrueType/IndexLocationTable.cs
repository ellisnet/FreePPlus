// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Fonts.Tables.General;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.TrueType;

//was previously: namespace SixLabors.Fonts.Tables.TrueType;

internal sealed class IndexLocationTable : Table
{
    internal const string TableName = "loca";

    public IndexLocationTable(uint[] convertedData)
    {
        GlyphOffsets = convertedData;
    }

    public uint[] GlyphOffsets { get; }

    public static IndexLocationTable? Load(FontReader fontReader)
    {
        var head = fontReader.GetTable<HeadTable>();

        var maxp = fontReader.GetTable<MaximumProfileTable>();

        // Must not get a binary reader until all depended data is retrieved in case they need to use the stream.
        if (!fontReader.TryGetReaderAtTablePosition(TableName, out var binaryReader)) return null;

        using (binaryReader)
        {
            return Load(binaryReader, maxp.GlyphCount, head.IndexLocationFormat);
        }
    }

    public static IndexLocationTable Load(BigEndianBinaryReader reader, int glyphCount,
        HeadTable.IndexLocationFormats format)
    {
        var entryCount = glyphCount + 1;

        if (format == HeadTable.IndexLocationFormats.Offset16)
        {
            // Type     | Name        | Description
            // ---------|-------------|---------------------------------------
            // Offset16 | offsets[n]  | The actual local offset divided by 2 is stored. The value of n is numGlyphs + 1. The value for numGlyphs is found in the 'maxp' table.
            using Buffer<ushort> dataBuffer = new(entryCount);
            var data = dataBuffer.GetSpan();
            reader.ReadUInt16Array(data);

            var convertedData = new uint[entryCount];
            for (var i = 0; i < entryCount; i++) convertedData[i] = (uint)(data[i] * 2);

            return new IndexLocationTable(convertedData);
        }

        if (format == HeadTable.IndexLocationFormats.Offset32)
        {
            // Type     | Name        | Description
            // ---------|-------------|---------------------------------------
            // Offset32 | offsets[n]  | The actual local offset is stored. The value of n is numGlyphs + 1. The value for numGlyphs is found in the 'maxp' table.
            var data = reader.ReadUInt32Array(entryCount);

            return new IndexLocationTable(data);
        }

        throw new InvalidFontTableException("indexToLocFormat an invalid value", "head");
    }
}