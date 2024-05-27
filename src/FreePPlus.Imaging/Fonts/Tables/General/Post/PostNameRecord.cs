// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.General.Post;

//was previously: namespace SixLabors.Fonts.Tables.General.Post;

internal class PostNameRecord
{
    internal PostNameRecord(ushort nameIndex, string name)
    {
        Name = name;
        NameIndex = nameIndex;
    }

    public ushort NameIndex { get; }

    public string Name { get; }
}