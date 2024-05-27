// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Globalization;
using System.IO;
using FreePPlus.Imaging.Fonts.Tables;
using FreePPlus.Imaging.Fonts.Tables.General;
using FreePPlus.Imaging.Fonts.Tables.General.Name;
using FreePPlus.Imaging.Fonts.WellKnownIds;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Provides basic descriptive metadata for the font.
/// </summary>
public class FontDescription
{
    private readonly NameTable nameTable;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FontDescription" /> class.
    /// </summary>
    /// <param name="nameTable">The name table.</param>
    /// <param name="os2">The os2 table.</param>
    /// <param name="head">The head table.</param>
    internal FontDescription(NameTable nameTable, OS2Table? os2, HeadTable? head)
    {
        this.nameTable = nameTable;
        Style = ConvertStyle(os2, head);

        FontNameInvariantCulture = FontName(CultureInfo.InvariantCulture);
        FontFamilyInvariantCulture = FontFamily(CultureInfo.InvariantCulture);
        FontSubFamilyNameInvariantCulture = FontSubFamilyName(CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Gets the style.
    /// </summary>
    public FontStyle Style { get; }

    /// <summary>
    ///     Gets the name of the font in the invariant culture.
    /// </summary>
    public string FontNameInvariantCulture { get; }

    /// <summary>
    ///     Gets the name of the font family in the invariant culture.
    /// </summary>
    public string FontFamilyInvariantCulture { get; }

    /// <summary>
    ///     Gets the font sub family in the invariant culture.
    /// </summary>
    public string FontSubFamilyNameInvariantCulture { get; }

    /// <summary>
    ///     Gets the name of the font.
    /// </summary>
    /// <param name="culture">The culture to load metadata in.</param>
    /// <returns>The font name.</returns>
    public string FontName(CultureInfo culture)
    {
        return nameTable.FontName(culture);
    }

    /// <summary>
    ///     Gets the name of the font family.
    /// </summary>
    /// <param name="culture">The culture to load metadata in.</param>
    /// <returns>The font family name.</returns>
    public string FontFamily(CultureInfo culture)
    {
        return nameTable.FontFamilyName(culture);
    }

    /// <summary>
    ///     Gets the font sub family.
    /// </summary>
    /// <param name="culture">The culture to load metadata in.</param>
    /// <returns>The font sub family name.</returns>
    public string FontSubFamilyName(CultureInfo culture)
    {
        return nameTable.FontSubFamilyName(culture);
    }

    /// <summary>
    ///     Gets the name matching the given culture and id.
    ///     If <see cref="CultureInfo.InvariantCulture" /> is passed this method will return the first name matching the id.
    /// </summary>
    /// <param name="culture">The culture to load metadata in.</param>
    /// <param name="nameId">The name id to match.</param>
    /// <returns>The <see cref="string" /> name.</returns>
    public string GetNameById(CultureInfo culture, KnownNameIds nameId)
    {
        return nameTable.GetNameById(culture, nameId);
    }

    /// <summary>
    ///     Reads a <see cref="FontDescription" /> from the specified stream.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>a <see cref="FontDescription" />.</returns>
    public static FontDescription LoadDescription(string path)
    {
        Guard.NotNullOrWhiteSpace(path, nameof(path));

        using var fs = File.OpenRead(path);
        var reader = new FontReader(fs);
        return LoadDescription(reader);
    }

    /// <summary>
    ///     Reads a <see cref="FontDescription" /> from the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>a <see cref="FontDescription" />.</returns>
    public static FontDescription LoadDescription(Stream stream)
    {
        Guard.NotNull(stream, nameof(stream));

        // Only read the name tables.
        var reader = new FontReader(stream);

        return LoadDescription(reader);
    }

    /// <summary>
    ///     Reads a <see cref="FontDescription" /> from the specified stream.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>
    ///     a <see cref="FontDescription" />.
    /// </returns>
    internal static FontDescription LoadDescription(FontReader reader)
    {
        DebugGuard.NotNull(reader, nameof(reader));

        // NOTE: These fields are read in their optimized order
        // https://docs.microsoft.com/en-gb/typography/opentype/spec/recom#optimized-table-ordering
        var head = reader.TryGetTable<HeadTable>();
        var os2 = reader.TryGetTable<OS2Table>();
        var nameTable = reader.GetTable<NameTable>();

        return new FontDescription(nameTable, os2, head);
    }

    /// <summary>
    ///     Reads all the <see cref="FontDescription" />s from the file at the specified path (typically a .ttc file like
    ///     simsun.ttc).
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <returns>a <see cref="FontDescription" />.</returns>
    public static FontDescription[] LoadFontCollectionDescriptions(string path)
    {
        Guard.NotNullOrWhiteSpace(path, nameof(path));

        using var fs = File.OpenRead(path);
        return LoadFontCollectionDescriptions(fs);
    }

    /// <summary>
    ///     Reads all the <see cref="FontDescription" />s from the specified stream (typically a .ttc file like simsun.ttc).
    /// </summary>
    /// <param name="stream">The stream to read the font collection from.</param>
    /// <returns>a <see cref="FontDescription" />.</returns>
    public static FontDescription[] LoadFontCollectionDescriptions(Stream stream)
    {
        var startPos = stream.Position;
        var reader = new BigEndianBinaryReader(stream, true);
        var ttcHeader = TtcHeader.Read(reader);

        var result = new FontDescription[(int)ttcHeader.NumFonts];
        for (var i = 0; i < ttcHeader.NumFonts; ++i)
        {
            stream.Position = startPos + ttcHeader.OffsetTable[i];
            var fontReader = new FontReader(stream);
            result[i] = LoadDescription(fontReader);
        }

        return result;
    }

    private static FontStyle ConvertStyle(OS2Table? os2, HeadTable? head)
    {
        var style = FontStyle.Regular;

        if (os2 != null)
        {
            if (os2.FontStyle.HasFlag(OS2Table.FontStyleSelection.BOLD)) style |= FontStyle.Bold;

            if (os2.FontStyle.HasFlag(OS2Table.FontStyleSelection.ITALIC)) style |= FontStyle.Italic;
        }
        else if (head != null)
        {
            if (head.MacStyle.HasFlag(HeadTable.HeadMacStyle.Bold)) style |= FontStyle.Bold;

            if (head.MacStyle.HasFlag(HeadTable.HeadMacStyle.Italic)) style |= FontStyle.Italic;
        }

        return style;
    }
}