// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Memory;

//was previously: namespace SixLabors.ImageSharp.Memory;

/// <summary>
///     Exception thrown when the library detects an invalid memory allocation request,
///     or an attempt has been made to use an invalidated <see cref="IMemoryGroup{T}" />.
/// </summary>
public class InvalidMemoryOperationException : InvalidOperationException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="InvalidMemoryOperationException" /> class.
    /// </summary>
    /// <param name="message">The exception message text.</param>
    public InvalidMemoryOperationException(string message)
        : base(message) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="InvalidMemoryOperationException" /> class.
    /// </summary>
    public InvalidMemoryOperationException() { }
}