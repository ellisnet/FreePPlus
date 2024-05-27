// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

/// <summary>
///     Decodes the commands and numbers making up a Type 2 CharString. A Type 2 CharString extends on the Type 1
///     CharString format.
///     Compared to the Type 1 format, the Type 2 encoding offers smaller size and an opportunity for better rendering
///     quality and
///     performance. The Type 2 charstring operators are (with one exception) a superset of the Type 1 operators.
/// </summary>
/// <remarks>
///     A Type 2 charstring program is a sequence of unsigned 8-bit bytes that encode numbers and operators.
///     The byte value specifies a operator, a number, or subsequent bytes that are to be interpreted in a specific manner
/// </remarks>
internal ref struct CffEvaluationEngine
{
    private static readonly Random Random = new();
    private float? width;
    private int nStems;
    private float x;
    private float y;
    private RefStack<float> stack;
    private readonly ReadOnlySpan<byte> charStrings;
    private readonly ReadOnlySpan<byte[]> globalSubrBuffers;
    private readonly ReadOnlySpan<byte[]> localSubrBuffers;
    private TransformingGlyphRenderer transforming;
    private readonly int nominalWidthX;
    private readonly int globalBias;
    private readonly int localBias;
    private readonly Dictionary<int, float> trans;
    private bool isDisposed;

    public CffEvaluationEngine(
        ReadOnlySpan<byte> charStrings,
        ReadOnlySpan<byte[]> globalSubrBuffers,
        ReadOnlySpan<byte[]> localSubrBuffers,
        int nominalWidthX)
    {
        transforming = default;
        this.charStrings = charStrings;
        this.globalSubrBuffers = globalSubrBuffers;
        this.localSubrBuffers = localSubrBuffers;
        this.nominalWidthX = nominalWidthX;

        globalBias = CalculateBias(this.globalSubrBuffers.Length);
        localBias = CalculateBias(this.localSubrBuffers.Length);
        trans = new Dictionary<int, float>();

        x = 0;
        y = 0;
        width = null;
        nStems = 0;
        stack = new RefStack<float>(50);
        isDisposed = false;
    }

    public Bounds GetBounds()
    {
        Reset();

        // TODO: It would be nice to avoid the allocation here.
        CffBoundsFinder finder = new();
        transforming = new TransformingGlyphRenderer(Vector2.One, Vector2.Zero, finder);

        // Boolean IGlyphRenderer.BeginGlyph(..) is handled by the caller.
        Parse(charStrings);

        // Some CFF end without closing the latest contour.
        if (transforming.IsOpen) transforming.EndFigure();

        return finder.GetBounds();
    }

    public void RenderTo(IGlyphRenderer renderer, Vector2 scale, Vector2 offset)
    {
        Reset();

        transforming = new TransformingGlyphRenderer(scale, offset, renderer);

        // Boolean IGlyphRenderer.BeginGlyph(..) is handled by the caller.
        Parse(charStrings);

        // Some CFF end without closing the latest contour.
        if (transforming.IsOpen) transforming.EndFigure();
    }

    private void Parse(ReadOnlySpan<byte> buffer)
    {
        SimpleBinaryReader reader = new(buffer);
        var endCharEncountered = false;
        while (!endCharEncountered && reader.CanRead())
        {
            var b0 = reader.ReadByte();
            if (b0 < 32)
            {
                int index;
                ReadOnlySpan<byte> subr;
                bool phase;
                float c1x;
                float c1y;
                float c2x;
                float c2y;

                var oneByteOperator = (Type2Operator1)b0;
                switch (oneByteOperator)
                {
                    case Type2Operator1.Hstem:
                    case Type2Operator1.Vstem:
                    case Type2Operator1.Hstemhm:
                    case Type2Operator1.Vstemhm:

                        ParseStems();
                        break;

                    case Type2Operator1.Vmoveto:

                        if (stack.Length > 1) CheckWidth();

                        y += stack.Shift();
                        transforming.MoveTo(new Vector2(x, y));

                        stack.Clear();
                        break;

                    case Type2Operator1.Rlineto:

                        while (stack.Length >= 2)
                        {
                            x += stack.Shift();
                            y += stack.Shift();
                            transforming.LineTo(new Vector2(x, y));
                        }

                        stack.Clear();
                        break;

                    case Type2Operator1.Hlineto:
                    case Type2Operator1.Vlineto:
                        phase = oneByteOperator == Type2Operator1.Hlineto;

                        while (stack.Length >= 1)
                        {
                            if (phase)
                                x += stack.Shift();
                            else
                                y += stack.Shift();

                            transforming.LineTo(new Vector2(x, y));
                            phase = !phase;
                        }

                        stack.Clear();
                        break;

                    case Type2Operator1.Rrcurveto:

                        while (stack.Length > 0)
                            transforming.CubicBezierTo(
                                new Vector2(x += stack.Shift(), y += stack.Shift()),
                                new Vector2(x += stack.Shift(), y += stack.Shift()),
                                new Vector2(x += stack.Shift(), y += stack.Shift()));

                        stack.Clear();
                        break;

                    case Type2Operator1.Callsubr:
                        index = (int)stack.Pop() + localBias;
                        subr = localSubrBuffers[index];

                        if (subr.Length > 0) Parse(subr);

                        break;

                    case Type2Operator1.Return:

                        // TODO: CFF2
                        return;

                    case Type2Operator1.Endchar:

                        // TODO: CFF2
                        if (stack.Length > 0) CheckWidth();

                        endCharEncountered = true;
                        break;

                    case Type2Operator1.Reserved15_:

                        // TODO: CFF2
                        break;
                    case Type2Operator1.Reserved16_:

                        // TODO: CFF2
                        break;
                    case Type2Operator1.Hintmask:
                    case Type2Operator1.Cntrmask:

                        ParseStems();
                        reader.Position += (nStems + 7) >> 3;

                        break;

                    case Type2Operator1.Rmoveto:

                        if (stack.Length > 2) CheckWidth();

                        x += stack.Shift();
                        y += stack.Shift();
                        transforming.MoveTo(new Vector2(x, y));

                        stack.Clear();
                        break;

                    case Type2Operator1.Hmoveto:

                        if (stack.Length > 1) CheckWidth();

                        x += stack.Shift();
                        transforming.MoveTo(new Vector2(x, y));

                        stack.Clear();
                        break;

                    case Type2Operator1.Rcurveline:

                        while (stack.Length >= 8)
                            transforming.CubicBezierTo(
                                new Vector2(x += stack.Shift(), y += stack.Shift()),
                                new Vector2(x += stack.Shift(), y += stack.Shift()),
                                new Vector2(x += stack.Shift(), y += stack.Shift()));

                        transforming.LineTo(new Vector2(x += stack.Shift(), y += stack.Shift()));

                        stack.Clear();
                        break;

                    case Type2Operator1.Rlinecurve:

                        while (stack.Length >= 8)
                        {
                            x += stack.Shift();
                            y += stack.Shift();
                            transforming.LineTo(new Vector2(x, y));
                        }

                        c1x = x + stack.Shift();
                        c1y = y + stack.Shift();
                        c2x = c1x + stack.Shift();
                        c2y = c1y + stack.Shift();
                        x = c2x + stack.Shift();
                        y = c2y + stack.Shift();

                        transforming.CubicBezierTo(
                            new Vector2(c1x, c1y),
                            new Vector2(c2x, c2y),
                            new Vector2(x, y));

                        stack.Clear();
                        break;

                    case Type2Operator1.Vvcurveto:

                        if (stack.Length % 2 != 0) x += stack.Shift();

                        while (stack.Length >= 4)
                        {
                            c1x = x;
                            c1y = y + stack.Shift();
                            c2x = c1x + stack.Shift();
                            c2y = c1y + stack.Shift();
                            x = c2x;
                            y = c2y + stack.Shift();

                            transforming.CubicBezierTo(
                                new Vector2(c1x, c1y),
                                new Vector2(c2x, c2y),
                                new Vector2(x, y));
                        }

                        stack.Clear();
                        break;

                    case Type2Operator1.Hhcurveto:

                        if (stack.Length % 2 != 0) y += stack.Shift();

                        while (stack.Length >= 4)
                        {
                            c1x = x + stack.Shift();
                            c1y = y;
                            c2x = c1x + stack.Shift();
                            c2y = c1y + stack.Shift();
                            x = c2x + stack.Shift();
                            y = c2y;

                            transforming.CubicBezierTo(
                                new Vector2(c1x, c1y),
                                new Vector2(c2x, c2y),
                                new Vector2(x, y));
                        }

                        stack.Clear();
                        break;

                    case Type2Operator1.Shortint:

                        stack.Push(reader.ReadInt16BE());
                        break;

                    case Type2Operator1.Callgsubr:

                        index = (int)stack.Pop() + globalBias;
                        subr = globalSubrBuffers[index];

                        if (subr.Length > 0) Parse(subr);

                        break;

                    case Type2Operator1.Vhcurveto:
                    case Type2Operator1.Hvcurveto:

                        phase = oneByteOperator == Type2Operator1.Hvcurveto;
                        while (stack.Length >= 4)
                        {
                            if (phase)
                            {
                                c1x = x + stack.Shift();
                                c1y = y;
                                c2x = c1x + stack.Shift();
                                c2y = c1y + stack.Shift();
                                y = c2y + stack.Shift();
                                x = c2x + (stack.Length == 1 ? stack.Shift() : 0);
                            }
                            else
                            {
                                c1x = x;
                                c1y = y + stack.Shift();
                                c2x = c1x + stack.Shift();
                                c2y = c1y + stack.Shift();
                                x = c2x + stack.Shift();
                                y = c2y + (stack.Length == 1 ? stack.Shift() : 0);
                            }

                            transforming.CubicBezierTo(new Vector2(c1x, c1y), new Vector2(c2x, c2y), new Vector2(x, y));
                            phase = !phase;
                        }

                        stack.Clear();
                        break;

                    case Type2Operator1.Escape:

                        bool a;
                        bool b;
                        var twoByteOperator = reader.ReadByte();
                        if (twoByteOperator >= 38)
                        {
                            ThrowInvalidOperator(twoByteOperator);
                            return;
                        }

                        switch ((Type2Operator2)twoByteOperator)
                        {
                            case Type2Operator2.And:

                                a = stack.Pop() != 0;
                                b = stack.Pop() != 0;
                                stack.Push(a && b ? 1 : 0);
                                break;

                            case Type2Operator2.Or:

                                a = stack.Pop() != 0;
                                b = stack.Pop() != 0;
                                stack.Push(a || b ? 1 : 0);
                                break;

                            case Type2Operator2.Not:

                                a = stack.Pop() != 0;
                                stack.Push(a ? 1 : 0);
                                break;

                            case Type2Operator2.Abs:

                                stack.Push(Math.Abs(stack.Pop()));
                                break;

                            case Type2Operator2.Add:

                                stack.Push(stack.Pop() + stack.Pop());
                                break;

                            case Type2Operator2.Sub:

                                stack.Push(stack.Pop() - stack.Pop());
                                break;

                            case Type2Operator2.Div:

                                stack.Push(stack.Pop() / stack.Pop());
                                break;

                            case Type2Operator2.Neg:

                                stack.Push(-stack.Pop());
                                break;

                            case Type2Operator2.Eq:

                                stack.Push(stack.Pop() == stack.Pop() ? 1 : 0);
                                break;

                            case Type2Operator2.Drop:

                                stack.Pop();
                                break;

                            case Type2Operator2.Put:

                                var val = stack.Pop();
                                var idx = (int)stack.Pop();

                                trans[idx] = val;
                                break;

                            case Type2Operator2.Get:

                                idx = (int)stack.Pop();
                                trans.TryGetValue(idx, out var v);
                                stack.Push(v);
                                trans.Remove(idx);
                                break;

                            case Type2Operator2.Ifelse:

                                var s1 = stack.Pop();
                                var s2 = stack.Pop();
                                var v1 = stack.Pop();
                                var v2 = stack.Pop();

                                stack.Push(v1 <= v2 ? s1 : s2);
                                break;

                            case Type2Operator2.Random:
                                stack.Push((float)Random.NextDouble());
                                break;

                            case Type2Operator2.Mul:

                                stack.Push(stack.Pop() * stack.Pop());
                                break;

                            case Type2Operator2.Sqrt:

                                stack.Push(MathF.Sqrt(stack.Pop()));
                                break;

                            case Type2Operator2.Dup:

                                var m = stack.Pop();
                                stack.Push(m);
                                stack.Push(m);
                                break;

                            case Type2Operator2.Exch:

                                var ex = stack.Pop();
                                var ch = stack.Pop();
                                stack.Push(ch);
                                stack.Push(ex);
                                break;

                            case Type2Operator2.Index:

                                idx = (int)stack.Pop();
                                if (idx < 0)
                                    idx = 0;
                                else if (idx > stack.Length - 1) idx = stack.Length - 1;

                                stack.Push(stack[idx]);
                                break;

                            case Type2Operator2.Roll:

                                var n = (int)stack.Pop();
                                var j = stack.Pop();

                                if (j >= 0)
                                    while (j > 0)
                                    {
                                        var t = stack[n - 1];
                                        for (var i = n - 2; i >= 0; i--) stack[i + 1] = stack[i];

                                        stack[0] = t;
                                        j--;
                                    }
                                else
                                    while (j < 0)
                                    {
                                        var t = stack[0];
                                        for (var i = 0; i <= n; i++) stack[i] = stack[i + 1];

                                        stack[n - 1] = t;
                                        j++;
                                    }

                                break;

                            case Type2Operator2.Hflex:

                                c1x = x + stack.Shift();
                                c1y = y;
                                c2x = c1x + stack.Shift();
                                c2y = c1y + stack.Shift();
                                var c3x = c2x + stack.Shift();
                                var c3y = c2y;
                                var c4x = c3x + stack.Shift();
                                var c4y = c3y;
                                var c5x = c4x + stack.Shift();
                                var c5y = c4y;
                                var c6x = c5x + stack.Shift();
                                var c6y = c5y;
                                x = c6x;
                                y = c6y;

                                transforming.CubicBezierTo(new Vector2(c1x, c1y), new Vector2(c2x, c2y),
                                    new Vector2(c3x, c3y));
                                transforming.CubicBezierTo(new Vector2(c4x, c4y), new Vector2(c5x, c5y),
                                    new Vector2(c6x, c6y));

                                stack.Clear();
                                break;

                            case Type2Operator2.Flex:

                                transforming.CubicBezierTo(new Vector2(stack.Shift(), stack.Shift()),
                                    new Vector2(stack.Shift(), stack.Shift()),
                                    new Vector2(stack.Shift(), stack.Shift()));
                                transforming.CubicBezierTo(new Vector2(stack.Shift(), stack.Shift()),
                                    new Vector2(stack.Shift(), stack.Shift()),
                                    new Vector2(stack.Shift(), stack.Shift()));

                                stack.Shift();

                                stack.Clear();
                                break;

                            case Type2Operator2.Hflex1:

                                c1x = x + stack.Shift();
                                c1y = y + stack.Shift();
                                c2x = c1x + stack.Shift();
                                c2y = c1y + stack.Shift();
                                c3x = c2x + stack.Shift();
                                c3y = c2y;
                                c4x = c3x + stack.Shift();
                                c4y = c3y;
                                c5x = c4x + stack.Shift();
                                c5y = c4y + stack.Shift();
                                c6x = c5x + stack.Shift();
                                c6y = c5y;
                                x = c6x;
                                y = c6y;

                                transforming.CubicBezierTo(new Vector2(c1x, c1y), new Vector2(c2x, c2y),
                                    new Vector2(c3x, c3y));
                                transforming.CubicBezierTo(new Vector2(c4x, c4y), new Vector2(c5x, c5y),
                                    new Vector2(c6x, c6y));

                                stack.Clear();
                                break;

                            case Type2Operator2.Flex1:

                                var startX = x;
                                var startY = y;

                                c1x = x + stack.Shift();
                                c1y = y + stack.Shift();

                                c2x = c1x + stack.Shift();
                                c2y = c1y + stack.Shift();

                                c3x = c2x + stack.Shift();
                                c3y = c2y + stack.Shift();

                                c4x = c3x + stack.Shift();
                                c4y = c3y + stack.Shift();

                                c5x = c4x + stack.Shift();
                                c5y = c4y + stack.Shift();

                                if (MathF.Abs(x - startX) > Math.Abs(y - startY))
                                {
                                    // horizontal
                                    c6x = c5x + stack.Shift();
                                    c6y = startY;
                                }
                                else
                                {
                                    c6x = startX;
                                    c6y = c5y + stack.Shift();
                                }

                                x = c6x;
                                y = c6y;

                                transforming.CubicBezierTo(new Vector2(c1x, c1y), new Vector2(c2x, c2y),
                                    new Vector2(c3x, c3y));
                                transforming.CubicBezierTo(new Vector2(c4x, c4y), new Vector2(c5x, c5y),
                                    new Vector2(c6x, c6y));

                                stack.Clear();
                                break;
                        }

                        break;
                }
            }
            else if (b0 < 247)
            {
                stack.Push(b0 - 139);
            }
            else if (b0 < 251)
            {
                var b1 = reader.ReadByte();
                stack.Push((b0 - 247) * 256 + b1 + 108);
            }
            else if (b0 < 255)
            {
                var b1 = reader.ReadByte();
                stack.Push(-(b0 - 251) * 256 - b1 - 108);
            }
            else
            {
                stack.Push(reader.ReadFloatFixed1616());
            }
        }
    }

    public void Dispose()
    {
        if (isDisposed) return;

        stack.Dispose();
        isDisposed = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateBias(int count)
    {
        if (count == 0) return 0;

        return count < 1240 ? 107 : count < 33900 ? 1131 : 32768;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ParseStems()
    {
        if (stack.Length % 2 != 0) CheckWidth();

        nStems += stack.Length >> 1;
        stack.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckWidth()
    {
        if (width == null) width = stack.Shift() + nominalWidthX;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Reset()
    {
        x = 0;
        y = 0;
        width = null;
        nStems = 0;
        stack.Clear();
        trans.Clear();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowInvalidOperator(byte @operator)
    {
        throw new InvalidFontFileException($"Unknown operator:{@operator}");
    }
}