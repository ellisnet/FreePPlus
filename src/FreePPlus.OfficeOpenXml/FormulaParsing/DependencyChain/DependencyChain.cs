using System.Collections.Generic;

namespace OfficeOpenXml.FormulaParsing;

internal class DependencyChain
{
    internal List<int> CalcOrder = new();
    internal Dictionary<ulong, int> index = new();
    internal List<FormulaCell> list = new();

    internal void Add(FormulaCell f)
    {
        list.Add(f);
        f.Index = list.Count - 1;
        index.Add(ExcelCellBase.GetCellID(f.SheetID, f.Row, f.Column), f.Index);
    }
}