// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.General;

//was previously: namespace SixLabors.Fonts.Tables.General;

internal sealed class MaximumProfileTable : Table
{
    internal const string TableName = "maxp";

    public MaximumProfileTable(ushort numGlyphs)
    {
        GlyphCount = numGlyphs;
    }

    public MaximumProfileTable(ushort numGlyphs, ushort maxPoints, ushort maxContours, ushort maxCompositePoints,
        ushort maxCompositeContours, ushort maxZones, ushort maxTwilightPoints, ushort maxStorage,
        ushort maxFunctionDefs, ushort maxInstructionDefs, ushort maxStackElements, ushort maxSizeOfInstructions,
        ushort maxComponentElements, ushort maxComponentDepth)
        : this(numGlyphs)
    {
        MaxPoints = maxPoints;
        MaxContours = maxContours;
        MaxCompositePoints = maxCompositePoints;
        MaxCompositeContours = maxCompositeContours;
        MaxZones = maxZones;
        MaxTwilightPoints = maxTwilightPoints;
        MaxStorage = maxStorage;
        MaxFunctionDefs = maxFunctionDefs;
        MaxInstructionDefs = maxInstructionDefs;
        MaxStackElements = maxStackElements;
        MaxSizeOfInstructions = maxSizeOfInstructions;
        MaxComponentElements = maxComponentElements;
        MaxComponentDepth = maxComponentDepth;
    }

    public ushort MaxPoints { get; }

    public ushort MaxContours { get; }

    public ushort MaxCompositePoints { get; }

    public ushort MaxCompositeContours { get; }

    public ushort MaxZones { get; }

    public ushort MaxTwilightPoints { get; }

    public ushort MaxStorage { get; }

    public ushort MaxFunctionDefs { get; }

    public ushort MaxInstructionDefs { get; }

    public ushort MaxStackElements { get; }

    public ushort MaxSizeOfInstructions { get; }

    public ushort MaxComponentElements { get; }

    public ushort MaxComponentDepth { get; }

    public ushort GlyphCount { get; }

    public static MaximumProfileTable Load(FontReader reader)
    {
        using (var r = reader.GetReaderAtTablePosition(TableName))
        {
            return Load(r);
        }
    }

    public static MaximumProfileTable Load(BigEndianBinaryReader reader)
    {
        // This table establishes the memory requirements for this font.Fonts with CFF data must use Version 0.5 of this table, specifying only the numGlyphs field.Fonts with TrueType outlines must use Version 1.0 of this table, where all data is required.
        // Version 0.5
        // Type   | Name                 | Description
        // -------|----------------------|---------------------------------------
        // Fixed  | Table version number | 0x00005000 for version 0.5 (Note the difference in the representation of a non - zero fractional part, in Fixed numbers.)
        // uint16 | numGlyphs            | The number of glyphs in the font.
        var version = reader.ReadFixed();
        var numGlyphs = reader.ReadUInt16();
        if (version == 0.5) return new MaximumProfileTable(numGlyphs);

        // Version 1.0
        // Type   | Name                  | Description
        // -------|-----------------------|---------------------------------------
        // *Fixed | Table version number  | 0x00010000 for version 1.0.
        // *uint16| numGlyphs             | The number of glyphs in the font.
        // uint16 | maxPoints             | Maximum points in a non - composite glyph.
        // uint16 | maxContours           | Maximum contours in a non - composite glyph.
        // uint16 | maxCompositePoints    | Maximum points in a composite glyph.
        // uint16 | maxCompositeContours  | Maximum contours in a composite glyph.
        // uint16 | maxZones              | 1 if instructions do not use the twilight zone (Z0), or 2 if instructions do use Z0; should be set to 2 in most cases.
        // uint16 | maxTwilightPoints     | Maximum points used in Z0.
        // uint16 | maxStorage            | Number of Storage Area locations.
        // uint16 | maxFunctionDefs       | Number of FDEFs, equals to the highest function number +1.
        // uint16 | maxInstructionDefs    | Number of IDEFs.
        // uint16 | maxStackElements      | Maximum stack depth2.
        // uint16 | maxSizeOfInstructions | Maximum byte count for glyph instructions.
        // uint16 | maxComponentElements  | Maximum number of components referenced at “top level” for any composite glyph.
        // uint16 | maxComponentDepth     | Maximum levels of recursion; 1 for simple components.
        var maxPoints = reader.ReadUInt16();
        var maxContours = reader.ReadUInt16();
        var maxCompositePoints = reader.ReadUInt16();
        var maxCompositeContours = reader.ReadUInt16();

        var maxZones = reader.ReadUInt16();
        var maxTwilightPoints = reader.ReadUInt16();
        var maxStorage = reader.ReadUInt16();
        var maxFunctionDefs = reader.ReadUInt16();
        var maxInstructionDefs = reader.ReadUInt16();
        var maxStackElements = reader.ReadUInt16();
        var maxSizeOfInstructions = reader.ReadUInt16();
        var maxComponentElements = reader.ReadUInt16();
        var maxComponentDepth = reader.ReadUInt16();

        return new MaximumProfileTable(
            numGlyphs,
            maxPoints,
            maxContours,
            maxCompositePoints,
            maxCompositeContours,
            maxZones,
            maxTwilightPoints,
            maxStorage,
            maxFunctionDefs,
            maxInstructionDefs,
            maxStackElements,
            maxSizeOfInstructions,
            maxComponentElements,
            maxComponentDepth);
    }
}