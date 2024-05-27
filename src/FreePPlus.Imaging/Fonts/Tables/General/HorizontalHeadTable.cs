// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

#pragma warning disable IDE0059
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.General;

//was previously: namespace SixLabors.Fonts.Tables.General;

internal class HorizontalHeadTable : Table
{
    internal const string TableName = "hhea";

    public HorizontalHeadTable(
        short ascender,
        short descender,
        short lineGap,
        ushort advanceWidthMax,
        short minLeftSideBearing,
        short minRightSideBearing,
        short xMaxExtent,
        short caretSlopeRise,
        short caretSlopeRun,
        short caretOffset,
        ushort numberOfHMetrics)
    {
        Ascender = ascender;
        Descender = descender;
        LineGap = lineGap;
        AdvanceWidthMax = advanceWidthMax;
        MinLeftSideBearing = minLeftSideBearing;
        MinRightSideBearing = minRightSideBearing;
        XMaxExtent = xMaxExtent;
        CaretSlopeRise = caretSlopeRise;
        CaretSlopeRun = caretSlopeRun;
        CaretOffset = caretOffset;
        NumberOfHMetrics = numberOfHMetrics;
    }

    public ushort AdvanceWidthMax { get; }

    public short Ascender { get; }

    public short CaretOffset { get; }

    public short CaretSlopeRise { get; }

    public short CaretSlopeRun { get; }

    public short Descender { get; }

    public short LineGap { get; }

    public short MinLeftSideBearing { get; }

    public short MinRightSideBearing { get; }

    public ushort NumberOfHMetrics { get; }

    public short XMaxExtent { get; }

    public static HorizontalHeadTable? Load(FontReader fontReader)
    {
        if (!fontReader.TryGetReaderAtTablePosition(TableName, out var binaryReader)) return null;

        using (binaryReader)
        {
            return Load(binaryReader);
        }
    }

    public static HorizontalHeadTable Load(BigEndianBinaryReader reader)
    {
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | Type   | Name                | Description                                                                     |
        // +========+=====================+=================================================================================+
        // | Fixed  | version             | 0x00010000 (1.0)                                                                |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | FWord  | ascent              | Distance from baseline of highest ascender                                      |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | FWord  | descent             | Distance from baseline of lowest descender                                      |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | FWord  | lineGap             | typographic line gap                                                            |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | uFWord | advanceWidthMax     | must be consistent with horizontal metrics                                      |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | FWord  | minLeftSideBearing  | must be consistent with horizontal metrics                                      |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | FWord  | minRightSideBearing | must be consistent with horizontal metrics                                      |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | FWord  | xMaxExtent          | max(lsb + (xMax-xMin))                                                          |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | int16  | caretSlopeRise      | used to calculate the slope of the caret (rise/run) set to 1 for vertical caret |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | int16  | caretSlopeRun       | 0 for vertical                                                                  |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | FWord  | caretOffset         | set value to 0 for non-slanted fonts                                            |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | int16  | reserved            | set value to 0                                                                  |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | int16  | reserved            | set value to 0                                                                  |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | int16  | reserved            | set value to 0                                                                  |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | int16  | reserved            | set value to 0                                                                  |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | int16  | metricDataFormat    | 0 for current format                                                            |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        // | uint16 | numOfLongHorMetrics | number of advance widths in metrics table                                       |
        // +--------+---------------------+---------------------------------------------------------------------------------+
        var majorVersion = reader.ReadUInt16();
        var minorVersion = reader.ReadUInt16();
        var ascender = reader.ReadFWORD();
        var descender = reader.ReadFWORD();
        var lineGap = reader.ReadFWORD();
        var advanceWidthMax = reader.ReadUFWORD();
        var minLeftSideBearing = reader.ReadFWORD();
        var minRightSideBearing = reader.ReadFWORD();
        var xMaxExtent = reader.ReadFWORD();
        var caretSlopeRise = reader.ReadInt16();
        var caretSlopeRun = reader.ReadInt16();
        var caretOffset = reader.ReadInt16();
        reader.ReadInt16(); // reserved
        reader.ReadInt16(); // reserved
        reader.ReadInt16(); // reserved
        reader.ReadInt16(); // reserved
        var metricDataFormat = reader.ReadInt16(); // 0
        if (metricDataFormat != 0)
            throw new InvalidFontTableException($"Expected metricDataFormat = 0 found {metricDataFormat}", TableName);

        var numberOfHMetrics = reader.ReadUInt16();

        return new HorizontalHeadTable(
            ascender,
            descender,
            lineGap,
            advanceWidthMax,
            minLeftSideBearing,
            minRightSideBearing,
            xMaxExtent,
            caretSlopeRise,
            caretSlopeRun,
            caretOffset,
            numberOfHMetrics);
    }
}