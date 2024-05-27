// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     The font styles
/// </summary>
[Flags]
public enum FontStyle
{
    /// <summary>
    ///     Regular
    /// </summary>
    Regular = 0,

    /// <summary>
    ///     Bold
    /// </summary>
    Bold = 1,

    /// <summary>
    ///     Italic
    /// </summary>
    Italic = 2,

    /// <summary>
    ///     Bold and Italic
    /// </summary>
    BoldItalic = 3,

    Underline = 4, // TODO: Not yet supported, but enabled for anything that specifies it

    Strikeout = 8 // TODO: Not yet supported, but enabled for anything that specifies it
}