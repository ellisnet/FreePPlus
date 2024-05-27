// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

internal struct SkippingGlyphIterator
{
    private readonly FontMetrics fontMetrics;
    private bool ignoreMarks;
    private bool ignoreBaseGlypghs;
    private bool ignoreLigatures;
    private ushort markAttachmentType;

    public SkippingGlyphIterator(FontMetrics fontMetrics, IGlyphShapingCollection collection, int index,
        LookupFlags lookupFlags)
    {
        this.fontMetrics = fontMetrics;
        Collection = collection;
        Index = index;
        ignoreMarks = (lookupFlags & LookupFlags.IgnoreMarks) != 0;
        ignoreBaseGlypghs = (lookupFlags & LookupFlags.IgnoreBaseGlypghs) != 0;
        ignoreLigatures = (lookupFlags & LookupFlags.IgnoreLigatures) != 0;
        markAttachmentType = 0; // TODO: Lookup HarfBuzz to see how this is assigned.
    }

    public IGlyphShapingCollection Collection { get; }

    public int Index { get; set; }

    public int Next()
    {
        Move(1);
        return Index;
    }

    public int Increment(int count = 1)
    {
        var direction = count < 0 ? -1 : 1;
        count = Math.Abs(count);
        while (count-- > 0) Move(direction);

        return Index;
    }

    public void Reset(int index, LookupFlags lookupFlags)
    {
        Index = index;
        ignoreMarks = (lookupFlags & LookupFlags.IgnoreMarks) != 0;
        ignoreBaseGlypghs = (lookupFlags & LookupFlags.IgnoreBaseGlypghs) != 0;
        ignoreLigatures = (lookupFlags & LookupFlags.IgnoreLigatures) != 0;
        markAttachmentType = 0; // TODO: Lookup HarfBuzz
    }

    private void Move(int direction)
    {
        Index += direction;
        while (Index >= 0 && Index < Collection.Count)
        {
            if (!ShouldIgnore(Index)) break;

            Index += direction;
        }
    }

    private bool ShouldIgnore(int index)
    {
        var data = Collection.GetGlyphShapingData(index);
        var shapingClass = AdvancedTypographicUtils.GetGlyphShapingClass(fontMetrics, data.GlyphId, data);
        return (ignoreMarks && shapingClass.IsMark) ||
               (ignoreBaseGlypghs && shapingClass.IsBase) ||
               (ignoreLigatures && shapingClass.IsLigature) ||
               (markAttachmentType > 0 && shapingClass.IsMark && shapingClass.MarkAttachmentType != markAttachmentType);
    }
}