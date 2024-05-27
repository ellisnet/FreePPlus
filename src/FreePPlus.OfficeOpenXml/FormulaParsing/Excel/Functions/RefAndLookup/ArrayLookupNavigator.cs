using System.Linq;
using OfficeOpenXml.FormulaParsing.Exceptions;
using OfficeOpenXml.FormulaParsing.Utilities;

namespace OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

public class ArrayLookupNavigator : LookupNavigator
{
    private readonly FunctionArgument[] _arrayData;
    private object _currentValue;
    private int _index;

    public ArrayLookupNavigator(LookupDirection direction, LookupArguments arguments, ParsingContext parsingContext)
        : base(direction, arguments, parsingContext)
    {
        Require.That(arguments).Named("arguments").IsNotNull();
        Require.That(arguments.DataArray).Named("arguments.DataArray").IsNotNull();
        _arrayData = arguments.DataArray.ToArray();
        Initialize();
    }

    public override int Index => _index;

    public override object CurrentValue => _arrayData[_index].Value;

    private void Initialize()
    {
        if (Arguments.LookupIndex >= _arrayData.Length) throw new ExcelErrorValueException(eErrorType.Ref);
        SetCurrentValue();
    }

    private void SetCurrentValue()
    {
        _currentValue = _arrayData[_index];
    }

    private bool HasNext()
    {
        if (Direction == LookupDirection.Vertical)
            return _index < _arrayData.Length - 1;
        return false;
    }

    public override bool MoveNext()
    {
        if (!HasNext()) return false;
        if (Direction == LookupDirection.Vertical) _index++;
        SetCurrentValue();
        return true;
    }

    public override object GetLookupValue()
    {
        return _arrayData[_index].Value;
    }
}