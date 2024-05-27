// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using FreePPlus.Imaging.Fonts.Unicode;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.Shapers;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.Shapers;

/// <summary>
///     This is a shaper for the Hangul script, used by the Korean language.
///     The shaping state machine was ported from fontkit.
///     <see href="https://github.com/foliojs/fontkit/blob/master/src/opentype/shapers/HangulShaper.js" />
/// </summary>
internal sealed class HangulShaper : DefaultShaper
{
    private const int HangulBase = 0xac00;
    private const int LBase = 0x1100; // lead
    private const int VBase = 0x1161; // vowel
    private const int TBase = 0x11a7; // trail
    private const int LCount = 19;
    private const int VCount = 21;
    private const int TCount = 28;
    private const int LEnd = LBase + LCount - 1;
    private const int VEnd = VBase + VCount - 1;
    private const int TEnd = TBase + TCount - 1;
    private const int DottedCircle = 0x25cc;

    // Character categories
    private const byte X = 0; // Other character
    private const byte L = 1; // Leading consonant
    private const byte V = 2; // Medial vowel
    private const byte T = 3; // Trailing consonant
    private const byte LV = 4; // Composed <LV> syllable
    private const byte LVT = 5; // Composed <LVT> syllable
    private const byte M = 6; // Tone mark

    // State machine actions
    private const byte None = 0;
    private const byte Decompose = 1;
    private const byte Compose = 2;
    private const byte ToneMark = 4;
    private const byte Invalid = 5;
    private static readonly Tag LjmoTag = Tag.Parse("ljmo");

    private static readonly Tag VjmoTag = Tag.Parse("vjmo");

    private static readonly Tag TjmoTag = Tag.Parse("tjmo");

    // Build a state machine that accepts valid syllables, and applies actions along the way.
    // The logic this is implementing is documented at the top of the file.
    private static readonly byte[,][] StateTable =
    {
        // #             X                       L                       V                       T                       LV                           LVT                          M
        // State 0: start state
        {
            new byte[] { None, 0 }, new byte[] { None, 1 }, new byte[] { None, 0 }, new byte[] { None, 0 },
            new byte[] { Decompose, 2 }, new byte[] { Decompose, 3 }, new byte[] { Invalid, 0 }
        },

        // State 1: <L>
        {
            new byte[] { None, 0 }, new byte[] { None, 1 }, new byte[] { Compose, 2 }, new byte[] { None, 0 },
            new byte[] { Decompose, 2 }, new byte[] { Decompose, 3 }, new byte[] { Invalid, 0 }
        },

        // State 2: <L,V> or <LV>
        {
            new byte[] { None, 0 }, new byte[] { None, 1 }, new byte[] { None, 0 }, new byte[] { Compose, 3 },
            new byte[] { Decompose, 2 }, new byte[] { Decompose, 3 }, new byte[] { ToneMark, 0 }
        },

        // State 3: <L,V,T> or <LVT>
        {
            new byte[] { None, 0 }, new byte[] { None, 1 }, new byte[] { None, 0 }, new byte[] { None, 0 },
            new byte[] { Decompose, 2 }, new byte[] { Decompose, 3 }, new byte[] { ToneMark, 0 }
        }
    };

    public HangulShaper(TextOptions textOptions)
        : base(MarkZeroingMode.None, textOptions) { }

    public override void AssignFeatures(IGlyphShapingCollection collection, int index, int count)
    {
        AddFeature(collection, index, count, LjmoTag, false);
        AddFeature(collection, index, count, VjmoTag, false);
        AddFeature(collection, index, count, TjmoTag, false);

        base.AssignFeatures(collection, index, count);

        for (var i = index; i < count; i++)
            // Uniscribe does not apply 'calt' for Hangul, and certain fonts
            // (Noto Sans CJK, Source Sans Han, etc) apply all of jamo lookups
            // in calt, which is not desirable.
            collection.DisableShapingFeature(i, CaltTag);

        // Apply the state machine to map glyphs to features.
        if (collection is GlyphSubstitutionCollection substitutionCollection)
        {
            // GSub
            var state = 0;
            for (var i = 0; i < count; i++)
            {
                if (i + index >= substitutionCollection.Count) break;

                var data = substitutionCollection.GetGlyphShapingData(i + index);
                var codePoint = data.CodePoint;
                var type = GetSyllableType(codePoint);
                var actionsWithState = StateTable[state, type];
                var action = actionsWithState[0];
                state = actionsWithState[1];
                switch (action)
                {
                    case Decompose:

                        // Decompose the composed syllable if it is not supported by the font.
                        if (data.GlyphId == 0) i = DecomposeGlyph(substitutionCollection, data, i);

                        break;

                    case Compose:

                        // Found a decomposed syllable. Try to compose if supported by the font.
                        i = ComposeGlyph(substitutionCollection, data, i, type);
                        break;

                    case ToneMark:

                        // Got a valid syllable, followed by a tone mark. Move the tone mark to the beginning of the syllable.
                        ReOrderToneMark(substitutionCollection, data, i);
                        break;

                    case Invalid:

                        // Tone mark has no valid syllable to attach to, so insert a dotted circle.
                        i = InsertDottedCircle(substitutionCollection, data, i);
                        break;
                }
            }
        }
        else
        {
            // GPos
            // Simply loop and enable based on type.
            // Glyph substitution has handled [de]composition.
            for (var i = 0; i < count; i++)
            {
                if (i + index >= collection.Count) break;

                var data = collection.GetGlyphShapingData(i + index);
                var codePoint = data.CodePoint;
                switch (GetSyllableType(codePoint))
                {
                    case L:
                        collection.EnableShapingFeature(i, LjmoTag);
                        break;
                    case V:
                        collection.EnableShapingFeature(i, VjmoTag);
                        break;
                    case T:
                        collection.EnableShapingFeature(i, TjmoTag);
                        break;
                    case LV:
                        collection.EnableShapingFeature(i, LjmoTag);
                        collection.EnableShapingFeature(i, VjmoTag);
                        break;
                    case LVT:
                        collection.EnableShapingFeature(i, LjmoTag);
                        collection.EnableShapingFeature(i, VjmoTag);
                        collection.EnableShapingFeature(i, TjmoTag);
                        break;
                }
            }
        }
    }

    private static int GetSyllableType(CodePoint codePoint)
    {
        var type = CodePoint.GetGraphemeClusterClass(codePoint);
        var value = codePoint.Value;

        return type switch
        {
            GraphemeClusterClass.HangulLead => L,
            GraphemeClusterClass.HangulVowel => V,
            GraphemeClusterClass.HangulTail => T,
            GraphemeClusterClass.HangulLeadVowel => LV,
            GraphemeClusterClass.HangulLeadVowelTail => LVT,

            // HANGUL SINGLE DOT TONE MARK
            // HANGUL DOUBLE DOT TONE MARK
            _ => value is >= 0x302E and <= 0x302F ? M : X
        };
    }

    private static int GetSyllableLength(CodePoint codePoint)
    {
        return GetSyllableType(codePoint) switch
        {
            LV or LVT => 1,
            V => 2,
            T => 3,
            _ => 0
        };
    }

    private int DecomposeGlyph(GlyphSubstitutionCollection collection, GlyphShapingData data, int index)
    {
        // Decompose the syllable into a sequence of glyphs.
        var s = data.CodePoint.Value - HangulBase;
        var t = TBase + s % TCount;
        s = (s / TCount) | 0;
        var l = (LBase + s / VCount) | 0;
        var v = VBase + s % VCount;

        var metrics = data.TextRun.Font!.FontMetrics;

        // Don't decompose if all of the components are not available
        if (!metrics.TryGetGlyphId(new CodePoint(l), out var ljmo) ||
            !metrics.TryGetGlyphId(new CodePoint(v), out var vjmo) ||
            (!metrics.TryGetGlyphId(new CodePoint(t), out var tjmo) && t != TBase))
            return index;

        // Replace the current glyph with decomposed L, V, and T glyphs,
        // and apply the proper OpenType features to each component.
        if (t <= TBase)
        {
            Span<ushort> ii = stackalloc ushort[2];
            ii[0] = ljmo;
            ii[1] = vjmo;

            collection.Replace(index, ii);
            collection.EnableShapingFeature(index, LjmoTag);
            collection.EnableShapingFeature(index + 1, VjmoTag);
            return index + 1;
        }

        Span<ushort> iii = stackalloc ushort[3];
        iii[0] = ljmo;
        iii[1] = vjmo;
        iii[2] = tjmo;

        collection.Replace(index, iii);
        collection.EnableShapingFeature(index, LjmoTag);
        collection.EnableShapingFeature(index + 1, VjmoTag);
        collection.EnableShapingFeature(index + 2, TjmoTag);
        return index + 2;
    }

    private int ComposeGlyph(GlyphSubstitutionCollection collection, GlyphShapingData data, int index, int type)
    {
        if (index == 0) return index;

        var prev = collection.GetGlyphShapingData(index - 1);
        var prevCodePoint = prev.CodePoint;
        var prevType = GetSyllableType(prevCodePoint);

        // Figure out what type of syllable we're dealing with
        CodePoint lv = default;
        int ljmo = -1, vjmo = -1, tjmo = -1;

        if (prevType == LV && type == T)
        {
            // <LV,T>
            lv = prevCodePoint;
            tjmo = index;
        }
        else
        {
            if (type == V)
            {
                // <L,V>
                ljmo = index - 1;
                vjmo = index;
            }
            else
            {
                // <L,V,T>
                ljmo = index - 2;
                vjmo = index - 1;
                tjmo = index;
            }

            var l = collection.GetGlyphShapingData(ljmo).CodePoint;
            var v = collection.GetGlyphShapingData(vjmo).CodePoint;

            // Make sure L and V are combining characters
            if (IsCombiningL(l) && IsCombiningV(v))
                lv = new CodePoint(HangulBase + ((l.Value - LBase) * VCount + (v.Value - VBase)) * TCount);
        }

        var t = tjmo >= 0 ? collection.GetGlyphShapingData(tjmo).CodePoint : new CodePoint(TBase);
        if (lv != default && (t.Value == TBase || IsCombiningT(t)))
        {
            CodePoint s = new(lv.Value + (t.Value - TBase));

            // Replace with a composed glyph if supported by the font,
            // otherwise apply the proper OpenType features to each component.
            var metrics = data.TextRun.Font!.FontMetrics;
            if (metrics.TryGetGlyphId(s, out var id))
            {
                var del = prevType == V ? 3 : 2;
                var idx = index - del + 1;
                collection.Replace(idx, del - 1, id);
                collection.GetGlyphShapingData(idx).CodePoint = s;
                return idx;
            }
        }

        // Didn't compose (either a non-combining component or unsupported by font).
        if (ljmo >= 0) collection.EnableShapingFeature(ljmo, LjmoTag);

        if (vjmo >= 0) collection.EnableShapingFeature(vjmo, VjmoTag);

        if (tjmo >= 0) collection.EnableShapingFeature(tjmo, TjmoTag);

        if (prevType == LV)
        {
            // Sequence was originally <L,V>, which got combined earlier.
            // Either the T was non-combining, or the LVT glyph wasn't supported.
            // Decompose the glyph again and apply OT features.
            data = collection.GetGlyphShapingData(index - 1);
            DecomposeGlyph(collection, data, index - 1);
            return index + 1;
        }

        return index;
    }

    private void ReOrderToneMark(GlyphSubstitutionCollection collection, GlyphShapingData data, int index)
    {
        if (index == 0) return;

        // Move tone mark to the beginning of the previous syllable, unless it is zero width
        // We don't have access to the glyphs metrics as an array when substituting so we have to loop.
        var metrics = data.TextRun.Font!.FontMetrics;
        foreach (var gm in metrics.GetGlyphMetrics(data.CodePoint, collection.TextOptions.ColorFontSupport))
            if (gm.AdvanceWidth == 0)
                return;

        var prev = collection.GetGlyphShapingData(index - 1);
        var len = GetSyllableLength(prev.CodePoint);
        collection.MoveGlyph(index, index - len);
    }

    private int InsertDottedCircle(GlyphSubstitutionCollection collection, GlyphShapingData data, int index)
    {
        var after = false;
        var metrics = data.TextRun.Font!.FontMetrics;

        if (metrics.TryGetGlyphId(new CodePoint(DottedCircle), out var id))
        {
            foreach (var gm in metrics.GetGlyphMetrics(data.CodePoint, collection.TextOptions.ColorFontSupport))
                if (gm.AdvanceWidth != 0)
                {
                    after = true;
                    break;
                }

            // If the tone mark is zero width, insert the dotted circle before, otherwise after
            Span<ushort> glyphs = stackalloc ushort[2];
            if (after)
            {
                glyphs[0] = data.GlyphId;
                glyphs[1] = id;
            }
            else
            {
                glyphs[0] = id;
                glyphs[1] = data.GlyphId;
            }

            collection.Replace(index, glyphs);
            return index + 1;
        }

        return index;
    }

    private static bool IsCombiningL(CodePoint code)
    {
        return UnicodeUtility.IsInRangeInclusive((uint)code.Value, LBase, LEnd);
    }

    private static bool IsCombiningV(CodePoint code)
    {
        return UnicodeUtility.IsInRangeInclusive((uint)code.Value, VBase, VEnd);
    }

    private static bool IsCombiningT(CodePoint code)
    {
        return UnicodeUtility.IsInRangeInclusive((uint)code.Value, TBase + 1, TEnd);
    }
}