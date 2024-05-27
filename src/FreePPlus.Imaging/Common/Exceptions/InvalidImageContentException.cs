// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     The exception that is thrown when the library tries to load
///     an image which contains invalid content.
/// </summary>
public sealed class InvalidImageContentException : ImageFormatException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="InvalidImageContentException" /> class with the name of the
    ///     parameter that causes this exception.
    /// </summary>
    /// <param name="errorMessage">The error message that explains the reason for this exception.</param>
    public InvalidImageContentException(string errorMessage)
        : base(errorMessage) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="InvalidImageContentException" /> class with the name of the
    ///     parameter that causes this exception.
    /// </summary>
    /// <param name="errorMessage">The error message that explains the reason for this exception.</param>
    /// <param name="innerException">
    ///     The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic)
    ///     if no inner exception is specified.
    /// </param>
    public InvalidImageContentException(string errorMessage, Exception innerException)
        : base(errorMessage, innerException) { }
}