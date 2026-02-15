using CodeBrix.Imaging;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

#if SAVE_TEMP_FILES
using System.IO;
#endif

namespace FreePPlus.OfficeOpenXml.Tests;

public class DrawingTests
{
#if SAVE_TEMP_FILES
    public const string TempFolder = @"C:\Temp";
#endif

    /// <summary>
    ///     Creates a small test image programmatically using CodeBrix.Imaging.
    /// </summary>
    private static Image CreateTestImage(int width = 100, int height = 50) =>
        ExcelPicture.CreateImage(width, height);
    
    private static void AddTestData(ExcelWorksheet ws)
    {
        ws.Cells["U19"].Value = new DateTime(2009, 12, 31);
        ws.Cells["U20"].Value = new DateTime(2010, 1, 1);
        ws.Cells["U21"].Value = new DateTime(2010, 1, 2);
        ws.Cells["U22"].Value = new DateTime(2010, 1, 3);
        ws.Cells["U23"].Value = new DateTime(2010, 1, 4);
        ws.Cells["U24"].Value = new DateTime(2010, 1, 5);
        ws.Cells["U19:U24"].Style.Numberformat.Format = "yyyy-mm-dd";

        ws.Cells["V19"].Value = 100;
        ws.Cells["V20"].Value = 102;
        ws.Cells["V21"].Value = 101;
        ws.Cells["V22"].Value = 103;
        ws.Cells["V23"].Value = 105;
        ws.Cells["V24"].Value = 104;

        ws.Cells["W19"].Value = 105;
        ws.Cells["W20"].Value = 108;
        ws.Cells["W21"].Value = 104;
        ws.Cells["W22"].Value = 121;
        ws.Cells["W23"].Value = 103;
        ws.Cells["W24"].Value = 109;
    }

    private static void AddTestSeries(ExcelWorksheet ws, ExcelChart chrt)
    {
        AddTestData(ws);
        chrt.Series.Add("'" + ws.Name + "'!V19:V24", "'" + ws.Name + "'!U19:U24");
    }

    #region Bar Chart Tests

    [Fact]
    public async Task BarChartHasCorrectChartType()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("BarChart");

        var chrt = ws.Drawings.AddChart("barChart", eChartType.BarClustered) as ExcelBarChart;
        AddTestSeries(ws, chrt);

        Assert.NotNull(chrt);
        Assert.Equal(eChartType.BarClustered, chrt.ChartType);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(BarChartHasCorrectChartType)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        //Satisfy the compiler that the async method is doing something async, even though in this case it isn't really necessary.
        await Task.CompletedTask;
#endif

    }

    [Fact]
    public async Task BarChartHasCorrectDirection()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("BarChart");

        var chrt = ws.Drawings.AddChart("barChart", eChartType.BarClustered) as ExcelBarChart;
        AddTestSeries(ws, chrt);

        Assert.Equal(eDirection.Bar, chrt!.Direction);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(BarChartHasCorrectDirection)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task BarChartHasCorrectGrouping()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("BarChart");

        var chrt = ws.Drawings.AddChart("barChart", eChartType.BarClustered) as ExcelBarChart;
        AddTestSeries(ws, chrt);

        Assert.Equal(eGrouping.Clustered, chrt!.Grouping);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(BarChartHasCorrectGrouping)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task BarChartHasCorrectShape()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("BarChart");

        var chrt = ws.Drawings.AddChart("barChart", eChartType.BarClustered) as ExcelBarChart;
        AddTestSeries(ws, chrt);

        Assert.Equal(eShape.Box, chrt!.Shape);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(BarChartHasCorrectShape)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task BarChartPropertiesCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("BarChart");

        var chrt = ws.Drawings.AddChart("barChart", eChartType.BarClustered) as ExcelBarChart;
        AddTestSeries(ws, chrt);
        chrt!.VaryColors = true;
        chrt.ShowHiddenData = true;
        chrt.DisplayBlanksAs = eDisplayBlanksAs.Zero;
        chrt.Title.RichText.Text = "Barchart Test";
        chrt.GapWidth = 5;

        Assert.True(chrt.VaryColors);
        Assert.True(chrt.ShowHiddenData);
        Assert.Equal(eDisplayBlanksAs.Zero, chrt.DisplayBlanksAs);
        Assert.Equal(5, chrt.GapWidth);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(BarChartPropertiesCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Pie Chart Tests

    [Fact]
    public async Task PieChartHasCorrectChartType()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("PieChart");

        var chrt = ws.Drawings.AddChart("pieChart", eChartType.Pie) as ExcelPieChart;
        AddTestSeries(ws, chrt);

        Assert.NotNull(chrt);
        Assert.Equal(eChartType.Pie, chrt.ChartType);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(PieChartHasCorrectChartType)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task PieChartHasVaryColors()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("PieChart");

        var chrt = ws.Drawings.AddChart("pieChart", eChartType.Pie) as ExcelPieChart;
        AddTestSeries(ws, chrt);

        Assert.True(chrt!.VaryColors);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(PieChartHasVaryColors)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task PieChart3DHasCorrectChartType()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("PieChart3d");

        var chrt = ws.Drawings.AddChart("pieChart3d", eChartType.Pie3D) as ExcelPieChart;
        AddTestSeries(ws, chrt);

        Assert.NotNull(chrt);
        Assert.Equal(eChartType.Pie3D, chrt.ChartType);
        Assert.True(chrt.VaryColors);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(PieChart3DHasCorrectChartType)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task PieChartDataLabelCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("PieChart");

        var chrt = ws.Drawings.AddChart("pieChart", eChartType.Pie) as ExcelPieChart;
        AddTestSeries(ws, chrt);
        chrt!.DataLabel.ShowPercent = true;
        chrt.Title.Text = "Piechart";

        Assert.Equal("Piechart", chrt.Title.Text);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(PieChartDataLabelCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Line Chart Tests

    [Fact]
    public async Task LineChartHasCorrectChartType()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Line");

        var chrt = ws.Drawings.AddChart("Line1", eChartType.Line) as ExcelLineChart;
        AddTestSeries(ws, chrt);

        Assert.NotNull(chrt);
        Assert.Equal(eChartType.Line, chrt.ChartType);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(LineChartHasCorrectChartType)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task LineChartPropertiesCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Line");

        var chrt = ws.Drawings.AddChart("Line1", eChartType.Line) as ExcelLineChart;
        AddTestSeries(ws, chrt);
        chrt!.VaryColors = true;
        chrt.Smooth = false;
        chrt.Title.Text = "Line Chart";
        chrt.Series[0].Header = "Line serie 1";
        chrt.EditAs = eEditAs.OneCell;
        chrt.DisplayBlanksAs = eDisplayBlanksAs.Span;

        Assert.True(chrt.VaryColors);
        Assert.False(chrt.Smooth);
        Assert.Equal("Line Chart", chrt.Title.Text);
        Assert.Equal("Line serie 1", chrt.Series[0].Header);
        Assert.Equal(eEditAs.OneCell, chrt.EditAs);
        Assert.Equal(eDisplayBlanksAs.Span, chrt.DisplayBlanksAs);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(LineChartPropertiesCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task LineChartTrendLineCanBeAdded()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Line");

        var chrt = ws.Drawings.AddChart("Line1", eChartType.Line) as ExcelLineChart;
        AddTestSeries(ws, chrt);
        var tl = chrt!.Series[0].TrendLines.Add(eTrendLine.Polynomial);
        tl.Name = "Test";
        tl.DisplayRSquaredValue = true;
        tl.DisplayEquation = true;
        tl.Forward = 15;
        tl.Backward = 1;
        tl.Intercept = 6;
        tl.Order = 5;

        Assert.Equal("Test", tl.Name);
        Assert.True(tl.DisplayRSquaredValue);
        Assert.True(tl.DisplayEquation);
        Assert.Equal(15, tl.Forward);
        Assert.Equal(1, tl.Backward);
        Assert.Equal(6, tl.Intercept);
        Assert.Equal(5, tl.Order);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(LineChartTrendLineCanBeAdded)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task LineChartAxisTitleCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Line");

        var chrt = ws.Drawings.AddChart("Line1", eChartType.Line) as ExcelLineChart;
        AddTestSeries(ws, chrt);
        chrt!.Axis[0].Title.Text = "Axis 0";
        chrt.Axis[0].Title.Rotation = 90;
        chrt.Axis[1].Title.Text = "Axis 1";

        Assert.Equal("Axis 0", chrt.Axis[0].Title.Text);
        Assert.Equal("Axis 1", chrt.Axis[1].Title.Text);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(LineChartAxisTitleCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task LineMarkersChartHasCorrectChartType()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("LineMarker");

        var chrt = ws.Drawings.AddChart("Line1", eChartType.LineMarkers) as ExcelLineChart;
        AddTestSeries(ws, chrt);

        Assert.NotNull(chrt);
        Assert.Equal(eChartType.LineMarkers, chrt.ChartType);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(LineMarkersChartHasCorrectChartType)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Scatter Chart Tests

    [Fact]
    public async Task ScatterChartHasCorrectChartType()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Scatter");

        var chrt = ws.Drawings.AddChart("ScatterChart1", eChartType.XYScatterSmoothNoMarkers) as ExcelScatterChart;
        AddTestSeries(ws, chrt);

        Assert.NotNull(chrt);
        Assert.Equal(eChartType.XYScatterSmoothNoMarkers, chrt.ChartType);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ScatterChartHasCorrectChartType)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task ScatterChartTitleAndDataLabelCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Scatter");

        var chrt = ws.Drawings.AddChart("ScatterChart1", eChartType.XYScatterSmoothNoMarkers) as ExcelScatterChart;
        AddTestSeries(ws, chrt);
        var r1 = chrt!.Title.RichText.Add("Header");
        r1.Bold = true;
        var r2 = chrt.Title.RichText.Add(" Text");
        r2.UnderLine = eUnderLineType.WavyHeavy;
        chrt.VaryColors = true;

        var ser = chrt.Series[0] as ExcelScatterChartSeriesItem;
        ser!.DataLabel.Position = eLabelPosition.Center;
        ser.DataLabel.ShowValue = true;
        ser.DataLabel.ShowCategory = true;

        Assert.True(chrt.VaryColors);
        Assert.Equal(eLabelPosition.Center, ser.DataLabel.Position);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ScatterChartTitleAndDataLabelCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task ScatterChartSeriesHeaderCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Scatter");

        var chrt = ws.Drawings.AddChart("ScatterChart1", eChartType.XYScatterSmoothNoMarkers) as ExcelScatterChart;
        AddTestSeries(ws, chrt);
        chrt!.Series[0].Header = "Test series";

        Assert.Equal("Test series", chrt.Series[0].Header);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ScatterChartSeriesHeaderCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Bubble Chart Tests

    [Fact]
    public async Task BubbleChartCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Bubble");

        var chrt = ws.Drawings.AddChart("Bubble", eChartType.Bubble) as ExcelBubbleChart;
        AddTestData(ws);
        chrt!.Series.Add("V19:V24", "U19:U24");

        Assert.NotNull(chrt);
        Assert.Equal(eChartType.Bubble, chrt.ChartType);
        Assert.Equal(1, chrt.Series.Count);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(BubbleChartCanBeCreated)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task Bubble3DChartCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Bubble");

        var chrt = ws.Drawings.AddChart("Bubble3d", eChartType.Bubble3DEffect) as ExcelBubbleChart;
        AddTestData(ws);
        chrt!.Series.Add("V19:V24", "U19:U24", "W19:W24");
        chrt.Style = eChartStyle.Style25;
        chrt.Title.Text = "Header Text";

        Assert.Equal(eChartType.Bubble3DEffect, chrt.ChartType);
        Assert.Equal("Header Text", chrt.Title.Text);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(Bubble3DChartCanBeCreated)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Radar Chart Tests

    [Fact]
    public async Task RadarChartCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Radar");
        AddTestData(ws);

        var chrt = ws.Drawings.AddChart("Radar1", eChartType.Radar) as ExcelRadarChart;
        var s = chrt!.Series.Add("V19:V24", "U19:U24");
        s.Header = "serie1";
        chrt.Title.Text = "Radar Chart 1";

        Assert.NotNull(chrt);
        Assert.Equal("serie1", s.Header);
        Assert.Equal("Radar Chart 1", chrt.Title.Text);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(RadarChartCanBeCreated)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task RadarFilledChartCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Radar");
        AddTestData(ws);

        var chrt = ws.Drawings.AddChart("Radar2", eChartType.RadarFilled) as ExcelRadarChart;
        var s = chrt!.Series.Add("V19:V24", "U19:U24");
        s.Header = "serie1";
        chrt.Title.Text = "Radar Chart 2";

        Assert.NotNull(chrt);
        Assert.Equal(eChartType.RadarFilled, chrt.ChartType);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(RadarFilledChartCanBeCreated)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Surface Chart Tests

    [Fact]
    public async Task SurfaceChartCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Surface");
        AddTestData(ws);

        var chrt = ws.Drawings.AddChart("Surface1", eChartType.Surface) as ExcelSurfaceChart;
        var s = chrt!.Series.Add("V19:V24", "U19:U24");
        var s2 = chrt.Series.Add("W19:W24", "U19:U24");
        s.Header = "serie1";
        chrt.Title.Text = "Surface Chart 1";

        Assert.NotNull(chrt);
        Assert.Equal(eChartType.Surface, chrt.ChartType);
        Assert.Equal(2, chrt.Series.Count);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(SurfaceChartCanBeCreated)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Pyramid Chart Tests

    [Fact]
    public async Task PyramidChartCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Pyramid");

        var chrt = ws.Drawings.AddChart("Pyramid1", eChartType.PyramidCol) as ExcelBarChart;
        AddTestSeries(ws, chrt);
        chrt!.VaryColors = true;
        chrt.Title.Text = "Header Text";
        chrt.DataLabel.ShowValue = true;

        Assert.NotNull(chrt);
        Assert.True(chrt.VaryColors);
        Assert.Equal("Header Text", chrt.Title.Text);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(PyramidChartCanBeCreated)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Cone Chart Tests

    [Fact]
    public async Task ConeChartCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Cone");

        var chrt = ws.Drawings.AddChart("Cone1", eChartType.ConeBarClustered) as ExcelBarChart;
        AddTestSeries(ws, chrt);
        chrt!.VaryColors = true;
        chrt.Title.Text = "Cone bar";
        chrt.Series[0].Header = "Serie 1";
        chrt.Legend.Position = eLegendPosition.Right;
        chrt.Axis[1].DisplayUnit = 100000;

        Assert.NotNull(chrt);
        Assert.Equal(100000, chrt.Axis[1].DisplayUnit);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ConeChartCanBeCreated)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Column Chart Tests

    [Fact]
    public async Task ColumnChartCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Column");

        var chrt = ws.Drawings.AddChart("Column1", eChartType.ColumnClustered3D) as ExcelBarChart;
        AddTestSeries(ws, chrt);
        chrt!.VaryColors = true;
        chrt.View3D.RightAngleAxes = true;
        chrt.View3D.DepthPercent = 99;
        chrt.Title.Text = "Column";
        chrt.Series[0].Header = "Serie 1";
        chrt.EditAs = eEditAs.TwoCell;
        chrt.Axis[1].DisplayUnit = 10020;

        Assert.NotNull(chrt);
        Assert.Equal(10020, chrt.Axis[1].DisplayUnit);
        Assert.Equal(eEditAs.TwoCell, chrt.EditAs);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ColumnChartCanBeCreated)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Doughnut Chart Tests

    [Fact]
    public async Task DoughnutChartCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Dougnut");

        var chrt = ws.Drawings.AddChart("Dougnut1", eChartType.DoughnutExploded) as ExcelDoughnutChart;
        AddTestSeries(ws, chrt);
        chrt!.Title.Text = "Doughnut Exploded";
        chrt.Series[0].Header = "Serie 1";
        chrt.EditAs = eEditAs.Absolute;

        Assert.NotNull(chrt);
        Assert.Equal("Doughnut Exploded", chrt.Title.Text);
        Assert.Equal(eEditAs.Absolute, chrt.EditAs);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(DoughnutChartCanBeCreated)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Shape Tests

    [Fact]
    public async Task ShapeCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Shapes");

        var shape = ws.Drawings.AddShape("shape1", eShapeStyle.Rect);
        shape.SetPosition(100, 100);
        shape.SetSize(300, 300);
        shape.Text = "Rectangle";

        Assert.NotNull(shape);
        Assert.Equal("Rectangle", shape.Text);
        Assert.Equal(eShapeStyle.Rect, shape.Style);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ShapeCanBeCreated)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task ShapeTextAnchoringCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Shapes");

        var shape = ws.Drawings.AddShape("shape1", eShapeStyle.Rect);
        shape.SetPosition(100, 100);
        shape.SetSize(300, 300);
        shape.Text = "Test Shape";
        shape.TextAnchoring = eTextAnchoringType.Top;

        Assert.Equal(eTextAnchoringType.Top, shape.TextAnchoring);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ShapeTextAnchoringCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task ShapeTextVerticalCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Shapes");

        var shape = ws.Drawings.AddShape("shape2", eShapeStyle.Ellipse);
        shape.SetPosition(100, 100);
        shape.SetSize(300, 300);
        shape.Text = "Vertical Text";
        shape.TextVertical = eTextVerticalType.Vertical;

        Assert.Equal(eTextVerticalType.Vertical, shape.TextVertical);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ShapeTextVerticalCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task ShapeTextAlignmentCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Shapes");

        var shape = ws.Drawings.AddShape("shape1", eShapeStyle.Rect);
        shape.SetPosition(100, 100);
        shape.SetSize(300, 300);
        shape.Text = "Right Aligned";
        shape.TextAlignment = eTextAlignment.Right;

        Assert.Equal(eTextAlignment.Right, shape.TextAlignment);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ShapeTextAlignmentCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task ShapeFillCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Shapes");

        var shape = ws.Drawings.AddShape("shape1", eShapeStyle.Rect);
        shape.SetPosition(100, 100);
        shape.SetSize(300, 300);
        shape.Fill.Style = eFillStyle.SolidFill;
        shape.Fill.Transparancy = 50;

        Assert.Equal(eFillStyle.SolidFill, shape.Fill.Style);
        Assert.Equal(50, shape.Fill.Transparancy);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ShapeFillCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task ShapeNoFillCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Shapes");

        var shape = ws.Drawings.AddShape("shape1", eShapeStyle.Rect);
        shape.SetPosition(100, 100);
        shape.SetSize(300, 300);
        shape.Fill.Style = eFillStyle.NoFill;

        Assert.Equal(eFillStyle.NoFill, shape.Fill.Style);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ShapeNoFillCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task ShapeBorderCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Shapes");

        var shape = ws.Drawings.AddShape("shape1", eShapeStyle.Rect);
        shape.SetPosition(100, 100);
        shape.SetSize(300, 300);
        shape.Border.Fill.Style = eFillStyle.SolidFill;
        shape.Border.LineCap = eLineCap.Round;
        shape.Border.LineStyle = eLineStyle.LongDash;

        Assert.Equal(eFillStyle.SolidFill, shape.Border.Fill.Style);
        Assert.Equal(eLineCap.Round, shape.Border.LineCap);
        Assert.Equal(eLineStyle.LongDash, shape.Border.LineStyle);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ShapeBorderCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task ShapeFontPropertiesCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Shapes");

        var shape = ws.Drawings.AddShape("shape1", eShapeStyle.Rect);
        shape.SetPosition(100, 100);
        shape.SetSize(300, 300);
        shape.Font.Bold = true;
        shape.Font.Italic = true;
        shape.Font.LatinFont = "Arial";
        shape.Font.UnderLine = eUnderLineType.Dotted;

        Assert.True(shape.Font.Bold);
        Assert.True(shape.Font.Italic);
        Assert.Equal("Arial", shape.Font.LatinFont);
        Assert.Equal(eUnderLineType.Dotted, shape.Font.UnderLine);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ShapeFontPropertiesCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task ShapeRichTextCanBeAdded()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Shapes");

        var shape = ws.Drawings.AddShape("shape1", eShapeStyle.Rect);
        shape.SetPosition(100, 100);
        shape.SetSize(300, 300);
        shape.Text = "Base text";
        var rt = shape.RichText.Add("Added formatted richtext");
        rt.Bold = true;
        rt.Italic = true;
        rt.Size = 17;

        Assert.True(rt.Bold);
        Assert.True(rt.Italic);
        Assert.Equal(17, rt.Size);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ShapeRichTextCanBeAdded)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task ShapeLockTextCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Shapes");

        var shape = ws.Drawings.AddShape("shape1", eShapeStyle.Rect);
        shape.SetPosition(100, 100);
        shape.SetSize(300, 300);
        shape.LockText = false;

        Assert.False(shape.LockText);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ShapeLockTextCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task ShapeLineEndsCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Shapes");

        var shape = ws.Drawings.AddShape("shape1", eShapeStyle.Line);
        shape.SetPosition(100, 100);
        shape.SetSize(300, 300);
        shape.LineEnds.TailEnd = eEndStyle.Oval;
        shape.LineEnds.TailEndSizeWidth = eEndSize.Large;
        shape.LineEnds.TailEndSizeHeight = eEndSize.Large;
        shape.LineEnds.HeadEnd = eEndStyle.Arrow;
        shape.LineEnds.HeadEndSizeHeight = eEndSize.Small;
        shape.LineEnds.HeadEndSizeWidth = eEndSize.Small;

        Assert.Equal(eEndStyle.Oval, shape.LineEnds.TailEnd);
        Assert.Equal(eEndSize.Large, shape.LineEnds.TailEndSizeWidth);
        Assert.Equal(eEndStyle.Arrow, shape.LineEnds.HeadEnd);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(ShapeLineEndsCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task MultipleShapeStylesCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Shapes");

        var styles = new[] { eShapeStyle.Rect, eShapeStyle.Ellipse, eShapeStyle.Diamond, eShapeStyle.Heart, eShapeStyle.Star5 };
        var y = 100;
        for (var i = 0; i < styles.Length; i++)
        {
            var shape = ws.Drawings.AddShape("shape" + (i + 1), styles[i]);
            shape.SetPosition(y, 100);
            shape.SetSize(300, 300);
            shape.Text = styles[i].ToString();
            y += 400;
        }

        Assert.Equal(styles.Length, ws.Drawings.Count);
        for (var i = 0; i < styles.Length; i++)
        {
            var shape = ws.Drawings[i] as ExcelShape;
            Assert.NotNull(shape);
            Assert.Equal(styles[i], shape.Style);
            Assert.Equal(styles[i].ToString(), shape.Text);
        }

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(MultipleShapeStylesCanBeCreated)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Picture Tests

    [Fact]
    public async Task PictureCanBeAddedWithImage()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Picture");
        using var image = CreateTestImage();

        var pic = ws.Drawings.AddPicture("Pic1", image);

        Assert.NotNull(pic);
        Assert.Equal("Pic1", pic.Name);
        Assert.Equal(1, ws.Drawings.Count);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(PictureCanBeAddedWithImage)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task PicturePositionCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Picture");
        using var image = CreateTestImage();

        var pic = ws.Drawings.AddPicture("Pic1", image);
        pic.SetPosition(150, 200);

        Assert.NotNull(pic);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(PicturePositionCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task PictureSizeCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Picture");
        using var image = CreateTestImage();

        var pic = ws.Drawings.AddPicture("Pic1", image);
        pic.SetPosition(400, 200);
        pic.SetSize(150);

        Assert.NotNull(pic);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(PictureSizeCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task PicturePixelSizeCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Picture");
        using var image = CreateTestImage();

        var pic = ws.Drawings.AddPicture("PicPixelSized", image);
        pic.SetPosition(800, 800);
        pic.SetSize(200, 100);

        Assert.NotNull(pic);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(PicturePixelSizeCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task PictureBorderAndFillCanBeSet()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Picture");
        using var image = CreateTestImage();

        var pic = ws.Drawings.AddPicture("Pic1", image);
        pic.Border.LineStyle = eLineStyle.Solid;
        pic.Fill.Style = eFillStyle.SolidFill;
        pic.Fill.Transparancy = 50;

        Assert.Equal(eLineStyle.Solid, pic.Border.LineStyle);
        Assert.Equal(eFillStyle.SolidFill, pic.Fill.Style);
        Assert.Equal(50, pic.Fill.Transparancy);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(PictureBorderAndFillCanBeSet)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task MultiplePicturesCanBeAdded()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Picture");
        using var image = CreateTestImage();

        ws.Drawings.AddPicture("Pic1", image);
        ws.Drawings.AddPicture("Pic2", image);
        ws.Drawings.AddPicture("Pic3", image);

        Assert.Equal(3, ws.Drawings.Count);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(MultiplePicturesCanBeAdded)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task PictureSizingAndPositioningWithEditAs()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("DrawingPosSize");
        using var image = CreateTestImage();

        var pic1 = ws.Drawings.AddPicture("Pic1", image);
        pic1.SetPosition(1, 0, 1, 0);

        var pic2 = ws.Drawings.AddPicture("Pic2", image);
        pic2.EditAs = eEditAs.Absolute;
        pic2.SetPosition(10, 5, 1, 4);

        var pic3 = ws.Drawings.AddPicture("Pic3", image);
        pic3.EditAs = eEditAs.TwoCell;
        pic3.SetPosition(20, 5, 2, 4);

        Assert.Equal(eEditAs.Absolute, pic2.EditAs);
        Assert.Equal(eEditAs.TwoCell, pic3.EditAs);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(PictureSizingAndPositioningWithEditAs)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Delete Drawing Tests

    [Fact]
    public async Task DrawingCanBeRemovedByIndex()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("DeleteDrawing");

        ws.Drawings.AddChart("Chart1", eChartType.Line);
        ws.Drawings.AddChart("Chart2", eChartType.Line);
        ws.Drawings.AddShape("Shape1", eShapeStyle.ActionButtonBackPrevious);

        Assert.Equal(3, ws.Drawings.Count);

        ws.Drawings.Remove(1);

        Assert.Equal(2, ws.Drawings.Count);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(DrawingCanBeRemovedByIndex)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task DrawingCanBeRemovedByObject()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("DeleteDrawing");

        var chart1 = ws.Drawings.AddChart("Chart1", eChartType.Line);
        var chart2 = ws.Drawings.AddChart("Chart2", eChartType.Line);
        ws.Drawings.AddShape("Shape1", eShapeStyle.ActionButtonBackPrevious);

        ws.Drawings.Remove(chart2);

        Assert.Equal(2, ws.Drawings.Count);
        Assert.Null(ws.Drawings["Chart2"]);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(DrawingCanBeRemovedByObject)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task DrawingCanBeRemovedByName()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("DeleteDrawing");

        ws.Drawings.AddChart("Chart1", eChartType.Line);
        ws.Drawings.AddChart("Chart2", eChartType.Line);
        ws.Drawings.AddShape("Shape1", eShapeStyle.ActionButtonBackPrevious);

        ws.Drawings.Remove("Chart1");

        Assert.Equal(2, ws.Drawings.Count);
        Assert.Null(ws.Drawings["Chart1"]);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(DrawingCanBeRemovedByName)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task DrawingsCanBeCleared()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("ClearDrawing");

        ws.Drawings.AddChart("Chart1", eChartType.Line);
        ws.Drawings.AddChart("Chart2", eChartType.Line);
        ws.Drawings.AddShape("Shape1", eShapeStyle.ActionButtonBackPrevious);

        Assert.Equal(3, ws.Drawings.Count);

        ws.Drawings.Clear();

        Assert.Equal(0, ws.Drawings.Count);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(DrawingsCanBeCleared)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task RemoveMultipleDrawingsSequentially()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("DeleteDrawing");
        using var image = CreateTestImage();

        ws.Drawings.AddChart("Chart1", eChartType.Line);
        var chart2 = ws.Drawings.AddChart("Chart2", eChartType.Line);
        ws.Drawings.AddShape("Shape1", eShapeStyle.ActionButtonBackPrevious);
        ws.Drawings.AddPicture("Pic1", image);

        Assert.Equal(4, ws.Drawings.Count);

        ws.Drawings.Remove(2);
        ws.Drawings.Remove(chart2);
        ws.Drawings.Remove("Pic1");

        Assert.Equal(1, ws.Drawings.Count);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(RemoveMultipleDrawingsSequentially)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Multi Chart Series Tests

    [Fact]
    public async Task MultiChartSeriesCanBeCreated()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("MultiChartTypes");
        AddTestData(ws);

        var chart = ws.Drawings.AddChart("chtPie", eChartType.LineMarkers);
        chart.SetPosition(100, 100);
        chart.SetSize(800, 600);
        AddTestSeries(ws, chart);
        chart.Series[0].Header = "Serie5";
        chart.Style = eChartStyle.Style27;

        ws.Cells["W19"].Value = 120;
        ws.Cells["W20"].Value = 122;
        ws.Cells["W21"].Value = 121;
        ws.Cells["W22"].Value = 123;
        ws.Cells["W23"].Value = 125;
        ws.Cells["W24"].Value = 124;

        var cs2 = chart.PlotArea.ChartTypes.Add(eChartType.ColumnClustered);
        var s = cs2.Series.Add(ws.Cells["W19:W24"], ws.Cells["U19:U24"]);
        s.Header = "Serie4";

        Assert.NotNull(cs2);
        Assert.Equal(2, chart.PlotArea.ChartTypes.Count);
        Assert.Equal("Serie4", s.Header);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(MultiChartSeriesCanBeCreated)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task MultiChartSeriesWithSecondaryAxis()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("MultiChartTypes");
        AddTestData(ws);

        ws.Cells["X19"].Value = 90;
        ws.Cells["X20"].Value = 52;
        ws.Cells["X21"].Value = 88;
        ws.Cells["X22"].Value = 75;
        ws.Cells["X23"].Value = 77;
        ws.Cells["X24"].Value = 99;

        var chart = ws.Drawings.AddChart("chtLine", eChartType.LineMarkers);
        AddTestSeries(ws, chart);

        var cs2 = chart.PlotArea.ChartTypes.Add(eChartType.ColumnClustered);
        cs2.Series.Add(ws.Cells["W19:W24"], ws.Cells["U19:U24"]);

        var cs3 = chart.PlotArea.ChartTypes.Add(eChartType.Line);
        var s = cs3.Series.Add(ws.Cells["X19:X24"], ws.Cells["U19:U24"]);
        s.Header = "Serie1";
        cs3.UseSecondaryAxis = true;

        Assert.Equal(3, chart.PlotArea.ChartTypes.Count);
        Assert.True(cs3.UseSecondaryAxis);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(MultiChartSeriesWithSecondaryAxis)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Header Address Tests

    [Fact]
    public async Task SeriesHeaderAddressCanBeSetToSingleCell()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Draw");

        var chart = ws.Drawings.AddChart("NewChart1", eChartType.Area) as ExcelChart;
        var ser1 = chart!.Series.Add("A1:A2", "B1:B2");
        ser1.HeaderAddress = new ExcelAddress("A1");

        Assert.NotNull(ser1.HeaderAddress);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(SeriesHeaderAddressCanBeSetToSingleCell)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task SeriesHeaderAddressCanBeSetToRow()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Draw");

        var chart = ws.Drawings.AddChart("NewChart1", eChartType.Area) as ExcelChart;
        var ser1 = chart!.Series.Add("A1:A2", "B1:B2");
        ser1.HeaderAddress = new ExcelAddress("A1:B1");

        Assert.NotNull(ser1.HeaderAddress);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(SeriesHeaderAddressCanBeSetToRow)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task SeriesHeaderAddressCanBeSetToColumn()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Draw");

        var chart = ws.Drawings.AddChart("NewChart1", eChartType.Area) as ExcelChart;
        var ser1 = chart!.Series.Add("A1:A2", "B1:B2");
        ser1.HeaderAddress = new ExcelAddress("A1:A2");

        Assert.NotNull(ser1.HeaderAddress);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(SeriesHeaderAddressCanBeSetToColumn)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Line2 Test

    [Fact]
    public async Task LineChartWithExcelRangeGetAddress()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("LineIssue");
        var chart = ws.Drawings.AddChart("LineChart", eChartType.Line);

        ws.Cells["A1"].Value = 1;
        ws.Cells["A2"].Value = 2;
        ws.Cells["A3"].Value = 3;
        ws.Cells["A4"].Value = 4;
        ws.Cells["A5"].Value = 5;
        ws.Cells["A6"].Value = 6;

        ws.Cells["B1"].Value = 10000;
        ws.Cells["B2"].Value = 10100;
        ws.Cells["B3"].Value = 10200;
        ws.Cells["B4"].Value = 10150;
        ws.Cells["B5"].Value = 10250;
        ws.Cells["B6"].Value = 10200;

        chart.Series.Add(ExcelRange.GetAddress(1, 2, ws.Dimension.End.Row, 2),
            ExcelRange.GetAddress(1, 1, ws.Dimension.End.Row, 1));
        chart.Series[0].Header = "Blah";

        Assert.Equal(1, chart.Series.Count);
        Assert.Equal("Blah", chart.Series[0].Header);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(LineChartWithExcelRangeGetAddress)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Drawing Count and Indexer Tests

    [Fact]
    public async Task DrawingIndexerByNameReturnsCorrectDrawing()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("DrawIndex");

        ws.Drawings.AddChart("Chart1", eChartType.Line);
        ws.Drawings.AddShape("Shape1", eShapeStyle.Rect);
        ws.Drawings.AddChart("Chart2", eChartType.Pie);

        Assert.NotNull(ws.Drawings["Chart1"]);
        Assert.NotNull(ws.Drawings["Shape1"]);
        Assert.NotNull(ws.Drawings["Chart2"]);
        Assert.IsType<ExcelShape>(ws.Drawings["Shape1"]);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(DrawingIndexerByNameReturnsCorrectDrawing)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task DrawingIndexerByPositionReturnsCorrectDrawing()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("DrawIndex");

        ws.Drawings.AddChart("Chart1", eChartType.Line);
        ws.Drawings.AddShape("Shape1", eShapeStyle.Rect);

        Assert.Equal("Chart1", ws.Drawings[0].Name);
        Assert.Equal("Shape1", ws.Drawings[1].Name);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(DrawingIndexerByPositionReturnsCorrectDrawing)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task DrawingIndexerByNameIsCaseInsensitive()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("DrawIndex");

        ws.Drawings.AddChart("Chart1", eChartType.Line);

        Assert.NotNull(ws.Drawings["chart1"]);
        Assert.NotNull(ws.Drawings["CHART1"]);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(DrawingIndexerByNameIsCaseInsensitive)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task DrawingIndexerReturnsNullForNonExistentName()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("DrawIndex");

        ws.Drawings.AddChart("Chart1", eChartType.Line);

        Assert.Null(ws.Drawings["NonExistent"]);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(DrawingIndexerReturnsNullForNonExistentName)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    [Fact]
    public async Task DuplicateNameThrowsException()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("DrawIndex");

        ws.Drawings.AddChart("Chart1", eChartType.Line);

        Assert.Throws<Exception>(() => ws.Drawings.AddChart("Chart1", eChartType.Pie));

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(DuplicateNameThrowsException)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Mixed Drawing Types Tests

    [Fact]
    public async Task MixedDrawingTypesCanBeAdded()
    {
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("Mixed");
        using var image = CreateTestImage();

        ws.Drawings.AddChart("Chart1", eChartType.Line);
        ws.Drawings.AddShape("Shape1", eShapeStyle.Rect);
        ws.Drawings.AddPicture("Pic1", image);
        ws.Drawings.AddChart("Chart2", eChartType.Pie);

        Assert.Equal(4, ws.Drawings.Count);
        Assert.IsAssignableFrom<ExcelChart>(ws.Drawings["Chart1"]);
        Assert.IsType<ExcelShape>(ws.Drawings["Shape1"]);
        Assert.IsType<ExcelPicture>(ws.Drawings["Pic1"]);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(MixedDrawingTypesCanBeAdded)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion

    #region Embedded Resource Picture Tests

    [Theory]
    [InlineData("test-image-01.bmp")]
    [InlineData("test-image-01.jpg")]
    [InlineData("test-image-01.png")]
    public async Task PictureCanBeAddedFromEmbeddedResource(string sampleImageFilename)
    {
        var resourceName = $"FreePPlus.OfficeOpenXml.Tests.SampleFiles.{sampleImageFilename}";
        await using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        Assert.NotNull(resourceStream);

        using var image = await Image.LoadAsync(resourceStream, CancellationToken.None);
        using var pck = new ExcelPackage();
        var ws = pck.Workbook.Worksheets.Add("EmbeddedPicture");

        var pic = ws.Drawings.AddPicture("Pic1", image);
        pic.SetPosition(2, 10, 2, 32);

        Assert.NotNull(pic);
        Assert.Equal("Pic1", pic.Name);
        Assert.Equal(1, ws.Drawings.Count);

#if SAVE_TEMP_FILES
        if (Directory.Exists(TempFolder))
        {
            var filename = Path.Combine(TempFolder, $"{DateTime.Now.Ticks}_{nameof(PictureCanBeAddedFromEmbeddedResource)}_{Path.GetFileNameWithoutExtension(sampleImageFilename)}.xlsx");
            await pck.SaveAsAsync(new FileInfo(filename));
        }
#else
        await Task.CompletedTask;
#endif
    }

    #endregion
}