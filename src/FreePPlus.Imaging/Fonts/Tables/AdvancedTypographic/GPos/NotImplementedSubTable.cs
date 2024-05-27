// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.GPos;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.GPos;

internal class NotImplementedSubTable : LookupSubTable
{
    public NotImplementedSubTable()
        : base(default) { }

    public override bool TryUpdatePosition(
        FontMetrics fontMetrics,
        GPosTable table,
        GlyphPositioningCollection collection,
        Tag feature,
        int index,
        int count)
    {
        return false;
    }
}