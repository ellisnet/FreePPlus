// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Diagnostics.CodeAnalysis;

namespace FreePPlus.Imaging.Fonts.Native;

//was previously: namespace SixLabors.Fonts.Native;

/// <summary>
///     Options you can use to determine how CFURL functions parse a file system path name.
/// </summary>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter",
    Justification = "Verbatim constants from the macOS SDK")]
internal enum CFURLPathStyle : long
{
    /// <summary>
    ///     Indicates a POSIX style path name. Components are slash delimited. A leading slash indicates an absolute path; a
    ///     trailing slash is not significant.
    /// </summary>
    kCFURLPOSIXPathStyle = 0
}