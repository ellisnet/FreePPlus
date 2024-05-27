// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FreePPlus.Imaging.Fonts.Utilities;
using FreePPlus.Imaging.Fonts.WellKnownIds;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Tables.General.Name;

//was previously: namespace SixLabors.Fonts.Tables.General.Name;

internal class NameTable : Table
{
    internal const string TableName = "name";
    private readonly NameRecord[] names;

#pragma warning disable IDE0060

    internal NameTable(NameRecord[] names, string[] languages)
    {
        this.names = names;
    }

#pragma warning restore IDE0060

    /// <summary>
    ///     Gets the name of the font.
    /// </summary>
    /// <value>
    ///     The name of the font.
    /// </value>
    public string Id(CultureInfo culture)
    {
        return GetNameById(culture, KnownNameIds.UniqueFontID);
    }

    /// <summary>
    ///     Gets the name of the font.
    /// </summary>
    /// <value>
    ///     The name of the font.
    /// </value>
    public string FontName(CultureInfo culture)
    {
        return GetNameById(culture, KnownNameIds.FullFontName);
    }

    /// <summary>
    ///     Gets the name of the font.
    /// </summary>
    /// <value>
    ///     The name of the font.
    /// </value>
    public string FontFamilyName(CultureInfo culture)
    {
        return GetNameById(culture, KnownNameIds.FontFamilyName);
    }

    /// <summary>
    ///     Gets the name of the font.
    /// </summary>
    /// <value>
    ///     The name of the font.
    /// </value>
    public string FontSubFamilyName(CultureInfo culture)
    {
        return GetNameById(culture, KnownNameIds.FontSubfamilyName);
    }

    public string GetNameById(CultureInfo culture, KnownNameIds nameId)
    {
        var languageId = culture.LCID;
        NameRecord? usaVersion = null;
        NameRecord? firstWindows = null;
        NameRecord? first = null;
        foreach (var name in names)
            if (name.NameID == nameId)
            {
                // Get just the first one, just in case.
                first ??= name;
                if (name.Platform == PlatformIDs.Windows)
                {
                    // If us not found return the first windows one.
                    firstWindows ??= name;
                    if (name.LanguageID == 0x0409)
                        // Grab the us version as its on next best match.
                        usaVersion ??= name;

                    if (name.LanguageID == languageId)
                        // Return the most exact first.
                        return name.Value;
                }
            }

        return usaVersion?.Value ??
               firstWindows?.Value ??
               first?.Value ??
               string.Empty;
    }

    public string GetNameById(CultureInfo culture, ushort nameId)
    {
        return GetNameById(culture, (KnownNameIds)nameId);
    }

    public static NameTable? Load(FontReader fontReader)
    {
        if (!fontReader.TryGetReaderAtTablePosition(TableName, out var binaryReader))
            throw new InvalidFontTableException($"Table '{TableName}' is missing", TableName);

        using (binaryReader)
        {
            // Move to start of table.
            return Load(binaryReader);
        }
    }

    public static NameTable Load(BigEndianBinaryReader reader)
    {
        var strings = new List<StringLoader>();
        var format = reader.ReadUInt16();
        var nameCount = reader.ReadUInt16();
        var stringOffset = reader.ReadUInt16();

        var names = new NameRecord[nameCount];

        for (var i = 0; i < nameCount; i++)
        {
            names[i] = NameRecord.Read(reader);
            var sr = names[i].StringReader;
            if (sr is not null) strings.Add(sr);
        }

        var langs = Array.Empty<StringLoader>();
        if (format == 1)
        {
            // Format 1 adds language data.
            var langCount = reader.ReadUInt16();
            langs = new StringLoader[langCount];

            for (var i = 0; i < langCount; i++)
            {
                langs[i] = StringLoader.Create(reader);
                strings.Add(langs[i]);
            }
        }

        foreach (var readable in strings)
        {
            var readableStartOffset = stringOffset + readable.Offset;

            reader.Seek(readableStartOffset, SeekOrigin.Begin);

            readable.LoadValue(reader);
        }

        var langNames = langs?.Select(x => x.Value).ToArray() ?? Array.Empty<string>();

        return new NameTable(names, langNames);
    }
}