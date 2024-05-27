// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Exception for detailing missing font families.
/// </summary>
/// <seealso cref="FontException" />
public class FontFamilyNotFoundException : FontException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FontFamilyNotFoundException" /> class.
    /// </summary>
    /// <param name="family">The name of the missing font family.</param>
    public FontFamilyNotFoundException(string family)
        : this(family, Array.Empty<string>()) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FontFamilyNotFoundException" /> class.
    /// </summary>
    /// <param name="family">The name of the missing font family.</param>
    /// <param name="searchDirectories">
    ///     The collection of directories that were searched for the font family.
    ///     Pass an empty collection if font families were not searched in directories.
    /// </param>
    public FontFamilyNotFoundException(string family, IReadOnlyCollection<string> searchDirectories)
        : base(GetMessage(family, searchDirectories))
    {
        FontFamily = family;
        SearchDirectories = searchDirectories;
    }

    /// <summary>
    ///     Gets the name of the font family that was not found.
    /// </summary>
    public string FontFamily { get; }

    /// <summary>
    ///     Gets the collection of directories that were unsuccessfully searched for the font family.
    /// </summary>
    /// <remarks>
    ///     If the exception did not originate from the <see cref="SystemFonts.Collection" /> then this property will be empty.
    /// </remarks>
    public IReadOnlyCollection<string> SearchDirectories { get; }

    private static string GetMessage(string family, IReadOnlyCollection<string> searchDirectories)
    {
        if (searchDirectories.Count == 0) return $"The \"{family}\" font family could not be found";

        if (searchDirectories.Count == 1)
            return
                $"The \"{family}\" font family could not be found in the following directory: {searchDirectories.First()}";

        return
            $"The \"{family}\" font family could not be found in the following directories:{Environment.NewLine}{string.Join(Environment.NewLine, searchDirectories.Select(e => $" * {e}"))}";
    }
}