using FreePPlus.Imaging;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using System;
using System.IO;
using FluentAssertions;
using Xunit;

#pragma warning disable IDE0059

namespace FreePPlus.OfficeOpenXml.Tests.ConditionalFormatting;

public class ConditionalFormattingTests : IDisposable
{
    private ExcelPackage _pck;

    public ConditionalFormattingTests()
    {
        if (!Directory.Exists("Test"))
        {
            Directory.CreateDirectory(string.Format("Test"));
        }

        _pck = new ExcelPackage(new FileInfo(@"Test\ConditionalFormatting.xlsx"));
    }

    [Fact]
    public void TwoColorScale()
    {
        var ws = _pck.Workbook.Worksheets.Add("ColorScale");
        ws.ConditionalFormatting.AddTwoColorScale(ws.Cells["A1:A5"]);
        ws.SetValue(1, 1, 1);
        ws.SetValue(2, 1, 2);
        ws.SetValue(3, 1, 3);
        ws.SetValue(4, 1, 4);
        ws.SetValue(5, 1, 5);
    }

    //Ignoring the following two tests for now - they seem to rely on an unprovided 'cf.xlsx' file
#pragma warning disable xUnit1013
#pragma warning disable CA1822

    //[TestMethod]
    //[Ignore]
    public void ReadConditionalFormatting()
    {
        var pck = new ExcelPackage(new FileInfo(@"c:\temp\cf.xlsx"));

        var ws = pck.Workbook.Worksheets[1];

        ws.ConditionalFormatting.Count.Should().Be(6);
        ws.ConditionalFormatting[0].Type.Should().Be(eExcelConditionalFormattingRuleType.DataBar);

        var cf1 = ws.ConditionalFormatting.AddEqual(ws.Cells["C3"]);
        //cf1.Formula = "TRUE";
        var cf2 = ws.Cells["C8:C12"].ConditionalFormatting.AddExpression();
        var cf3 = ws.Cells["d12:D22,H12:H22"].ConditionalFormatting.AddFourIconSet(eExcelconditionalFormatting4IconsSetType.RedToBlack);
        pck.SaveAs(new FileInfo(@"c:\temp\cf2.xlsx"));
    }

    //[TestMethod]
    //[Ignore]
    public void ReadConditionalFormattingError()
    {
        var pck = new ExcelPackage(new FileInfo(@"c:\temp\CofCTemplate.xlsx"));

        var ws = pck.Workbook.Worksheets[1];
        pck.SaveAs(new FileInfo(@"c:\temp\cf2.xlsx"));
    }

#pragma warning restore CA1822
#pragma warning restore xUnit1013

    [Fact]
    public void TwoBackColor()
    {
        var ws = _pck.Workbook.Worksheets.Add("TwoBackColor");
        IExcelConditionalFormattingEqual condition1 = ws.ConditionalFormatting.AddEqual(ws.Cells["A1"]);
        condition1.StopIfTrue = true;
        condition1.Priority = 1;
        condition1.Formula = "TRUE";
        condition1.Style.Fill.BackgroundColor.Color = Color.Green;
        IExcelConditionalFormattingEqual condition2 = ws.ConditionalFormatting.AddEqual(ws.Cells["A2"]);
        condition2.StopIfTrue = true;
        condition2.Priority = 2;
        condition2.Formula = "FALSE";
        condition2.Style.Fill.BackgroundColor.Color = Color.Red;
    }

    [Fact]
    public void Databar()
    {
        var ws = _pck.Workbook.Worksheets.Add("Databar");
        var cf = ws.ConditionalFormatting.AddDatabar(ws.Cells["A1:A5"], Color.BlueViolet);
        ws.SetValue(1, 1, 1);
        ws.SetValue(2, 1, 2);
        ws.SetValue(3, 1, 3);
        ws.SetValue(4, 1, 4);
        ws.SetValue(5, 1, 5);
    }

    [Fact]
    public void DatabarChangingAddressAddsConditionalFormatNodeInSchemaOrder()
    {
        var ws = _pck.Workbook.Worksheets.Add("DatabarAddressing");
        // Ensure there is at least one element that always exists below ConditionalFormatting nodes.   
        ws.HeaderFooter.AlignWithMargins = true;
        var cf = ws.ConditionalFormatting.AddDatabar(ws.Cells["A1:A5"], Color.BlueViolet);
        Assert.Equal("sheetData", cf.Node.ParentNode.PreviousSibling.LocalName);
        Assert.Equal("headerFooter", cf.Node.ParentNode.NextSibling.LocalName);
        cf.Address = new ExcelAddress("C3");
        Assert.Equal("sheetData", cf.Node.ParentNode.PreviousSibling.LocalName);
        Assert.Equal("headerFooter", cf.Node.ParentNode.NextSibling.LocalName);
    }

    [Fact]
    public void IconSet()
    {
        var ws = _pck.Workbook.Worksheets.Add("IconSet");
        var cf = ws.ConditionalFormatting.AddThreeIconSet(ws.Cells["A1:A3"], eExcelconditionalFormatting3IconsSetType.Symbols);
        ws.SetValue(1, 1, 1);
        ws.SetValue(2, 1, 2);
        ws.SetValue(3, 1, 3);

        var cf4 = ws.ConditionalFormatting.AddFourIconSet(ws.Cells["B1:B4"], eExcelconditionalFormatting4IconsSetType.Rating);
        cf4.Icon1.Type = eExcelConditionalFormattingValueObjectType.Formula;
        cf4.Icon1.Formula = "0";
        cf4.Icon2.Type = eExcelConditionalFormattingValueObjectType.Formula;
        cf4.Icon2.Formula = "1/3";
        cf4.Icon3.Type = eExcelConditionalFormattingValueObjectType.Formula;
        cf4.Icon3.Formula = "2/3";
        ws.SetValue(1, 2, 1);
        ws.SetValue(2, 2, 2);
        ws.SetValue(3, 2, 3);
        ws.SetValue(4, 2, 4);

        var cf5 = ws.ConditionalFormatting.AddFiveIconSet(ws.Cells["C1:C5"], eExcelconditionalFormatting5IconsSetType.Quarters);
        cf5.Icon1.Type = eExcelConditionalFormattingValueObjectType.Num;
        cf5.Icon1.Value = 1;
        cf5.Icon2.Type = eExcelConditionalFormattingValueObjectType.Num;
        cf5.Icon2.Value = 2;
        cf5.Icon3.Type = eExcelConditionalFormattingValueObjectType.Num;
        cf5.Icon3.Value = 3;
        cf5.Icon4.Type = eExcelConditionalFormattingValueObjectType.Num;
        cf5.Icon4.Value = 4;
        cf5.Icon5.Type = eExcelConditionalFormattingValueObjectType.Num;
        cf5.Icon5.Value = 5;
        cf5.ShowValue = false;
        cf5.Reverse = true;

        ws.SetValue(1, 3, 1);
        ws.SetValue(2, 3, 2);
        ws.SetValue(3, 3, 3);
        ws.SetValue(4, 3, 4);
        ws.SetValue(5, 3, 5);
    }

    #region | IDisposable implementation |

    public void Dispose()
    {
        _pck = null;
    }

    #endregion
}
