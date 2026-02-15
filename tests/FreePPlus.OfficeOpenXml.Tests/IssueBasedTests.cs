using CodeBrix.Imaging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

/// <summary>
///     Tests inspired by issue-based tests from the original EPPlus project.
///     These exercise specific bug fixes and edge cases.
/// </summary>
public class IssueBasedTests
{
#if SAVE_TEMP_FILES
    public const string TempFolder = @"C:\Temp";
#endif

    #region Number Format and Text Tests

    [Fact]
    public void Issue15041_NumberFormatWithEscapedDots()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Test");
        ws.Cells["A1"].Value = 202100083;
        ws.Cells["A1"].Style.Numberformat.Format = "00\\.00\\.00\\.000\\.0";

        Assert.Equal("02.02.10.008.3", ws.Cells["A1"].Text);
    }

    [Fact]
    public void Issue15031_TimeSpanConversion()
    {
        var d = ConvertUtil.GetValueDouble(new TimeSpan(35, 59, 1));

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Test");
        ws.Cells["A1"].Value = d;
        ws.Cells["A1"].Style.Numberformat.Format = "[t]:mm:ss";

        Assert.NotEqual(0d, d);
    }

    [Fact]
    public void Issue15022_AutoFitColumnsWithNumberFormat()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Test");
        ws.Cells.AutoFitColumns();
        ws.Cells["A1"].Style.Numberformat.Format = "0";
        ws.Cells.AutoFitColumns();
    }

    [Fact]
    public void Issue15212_CurrencyFormatBrazilian()
    {
        var s = "_(\"R$ \"* #,##0.00_);_(\"R$ \"* (#,##0.00);_(\"R$ \"* \"-\"??_);_(@_) )";
        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("StyleBug");
        ws.Cells["A1"].Value = 5698633.64;
        ws.Cells["A1"].Style.Numberformat.Format = s;

        var t = ws.Cells["A1"].Text;
        Assert.NotNull(t);
        Assert.NotEmpty(t);
    }

    [Fact]
    public void Issue15188_DateFormatDisplaysCorrectly()
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("test");
        worksheet.Column(6).Style.Numberformat.Format = "mm/dd/yyyy";
        worksheet.Cells[2, 6].Value = DateTime.Today;

        var text = worksheet.Cells[2, 6].Text;
        Assert.NotNull(text);
        Assert.NotEmpty(text);
    }

    [Fact]
    public void Issue333_DateTextFormat()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("TextBug");
        ws.Cells["A1"].Value = new DateTime(2019, 3, 7);
        ws.Cells["A1"].Style.Numberformat.Format = "mm-dd-yy";

        // Built-in format ID 14 ("mm-dd-yy") always returns the system short date
        // via ToShortDateString(), so the result is locale-dependent.
        var text = ws.Cells["A1"].Text;
        Assert.NotNull(text);
        Assert.NotEmpty(text);

        // Verify a custom date format that does get translated to .NET format
        ws.Cells["A2"].Value = new DateTime(2019, 3, 7);
        ws.Cells["A2"].Style.Numberformat.Format = "yyyy\\-MM\\-dd";
        Assert.Equal("2019-03-07", ws.Cells["A2"].Text);
    }

    #endregion

    #region Cell Value and Copy Tests

    [Fact]
    public void Issue15168_CellValueAssignment()
    {
        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("Test");
        ws.Cells[1, 1].Value = "A1";
        ws.Cells[2, 1].Value = "A2";
        ws.Cells[2, 1].Value = ws.Cells[1, 1].Value;

        Assert.Equal("A1", ws.Cells[1, 1].Value);
        Assert.Equal("A1", ws.Cells[2, 1].Value);
    }

    [Fact]
    public void Issue15377_NullableDoubleValue()
    {
        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("ws1");
        ws.Cells["A1"].Value = (double?)1;

        var v = ws.GetValue(1, 1);
        Assert.NotNull(v);
        Assert.Equal(1d, Convert.ToDouble(v));
    }

    [Fact]
    public void Issue15460WithString_ArrayValueSavesFirstElement()
    {
        var file = new FileInfo(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx"));
        try
        {
            using (var package = new ExcelPackage(file))
            {
                var sheet = package.Workbook.Worksheets.Add("New Sheet");
                sheet.Cells[3, 3].Value = new[] { "value1", "value2", "value3" };
                package.Save();
            }

            using (var package = new ExcelPackage(file))
            {
                var sheet = package.Workbook.Worksheets["New Sheet"];
                Assert.Equal("value1", sheet.Cells[3, 3].Value);
            }
        }
        finally
        {
            if (file.Exists) file.Delete();
        }
    }

    [Fact]
    public void Issue15460WithNull_NullArrayValueSavesEmpty()
    {
        var file = new FileInfo(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx"));
        try
        {
            using (var package = new ExcelPackage(file))
            {
                var sheet = package.Workbook.Worksheets.Add("New Sheet");
                sheet.Cells[3, 3].Value = new[] { null, "value2", "value3" };
                package.Save();
            }

            using (var package = new ExcelPackage(file))
            {
                var sheet = package.Workbook.Worksheets["New Sheet"];
                Assert.Equal(string.Empty, sheet.Cells[3, 3].Value);
            }
        }
        finally
        {
            if (file.Exists) file.Delete();
        }
    }

    [Fact]
    public void Issue15460WithNonStringPrimitive_IntArrayValueSavesFirstElement()
    {
        var file = new FileInfo(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx"));
        try
        {
            using (var package = new ExcelPackage(file))
            {
                var sheet = package.Workbook.Worksheets.Add("New Sheet");
                sheet.Cells[3, 3].Value = new[] { 5, 6, 7 };
                package.Save();
            }

            using (var package = new ExcelPackage(file))
            {
                var sheet = package.Workbook.Worksheets["New Sheet"];
                Assert.Equal(5d, sheet.Cells[3, 3].Value);
            }
        }
        finally
        {
            if (file.Exists) file.Delete();
        }
    }

    #endregion

    #region Merge Tests

    [Fact]
    public void IssueMergedCells_MergeAndUnmerge()
    {
        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("t");
        ws.Cells["A1:A5,C1:C8"].Merge = true;
        ws.Cells["C1:C8"].Merge = false;
        ws.Cells["A1:A8"].Merge = false;
    }

    [Fact]
    public void Issue15179_MergeDeleteAndSetValue()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("MergeDeleteBug");
        ws.Cells["E3:F3"].Merge = true;
        ws.Cells["E3:F3"].Merge = false;
        ws.DeleteRow(2, 6);
        ws.Cells["A1"].Value = 0;

        var s = ws.Cells["A1"].Value.ToString();
        Assert.Equal("0", s);
    }

    #endregion

    #region Formula Tests

    [Fact]
    public void Issue15128_CopyFormula()
    {
        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("t");
        ws.Cells["A1"].Value = 1;
        ws.Cells["B1"].Value = 2;
        ws.Cells["B2"].Formula = "A1+$B$1";
        ws.Cells["C1"].Value = "Test";
        ws.Cells["A1:B2"].Copy(ws.Cells["C1"]);
        ws.Cells["B2"].Copy(ws.Cells["D1"]);

        // Verify the copy didn't throw and cells have values
        Assert.Equal(1, ws.Cells["A1"].Value);
        Assert.Equal(2, ws.Cells["B1"].Value);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(Issue15128_CopyFormula)}.xlsx");
            p.SaveAs(new FileInfo(filename));
        }
#endif
    }

    [Fact]
    public void IssueTranslate_FormulaR1C1RoundTrip()
    {
        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("Trans");
        ws.Cells["A1:A2"].Formula = "IF(1=1, \"A's B C\",\"D\") ";
        var fr = ws.Cells["A1:A2"].FormulaR1C1;
        ws.Cells["A1:A2"].FormulaR1C1 = fr;

        Assert.Equal("IF(1=1,\"A's B C\",\"D\")", ws.Cells["A2"].Formula);
    }

    [Fact]
    public void Issue15455_VLOOKUPCalculation()
    {
        using var pck = new ExcelPackage();
        var sheet1 = pck.Workbook.Worksheets.Add("sheet1");
        var sheet2 = pck.Workbook.Worksheets.Add("Sheet2");

        sheet1.Cells["C2"].Value = 3;
        sheet1.Cells["C3"].Formula = "VLOOKUP(E1, Sheet2!A1:D6, C2, 0)";
        sheet1.Cells["E1"].Value = "d";
        sheet2.Cells["A1"].Value = "d";
        sheet2.Cells["C1"].Value = "dg";

        pck.Workbook.Calculate();
        var c3 = sheet1.Cells["C3"].Value;
        Assert.Equal("dg", c3);
    }

    [Fact]
    public void Issue204_FormulaR1C1CrossSheet()
    {
        using var pack = new ExcelPackage();
        var sheet1 = pack.Workbook.Worksheets.Add("Sheet 1");
        var sheet2 = pack.Workbook.Worksheets.Add("Sheet 2");

        sheet1.Cells[1, 1].Value = 1;
        sheet2.Cells[1, 1].Value = 2;

        var formula = string.Format("'{0}'!R1C1", sheet1.Name);
        var cell = sheet2.Cells[2, 1];
        cell.FormulaR1C1 = formula;

        Assert.Equal(formula.ToUpper(), cell.FormulaR1C1.ToUpper());
    }

    [Fact]
    public void Issue348_FormulaR1C1WithFullColumnReference()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("S1");
        string formula = "VLOOKUP(C2,A:B,1,0)";
        ws.Cells[2, 4].Formula = formula;

        var t1 = ws.Cells[2, 4].FormulaR1C1;
        Assert.NotNull(t1);
        Assert.NotEmpty(t1);
    }

    #endregion

    #region SUMIFS Tests

    [Fact]
    public void Issue15548_SumIfsShouldHandleGaps()
    {
        using var package = new ExcelPackage();
        var test = package.Workbook.Worksheets.Add("Test");
        test.Cells["A1"].Value = 1;
        test.Cells["B1"].Value = "A";
        // test.Cells["A2"] is default (gap)
        test.Cells["B2"].Value = "A";
        test.Cells["A3"].Value = 1;
        test.Cells["B4"].Value = "B";
        test.Cells["D2"].Formula = "SUMIFS(A1:A3,B1:B3,\"A\")";

        test.Calculate();
        var result = test.Cells["D2"].GetValue<double>();
        Assert.Equal(1d, result);
    }

    [Fact]
    public void Issue15548_SumIfsShouldHandleBadData()
    {
        using var package = new ExcelPackage();
        var test = package.Workbook.Worksheets.Add("Test");
        test.Cells["A1"].Value = 1;
        test.Cells["B1"].Value = "A";
        test.Cells["A2"].Value = "Not a number";
        test.Cells["B2"].Value = "A";
        test.Cells["A3"].Value = 1;
        test.Cells["B4"].Value = "B";
        test.Cells["D2"].Formula = "SUMIFS(A1:A3,B1:B3,\"A\")";

        test.Calculate();
        var result = test.Cells["D2"].GetValue<double>();
        Assert.Equal(1d, result);
    }

    #endregion

    #region Array Formula Tests

    [Fact]
    public void Issue_15641_ArrayFormulaCopy()
    {
        using var ep = new ExcelPackage();
        var sheet = ep.Workbook.Worksheets.Add("A Sheet");
        sheet.Cells[1, 1].CreateArrayFormula("IF(SUM(B1:J1)>0,SUM(B2:J2))");
        sheet.Cells[2, 1].Value = sheet.Cells[1, 1].IsArrayFormula;
        sheet.Cells[1, 1].Copy(sheet.Cells[3, 1]);

        Assert.True(sheet.Cells[1, 1].IsArrayFormula);
        Assert.True(sheet.Cells[3, 1].IsArrayFormula);
    }

    [Fact]
    public void Issue63_ArrayFormulaPersistence()
    {
        var newFile = new FileInfo(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx"));
        try
        {
            using (var package = new ExcelPackage(newFile))
            {
                var ws = package.Workbook.Worksheets.Add("ArrayTest");
                ws.Cells["A1"].Value = 1;
                ws.Cells["A2"].Value = 2;
                ws.Cells["A3"].Value = 3;
                ws.Cells["B1:B3"].CreateArrayFormula("A1:A3");
                package.Save();
            }

            Assert.True(File.Exists(newFile.FullName));

            using (var package = new ExcelPackage(newFile))
            {
                Assert.Equal("A1:A3", package.Workbook.Worksheets["ArrayTest"].Cells["B1"].Formula);
                Assert.True(package.Workbook.Worksheets["ArrayTest"].Cells["B1"].IsArrayFormula);
            }
        }
        finally
        {
            File.Delete(newFile.FullName);
        }
    }

    #endregion

    #region Style Tests

    [Fact]
    public void Issue15141_BorderAroundWithColumn()
    {
        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Test");
        sheet.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        sheet.Cells.Style.Fill.BackgroundColor.SetColor(Color.White);
        sheet.Cells[1, 1, 1, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        sheet.Cells[1, 5, 2, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        var column = sheet.Column(3);
        Assert.NotNull(column);
    }

    [Fact]
    public void Issue15438_FontColorIndexedLookup()
    {
        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("Test");
        var c = ws.Cells["A1"].Style.Font.Color;
        c.Indexed = 3;

        Assert.Equal("#FF00FF00", c.LookupColor(c));
    }

    [Fact]
    public void Issue195_NamedStyleInheritance()
    {
        using var pkg = new ExcelPackage();

        var defaultStyle = pkg.Workbook.Styles.CreateNamedStyle("Default");
        defaultStyle.Style.Font.Name = "Arial";
        defaultStyle.Style.Font.Size = 18;
        defaultStyle.Style.Font.UnderLine = true;

        var boldStyle = pkg.Workbook.Styles.CreateNamedStyle("Bold", defaultStyle.Style);
        boldStyle.Style.Font.Color.SetColor(Color.Red);

        Assert.Equal("Arial", defaultStyle.Style.Font.Name);
        Assert.Equal(18, defaultStyle.Style.Font.Size);
        Assert.Equal("Arial", boldStyle.Style.Font.Name);
        Assert.Equal(18, boldStyle.Style.Font.Size);
        Assert.Equal("FFFF0000", boldStyle.Style.Font.Color.Rgb);
    }

    [Fact]
    public void Issue15397_ColumnStyleWithInsert()
    {
        using var p = new ExcelPackage();
        var workSheet = p.Workbook.Worksheets.Add("styleerror");

        workSheet.Cells["F:G"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        workSheet.Cells["F:G"].Style.Fill.BackgroundColor.SetColor(Color.Red);

        workSheet.Cells["A:A,C:C"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        workSheet.Cells["A:A,C:C"].Style.Fill.BackgroundColor.SetColor(Color.Red);

        workSheet.Cells["A:H"].Style.Font.Color.SetColor(Color.Blue);

        workSheet.Cells["I:I"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        workSheet.Cells["I:I"].Style.Fill.BackgroundColor.SetColor(Color.Red);
        workSheet.Cells["I2"].Style.Fill.BackgroundColor.SetColor(Color.Green);
        workSheet.Cells["I4"].Style.Fill.BackgroundColor.SetColor(Color.Blue);
        workSheet.Cells["I9"].Style.Fill.BackgroundColor.SetColor(Color.Pink);

        workSheet.InsertColumn(2, 2, 9);
        workSheet.Column(45).Width = 0;

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(Issue15397_ColumnStyleWithInsert)}.xlsx");
            p.SaveAs(new FileInfo(filename));
        }
#endif
    }

    #endregion

    #region Load Data Tests

    [Fact]
    public void Issue57_LoadFromArraysEmpty()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("test");

        var result = ws.Cells["A1"].LoadFromArrays(Enumerable.Empty<object[]>());
        Assert.Null(result);
    }

    [Fact]
    public void Issue61_LoadFromDataTable()
    {
        using var table1 = new DataTable("TestTable");
        table1.Columns.Add("name");
        table1.Columns.Add("id");

        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("i61");
        ws.Cells["A1"].LoadFromDataTable(table1, true);

        // Should not throw and headers should be loaded
        Assert.Equal("name", ws.Cells["A1"].Value);
        Assert.Equal("id", ws.Cells["B1"].Value);
    }

    public class Cls1
    {
        public int Prop1 { get; set; }
    }

    public class Cls2 : Cls1
    {
        public string Prop2 { get; set; }
    }

    [Fact]
    public void LoadFromColIssue_InheritedClassProperties()
    {
        var l = new List<Cls1>();
        l.Add(new Cls1 { Prop1 = 1 });
        l.Add(new Cls2 { Prop1 = 1, Prop2 = "test1" });

        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("Test");
        ws.Cells["A1"].LoadFromCollection(l, true, TableStyles.Light16,
            BindingFlags.Instance | BindingFlags.Public,
            new MemberInfo[] { typeof(Cls2).GetProperty("Prop2") });

        // Verify it loaded without exception
        Assert.Equal("Prop2", ws.Cells["A1"].Value);
    }

    #endregion

    #region Worksheet Name and Named Range Tests

    [Fact]
    public void Issue66_SpecialCharsInSheetName()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Test!");
        ws.Cells["A1"].Value = 1;
        ws.Cells["B1"].Formula = "A1";

        var wb = pck.Workbook;
        wb.Names.Add("Name1", ws.Cells["A1:A2"]);
        ws.Names.Add("Name2", ws.Cells["A1"]);

        pck.Save();

        using var pck2 = new ExcelPackage(pck.Stream);
        ws = pck2.Workbook.Worksheets["Test!"];
        Assert.NotNull(ws);
    }

    [Fact]
    public void Issue387_WorkbookAndSheetNamedRanges()
    {
        using var package = new ExcelPackage();
        var workbook = package.Workbook;
        var worksheet = workbook.Worksheets.Add("One");
        worksheet.Cells[1, 3].Value = "Hello";

        var cells = worksheet.Cells["A3"];
        worksheet.Names.Add("R0", cells);
        workbook.Names.Add("Q0", cells);

        Assert.True(worksheet.Names.ContainsKey("R0"));
        Assert.True(workbook.Names.ContainsKey("Q0"));
    }

    #endregion

    #region Stream Tests

    [Fact]
    public void Issue184_DisposingExternalStream()
    {
        var stream = new MemoryStream();
        using (var excelPackage = new ExcelPackage(stream))
        {
            var worksheet = excelPackage.Workbook.Worksheets.Add("Issue 184");
            worksheet.Cells[1, 1].Value = "Hello EPPlus!";
            excelPackage.SaveAs(stream);
        }

        // Disposing ExcelPackage should not dispose the external stream
        Assert.True(stream.Length > 0);
    }

    [Fact]
    public void Issue234_InvalidDataThrowsException()
    {
        using var s = new MemoryStream();
        var data = System.Text.Encoding.UTF8.GetBytes("Bad data").ToArray();
        s.Write(data, 0, data.Length);
        s.Position = 0;

        Assert.Throws<InvalidDataException>(() => new ExcelPackage(s));
    }

    #endregion

    #region Formula Save/Load Tests

    [Fact]
    public void Issue15056_EmptyFormulaSave()
    {
        var file = new FileInfo(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx"));
        try
        {
            using (var ep = new ExcelPackage(file))
            {
                var s = ep.Workbook.Worksheets.Add("test");
                s.Cells["A1:A2"].Formula = "";
                ep.Save();
            }

            Assert.True(file.Exists);
        }
        finally
        {
            if (file.Exists) file.Delete();
        }
    }

    #endregion

    #region AutoFit Tests

    [Fact]
    public void Issue445_AutoFitColumnsLargeText()
    {
        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("AutoFit");
        ws.Cells[1, 1].Value = new string('a', 50000);

        // Should not hang or throw
        ws.Cells[1, 1].AutoFitColumns();
    }

    #endregion

    #region Hyperlink Tests

    [Fact]
    public async Task Issue332_HyperlinkRoundTrip()
    {
        var file = new FileInfo(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx"));
        try
        {
            using (var pkg = new ExcelPackage(file))
            {
                var ws = pkg.Workbook.Worksheets.Add("Hyperlink");
                ws.Cells["A1"].Hyperlink = new ExcelHyperLink("A2", "A2");
                pkg.Save();
            }

            using (var pkg = new ExcelPackage(file))
            {
                var ws = pkg.Workbook.Worksheets["Hyperlink"];
                Assert.NotNull(ws.Cells["A1"].Hyperlink);
            }

#if SAVE_TEMP_FILES
            if (Directory.Exists(TempFolder))
            {
                var destFile = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(Issue332_HyperlinkRoundTrip)}.xlsx");
                File.Copy(file.FullName, destFile);
            }
#endif
        }
        finally
        {
            if (file.Exists) file.Delete();
        }

        await Task.CompletedTask;
    }

    #endregion

    #region RichText Tests

    [Fact]
    public void Issue15374_RichTextInCells()
    {
        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("RT");
        var r = ws.Cells["A1"];
        r.RichText.Text = "Cell 1";
        r["A2"].RichText.Add("Cell 2");

        Assert.NotNull(r.RichText);
        Assert.NotNull(ws.Cells["A2"].RichText);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(Issue15374_RichTextInCells)}.xlsx");
            p.SaveAs(new FileInfo(filename));
        }
#endif
    }

    [Fact]
    public void Issuer14801_RichTextPreserveSpace()
    {
        using var p = new ExcelPackage();
        var workSheet = p.Workbook.Worksheets.Add("rterror");
        var cell = workSheet.Cells["A1"];
        cell.RichText.Add("toto: ");
        cell.RichText[0].PreserveSpace = true;
        cell.RichText[0].Bold = true;
        cell.RichText.Add("tata");
        cell.RichText[1].Bold = false;
        cell.RichText[1].Color = Color.Green;

        Assert.Equal(2, cell.RichText.Count);
        Assert.True(cell.RichText[0].Bold);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(Issuer14801_RichTextPreserveSpace)}.xlsx");
            p.SaveAs(new FileInfo(filename));
        }
#endif
    }

    #endregion

    #region Workbook Worksheet Tests

    [Fact]
    public void Issue15113_MergedCellsWithStyle()
    {
        using var p = new ExcelPackage();
        var ws = p.Workbook.Worksheets.Add("t");
        ws.Cells["A1"].Value = " Performance Update";
        ws.Cells["A1:H1"].Merge = true;
        ws.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
        ws.Cells["A1:H1"].Style.Font.Size = 14;
        ws.Cells["A1:H1"].Style.Font.Color.SetColor(Color.Red);
        ws.Cells["A1:H1"].Style.Font.Bold = true;

        Assert.True(ws.Cells["A1:H1"].Merge);
        Assert.Equal(14, ws.Cells["A1"].Style.Font.Size);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(Issue15113_MergedCellsWithStyle)}.xlsx");
            p.SaveAs(new FileInfo(filename));
        }
#endif
    }

    [Fact]
    public void Issuer15445_ActiveCellAndSelectedRange()
    {
        using var p = new ExcelPackage();
        var ws1 = p.Workbook.Worksheets.Add("ws1");
        var ws2 = p.Workbook.Worksheets.Add("ws2");
        ws2.View.SelectedRange = "A1:B3 D12:D15";
        ws2.View.ActiveCell = "D15";

        Assert.Equal("D15", ws2.View.ActiveCell);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(Issuer15445_ActiveCellAndSelectedRange)}.xlsx");
            p.SaveAs(new FileInfo(filename));
        }
#endif
    }

    [Fact]
    public void Issue241_DefaultRowHeight()
    {
        using var pck = new ExcelPackage();
        var wks = pck.Workbook.Worksheets.Add("test");
        wks.DefaultRowHeight = 35;

        Assert.Equal(35, wks.DefaultRowHeight);
    }

    [Fact]
    public void Issue220_SheetNameWithApostrophe()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Deal's History");
        var a = ws.Cells["A:B"];
        ws.AutoFilterAddress = ws.Cells["A1:C3"];
        pck.Workbook.Names.Add("Test", ws.Cells["B1:D2"]);

        var a2 = new ExcelAddress("'Deal''s History'!a1:a3");
        Assert.Equal("Deal's History", a2.WorkSheet);
    }

    [Fact]
    public void Issue233_InvalidTableNameThrowsException()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Sheet1");
        var tableName = "data!InvalidName";

        Assert.Throws<ArgumentException>(() => ws.Tables.Add(ws.Cells["A1:D4"], tableName));
    }

    #endregion
}
