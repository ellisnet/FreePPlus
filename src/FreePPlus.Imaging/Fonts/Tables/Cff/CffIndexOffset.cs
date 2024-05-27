// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal readonly struct CffIndexOffset
{
    /// <summary>
    ///     The starting offset
    /// </summary>
    public readonly int Start;

    /// <summary>
    ///     The length
    /// </summary>
    public readonly int Length;

    public CffIndexOffset(int start, int len)
    {
        Start = start;
        Length = len;
    }

#if DEBUG
    public override string ToString()
    {
        return "Start:" + Start + ",Length:" + Length;
    }
#endif
}