﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Represents the ICC profile version number.
/// </summary>
public readonly struct IccVersion : IEquatable<IccVersion>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccVersion" /> struct.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <param name="patch">The patch version number.</param>
    public IccVersion(int major, int minor, int patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    /// <summary>
    ///     Gets the major version number.
    /// </summary>
    public int Major { get; }

    /// <summary>
    ///     Gets the minor version number.
    /// </summary>
    public int Minor { get; }

    /// <summary>
    ///     Gets the patch number.
    /// </summary>
    public int Patch { get; }

    /// <inheritdoc />
    public bool Equals(IccVersion other)
    {
        return Major == other.Major &&
               Minor == other.Minor &&
               Patch == other.Patch;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return string.Join(".", Major, Minor, Patch);
    }
}