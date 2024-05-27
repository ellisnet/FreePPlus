// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal struct FDRangeProvider
{
    // helper class
    private readonly int format;
    private readonly FDRange3[] ranges;
    private readonly Dictionary<int, byte> fdSelectMap;
    private ushort currentGlyphIndex;
    private ushort endGlyphIndexMax;
    private FDRange3 currentRange;
    private int currentSelectedRangeIndex;

    public FDRangeProvider(CidFontInfo cidFontInfo)
    {
        format = cidFontInfo.FdSelectFormat;
        ranges = cidFontInfo.FdRanges;
        fdSelectMap = cidFontInfo.FdSelectMap;
        currentGlyphIndex = 0;
        currentSelectedRangeIndex = 0;

        if (ranges.Length is not 0)
        {
            currentRange = ranges[0];
            endGlyphIndexMax = ranges[1].First;
        }
        else
        {
            // empty
            currentRange = default;
            endGlyphIndexMax = 0;
        }

        SelectedFDArray = 0;
    }

    public byte SelectedFDArray { get; private set; }

    public void SetCurrentGlyphIndex(ushort index)
    {
        switch (format)
        {
            case 0:
                currentGlyphIndex = fdSelectMap[index];
                break;

            case 3:
                // Find proper range for selected index.
                if (index >= currentRange.First && index < endGlyphIndexMax)
                {
                    // Ok, in current range.
                    SelectedFDArray = currentRange.FontDictionary;
                }
                else
                {
                    // Move to next range.
                    currentSelectedRangeIndex++;
                    currentRange = ranges[currentSelectedRangeIndex];

                    endGlyphIndexMax = ranges[currentSelectedRangeIndex + 1].First;
                    if (index >= currentRange.First && index < endGlyphIndexMax)
                        SelectedFDArray = currentRange.FontDictionary;
                    else
                        throw new NotSupportedException();
                }

                currentGlyphIndex = index;

                break;

            default:
                throw new NotSupportedException();
        }
    }
}