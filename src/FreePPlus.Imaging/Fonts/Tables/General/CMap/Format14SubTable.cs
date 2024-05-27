// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.IO;
using FreePPlus.Imaging.Fonts.Unicode;
using FreePPlus.Imaging.Fonts.WellKnownIds;

#pragma warning disable IDE0059
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.General.CMap;

//was previously: namespace SixLabors.Fonts.Tables.General.CMap;

/// <summary>
///     Subtable format 14 specifies the Unicode Variation Sequences (UVSes) supported by the font.
///     A Variation Sequence, according to the Unicode Standard, comprises a base character followed
///     by a variation selector. For example, &lt;U+82A6, U+E0101&gt;.
/// </summary>
internal sealed class Format14SubTable : CMapSubTable
{
    private readonly Dictionary<int, VariationSelector> variationSelectors;

    private Format14SubTable(Dictionary<int, VariationSelector> variationSelectors, PlatformIDs platform,
        ushort encoding)
        : base(platform, encoding, 5)
    {
        this.variationSelectors = variationSelectors;
    }

    public static IEnumerable<Format14SubTable> Load(
        IEnumerable<EncodingRecord> encodings,
        BigEndianBinaryReader reader,
        long offset)
    {
        // +-------------------+------------------------------------+------------------------------------------------------+
        // | Type              | Name                               | Description                                          |
        // +===================+====================================+======================================================+
        // | uint16            | format                             | Subtable format. Set to 14.                          |
        // +-------------------+------------------------------------+------------------------------------------------------+
        // | uint32            | length                             | Byte length of this subtable (including this header) |
        // +-------------------+------------------------------------+------------------------------------------------------+
        // | uint32            | numVarSelectorRecords              | Number of variation Selector Records                 |
        // +-------------------+------------------------------------+------------------------------------------------------+
        // | VariationSelector | varSelector[numVarSelectorRecords] | Array of VariationSelector records.                  |
        // +-------------------+------------------------------------+------------------------------------------------------+
        var length = reader.ReadUInt32();
        var numVarSelectorRecords = reader.ReadUInt32();

        var variationSelectors = new Dictionary<int, VariationSelector>();
        var varSelectors = new int[numVarSelectorRecords];
        var defaultUVSOffsets = new uint[numVarSelectorRecords];
        var nonDefaultUVSOffsets = new uint[numVarSelectorRecords];
        for (var i = 0; i < numVarSelectorRecords; ++i)
        {
            // +----------+---------------------+----------------------------------------------------+
            // | Type     | Name                | Description                                        |
            // +==========+=====================+====================================================+
            // | uint24   | varSelector         | Variation selector                                 |
            // +----------+---------------------+----------------------------------------------------+
            // | Offset32 | defaultUVSOffset    | Offset from the start of the format 14 subtable to |
            // |          |                     | Default UVS Table. May be 0.                       |
            // +----------+---------------------+----------------------------------------------------+
            // | Offset32 | nonDefaultUVSOffset | Offset from the start of the format 14 subtable to |
            // |          |                     | Non-Default UVS Table. May be 0.                   |
            // +----------+---------------------+----------------------------------------------------+
            varSelectors[i] = reader.ReadUInt24();
            defaultUVSOffsets[i] = reader.ReadUInt32();
            nonDefaultUVSOffsets[i] = reader.ReadUInt32();
        }

        for (var i = 0; i < numVarSelectorRecords; ++i)
        {
            var selector = new VariationSelector();
            if (defaultUVSOffsets[i] != 0)
            {
                // Default UVS table
                // +--------------+-------------------------------+-------------------------------------+
                // | Type         | Name                          | Description                         |
                // +==============+===============================+=====================================+
                // | uint32       | numUnicodeValueRanges         | Number of Unicode character ranges. |
                // +--------------+-------------------------------+-------------------------------------+
                // | UnicodeRange | ranges[numUnicodeValueRanges] | Array of UnicodeRange records.      |
                // +--------------+-------------------------------+-------------------------------------+

                // UnicodeRange Record
                // +--------+-------------------+-------------------------------------------+
                // | Type   | Name              | Description                               |
                // +========+===================+===========================================+
                // | uint24 | startUnicodeValue | First value in this range                 |
                // +--------+-------------------+-------------------------------------------+
                // | uint8  | additionalCount   | Number of additional values in this range |
                // +--------+-------------------+-------------------------------------------+
                reader.Seek(offset + defaultUVSOffsets[i], SeekOrigin.Begin);
                var numUnicodeValueRanges = reader.ReadUInt32();
                for (var n = 0; n < numUnicodeValueRanges; n++)
                {
                    var startCode = reader.ReadUInt24();
                    selector.DefaultStartCodes.Add(startCode);
                    selector.DefaultEndCodes.Add(startCode + reader.ReadByte());
                }
            }

            if (nonDefaultUVSOffsets[i] != 0)
            {
                // Non-Default UVS table
                // +------------+-----------------------------+------------------------------------+
                // | Type       | Name                        | Description                        |
                // +============+=============================+====================================+
                // | uint32     | numUVSMappings              | Number of UVS Mappings that follow |
                // +------------+-----------------------------+------------------------------------+
                // | UVSMapping | uvsMappings[numUVSMappings] | Array of UVSMapping records.       |
                // +------------+-----------------------------+------------------------------------+

                // UVSMapping Record
                // +--------+--------------+-------------------------------+
                // | Type   | Name         | Description                   |
                // +========+==============+===============================+
                // | uint24 | unicodeValue | Base Unicode value of the UVS |
                // +--------+--------------+-------------------------------+
                // | uint16 | glyphID      | Glyph ID of the UVS           |
                // +--------+--------------+-------------------------------+
                reader.Seek(offset + nonDefaultUVSOffsets[i], SeekOrigin.Begin);
                var numUVSMappings = reader.ReadUInt32();
                for (var n = 0; n < numUVSMappings; n++)
                {
                    var unicodeValue = reader.ReadUInt24();
                    var glyphID = reader.ReadUInt16();
                    selector.UVSMappings.Add(unicodeValue, glyphID);
                }
            }

            variationSelectors.Add(varSelectors[i], selector);
        }

        foreach (var encoding in encodings)
            yield return new Format14SubTable(variationSelectors, encoding.PlatformID, encoding.EncodingID);
    }

    public override bool TryGetGlyphId(CodePoint codePoint, out ushort glyphId)
    {
        glyphId = 0;
        return false;
    }

    public ushort CharacterPairToGlyphId(CodePoint codePoint, ushort defaultGlyphIndex, CodePoint nextCodePoint)
    {
        // Only check codepoint if nextCodepoint is a variation selector
        if (variationSelectors.TryGetValue(nextCodePoint.Value, out var sel))
        {
            // If the sequence is a non-default UVS, return the mapped glyph
            if (sel.UVSMappings.TryGetValue(codePoint.Value, out var ret)) return ret;

            // If the sequence is a default UVS, return the default glyph
            for (var i = 0; i < sel.DefaultStartCodes.Count; ++i)
                if (codePoint.Value >= sel.DefaultStartCodes[i] && codePoint.Value < sel.DefaultEndCodes[i])
                    return defaultGlyphIndex;

            // At this point we are neither a non-default UVS nor a default UVS,
            // but we know the nextCodepoint is a variation selector. Unicode says
            // this glyph should be invisible: “no visible rendering for the VS”
            // (http://unicode.org/faq/unsup_char.html#4)
            return defaultGlyphIndex;
        }

        // In all other cases, return 0
        return 0;
    }

    private class VariationSelector
    {
        public List<int> DefaultStartCodes { get; } = new();

        public List<int> DefaultEndCodes { get; } = new();

        public Dictionary<int, ushort> UVSMappings { get; } = new();
    }
}