using System;
using OfficeOpenXml;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

public class CalculationTests
{
    #region DataType Tests

    [Fact]
    public void CalculationTestDatatypes()
    {
        var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Calc1");

        ws.SetValue("A1", (short)1);
        ws.SetValue("A2", (long)2);
        ws.SetValue("A3", (float)3);
        ws.SetValue("A4", (double)4);
        ws.SetValue("A5", (decimal)5);
        ws.SetValue("A6", (byte)6);
        ws.SetValue("A7", null);
        ws.Cells["A10"].Formula = "Sum(A1:A8)";
        ws.Cells["A11"].Formula = "SubTotal(9,A1:A8)";
        ws.Cells["A12"].Formula = "Average(A1:A8)";

        ws.Calculate();

        Assert.Equal(21D, ws.Cells["a10"].Value);
        Assert.Equal(21D, ws.Cells["a11"].Value);
        Assert.Equal(21D / 6, ws.Cells["a12"].Value);
    }

    #endregion

    #region Formula Calculation Tests

    [Fact]
    public void CalculateTest()
    {
        var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Calc1");

        ws.SetValue("A1", (short)1);

        var v = ws.Calculate("2.5-A1+ABS(-3.0)-SIN(3)");
        Assert.Equal(4.3589, Math.Round((double)v, 4));

        ws.Row(1).Hidden = true;
        v = ws.Calculate("subtotal(109,a1:a10)");
        Assert.Equal(0D, v);

        v = ws.Calculate("-subtotal(9,a1:a3)");
        Assert.Equal(-1D, v);
    }

    [Fact]
    public void CalculateTestIsFunctions()
    {
        var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Calc1");

        ws.SetValue(1, 1, 1.0D);
        ws.SetFormula(1, 2, "isblank(A1:A5)");
        ws.SetFormula(1, 3, "concatenate(a1,a2,a3)");
        ws.SetFormula(1, 4, "Row()");
        ws.SetFormula(1, 5, "Row(a3)");

        ws.Calculate();

        // Verify formulas were calculated without error
        Assert.False(ws.Cells[1, 2].Value is ExcelErrorValue);
    }

    #endregion

    #region Named Range Calculation Tests

    [Fact]
    public void CalcTwiceError()
    {
        var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CalcTest");

        ws.Names.AddValue("PRICE", 10);
        ws.Names.AddValue("QUANTITY", 11);
        ws.Cells["A1"].Formula = "PRICE*QUANTITY";
        ws.Names.AddFormula("AMOUNT", "PRICE*QUANTITY");

        ws.Names["PRICE"].Value = 30;
        ws.Names["QUANTITY"].Value = 10;
        ws.Calculate();

        Assert.Equal(300D, ws.Cells["A1"].Value);
        Assert.Equal(300D, ws.Names["AMOUNT"].Value);

        ws.Names["PRICE"].Value = 40;
        ws.Names["QUANTITY"].Value = 20;
        ws.Calculate();

        Assert.Equal(800D, ws.Cells["A1"].Value);
        Assert.Equal(800D, ws.Names["AMOUNT"].Value);
    }

    #endregion

    #region String Function Tests

    [Fact]
    public void LeftRightFunctionTest()
    {
        var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CalcTest");

        ws.SetValue("A1", "asdf");
        ws.Cells["A2"].Formula = "Left(A1, 3)";
        ws.Cells["A3"].Formula = "Left(A1, 10)";
        ws.Cells["A4"].Formula = "Right(A1, 3)";
        ws.Cells["A5"].Formula = "Right(A1, 10)";

        ws.Calculate();

        Assert.Equal("asd", ws.Cells["A2"].Value);
        Assert.Equal("asdf", ws.Cells["A3"].Value);
        Assert.Equal("sdf", ws.Cells["A4"].Value);
        Assert.Equal("asdf", ws.Cells["A5"].Value);
    }

    #endregion

    #region IF Function Tests

    [Fact]
    public void IfFunctionTest()
    {
        var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CalcTest");

        ws.SetValue("A1", 123);
        ws.Cells["A2"].Formula = "IF(A1 = 123, 1, -1)";
        ws.Cells["A3"].Formula = "IF(A1 = 1, 1)";
        ws.Cells["A4"].Formula = "IF(A1 = 1, 1, -1)";
        ws.Cells["A5"].Formula = "IF(A1 = 123, 5)";

        ws.Calculate();

        Assert.Equal(1d, ws.Cells["A2"].Value);
        Assert.Equal(false, ws.Cells["A3"].Value);
        Assert.Equal(-1d, ws.Cells["A4"].Value);
        Assert.Equal(5d, ws.Cells["A5"].Value);
    }

    [Fact]
    public void IfError()
    {
        var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CalcTest");

        ws.Cells["A1"].Value = "test1";
        ws.Cells["A5"].Value = "test2";
        ws.Cells["A2"].Value = "Sant";
        ws.Cells["A3"].Value = "Falskt";
        ws.Cells["A4"].Formula = "if(A1>=A5,true,A3)";
        ws.Cells["B1"].Formula = "isText(a1)";
        ws.Cells["B2"].Formula = "isText(\"Test\")";
        ws.Cells["B3"].Formula = "isText(1)";
        ws.Cells["B4"].Formula = "isText(true)";
        ws.Cells["c1"].Formula = "mid(a1,4,15)";

        ws.Calculate();

        Assert.Equal(true, ws.Cells["B1"].Value);
        Assert.Equal(true, ws.Cells["B2"].Value);
        Assert.Equal(false, ws.Cells["B3"].Value);
        Assert.Equal(false, ws.Cells["B4"].Value);
        Assert.Equal("t1", ws.Cells["c1"].Value);
    }

    #endregion

    #region INT Function Tests

    [Fact]
    public void INTFunctionTest()
    {
        var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CalcTest");

        var currentDate = DateTime.UtcNow.Date;
        ws.SetValue("A1", currentDate.ToString("MM/dd/yyyy"));
        ws.SetValue("A2", currentDate.Date);
        ws.SetValue("A3", "31.1");
        ws.SetValue("A4", 31.1);
        ws.Cells["A5"].Formula = "INT(A1)";
        ws.Cells["A6"].Formula = "INT(A2)";
        ws.Cells["A7"].Formula = "INT(A3)";
        ws.Cells["A8"].Formula = "INT(A4)";

        ws.Calculate();

        Assert.Equal((int)currentDate.ToOADate(), ws.Cells["A5"].Value);
        Assert.Equal((int)currentDate.ToOADate(), ws.Cells["A6"].Value);
        Assert.Equal(31, ws.Cells["A7"].Value);
        Assert.Equal(31, ws.Cells["A8"].Value);
    }

    #endregion

    #region Date Math Tests

    [Fact]
    public void CalculateDateMath()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Test");

        var dateCell = worksheet.Cells[2, 2];
        var date = new DateTime(2013, 1, 1);
        dateCell.Value = date;

        var quotedDateCell = worksheet.Cells[2, 3];
        quotedDateCell.Formula = $"\"{date.ToString("d")}\"";

        var dateFormula = "B2";
        var dateFormulaWithMath = "B2+1";
        var quotedDateFormulaWithMath = $"\"{date.ToString("d")}\"+1";
        var quotedDateReferenceFormulaWithMath = "C2+1";

        var expectedDate = 41275.0; // January 1, 2013
        var expectedDateWithMath = 41276.0; // January 2, 2013

        Assert.Equal(expectedDate, worksheet.Calculate(dateFormula));
        Assert.Equal(expectedDateWithMath, worksheet.Calculate(dateFormulaWithMath));
        Assert.Equal(expectedDateWithMath, worksheet.Calculate(quotedDateFormulaWithMath));
        Assert.Equal(expectedDateWithMath, worksheet.Calculate(quotedDateReferenceFormulaWithMath));

        var formulaCell = worksheet.Cells[2, 4];
        formulaCell.Formula = dateFormulaWithMath;
        formulaCell.Calculate();
        Assert.Equal(expectedDateWithMath, formulaCell.Value);

        formulaCell.Formula = quotedDateReferenceFormulaWithMath;
        formulaCell.Calculate();
        Assert.Equal(expectedDateWithMath, formulaCell.Value);
    }

    #endregion
}
