// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Globalization;

namespace FreePPlus.Imaging.Fonts.Unicode;

//was previously: namespace SixLabors.Fonts.Unicode;

/// <summary>
///     Supports a simple iteration over a linebreak collection.
///     Implementation of the Unicode Line Break Algorithm. UAX:14
///     <see href="https://www.unicode.org/reports/tr14/tr14-37.html" />
///     Methods are pattern-matched by compiler to allow using foreach pattern.
/// </summary>
internal ref struct LineBreakEnumerator
{
    private readonly ReadOnlySpan<char> source;
    private int charPosition;
    private readonly int pointsLength;
    private int position;
    private int lastPosition;
    private LineBreakClass currentClass;
    private LineBreakClass nextClass;
    private bool first;
    private int alphaNumericCount;
    private bool lb8a;
    private bool lb21a;
    private bool lb22ex;
    private bool lb24ex;
    private bool lb25ex;
    private bool lb30;
    private int lb30a;
    private bool lb31;

    public LineBreakEnumerator(ReadOnlySpan<char> source)
        : this()
    {
        this.source = source;
        pointsLength = CodePoint.GetCodePointCount(source);
        charPosition = 0;
        position = 0;
        lastPosition = 0;
        currentClass = LineBreakClass.XX;
        nextClass = LineBreakClass.XX;
        first = true;
        lb8a = false;
        lb21a = false;
        lb22ex = false;
        lb24ex = false;
        lb25ex = false;
        alphaNumericCount = 0;
        lb31 = false;
        lb30 = false;
        lb30a = 0;
    }

    public LineBreak Current { get; private set; }

    /// <summary>
    ///     Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that iterates through the collection.</returns>
    public LineBreakEnumerator GetEnumerator()
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
        // Get the first char if we're at the beginning of the string.
        if (first)
        {
            var firstClass = NextCharClass();
            first = false;
            currentClass = MapFirst(firstClass);
            nextClass = firstClass;
            lb8a = firstClass == LineBreakClass.ZWJ;
            lb30a = 0;
        }

        while (position < pointsLength)
        {
            lastPosition = position;
            var lastClass = nextClass;
            nextClass = NextCharClass();

            // Explicit newline
            switch (currentClass)
            {
                case LineBreakClass.BK:
                case LineBreakClass.CR when nextClass != LineBreakClass.LF:
                    currentClass = MapFirst(nextClass);
                    Current = new LineBreak(FindPriorNonWhitespace(lastPosition), lastPosition, true);
                    return true;
            }

            var shouldBreak = GetSimpleBreak() ?? (bool?)GetPairTableBreak(lastClass);

            // Rule LB8a
            lb8a = nextClass == LineBreakClass.ZWJ;

            if (shouldBreak.Value)
            {
                Current = new LineBreak(FindPriorNonWhitespace(lastPosition), lastPosition);
                return true;
            }
        }

        if (position >= pointsLength && lastPosition < pointsLength)
        {
            lastPosition = pointsLength;
            var required = false;
            switch (currentClass)
            {
                case LineBreakClass.BK:
                case LineBreakClass.CR when nextClass != LineBreakClass.LF:
                    required = true;
                    break;
            }

            Current = new LineBreak(FindPriorNonWhitespace(pointsLength), lastPosition, required);
            return true;
        }

        Current = default;
        return false;
    }

    private LineBreakClass MapClass(CodePoint cp, LineBreakClass c)
    {
        // LB 1
        // ==========================================
        // Resolved Original    General_Category
        // ==========================================
        // AL       AI, SG, XX  Any
        // CM       SA          Only Mn or Mc
        // AL       SA          Any except Mn and Mc
        // NS       CJ          Any
        switch (c)
        {
            case LineBreakClass.AI:
            case LineBreakClass.SG:
            case LineBreakClass.XX:
                return LineBreakClass.AL;

            case LineBreakClass.SA:
                var category = CodePoint.GetGeneralCategory(cp);
                return category is UnicodeCategory.NonSpacingMark or UnicodeCategory.SpacingCombiningMark
                    ? LineBreakClass.CM
                    : LineBreakClass.AL;

            case LineBreakClass.CJ:
                return LineBreakClass.NS;

            default:
                return c;
        }
    }

    private LineBreakClass MapFirst(LineBreakClass c)
    {
        return c switch
        {
            LineBreakClass.LF or LineBreakClass.NL => LineBreakClass.BK,
            LineBreakClass.SP => LineBreakClass.WJ,
            _ => c
        };
    }

    private bool IsAlphaNumeric(LineBreakClass cls)
    {
        return cls is LineBreakClass.AL
            or LineBreakClass.HL
            or LineBreakClass.NU;
    }

    private LineBreakClass PeekNextCharClass()
    {
        var cp = CodePoint.DecodeFromUtf16At(source, charPosition);
        return MapClass(cp, CodePoint.GetLineBreakClass(cp));
    }

    // Get the next character class
    private LineBreakClass NextCharClass()
    {
        var cp = CodePoint.DecodeFromUtf16At(source, charPosition, out var count);
        var cls = MapClass(cp, CodePoint.GetLineBreakClass(cp));
        charPosition += count;
        position++;

        // Keep track of alphanumeric + any combining marks.
        // This is used for LB22 and LB30.
        if (IsAlphaNumeric(currentClass) || (alphaNumericCount > 0 && cls == LineBreakClass.CM)) alphaNumericCount++;

        // Track combining mark exceptions. LB22
        if (cls == LineBreakClass.CM)
            switch (currentClass)
            {
                case LineBreakClass.BK:
                case LineBreakClass.CB:
                case LineBreakClass.EX:
                case LineBreakClass.LF:
                case LineBreakClass.NL:
                case LineBreakClass.SP:
                case LineBreakClass.ZW:
                case LineBreakClass.CR:
                    lb22ex = true;
                    break;
            }

        // Track combining mark exceptions. LB31
        if (first && cls == LineBreakClass.CM) lb31 = true;

        if (cls == LineBreakClass.CM)
            switch (currentClass)
            {
                case LineBreakClass.BK:
                case LineBreakClass.CB:
                case LineBreakClass.EX:
                case LineBreakClass.LF:
                case LineBreakClass.NL:
                case LineBreakClass.SP:
                case LineBreakClass.ZW:
                case LineBreakClass.CR:
                case LineBreakClass.ZWJ:
                    lb31 = true;
                    break;
            }

        if (first
            && (cls == LineBreakClass.PO || cls == LineBreakClass.PR || cls == LineBreakClass.SP))
            lb31 = true;

        if (currentClass == LineBreakClass.AL
            && (cls == LineBreakClass.PO || cls == LineBreakClass.PR || cls == LineBreakClass.SP))
            lb31 = true;

        // Reset LB31 if next is U+0028 (Left Opening Parenthesis)
        if (lb31
            && currentClass != LineBreakClass.PO
            && currentClass != LineBreakClass.PR
            && cls == LineBreakClass.OP && cp.Value == 0x0028)
            lb31 = false;

        // Rule LB24
        if (first && (cls == LineBreakClass.CL || cls == LineBreakClass.CP)) lb24ex = true;

        // Rule LB25
        if (first
            && (cls == LineBreakClass.CL || cls == LineBreakClass.IS || cls == LineBreakClass.SY))
            lb25ex = true;

        if (cls is LineBreakClass.SP or LineBreakClass.WJ or LineBreakClass.AL)
        {
            var next = PeekNextCharClass();
            if (next is LineBreakClass.CL or LineBreakClass.IS or LineBreakClass.SY) lb25ex = true;
        }

        // AlphaNumeric + and combining marks can break for OP except.
        // - U+0028 (Left Opening Parenthesis)
        // - U+005B (Opening Square Bracket)
        // - U+007B (Left Curly Bracket)
        // See custom columns|rules in the text pair table.
        // https://www.unicode.org/Public/13.0.0/ucd/auxiliary/LineBreakTest.html
        lb30 = alphaNumericCount > 0
               && cls == LineBreakClass.OP
               && cp.Value != 0x0028
               && cp.Value != 0x005B
               && cp.Value != 0x007B;

        return cls;
    }

    private bool? GetSimpleBreak()
    {
        // handle classes not handled by the pair table
        switch (nextClass)
        {
            case LineBreakClass.SP:
                return false;

            case LineBreakClass.BK:
            case LineBreakClass.LF:
            case LineBreakClass.NL:
                currentClass = LineBreakClass.BK;
                return false;

            case LineBreakClass.CR:
                currentClass = LineBreakClass.CR;
                return false;
        }

        return null;
    }

    private bool GetPairTableBreak(LineBreakClass lastClass)
    {
        // If not handled already, use the pair table
        var shouldBreak = false;
        switch (LineBreakPairTable.Table[(int)currentClass][(int)nextClass])
        {
            case LineBreakPairTable.DIBRK: // Direct break
                shouldBreak = true;
                break;

            // TODO: Rewrite this so that it defaults to true and rules are set as exceptions.
            case LineBreakPairTable.INBRK: // Possible indirect break

                // LB31
                if (lb31 && nextClass == LineBreakClass.OP)
                {
                    shouldBreak = true;
                    lb31 = false;
                    break;
                }

                // LB30
                if (lb30)
                {
                    shouldBreak = true;
                    lb30 = false;
                    alphaNumericCount = 0;
                    break;
                }

                // LB25
                if (lb25ex && (nextClass == LineBreakClass.PR || nextClass == LineBreakClass.NU))
                {
                    shouldBreak = true;
                    lb25ex = false;
                    break;
                }

                // LB24
                if (lb24ex && (nextClass == LineBreakClass.PO || nextClass == LineBreakClass.PR))
                {
                    shouldBreak = true;
                    lb24ex = false;
                    break;
                }

                // LB18
                shouldBreak = lastClass == LineBreakClass.SP;
                break;

            case LineBreakPairTable.CIBRK:
                shouldBreak = lastClass == LineBreakClass.SP;
                if (!shouldBreak) return false;

                break;

            case LineBreakPairTable.CPBRK: // prohibited for combining marks
                if (lastClass != LineBreakClass.SP) return false;

                break;

            case LineBreakPairTable.PRBRK:
                break;
        }

        // Rule LB22
        if (nextClass == LineBreakClass.IN)
            switch (lastClass)
            {
                case LineBreakClass.BK:
                case LineBreakClass.CB:
                case LineBreakClass.EX:
                case LineBreakClass.LF:
                case LineBreakClass.NL:
                case LineBreakClass.SP:
                case LineBreakClass.ZW:

                    // Allow break
                    break;
                case LineBreakClass.CM:
                    if (lb22ex)
                    {
                        // Allow break
                        lb22ex = false;
                        break;
                    }

                    shouldBreak = false;
                    break;
                default:
                    shouldBreak = false;
                    break;
            }

        if (lb8a) shouldBreak = false;

        // Rule LB21a
        if (lb21a && (currentClass == LineBreakClass.HY || currentClass == LineBreakClass.BA))
        {
            shouldBreak = false;
            lb21a = false;
        }
        else
        {
            lb21a = currentClass == LineBreakClass.HL;
        }

        // Rule LB30a
        if (currentClass == LineBreakClass.RI)
        {
            lb30a++;
            if (lb30a == 2 && nextClass == LineBreakClass.RI)
            {
                shouldBreak = true;
                lb30a = 0;
            }
        }
        else
        {
            lb30a = 0;
        }

        // Rule LB30b
        if (nextClass == LineBreakClass.EM && lastPosition > 0)
        {
            // Mahjong Tiles (Unicode block) are extended pictographics but have a class of ID
            // Unassigned codepoints with Line_Break=ID in some blocks are also assigned the Extended_Pictographic property.
            // Those blocks are intended for future allocation of emoji characters.
            var cp = CodePoint.DecodeFromUtf16At(source, lastPosition - 1, out var _);
            if (UnicodeUtility.IsInRangeInclusive((uint)cp.Value, 0x1F000, 0x1F02F)) shouldBreak = false;
        }

        currentClass = nextClass;

        return shouldBreak;
    }

    private int FindPriorNonWhitespace(int from)
    {
        if (from > 0)
        {
            var cp = CodePoint.DecodeFromUtf16At(source, from - 1, out var count);
            var cls = CodePoint.GetLineBreakClass(cp);

            if (cls is LineBreakClass.BK or LineBreakClass.LF or LineBreakClass.CR) from -= count;
        }

        while (from > 0)
        {
            var cp = CodePoint.DecodeFromUtf16At(source, from - 1, out var count);
            var cls = CodePoint.GetLineBreakClass(cp);

            if (cls == LineBreakClass.SP)
                from -= count;
            else
                break;
        }

        return from;
    }
}