// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Unicode;

//was previously: namespace SixLabors.Fonts.Unicode;

/// <summary>
///     Represents a Unicode value ([ U+0000..U+10FFFF ], inclusive).
/// </summary>
/// <remarks>
///     This type's constructors and conversion operators validate the input, so consumers can call the APIs
///     assuming that the underlying <see cref="CodePoint" /> instance is well-formed.
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct CodePoint : IComparable, IComparable<CodePoint>, IEquatable<CodePoint>
{
    // Supplementary plane code points are encoded as 2 UTF-16 code units
    private const int MaxUtf16CharsPerCodePoint = 2;

    // Supplementary plane code points are encoded as 4 UTF-8 code units
    internal const int MaxUtf8BytesPerCodePoint = 4;
    private const byte IsWhiteSpaceFlag = 0x80;
    private const byte IsLetterOrDigitFlag = 0x40;
    private const byte UnicodeCategoryMask = 0x1F;

    private readonly uint value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CodePoint" /> struct.
    /// </summary>
    /// <param name="value">The char representing the UTF-16 code unit</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <paramref name="value" /> represents a UTF-16 surrogate code point
    ///     U+D800..U+DFFF, inclusive.
    /// </exception>
    public CodePoint(char value)
    {
        uint expanded = value;

        if (UnicodeUtility.IsSurrogateCodePoint(expanded))
            ThrowArgumentOutOfRange(expanded, nameof(value), "Must not be in [ U+D800..U+DFFF ], inclusive.");

        this.value = expanded;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CodePoint" /> struct.
    /// </summary>
    /// <param name="highSurrogate">A char representing a UTF-16 high surrogate code unit.</param>
    /// <param name="lowSurrogate">A char representing a UTF-16 low surrogate code unit.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <paramref name="highSurrogate" /> does not represent a UTF-16 high surrogate code unit
    ///     or <paramref name="lowSurrogate" /> does not represent a UTF-16 low surrogate code unit.
    /// </exception>
    public CodePoint(char highSurrogate, char lowSurrogate)
        : this((uint)char.ConvertToUtf32(highSurrogate, lowSurrogate), false) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CodePoint" /> struct.
    /// </summary>
    /// <param name="value">The value to create the codepoint.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <paramref name="value" /> does not represent a value Unicode scalar value.
    /// </exception>
    public CodePoint(int value)
        : this((uint)value) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CodePoint" /> struct.
    /// </summary>
    /// <param name="value">The value to create the codepoint.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     If <paramref name="value" /> does not represent a value Unicode scalar value.
    /// </exception>
    public CodePoint(uint value)
    {
        if (!IsValid(value))
            ThrowArgumentOutOfRange(value, nameof(value), "Must be in [ U+0000..U+10FFFF ], inclusive.");

        this.value = value;
    }

    // Non-validating ctor
#pragma warning disable IDE0060 // Remove unused parameter
    private CodePoint(uint scalarValue, bool unused)
    {
        UnicodeUtility.DebugAssertIsValidCodePoint(scalarValue);
        value = scalarValue;
    }
#pragma warning restore IDE0060 // Remove unused parameter

    // Contains information about the ASCII character range [ U+0000..U+007F ], with:
    // - 0x80 bit if set means 'is whitespace'
    // - 0x40 bit if set means 'is letter or digit'
    // - 0x20 bit is reserved for future use
    // - bottom 5 bits are the UnicodeCategory of the character
    private static ReadOnlySpan<byte> AsciiCharInfo => new byte[]
    {
        0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x8E, 0x8E, 0x8E, 0x8E, 0x8E, 0x0E,
        0x0E, // U+0000..U+000F
        0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E, 0x0E,
        0x0E, // U+0010..U+001F
        0x8B, 0x18, 0x18, 0x18, 0x1A, 0x18, 0x18, 0x18, 0x14, 0x15, 0x18, 0x19, 0x18, 0x13, 0x18,
        0x18, // U+0020..U+002F
        0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x48, 0x18, 0x18, 0x19, 0x19, 0x19,
        0x18, // U+0030..U+003F
        0x18, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
        0x40, // U+0040..U+004F
        0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x14, 0x18, 0x15, 0x1B,
        0x12, // U+0050..U+005F
        0x1B, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41,
        0x41, // U+0060..U+006F
        0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x41, 0x14, 0x19, 0x15, 0x19, 0x0E // U+0070..U+007F
    };

    /// <summary>
    ///     Gets a value indicating whether this value is ASCII ([ U+0000..U+007F ])
    ///     and therefore representable by a single UTF-8 code unit.
    /// </summary>
    public bool IsAscii => UnicodeUtility.IsAsciiCodePoint(value);

    /// <summary>
    ///     Gets a value indicating whether this value is within the BMP ([ U+0000..U+FFFF ])
    ///     and therefore representable by a single UTF-16 code unit.
    /// </summary>
    public bool IsBmp => UnicodeUtility.IsBmpCodePoint(value);

    /// <summary>
    ///     Gets the Unicode plane (0 to 16, inclusive) which contains this scalar.
    /// </summary>
    public int Plane => UnicodeUtility.GetPlane(value);

    // Displayed as "'<char>' (U+XXXX)"; e.g., "'e' (U+0065)"
    private string DebuggerDisplay =>
        FormattableString.Invariant($"U+{value:X4} '{(IsValid(value) ? ToString() : "\uFFFD")}'");

    /// <summary>
    ///     Gets the Unicode value as an integer.
    /// </summary>
    public int Value => (int)value;

    /// <summary>
    ///     Gets the length in code units (<see cref="char" />) of the
    ///     UTF-16 sequence required to represent this scalar value.
    /// </summary>
    /// <remarks>
    ///     The return value will be 1 or 2.
    /// </remarks>
    public int Utf16SequenceLength
    {
        get
        {
            var codeUnitCount = UnicodeUtility.GetUtf16SequenceLength(value);
            Debug.Assert(codeUnitCount is > 0 and <= MaxUtf16CharsPerCodePoint,
                $"Invalid Utf16SequenceLength {codeUnitCount}.");
            return codeUnitCount;
        }
    }

    /// <summary>
    ///     Gets the length in code units of the
    ///     UTF-8 sequence required to represent this scalar value.
    /// </summary>
    /// <remarks>
    ///     The return value will be 1 through 4, inclusive.
    /// </remarks>
    public int Utf8SequenceLength
    {
        get
        {
            var codeUnitCount = UnicodeUtility.GetUtf8SequenceLength(value);
            Debug.Assert(codeUnitCount is > 0 and <= MaxUtf8BytesPerCodePoint,
                $"Invalid Utf8SequenceLength {codeUnitCount}.");
            return codeUnitCount;
        }
    }

    /// <summary>
    ///     Gets a <see cref="CodePoint" /> instance that represents the Unicode replacement character U+FFFD.
    /// </summary>
    public static CodePoint ReplacementChar { get; } = new(0xFFFD);

    // Operators below are explicit because they may throw.
    public static explicit operator CodePoint(char ch)
    {
        return new CodePoint(ch);
    }

    public static explicit operator CodePoint(uint value)
    {
        return new CodePoint(value);
    }

    public static explicit operator CodePoint(int value)
    {
        return new CodePoint(value);
    }

    public static bool operator ==(CodePoint left, CodePoint right)
    {
        return left.value == right.value;
    }

    public static bool operator !=(CodePoint left, CodePoint right)
    {
        return left.value != right.value;
    }

    public static bool operator <(CodePoint left, CodePoint right)
    {
        return left.value < right.value;
    }

    public static bool operator <=(CodePoint left, CodePoint right)
    {
        return left.value <= right.value;
    }

    public static bool operator >(CodePoint left, CodePoint right)
    {
        return left.value > right.value;
    }

    public static bool operator >=(CodePoint left, CodePoint right)
    {
        return left.value >= right.value;
    }

    /// <summary>
    ///     Returns <see langword="true" /> if <paramref name="value" /> is a valid Unicode code
    ///     point, i.e., is in [ U+0000..U+10FFFF ], inclusive.
    /// </summary>
    /// <param name="value">The value to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="value" /> represents a valid codepoint; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsValid(int value)
    {
        return IsValid((uint)value);
    }

    /// <summary>
    ///     Returns <see langword="true" /> if <paramref name="value" /> is a valid Unicode code
    ///     point, i.e., is in [ U+0000..U+10FFFF ], inclusive.
    /// </summary>
    /// <param name="value">The value to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="value" /> represents a valid codepoint; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsValid(uint value)
    {
        return UnicodeUtility.IsValidCodePoint(value);
    }

    /// <summary>
    ///     Gets a value indicating whether the given codepoint is white space.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a whitespace character; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsWhiteSpace(CodePoint codePoint)
    {
        if (codePoint.IsAscii) return (AsciiCharInfo[codePoint.Value] & IsWhiteSpaceFlag) != 0;

        // Only BMP code points can be white space, so only call into char
        // if the incoming value is within the BMP.
        return codePoint.IsBmp && char.IsWhiteSpace((char)codePoint.Value);
    }

    /// <summary>
    ///     Gets a value indicating whether the given codepoint is a non-breaking space.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a non-breaking space character; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsNonBreakingSpace(CodePoint codePoint)
    {
        return codePoint.Value == 0x00A0;
    }

    /// <summary>
    ///     Gets a value indicating whether the given codepoint is a zero-width-non-joiner.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a zero-width-non-joiner character; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsZeroWidthNonJoiner(CodePoint codePoint)
    {
        return codePoint.Value == 0x200C;
    }

    /// <summary>
    ///     Gets a value indicating whether the given codepoint is a zero-width-joiner.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a zero-width-joiner character; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsZeroWidthJoiner(CodePoint codePoint)
    {
        return codePoint.Value == 0x200D;
    }

    /// <summary>
    ///     Gets a value indicating whether the given codepoint is a variation selector.
    ///     <see href="https://en.wikipedia.org/wiki/Variation_Selectors_%28Unicode_block%29" />
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a variation selector character; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsVariationSelector(CodePoint codePoint)
    {
        return (codePoint.Value & 0xFFF0) == 0xFE00;
    }

    /// <summary>
    ///     Gets a value indicating whether the given codepoint is a control character.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a control character; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsControl(CodePoint codePoint)
    {
        // Per the Unicode stability policy, the set of control characters
        // is forever fixed at [ U+0000..U+001F ], [ U+007F..U+009F ]. No
        // characters will ever be added to or removed from the "control characters"
        // group. See https://www.unicode.org/policies/stability_policy.html.
        //
        // Logic below depends on CodePoint.Value never being -1 (since CodePoint is a validating type)
        // 00..1F (+1) => 01..20 (&~80) => 01..20
        // 7F..9F (+1) => 80..A0 (&~80) => 00..20
        return ((codePoint.value + 1) & ~0x80u) <= 0x20u;
    }

    /// <summary>
    ///     Returns a value that indicates whether the specified codepoint is categorized as a decimal digit.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a decimal digit; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsDigit(CodePoint codePoint)
    {
        if (codePoint.IsAscii)
            return UnicodeUtility.IsInRangeInclusive(codePoint.value, '0', '9');
        return GetGeneralCategory(codePoint) == UnicodeCategory.DecimalDigitNumber;
    }

    /// <summary>
    ///     Returns a value that indicates whether the specified codepoint is categorized as a letter.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a letter; otherwise, <see langword="false" />
    /// </returns>
    public static bool IsLetter(CodePoint codePoint)
    {
        if (codePoint.IsAscii)
            return ((codePoint.value - 'A') & ~0x20u) <= 'Z' - 'A'; // [A-Za-z]
        return IsCategoryLetter(GetGeneralCategory(codePoint));
    }

    /// <summary>
    ///     Returns a value that indicates whether the specified codepoint is categorized as a letter or decimal digit.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a letter or decimal digit; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsLetterOrDigit(CodePoint codePoint)
    {
        if (codePoint.IsAscii)
            return (AsciiCharInfo[codePoint.Value] & IsLetterOrDigitFlag) != 0;
        return IsCategoryLetterOrDecimalDigit(GetGeneralCategory(codePoint));
    }

    /// <summary>
    ///     Returns a value that indicates whether the specified codepoint is categorized as a lowercase letter.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a lowercase letter; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsLower(CodePoint codePoint)
    {
        if (codePoint.IsAscii)
            return UnicodeUtility.IsInRangeInclusive(codePoint.value, 'a', 'z');
        return GetGeneralCategory(codePoint) == UnicodeCategory.LowercaseLetter;
    }

    /// <summary>
    ///     Returns a value that indicates whether the specified codepoint is categorized as a number.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a number; otherwise, <see langword="false" />
    /// </returns>
    public static bool IsNumber(CodePoint codePoint)
    {
        if (codePoint.IsAscii)
            return UnicodeUtility.IsInRangeInclusive(codePoint.value, '0', '9');
        return IsCategoryNumber(GetGeneralCategory(codePoint));
    }

    /// <summary>
    ///     Returns a value that indicates whether the specified codepoint is categorized as punctuation.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is punctuation; otherwise, <see langword="false" />
    /// </returns>
    public static bool IsPunctuation(CodePoint codePoint)
    {
        return IsCategoryPunctuation(GetGeneralCategory(codePoint));
    }

    /// <summary>
    ///     Returns a value that indicates whether the specified codepoint is categorized as a separator.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a separator; otherwise, <see langword="false" />
    /// </returns>
    public static bool IsSeparator(CodePoint codePoint)
    {
        return IsCategorySeparator(GetGeneralCategory(codePoint));
    }

    /// <summary>
    ///     Returns a value that indicates whether the specified codepoint is categorized as a symbol.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a symbol; otherwise, <see langword="false" />
    /// </returns>
    public static bool IsSymbol(CodePoint codePoint)
    {
        return IsCategorySymbol(GetGeneralCategory(codePoint));
    }

    /// <summary>
    ///     Returns a value that indicates whether the specified codepoint is categorized as a mark.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a symbol; otherwise, <see langword="false" />
    /// </returns>
    public static bool IsMark(CodePoint codePoint)
    {
        return IsCategoryMark(GetGeneralCategory(codePoint));
    }

    /// <summary>
    ///     Returns a value that indicates whether the specified codepoint is categorized as an uppercase letter.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a uppercase letter; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsUpper(CodePoint codePoint)
    {
        if (codePoint.IsAscii)
            return UnicodeUtility.IsInRangeInclusive(codePoint.value, 'A', 'Z');
        return GetGeneralCategory(codePoint) == UnicodeCategory.UppercaseLetter;
    }

    /// <summary>
    ///     Gets a value indicating whether the given codepoint is a tabulation indicator.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a tabulation indicator; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsTabulation(CodePoint codePoint)
    {
        return codePoint.value == 0x0009;
    }

    /// <summary>
    ///     Gets a value indicating whether the given codepoint is a new line indicator.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="codePoint" /> is a new line indicator; otherwise,
    ///     <see langword="false" />
    /// </returns>
    public static bool IsNewLine(CodePoint codePoint)
    {
        return codePoint.Value switch
        {
            // See https://www.unicode.org/standard/reports/tr13/tr13-5.html
            0x000A // LINE FEED (LF)
                or 0x000B // LINE TABULATION (VT)
                or 0x000C // FORM FEED (FF)
                or 0x000D // CARRIAGE RETURN (CR)
                or 0x0085 // NEXT LINE (NEL)
                or 0x2028 // LINE SEPARATOR (LS)
                or 0x2029 => true, // PARAGRAPH SEPARATOR (PS)
            _ => false
        };
    }

    /// <summary>
    ///     Returns the number of codepoints in a given string buffer.
    /// </summary>
    /// <param name="source">The source buffer to parse.</param>
    /// <returns>The <see cref="int" /> count.</returns>
    public static int GetCodePointCount(ReadOnlySpan<char> source)
    {
        if (source.IsEmpty) return 0;

        var count = 0;
        var enumerator = new SpanCodePointEnumerator(source);
        while (enumerator.MoveNext()) count++;

        return count;
    }

    /// <summary>
    ///     Gets the canonical representation of a given codepoint.
    ///     <see href="http://www.unicode.org/L2/L2013/13123-norm-and-bpa.pdf" />
    /// </summary>
    /// <param name="codePoint">The code point to be mapped.</param>
    /// <returns>The mapped canonical code point, or the passed <paramref name="codePoint" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static CodePoint GetCanonicalType(CodePoint codePoint)
    {
        if (codePoint.Value == 0x3008) return new CodePoint(0x2329);

        if (codePoint.Value == 0x3009) return new CodePoint(0x232A);

        return codePoint;
    }

    /// <summary>
    ///     Gets the <see cref="BidiClass" /> for the given codepoint.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>The <see cref="BidiClass" />.</returns>
    public static BidiClass GetBidiClass(CodePoint codePoint)
    {
        return new BidiClass(codePoint);
    }

    /// <summary>
    ///     Gets the codepoint representing the bidi mirror for this instance.
    ///     <see href="http://www.unicode.org/reports/tr44/#Bidi_Mirrored" />
    /// </summary>
    /// <param name="codePoint">The code point to be mapped.</param>
    /// <param name="mirror">
    ///     When this method returns, contains the codepoint representing the bidi mirror for this instance;
    ///     otherwise, the default value for the type of the <paramref name="codePoint" /> parameter.
    ///     This parameter is passed uninitialized.
    ///     .
    /// </param>
    /// <returns><see langword="true" /> if this instance has a mirror; otherwise, <see langword="false" /></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetBidiMirror(CodePoint codePoint, out CodePoint mirror)
    {
        var value = UnicodeData.GetBidiMirror(codePoint.value);

        if (value == 0u)
        {
            mirror = default;
            return false;
        }

        mirror = new CodePoint(value);
        return true;
    }

    /// <summary>
    ///     Gets the codepoint representing the vertical mirror for this instance.
    ///     <see href="https://www.unicode.org/reports/tr50/#vertical_alternates" />
    /// </summary>
    /// <param name="codePoint">The code point to be mapped.</param>
    /// <param name="mirror">
    ///     When this method returns, contains the codepoint representing the vertical mirror for this instance;
    ///     otherwise, the default value for the type of the <paramref name="codePoint" /> parameter.
    ///     This parameter is passed uninitialized.
    ///     .
    /// </param>
    /// <returns><see langword="true" /> if this instance has a mirror; otherwise, <see langword="false" /></returns>
    public static bool TryGetVerticalMirror(CodePoint codePoint, out CodePoint mirror)
    {
        var value = UnicodeUtility.GetVerticalMirror((uint)codePoint.Value);

        if (value == 0u)
        {
            mirror = default;
            return false;
        }

        mirror = new CodePoint(value);
        return true;
    }

    /// <summary>
    ///     Gets the <see cref="LineBreakClass" /> for the given codepoint.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>The <see cref="LineBreakClass" />.</returns>
    public static LineBreakClass GetLineBreakClass(CodePoint codePoint)
    {
        return UnicodeData.GetLineBreakClass(codePoint.value);
    }

    /// <summary>
    ///     Gets the <see cref="GraphemeClusterClass" /> for the given codepoint.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>The <see cref="GraphemeClusterClass" />.</returns>
    public static GraphemeClusterClass GetGraphemeClusterClass(CodePoint codePoint)
    {
        return UnicodeData.GetGraphemeClusterClass(codePoint.value);
    }

    /// <summary>
    ///     Gets the <see cref="JoiningClass" /> for the given codepoint.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>The <see cref="BidiClass" />.</returns>
    internal static JoiningClass GetJoiningClass(CodePoint codePoint)
    {
        return new JoiningClass(codePoint);
    }

    /// <summary>
    ///     Gets the <see cref="ScriptClass" /> for the given codepoint.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>The <see cref="ScriptClass" />.</returns>
    internal static ScriptClass GetScriptClass(CodePoint codePoint)
    {
        return UnicodeData.GetScriptClass(codePoint.value);
    }

    /// <summary>
    ///     Gets the <see cref="UnicodeCategory" /> for the given codepoint.
    /// </summary>
    /// <param name="codePoint">The codepoint to evaluate.</param>
    /// <returns>The <see cref="UnicodeCategory" />.</returns>
    public static UnicodeCategory GetGeneralCategory(CodePoint codePoint)
    {
        if (codePoint.IsAscii) return (UnicodeCategory)(AsciiCharInfo[codePoint.Value] & UnicodeCategoryMask);

        return UnicodeData.GetUnicodeCategory(codePoint.value);
    }

    /// <summary>
    ///     Reads the <see cref="CodePoint" /> at specified position.
    /// </summary>
    /// <param name="text">The text to read from.</param>
    /// <param name="index">The index to read at.</param>
    /// <param name="charsConsumed">The count of chars consumed reading the buffer.</param>
    /// <returns>The <see cref="CodePoint" />.</returns>
    internal static CodePoint ReadAt(string text, int index, out int charsConsumed)
    {
        return DecodeFromUtf16At(text.AsMemory().Span, index, out charsConsumed);
    }

    /// <summary>
    ///     Decodes the <see cref="CodePoint" /> from the provided UTF-16 source buffer at the specified position.
    /// </summary>
    /// <param name="source">The buffer to read from.</param>
    /// <param name="index">The index to read at.</param>
    /// <returns>The <see cref="CodePoint" />.</returns>
    internal static CodePoint DecodeFromUtf16At(ReadOnlySpan<char> source, int index)
    {
        return DecodeFromUtf16At(source, index, out var _);
    }

    /// <summary>
    ///     Decodes the <see cref="CodePoint" /> from the provided UTF-16 source buffer at the specified position.
    /// </summary>
    /// <param name="source">The buffer to read from.</param>
    /// <param name="index">The index to read at.</param>
    /// <param name="charsConsumed">The count of chars consumed reading the buffer.</param>
    /// <returns>The <see cref="CodePoint" />.</returns>
    internal static CodePoint DecodeFromUtf16At(ReadOnlySpan<char> source, int index, out int charsConsumed)
    {
        if (index >= source.Length)
        {
            charsConsumed = 0;
            return default;
        }

        // Optimistically assume input is within BMP.
        charsConsumed = 1;
        uint code = source[index];

        // High surrogate
        if (UnicodeUtility.IsHighSurrogateCodePoint(code))
        {
            uint hi, low;

            hi = code;
            index++;

            if (index == source.Length) return ReplacementChar;

            low = source[index];

            if (UnicodeUtility.IsLowSurrogateCodePoint(low))
            {
                charsConsumed = 2;
                return new CodePoint(UnicodeUtility.GetScalarFromUtf16SurrogatePair(hi, low));
            }

            return ReplacementChar;
        }

        return new CodePoint(code);
    }

    /// <inheritdoc cref="IComparable.CompareTo" />
    int IComparable.CompareTo(object? obj)
    {
        if (obj is null) return 1; // non-null ("this") always sorts after null

        if (obj is CodePoint other) return CompareTo(other);

        throw new ArgumentException("Object must be of type CodePoint.");
    }

    /// <inheritdoc />
    public int CompareTo(CodePoint other) // Values don't span entire 32-bit domain so won't integer overflow.
    {
    return Value - other.Value;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is CodePoint point && Equals(point);
    }

    /// <inheritdoc />
    public bool Equals(CodePoint other)
    {
        return value == other.value;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(value);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (IsBmp)
        {
            return ((char)value).ToString();
        }

        Span<char> buffer = stackalloc char[MaxUtf16CharsPerCodePoint];
        UnicodeUtility.GetUtf16SurrogatesFromSupplementaryPlaneCodePoint(value, out buffer[0], out buffer[1]);
        return buffer.ToString();
    }

    /// <summary>
    ///     Returns this instance displayed as &quot;&apos;&lt;char&gt;&apos; (U+XXXX)&quot;; e.g., &quot;&apos;e&apos;
    ///     (U+0065)&quot;
    /// </summary>
    /// <returns>The <see cref="string" />.</returns>
    internal string ToDebuggerDisplay()
    {
        return DebuggerDisplay;
    }

    // Returns true if this Unicode category represents a letter
    private static bool IsCategoryLetter(UnicodeCategory category)
    {
        return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.UppercaseLetter,
            (uint)UnicodeCategory.OtherLetter);
    }

    // Returns true if this Unicode category represents a letter or a decimal digit
    private static bool IsCategoryLetterOrDecimalDigit(UnicodeCategory category)
    {
        return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.UppercaseLetter,
                   (uint)UnicodeCategory.OtherLetter)
               || category == UnicodeCategory.DecimalDigitNumber;
    }

    // Returns true if this Unicode category represents a number
    private static bool IsCategoryNumber(UnicodeCategory category)
    {
        return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.DecimalDigitNumber,
            (uint)UnicodeCategory.OtherNumber);
    }

    // Returns true if this Unicode category represents a punctuation mark
    private static bool IsCategoryPunctuation(UnicodeCategory category)
    {
        return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.ConnectorPunctuation,
            (uint)UnicodeCategory.OtherPunctuation);
    }

    // Returns true if this Unicode category represents a separator
    private static bool IsCategorySeparator(UnicodeCategory category)
    {
        return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.SpaceSeparator,
            (uint)UnicodeCategory.ParagraphSeparator);
    }

    // Returns true if this Unicode category represents a symbol
    private static bool IsCategorySymbol(UnicodeCategory category)
    {
        return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.MathSymbol,
            (uint)UnicodeCategory.OtherSymbol);
    }

    // Returns true if this Unicode category represents a mark
    private static bool IsCategoryMark(UnicodeCategory category)
    {
        return UnicodeUtility.IsInRangeInclusive((uint)category, (uint)UnicodeCategory.NonSpacingMark,
            (uint)UnicodeCategory.EnclosingMark);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowArgumentOutOfRange(uint value, string paramName, string message)
    {
        throw new ArgumentOutOfRangeException(paramName,
            $"The value {UnicodeUtility.ToHexString(value)} is not a valid Unicode code point value. {message}");
    }
}