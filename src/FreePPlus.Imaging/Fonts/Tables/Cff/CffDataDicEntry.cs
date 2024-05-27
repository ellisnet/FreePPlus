// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Text;

namespace FreePPlus.Imaging.Fonts.Tables.Cff;

//was previously: namespace SixLabors.Fonts.Tables.Cff;

internal class CffDataDicEntry
{
    public CffDataDicEntry(CFFOperator @operator, CffOperand[] operands)
    {
        Operator = @operator;
        Operands = operands;
    }

    public CFFOperator Operator { get; }

    public CffOperand[] Operands { get; }

#if DEBUG
    public override string ToString()
    {
        StringBuilder builder = new();
        var j = Operands.Length;
        for (var i = 0; i < j; ++i)
        {
            if (i > 0) builder.Append(" ");

            builder.Append(Operands[i].ToString());
        }

        builder.Append(" ");
        builder.Append(Operator?.ToString() ?? string.Empty);
        return builder.ToString();
    }
#endif
}