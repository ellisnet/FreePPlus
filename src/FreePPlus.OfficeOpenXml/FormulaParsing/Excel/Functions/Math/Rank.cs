﻿using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using OfficeOpenXml.Utils;

namespace OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

public class Rank : ExcelFunction
{
    private readonly bool _isAvg;

    public Rank(bool isAvg = false)
    {
        _isAvg = isAvg;
    }

    public override CompileResult Execute(IEnumerable<FunctionArgument> arguments, ParsingContext context)
    {
        ValidateArguments(arguments, 2);
        var number = ArgToDecimal(arguments, 0);
        var refer = arguments.ElementAt(1);
        var asc = false;
        if (arguments.Count() > 2) asc = ArgToBool(arguments, 2);
        var l = new List<double>();

        foreach (var c in refer.ValueAsRangeInfo)
        {
            var v = ConvertUtil.GetValueDouble(c.Value, false, true);
            if (!double.IsNaN(v)) l.Add(v);
        }

        l.Sort();
        double ix;
        if (asc)
        {
            ix = l.IndexOf(number) + 1;
            if (_isAvg)
            {
                var st = Convert.ToInt32(ix);
                while (l.Count > st && l[st] == number) st++;
                if (st > ix) ix = ix + (st - ix) / 2D;
            }
        }
        else
        {
            ix = l.LastIndexOf(number);
            if (_isAvg)
            {
                var st = Convert.ToInt32(ix) - 1;
                while (0 <= st && l[st] == number) st--;
                if (st + 1 < ix) ix = ix - (ix - st - 1) / 2D;
            }

            ix = l.Count - ix;
        }

        if (ix <= 0 || ix > l.Count)
            return new CompileResult(ExcelErrorValue.Create(eErrorType.NA), DataType.ExcelError);
        return CreateResult(ix, DataType.Decimal);
    }
}