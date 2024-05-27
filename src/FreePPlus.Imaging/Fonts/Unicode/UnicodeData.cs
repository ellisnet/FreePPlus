// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using FreePPlus.Imaging.Fonts.Unicode.Resources;

namespace FreePPlus.Imaging.Fonts.Unicode;

//was previously: namespace SixLabors.Fonts.Unicode;

internal static class UnicodeData
{
    private static readonly Lazy<UnicodeTrie> LazyBidiTrie = new(() => GetBidiTrie());
    private static readonly Lazy<UnicodeTrie> LazyBidiMirrorTrie = new(() => GetBidiMirrorTrie());
    private static readonly Lazy<UnicodeTrie> LazyGraphemeTrie = new(() => GetGraphemeTrie());
    private static readonly Lazy<UnicodeTrie> LazyLinebreakTrie = new(() => GetLineBreakTrie());
    private static readonly Lazy<UnicodeTrie> LazyScriptTrie = new(() => GetScriptTrie());
    private static readonly Lazy<UnicodeTrie> LazyCategoryTrie = new(() => GetCategoryTrie());
    private static readonly Lazy<UnicodeTrie> LazyArabicShapingTrie = new(() => GetArabicShapingTrie());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetBidiData(uint codePoint)
    {
        return LazyBidiTrie.Value.Get(codePoint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetBidiMirror(uint codePoint)
    {
        return LazyBidiMirrorTrie.Value.Get(codePoint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GraphemeClusterClass GetGraphemeClusterClass(uint codePoint)
    {
        return (GraphemeClusterClass)LazyGraphemeTrie.Value.Get(codePoint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LineBreakClass GetLineBreakClass(uint codePoint)
    {
        return (LineBreakClass)LazyLinebreakTrie.Value.Get(codePoint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScriptClass GetScriptClass(uint codePoint)
    {
        return (ScriptClass)LazyScriptTrie.Value.Get(codePoint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetJoiningClass(uint codePoint)
    {
        return LazyArabicShapingTrie.Value.Get(codePoint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UnicodeCategory GetUnicodeCategory(uint codePoint)
    {
        return (UnicodeCategory)LazyCategoryTrie.Value.Get(codePoint);
    }

    private static UnicodeTrie GetBidiTrie()
    {
        return new UnicodeTrie(BidiTrie.Data);
    }

    private static UnicodeTrie GetBidiMirrorTrie()
    {
        return new UnicodeTrie(BidiMirrorTrie.Data);
    }

    private static UnicodeTrie GetGraphemeTrie()
    {
        return new UnicodeTrie(GraphemeTrie.Data);
    }

    private static UnicodeTrie GetLineBreakTrie()
    {
        return new UnicodeTrie(LineBreakTrie.Data);
    }

    private static UnicodeTrie GetScriptTrie()
    {
        return new UnicodeTrie(ScriptTrie.Data);
    }

    private static UnicodeTrie GetCategoryTrie()
    {
        return new UnicodeTrie(UnicodeCategoryTrie.Data);
    }

    private static UnicodeTrie GetArabicShapingTrie()
    {
        return new UnicodeTrie(ArabicShapingTrie.Data);
    }
}