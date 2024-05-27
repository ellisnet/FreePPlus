// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Numerics;
using FreePPlus.Imaging.Fonts.Tables.TrueType.Glyphs;

namespace FreePPlus.Imaging.Fonts.Tables.Woff;

//was previously: namespace SixLabors.Fonts.Tables.Woff;

// Source code is based on https://github.com/LayoutFarm/Typography
// see https://github.com/LayoutFarm/Typography/blob/master/Typography.OpenFont/WebFont/Woff2Reader.cs
// TODO: There's still some cleanup required here to bring the code up to a maintainable standard.
internal static class Woff2Utils
{
    private const byte OneMoreByteCode1 = 255;
    private const byte OneMoreByteCode2 = 254;
    private const byte WordCode = 253;

    private const byte LowestUCode = 253;

    // We don't reuse the const tag headers from our table types for clarity.
    private static readonly string[] KnownTableTags =
    {
        "cmap", "head", "hhea", "hmtx", "maxp", "name", "OS/2", "post", "cvt ",
        "fpgm", "glyf", "loca", "prep", "CFF ", "VORG", "EBDT", "EBLC", "gasp",
        "hdmx", "kern", "LTSH", "PCLT", "VDMX", "vhea", "vmtx", "BASE", "GDEF",
        "GPOS", "GSUB", "EBSC", "JSTF", "MATH", "CBDT", "CBLC", "COLR", "CPAL",
        "SVG ", "sbix", "acnt", "avar", "bdat", "bloc", "bsln", "cvar", "fdsc",
        "feat", "fmtx", "fvar", "gvar", "hsty", "just", "lcar", "mort", "morx",
        "opbd", "prop", "trak", "Zapf", "Silf", "Glat", "Gloc", "Feat", "Sill"
    };

    public static ReadOnlyDictionary<string, TableHeader> ReadWoff2Headers(BigEndianBinaryReader reader, int tableCount)
    {
        uint expectedTableStartAt = 0;
        var headers = new Dictionary<string, TableHeader>(tableCount);
        for (var i = 0; i < tableCount; i++)
        {
            var woffTableHeader = Read(reader, expectedTableStartAt, out var nextExpectedTableStartAt);
            expectedTableStartAt = nextExpectedTableStartAt;
            headers.Add(woffTableHeader.Tag, woffTableHeader);
        }

        return new ReadOnlyDictionary<string, TableHeader>(headers);
    }

    public static Woff2TableHeader Read(BigEndianBinaryReader reader, uint expectedTableStartAt,
        out uint nextExpectedTableStartAt)
    {
        // Leave the first byte open to store flagByte
        const uint woff2FlagsTransform = 1 << 8;
        var flagsByte = reader.ReadByte();
        var knownTable = flagsByte & 0x3F;
        var tableName = knownTable == 0x3F ? reader.ReadTag() : KnownTableTags[knownTable];

        uint flags = 0;
        var xformVersion = (byte)((flagsByte >> 6) & 0x03);

        // 0 means xform for glyph/loca, non-0 for others
        if (tableName is "glyf" or "loca")
        {
            if (xformVersion == 0) flags |= woff2FlagsTransform;
        }
        else if (xformVersion != 0)
        {
            flags |= woff2FlagsTransform;
        }

        flags |= xformVersion;

        if (!ReadUIntBase128(reader, out var tableOrigLength))
            throw new FontException("Error parsing woff2 table header");

        var tableTransformLength = tableOrigLength;
        if ((flags & woff2FlagsTransform) != 0)
        {
            if (!ReadUIntBase128(reader, out tableTransformLength))
                throw new FontException("Error parsing woff2 table header");

            if (tableName == "loca" && tableTransformLength > 0)
                throw new FontException("Error parsing woff2 table header");
        }

        nextExpectedTableStartAt = expectedTableStartAt + tableTransformLength;
        if (nextExpectedTableStartAt < expectedTableStartAt)
            throw new FontException("Error parsing woff2 table header");

        return new Woff2TableHeader(tableName, 0, expectedTableStartAt, tableTransformLength);
    }

    public static GlyphLoader[] LoadAllGlyphs(BigEndianBinaryReader reader, EmptyGlyphLoader emptyGlyphLoader)
    {
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | Data Type | Semantic              | Description and value type (if applicable)                                                            |
        // +===========+=======================+=======================================================================================================+
        // | Fixed     | version               | = 0x00000000                                                                                          |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | UInt16    | numGlyphs             | Number of glyphs                                                                                      |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | UInt16    | indexFormat           | Offset format for loca table, should be consistent with indexToLocFormat                              |
        // |           |                       | of the original head table (see specification)                                                        |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | UInt32    | nContourStreamSize    | Size of nContour stream in bytes                                                                      |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | UInt32    | nPointsStreamSize     | Size of nPoints stream in bytes                                                                       |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | UInt32    | flagStreamSize        | Size of flag stream in bytes                                                                          |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | UInt32    | glyphStreamSize       | Size of glyph stream in bytes (a stream of variable-length encoded values, see description below)     |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | UInt32    | compositeStreamSize   | Size of composite stream in bytes (a stream of variable-length encoded values, see description below) |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | UInt32    | bboxStreamSize        | Size of bbox data in bytes representing combined length of bboxBitmap (a packed bit array)            |
        // |           |                       | and bboxStream (a stream of Int16 values)                                                             |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | UInt32    | instructionStreamSize | Size of instruction stream (a stream of UInt8 values)                                                 |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | Int16     | nContourStream[]      | Stream of Int16 values representing number of contours for each glyph record                          |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | 255UInt16 | nPointsStream[]       | Stream of values representing number of outline points for each contour in glyph records              |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | UInt8     | flagStream[]          | Stream of UInt8 values representing flag values for each outline point.                               |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | Vary      | glyphStream[]         | Stream of bytes representing point coordinate values using variable length                            |
        // |           |                       | encoding format (defined in subclause 5.2)                                                            |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | Vary      | compositeStream[]     | Stream of bytes representing component flag values and associated composite glyph data                |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | UInt8     | bboxBitmap[]          | Bitmap (a numGlyphs-long bit array) indicating explicit bounding boxes                                |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | Int16     | bboxStream[]          | Stream of Int16 values representing glyph bounding box data                                           |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        // | UInt8     | instructionStream[]   | Stream of UInt8 values representing a set of instructions for each corresponding glyph                |
        // +-----------+-----------------------+-------------------------------------------------------------------------------------------------------+
        var version = reader.ReadUInt32();
        var numGlyphs = reader.ReadUInt16();
        var indexFormatOffset = reader.ReadUInt16();

        var nContourStreamSize = reader.ReadUInt32();
        var nPointsStreamSize = reader.ReadUInt32();
        var flagStreamSize = reader.ReadUInt32();
        var glyphStreamSize = reader.ReadUInt32();
        var compositeStreamSize = reader.ReadUInt32();
        var bboxStreamSize = reader.ReadUInt32();
        var instructionStreamSize = reader.ReadUInt32();

        var nCountStreamOffset = reader.BaseStream.Position;
        var nPointStreamOffset = nCountStreamOffset + nContourStreamSize;
        var flagStreamOffset = nPointStreamOffset + nPointsStreamSize;
        var glyphStreamOffset = flagStreamOffset + flagStreamSize;
        var compositeStreamOffset = glyphStreamOffset + glyphStreamSize;

        var bboxStreamOffset = compositeStreamOffset + compositeStreamSize;
        var instructionStreamOffset = bboxStreamOffset + bboxStreamSize;

        var glyphs = new GlyphVector[numGlyphs];
        var allGlyphs = new GlyphData[numGlyphs];
        var glyphLoaders = new GlyphLoader[numGlyphs];
        var compositeGlyphs = new List<ushort>();
        var contourCount = 0;
        for (ushort i = 0; i < numGlyphs; i++)
        {
            var numContour = reader.ReadInt16();
            allGlyphs[i] = new GlyphData(i, numContour);
            if (numContour > 0)
                contourCount += numContour;
            // >0 => simple glyph
            // -1 = composite
            // 0 = empty glyph
            else if (numContour < 0)
                // Composite glyph, resolve later.
                compositeGlyphs.Add(i);
        }

        var pntPerContours = new ushort[contourCount];
        for (var i = 0; i < contourCount; i++)
            // Each of these is the number of points of that contour.
            pntPerContours[i] = Read255UInt16(reader);

        // FlagStream, flags value for each point.
        // Each byte in flags stream represents one point.
        var flagStream = reader.ReadBytes((int)flagStreamSize);

        // Some composite glyphs have instructions so we must check all composite glyphs before read the glyph stream.
        using (var compositeMemoryStream = new MemoryStream())
        {
            reader.BaseStream.Position = compositeStreamOffset;
            compositeMemoryStream.Write(reader.ReadBytes((int)compositeStreamSize), 0, (int)compositeStreamSize);
            compositeMemoryStream.Position = 0;

            using (var compositeReader = new BigEndianBinaryReader(compositeMemoryStream, false))
            {
                for (ushort i = 0; i < compositeGlyphs.Count; i++)
                {
                    var compositeGlyphIndex = compositeGlyphs[i];
                    allGlyphs[compositeGlyphIndex].CompositeHasInstructions = CompositeHasInstructions(compositeReader);
                }
            }

            reader.BaseStream.Position = glyphStreamOffset;
        }

        var curFlagsIndex = 0;
        var pntContourIndex = 0;
        var instructionsIndex = instructionStreamOffset;
        for (var i = 0; i < allGlyphs.Length; i++)
            glyphs[i] = ReadSimpleGlyphData(
                reader,
                ref allGlyphs[i],
                pntPerContours,
                ref pntContourIndex,
                flagStream,
                ref curFlagsIndex,
                ref instructionsIndex);

        // Now we read the composite stream again and create composite glyphs.
        for (ushort i = 0; i < compositeGlyphs.Count; i++)
        {
            int compositeGlyphIndex = compositeGlyphs[i];
            glyphs[compositeGlyphIndex] = ReadCompositeGlyphData(glyphs, reader);
        }

        var bitmapCount = (numGlyphs + 7) / 8;
        var bboxBitmap = ExpandBitmap(reader.ReadBytes(bitmapCount));
        for (ushort i = 0; i < numGlyphs; i++)
        {
            var tempGlyph = allGlyphs[i];
            var hasBbox = bboxBitmap[i];
            if (hasBbox == 1)
                // Read bbox from the bboxstream.
                glyphs[i] = GlyphVector.WithCompositeBounds(glyphs[i], Bounds.Load(reader));
            else if (tempGlyph.NumContour < 0)
                throw new NotSupportedException("Composite glyph must have a bounding box.");
        }

        for (ushort i = 0; i < numGlyphs; i++)
        {
            if (!glyphs[i].HasValue())
            {
                glyphLoaders[i] = emptyGlyphLoader;
                continue;
            }

            glyphLoaders[i] = new TransformedGlyphLoader(glyphs[i]);
        }

        return glyphLoaders;
    }

    private static GlyphVector ReadSimpleGlyphData(
        BigEndianBinaryReader reader,
        ref GlyphData glyphData,
        ushort[] pntPerContours,
        ref int pntContourIndex,
        byte[] flagStream,
        ref int flagStreamIndex,
        ref long instructionStreamOffset)
    {
        if (glyphData.NumContour == 0) return default;

        if (glyphData.NumContour < 0)
        {
            // Composite glyph. Check if this has instruction or not
            // and read the length. We don't actually use the data but it ensures
            // we maintain the correct location within the stream.
            if (glyphData.CompositeHasInstructions) Read255UInt16(reader);

            return default; // Skip composite glyph (resolve later).
        }

        var curX = 0;
        var curY = 0;
        int numContour = glyphData.NumContour;
        var endPoints = new ushort[numContour];
        ushort pointCount = 0;

        for (ushort i = 0; i < numContour; i++)
        {
            var numPoint = pntPerContours[pntContourIndex++];
            pointCount += numPoint;
            endPoints[i] = (ushort)(pointCount - 1);
        }

        var controlPoints = new Vector2[pointCount];
        var onCurves = new bool[pointCount];
        var n = 0;
        for (var i = 0; i < numContour; i++)
        {
            int endContour = endPoints[i];
            for (; n <= endContour; ++n)
            {
                var f = flagStream[flagStreamIndex++];

                // int f1 = (f >> 7); // Most significant 1 bit -> on/off curve.
                var xyFormat = f & 0x7F; // Remaining 7 bits x, y format.

                var enc = TripleEncodingTable.EncTable[xyFormat]; // 0-128

                var packedXY =
                    reader.ReadBytes(enc.ByteCount -
                                     1); // byte count include 1 byte flags, so actual read=> byteCount-1

                int x;
                int y;
                switch (enc.XBits)
                {
                    default:
                        throw new NotSupportedException();
                    case 0: // 0,8,
                        x = 0;
                        y = enc.Ty(packedXY[0]);
                        break;
                    case 4: // 4,4
                        x = enc.Tx(packedXY[0] >> 4);
                        y = enc.Ty(packedXY[0] & 0xF);
                        break;
                    case 8: // 8,0 or 8,8
                        x = enc.Tx(packedXY[0]);
                        y = enc.YBits == 8 ? enc.Ty(packedXY[1]) : 0;
                        break;
                    case 12: // 12,12
                        x = enc.Tx((packedXY[0] << 4) | (packedXY[1] >> 4));
                        y = enc.Ty(((packedXY[1] & 0xF) << 8) | packedXY[2]);
                        break;
                    case 16: // 16,16
                        x = enc.Tx((packedXY[0] << 8) | packedXY[1]);
                        y = enc.Ty((packedXY[2] << 8) | packedXY[3]);
                        break;
                }

                // Most significant 1 bit -> on/off curve.
                onCurves[n] = f >> 7 == 0;
                controlPoints[n] = new Vector2(curX += x, curY += y);
            }
        }

        // Read the instructions
        var instructionSize = Read255UInt16(reader);
        var position = reader.BaseStream.Position;
        reader.BaseStream.Position = instructionStreamOffset;
        var instructions = reader.ReadBytes(instructionSize);
        instructionStreamOffset += instructionSize;
        reader.BaseStream.Position = position;

        // Passing default here will cause the bounds to be calculated from the vector controls points.
        // They can be overwritten later on for composite glyphs.
        return new GlyphVector(controlPoints, onCurves, endPoints, default, instructions);
    }

    private static bool CompositeHasInstructions(BigEndianBinaryReader reader)
    {
        var weHaveInstructions = false;
        var flags = CompositeGlyphFlags.MoreComponents;
        while ((flags & CompositeGlyphFlags.MoreComponents) != 0)
        {
            flags = reader.ReadUInt16<CompositeGlyphFlags>();
            weHaveInstructions |= (flags & CompositeGlyphFlags.WeHaveInstructions) != 0;
            var argSize = 2; // glyph index
            if ((flags & CompositeGlyphFlags.Args1And2AreWords) != 0)
                argSize += 4;
            else
                argSize += 2;

            if ((flags & CompositeGlyphFlags.WeHaveAScale) != 0)
                argSize += 2;
            else if ((flags & CompositeGlyphFlags.WeHaveXAndYScale) != 0)
                argSize += 4;
            else if ((flags & CompositeGlyphFlags.WeHaveATwoByTwo) != 0) argSize += 8;

            reader.BaseStream.Seek(argSize, SeekOrigin.Current);
        }

        return weHaveInstructions;
    }

    private static GlyphVector ReadCompositeGlyphData(GlyphVector[] createdGlyphs, BigEndianBinaryReader reader)
    {
        GlyphVector composite = default;
        CompositeGlyphFlags flags;
        do
        {
            flags = reader.ReadUInt16<CompositeGlyphFlags>();
            var glyphIndex = reader.ReadUInt16();
            if (!createdGlyphs[glyphIndex].HasValue())
            {
                // This glyph has not been read yet, resolve it first.
                var position = reader.BaseStream.Position;
                createdGlyphs[glyphIndex] = ReadCompositeGlyphData(createdGlyphs, reader);
                reader.BaseStream.Position = position;
            }

            CompositeGlyphLoader.LoadArguments(reader, flags, out var dx, out var dy);

            var transform = Matrix3x2.Identity;
            transform.Translation = new Vector2(dx, dy);

            if ((flags & CompositeGlyphFlags.WeHaveAScale) != 0)
            {
                var scale = reader.ReadF2dot14();
                transform.M11 = scale;
                transform.M21 = scale;
            }
            else if ((flags & CompositeGlyphFlags.WeHaveXAndYScale) != 0)
            {
                transform.M11 = reader.ReadF2dot14();
                transform.M22 = reader.ReadF2dot14();
            }
            else if ((flags & CompositeGlyphFlags.WeHaveATwoByTwo) != 0)
            {
                transform.M11 = reader.ReadF2dot14();
                transform.M12 = reader.ReadF2dot14();
                transform.M21 = reader.ReadF2dot14();
                transform.M22 = reader.ReadF2dot14();
            }

            // Composite bounds are read later.
            composite = GlyphVector.Append(composite,
                GlyphVector.Transform(GlyphVector.DeepClone(createdGlyphs[glyphIndex]), transform), default);
        } while ((flags & CompositeGlyphFlags.MoreComponents) != 0);

        return composite;
    }

    private static byte[] ExpandBitmap(byte[] orgBBoxBitmap)
    {
        var expandArr = new byte[orgBBoxBitmap.Length * 8];

        var index = 0;
        for (var i = 0; i < orgBBoxBitmap.Length; i++)
        {
            var b = orgBBoxBitmap[i];
            expandArr[index++] = (byte)((b >> 7) & 0x1);
            expandArr[index++] = (byte)((b >> 6) & 0x1);
            expandArr[index++] = (byte)((b >> 5) & 0x1);
            expandArr[index++] = (byte)((b >> 4) & 0x1);
            expandArr[index++] = (byte)((b >> 3) & 0x1);
            expandArr[index++] = (byte)((b >> 2) & 0x1);
            expandArr[index++] = (byte)((b >> 1) & 0x1);
            expandArr[index++] = (byte)((b >> 0) & 0x1);
        }

        return expandArr;
    }

    /// <summary>
    ///     Reads the UIntBase128 Data Type.
    /// </summary>
    /// <param name="reader">The binary reader using big endian encoding.</param>
    /// <param name="result">The result as uint.</param>
    /// <returns>true, if succeeded.</returns>
    private static bool ReadUIntBase128(BigEndianBinaryReader reader, out uint result)
    {
        // UIntBase128 is a different variable length encoding of unsigned integers,
        // suitable for values up to 2^(32) - 1.
        // A UIntBase128 encoded number is a sequence of bytes for which the most significant bit
        // is set for all but the last byte,
        // and clear for the last byte.
        //
        // The number itself is base 128 encoded in the lower 7 bits of each byte.
        // Thus, a decoding procedure for a UIntBase128 is:
        // start with value = 0.
        // Consume a byte, setting value = old value times 128 + (byte bitwise - and 127).
        // Repeat last step until the most significant bit of byte is false.
        //
        // UIntBase128 encoding format allows a possibility of sub-optimal encoding,
        // where e.g.the same numerical value can be represented with variable number of bytes(utilizing leading 'zeros').
        // For example, the value 63 could be encoded as either one byte 0x3F or two(or more) bytes: [0x80, 0x3f].
        // An encoder must not allow this to happen and must produce shortest possible encoding.
        // A decoder MUST reject the font file if it encounters a UintBase128 - encoded value with leading zeros(a value that starts with the byte 0x80),
        // if UintBase128 - encoded sequence is longer than 5 bytes,
        // or if a UintBase128 - encoded value exceeds 232 - 1.
        uint accum = 0;
        result = 0;
        for (var i = 0; i < 5; i++)
        {
            var data_byte = reader.ReadByte();

            // No leading 0's
            if (i == 0 && data_byte == 0x80) return false;

            // If any of top 7 bits are set then << 7 would overflow.
            if ((accum & 0xFE000000) != 0) return false;

            accum = (accum << 7) | (uint)(data_byte & 0x7F);

            // Spin until most significant bit of data byte is false.
            if ((data_byte & 0x80) == 0)
            {
                result = accum;
                return true;
            }
        }

        // UIntBase128 sequence exceeds 5 bytes.
        return false;
    }

    /// <summary>
    ///     Reads the UIntBase255 Data Type.
    /// </summary>
    /// <param name="reader">The binary reader using big endian encoding.</param>
    /// <returns>The UIntBase255 result.</returns>
    private static ushort Read255UInt16(BigEndianBinaryReader reader)
    {
        // 255UInt16 Variable-length encoding of a 16-bit unsigned integer for optimized intermediate font data storage.
        // 255UInt16 is a variable-length encoding of an unsigned integer
        // in the range 0 to 65535 inclusive.
        // This data type is intended to be used as intermediate representation of various font values,
        // which are typically expressed as UInt16 but represent relatively small values.
        // Depending on the encoded value, the length of the data field may be one to three bytes,
        // where the value of the first byte either represents the small value itself or is treated as a code that defines the format of the additional byte(s).
        var code = reader.ReadByte();
        if (code == WordCode)
        {
            int value = reader.ReadByte();
            value <<= 8;
            value &= 0xff00;
            int value2 = reader.ReadByte();
            value |= value2 & 0x00ff;

            return (ushort)value;
        }

        if (code == OneMoreByteCode1)
            return (ushort)(reader.ReadByte() + LowestUCode);
        if (code == OneMoreByteCode2)
            return (ushort)(reader.ReadByte() + LowestUCode * 2);
        return code;
    }

    private struct GlyphData
    {
        public readonly ushort GlyphIndex;
        public readonly short NumContour;
        public bool CompositeHasInstructions;

        public GlyphData(ushort glyphIndex, short contourCount)
        {
            GlyphIndex = glyphIndex;
            NumContour = contourCount;
            CompositeHasInstructions = false;
        }
    }
}