using System;
using System.IO;
using System.Threading;
using CodeBrix.Imaging;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.FormulaParsing;
using OfficeOpenXml.Style;
using Xunit;

namespace FreePPlus.OfficeOpenXml.Tests;

public class ReadTemplateTests
{
#if SAVE_TEMP_FILES
    public const string TempFolder = @"C:\Temp";
#endif

    #region Blank Stream Tests

    [Fact]
    public void ReadBlankStreamCreatesValidPackage()
    {
        var stream = new MemoryStream();
        using (var pck = new ExcelPackage(stream))
        {
            var ws = pck.Workbook.Worksheets.Add("Perf");
            pck.SaveAs(stream);
        }

        stream.Position = 0;
        using (var pck2 = new ExcelPackage(stream))
        {
            Assert.Single(pck2.Workbook.Worksheets);
            Assert.Equal("Perf", pck2.Workbook.Worksheets[0].Name);
        }

        stream.Close();
    }

    [Fact]
    public void ReadBlankStreamWithMultipleWorksheets()
    {
        var stream = new MemoryStream();
        using (var pck = new ExcelPackage(stream))
        {
            pck.Workbook.Worksheets.Add("Sheet1");
            pck.Workbook.Worksheets.Add("Sheet2");
            pck.Workbook.Worksheets.Add("Sheet3");
            pck.SaveAs(stream);
        }

        stream.Position = 0;
        using (var pck2 = new ExcelPackage(stream))
        {
            Assert.Equal(3, pck2.Workbook.Worksheets.Count);
        }

        stream.Close();
    }

    #endregion

    #region Cell Value Round-Trip Tests

    [Fact]
    public void CellValueSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].Value = "Hello";
            ws.Cells["B1"].Value = 42;
            ws.Cells["C1"].Value = 3.14;
            ws.Cells["D1"].Value = true;
            ws.Cells["E1"].Value = new DateTime(2024, 1, 15);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal("Hello", ws2.Cells["A1"].Value);
        Assert.Equal(42d, ws2.Cells["B1"].Value);
        Assert.Equal(3.14, ws2.Cells["C1"].Value);
        Assert.Equal(true, ws2.Cells["D1"].Value);
    }

    [Fact]
    public void ModifiedCellValueSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["G4"].Value = 12;
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal(12d, ws2.Cells["G4"].Value);
    }

    #endregion

    #region Style Round-Trip Tests

    [Fact]
    public void NamedStyleSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            var style = pck.Workbook.Styles.CreateNamedStyle("CustomStyle");
            style.Style.Font.Bold = true;
            style.Style.Font.Size = 14;
            ws.Cells["A1"].StyleName = "CustomStyle";
            ws.Cells["A1"].Value = "Styled";
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal("CustomStyle", ws2.Cells["A1"].StyleName);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(NamedStyleSurvivesRoundTrip)}.xlsx");
            pck2.SaveAs(new FileInfo(filename));
        }
#endif
    }

    [Fact]
    public void FillStyleSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].Value = "Red Fill";
            ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.Red);
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal(ExcelFillStyle.Solid, ws2.Cells["A1"].Style.Fill.PatternType);
    }

    [Fact]
    public void ColumnStyleSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            var col = ws.Column(1);
            col.Style.Fill.PatternType = ExcelFillStyle.Solid;
            col.Style.Fill.BackgroundColor.SetColor(Color.Red);
            ws.Cells["A1"].Value = "Column styled";
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal(ExcelFillStyle.Solid, ws2.Column(1).Style.Fill.PatternType);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ColumnStyleSurvivesRoundTrip)}.xlsx");
            pck2.SaveAs(new FileInfo(filename));
        }
#endif
    }

    [Fact]
    public void NumberFormatSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].Value = 1234.56;
            ws.Cells["A1"].Style.Numberformat.Format = "0.00";
            ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal("0.00", ws2.Cells["A1"].Style.Numberformat.Format);
        Assert.Equal(ExcelHorizontalAlignment.Right, ws2.Cells["A1"].Style.HorizontalAlignment);
        Assert.Equal(ExcelVerticalAlignment.Center, ws2.Cells["A1"].Style.VerticalAlignment);
    }

    #endregion

    #region RichText Tests

    [Fact]
    public void RichTextWithNewlineSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("test");
            using (var rng = ws.Cells["A1"])
            {
                var rt1 = rng.RichText.Add("TEXT1\r\n");
                rt1.Bold = true;
                rng.Style.WrapText = true;
                var rt2 = rng.RichText.Add("TEXT2");
                rt2.Bold = false;
            }

            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.True(ws2.Cells["A1"].IsRichText);
        Assert.Equal(2, ws2.Cells["A1"].RichText.Count);
        Assert.True(ws2.Cells["A1"].RichText[0].Bold);
        Assert.False(ws2.Cells["A1"].RichText[1].Bold);
        Assert.True(ws2.Cells["A1"].Style.WrapText);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(RichTextWithNewlineSurvivesRoundTrip)}.xlsx");
            pck2.SaveAs(new FileInfo(filename));
        }
#endif
    }

    [Fact]
    public void RichTextPreservesFormattingOnReread()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Sheet1");
            using (var rng = ws.Cells["A1"])
            {
                var rt1 = rng.RichText.Add("Bold Text ");
                rt1.Bold = true;
                rt1.Size = 14;
                var rt2 = rng.RichText.Add("Normal Text");
                rt2.Bold = false;
                rt2.Size = 11;
            }

            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.True(ws2.Cells["A1"].RichText[0].Bold);
        Assert.Equal(14, ws2.Cells["A1"].RichText[0].Size);
        Assert.False(ws2.Cells["A1"].RichText[1].Bold);
        Assert.Equal(11, ws2.Cells["A1"].RichText[1].Size);
    }

    #endregion

    #region Comment Tests

    [Fact]
    public void CommentAddAndRemoveSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Comment");
            ws.Cells["A1"].AddComment("Test comment", "Author");
            ws.Comments.RemoveAt(0);
            ws.Cells["B2"].AddComment("Remaining comment", "Author2");
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Null(ws2.Cells["A1"].Comment);
        Assert.NotNull(ws2.Cells["B2"].Comment);
        Assert.Equal("Remaining comment", ws2.Cells["B2"].Comment.Text);
        Assert.Equal("Author2", ws2.Cells["B2"].Comment.Author);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(CommentAddAndRemoveSurvivesRoundTrip)}.xlsx");
            pck2.SaveAs(new FileInfo(filename));
        }
#endif
    }

    [Fact]
    public void CommentOnlyAddSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Comment");
            ws.Cells["A1"].AddComment("Test comment", "J");
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.NotNull(ws2.Cells["A1"].Comment);
        Assert.Equal("Test comment", ws2.Cells["A1"].Comment.Text);
        Assert.Equal("J", ws2.Cells["A1"].Comment.Author);
    }

    #endregion

    #region Worksheet Protection Tests

    [Fact]
    public void WorksheetProtectionSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Protected");
            ws.Cells["A1"].Value = "Protected content";
            ws.Protection.AllowInsertColumns = true;
            ws.Protection.SetPassword("test");
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.True(ws2.Protection.IsProtected);
        Assert.True(ws2.Protection.AllowInsertColumns);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(WorksheetProtectionSurvivesRoundTrip)}.xlsx");
            pck2.SaveAs(new FileInfo(filename));
        }
#endif
    }

    #endregion

    #region Data Validation Tests

    [Fact]
    public void IntegerDataValidationSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Validation");
            var dv = ws.Cells["A1"].DataValidation.AddIntegerDataValidation();
            dv.Formula.Value = 1;
            dv.Formula2.Value = 100;
            dv.Operator = ExcelDataValidationOperator.between;
            ws.Cells["A1"].Value = 50;
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal(1, ws2.DataValidations.Count);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(IntegerDataValidationSurvivesRoundTrip)}.xlsx");
            pck2.SaveAs(new FileInfo(filename));
        }
#endif
    }

    #endregion

    #region Worksheet Copy Tests

    [Fact]
    public void CopyWorksheetToNewPackage()
    {
        byte[] sourceBytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Source");
            ws.Cells["A1"].Value = "Data1";
            ws.Cells["B1"].Value = "Data2";
            ws.Cells["A2"].Value = 100;
            ws.Cells["B2"].Value = 200;
            sourceBytes = pck.GetAsByteArray();
        }

        using var sourceMs = new MemoryStream(sourceBytes);
        using var sourcePck = new ExcelPackage(sourceMs);
        var sourceWs = sourcePck.Workbook.Worksheets[0];

        using var destPck = new ExcelPackage();
        destPck.Workbook.Worksheets.Add("Copied", sourceWs);

        Assert.Single(destPck.Workbook.Worksheets);
        Assert.Equal("Copied", destPck.Workbook.Worksheets[0].Name);
        Assert.Equal("Data1", destPck.Workbook.Worksheets[0].Cells["A1"].Value);
        Assert.Equal("Data2", destPck.Workbook.Worksheets[0].Cells["B1"].Value);
        Assert.Equal(100d, destPck.Workbook.Worksheets[0].Cells["A2"].Value);
        Assert.Equal(200d, destPck.Workbook.Worksheets[0].Cells["B2"].Value);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(CopyWorksheetToNewPackage)}.xlsx");
            destPck.SaveAs(new FileInfo(filename));
        }
#endif
    }

    #endregion

    #region Dimension and Iteration Tests

    [Fact]
    public void DimensionReflectsPopulatedCells()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Data");
            for (var row = 1; row <= 5; row++)
            for (var col = 1; col <= 3; col++)
                ws.Cells[row, col].Value = $"R{row}C{col}";
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.NotNull(ws2.Dimension);
        Assert.Equal(1, ws2.Dimension.Start.Row);
        Assert.Equal(1, ws2.Dimension.Start.Column);
        Assert.Equal(5, ws2.Dimension.End.Row);
        Assert.Equal(3, ws2.Dimension.End.Column);
    }

    [Fact]
    public void CanIterateAllCellsFromReloadedWorksheet()
    {
        const int rows = 10;
        const int columns = 5;
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Testsheet");
            for (var row = 1; row <= rows; row++)
            for (var col = 1; col <= columns; col++)
                ws.Cells[row, col].Value = $"R{row}C{col}";
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.True(ws2.Dimension.End.Row >= rows);
        Assert.True(ws2.Dimension.End.Column >= columns);

        var lines = new System.Collections.Generic.List<string>();
        var lastRow = ws2.Dimension.End.Row;
        var lastColumn = ws2.Dimension.End.Column;
        var rowCount = 1;
        while (rowCount <= lastRow)
        {
            var columnCount = 1;
            var line = "";
            while (columnCount <= lastColumn)
            {
                line += ws2.Cells[rowCount, columnCount].Value + "|";
                columnCount++;
            }

            lines.Add(line);
            rowCount++;
        }

        Assert.Equal(rows, lines.Count);
        Assert.Contains("R1C1|", lines[0]);
        Assert.Contains("R10C5|", lines[9]);
    }

    #endregion

    #region Create and Read Xlsx Tests

    [Fact]
    public void CreateAndReadXlsxWithFormattedCells()
    {
        const int pRows = 20;
        const int pColumns = 5;

        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Testsheet");

            for (var row = 1; row <= pRows; row++)
            for (var col = 1; col <= pColumns; col++)
            {
                if (col > 1 && row > 2)
                {
                    using (var range = ws.Cells[row, col])
                    {
                        range.Style.Numberformat.Format = "0";
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    ws.Cells[row, col].Value = row * col;
                }
            }

            // Style the first column range starting from row 3
            using (var range = ws.Cells[ExcelCellBase.GetAddress(3, 1, pRows, 1)])
            {
                _ = range.Style;
            }

            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal("Testsheet", ws2.Name);
        Assert.Equal(6d, ws2.Cells[3, 2].Value); // row 3 * col 2
        Assert.Equal(100d, ws2.Cells[20, 5].Value); // row 20 * col 5
        Assert.Equal("0", ws2.Cells[3, 2].Style.Numberformat.Format);
        Assert.Equal(ExcelHorizontalAlignment.Right, ws2.Cells[3, 2].Style.HorizontalAlignment);
        Assert.Equal(ExcelVerticalAlignment.Center, ws2.Cells[3, 2].Style.VerticalAlignment);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(CreateAndReadXlsxWithFormattedCells)}.xlsx");
            pck2.SaveAs(new FileInfo(filename));
        }
#endif
    }

    [Fact]
    public void CreateXlsxFromByteArrayRoundTrip()
    {
        byte[] templateBytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Template");
            ws.Cells["A1"].Value = "Template Data";
            ws.Cells["B1"].Value = 42;
            templateBytes = pck.GetAsByteArray();
        }

        // Read from byte array
        using var ms = new MemoryStream(templateBytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal("Template Data", ws2.Cells["A1"].Value);
        Assert.Equal(42d, ws2.Cells["B1"].Value);
    }

    #endregion

    #region Formula Calculation Tests

    [Fact]
    public void CalculateWithCircularReferenceOption()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Calc");
        ws.Cells["A1"].Value = 10;
        ws.Cells["A2"].Value = 20;
        ws.Cells["A3"].Formula = "SUM(A1:A2)";

        pck.Workbook.Calculate(new ExcelCalculationOption
        {
            AllowCirculareReferences = true
        });

        Assert.Equal(30d, ws.Cells["A3"].Value);
    }

    [Fact]
    public void FormulaSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Formulas");
            ws.Cells["A1"].Value = 5;
            ws.Cells["A2"].Value = 10;
            ws.Cells["A3"].Formula = "SUM(A1:A2)";
            pck.Workbook.Calculate();
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal("SUM(A1:A2)", ws2.Cells["A3"].Formula);
        Assert.Equal(15d, ws2.Cells["A3"].Value);
    }

    #endregion

    #region Threading Tests

    [Fact]
    public void ConcurrentCellWritesProduceValidWorkbook()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Threading");

        const int threadsCount = 10;
        const int rowsPerThread = 100;
        var countdown = new CountdownEvent(threadsCount);

        for (var t = 0; t < threadsCount; t++)
        {
            var startRow = 1 + t * rowsPerThread;
            var thread = new Thread(() =>
            {
                try
                {
                    for (var row = startRow; row < startRow + rowsPerThread; row++)
                    for (var col = 1; col <= 10; col++)
                        lock (ws)
                        {
                            ws.Cells[row, col].Value = row * col;
                        }
                }
                finally
                {
                    countdown.Signal();
                }
            });
            thread.Start();
        }

        countdown.Wait(TimeSpan.FromSeconds(30), CancellationToken.None);

        // Verify data was written
        var bytes = pck.GetAsByteArray();
        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.NotNull(ws2.Dimension);
        Assert.True(ws2.Dimension.End.Row >= threadsCount * rowsPerThread);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ConcurrentCellWritesProduceValidWorkbook)}.xlsx");
            pck2.SaveAs(new FileInfo(filename));
        }
#endif
    }

    #endregion

    #region Large Data Tests

    [Fact]
    public void LargeWorksheetCanBeCreatedAndReread()
    {
        const int rows = 500;
        const int cols = 20;

        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("LargeSheet");
            for (var r = 1; r <= rows; r++)
            for (var c = 1; c <= cols; c++)
                ws.Cells[r, c].Value = r * c;
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal(rows, ws2.Dimension.End.Row);
        Assert.Equal(cols, ws2.Dimension.End.Column);
        Assert.Equal((double)(rows * cols), ws2.Cells[rows, cols].Value);
        Assert.Equal(1d, ws2.Cells[1, 1].Value);
    }

    #endregion

    #region Mixed Content Tests

    [Fact]
    public void WorksheetWithMixedContentSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            var ws = pck.Workbook.Worksheets.Add("Mixed");

            // Values
            ws.Cells["A1"].Value = "Header";
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A2"].Value = 42;
            ws.Cells["A3"].Formula = "A2*2";

            // Comment
            ws.Cells["B1"].Value = "With comment";
            ws.Cells["B1"].AddComment("A comment", "Author");

            // RichText
            using (var rng = ws.Cells["C1"])
            {
                var rt = rng.RichText.Add("Rich ");
                rt.Bold = true;
                var rt2 = rng.RichText.Add("Text");
                rt2.Italic = true;
            }

            // Number format
            ws.Cells["D1"].Value = 0.75;
            ws.Cells["D1"].Style.Numberformat.Format = "0.00%";

            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);
        var ws2 = pck2.Workbook.Worksheets[0];

        Assert.Equal("Header", ws2.Cells["A1"].Value);
        Assert.True(ws2.Cells["A1"].Style.Font.Bold);
        Assert.Equal(42d, ws2.Cells["A2"].Value);
        Assert.Equal("A2*2", ws2.Cells["A3"].Formula);
        Assert.NotNull(ws2.Cells["B1"].Comment);
        Assert.Equal("A comment", ws2.Cells["B1"].Comment.Text);
        Assert.True(ws2.Cells["C1"].IsRichText);
        Assert.Equal("0.00%", ws2.Cells["D1"].Style.Numberformat.Format);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(WorksheetWithMixedContentSurvivesRoundTrip)}.xlsx");
            pck2.SaveAs(new FileInfo(filename));
        }
#endif
    }

    #endregion

    #region Empty and Edge Case Tests

    [Fact]
    public void EmptyWorksheetHasNullDimension()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Empty");

        Assert.Null(ws.Dimension);
    }

    [Fact]
    public void EmptyWorksheetSurvivesRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            pck.Workbook.Worksheets.Add("Empty");
            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);

        Assert.Single(pck2.Workbook.Worksheets);
        Assert.Equal("Empty", pck2.Workbook.Worksheets[0].Name);
        Assert.Null(pck2.Workbook.Worksheets[0].Dimension);
    }

    [Fact]
    public void MultipleWorksheetsSurviveRoundTrip()
    {
        byte[] bytes;
        using (var pck = new ExcelPackage())
        {
            for (var i = 1; i <= 5; i++)
            {
                var ws = pck.Workbook.Worksheets.Add($"Sheet{i}");
                ws.Cells["A1"].Value = $"Data on Sheet{i}";
            }

            bytes = pck.GetAsByteArray();
        }

        using var ms = new MemoryStream(bytes);
        using var pck2 = new ExcelPackage(ms);

        Assert.Equal(5, pck2.Workbook.Worksheets.Count);
        for (var i = 0; i < 5; i++)
        {
            Assert.Equal($"Sheet{i + 1}", pck2.Workbook.Worksheets[i].Name);
            Assert.Equal($"Data on Sheet{i + 1}", pck2.Workbook.Worksheets[i].Cells["A1"].Value);
        }
    }

    #endregion
}
