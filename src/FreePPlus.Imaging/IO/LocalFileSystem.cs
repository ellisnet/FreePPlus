﻿// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.IO;

namespace FreePPlus.Imaging.IO;

//was previously: namespace SixLabors.ImageSharp.IO;

/// <summary>
///     A wrapper around the local File apis.
/// </summary>
internal sealed class LocalFileSystem : IFileSystem
{
    /// <inheritdoc />
    public Stream OpenRead(string path)
    {
        return File.OpenRead(path);
    }

    /// <inheritdoc />
    public Stream Create(string path)
    {
        return File.Create(path);
    }
}