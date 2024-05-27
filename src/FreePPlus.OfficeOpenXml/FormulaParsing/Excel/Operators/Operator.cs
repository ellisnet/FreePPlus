﻿/*******************************************************************************
 * You may amend and distribute as you like, but don't remove this header!
 *
 * EPPlus provides server-side generation of Excel 2007/2010 spreadsheets.
 * See https://github.com/JanKallman/EPPlus for details.
 *
 * Copyright (C) 2011  Jan Källman
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.

 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Lesser General Public License for more details.
 *
 * The GNU Lesser General Public License can be viewed at http://www.opensource.org/licenses/lgpl-license.php
 * If you unfamiliar with this license or have questions about it, here is an http://www.gnu.org/licenses/gpl-faq.html
 *
 * All code and executables are provided "as is" with no warranty either express or implied.
 * The author accepts no liability for any damage or loss of business that this product may cause.
 *
 * Code change notes:
 *
 * Author							Change						Date
 * ******************************************************************************
 * Mats Alm   		                Added       		        2013-03-01 (Prior file history on https://github.com/swmal/ExcelFormulaParser)
 *******************************************************************************/

using System;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using OfficeOpenXml.Utils;

namespace OfficeOpenXml.FormulaParsing.Excel.Operators;

public class Operator : IOperator
{
    private const int PrecedencePercent = 2;
    private const int PrecedenceExp = 4;
    private const int PrecedenceMultiplyDevide = 6;
    private const int PrecedenceIntegerDivision = 8;
    private const int PrecedenceModulus = 10;
    private const int PrecedenceAddSubtract = 12;
    private const int PrecedenceConcat = 15;
    private const int PrecedenceComparison = 25;

    private static IOperator _plus;

    private static IOperator _minus;

    private static IOperator _multiply;

    private static IOperator _divide;

    private static IOperator _greaterThan;

    private static IOperator _eq;

    private static IOperator _notEqualsTo;

    private static IOperator _greaterThanOrEqual;

    private static IOperator _lessThan;

    private static IOperator _percent;

    private readonly Func<CompileResult, CompileResult, CompileResult> _implementation;
    private readonly Operators _operator;
    private readonly int _precedence;

    private Operator() { }

    private Operator(Operators @operator, int precedence,
        Func<CompileResult, CompileResult, CompileResult> implementation)
    {
        _implementation = implementation;
        _precedence = precedence;
        _operator = @operator;
    }

    public static IOperator Plus
    {
        get
        {
            return _plus ?? (_plus = new Operator(Operators.Plus, PrecedenceAddSubtract, (l, r) =>
            {
                l = l == null || l.Result == null ? new CompileResult(0, DataType.Integer) : l;
                r = r == null || r.Result == null ? new CompileResult(0, DataType.Integer) : r;
                ExcelErrorValue errorVal;
                if (EitherIsError(l, r, out errorVal)) return new CompileResult(errorVal);
                if (l.DataType == DataType.Integer && r.DataType == DataType.Integer)
                    return new CompileResult(l.ResultNumeric + r.ResultNumeric, DataType.Integer);
                if ((l.IsNumeric || l.IsNumericString || l.IsDateString || l.Result is ExcelDataProvider.IRangeInfo) &&
                    (r.IsNumeric || r.IsNumericString || r.IsDateString || r.Result is ExcelDataProvider.IRangeInfo))
                    return new CompileResult(l.ResultNumeric + r.ResultNumeric, DataType.Decimal);
                return new CompileResult(eErrorType.Value);
            }));
        }
    }

    public static IOperator Minus
    {
        get
        {
            return _minus ?? (_minus = new Operator(Operators.Minus, PrecedenceAddSubtract, (l, r) =>
            {
                l = l == null || l.Result == null ? new CompileResult(0, DataType.Integer) : l;
                r = r == null || r.Result == null ? new CompileResult(0, DataType.Integer) : r;
                if (l.DataType == DataType.Integer && r.DataType == DataType.Integer)
                    return new CompileResult(l.ResultNumeric - r.ResultNumeric, DataType.Integer);
                if ((l.IsNumeric || l.IsNumericString || l.IsDateString || l.Result is ExcelDataProvider.IRangeInfo) &&
                    (r.IsNumeric || r.IsNumericString || r.IsDateString || r.Result is ExcelDataProvider.IRangeInfo))
                    return new CompileResult(l.ResultNumeric - r.ResultNumeric, DataType.Decimal);

                return new CompileResult(eErrorType.Value);
            }));
        }
    }

    public static IOperator Multiply
    {
        get
        {
            return _multiply ?? (_multiply = new Operator(Operators.Multiply, PrecedenceMultiplyDevide, (l, r) =>
            {
                l = l ?? new CompileResult(0, DataType.Integer);
                r = r ?? new CompileResult(0, DataType.Integer);
                if (l.DataType == DataType.Integer && r.DataType == DataType.Integer)
                    return new CompileResult(l.ResultNumeric * r.ResultNumeric, DataType.Integer);
                if ((l.IsNumeric || l.IsNumericString || l.IsDateString || l.Result is ExcelDataProvider.IRangeInfo) &&
                    (r.IsNumeric || r.IsNumericString || r.IsDateString || r.Result is ExcelDataProvider.IRangeInfo))
                    return new CompileResult(l.ResultNumeric * r.ResultNumeric, DataType.Decimal);
                return new CompileResult(eErrorType.Value);
            }));
        }
    }

    public static IOperator Divide
    {
        get
        {
            return _divide ?? (_divide = new Operator(Operators.Divide, PrecedenceMultiplyDevide, (l, r) =>
            {
                if (!(l.IsNumeric || l.IsNumericString || l.IsDateString || l.Result is ExcelDataProvider.IRangeInfo) ||
                    !(r.IsNumeric || r.IsNumericString || r.IsDateString || r.Result is ExcelDataProvider.IRangeInfo))
                    return new CompileResult(eErrorType.Value);
                var left = l.ResultNumeric;
                var right = r.ResultNumeric;
                if (Math.Abs(right - 0d) < double.Epsilon)
                    return new CompileResult(eErrorType.Div0);
                if ((l.IsNumeric || l.IsNumericString || l.IsDateString || l.Result is ExcelDataProvider.IRangeInfo) &&
                    (r.IsNumeric || r.IsNumericString || r.IsDateString || r.Result is ExcelDataProvider.IRangeInfo))
                    return new CompileResult(left / right, DataType.Decimal);
                return new CompileResult(eErrorType.Value);
            }));
        }
    }

    public static IOperator Exp
    {
        get
        {
            return new Operator(Operators.Exponentiation, PrecedenceExp, (l, r) =>
            {
                if (l == null && r == null) return new CompileResult(eErrorType.Value);
                l = l ?? new CompileResult(0, DataType.Integer);
                r = r ?? new CompileResult(0, DataType.Integer);
                if ((l.IsNumeric || l.IsNumericString || l.IsDateString || l.Result is ExcelDataProvider.IRangeInfo) &&
                    (r.IsNumeric || r.IsNumericString || r.IsDateString || r.Result is ExcelDataProvider.IRangeInfo))
                    return new CompileResult(Math.Pow(l.ResultNumeric, r.ResultNumeric), DataType.Decimal);
                return new CompileResult(0d, DataType.Decimal);
            });
        }
    }

    public static IOperator Concat
    {
        get
        {
            return new Operator(Operators.Concat, PrecedenceConcat, (l, r) =>
            {
                l = l ?? new CompileResult(string.Empty, DataType.String);
                r = r ?? new CompileResult(string.Empty, DataType.String);
                var lStr = l.Result != null ? l.ResultValue.ToString() : string.Empty;
                var rStr = r.Result != null ? r.ResultValue.ToString() : string.Empty;
                return new CompileResult(string.Concat(lStr, rStr), DataType.String);
            });
        }
    }

    public static IOperator GreaterThan
    {
        get
        {
            return _greaterThan ??
                   (_greaterThan =
                       new Operator(Operators.GreaterThan, PrecedenceComparison,
                           (l, r) => Compare(l, r, compRes => compRes > 0)));
        }
    }

    public static IOperator Eq
    {
        get
        {
            return _eq ??
                   (_eq =
                       new Operator(Operators.Equals, PrecedenceComparison,
                           (l, r) => Compare(l, r, compRes => compRes == 0)));
        }
    }

    public static IOperator NotEqualsTo
    {
        get
        {
            return _notEqualsTo ??
                   (_notEqualsTo =
                       new Operator(Operators.NotEqualTo, PrecedenceComparison,
                           (l, r) => Compare(l, r, compRes => compRes != 0)));
        }
    }

    public static IOperator GreaterThanOrEqual
    {
        get
        {
            return _greaterThanOrEqual ??
                   (_greaterThanOrEqual =
                       new Operator(Operators.GreaterThanOrEqual, PrecedenceComparison,
                           (l, r) => Compare(l, r, compRes => compRes >= 0)));
        }
    }

    public static IOperator LessThan
    {
        get
        {
            return _lessThan ??
                   (_lessThan =
                       new Operator(Operators.LessThan, PrecedenceComparison,
                           (l, r) => Compare(l, r, compRes => compRes < 0)));
        }
    }

    public static IOperator LessThanOrEqual
    {
        get
        {
            //return new Operator(Operators.LessThanOrEqual, PrecedenceComparison, (l, r) => new CompileResult(Compare(l, r) <= 0, DataType.Boolean));
            return new Operator(Operators.LessThanOrEqual, PrecedenceComparison,
                (l, r) => Compare(l, r, compRes => compRes <= 0));
        }
    }

    public static IOperator Percent
    {
        get
        {
            if (_percent == null)
                _percent = new Operator(Operators.Percent, PrecedencePercent, (l, r) =>
                {
                    l = l ?? new CompileResult(0, DataType.Integer);
                    r = r ?? new CompileResult(0, DataType.Integer);
                    if (l.DataType == DataType.Integer && r.DataType == DataType.Integer)
                        return new CompileResult(l.ResultNumeric * r.ResultNumeric, DataType.Integer);
                    if ((l.IsNumeric || l.IsNumericString || l.IsDateString ||
                         l.Result is ExcelDataProvider.IRangeInfo) &&
                        (r.IsNumeric || r.IsNumericString || r.IsDateString ||
                         r.Result is ExcelDataProvider.IRangeInfo))
                        return new CompileResult(l.ResultNumeric * r.ResultNumeric, DataType.Decimal);
                    return new CompileResult(eErrorType.Value);
                });
            return _percent;
        }
    }

    int IOperator.Precedence => _precedence;

    Operators IOperator.Operator => _operator;

    public CompileResult Apply(CompileResult left, CompileResult right)
    {
        if (left.Result is ExcelErrorValue)
            return new CompileResult(left.Result, DataType.ExcelError);
        //throw(new ExcelErrorValueException((ExcelErrorValue)left.Result));
        if (right.Result is ExcelErrorValue) return new CompileResult(right.Result, DataType.ExcelError);
        //throw(new ExcelErrorValueException((ExcelErrorValue)right.Result));
        return _implementation(left, right);
    }

    public override string ToString()
    {
        return "Operator: " + _operator;
    }

    private static object GetObjFromOther(CompileResult obj, CompileResult other)
    {
        if (obj.Result == null)
        {
            if (other.DataType == DataType.String) return string.Empty;
            return 0d;
        }

        return obj.ResultValue;
    }

    private static CompileResult Compare(CompileResult l, CompileResult r, Func<int, bool> comparison)
    {
        ExcelErrorValue errorVal;
        if (EitherIsError(l, r, out errorVal)) return new CompileResult(errorVal);
        object left, right;
        left = GetObjFromOther(l, r);
        right = GetObjFromOther(r, l);
        if (ConvertUtil.IsNumeric(left) && ConvertUtil.IsNumeric(right))
        {
            var lnum = ConvertUtil.GetValueDouble(left);
            var rnum = ConvertUtil.GetValueDouble(right);
            if (Math.Abs(lnum - rnum) < double.Epsilon) return new CompileResult(comparison(0), DataType.Boolean);
            var comparisonResult = lnum.CompareTo(rnum);
            return new CompileResult(comparison(comparisonResult), DataType.Boolean);
        }
        else
        {
            var comparisonResult = CompareString(left, right);
            return new CompileResult(comparison(comparisonResult), DataType.Boolean);
        }
    }

    private static int CompareString(object l, object r)
    {
        var sl = (l ?? "").ToString();
        var sr = (r ?? "").ToString();
        return string.Compare(sl, sr, StringComparison.OrdinalIgnoreCase);
    }

    private static bool EitherIsError(CompileResult l, CompileResult r, out ExcelErrorValue errorVal)
    {
        if (l.DataType == DataType.ExcelError)
        {
            errorVal = (ExcelErrorValue)l.Result;
            return true;
        }

        if (r.DataType == DataType.ExcelError)
        {
            errorVal = (ExcelErrorValue)r.Result;
            return true;
        }

        errorVal = null;
        return false;
    }
}