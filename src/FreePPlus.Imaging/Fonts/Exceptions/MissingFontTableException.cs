// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Exception font loading can throw if it finds a required table is missing during font loading.
/// </summary>
/// <seealso cref="Exception" />
public class MissingFontTableException : InvalidFontFileException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MissingFontTableException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="table">The table.</param>
    public MissingFontTableException(string message, string table)
        : base(message)
    {
        Table = table;
    }

    /// <summary>
    ///     Gets the table where the error originated.
    /// </summary>
    public string Table { get; }
}