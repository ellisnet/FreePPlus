/* Copyright (C) 2011  Jan Källman
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
 *******************************************************************************
 * Mats Alm   		                Added		                2013-12-03
 * Eric Beiler                      Enable Multiple Selections  2015-09-01
 *******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;

namespace OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

public class Choose : ExcelFunction
{
    public override CompileResult Execute(IEnumerable<FunctionArgument> arguments, ParsingContext context)
    {
        ValidateArguments(arguments, 2);
        var items = new List<object>();
        for (var x = 0; x < arguments.Count(); x++) items.Add(arguments.ElementAt(x).ValueFirst);

        var chooseIndeces = arguments.ElementAt(0).ValueFirst as IEnumerable<FunctionArgument>;
        if (chooseIndeces != null && chooseIndeces.Count() > 1)
        {
            var intParser = new IntArgumentParser();
            var values = chooseIndeces.Select(chosenIndex => items[(int)intParser.Parse(chosenIndex.ValueFirst)])
                .ToArray();
            return CreateResult(values, DataType.Enumerable);
        }

        var index = ArgToInt(arguments, 0);
        return CreateResult(items[index].ToString(), DataType.String);
    }
}

public class ChoosenInfo : ExcelDataProvider.IRangeInfo
{
    private readonly string[] chosenIndeces;

    public ChoosenInfo(string[] chosenIndeces)
    {
        this.chosenIndeces = chosenIndeces;
    }

    public bool IsEmpty => false;

    public bool IsMulti => true;

    public int GetNCells()
    {
        return 0;
    }

    public ExcelAddressBase Address => null;

    public object GetValue(int row, int col)
    {
        return null;
    }

    public object GetOffset(int rowOffset, int colOffset)
    {
        return null;
    }

    public ExcelWorksheet Worksheet => null;

    public ExcelDataProvider.ICellInfo Current => null;

    public void Dispose() { }

    object IEnumerator.Current => chosenIndeces[0];

    public bool MoveNext()
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public IEnumerator<ExcelDataProvider.ICellInfo> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}