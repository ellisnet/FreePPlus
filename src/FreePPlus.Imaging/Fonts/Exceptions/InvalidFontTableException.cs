// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Exception font loading can throw if it encounters invalid data during font loading.
/// </summary>
/// <seealso cref="Exception" />
public class InvalidFontTableException : InvalidFontFileException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="InvalidFontTableException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="table">The table.</param>
    public InvalidFontTableException(string message, string table)
        : base(message)
    {
        Table = table;
    }

    /// <summary>
    ///     Gets the table where the error originated.
    /// </summary>
    public string Table { get; }
}