// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal readonly struct CffOperand
{
    public CffOperand(double number, OperandKind kind)
    {
        Kind = kind;
        RealNumValue = number;
    }

    public readonly OperandKind Kind { get; }

    public readonly double RealNumValue { get; }

#if DEBUG
    public override string ToString()
    {
        return Kind switch
        {
            OperandKind.IntNumber => ((int)RealNumValue).ToString(),
            _ => RealNumValue.ToString()
        };
    }
#endif
}