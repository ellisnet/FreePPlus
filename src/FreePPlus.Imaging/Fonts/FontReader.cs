// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using FreePPlus.Imaging.Fonts.Tables;
using FreePPlus.Imaging.Fonts.Tables.Woff;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

internal sealed class FontReader
{
    private readonly Dictionary<Type, Table> loadedTables = new();
    private readonly TableLoader loader;
    private readonly Stream stream;

    internal FontReader(Stream stream, TableLoader loader)
    {
        this.loader = loader;

        var loadHeader = TableHeader.Read;
        var startOfFilePosition = stream.Position;

        this.stream = stream;
        var reader = new BigEndianBinaryReader(stream, true);

        // we should immediately read the table header to learn which tables we have and what order they are in
        var version = reader.ReadUInt32();
        ushort tableCount;
        if (version == 0x774F4646)
        {
            // This is a woff file.
            TableFormat = TableFormat.Woff;

            // WOFFHeader
            // UInt32 | signature      | 0x774F4646 'wOFF'
            // UInt32 | flavor         | The "sfnt version" of the input font.
            // UInt32 | length         | Total size of the WOFF file.
            // UInt16 | numTables      | Number of entries in directory of font tables.
            // UInt16 | reserved       | Reserved; set to zero.
            // UInt32 | totalSfntSize  | Total size needed for the uncompressed font data, including the sfnt header, directory, and font tables(including padding).
            // UInt16 | majorVersion   | Major version of the WOFF file.
            // UInt16 | minorVersion   | Minor version of the WOFF file.
            // UInt32 | metaOffset     | Offset to metadata block, from beginning of WOFF file.
            // UInt32 | metaLength     | Length of compressed metadata block.
            // UInt32 | metaOrigLength | Uncompressed size of metadata block.
            // UInt32 | privOffset     | Offset to private data block, from beginning of WOFF file.
            // UInt32 | privLength     | Length of private data block.
            var flavor = reader.ReadUInt32();
            OutlineType = (OutlineType)flavor;
            var length = reader.ReadUInt32();
            tableCount = reader.ReadUInt16();
            var reserved = reader.ReadUInt16();
            var totalSfntSize = reader.ReadUInt32();
            var majorVersion = reader.ReadUInt16();
            var minorVersion = reader.ReadUInt16();
            var metaOffset = reader.ReadUInt32();
            var metaLength = reader.ReadUInt32();
            var metaOrigLength = reader.ReadUInt32();
            var privOffset = reader.ReadUInt32();
            var privLength = reader.ReadUInt32();
            CompressedTableData = true;
            loadHeader = WoffTableHeader.Read;
        }
        else if (version == 0x774F4632)
        {
            // This is a woff2 file.
            TableFormat = TableFormat.Woff2;

#if NETSTANDARD2_0
                throw new NotSupportedException("Brotli compression is not available and is required for decoding woff2");
#else

            var flavor = reader.ReadUInt32();
            OutlineType = (OutlineType)flavor;
            var length = reader.ReadUInt32();
            tableCount = reader.ReadUInt16();
            var reserved = reader.ReadUInt16();
            var totalSfntSize = reader.ReadUInt32();
            var totalCompressedSize = reader.ReadUInt32();
            var majorVersion = reader.ReadUInt16();
            var minorVersion = reader.ReadUInt16();
            var metaOffset = reader.ReadUInt32();
            var metaLength = reader.ReadUInt32();
            var metaOrigLength = reader.ReadUInt32();
            var privOffset = reader.ReadUInt32();
            var privLength = reader.ReadUInt32();
            CompressedTableData = true;
            Headers = Woff2Utils.ReadWoff2Headers(reader, tableCount);

            var compressedBuffer = reader.ReadBytes((int)totalCompressedSize);
            var decompressedStream = new MemoryStream();
            using var input = new MemoryStream(compressedBuffer);
            using var decompressor = new BrotliStream(input, CompressionMode.Decompress);
            decompressor.CopyTo(decompressedStream);
            decompressedStream.Position = 0;
            this.stream.Dispose();
            this.stream = decompressedStream;
            return;
#endif
        }
        else
        {
            // This is a standard *.otf file (this is named the Offset Table).
            TableFormat = TableFormat.Otf;

            OutlineType = (OutlineType)version;
            tableCount = reader.ReadUInt16();
            var searchRange = reader.ReadUInt16();
            var entrySelector = reader.ReadUInt16();
            var rangeShift = reader.ReadUInt16();
            CompressedTableData = false;
        }

        var headers = new Dictionary<string, TableHeader>(tableCount);
        for (var i = 0; i < tableCount; i++)
        {
            var tbl = loadHeader(reader);
            headers[tbl.Tag] = tbl;
        }

        Headers = new ReadOnlyDictionary<string, TableHeader>(headers);
    }

    public FontReader(Stream stream)
        : this(stream, TableLoader.Default) { }

    public TableFormat TableFormat { get; }

    public IReadOnlyDictionary<string, TableHeader> Headers { get; }

    public bool CompressedTableData { get; }

    public OutlineType OutlineType { get; }

    public TTableType? TryGetTable<TTableType>()
        where TTableType : Table
    {
        if (loadedTables.TryGetValue(typeof(TTableType), out var table))
        {
            return (TTableType)table;
        }

        var loadedTable = loader.Load<TTableType>(this);
        if (loadedTable is null) return null;

        table = loadedTable;
        loadedTables.Add(typeof(TTableType), loadedTable);

        return (TTableType)table;
    }

    public TTableType GetTable<TTableType>()
        where TTableType : Table
    {
        var tbl = TryGetTable<TTableType>();

        if (tbl is null)
        {
            var tag = loader.GetTag<TTableType>();
            throw new MissingFontTableException($"Table '{tag}' is missing", tag!);
        }

        return tbl;
    }

    public TableHeader? GetHeader(string tag)
    {
        return Headers.TryGetValue(tag, out var header)
            ? header
            : null;
    }

    public BigEndianBinaryReader GetReaderAtTablePosition(string tableName)
    {
        if (!TryGetReaderAtTablePosition(tableName, out var reader))
            throw new InvalidFontTableException("Unable to find table", tableName);

        return reader!;
    }

    public bool TryGetReaderAtTablePosition(string tableName, [NotNullWhen(true)] out BigEndianBinaryReader? reader)
    {
        return TryGetReaderAtTablePosition(tableName, out reader, out _);
    }

    public bool TryGetReaderAtTablePosition(string tableName, [NotNullWhen(true)] out BigEndianBinaryReader? reader,
        [NotNullWhen(true)] out TableHeader? header)
    {
        header = GetHeader(tableName);
        if (header == null)
        {
            reader = null;
            return false;
        }

        reader = header?.CreateReader(stream);
        return reader != null;
    }
}