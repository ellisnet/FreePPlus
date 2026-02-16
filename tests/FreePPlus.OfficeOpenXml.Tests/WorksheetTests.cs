using CodeBrix.Imaging;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

public class WorksheetTests
{
#if SAVE_TEMP_FILES
    public const string TempFolder = @"C:\Temp";
#endif

    #region Worksheet Creation and Naming

    [Fact]
    public void AddWorksheet_SetsName()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("TestSheet");

        Assert.Equal("TestSheet", ws.Name);
    }

    [Fact]
    public void AddMultipleWorksheets_HaveCorrectCount()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("Sheet1");
        pck.Workbook.Worksheets.Add("Sheet2");
        pck.Workbook.Worksheets.Add("Sheet3");

        Assert.Equal(3, pck.Workbook.Worksheets.Count);
    }

    [Fact]
    public void AddDuplicateWorksheetName_ThrowsException()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("Sheet1");

        Assert.Throws<InvalidOperationException>(() => pck.Workbook.Worksheets.Add("Sheet1"));
    }

    [Fact]
    public void RenameWorksheet_UpdatesName()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("OldName");
        ws.Name = "NewName";

        Assert.Equal("NewName", ws.Name);
    }

    [Fact]
    public void RenameWithStartApostrophe_ThrowsException()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.Throws<ArgumentException>(() => ws.Name = "'Sheet1");
    }

    [Fact]
    public void RenameWithEndApostrophe_ThrowsException()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.Throws<ArgumentException>(() => ws.Name = "Sheet1'");
    }

    [Fact]
    public void WorksheetNameMaxLength_IsTruncatedTo31()
    {
        using var pck = new ExcelPackage();
        var longName = new string('A', 50);
        var ws = pck.Workbook.Worksheets.Add(longName);

        Assert.Equal(31, ws.Name.Length);
    }

    [Fact]
    public void WorksheetAccessByName_ReturnsCorrectSheet()
    {
        using var pck = new ExcelPackage();
        var ws1 = pck.Workbook.Worksheets.Add("First");
        var ws2 = pck.Workbook.Worksheets.Add("Second");

        Assert.Same(ws2, pck.Workbook.Worksheets["Second"]);
    }

    [Fact]
    public void WorksheetAccessByName_IsCaseInsensitive()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("TestSheet");

        Assert.Same(ws, pck.Workbook.Worksheets["testsheet"]);
    }

    #endregion

    #region Hidden State

    [Fact]
    public void NewWorksheet_IsVisible()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.Equal(eWorkSheetHidden.Visible, ws.Hidden);
    }

    [Fact]
    public void HideWorksheet_SetsHiddenState()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Hidden = eWorkSheetHidden.Hidden;

        Assert.Equal(eWorkSheetHidden.Hidden, ws.Hidden);
    }

    [Fact]
    public void VeryHideWorksheet_SetsVeryHiddenState()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Hidden = eWorkSheetHidden.VeryHidden;

        Assert.Equal(eWorkSheetHidden.VeryHidden, ws.Hidden);
    }

    [Fact]
    public void HiddenWorksheet_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            pck.Workbook.Worksheets.Add("Visible");
            var ws = pck.Workbook.Worksheets.Add("Hidden");
            ws.Hidden = eWorkSheetHidden.Hidden;
            var ws2 = pck.Workbook.Worksheets.Add("VeryHidden");
            ws2.Hidden = eWorkSheetHidden.VeryHidden;
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            Assert.Equal(eWorkSheetHidden.Visible, pck.Workbook.Worksheets["Visible"].Hidden);
            Assert.Equal(eWorkSheetHidden.Hidden, pck.Workbook.Worksheets["Hidden"].Hidden);
            Assert.Equal(eWorkSheetHidden.VeryHidden, pck.Workbook.Worksheets["VeryHidden"].Hidden);
        }
    }

    #endregion

    #region Cell Values and Formatting

    [Fact]
    public void SetCellValue_StoresValue()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Hello";
        ws.Cells["B1"].Value = 42;
        ws.Cells["C1"].Value = 3.14;
        ws.Cells["D1"].Value = true;
        ws.Cells["E1"].Value = new DateTime(2024, 1, 15);

        Assert.Equal("Hello", ws.Cells["A1"].Value);
        Assert.Equal(42, ws.Cells["B1"].Value);
        Assert.Equal(3.14, ws.Cells["C1"].Value);
        Assert.Equal(true, ws.Cells["D1"].Value);
        Assert.Equal(new DateTime(2024, 1, 15), ws.Cells["E1"].Value);
    }

    [Fact]
    public void ValueText_ReturnsFormattedNumber()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = 10000.5123;
        ws.Cells["A1"].Style.Numberformat.Format = "#,##0.00";

        var text = ws.Cells["A1"].Text;

        Assert.Contains("10", text);
    }

    [Fact]
    public void DateFormatText_ReturnsFormattedDate()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = new DateTime(2024, 6, 15);
        ws.Cells["A1"].Style.Numberformat.Format = "yyyy-MM-dd";

        var text = ws.Cells["A1"].Text;

        Assert.Equal("2024-06-15", text);
    }

    [Fact]
    public void Encoding_HandlesSpecialCharacters()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "ÅÄÖ";
        ws.Cells["B1"].Value = "Chinese: 你好";
        ws.Cells["C1"].Value = "Emoji: ✓";
        ws.Cells["D1"].Value = "Tab\there";
        ws.Cells["E1"].Value = "Line\nbreak";

        Assert.Equal("ÅÄÖ", ws.Cells["A1"].Value);
        Assert.Equal("Chinese: 你好", ws.Cells["B1"].Value);
        Assert.Equal("Emoji: ✓", ws.Cells["C1"].Value);
    }

    [Fact]
    public void Encoding_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Encoding");
            ws.Cells["A1"].Value = "ÅÄÖ åäö";
            ws.Cells["B1"].Value = "Special <>&\"'chars";
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Encoding"];
            Assert.Equal("ÅÄÖ åäö", ws.Cells["A1"].Value);
            Assert.Equal("Special <>&\"'chars", ws.Cells["B1"].Value);
        }
    }

    #endregion

    #region Insert and Delete Rows

    [Fact]
    public void InsertRow_ShiftsExistingDataDown()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Row1";
        ws.Cells["A2"].Value = "Row2";
        ws.Cells["A3"].Value = "Row3";

        ws.InsertRow(2, 2);

        Assert.Equal("Row1", ws.Cells["A1"].Value);
        Assert.Null(ws.Cells["A2"].Value);
        Assert.Null(ws.Cells["A3"].Value);
        Assert.Equal("Row2", ws.Cells["A4"].Value);
        Assert.Equal("Row3", ws.Cells["A5"].Value);
    }

    [Fact]
    public void InsertRows_SetsOutlineLevel()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Row(15).OutlineLevel = 1;
        ws.InsertRow(2, 10, 15);

        for (var i = 2; i < 12; i++)
        {
            Assert.Equal(1, ws.Row(i).OutlineLevel);
        }
    }

    [Fact]
    public void InsertColumns_SetsOutlineLevel()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Column(15).OutlineLevel = 1;
        ws.InsertColumn(2, 10, 15);

        for (var i = 2; i < 12; i++)
        {
            Assert.Equal(1, ws.Column(i).OutlineLevel);
        }
    }

    [Fact]
    public void DeleteRow_ShiftsDataUp()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Row1";
        ws.Cells["A2"].Value = "Row2";
        ws.Cells["A3"].Value = "Row3";
        ws.Cells["A4"].Value = "Row4";

        ws.DeleteRow(2, 1);

        Assert.Equal("Row1", ws.Cells["A1"].Value);
        Assert.Equal("Row3", ws.Cells["A2"].Value);
        Assert.Equal("Row4", ws.Cells["A3"].Value);
    }

    [Fact]
    public void InsertColumn_ShiftsDataRight()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Col1";
        ws.Cells["B1"].Value = "Col2";
        ws.Cells["C1"].Value = "Col3";

        ws.InsertColumn(2, 1);

        Assert.Equal("Col1", ws.Cells["A1"].Value);
        Assert.Null(ws.Cells["B1"].Value);
        Assert.Equal("Col2", ws.Cells["C1"].Value);
        Assert.Equal("Col3", ws.Cells["D1"].Value);
    }

    [Fact]
    public void DeleteColumn_ShiftsDataLeft()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Col1";
        ws.Cells["B1"].Value = "Col2";
        ws.Cells["C1"].Value = "Col3";

        ws.DeleteColumn(2, 1);

        Assert.Equal("Col1", ws.Cells["A1"].Value);
        Assert.Equal("Col3", ws.Cells["B1"].Value);
    }

    #endregion

    #region Formula Reference Updates

    [Fact]
    public void InsertRows_UpdatesFormulaReferences()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells[5, 3].Value = 3;
        ws.Cells[2, 2].Formula = "C5";
        ws.InsertRow(3, 10);

        Assert.Equal("C15", ws.Cells[2, 2].Formula);
    }

    [Fact]
    public void CrossSheetInsertRows_UpdatesReferences()
    {
        using var pck = new ExcelPackage();
        var sheet1 = pck.Workbook.Worksheets.Add("Sheet1");
        var sheet2 = pck.Workbook.Worksheets.Add("Sheet2");

        sheet1.Cells[2, 2].Formula = "Sheet2!C3";
        sheet2.InsertRow(3, 10);

        Assert.Equal("'SHEET2'!C13", sheet1.Cells[2, 2].Formula);
    }

    [Fact]
    public void CrossSheetInsertColumns_UpdatesReferences()
    {
        using var pck = new ExcelPackage();
        var sheet1 = pck.Workbook.Worksheets.Add("Sheet1");
        var sheet2 = pck.Workbook.Worksheets.Add("Sheet2");

        sheet1.Cells[2, 2].Formula = "Sheet2!C3";
        sheet2.InsertColumn(3, 10);

        Assert.Equal("'SHEET2'!M3", sheet1.Cells[2, 2].Formula);
    }

    [Fact]
    public void CrossSheetInsertRowAfterReference_HasNoEffect()
    {
        using var pck = new ExcelPackage();
        var sheet1 = pck.Workbook.Worksheets.Add("Sheet1");
        var sheet2 = pck.Workbook.Worksheets.Add("Sheet2");

        sheet1.Cells[2, 2].Formula = "Sheet2!C3";
        sheet2.InsertRow(4, 10);

        Assert.Equal("'SHEET2'!C3", sheet1.Cells[2, 2].Formula);
    }

    [Fact]
    public void CrossSheetInsertColumnAfterReference_HasNoEffect()
    {
        using var pck = new ExcelPackage();
        var sheet1 = pck.Workbook.Worksheets.Add("Sheet1");
        var sheet2 = pck.Workbook.Worksheets.Add("Sheet2");

        sheet1.Cells[2, 2].Formula = "Sheet2!C3";
        sheet2.InsertColumn(4, 10);

        Assert.Equal("'SHEET2'!C3", sheet1.Cells[2, 2].Formula);
    }

    [Fact]
    public void CrossSheetReference_UpdatedWhenSheetRenamed()
    {
        using var pck = new ExcelPackage();
        var sheet = pck.Workbook.Worksheets.Add("Sheet1");
        var otherSheet = pck.Workbook.Worksheets.Add("Other Sheet");

        sheet.Cells[3, 3].Formula = "'Other Sheet'!C3";
        otherSheet.Name = "New Name";

        Assert.Equal("'New Name'!C3", sheet.Cells[3, 3].Formula);
    }

    [Fact]
    public void InsertRow_UpdatesSameSheetFormulas()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Formula = "SUM(A5:A10)";

        ws.InsertRow(3, 2);

        Assert.Equal("SUM(A7:A12)", ws.Cells["A1"].Formula);
    }

    #endregion

    #region Cell Copy with Cross-Sheet References

    [Fact]
    public void CopyCellUpdatesRelativeCrossSheetReferences()
    {
        using var pck = new ExcelPackage();
        var sheet1 = pck.Workbook.Worksheets.Add("Sheet1");
        var sheet2 = pck.Workbook.Worksheets.Add("Sheet2");

        sheet1.Cells[3, 3].Formula = "Sheet2!C3";
        sheet1.Cells[3, 3].Copy(sheet1.Cells[4, 4]);

        Assert.Equal("'SHEET2'!D4", sheet1.Cells[4, 4].Formula);
    }

    [Fact]
    public void CopyCellUpdatesAbsoluteCrossSheetReferences()
    {
        using var pck = new ExcelPackage();
        var sheet1 = pck.Workbook.Worksheets.Add("Sheet1");
        var sheet2 = pck.Workbook.Worksheets.Add("Sheet2");

        sheet1.Cells[3, 3].Formula = "Sheet2!$C$3";
        sheet1.Cells[3, 3].Copy(sheet1.Cells[4, 4]);

        Assert.Equal("'SHEET2'!$C$3", sheet1.Cells[4, 4].Formula);
    }

    [Fact]
    public void CopyCellUpdatesRowAbsoluteCrossSheetReferences()
    {
        using var pck = new ExcelPackage();
        var sheet1 = pck.Workbook.Worksheets.Add("Sheet1");
        var sheet2 = pck.Workbook.Worksheets.Add("Sheet2");

        sheet1.Cells[3, 3].Formula = "Sheet2!C$3";
        sheet1.Cells[3, 3].Copy(sheet1.Cells[4, 4]);

        Assert.Equal("'SHEET2'!D$3", sheet1.Cells[4, 4].Formula);
    }

    [Fact]
    public void CopyCellUpdatesColumnAbsoluteCrossSheetReferences()
    {
        using var pck = new ExcelPackage();
        var sheet1 = pck.Workbook.Worksheets.Add("Sheet1");
        var sheet2 = pck.Workbook.Worksheets.Add("Sheet2");

        sheet1.Cells[3, 3].Formula = "Sheet2!$C3";
        sheet1.Cells[3, 3].Copy(sheet1.Cells[4, 4]);

        Assert.Equal("'SHEET2'!$C4", sheet1.Cells[4, 4].Formula);
    }

    #endregion

    #region Merged Cells

    [Fact]
    public void MergeCells_SetsAndRetrieves()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1:A4"].Merge = true;

        Assert.True(ws.Cells["A1:A4"].Merge);
        Assert.Contains("A1:A4", ws.MergedCells);
    }

    [Fact]
    public void MergeCells_MultipleRanges()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1:D1"].Merge = true;
        ws.Cells["E1:H1"].Merge = true;

        Assert.Contains("A1:D1", ws.MergedCells);
        Assert.Contains("E1:H1", ws.MergedCells);
    }

    [Fact]
    public void MergedCells_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1:C3"].Merge = true;
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.True(ws.Cells["A1:C3"].Merge);
        }
    }

    #endregion

    #region AutoFilter

    [Fact]
    public void AutoFilter_SetsOnRange()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "Header1";
        ws.Cells["B1"].Value = "Header2";
        ws.Cells["A2"].Value = "Data1";
        ws.Cells["B2"].Value = "Data2";

        ws.Cells["A1:B2"].AutoFilter = true;

        Assert.True(ws.Cells["A1:B2"].AutoFilter);
    }

    #endregion

    #region Protection

    [Fact]
    public void Protection_DefaultIsNotProtected()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.False(ws.Protection.IsProtected);
    }

    [Fact]
    public void Protection_SetProtected()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Protection.IsProtected = true;

        Assert.True(ws.Protection.IsProtected);
    }

    [Fact]
    public void Protection_SetPassword()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Protection.SetPassword("TestPassword");

        Assert.True(ws.Protection.IsProtected);
    }

    [Fact]
    public void Protection_AllowSettings()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Protection.IsProtected = true;
        ws.Protection.AllowSort = true;
        ws.Protection.AllowAutoFilter = true;
        ws.Protection.AllowInsertRows = true;

        Assert.True(ws.Protection.AllowSort);
        Assert.True(ws.Protection.AllowAutoFilter);
        Assert.True(ws.Protection.AllowInsertRows);
    }

    [Fact]
    public void Protection_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Protection.SetPassword("MyPassword");
            ws.Protection.AllowSort = true;
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.True(ws.Protection.IsProtected);
            Assert.True(ws.Protection.AllowSort);
        }
    }

    #endregion

    #region View Properties

    [Fact]
    public void ShowGridLines_DefaultIsTrue()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.True(ws.View.ShowGridLines);
    }

    [Fact]
    public void ShowGridLines_CanBeDisabled()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.View.ShowGridLines = false;

        Assert.False(ws.View.ShowGridLines);
    }

    [Fact]
    public void ShowGridLines_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.View.ShowGridLines = false;
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            Assert.False(pck.Workbook.Worksheets["Sheet1"].View.ShowGridLines);
        }
    }

    [Fact]
    public void FreezePanes_SetsPanes()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.View.FreezePanes(2, 3);

        Assert.NotNull(ws.View.Panes);
        Assert.True(ws.View.Panes.Length > 0);
    }

    [Fact]
    public void PageBreakView_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.View.PageBreakView = true;

        Assert.True(ws.View.PageBreakView);
    }

    [Fact]
    public void PageLayoutView_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.View.PageLayoutView = true;

        Assert.True(ws.View.PageLayoutView);
    }

    [Fact]
    public void ShowHeaders_CanBeToggled()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.View.ShowHeaders = true;

        Assert.True(ws.View.ShowHeaders);
    }

    [Fact]
    public void ZoomScale_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.View.ZoomScale = 150;

        Assert.Equal(150, ws.View.ZoomScale);
    }

    [Fact]
    public void ZoomScale_OutOfRange_Throws()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.Throws<ArgumentOutOfRangeException>(() => ws.View.ZoomScale = 5);
        Assert.Throws<ArgumentOutOfRangeException>(() => ws.View.ZoomScale = 500);
    }

    [Fact]
    public void RightToLeft_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.View.RightToLeft = true;

        Assert.True(ws.View.RightToLeft);
    }

    [Fact]
    public void SelectedRange_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.View.SelectedRange = "B2:D5";

        Assert.Equal("B2:D5", ws.View.SelectedRange);
    }

    [Fact]
    public void ActiveCell_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.View.ActiveCell = "C3";

        Assert.Equal("C3", ws.View.ActiveCell);
    }

    #endregion

    #region Worksheet Dimensions

    [Fact]
    public void Dimension_IsNullForEmptySheet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.Null(ws.Dimension);
    }

    [Fact]
    public void Dimension_ReflectsDataRange()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["B2"].Value = "Start";
        ws.Cells["E10"].Value = "End";

        Assert.NotNull(ws.Dimension);
        Assert.Equal(2, ws.Dimension.Start.Row);
        Assert.Equal(2, ws.Dimension.Start.Column);
        Assert.Equal(10, ws.Dimension.End.Row);
        Assert.Equal(5, ws.Dimension.End.Column);
    }

    #endregion

    #region Default Row Height and Column Width

    [Fact]
    public void DefaultRowHeight_HasDefault()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.True(ws.DefaultRowHeight > 0);
    }

    [Fact]
    public void DefaultRowHeight_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.DefaultRowHeight = 20;

        Assert.Equal(20, ws.DefaultRowHeight);
    }

    [Fact]
    public void DefaultColWidth_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.DefaultColWidth = 15;

        Assert.Equal(15, ws.DefaultColWidth);
    }

    [Fact]
    public void DefaultColWidth_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.DefaultColWidth = 25;
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            Assert.Equal(25, pck.Workbook.Worksheets["Sheet1"].DefaultColWidth);
        }
    }

    #endregion

    #region Tab Color

    [Fact]
    public void TabColor_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.TabColor = Color.Red;

        Assert.Equal(Color.Red, ws.TabColor);
    }

    [Fact]
    public void TabColor_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.TabColor = Color.Blue;
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.Equal(Color.Blue, ws.TabColor);
        }
    }

    #endregion

    #region Row and Column Properties

    [Fact]
    public void RowHeight_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Row(1).Height = 30;

        Assert.Equal(30, ws.Row(1).Height);
    }

    [Fact]
    public void RowHidden_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Row(2).Hidden = true;

        Assert.True(ws.Row(2).Hidden);
    }

    [Fact]
    public void ColumnWidth_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Column(1).Width = 20;

        Assert.Equal(20, ws.Column(1).Width);
    }

    [Fact]
    public void ColumnHidden_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Column(2).Hidden = true;

        Assert.True(ws.Column(2).Hidden);
    }

    [Fact]
    public void OutlineLevel_RowCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Row(1).OutlineLevel = 2;

        Assert.Equal(2, ws.Row(1).OutlineLevel);
    }

    [Fact]
    public void OutlineLevel_ColumnCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Column(1).OutlineLevel = 3;

        Assert.Equal(3, ws.Column(1).OutlineLevel);
    }

    #endregion

    #region Hyperlinks

    [Fact]
    public void Hyperlink_ExternalUrl()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Hyperlink = new Uri("https://www.example.com");
        ws.Cells["A1"].Value = "Example Link";

        Assert.NotNull(ws.Cells["A1"].Hyperlink);
        Assert.Contains("example.com", ws.Cells["A1"].Hyperlink.AbsoluteUri);
    }

    [Fact]
    public void Hyperlink_InternalReference()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        pck.Workbook.Worksheets.Add("Sheet2");

        var hl = new ExcelHyperLink("Sheet2!A1", "Go to Sheet2");
        ws.Cells["A1"].Hyperlink = hl;

        Assert.NotNull(ws.Cells["A1"].Hyperlink);
        var excelHl = ws.Cells["A1"].Hyperlink as ExcelHyperLink;
        Assert.NotNull(excelHl);
        Assert.Equal("Sheet2!A1", excelHl.ReferenceAddress);
    }

    [Fact]
    public void Hyperlink_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].Hyperlink = new Uri("https://www.example.com");
            ws.Cells["A1"].Value = "Example";
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.NotNull(ws.Cells["A1"].Hyperlink);
        }
    }

    #endregion

    #region Printer Settings

    [Fact]
    public void RepeatRowsAndColumns_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.PrinterSettings.RepeatColumns = new ExcelAddress("A:A");
        ws.PrinterSettings.RepeatRows = new ExcelAddress("1:1");

        Assert.NotNull(ws.PrinterSettings.RepeatColumns);
        Assert.NotNull(ws.PrinterSettings.RepeatRows);
    }

    [Fact]
    public void PrintArea_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.PrinterSettings.PrintArea = ws.Cells["A1:D10"];

        Assert.NotNull(ws.PrinterSettings.PrintArea);
    }

    [Fact]
    public void PrinterSettings_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.PrinterSettings.RepeatRows = new ExcelAddress("1:2");
            ws.PrinterSettings.RepeatColumns = new ExcelAddress("A:B");
            ws.PrinterSettings.Orientation = eOrientation.Landscape;
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.NotNull(ws.PrinterSettings.RepeatRows);
            Assert.NotNull(ws.PrinterSettings.RepeatColumns);
            Assert.Equal(eOrientation.Landscape, ws.PrinterSettings.Orientation);
        }
    }

    #endregion

    #region Worksheet Copy

    [Fact]
    public void CopyWorksheet_CopiesName()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Original");
        ws.Cells["A1"].Value = "Hello";
        ws.Cells["B1"].Value = 42;

        var copy = pck.Workbook.Worksheets.Add("Copy", ws);

        Assert.Equal("Copy", copy.Name);
        Assert.Equal("Hello", copy.Cells["A1"].Value);
        Assert.Equal(42, copy.Cells["B1"].Value);
    }

    [Fact]
    public void CopyWorksheet_CopiesFormulas()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Original");
        ws.Cells["A1"].Value = 10;
        ws.Cells["A2"].Value = 20;
        ws.Cells["A3"].Formula = "SUM(A1:A2)";

        var copy = pck.Workbook.Worksheets.Add("Copy", ws);

        Assert.Equal("SUM(A1:A2)", copy.Cells["A3"].Formula);
    }

    [Fact]
    public void CopyWorksheet_CopiesMergedCells()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Original");
        ws.Cells["A1:C1"].Merge = true;

        var copy = pck.Workbook.Worksheets.Add("Copy", ws);

        Assert.True(copy.Cells["A1:C1"].Merge);
    }

    [Fact]
    public void CopyWorksheetWithSharedFormula_Works()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = 1;
        ws.Cells["A2"].Value = 2;
        ws.Cells["A3"].Value = 3;
        ws.Cells["B1"].Formula = "A1*2";
        ws.Cells["B2"].Formula = "A2*2";
        ws.Cells["B3"].Formula = "A3*2";

        var copy = pck.Workbook.Worksheets.Add("Copy", ws);

        Assert.NotNull(copy.Cells["B1"].Formula);
    }

    #endregion

    #region Table Operations

    [Fact]
    public void Table_AddTable()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Name";
        ws.Cells["B1"].Value = "Age";
        ws.Cells["A2"].Value = "John";
        ws.Cells["B2"].Value = 30;

        var table = ws.Tables.Add(ws.Cells["A1:B2"], "TestTable");

        Assert.NotNull(table);
        Assert.Equal("TestTable", table.Name);
        Assert.Equal(1, ws.Tables.Count);
    }

    [Fact]
    public void Table_DeleteTable()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Name";
        ws.Cells["B1"].Value = "Age";
        ws.Cells["A2"].Value = "John";
        ws.Cells["B2"].Value = 30;

        ws.Tables.Add(ws.Cells["A1:B2"], "TestTable");
        Assert.Equal(1, ws.Tables.Count);

        ws.Tables.Delete("TestTable");
        Assert.Equal(0, ws.Tables.Count);
    }

    [Fact]
    public void TableName_CannotStartWithNumber()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Col1";

        Assert.Throws<ArgumentException>(() =>
            ws.Tables.Add(ws.Cells["A1"], "5TestTable"));
    }

    [Fact]
    public void TableName_CannotContainWhitespace()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Col1";

        Assert.Throws<ArgumentException>(() =>
            ws.Tables.Add(ws.Cells["A1"], "Test Table"));
    }

    [Fact]
    public void TableName_CanStartWithBackslash()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Col1";

        var table = ws.Tables.Add(ws.Cells["A1"], "\\TestTable");
        Assert.NotNull(table);
    }

    [Fact]
    public void TableName_CanStartWithUnderscore()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Col1";

        var table = ws.Tables.Add(ws.Cells["A1"], "_TestTable");
        Assert.NotNull(table);
    }

    [Fact]
    public void Table_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].Value = "Name";
            ws.Cells["B1"].Value = "Value";
            ws.Cells["A2"].Value = "Test";
            ws.Cells["B2"].Value = 100;
            ws.Tables.Add(ws.Cells["A1:B2"], "MyTable");
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.Equal(1, ws.Tables.Count);
            Assert.Equal("MyTable", ws.Tables[0].Name);
        }
    }

    #endregion

    #region Named Ranges

    [Fact]
    public void NamedRange_SheetLevel()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Test";

        ws.Names.Add("MyName", ws.Cells["A1"]);

        Assert.Equal(1, ws.Names.Count);
        Assert.NotNull(ws.Names["MyName"]);
    }

    [Fact]
    public void NamedRange_WorkbookLevel()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Test";

        pck.Workbook.Names.Add("GlobalName", ws.Cells["A1"]);

        Assert.Equal(1, pck.Workbook.Names.Count);
        Assert.NotNull(pck.Workbook.Names["GlobalName"]);
    }

    [Fact]
    public void NamedRange_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].Value = 42;
            ws.Names.Add("TestName", ws.Cells["A1"]);
            pck.Workbook.Names.Add("GlobalName", ws.Cells["B1"]);
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.Equal(1, ws.Names.Count);
            Assert.Equal(1, pck.Workbook.Names.Count);
        }
    }

    #endregion

    #region Style and Formatting

    [Fact]
    public void StyleFill_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

        Assert.Equal(ExcelFillStyle.Solid, ws.Cells["A1"].Style.Fill.PatternType);
    }

    [Fact]
    public void StyleFont_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Style.Font.Bold = true;
        ws.Cells["A1"].Style.Font.Italic = true;
        ws.Cells["A1"].Style.Font.Size = 14;

        Assert.True(ws.Cells["A1"].Style.Font.Bold);
        Assert.True(ws.Cells["A1"].Style.Font.Italic);
        Assert.Equal(14, ws.Cells["A1"].Style.Font.Size);
    }

    [Fact]
    public void StyleBorder_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        ws.Cells["A1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

        Assert.Equal(ExcelBorderStyle.Thin, ws.Cells["A1"].Style.Border.Top.Style);
        Assert.Equal(ExcelBorderStyle.Thick, ws.Cells["A1"].Style.Border.Bottom.Style);
    }

    [Fact]
    public void StyleAlignment_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

        Assert.Equal(ExcelHorizontalAlignment.Center, ws.Cells["A1"].Style.HorizontalAlignment);
        Assert.Equal(ExcelVerticalAlignment.Center, ws.Cells["A1"].Style.VerticalAlignment);
    }

    [Fact]
    public void StyleWrapText_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Style.WrapText = true;

        Assert.True(ws.Cells["A1"].Style.WrapText);
    }

    #endregion

    #region Comments

    [Fact]
    public void Comment_CanBeAdded()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Data";

        ws.Cells["A1"].AddComment("This is a comment", "Author");

        Assert.Equal(1, ws.Comments.Count);
        Assert.Equal("This is a comment", ws.Comments[0].Text);
        Assert.Equal("Author", ws.Comments[0].Author);
    }

    [Fact]
    public void Comment_ShiftsWithRowInsert()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A5"].Value = "Data";
        ws.Cells["A5"].AddComment("Comment on A5", "Author");

        ws.InsertRow(3, 2);

        // The comment should have shifted from row 5 to row 7
        Assert.Equal(1, ws.Comments.Count);
        Assert.Equal(7, ws.Comments[0].Range.Start.Row);
    }

    [Fact]
    public void Comment_ShiftsWithColumnInsert()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["C1"].Value = "Data";
        ws.Cells["C1"].AddComment("Comment on C1", "Author");

        ws.InsertColumn(2, 2);

        // The comment should have shifted from column C(3) to column E(5)
        Assert.Equal(1, ws.Comments.Count);
        Assert.Equal(5, ws.Comments[0].Range.Start.Column);
    }

    [Fact]
    public void Comment_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].Value = "Test";
            ws.Cells["A1"].AddComment("My Comment", "TestAuthor");
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.Equal(1, ws.Comments.Count);
            Assert.Equal("My Comment", ws.Comments[0].Text);
        }
    }

    #endregion

    #region Date1904

    [Fact]
    public void Date1904_DefaultIsFalse()
    {
        using var pck = new ExcelPackage();
        Assert.False(pck.Workbook.Date1904);
    }

    [Fact]
    public void Date1904_CanBeEnabled()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Date1904 = true;
        Assert.True(pck.Workbook.Date1904);
    }

    [Fact]
    public void Date1904_WithoutSetting_CorrectDate()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var dateTest = new DateTime(2008, 2, 29);

        ws.Cells[1, 1].Value = dateTest;
        ws.Cells[1, 1].Style.Numberformat.Format = "dd/MM/yyyy";

        Assert.Equal(dateTest, ws.Cells[1, 1].Value);
    }

    [Fact]
    public void Date1904_WithSetting_CorrectDate()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Date1904 = true;
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var dateTest = new DateTime(2008, 2, 29);

        ws.Cells[1, 1].Value = dateTest;
        ws.Cells[1, 1].Style.Numberformat.Format = "dd/MM/yyyy";

        Assert.Equal(dateTest, ws.Cells[1, 1].Value);
    }

    [Fact]
    public void Date1904_RoundTrips()
    {
        using var ms = new MemoryStream();
        var dateTest = new DateTime(2024, 6, 15, 10, 30, 0);

        using (var pck = new ExcelPackage())
        {
            pck.Workbook.Date1904 = true;
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells[1, 1].Value = dateTest;
            ws.Cells[1, 1].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            Assert.True(pck.Workbook.Date1904);
        }
    }

    #endregion

    #region Worksheet Delete

    [Fact]
    public void DeleteWorksheet_ByName()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("Sheet1");
        pck.Workbook.Worksheets.Add("Sheet2");
        pck.Workbook.Worksheets.Add("Sheet3");

        pck.Workbook.Worksheets.Delete("Sheet2");

        Assert.Equal(2, pck.Workbook.Worksheets.Count);
        Assert.Null(pck.Workbook.Worksheets["Sheet2"]);
    }

    [Fact]
    public void DeleteWorksheet_ByIndex()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("Sheet1");
        pck.Workbook.Worksheets.Add("Sheet2");

        pck.Workbook.Worksheets.Delete(1);

        Assert.Equal(1, pck.Workbook.Worksheets.Count);
    }

    #endregion

    #region Worksheet Move

    [Fact]
    public void MoveWorksheet_Before()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("Sheet1");
        pck.Workbook.Worksheets.Add("Sheet2");
        pck.Workbook.Worksheets.Add("Sheet3");

        pck.Workbook.Worksheets.MoveBefore("Sheet3", "Sheet1");

        Assert.Equal("Sheet3", pck.Workbook.Worksheets[0].Name);
    }

    [Fact]
    public void MoveWorksheet_After()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("Sheet1");
        pck.Workbook.Worksheets.Add("Sheet2");
        pck.Workbook.Worksheets.Add("Sheet3");

        pck.Workbook.Worksheets.MoveAfter("Sheet1", "Sheet3");

        Assert.Equal("Sheet1", pck.Workbook.Worksheets[2].Name);
    }

    [Fact]
    public void MoveWorksheet_ToStart()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("Sheet1");
        pck.Workbook.Worksheets.Add("Sheet2");
        pck.Workbook.Worksheets.Add("Sheet3");

        pck.Workbook.Worksheets.MoveToStart("Sheet3");

        Assert.Equal("Sheet3", pck.Workbook.Worksheets[0].Name);
    }

    [Fact]
    public void MoveWorksheet_ToEnd()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("Sheet1");
        pck.Workbook.Worksheets.Add("Sheet2");
        pck.Workbook.Worksheets.Add("Sheet3");

        pck.Workbook.Worksheets.MoveToEnd("Sheet1");

        Assert.Equal("Sheet1", pck.Workbook.Worksheets[2].Name);
    }

    #endregion

    #region Select and TabSelected

    [Fact]
    public void Select_SetsSelectedRange()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Select("B2:D4");

        Assert.Equal("B2:D4", ws.View.SelectedRange);
    }

    [Fact]
    public void TabSelected_FirstSheetIsNotSelectedByDefault()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.False(ws.View.TabSelected);
    }

    #endregion

    #region Header and Footer

    [Fact]
    public void HeaderFooter_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.HeaderFooter.OddHeader.LeftAlignedText = "Left Header";
        ws.HeaderFooter.OddHeader.CenteredText = "Center Header";
        ws.HeaderFooter.OddHeader.RightAlignedText = "Right Header";
        ws.HeaderFooter.OddFooter.CenteredText = "Page &P of &N";

        Assert.Equal("Left Header", ws.HeaderFooter.OddHeader.LeftAlignedText);
        Assert.Equal("Center Header", ws.HeaderFooter.OddHeader.CenteredText);
        Assert.Equal("Right Header", ws.HeaderFooter.OddHeader.RightAlignedText);
        Assert.Equal("Page &P of &N", ws.HeaderFooter.OddFooter.CenteredText);
    }

    [Fact]
    public void HeaderFooter_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.HeaderFooter.OddHeader.CenteredText = "Test Header";
            ws.HeaderFooter.OddFooter.CenteredText = "Test Footer";
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.Equal("Test Header", ws.HeaderFooter.OddHeader.CenteredText);
            Assert.Equal("Test Footer", ws.HeaderFooter.OddFooter.CenteredText);
        }
    }

    #endregion

    #region Formula Features

    [Fact]
    public void Formula_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = 10;
        ws.Cells["A2"].Value = 20;
        ws.Cells["A3"].Formula = "SUM(A1:A2)";

        Assert.Equal("SUM(A1:A2)", ws.Cells["A3"].Formula);
    }

    [Fact]
    public void FormulaR1C1_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A3"].FormulaR1C1 = "SUM(R[-2]C:R[-1]C)";

        Assert.NotEmpty(ws.Cells["A3"].FormulaR1C1);
    }

    [Fact]
    public void Formula_CrossSheetReference()
    {
        using var pck = new ExcelPackage();
        var ws1 = pck.Workbook.Worksheets.Add("Sheet1");
        var ws2 = pck.Workbook.Worksheets.Add("Sheet2");
        ws2.Cells["A1"].Value = 100;

        ws1.Cells["A1"].Formula = "Sheet2!A1";

        Assert.Equal("Sheet2!A1", ws1.Cells["A1"].Formula);
    }

    [Fact]
    public void Formula_OverwriteWithValue()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Formula = "1+1";
        ws.Cells["A1"].Value = 42;

        Assert.Equal(42, ws.Cells["A1"].Value);
    }

    [Fact]
    public void Formula_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].Value = 5;
            ws.Cells["A2"].Value = 10;
            ws.Cells["A3"].Formula = "SUM(A1:A2)";
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sheet1"];
            Assert.Equal("SUM(A1:A2)", ws.Cells["A3"].Formula);
        }
    }

    #endregion

    #region Copy Range

    [Fact]
    public void CopyRange_CopiesValues()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Test";
        ws.Cells["A2"].Value = 42;

        ws.Cells["A1:A2"].Copy(ws.Cells["C1"]);

        Assert.Equal("Test", ws.Cells["C1"].Value);
        Assert.Equal(42, ws.Cells["C2"].Value);
    }

    [Fact]
    public void CopyRange_CopiesMergedCells()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1:B1"].Merge = true;
        ws.Cells["A1"].Value = "Merged";

        ws.Cells["A1:B2"].Copy(ws.Cells["D1"]);

        Assert.True(ws.Cells["D1:E1"].Merge);
    }

    #endregion

    #region Outline Summary

    [Fact]
    public void OutlineSummaryBelow_DefaultIsFalse()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.False(ws.OutLineSummaryBelow);
    }

    [Fact]
    public void OutlineSummaryBelow_CanBeDisabled()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.OutLineSummaryBelow = false;

        Assert.False(ws.OutLineSummaryBelow);
    }

    [Fact]
    public void OutlineSummaryRight_DefaultIsFalse()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        Assert.False(ws.OutLineSummaryRight);
    }

    #endregion

    #region Data Load and Large Data

    [Fact]
    public void LoadData_ManyRows()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        for (var r = 1; r <= 1000; r++)
        {
            ws.Cells[r, 1].Value = r;
            ws.Cells[r, 2].Value = $"Row {r}";
            ws.Cells[r, 3].Value = r * 1.5;
        }

        Assert.Equal(1, ws.Cells[1, 1].Value);
        Assert.Equal(1000, ws.Cells[1000, 1].Value);
        Assert.Equal("Row 500", ws.Cells[500, 2].Value);
    }

    [Fact]
    public void GetValueSetValue_WorksForVariousTypes()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.SetValue(1, 1, "String");
        ws.SetValue(2, 1, 42);
        ws.SetValue(3, 1, 3.14);
        ws.SetValue(4, 1, true);
        ws.SetValue(5, 1, new DateTime(2024, 1, 1));

        Assert.Equal("String", ws.GetValue(1, 1));
        Assert.Equal(42, ws.GetValue(2, 1));
        Assert.Equal(3.14, ws.GetValue(3, 1));
        Assert.Equal(true, ws.GetValue(4, 1));
        Assert.Equal(new DateTime(2024, 1, 1), ws.GetValue(5, 1));
    }

    [Fact]
    public void GetValueGeneric_ReturnsTypedValue()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = 42;
        ws.Cells["A2"].Value = "Hello";

        Assert.Equal(42, ws.GetValue<int>(1, 1));
        Assert.Equal("Hello", ws.GetValue<string>(2, 1));
    }

    #endregion

    #region Conditional Formatting

    [Fact]
    public void ConditionalFormatting_CanBeAdded()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        var cf = ws.ConditionalFormatting.AddGreaterThan(ws.Cells["A1:A10"]);
        cf.Formula = "5";
        cf.Style.Font.Bold = true;

        Assert.Equal(1, ws.ConditionalFormatting.Count);
    }

    #endregion

    #region Data Validation

    [Fact]
    public void DataValidation_WholeNumber()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        var dv = ws.DataValidations.AddIntegerValidation("A1:A10");
        dv.Operator = ExcelDataValidationOperator.between;
        dv.Formula.Value = 1;
        dv.Formula2.Value = 100;

        Assert.Equal(1, ws.DataValidations.Count);
    }

    #endregion

    #region Copy Row with Outline Levels

    [Fact]
    public void CopyRow_SetsOutlineLevelsCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Row(1).OutlineLevel = 0;
        ws.Row(2).OutlineLevel = 1;
        ws.Row(3).OutlineLevel = 2;
        ws.Row(4).OutlineLevel = 1;
        ws.Row(5).OutlineLevel = 0;

        // Copy row pattern by inserting rows and copying from source
        // Note: InsertRow uses Row(copyStylesFromRow + rows).OutlineLevel for the outline level
        ws.InsertRow(10, 1, 2);
        ws.InsertRow(11, 1, 3);
        ws.InsertRow(12, 1, 4);

        Assert.Equal(2, ws.Row(10).OutlineLevel);  // Row(2+1)=Row(3) outline=2
        Assert.Equal(1, ws.Row(11).OutlineLevel);  // Row(3+1)=Row(4) outline=1
        Assert.Equal(0, ws.Row(12).OutlineLevel);  // Row(4+1)=Row(5) outline=0
    }

    [Fact]
    public void CopyColumn_SetsOutlineLevelsCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Column(1).OutlineLevel = 0;
        ws.Column(2).OutlineLevel = 1;
        ws.Column(3).OutlineLevel = 2;

        ws.InsertColumn(10, 1, 2);
        ws.InsertColumn(11, 1, 3);

        Assert.Equal(1, ws.Column(10).OutlineLevel);
        Assert.Equal(2, ws.Column(11).OutlineLevel);
    }

    #endregion

    #region Worksheet Enumeration

    [Fact]
    public void Worksheets_CanBeEnumerated()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("Sheet1");
        pck.Workbook.Worksheets.Add("Sheet2");
        pck.Workbook.Worksheets.Add("Sheet3");

        var names = pck.Workbook.Worksheets.Select(ws => ws.Name).ToList();

        Assert.Equal(3, names.Count);
        Assert.Contains("Sheet1", names);
        Assert.Contains("Sheet2", names);
        Assert.Contains("Sheet3", names);
    }

    #endregion

    #region Round-Trip Persistence

    [Fact]
    public async Task FullWorksheet_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Complete");
            ws.TabColor = Color.Green;
            ws.DefaultRowHeight = 18;
            ws.DefaultColWidth = 12;
            ws.View.ShowGridLines = false;

            ws.Cells["A1"].Value = "Name";
            ws.Cells["B1"].Value = "Value";
            ws.Cells["A1:B1"].Style.Font.Bold = true;

            ws.Cells["A2"].Value = "Item1";
            ws.Cells["B2"].Value = 100;
            ws.Cells["A3"].Value = "Item2";
            ws.Cells["B3"].Value = 200;
            ws.Cells["B4"].Formula = "SUM(B2:B3)";

            ws.Cells["D1:F1"].Merge = true;
            ws.Cells["D1"].Value = "Merged Header";

            ws.HeaderFooter.OddHeader.CenteredText = "Test Report";

            await pck.SaveAsAsync(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Complete"];
            Assert.NotNull(ws);
            Assert.Equal(Color.Green, ws.TabColor);
            Assert.Equal(18, ws.DefaultRowHeight);
            Assert.Equal(12, ws.DefaultColWidth);
            Assert.False(ws.View.ShowGridLines);
            Assert.Equal("Name", ws.Cells["A1"].Value);
            Assert.Equal(100d, Convert.ToDouble(ws.Cells["B2"].Value));
            Assert.Equal("SUM(B2:B3)", ws.Cells["B4"].Formula);
            Assert.True(ws.Cells["D1:F1"].Merge);
            Assert.Equal("Test Report", ws.HeaderFooter.OddHeader.CenteredText);
        }

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            ms.Position = 0;
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(FullWorksheet_RoundTrips)}.xlsx");
            await File.WriteAllBytesAsync(filename, ms.ToArray(), CancellationToken.None);
        }
#endif
    }

    [Fact]
    public async Task MultipleWorksheets_RoundTrip()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            for (var i = 1; i <= 5; i++)
            {
                var ws = pck.Workbook.Worksheets.Add($"Sheet{i}");
                ws.Cells["A1"].Value = $"Data on Sheet {i}";
                ws.Cells["A2"].Value = i * 100;
            }

            await pck.SaveAsAsync(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            Assert.Equal(5, pck.Workbook.Worksheets.Count);
            for (var i = 1; i <= 5; i++)
            {
                var ws = pck.Workbook.Worksheets[$"Sheet{i}"];
                Assert.Equal($"Data on Sheet {i}", ws.Cells["A1"].Value);
                Assert.Equal(i * 100d, Convert.ToDouble(ws.Cells["A2"].Value));
            }
        }

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            ms.Position = 0;
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(MultipleWorksheets_RoundTrip)}.xlsx");
            await File.WriteAllBytesAsync(filename, ms.ToArray(), CancellationToken.None);
        }
#endif
    }

    [Fact]
    public async Task WorksheetWithStyles_RoundTrip()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Styled");

            ws.Cells["A1"].Value = "Bold";
            ws.Cells["A1"].Style.Font.Bold = true;

            ws.Cells["A2"].Value = "Colored Fill";
            ws.Cells["A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["A2"].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

            ws.Cells["A3"].Value = 12345.678;
            ws.Cells["A3"].Style.Numberformat.Format = "#,##0.00";

            ws.Row(5).Height = 30;
            ws.Column(3).Width = 25;

            await pck.SaveAsAsync(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Styled"];
            Assert.True(ws.Cells["A1"].Style.Font.Bold);
            Assert.Equal(ExcelFillStyle.Solid, ws.Cells["A2"].Style.Fill.PatternType);
            Assert.Equal("#,##0.00", ws.Cells["A3"].Style.Numberformat.Format);
            Assert.Equal(30, ws.Row(5).Height);
            Assert.Equal(25, ws.Column(3).Width);
        }

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            ms.Position = 0;
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(WorksheetWithStyles_RoundTrip)}.xlsx");
            await File.WriteAllBytesAsync(filename, ms.ToArray(), CancellationToken.None);
        }
#endif
    }

    #endregion

    #region LoadFromText

    [Fact]
    public void LoadFromText_BasicCsv()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CSV");

        var csv = "Name,Age,City\nAlice,30,New York\nBob,25,London";
        ws.Cells["A1"].LoadFromText(csv, new ExcelTextFormat
        {
            Delimiter = ',',
            EOL = "\n"
        });

        Assert.Equal("Name", ws.Cells["A1"].Value);
        Assert.Equal("Age", ws.Cells["B1"].Value);
        Assert.Equal("Alice", ws.Cells["A2"].Value);
        Assert.Equal("Bob", ws.Cells["A3"].Value);
    }

    [Fact]
    public void LoadFromText_WithQuotedFields()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CSV");

        var csv = "\"Name\",\"Value\"\r\n\"Item, with comma\",\"100\"";
        ws.Cells["A1"].LoadFromText(csv, new ExcelTextFormat
        {
            Delimiter = ',',
            TextQualifier = '"'
        });

        Assert.Equal("Name", ws.Cells["A1"].Value);
        Assert.Equal("Value", ws.Cells["B1"].Value);
        Assert.Equal("Item, with comma", ws.Cells["A2"].Value);
        Assert.Equal("100", ws.Cells["B2"].Value);
    }

    #endregion

    #region Culture-Specific Date Handling

    [Fact]
    public void DateFunctions_WorkWithUSCulture()
    {
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Sheet1");

            var testDate = new DateTime(2024, 6, 15);
            ws.Cells["A1"].Value = testDate;
            ws.Cells["A1"].Style.Numberformat.Format = "MM/dd/yyyy";

            var text = ws.Cells["A1"].Text;
            Assert.Equal("06/15/2024", text);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void DateFunctions_WorkWithGBCulture()
    {
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Sheet1");

            var testDate = new DateTime(2024, 6, 15);
            ws.Cells["A1"].Value = testDate;
            ws.Cells["A1"].Style.Numberformat.Format = "dd/MM/yyyy";

            var text = ws.Cells["A1"].Text;
            Assert.Equal("15/06/2024", text);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
    }

    #endregion

    #region Worksheet with Complex Scenarios

    [Fact]
    public async Task ComplexWorksheet_WithTablesAndFormulas()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sales");

            // Headers
            ws.Cells["A1"].Value = "Product";
            ws.Cells["B1"].Value = "Quantity";
            ws.Cells["C1"].Value = "Price";
            ws.Cells["D1"].Value = "Total";

            // Data
            string[] products = { "Widget", "Gadget", "Doohickey" };
            int[] quantities = { 100, 200, 150 };
            double[] prices = { 9.99, 14.99, 4.99 };

            for (var i = 0; i < products.Length; i++)
            {
                var row = i + 2;
                ws.Cells[row, 1].Value = products[i];
                ws.Cells[row, 2].Value = quantities[i];
                ws.Cells[row, 3].Value = prices[i];
                ws.Cells[row, 4].Formula = $"B{row}*C{row}";
            }

            // Summary
            ws.Cells["A5"].Value = "Total";
            ws.Cells["D5"].Formula = "SUM(D2:D4)";
            ws.Cells["D5"].Style.Font.Bold = true;

            // Table
                var table = ws.Tables.Add(ws.Cells["A1:D4"], "SalesTable");

            // Named range
            ws.Names.Add("TotalSales", ws.Cells["D5"]);

            await pck.SaveAsAsync(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["Sales"];
            Assert.Equal(1, ws.Tables.Count);
            Assert.Equal("SalesTable", ws.Tables[0].Name);
            Assert.Equal("Widget", ws.Cells["A2"].Value);
            Assert.Equal("SUM(D2:D4)", ws.Cells["D5"].Formula);
            Assert.Equal(1, ws.Names.Count);
        }

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            ms.Position = 0;
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ComplexWorksheet_WithTablesAndFormulas)}.xlsx");
            await File.WriteAllBytesAsync(filename, ms.ToArray(), CancellationToken.None);
        }
#endif
    }

    [Fact]
    public async Task WorksheetCopy_PreservesAllProperties()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Source");
            ws.TabColor = Color.Red;
            ws.DefaultRowHeight = 20;
            ws.View.ShowGridLines = false;

            ws.Cells["A1"].Value = "Header";
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A2"].Value = 42;
            ws.Cells["A3"].Formula = "A2*2";
            ws.Cells["B1:D1"].Merge = true;

            ws.Row(1).Height = 25;
            ws.Column(2).Width = 15;

            var copy = pck.Workbook.Worksheets.Add("CopiedSheet", ws);

            Assert.Equal("Header", copy.Cells["A1"].Value);
            Assert.Equal(42, copy.Cells["A2"].Value);
            Assert.Equal("A2*2", copy.Cells["A3"].Formula);
            Assert.True(copy.Cells["B1:D1"].Merge);

            await pck.SaveAsAsync(ms);
        }

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            ms.Position = 0;
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(WorksheetCopy_PreservesAllProperties)}.xlsx");
            await File.WriteAllBytesAsync(filename, ms.ToArray(), CancellationToken.None);
        }
#endif
    }

    #endregion

    #region RichText

    [Fact]
    public void RichText_AddMultipleItems()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("RichText");

        var rs = ws.Cells["A1"].RichText;
        var r1 = rs.Add("Test");
        r1.Bold = true;

        var r2 = rs.Add(" of");
        r2.Size = 14;
        r2.Italic = true;

        var r3 = rs.Add(" rich");
        r3.FontName = "Arial";
        r3.Size = 18;
        r3.Italic = true;

        var r4 = rs.Add("text.");
        r4.Size = 8.25f;
        r4.Italic = true;
        r4.UnderLine = true;

        Assert.Equal(4, rs.Count);
        Assert.True(rs[0].Bold);
        Assert.Equal(14, rs[1].Size);
        Assert.Equal("Arial", rs[2].FontName);
        Assert.True(rs[3].UnderLine);
    }

    [Fact]
    public void RichText_InsertAtPosition()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("RichText");

        var rs = ws.Cells["A1"].RichText;
        rs.Add("First");
        rs.Add("Third");

        var rIns = rs.Insert(1, " inserted");
        rIns.Bold = true;

        Assert.Equal(3, rs.Count);
        Assert.Equal(" inserted", rs[1].Text);
        Assert.True(rs[1].Bold);
    }

    [Fact]
    public void RichText_AmpersandEscaping()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("RichText");

        ws.Cells["G1"].RichText.Add("Room 02 & 03");

        Assert.Equal("Room 02 & 03", ws.Cells["G1"].RichText.Text);
    }

    [Fact]
    public void RichText_TextPropertySetsAll()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("RichText");

        ws.Cells["A1"].RichText.Text = "Room 02 & 03";

        Assert.Equal("Room 02 & 03", ws.Cells["A1"].RichText.Text);
    }

    [Fact]
    public void RichText_RangeApplied()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("RichText");

        var rs = ws.Cells["A3:A4"].RichText;
        var r1 = rs.Add("Double");
        r1.FontName = "times new roman";
        r1.Size = 16;

        var r2 = rs.Add(" cells");
        r2.UnderLine = true;

        Assert.Equal(2, rs.Count);
        Assert.Equal("times new roman", rs[0].FontName);
        Assert.True(rs[1].UnderLine);
    }

    [Fact]
    public void RichText_RoundTrips()
    {
        using var ms = new MemoryStream();
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("RichText");
            var r1 = ws.Cells["A1"].RichText.Add("Bold ");
            r1.Bold = true;
            var r2 = ws.Cells["A1"].RichText.Add("Normal");
            r2.Bold = false;
            pck.SaveAs(ms);
        }

        ms.Position = 0;
        using (var pck = new ExcelPackage(ms))
        {
            var ws = pck.Workbook.Worksheets["RichText"];
            Assert.Equal(2, ws.Cells["A1"].RichText.Count);
            Assert.True(ws.Cells["A1"].RichText[0].Bold);
            Assert.False(ws.Cells["A1"].RichText[1].Bold);
        }
    }

    #endregion

    #region LoadFromArrays

    [Fact]
    public void LoadFromArrays_LoadsCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Arrays");

        var testArray = new List<object[]>
        {
            new object[] { 3, 4, 5, 6 },
            new object[] { "Test1", "test", "5", "6" }
        };

        ws.Cells["A1"].LoadFromArrays(testArray);

        Assert.Equal(3, ws.Cells["A1"].Value);
        Assert.Equal(6, ws.Cells["D1"].Value);
        Assert.Equal("Test1", ws.Cells["A2"].Value);
        Assert.Equal("6", ws.Cells["D2"].Value);
    }

    [Fact]
    public void LoadFromArrays_JaggedArrays()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Arrays");

        var testArray = new List<object[]>
        {
            new object[] { 1, 2, 3 },
            new object[] { 4, 5 }
        };

        var result = ws.Cells["A1"].LoadFromArrays(testArray);

        Assert.NotNull(result);
        Assert.Equal(1, ws.Cells["A1"].Value);
        Assert.Equal(3, ws.Cells["C1"].Value);
        Assert.Equal(4, ws.Cells["A2"].Value);
        Assert.Equal(5, ws.Cells["B2"].Value);
    }

    #endregion

    #region Array Formulas

    [Fact]
    public void ArrayFormula_CanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = 1;
        ws.Cells["B1"].Value = 2;

        ws.Cells["C1"].CreateArrayFormula("A1+B1");

        Assert.NotEmpty(ws.Cells["C1"].Formula);
    }

    [Fact]
    public void ArrayFormula_RangeFormula()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A50:A59"].CreateArrayFormula("C50+D50");

        Assert.NotEmpty(ws.Cells["A50"].Formula);
    }

    #endregion

    #region Formula Overwrite

    [Fact]
    public void FormulaOverwrite_InsideRange()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("FormulaOverwrite");

        ws.Cells["A1:G12"].Formula = "B1+C1";
        ws.Cells["B2:C3"].Formula = "G2+E1";

        // The inner range should have the overwritten formula
        Assert.Equal("G2+E1", ws.Cells["B2"].Formula);
    }

    [Fact]
    public void FormulaOverwrite_SameRange()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("FormulaOverwrite");

        ws.Cells["F1:F3"].Formula = "F2+F3";
        ws.Cells["F1:F3"].Formula = "F5+F6";

        Assert.Equal("F5+F6", ws.Cells["F1"].Formula);
    }

    [Fact]
    public void FormulaOverwrite_ValueOverwritesFormula()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("FormulaOverwrite");

        ws.Cells["A1:G12"].Formula = "B1+C1";
        ws.Cells["A1"].Value = "test";

        Assert.Equal("test", ws.Cells["A1"].Value);
    }

    #endregion

    #region Formula Error Cases

    [Fact]
    public void FormulaError_CountIf()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("FormulaError");

        ws.Cells["D5"].Formula = "COUNTIF(A1:A100,\"Miss\")";

        Assert.Equal("COUNTIF(A1:A100,\"Miss\")", ws.Cells["D5"].Formula);
    }

    [Fact]
    public void FormulaR1C1_WithSheetName()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet-RC1");

        ws.Cells["A4"].FormulaR1C1 = "+ROUNDUP('Sheet-RC1'!RC[1]/10,0)*10";

        Assert.NotEmpty(ws.Cells["A4"].FormulaR1C1);
    }

    [Fact]
    public void FormulaR1C1_SimpleExpression()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("FormulaError");

        ws.Cells["A4"].FormulaR1C1 = "+ROUNDUP(RC[1]/10,0)*10";

        Assert.NotEmpty(ws.Cells["A4"].FormulaR1C1);
    }

    #endregion

    #region CopyOverwrite

    [Fact]
    public void CopyOverwrite_DataShiftsCorrectly()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("CopyOverwrite");

        for (var col = 1; col < 15; col++)
        for (var row = 1; row < 30; row++)
            ws.SetValue(row, col, $"cell {ExcelCellBase.GetAddress(row, col)}");

        ws.Cells["A1:P30"].Copy(ws.Cells["B1"]);

        // After copying A1:P30 to B1, the value at B1 should be the original A1 value
        Assert.Equal("cell A1", ws.Cells["B1"].Value);
    }

    #endregion

    #region Range Clear

    [Fact]
    public void RangeClear_DoesNotClearSurroundingCells()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("test");

        ws.Cells[2, 2].Value = "something";
        ws.Cells[2, 3].Value = "something";
        ws.Cells[2, 3].Clear();

        Assert.NotNull(ws.Cells[2, 2].Value);
        Assert.Equal("something", ws.Cells[2, 2].Value);
        Assert.Null(ws.Cells[2, 3].Value);
    }

    #endregion

    #region Table Advanced

    [Fact]
    public void Table_ShowTotal()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["B1"].Value = "Header1";
        ws.Cells["C1"].Value = "Header2";
        ws.Cells["B2"].Value = 1;
        ws.Cells["C2"].Value = 3;
        ws.Cells["B3"].Value = 2;
        ws.Cells["C3"].Value = 4;

        var table = ws.Tables.Add(ws.Cells["B1:C3"], "TestTable");
        table.ShowTotal = true;
        table.Columns[0].TotalsRowFunction = RowFunctions.Sum;
        table.Columns[1].TotalsRowFunction = RowFunctions.Sum;

        Assert.True(table.ShowTotal);
    }

    [Fact]
    public void Table_TotalsRowFormula()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["B1"].Value = 123;

        var tbl = ws.Tables.Add(ws.Cells["B1:P12"], "TestTable");
        tbl.ShowTotal = true;
        tbl.Columns[9].TotalsRowFormula = $"SUM([{tbl.Columns[9].Name}])";

        Assert.NotNull(tbl.Columns[9].TotalsRowFormula);
    }

    [Fact]
    public void Table_CalculatedColumnFormula()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["B1"].Value = 123;

        var tbl = ws.Tables.Add(ws.Cells["B1:P12"], "TestTable");
        tbl.Columns[14].CalculatedColumnFormula = "TestTable[[#This Row],[123]]+TestTable[[#This Row],[Column2]]";

        Assert.NotNull(tbl.Columns[14].CalculatedColumnFormula);
    }

    [Fact]
    public void Table_ShowFilter_DefaultIsTrue()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Col1";
        ws.Cells["A2"].Value = 1;

        var tbl = ws.Tables.Add(ws.Cells["A1:A2"], "TestTable");

        Assert.True(tbl.ShowFilter);
    }

    [Fact]
    public void Table_ShowFilter_CanBeDisabled()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = "Col1";
        ws.Cells["A2"].Value = 1;

        var tbl = ws.Tables.Add(ws.Cells["A1:A2"], "TestTable");
        tbl.ShowFilter = false;

        Assert.False(tbl.ShowFilter);
    }

    [Fact]
    public void Table_TotalsRowFunctionEscapesSpecialCharacters()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("TotalsFormulaTest");

        ws.Cells["B1"].Value = "Column1";
        ws.Cells["C1"].Value = "[#'Column'2]";

        var tbl = ws.Tables.Add(ws.Cells["B1:C2"], "TestTable");
        tbl.ShowTotal = true;
        tbl.Columns[1].TotalsRowFunction = RowFunctions.Sum;

        Assert.Equal("SUBTOTAL(109,TestTable['['#''Column''2']])", ws.Cells["C3"].Formula);
    }

    [Fact]
    public void Table_WithSubtotalsParensInColumnName()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Table");

        ws.Cells["B2"].Value = "Header 1";
        ws.Cells["C2"].Value = "Header (2)";
        ws.Cells["B3"].Value = 1;
        ws.Cells["B4"].Value = 2;
        ws.Cells["C3"].Value = 3;
        ws.Cells["C4"].Value = 4;

        var table = ws.Tables.Add(ws.Cells["B2:C4"], "TestTable");
        table.ShowTotal = true;
        table.ShowHeader = true;
        table.Columns[0].TotalsRowFunction = RowFunctions.Sum;
        table.Columns[1].TotalsRowFunction = RowFunctions.Sum;

        ws.Cells["B5"].Calculate();
        Assert.Equal(3.0, ws.Cells["B5"].Value);

        ws.Cells["C5"].Calculate();
        Assert.Equal(7.0, ws.Cells["C5"].Value);
    }

    [Fact]
    public void Table_MultipleTablesAcrossSheets_Delete()
    {
        using var pck = new ExcelPackage();
        var wb = pck.Workbook;
        var sheet1 = wb.Worksheets.Add("WorkSheet A");
        var sheet2 = wb.Worksheets.Add("WorkSheet B");

        for (var i = 1; i <= 4; i++)
        {
            sheet1.Cells[1, i].Value = $"A{i}_";
            sheet2.Cells[1, i].Value = $"A{i}_";
        }

        sheet2.Tables.Add(sheet2.Cells["A1:D73"], "Tablea");
        sheet1.Tables.Add(sheet1.Cells["A1:D73"], "Table2");

        Assert.Equal(1, sheet2.Tables.Count);
        Assert.Equal(1, sheet1.Tables.Count);

        // Can't delete from wrong sheet
        Assert.Throws<ArgumentOutOfRangeException>(() => sheet1.Tables.Delete("Tablea"));

        sheet2.Tables.Delete("Tablea");
        Assert.Equal(0, sheet2.Tables.Count);
    }

    #endregion

    #region Worksheet Delete and Add

    [Fact]
    public void DeleteAndThenAdd_Works()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("NEW1");
        pck.Workbook.Worksheets.Add("NEW2");

        pck.Workbook.Worksheets.Delete(1);
        pck.Workbook.Worksheets.Add("NEW3");

        Assert.Equal(2, pck.Workbook.Worksheets.Count);
    }

    [Fact]
    public void DeleteByName_NonExistentThrows()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("NEW1");
        pck.Workbook.Worksheets.Add("NEW2");

        Assert.Throws<ArgumentException>(() => pck.Workbook.Worksheets.Delete("NEW3"));
    }

    #endregion

    #region Move Worksheet by Position

    [Fact]
    public void MoveBeforeByPosition()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("NEW1");
        pck.Workbook.Worksheets.Add("NEW2");
        pck.Workbook.Worksheets.Add("NEW3");
        pck.Workbook.Worksheets.Add("NEW4");
        pck.Workbook.Worksheets.Add("NEW5");

        pck.Workbook.Worksheets.MoveBefore(3, 1);

        Assert.Equal("NEW4", pck.Workbook.Worksheets[1].Name);
    }

    [Fact]
    public void MoveAfterByPosition()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("NEW1");
        pck.Workbook.Worksheets.Add("NEW2");
        pck.Workbook.Worksheets.Add("NEW3");
        pck.Workbook.Worksheets.Add("NEW4");
        pck.Workbook.Worksheets.Add("NEW5");

        pck.Workbook.Worksheets.MoveAfter(3, 1);

        Assert.Equal("NEW4", pck.Workbook.Worksheets[2].Name);
    }

    [Fact]
    public void MoveWorksheet_OrderPreservedAfterSave()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.Worksheets.Add("NEW1");
        pck.Workbook.Worksheets.Add("NEW2");
        pck.Workbook.Worksheets.Add("NEW3");
        pck.Workbook.Worksheets.Add("NEW4");
        pck.Workbook.Worksheets.Add("NEW5");

        pck.Workbook.Worksheets.MoveBefore("NEW4", "NEW2");

        using var ms = new MemoryStream();
        pck.SaveAs(ms);

        ms.Position = 0;
        using var pck2 = new ExcelPackage(ms);

        var positionId = 0;
        foreach (var worksheet in pck.Workbook.Worksheets)
        {
            Assert.Equal(worksheet.Name, pck2.Workbook.Worksheets[positionId].Name);
            positionId++;
        }
    }

    #endregion

    #region Copy Row/Column with Cells.Copy

    [Fact]
    public void CopyRowWithCellsCopy_SetsOutlineLevels()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Row(2).OutlineLevel = 1;
        ws.Row(3).OutlineLevel = 1;
        ws.Row(4).OutlineLevel = 0;

        // Set outline levels on rows to be copied over
        ws.Row(6).OutlineLevel = 17;
        ws.Row(7).OutlineLevel = 25;
        ws.Row(8).OutlineLevel = 29;

        ws.Cells["2:4"].Copy(ws.Cells["A6"]);

        Assert.Equal(1, ws.Row(2).OutlineLevel);
        Assert.Equal(1, ws.Row(3).OutlineLevel);
        Assert.Equal(0, ws.Row(4).OutlineLevel);
        Assert.Equal(1, ws.Row(6).OutlineLevel);
        Assert.Equal(1, ws.Row(7).OutlineLevel);
        Assert.Equal(0, ws.Row(8).OutlineLevel);
    }

    [Fact]
    public void CopyRowCrossSheet_SetsOutlineLevels()
    {
        using var pck = new ExcelPackage();
        var sheet1 = pck.Workbook.Worksheets.Add("Sheet1");
        var sheet2 = pck.Workbook.Worksheets.Add("Sheet2");

        sheet1.Row(2).OutlineLevel = 1;
        sheet1.Row(3).OutlineLevel = 1;
        sheet1.Row(4).OutlineLevel = 0;

        // Set outline levels on rows to be copied over
        sheet2.Row(6).OutlineLevel = 17;
        sheet2.Row(7).OutlineLevel = 25;
        sheet2.Row(8).OutlineLevel = 29;

        sheet1.Cells["2:4"].Copy(sheet2.Cells["A6"]);

        Assert.Equal(1, sheet1.Row(2).OutlineLevel);
        Assert.Equal(1, sheet1.Row(3).OutlineLevel);
        Assert.Equal(0, sheet1.Row(4).OutlineLevel);
        Assert.Equal(1, sheet2.Row(6).OutlineLevel);
        Assert.Equal(1, sheet2.Row(7).OutlineLevel);
        Assert.Equal(0, sheet2.Row(8).OutlineLevel);
    }

    [Fact]
    public void CopyColumnWithCellsCopy_SetsOutlineLevels()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Column(2).OutlineLevel = 1;
        ws.Column(3).OutlineLevel = 1;
        ws.Column(4).OutlineLevel = 0;

        // Set outline levels on columns to be copied over
        ws.Column(6).OutlineLevel = 17;
        ws.Column(7).OutlineLevel = 25;
        ws.Column(8).OutlineLevel = 29;

        ws.Cells["B:D"].Copy(ws.Cells["F1"]);

        Assert.Equal(1, ws.Column(2).OutlineLevel);
        Assert.Equal(1, ws.Column(3).OutlineLevel);
        Assert.Equal(0, ws.Column(4).OutlineLevel);
        Assert.Equal(1, ws.Column(6).OutlineLevel);
        Assert.Equal(1, ws.Column(7).OutlineLevel);
        Assert.Equal(0, ws.Column(8).OutlineLevel);
    }

    #endregion

    #region Shared Formula Copy Across Sheets

    [Fact]
    public void CopySheetWithSharedFormula_IndependentOfOriginal()
    {
        using var pck = new ExcelPackage();
        var sheet1 = pck.Workbook.Worksheets.Add("Sheet1");

        sheet1.Cells[2, 2, 5, 2].Value = new object[,] { { 1 }, { 2 }, { 3 }, { 4 } };
        sheet1.Cells["D2:D5"].Formula = "SUM(B2:C2)";

        var sheet2 = pck.Workbook.Worksheets.Copy(sheet1.Name, "Sheet2");

        // Inserting a column on sheet1 should modify the shared formula on sheet1, but not sheet2
        sheet1.InsertColumn(3, 1);

        Assert.Equal("SUM(B2:D2)", sheet1.Cells["E2"].Formula);
        Assert.Equal("SUM(B2:C2)", sheet2.Cells["D2"].Formula);
    }

    #endregion

    #region Text Format Codes

    [Fact]
    public void Text_DayFormatCode()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = new DateTime(2018, 2, 3);

        ws.Cells["A1"].Style.Numberformat.Format = "d";
        Assert.Equal("3", ws.Cells["A1"].Text);

        ws.Cells["A1"].Style.Numberformat.Format = "D";
        Assert.Equal("3", ws.Cells["A1"].Text);
    }

    [Fact]
    public void Text_MonthFormatCode()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = new DateTime(2018, 2, 3);

        ws.Cells["A1"].Style.Numberformat.Format = "M";

        Assert.Equal("2", ws.Cells["A1"].Text);
    }

    [Fact]
    public void Text_YearFormatCodes()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        ws.Cells["A1"].Value = new DateTime(2018, 2, 3);

        ws.Cells["A1"].Style.Numberformat.Format = "Y";
        Assert.Equal("18", ws.Cells["A1"].Text);

        ws.Cells["A1"].Style.Numberformat.Format = "YY";
        Assert.Equal("18", ws.Cells["A1"].Text);

        ws.Cells["A1"].Style.Numberformat.Format = "YYY";
        Assert.Equal("2018", ws.Cells["A1"].Text);
    }

    #endregion

    #region Sort

    [Fact]
    public void Sort_BasicAscending()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sort");

        ws.Cells["A1"].Value = 3;
        ws.Cells["A2"].Value = 1;
        ws.Cells["A3"].Value = 2;

        ws.Cells["A1:A3"].Sort(0);

        Assert.Equal(1, ws.Cells["A1"].Value);
        Assert.Equal(2, ws.Cells["A2"].Value);
        Assert.Equal(3, ws.Cells["A3"].Value);
    }

    [Fact]
    public void Sort_Descending()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sort");

        ws.Cells["A1"].Value = 1;
        ws.Cells["A2"].Value = 3;
        ws.Cells["A3"].Value = 2;

        ws.Cells["A1:A3"].Sort(0, true);

        Assert.Equal(3, ws.Cells["A1"].Value);
        Assert.Equal(2, ws.Cells["A2"].Value);
        Assert.Equal(1, ws.Cells["A3"].Value);
    }

    [Fact]
    public void Sort_MultipleColumns()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sort");

        ws.Cells["A1"].Value = 1;
        ws.Cells["B1"].Value = "B";
        ws.Cells["A2"].Value = 2;
        ws.Cells["B2"].Value = "A";
        ws.Cells["A3"].Value = 1;
        ws.Cells["B3"].Value = "A";

        ws.Cells["A1:B3"].Sort(new[] { 0, 1 }, new[] { false, false });

        Assert.Equal(1, ws.Cells["A1"].Value);
        Assert.Equal("A", ws.Cells["B1"].Value);
        Assert.Equal(1, ws.Cells["A2"].Value);
        Assert.Equal("B", ws.Cells["B2"].Value);
        Assert.Equal(2, ws.Cells["A3"].Value);
    }

    #endregion

    #region Style Advanced

    [Fact]
    public void Style_TextRotation()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "Rotated";
        ws.Cells["A1"].Style.TextRotation = 45;

        Assert.Equal(45, ws.Cells["A1"].Style.TextRotation);
    }

    [Fact]
    public void Style_Indent()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "Indent 2";
        ws.Cells["A1"].Style.Indent = 2;

        Assert.Equal(2, ws.Cells["A1"].Style.Indent);
    }

    [Fact]
    public void Style_ShrinkToFit()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = "Shrink";
        ws.Cells["A1"].Style.ShrinkToFit = true;

        Assert.True(ws.Cells["A1"].Style.ShrinkToFit);
    }

    [Fact]
    public void Style_ReadingOrder()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Style.ReadingOrder = ExcelReadingOrder.LeftToRight;
        Assert.Equal(ExcelReadingOrder.LeftToRight, ws.Cells["A1"].Style.ReadingOrder);

        ws.Cells["A2"].Style.ReadingOrder = ExcelReadingOrder.RightToLeft;
        Assert.Equal(ExcelReadingOrder.RightToLeft, ws.Cells["A2"].Style.ReadingOrder);

        ws.Cells["A3"].Style.ReadingOrder = ExcelReadingOrder.ContextDependent;
        Assert.Equal(ExcelReadingOrder.ContextDependent, ws.Cells["A3"].Style.ReadingOrder);
    }

    [Fact]
    public void Style_UnderlineTypes()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["F1"].Style.Font.UnderLineType = ExcelUnderLineType.Single;
        ws.Cells["F2"].Style.Font.UnderLineType = ExcelUnderLineType.Double;
        ws.Cells["F3"].Style.Font.UnderLineType = ExcelUnderLineType.SingleAccounting;
        ws.Cells["F4"].Style.Font.UnderLineType = ExcelUnderLineType.DoubleAccounting;
        ws.Cells["F5"].Style.Font.UnderLineType = ExcelUnderLineType.None;

        Assert.Equal(ExcelUnderLineType.Single, ws.Cells["F1"].Style.Font.UnderLineType);
        Assert.Equal(ExcelUnderLineType.Double, ws.Cells["F2"].Style.Font.UnderLineType);
        Assert.Equal(ExcelUnderLineType.SingleAccounting, ws.Cells["F3"].Style.Font.UnderLineType);
        Assert.Equal(ExcelUnderLineType.DoubleAccounting, ws.Cells["F4"].Style.Font.UnderLineType);
        Assert.Equal(ExcelUnderLineType.None, ws.Cells["F5"].Style.Font.UnderLineType);
    }

    [Fact]
    public void Style_VerticalAlignFont()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.Subscript;
        Assert.Equal(ExcelVerticalAlignmentFont.Subscript, ws.Cells["A1"].Style.Font.VerticalAlign);

        ws.Cells["A2"].Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.Superscript;
        Assert.Equal(ExcelVerticalAlignmentFont.Superscript, ws.Cells["A2"].Style.Font.VerticalAlign);

        ws.Cells["A3"].Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.None;
        Assert.Equal(ExcelVerticalAlignmentFont.None, ws.Cells["A3"].Style.Font.VerticalAlign);
    }

    [Fact]
    public void Style_Locked()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1:B100"].Style.Locked = false;

        Assert.False(ws.Cells["A1"].Style.Locked);
    }

    [Fact]
    public void Style_Hidden()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1:B12"].Style.Hidden = true;

        Assert.True(ws.Cells["A1"].Style.Hidden);
    }

    [Fact]
    public void Style_NamedStyles()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        var namedStyle = pck.Workbook.Styles.CreateNamedStyle("TestStyle");
        namedStyle.Style.Font.Bold = true;

        ws.Cells["A1:C1"].StyleName = "TestStyle";

        Assert.True(ws.Cells["A1"].Style.Font.Bold);
    }

    [Fact]
    public void Style_ClonedNamedStyles()
    {
        using var pck = new ExcelPackage();

        var defaultStyle = pck.Workbook.Styles.CreateNamedStyle("Default");
        defaultStyle.Style.Font.Name = "Arial";
        defaultStyle.Style.Font.Size = 18;
        defaultStyle.Style.Font.UnderLine = true;

        var boldStyle = pck.Workbook.Styles.CreateNamedStyle("Bold", defaultStyle.Style);
        boldStyle.Style.Font.Color.SetColor(Color.Red);

        Assert.Equal("Arial", defaultStyle.Style.Font.Name);
        Assert.Equal(18, defaultStyle.Style.Font.Size);
        Assert.Equal("FFFF0000", boldStyle.Style.Font.Color.Rgb);
    }

    #endregion

    #region Encoding Advanced

    [Fact]
    public void Encoding_SpecialXmlCharacters()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Encoding");

        ws.Cells["A1"].Value = "_x0099_";
        ws.Cells["A2"].Value = " Test \b" + (char)1 + " end\"";
        ws.Cells["A3"].Value = "_x0097_ test_x001D_1234";
        ws.Cells["A4"].Value = "test" + (char)31;

        Assert.Equal("_x0099_", ws.Cells["A1"].Value);
    }

    #endregion

    #region Address Multi-Range

    [Fact]
    public void Address_MultiRangeSetValue()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Address");

        ws.Cells["A1:A4,B5:B7"].Value = "AddressTest";

        Assert.Equal("AddressTest", ws.Cells["A1"].Value);
        Assert.Equal("AddressTest", ws.Cells["B5"].Value);
    }

    [Fact]
    public void Address_MultiRangeFormulaR1C1()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Address");

        ws.Cells["C4:G4,H8:H30,B15"].FormulaR1C1 = "RC[-1]+R1C[-1]";

        Assert.NotEmpty(ws.Cells["C4"].FormulaR1C1);
    }

    #endregion

    #region Printer Settings Advanced

    [Fact]
    public void PrinterSettings_BlackAndWhite()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.PrinterSettings.BlackAndWhite = true;

        Assert.True(ws.PrinterSettings.BlackAndWhite);
    }

    [Fact]
    public void PrinterSettings_Draft()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.PrinterSettings.Draft = true;

        Assert.True(ws.PrinterSettings.Draft);
    }

    [Fact]
    public void PrinterSettings_HorizontalCentered()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.PrinterSettings.HorizontalCentered = true;
        ws.PrinterSettings.VerticalCentered = true;

        Assert.True(ws.PrinterSettings.HorizontalCentered);
        Assert.True(ws.PrinterSettings.VerticalCentered);
    }

    [Fact]
    public void PrinterSettings_Margins()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.PrinterSettings.TopMargin = 1M;
        ws.PrinterSettings.LeftMargin = 1M;
        ws.PrinterSettings.BottomMargin = 1M;
        ws.PrinterSettings.RightMargin = 1M;

        Assert.Equal(1M, ws.PrinterSettings.TopMargin);
        Assert.Equal(1M, ws.PrinterSettings.LeftMargin);
        Assert.Equal(1M, ws.PrinterSettings.BottomMargin);
        Assert.Equal(1M, ws.PrinterSettings.RightMargin);
    }

    [Fact]
    public void PrinterSettings_PaperSize()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.PrinterSettings.PaperSize = ePaperSize.A4;

        Assert.Equal(ePaperSize.A4, ws.PrinterSettings.PaperSize);
    }

    [Fact]
    public void PrinterSettings_ShowGridLinesAndHeaders()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.PrinterSettings.ShowGridLines = true;
        ws.PrinterSettings.ShowHeaders = true;

        Assert.True(ws.PrinterSettings.ShowGridLines);
        Assert.True(ws.PrinterSettings.ShowHeaders);
    }

    [Fact]
    public void PageBreak_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Row(15).PageBreak = true;
        ws.Column(3).PageBreak = true;

        Assert.True(ws.Row(15).PageBreak);
        Assert.True(ws.Column(3).PageBreak);
    }

    #endregion

    #region Named Range Advanced

    [Fact]
    public void NamedRange_AddValue()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Names");

        ws.Names.AddValue("Value", 5);

        Assert.NotNull(ws.Names["Value"]);
    }

    [Fact]
    public void NamedRange_AddFormula()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Names");

        ws.Cells["A2"].Value = 10;
        ws.Cells["A3"].Value = 20;
        ws.Names.AddFormula("Formula", "Names!A2+Names!A3");

        Assert.NotNull(ws.Names["Formula"]);
    }

    [Fact]
    public void NamedRange_FullRow()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Names");

        ws.Names.Add("FullRow", ws.Cells["2:2"]);
        ws.Names.Add("FullCol", ws.Cells["A:A"]);

        Assert.Equal(1, ws.Names["FullCol"].Start.Row);
        Assert.Equal(ExcelPackage.MaxRows, ws.Names["FullCol"].End.Row);
    }

    #endregion

    #region LoadFromText Advanced

    [Fact]
    public void LoadFromText_EolInQuotedField()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Loaded Text");

        ws.Cells["A1"].LoadFromText("\"text with eol,\r\n in a cell\",\"other value\"",
            new ExcelTextFormat { TextQualifier = '"', EOL = ",\r\n", Delimiter = ',' });

        Assert.NotNull(ws.Cells["A1"].Value);
    }

    [Fact]
    public void LoadFromText_UnclosedDelimiter_Throws()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Loaded Text");

        Assert.ThrowsAny<Exception>(() =>
            ws.Cells["A1"].LoadFromText("\"text with eol,\r\n",
                new ExcelTextFormat { TextQualifier = '"', EOL = ",\r\n", Delimiter = ',' }));
    }

    #endregion

    #region Positive/Negative Infinity

    [Fact]
    public void PositiveInfinity_CanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells[3, 1].Value = double.PositiveInfinity;
        ws.Cells[3, 2].Value = double.NegativeInfinity;

        Assert.Equal(double.PositiveInfinity, ws.Cells[3, 1].Value);
        Assert.Equal(double.NegativeInfinity, ws.Cells[3, 2].Value);
    }

    #endregion

    #region CalcMode

    [Fact]
    public void CalcMode_CanBeSetToAutomatic()
    {
        using var pck = new ExcelPackage();
        pck.Workbook.CalcMode = ExcelCalcMode.Automatic;

        Assert.Equal(ExcelCalcMode.Automatic, pck.Workbook.CalcMode);
    }

    #endregion

    #region TimeSpan Values

    [Fact]
    public void TimeSpan_StoredAndFormatted()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["A1"].Value = new TimeSpan(9, 30, 30);
        ws.Cells["A1"].Style.Numberformat.Format = "hh:mm:ss";

        Assert.NotNull(ws.Cells["A1"].Value);
    }

    #endregion

    #region Workbook Properties

    [Fact]
    public void WorkbookProperties_CanBeSet()
    {
        using var pck = new ExcelPackage();

        pck.Workbook.Properties.Author = "Test Author";
        pck.Workbook.Properties.Title = "Test Title";
        pck.Workbook.Properties.Subject = "Test Subject";
        pck.Workbook.Properties.Category = "Test Category";
        pck.Workbook.Properties.Keywords = "Keywords";
        pck.Workbook.Properties.Comments = "Comments";
        pck.Workbook.Properties.Company = "Test Company";
        pck.Workbook.Properties.Status = "Status";
        pck.Workbook.Properties.Manager = "Manager";

        Assert.Equal("Test Author", pck.Workbook.Properties.Author);
        Assert.Equal("Test Title", pck.Workbook.Properties.Title);
        Assert.Equal("Test Subject", pck.Workbook.Properties.Subject);
        Assert.Equal("Test Category", pck.Workbook.Properties.Category);
    }

    [Fact]
    public void WorkbookProperties_CustomProperties()
    {
        using var pck = new ExcelPackage();

        pck.Workbook.Properties.SetCustomPropertyValue("DateTest", new DateTime(2008, 12, 31));
        pck.Workbook.Properties.SetCustomPropertyValue("Author", "Test Author");
        pck.Workbook.Properties.SetCustomPropertyValue("Count", 1);
        pck.Workbook.Properties.SetCustomPropertyValue("IsTested", false);

        Assert.NotNull(pck.Workbook.Properties.GetCustomPropertyValue("DateTest"));
    }

    #endregion

    #region AutoFilter Advanced

    [Fact]
    public void AutoFilter_ToggleOnOff()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Autofilter");

        ws.Cells["A1"].Value = "A1";
        ws.Cells["B1"].Value = "B1";
        ws.Cells["A2"].Value = 1;
        ws.Cells["B2"].Value = 2;

        ws.Cells["A1:B2"].AutoFilter = true;
        ws.Cells["A1:B2"].AutoFilter = false;
        ws.Cells["A1:B2"].AutoFilter = true;

        Assert.True(ws.Cells["A1:B2"].AutoFilter);
    }

    #endregion

    #region Select Multi-Range

    [Fact]
    public void Select_MultiRange()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Select(new ExcelAddress("3:4,E5:F6"));

        Assert.NotNull(ws.View.SelectedRange);
    }

    #endregion

    #region Value Text Format

    [Fact]
    public void ValueText_NegativeNumberFormat()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("TestFormat");

        ws.Cells[1, 1].Value = 25.96;
        ws.Cells[1, 1].Style.Numberformat.Format = "#,##0.00;(#,##0.00)";

        var text = ws.Cells[1, 1].Text;
        Assert.Contains("25", text);
    }

    [Fact]
    public void ValueText_ZeroDisplays()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");

        ws.Cells["E24"].Value = 0;

        Assert.Equal("0", ws.Cells["E24"].Text);
    }

    #endregion
}
