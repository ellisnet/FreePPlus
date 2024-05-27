// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;

namespace FreePPlus.Imaging.Fonts.Unicode;

//was previously: namespace SixLabors.Fonts.Unicode;

/// <summary>
///     Represents a unicode string and all associated attributes
///     for each character required for the Bidi algorithm
/// </summary>
internal class BidiData
{
    private readonly List<int> paragraphPositions = new();
    private ArrayBuilder<BidiPairedBracketType> pairedBracketTypes;
    private ArrayBuilder<int> pairedBracketValues;
    private ArrayBuilder<BidiPairedBracketType> savedPairedBracketTypes;
    private ArrayBuilder<BidiCharacterType> savedTypes;
    private ArrayBuilder<sbyte> tempLevelBuffer;
    private ArrayBuilder<BidiCharacterType> types;

    public sbyte ParagraphEmbeddingLevel { get; private set; }

    public bool HasBrackets { get; private set; }

    public bool HasEmbeddings { get; private set; }

    public bool HasIsolates { get; private set; }

    /// <summary>
    ///     Gets the length of the data held by the BidiData
    /// </summary>
    public int Length => types.Length;

    /// <summary>
    ///     Gets the bidi character type of each code point
    /// </summary>
    public ArraySlice<BidiCharacterType> Types { get; private set; }

    /// <summary>
    ///     Gets the paired bracket type for each code point
    /// </summary>
    public ArraySlice<BidiPairedBracketType> PairedBracketTypes { get; private set; }

    /// <summary>
    ///     Gets the paired bracket value for code point
    /// </summary>
    /// <remarks>
    ///     The paired bracket values are the code points
    ///     of each character where the opening code point
    ///     is replaced with the closing code point for easier
    ///     matching.  Also, bracket code points are mapped
    ///     to their canonical equivalents
    /// </remarks>
    public ArraySlice<int> PairedBracketValues { get; private set; }

    /// <summary>
    ///     Initialize with a text value.
    /// </summary>
    /// <param name="text">The text to process.</param>
    /// <param name="paragraphEmbeddingLevel">The paragraph embedding level</param>
    public void Init(ReadOnlySpan<char> text, sbyte paragraphEmbeddingLevel)
    {
        // Set working buffer sizes
        // TODO: This allocates more than it should for some arrays.
        var length = CodePoint.GetCodePointCount(text);
        types.Length = length;
        pairedBracketTypes.Length = length;
        pairedBracketValues.Length = length;

        paragraphPositions.Clear();
        ParagraphEmbeddingLevel = paragraphEmbeddingLevel;

        // Resolve the BidiCharacterType, paired bracket type and paired
        // bracket values for all code points
        HasBrackets = false;
        HasEmbeddings = false;
        HasIsolates = false;

        var i = 0;
        var codePointEnumerator = new SpanCodePointEnumerator(text);
        while (codePointEnumerator.MoveNext())
        {
            var codePoint = codePointEnumerator.Current;
            var bidi = CodePoint.GetBidiClass(codePoint);

            // Look up BidiCharacterType
            var dir = bidi.CharacterType;
            types[i] = dir;

            switch (dir)
            {
                case BidiCharacterType.LeftToRightEmbedding:
                case BidiCharacterType.LeftToRightOverride:
                case BidiCharacterType.RightToLeftEmbedding:
                case BidiCharacterType.RightToLeftOverride:
                case BidiCharacterType.PopDirectionalFormat:
                    HasEmbeddings = true;
                    break;

                case BidiCharacterType.LeftToRightIsolate:
                case BidiCharacterType.RightToLeftIsolate:
                case BidiCharacterType.FirstStrongIsolate:
                case BidiCharacterType.PopDirectionalIsolate:
                    HasIsolates = true;
                    break;
            }

            // Lookup paired bracket types
            var pbt = bidi.PairedBracketType;
            pairedBracketTypes[i] = pbt;

            if (pbt == BidiPairedBracketType.Open)
            {
                // Opening bracket types can never have a null pairing.
                bidi.TryGetPairedBracket(out var paired);
                pairedBracketValues[i] = CodePoint.GetCanonicalType(paired).Value;

                HasBrackets = true;
            }
            else if (pbt == BidiPairedBracketType.Close)
            {
                pairedBracketValues[i] = CodePoint.GetCanonicalType(codePoint).Value;
                HasBrackets = true;
            }

            i++;
        }

        // Create slices on work buffers
        Types = types.AsSlice();
        PairedBracketTypes = pairedBracketTypes.AsSlice();
        PairedBracketValues = pairedBracketValues.AsSlice();
    }

    /// <summary>
    ///     Save the Types and PairedBracketTypes of this bididata
    /// </summary>
    /// <remarks>
    ///     This is used when processing embedded style runs with
    ///     BidiCharacterType overrides.  TextLayout saves the data,
    ///     overrides the style runs to neutral, processes the bidi
    ///     data for the entire paragraph and then restores this data
    ///     before processing the embedded runs.
    /// </remarks>
    public void SaveTypes()
    {
        // Capture the types data
        savedTypes.Clear();
        savedTypes.Add(types.AsSlice());
        savedPairedBracketTypes.Clear();
        savedPairedBracketTypes.Add(pairedBracketTypes.AsSlice());
    }

    /// <summary>
    ///     Restore the data saved by SaveTypes
    /// </summary>
    public void RestoreTypes()
    {
        types.Clear();
        types.Add(savedTypes.AsSlice());
        pairedBracketTypes.Clear();
        pairedBracketTypes.Add(savedPairedBracketTypes.AsSlice());
    }

    /// <summary>
    ///     Gets a temporary level buffer. Used by TextLayout when
    ///     resolving style runs with different BidiCharacterType.
    /// </summary>
    /// <param name="length">Length of the required ExpandableBuffer</param>
    /// <returns>An uninitialized level ExpandableBuffer</returns>
    public ArraySlice<sbyte> GetTempLevelBuffer(int length)
    {
        tempLevelBuffer.Clear();
        return tempLevelBuffer.Add(length, false);
    }
}