// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Diagnostics;

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Represents the shaped bounds of a glyph.
///     Uses a class over a struct for ease of use.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal class GlyphShapingBounds
{
    private int height;
    private int width;
    private int x;
    private int y;

    public GlyphShapingBounds(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        IsDirtyXY = false;
        IsDirtyWH = false;
    }

    public int X
    {
        get => x;

        set
        {
            x = value;
            IsDirtyXY = true;
        }
    }

    public int Y
    {
        get => y;

        set
        {
            y = value;
            IsDirtyXY = true;
        }
    }

    public int Width
    {
        get => width;

        set
        {
            width = value;
            IsDirtyWH = true;
        }
    }

    public int Height
    {
        get => height;

        set
        {
            height = value;
            IsDirtyWH = true;
        }
    }

    public bool IsDirtyXY { get; private set; }

    public bool IsDirtyWH { get; private set; }

    private string DebuggerDisplay
        => FormattableString.Invariant($"{X} : {Y} : {Width} : {Height} : {IsDirtyXY} : {IsDirtyWH}");
}