// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;

#pragma warning disable IDE0059
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.General.Kern;

//was previously: namespace SixLabors.Fonts.Tables.General.Kern;

internal abstract class KerningSubTable
{
    private readonly KerningCoverage coverage;

    public KerningSubTable(KerningCoverage coverage)
    {
        this.coverage = coverage;
    }

    public static KerningSubTable? Load(BigEndianBinaryReader reader)
    {
        // Kerning subtables will share the same header format.
        // This header is used to identify the format of the subtable and the kind of information it contains:
        // +--------+----------+----------------------------------------------------------+
        // | Type   | Field    | Description                                              |
        // +========+==========+==========================================================+
        // | uint16 | version  | Kern subtable version number                             |
        // +--------+----------+----------------------------------------------------------+
        // | uint16 | length   | Length of the subtable, in bytes(including this header). |
        // +--------+----------+----------------------------------------------------------+
        // | uint16 | coverage | What type of information is contained in this table.     |
        // +--------+----------+----------------------------------------------------------+
        var subVersion = reader.ReadUInt16();
        var length = reader.ReadUInt16();
        var coverage = KerningCoverage.Read(reader);
        if (coverage.Format == 0)
            return Format0SubTable.Load(reader, coverage);
        // we don't support versions other than 'Format 0' same as Windows
        return null;
    }

    protected abstract bool TryGetOffset(ushort index1, ushort index2, out short offset);

    public bool TryApplyOffset(ushort index1, ushort index2, ref Vector2 result)
    {
        if (TryGetOffset(index1, index2, out var offset))
        {
            if (coverage.Horizontal)
            {
                // apply to X
                if (coverage.OverrideAccumulator)
                    result.X = offset;
                else
                    result.X += offset;
            }
            else
            {
                // apply to Y
                if (coverage.OverrideAccumulator)
                    result.Y = offset;
                else
                    result.Y += offset;
            }

            return true;
        }

        return false;
    }
}