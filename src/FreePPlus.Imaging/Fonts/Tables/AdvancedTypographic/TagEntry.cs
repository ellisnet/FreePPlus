// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Diagnostics;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic;

[DebuggerDisplay("Tag: {Tag}, Enabled: {Enabled}")]
internal struct TagEntry
{
    public TagEntry(Tag tag, bool enabled)
    {
        Tag = tag;
        Enabled = enabled;
    }

    public bool Enabled { get; set; }

    public Tag Tag { get; }
}