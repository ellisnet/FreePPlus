// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Formats.Png;

//was previously: namespace SixLabors.ImageSharp.Formats.Png;

/// <summary>
///     Stores text data contained in the iTXt, tEXt, and zTXt chunks.
///     Used for conveying textual information associated with the image, like the name of the author,
///     the copyright information, the date, where the image was created, or some other information.
/// </summary>
public readonly struct PngTextData : IEquatable<PngTextData>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PngTextData" /> struct.
    /// </summary>
    /// <param name="keyword">The keyword of the property.</param>
    /// <param name="value">The value of the property.</param>
    /// <param name="languageTag">An optional language tag.</param>
    /// <param name="translatedKeyword">A optional translated keyword.</param>
    public PngTextData(string keyword, string value, string languageTag, string translatedKeyword)
    {
        Guard.NotNullOrWhiteSpace(keyword, nameof(keyword));

        // No leading or trailing whitespace is allowed in keywords.
        Keyword = keyword.Trim();
        Value = value;
        LanguageTag = languageTag;
        TranslatedKeyword = translatedKeyword;
    }

    /// <summary>
    ///     Gets the keyword of this <see cref="PngTextData" /> which indicates
    ///     the type of information represented by the text string as described in https://www.w3.org/TR/PNG/#11keywords.
    /// </summary>
    /// <example>
    ///     Typical properties are the author, copyright information or other meta information.
    /// </example>
    public string Keyword { get; }

    /// <summary>
    ///     Gets the value of this <see cref="PngTextData" />.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Gets an optional language tag defined in https://www.w3.org/TR/PNG/#2-RFC-3066 indicates the human language used by
    ///     the translated keyword and the text.
    ///     If the first word is two or three letters long, it is an ISO language code https://www.w3.org/TR/PNG/#2-ISO-639.
    /// </summary>
    /// <example>
    ///     Examples: cn, en-uk, no-bok, x-klingon, x-KlInGoN.
    /// </example>
    public string LanguageTag { get; }

    /// <summary>
    ///     Gets an optional translated keyword, should contain a translation of the keyword into the language indicated by the
    ///     language tag.
    /// </summary>
    public string TranslatedKeyword { get; }

    /// <summary>
    ///     Compares two <see cref="PngTextData" /> objects. The result specifies whether the values
    ///     of the properties of the two <see cref="PngTextData" /> objects are equal.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="PngTextData" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="PngTextData" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the current left is equal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    public static bool operator ==(PngTextData left, PngTextData right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Compares two <see cref="PngTextData" /> objects. The result specifies whether the values
    ///     of the properties of the two <see cref="PngTextData" /> objects are unequal.
    /// </summary>
    /// <param name="left">
    ///     The <see cref="PngTextData" /> on the left side of the operand.
    /// </param>
    /// <param name="right">
    ///     The <see cref="PngTextData" /> on the right side of the operand.
    /// </param>
    /// <returns>
    ///     True if the current left is unequal to the <paramref name="right" /> parameter; otherwise, false.
    /// </returns>
    public static bool operator !=(PngTextData left, PngTextData right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="obj">
    ///     The object to compare with the current instance.
    /// </param>
    /// <returns>
    ///     true if <paramref name="obj" /> and this instance are the same type and represent the
    ///     same value; otherwise, false.
    /// </returns>
    public override bool Equals(object obj)
    {
        return obj is PngTextData other && Equals(other);
    }

    /// <summary>
    ///     Returns the hash code for this instance.
    /// </summary>
    /// <returns>
    ///     A 32-bit signed integer that is the hash code for this instance.
    /// </returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Keyword, Value, LanguageTag, TranslatedKeyword);
    }

    /// <summary>
    ///     Returns the fully qualified type name of this instance.
    /// </summary>
    /// <returns>
    ///     A <see cref="T:System.String" /> containing a fully qualified type name.
    /// </returns>
    public override string ToString()
    {
        return $"PngTextData [ Name={Keyword}, Value={Value} ]";
    }

    /// <summary>
    ///     Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    ///     True if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(PngTextData other)
    {
        return Keyword.Equals(other.Keyword)
               && Value.Equals(other.Value)
               && LanguageTag.Equals(other.LanguageTag)
               && TranslatedKeyword.Equals(other.TranslatedKeyword);
    }
}