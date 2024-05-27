// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

internal readonly struct GlyphShapingClass
{
    public GlyphShapingClass(bool isMark, bool isBase, bool isLigature, ushort markAttachmentType)
    {
        IsMark = isMark;
        IsBase = isBase;
        IsLigature = isLigature;
        MarkAttachmentType = markAttachmentType;
    }

    public bool IsMark { get; }

    public bool IsBase { get; }

    public bool IsLigature { get; }

    public ushort MarkAttachmentType { get; }
}