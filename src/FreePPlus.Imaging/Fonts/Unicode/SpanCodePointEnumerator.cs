// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Fonts.Unicode;

//was previously: namespace SixLabors.Fonts.Unicode;

/// <summary>
///     An enumerator for retrieving <see cref="CodePoint" /> instances from a <see cref="ReadOnlySpan{Char}" />.
///     Methods are pattern-matched by compiler to allow using foreach pattern.
/// </summary>
public ref struct SpanCodePointEnumerator
{
    private ReadOnlySpan<char> source;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpanCodePointEnumerator" /> struct.
    /// </summary>
    /// <param name="source">The buffer to read from.</param>
    public SpanCodePointEnumerator(ReadOnlySpan<char> source)
    {
        this.source = source;
        Current = CodePoint.ReplacementChar;
    }

    /// <summary>
    ///     Gets the element in the collection at the current position of the enumerator.
    /// </summary>
    public CodePoint Current { get; private set; }

    /// <summary>
    ///     Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that iterates through the collection.</returns>
    public SpanCodePointEnumerator GetEnumerator()
    {
        return this;
    }

    /// <summary>
    ///     Advances the enumerator to the next element of the collection.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the enumerator was successfully advanced to the next element;
    ///     <see langword="false" /> if the enumerator has passed the end of the collection.
    /// </returns>
    public bool MoveNext()
    {
        Current = CodePoint.DecodeFromUtf16At(source, 0, out var consumed);
        source = source.Slice(consumed);
        return consumed > 0;
    }
}