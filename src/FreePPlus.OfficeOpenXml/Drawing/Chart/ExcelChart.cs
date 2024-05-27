/*******************************************************************************
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
 *******************************************************************************
 * Jan Källman		Added		2009-10-01
 * Jan Källman		License changed GPL-->LGPL 2011-12-16
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using OfficeOpenXml.Packaging;
using OfficeOpenXml.Table.PivotTable;
using OfficeOpenXml.Utils;

#pragma warning disable IDE0051
#pragma warning disable IDE1006
#pragma warning disable CA1822

namespace OfficeOpenXml.Drawing.Chart;

#region Chart Enums

// ReSharper disable InconsistentNaming
/// <summary>
///     Chart type
/// </summary>
public enum eChartType
{
    Area3D = -4098,
    AreaStacked3D = 78,
    AreaStacked1003D = 79,
    BarClustered3D = 60,
    BarStacked3D = 61,
    BarStacked1003D = 62,
    Column3D = -4100,
    ColumnClustered3D = 54,
    ColumnStacked3D = 55,
    ColumnStacked1003D = 56,
    Line3D = -4101,
    Pie3D = -4102,
    PieExploded3D = 70,
    Area = 1,
    AreaStacked = 76,
    AreaStacked100 = 77,
    BarClustered = 57,
    BarOfPie = 71,
    BarStacked = 58,
    BarStacked100 = 59,
    Bubble = 15,
    Bubble3DEffect = 87,
    ColumnClustered = 51,
    ColumnStacked = 52,
    ColumnStacked100 = 53,
    ConeBarClustered = 102,
    ConeBarStacked = 103,
    ConeBarStacked100 = 104,
    ConeCol = 105,
    ConeColClustered = 99,
    ConeColStacked = 100,
    ConeColStacked100 = 101,
    CylinderBarClustered = 95,
    CylinderBarStacked = 96,
    CylinderBarStacked100 = 97,
    CylinderCol = 98,
    CylinderColClustered = 92,
    CylinderColStacked = 93,
    CylinderColStacked100 = 94,
    Doughnut = -4120,
    DoughnutExploded = 80,
    Line = 4,
    LineMarkers = 65,
    LineMarkersStacked = 66,
    LineMarkersStacked100 = 67,
    LineStacked = 63,
    LineStacked100 = 64,
    Pie = 5,
    PieExploded = 69,
    PieOfPie = 68,
    PyramidBarClustered = 109,
    PyramidBarStacked = 110,
    PyramidBarStacked100 = 111,
    PyramidCol = 112,
    PyramidColClustered = 106,
    PyramidColStacked = 107,
    PyramidColStacked100 = 108,
    Radar = -4151,
    RadarFilled = 82,
    RadarMarkers = 81,
    StockHLC = 88,
    StockOHLC = 89,
    StockVHLC = 90,
    StockVOHLC = 91,
    Surface = 83,
    SurfaceTopView = 85,
    SurfaceTopViewWireframe = 86,
    SurfaceWireframe = 84,
    XYScatter = -4169,
    XYScatterLines = 74,
    XYScatterLinesNoMarkers = 75,
    XYScatterSmooth = 72,
    XYScatterSmoothNoMarkers = 73
}

/// <summary>
///     Bar or column
/// </summary>
public enum eDirection
{
    Column,
    Bar
}

/// <summary>
///     How the series are grouped
/// </summary>
public enum eGrouping
{
    Standard,
    Clustered,
    Stacked,
    PercentStacked
}

/// <summary>
///     Shape for bar charts
/// </summary>
public enum eShape
{
    Box,
    Cone,
    ConeToMax,
    Cylinder,
    Pyramid,
    PyramidToMax
}

/// <summary>
///     Smooth or lines markers
/// </summary>
public enum eScatterStyle
{
    LineMarker,
    SmoothMarker
}

public enum eRadarStyle
{
    /// <summary>
    ///     Specifies that the radar chart shall be filled and have lines but no markers.
    /// </summary>
    Filled,

    /// <summary>
    ///     Specifies that the radar chart shall have lines and markers but no fill.
    /// </summary>
    Marker,

    /// <summary>
    ///     Specifies that the radar chart shall have lines but no markers and no fill.
    /// </summary>
    Standard
}

/// <summary>
///     Bar or pie
/// </summary>
public enum ePieType
{
    Bar,
    Pie
}

/// <summary>
///     Position of the labels
/// </summary>
public enum eLabelPosition
{
    BestFit,
    Left,
    Right,
    Center,
    Top,
    Bottom,
    InBase,
    InEnd,
    OutEnd
}

/// <summary>
///     Axis label position
/// </summary>
public enum eTickLabelPosition
{
    High,
    Low,
    NextTo,
    None
}

/// <summary>
///     Markerstyle
/// </summary>
public enum eMarkerStyle
{
    Circle,
    Dash,
    Diamond,
    Dot,
    None,
    Picture,
    Plus,
    Square,
    Star,
    Triangle,
    X
}

/// <summary>
///     The time unit of major and minor datetime axis values
/// </summary>
public enum eTimeUnit
{
    Years,
    Months,
    Days
}

/// <summary>
///     The build in style of the chart.
/// </summary>
public enum eChartStyle
{
    None,
    Style1,
    Style2,
    Style3,
    Style4,
    Style5,
    Style6,
    Style7,
    Style8,
    Style9,
    Style10,
    Style11,
    Style12,
    Style13,
    Style14,
    Style15,
    Style16,
    Style17,
    Style18,
    Style19,
    Style20,
    Style21,
    Style22,
    Style23,
    Style24,
    Style25,
    Style26,
    Style27,
    Style28,
    Style29,
    Style30,
    Style31,
    Style32,
    Style33,
    Style34,
    Style35,
    Style36,
    Style37,
    Style38,
    Style39,
    Style40,
    Style41,
    Style42,
    Style43,
    Style44,
    Style45,
    Style46,
    Style47,
    Style48
}

/// <summary>
///     Type of Trendline for a chart
/// </summary>
public enum eTrendLine
{
    /// <summary>
    ///     Specifies the trendline shall be an exponential curve in the form
    /// </summary>
    Exponential,

    /// <summary>
    ///     Specifies the trendline shall be a logarithmic curve in the form , where log is the natural
    /// </summary>
    Linear,

    /// <summary>
    ///     Specifies the trendline shall be a logarithmic curve in the form , where log is the natural
    /// </summary>
    Logarithmic,

    /// <summary>
    ///     Specifies the trendline shall be a moving average of period Period
    /// </summary>
    MovingAvgerage,

    /// <summary>
    ///     Specifies the trendline shall be a polynomial curve of order Order in the form
    /// </summary>
    Polynomial,

    /// <summary>
    ///     Specifies the trendline shall be a power curve in the form
    /// </summary>
    Power
}

/// <summary>
///     Specifies the possible ways to display blanks
/// </summary>
public enum eDisplayBlanksAs
{
    /// <summary>
    ///     Blank values shall be left as a gap
    /// </summary>
    Gap,

    /// <summary>
    ///     Blank values shall be spanned with a line (Line charts)
    /// </summary>
    Span,

    /// <summary>
    ///     Blank values shall be treated as zero
    /// </summary>
    Zero
}

public enum eSizeRepresents
{
    /// <summary>
    ///     Specifies the area of the bubbles shall be proportional to the bubble size value.
    /// </summary>
    Area,

    /// <summary>
    ///     Specifies the radius of the bubbles shall be proportional to the bubble size value.
    /// </summary>
    Width
}

// ReSharper restore InconsistentNaming

#endregion

/// <summary>
///     Base class for Chart object.
/// </summary>
public class ExcelChart : ExcelDrawing
{
    private const string RootPath = "c:chartSpace/c:chart/c:plotArea";

    public ExcelPivotTable PivotTableSource { get; private set; }

    /// <summary>
    ///     Get the name of the chart node
    /// </summary>
    /// <returns>The name</returns>
    protected string GetChartNodeText()
    {
        return ChartType switch
        {
            eChartType.Area3D => "c:area3DChart",
            eChartType.AreaStacked3D => "c:area3DChart",
            eChartType.AreaStacked1003D => "c:area3DChart",
            eChartType.Area => "c:areaChart",
            eChartType.AreaStacked => "c:areaChart",
            eChartType.AreaStacked100 => "c:areaChart",
            eChartType.BarClustered => "c:barChart",
            eChartType.BarStacked => "c:barChart",
            eChartType.BarStacked100 => "c:barChart",
            eChartType.ColumnClustered => "c:barChart",
            eChartType.ColumnStacked => "c:barChart",
            eChartType.ColumnStacked100 => "c:barChart",
            eChartType.BarClustered3D => "c:bar3DChart",
            eChartType.BarStacked3D => "c:bar3DChart",
            eChartType.BarStacked1003D => "c:bar3DChart",
            eChartType.ColumnClustered3D => "c:bar3DChart",
            eChartType.ColumnStacked3D => "c:bar3DChart",
            eChartType.ColumnStacked1003D => "c:bar3DChart",
            eChartType.ConeBarClustered => "c:bar3DChart",
            eChartType.ConeBarStacked => "c:bar3DChart",
            eChartType.ConeBarStacked100 => "c:bar3DChart",
            eChartType.ConeCol => "c:bar3DChart",
            eChartType.ConeColClustered => "c:bar3DChart",
            eChartType.ConeColStacked => "c:bar3DChart",
            eChartType.ConeColStacked100 => "c:bar3DChart",
            eChartType.CylinderBarClustered => "c:bar3DChart",
            eChartType.CylinderBarStacked => "c:bar3DChart",
            eChartType.CylinderBarStacked100 => "c:bar3DChart",
            eChartType.CylinderCol => "c:bar3DChart",
            eChartType.CylinderColClustered => "c:bar3DChart",
            eChartType.CylinderColStacked => "c:bar3DChart",
            eChartType.CylinderColStacked100 => "c:bar3DChart",
            eChartType.PyramidBarClustered => "c:bar3DChart",
            eChartType.PyramidBarStacked => "c:bar3DChart",
            eChartType.PyramidBarStacked100 => "c:bar3DChart",
            eChartType.PyramidCol => "c:bar3DChart",
            eChartType.PyramidColClustered => "c:bar3DChart",
            eChartType.PyramidColStacked => "c:bar3DChart",
            eChartType.PyramidColStacked100 => "c:bar3DChart",
            eChartType.Bubble => "c:bubbleChart",
            eChartType.Bubble3DEffect => "c:bubbleChart",
            eChartType.Doughnut => "c:doughnutChart",
            eChartType.DoughnutExploded => "c:doughnutChart",
            eChartType.Line => "c:lineChart",
            eChartType.LineMarkers => "c:lineChart",
            eChartType.LineMarkersStacked => "c:lineChart",
            eChartType.LineMarkersStacked100 => "c:lineChart",
            eChartType.LineStacked => "c:lineChart",
            eChartType.LineStacked100 => "c:lineChart",
            eChartType.Line3D => "c:line3DChart",
            eChartType.Pie => "c:pieChart",
            eChartType.PieExploded => "c:pieChart",
            eChartType.BarOfPie => "c:ofPieChart",
            eChartType.PieOfPie => "c:ofPieChart",
            eChartType.Pie3D => "c:pie3DChart",
            eChartType.PieExploded3D => "c:pie3DChart",
            eChartType.Radar => "c:radarChart",
            eChartType.RadarFilled => "c:radarChart",
            eChartType.RadarMarkers => "c:radarChart",
            eChartType.XYScatter => "c:scatterChart",
            eChartType.XYScatterLines => "c:scatterChart",
            eChartType.XYScatterLinesNoMarkers => "c:scatterChart",
            eChartType.XYScatterSmooth => "c:scatterChart",
            eChartType.XYScatterSmoothNoMarkers => "c:scatterChart",
            eChartType.Surface => "c:surface3DChart",
            eChartType.SurfaceWireframe => "c:surface3DChart",
            eChartType.SurfaceTopView => "c:surfaceChart",
            eChartType.SurfaceTopViewWireframe => "c:surfaceChart",
            eChartType.StockHLC => "c:stockChart",
            _ => throw new NotImplementedException("Chart type not implemented")
        };
    }

    /// <summary>
    ///     Add a secondary axis
    /// </summary>
    internal void AddAxis()
    {
        var catAx = ChartXml.CreateElement(string.Format("c:{0}", AddAxType()), ExcelPackage.SchemaChart);
        int axId;
        if (_axis.Length == 0)
        {
            _plotArea.TopNode.AppendChild(catAx);
            axId = 1;
        }
        else
        {
            _axis[0]!.TopNode!.ParentNode!.InsertAfter(catAx, _axis[^1].TopNode);
            axId = int.Parse(_axis[0].Id) < int.Parse(_axis[1].Id)
                ? int.Parse(_axis[1].Id) + 1
                : int.Parse(_axis[0].Id) + 1;
        }


        var valAx = ChartXml.CreateElement("c:valAx", ExcelPackage.SchemaChart);
        catAx!.ParentNode!.InsertAfter(valAx, catAx);

        if (_axis.Length == 0)
        {
            catAx.InnerXml = string.Format(
                "<c:axId val=\"{0}\"/><c:scaling><c:orientation val=\"minMax\"/></c:scaling><c:delete val=\"0\" /><c:axPos val=\"b\"/><c:tickLblPos val=\"nextTo\"/><c:crossAx val=\"{1}\"/><c:crosses val=\"autoZero\"/><c:auto val=\"1\"/><c:lblAlgn val=\"ctr\"/><c:lblOffset val=\"100\"/>",
                axId, axId + 1);
            valAx.InnerXml = string.Format(
                "<c:axId val=\"{1}\"/><c:scaling><c:orientation val=\"minMax\"/></c:scaling><c:delete val=\"0\" /><c:axPos val=\"l\"/><c:majorGridlines/><c:tickLblPos val=\"nextTo\"/><c:crossAx val=\"{0}\"/><c:crosses val=\"autoZero\"/><c:crossBetween val=\"between\"/>",
                axId, axId + 1);
        }
        else
        {
            catAx.InnerXml = string.Format(
                "<c:axId val=\"{0}\"/><c:scaling><c:orientation val=\"minMax\"/></c:scaling><c:delete val=\"1\" /><c:axPos val=\"b\"/><c:tickLblPos val=\"none\"/><c:crossAx val=\"{1}\"/><c:crosses val=\"autoZero\"/>",
                axId, axId + 1);
            valAx.InnerXml = string.Format(
                "<c:axId val=\"{0}\"/><c:scaling><c:orientation val=\"minMax\"/></c:scaling><c:delete val=\"0\" /><c:axPos val=\"r\"/><c:tickLblPos val=\"nextTo\"/><c:crossAx val=\"{1}\"/><c:crosses val=\"max\"/><c:crossBetween val=\"between\"/>",
                axId + 1, axId);
        }

        if (_axis.Length == 0)
        {
            _axis = new ExcelChartAxis[2];
        }
        else
        {
            var newAxis = new ExcelChartAxis[_axis.Length + 2];
            Array.Copy(_axis, newAxis, _axis.Length);
            _axis = newAxis;
        }

        _axis[^2] = new ExcelChartAxis(NameSpaceManager, catAx);
        _axis[^1] = new ExcelChartAxis(NameSpaceManager, valAx);
        foreach (var chart in _plotArea.ChartTypes) chart._axis = _axis;
    }

    internal void RemoveSecondaryAxis()
    {
        throw new NotImplementedException("Not yet implemented");
    }

    internal static ExcelChart GetChart(ExcelDrawings drawings, XmlNode node /*, XmlNode chartTypeNode*/)
    {
        var chartNode =
            node.SelectSingleNode("xdr:graphicFrame/a:graphic/a:graphicData/c:chart", drawings.NameSpaceManager);
        if (chartNode?.Attributes != null)
        {
            var drawingRelation = drawings.Part.GetRelationship(chartNode.Attributes["r:id"]!.Value);
            var uriChart = UriHelper.ResolvePartUri(drawings.UriDrawing, drawingRelation.TargetUri);

            var part = drawings.Part.Package.GetPart(uriChart);
            var chartXml = new XmlDocument();
            LoadXmlSafe(chartXml, part.GetStream());

            ExcelChart topChart = null;
            foreach (XmlElement n in chartXml.SelectSingleNode(RootPath, drawings.NameSpaceManager)!.ChildNodes)
                if (topChart == null)
                {
                    topChart = GetChart(n, drawings, node, uriChart, part, chartXml, null);
                    topChart?.PlotArea.ChartTypes.Add(topChart);
                }
                else
                {
                    var subChart = GetChart(n, null, null, null, null, null, topChart);
                    if (subChart != null) topChart.PlotArea.ChartTypes.Add(subChart);
                }

            return topChart;
        }

        return null;
    }

    internal static ExcelChart GetChart(XmlElement chartNode, ExcelDrawings drawings, XmlNode node, Uri uriChart,
        ZipPackagePart part, XmlDocument chartXml, ExcelChart topChart)
    {
        switch (chartNode.LocalName)
        {
            case "area3DChart":
            case "areaChart":
            case "stockChart":
                if (topChart == null)
                    return new ExcelChart(drawings, node, uriChart, part, chartXml, chartNode);
                return new ExcelChart(topChart, chartNode);
            case "surface3DChart":
            case "surfaceChart":
                if (topChart == null)
                    return new ExcelSurfaceChart(drawings, node, uriChart, part, chartXml, chartNode);
                return new ExcelSurfaceChart(topChart, chartNode);
            case "radarChart":
                if (topChart == null)
                    return new ExcelRadarChart(drawings, node, uriChart, part, chartXml, chartNode);
                return new ExcelRadarChart(topChart, chartNode);
            case "bubbleChart":
                if (topChart == null)
                    return new ExcelBubbleChart(drawings, node, uriChart, part, chartXml, chartNode);
                return new ExcelBubbleChart(topChart, chartNode);
            case "barChart":
            case "bar3DChart":
                if (topChart == null)
                    return new ExcelBarChart(drawings, node, uriChart, part, chartXml, chartNode);
                return new ExcelBarChart(topChart, chartNode);
            case "doughnutChart":
                if (topChart == null)
                    return new ExcelDoughnutChart(drawings, node, uriChart, part, chartXml, chartNode);
                return new ExcelDoughnutChart(topChart, chartNode);
            case "pie3DChart":
            case "pieChart":
                if (topChart == null)
                    return new ExcelPieChart(drawings, node, uriChart, part, chartXml, chartNode);
                return new ExcelPieChart(topChart, chartNode);
            case "ofPieChart":
                if (topChart == null)
                    return new ExcelOfPieChart(drawings, node, uriChart, part, chartXml, chartNode);
                return new ExcelBarChart(topChart, chartNode);
            case "lineChart":
            case "line3DChart":
                if (topChart == null)
                    return new ExcelLineChart(drawings, node, uriChart, part, chartXml, chartNode);
                return new ExcelLineChart(topChart, chartNode);
            case "scatterChart":
                if (topChart == null)
                    return new ExcelScatterChart(drawings, node, uriChart, part, chartXml, chartNode);
                return new ExcelScatterChart(topChart, chartNode);
            default:
                return null;
        }
    }

    internal static ExcelChart GetNewChart(ExcelDrawings drawings, XmlNode drawNode, eChartType chartType,
        ExcelChart topChart, ExcelPivotTable pivotTableSource)
    {
        return chartType switch
        {
            eChartType.Pie => new ExcelPieChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.PieExploded => new ExcelPieChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.Pie3D => new ExcelPieChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.PieExploded3D => new ExcelPieChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.BarOfPie => new ExcelOfPieChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.PieOfPie => new ExcelOfPieChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.Doughnut => new ExcelDoughnutChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.DoughnutExploded => new ExcelDoughnutChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.BarClustered => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.BarStacked => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.BarStacked100 => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.BarClustered3D => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.BarStacked3D => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.BarStacked1003D => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.ConeBarClustered => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.ConeBarStacked => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.ConeBarStacked100 =>
                new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.CylinderBarClustered => new ExcelBarChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.CylinderBarStacked => new ExcelBarChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.CylinderBarStacked100 => new ExcelBarChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.PyramidBarClustered => new ExcelBarChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.PyramidBarStacked =>
                new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.PyramidBarStacked100 => new ExcelBarChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.ColumnClustered => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.ColumnStacked => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.ColumnStacked100 => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.Column3D => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.ColumnClustered3D =>
                new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.ColumnStacked3D => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.ColumnStacked1003D => new ExcelBarChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.ConeCol => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.ConeColClustered => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.ConeColStacked => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.ConeColStacked100 =>
                new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.CylinderCol => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.CylinderColClustered => new ExcelBarChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.CylinderColStacked => new ExcelBarChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.CylinderColStacked100 => new ExcelBarChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.PyramidCol => new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.PyramidColClustered => new ExcelBarChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.PyramidColStacked =>
                new ExcelBarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.PyramidColStacked100 => new ExcelBarChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.XYScatter => new ExcelScatterChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.XYScatterLines => new ExcelScatterChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.XYScatterLinesNoMarkers => new ExcelScatterChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.XYScatterSmooth => new ExcelScatterChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.XYScatterSmoothNoMarkers => new ExcelScatterChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.Line => new ExcelLineChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.Line3D => new ExcelLineChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.LineMarkers => new ExcelLineChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.LineMarkersStacked => new ExcelLineChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.LineMarkersStacked100 => new ExcelLineChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.LineStacked => new ExcelLineChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.LineStacked100 => new ExcelLineChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.Bubble => new ExcelBubbleChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.Bubble3DEffect =>
                new ExcelBubbleChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.Radar => new ExcelRadarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.RadarFilled => new ExcelRadarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.RadarMarkers => new ExcelRadarChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.Surface => new ExcelSurfaceChart(drawings, drawNode, chartType, topChart, pivotTableSource),
            eChartType.SurfaceTopView => new ExcelSurfaceChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.SurfaceTopViewWireframe => new ExcelSurfaceChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            eChartType.SurfaceWireframe => new ExcelSurfaceChart(drawings, drawNode, chartType, topChart,
                pivotTableSource),
            _ => new ExcelChart(drawings, drawNode, chartType, topChart, pivotTableSource)
        };
    }

    internal void SetPivotSource(ExcelPivotTable pivotTableSource)
    {
        PivotTableSource = pivotTableSource;
        var chart = ChartXml.SelectSingleNode("c:chartSpace/c:chart", NameSpaceManager) as XmlElement;

        var pivotSource = ChartXml.CreateElement("pivotSource", ExcelPackage.SchemaChart);
        chart!.ParentNode!.InsertBefore(pivotSource, chart);
        pivotSource.InnerXml = string.Format("<c:name>[]{0}!{1}</c:name><c:fmtId val=\"0\"/>",
            PivotTableSource.WorkSheet.Name, pivotTableSource.Name);

        var fmts = ChartXml.CreateElement("pivotFmts", ExcelPackage.SchemaChart);
        chart.PrependChild(fmts);
        fmts.InnerXml = "<c:pivotFmt><c:idx val=\"0\"/><c:marker><c:symbol val=\"none\"/></c:marker></c:pivotFmt>";

        Series.AddPivotSeries(pivotTableSource);
    }

    internal override void DeleteMe()
    {
        try
        {
            Part.Package.DeletePart(UriChart);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("FreePPlus internal error when deleteing chart.", ex);
        }

        base.DeleteMe();
    }
    //string _chartPath;

    // ReSharper disable InconsistentNaming
    protected internal ExcelChartSeries _chartSeries;
    internal ExcelChartAxis[] _axis;
    protected XmlHelper _chartXmlHelper;

    // ReSharper restore InconsistentNaming

    #region Constructors

    internal ExcelChart(ExcelDrawings drawings, XmlNode node, eChartType type, bool isPivot) :
        base(drawings, node, "xdr:graphicFrame/xdr:nvGraphicFramePr/xdr:cNvPr/@name")
    {
        ChartType = type;
        CreateNewChart(drawings, type, null);

        Init(drawings, _chartNode);

        _chartSeries = new ExcelChartSeries(this, drawings.NameSpaceManager, _chartNode, isPivot);

        SetTypeProperties();
        LoadAxis();
    }

    internal ExcelChart(ExcelDrawings drawings, XmlNode node, eChartType type, ExcelChart topChart,
        ExcelPivotTable pivotTableSource) :
        base(drawings, node, "xdr:graphicFrame/xdr:nvGraphicFramePr/xdr:cNvPr/@name")
    {
        ChartType = type;
        CreateNewChart(drawings, type, topChart);

        Init(drawings, _chartNode);

        _chartSeries = new ExcelChartSeries(this, drawings.NameSpaceManager, _chartNode, pivotTableSource != null);
        if (pivotTableSource != null) SetPivotSource(pivotTableSource);

        SetTypeProperties();
        if (topChart == null)
        {
            LoadAxis();
        }
        else
        {
            _axis = topChart.Axis;
            if (_axis.Length > 0)
            {
                XAxis = _axis[0];
                YAxis = _axis[1];
            }
        }
    }

    internal ExcelChart(ExcelDrawings drawings, XmlNode node, Uri uriChart, ZipPackagePart part, XmlDocument chartXml,
        XmlNode chartNode) :
        base(drawings, node, "xdr:graphicFrame/xdr:nvGraphicFramePr/xdr:cNvPr/@name")
    {
        UriChart = uriChart;
        Part = part;
        ChartXml = chartXml;
        _chartNode = chartNode;
        InitChartLoad(drawings, chartNode);
        // ReSharper disable once VirtualMemberCallInConstructor
        ChartType = GetChartType(chartNode.LocalName);
    }

    internal ExcelChart(ExcelChart topChart, XmlNode chartNode) :
        base(topChart._drawings, topChart.TopNode, "xdr:graphicFrame/xdr:nvGraphicFramePr/xdr:cNvPr/@name")
    {
        UriChart = topChart.UriChart;
        Part = topChart.Part;
        ChartXml = topChart.ChartXml;
        _plotArea = topChart.PlotArea;
        _chartNode = chartNode;

        InitChartLoad(topChart._drawings, chartNode);
    }

    private void InitChartLoad(ExcelDrawings drawings, XmlNode chartNode)
    {
        //SetChartType();
        Init(drawings, chartNode);
        _chartSeries = new ExcelChartSeries(this, drawings.NameSpaceManager, _chartNode,
            false /*ChartXml.SelectSingleNode(_chartPath, drawings.NameSpaceManager)*/);
        LoadAxis();
    }

    private void Init(ExcelDrawings drawings, XmlNode chartNode)
    {
        //_chartXmlHelper = new XmlHelper(drawings.NameSpaceManager, chartNode);
        _chartXmlHelper = XmlHelperFactory.Create(drawings.NameSpaceManager, chartNode);
        _chartXmlHelper.SchemaNodeOrder = new[]
        {
            "ofPieType", "title", "pivotFmt", "autoTitleDeleted", "view3D", "floor", "sideWall", "backWall", "plotArea",
            "wireframe",
            "barDir", "grouping", "scatterStyle", "radarStyle", "varyColors", "ser", "dLbls", "bubbleScale",
            "showNegBubbles", "dropLines", "upDownBars", "marker",
            "smooth", "shape", "legend", "plotVisOnly", "dispBlanksAs", "gapWidth", "showDLblsOverMax", "overlap",
            "bandFmts", "axId", "spPr", "printSettings"
        };
        WorkSheet = drawings.Worksheet;
    }

    #endregion

    #region Private functions

    private void SetTypeProperties()
    {
        /******* Grouping *******/
        if (IsTypeClustered())
            Grouping = eGrouping.Clustered;
        else if (
            IsTypeStacked())
            Grouping = eGrouping.Stacked;
        else if (
            IsTypePercentStacked())
            Grouping = eGrouping.PercentStacked;

        /***** 3D Perspective *****/
        if (IsType3D())
        {
            View3D.RotY = 20;
            View3D.Perspective = 30; //Default to 30
            if (IsTypePieDoughnut())
                View3D.RotX = 30;
            else
                View3D.RotX = 15;
        }
    }

    private void CreateNewChart(ExcelDrawings drawings, eChartType type, ExcelChart topChart)
    {
        if (topChart == null)
        {
            if (TopNode?.OwnerDocument != null)
            {
                var graphFrame = TopNode.OwnerDocument.CreateElement("graphicFrame", ExcelPackage.SchemaSheetDrawings);
                graphFrame.SetAttribute("macro", "");
                TopNode.AppendChild(graphFrame);
                graphFrame.InnerXml = string.Format(
                    "<xdr:nvGraphicFramePr><xdr:cNvPr id=\"{0}\" name=\"Chart 1\" /><xdr:cNvGraphicFramePr /></xdr:nvGraphicFramePr><xdr:xfrm><a:off x=\"0\" y=\"0\" /> <a:ext cx=\"0\" cy=\"0\" /></xdr:xfrm><a:graphic><a:graphicData uri=\"http://schemas.openxmlformats.org/drawingml/2006/chart\"><c:chart xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" r:id=\"rId1\" />   </a:graphicData>  </a:graphic>",
                    _id);
                TopNode.AppendChild(
                    TopNode.OwnerDocument.CreateElement("clientData", ExcelPackage.SchemaSheetDrawings));

                var package = drawings.Worksheet._package.Package;
                UriChart = GetNewUri(package, "/xl/charts/chart{0}.xml");

                ChartXml = new XmlDocument
                {
                    PreserveWhitespace = ExcelPackage.PreserveWhitespace
                };
                LoadXmlSafe(ChartXml, ChartStartXml(type), Encoding.UTF8);

                // save it to the package
                Part = package.CreatePart(UriChart, "application/vnd.openxmlformats-officedocument.drawingml.chart+xml",
                    _drawings._package.Compression);

                StreamWriter streamChart = new(Part.GetStream(FileMode.Create, FileAccess.Write));
                ChartXml.Save(streamChart);
                package.Flush();

                var chartRelation = drawings.Part.CreateRelationship(
                    UriHelper.GetRelativeUri(drawings.UriDrawing, UriChart), TargetMode.Internal,
                    ExcelPackage.SchemaRelationships + "/chart");
                graphFrame.SelectSingleNode("a:graphic/a:graphicData/c:chart", NameSpaceManager)!.Attributes!["r:id"]!
                    .Value = chartRelation.Id;
                package.Flush();
                _chartNode = ChartXml.SelectSingleNode(
                    string.Format("c:chartSpace/c:chart/c:plotArea/{0}", GetChartNodeText()), NameSpaceManager);
            }
        }
        else
        {
            ChartXml = topChart.ChartXml;
            Part = topChart.Part;
            _plotArea = topChart.PlotArea;
            UriChart = topChart.UriChart;
            _axis = topChart._axis;

            var preNode = _plotArea.ChartTypes[^1].ChartNode;
            _chartNode = ChartXml.CreateElement(GetChartNodeText(), ExcelPackage.SchemaChart);
            preNode.ParentNode!.InsertAfter(_chartNode, preNode);
            if (topChart.Axis.Length == 0) AddAxis();
            var seriesXml = GetChartSeriesStartXml(type, int.Parse(topChart.Axis[0].Id), int.Parse(topChart.Axis[1].Id),
                topChart.Axis.Length > 2 ? int.Parse(topChart.Axis[2].Id) : -1);
            _chartNode.InnerXml = seriesXml;
        }

        GetPositionSize();
    }

    private void LoadAxis()
    {
        var nl = _chartNode.SelectNodes("c:axId", NameSpaceManager);
        if (nl != null)
        {
            List<ExcelChartAxis> l = new();
            foreach (XmlNode node in nl)
            {
                var id = node!.Attributes!["val"]!.Value;
                var axNode = ChartXml.SelectNodes(RootPath + string.Format("/*/c:axId[@val=\"{0}\"]", id),
                    NameSpaceManager);
                if (axNode != null && axNode.Count > 1)
                    foreach (XmlNode axn in axNode)
                        if (axn!.ParentNode!.LocalName.EndsWith("Ax"))
                        {
                            var axisNode = axNode![1]!.ParentNode;
                            ExcelChartAxis ax = new(NameSpaceManager, axisNode);
                            l.Add(ax);
                        }
            }

            _axis = l.ToArray();

            if (_axis.Length > 0) XAxis = _axis[0];
            if (_axis.Length > 1) YAxis = _axis[1];
        }
    }

    //private void SetChartType()
    //{
    //    ChartType = 0;
    //    //_plotArea = new ExcelChartPlotArea(NameSpaceManager, ChartXml.SelectSingleNode("c:chartSpace/c:chart/c:plotArea", NameSpaceManager));
    //    int pos=0;
    //    foreach (XmlElement n in ChartXml.SelectSingleNode(rootPath, _drawings.NameSpaceManager).ChildNodes)
    //    {
    //        if (pos == 0)
    //        {
    //            ChartType = GetChartType(n.Name);
    //            if (ChartType != 0)
    //            {
    //                //_chartPath = rootPath + "/" + n.Name;
    //                PlotArea.ChartTypes.Add(this);
    //            }
    //        }
    //        else
    //        {
    //            var chartSerieType = GetChart(_drawings, TopNode/*, n*/);
    //            chartSerieType = GetChart(n, _drawings, TopNode, UriChart, Part, ChartXml, null, isPivot);
    //            PlotArea.ChartTypes.Add(chartSerieType);
    //            //var chartType = GetChartType(n.Name);
    //        }
    //        if (ChartType != 0)
    //        {
    //            pos++;
    //        }
    //    }
    //}

    internal virtual eChartType GetChartType(string name)
    {
        switch (name)
        {
            case "area3DChart":
                if (Grouping == eGrouping.Stacked)
                    return eChartType.AreaStacked3D;
                if (Grouping == eGrouping.PercentStacked)
                    return eChartType.AreaStacked1003D;
                return eChartType.Area3D;
            case "areaChart":
                if (Grouping == eGrouping.Stacked)
                    return eChartType.AreaStacked;
                if (Grouping == eGrouping.PercentStacked)
                    return eChartType.AreaStacked100;
                return eChartType.Area;
            case "doughnutChart":
                return eChartType.Doughnut;
            case "pie3DChart":
                return eChartType.Pie3D;
            case "pieChart":
                return eChartType.Pie;
            case "radarChart":
                return eChartType.Radar;
            case "scatterChart":
                return eChartType.XYScatter;
            case "surface3DChart":
            case "surfaceChart":
                return eChartType.Surface;
            case "stockChart":
                return eChartType.StockHLC;
            default:
                return 0;
        }
    }

    #region Xml init Functions

    private string ChartStartXml(eChartType chartType)
    {
        StringBuilder xml = new();
        var axId = 1;
        var xAxId = 2;
        var serAxId = IsTypeSurface() ? 3 : -1;

        xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
        xml.AppendFormat("<c:chartSpace xmlns:c=\"{0}\" xmlns:a=\"{1}\" xmlns:r=\"{2}\">", ExcelPackage.SchemaChart,
            ExcelPackage.SchemaDrawings, ExcelPackage.SchemaRelationships);
        xml.Append("<c:chart>");
        xml.AppendFormat("{0}{1}<c:plotArea><c:layout/>", AddPerspectiveXml(chartType), AddSurfaceXml(chartType));

        var chartNodeText = GetChartNodeText();
        xml.AppendFormat("<{0}>", chartNodeText);
        xml.Append(GetChartSeriesStartXml(chartType, axId, xAxId, serAxId));
        xml.AppendFormat("</{0}>", chartNodeText);

        //Axis
        if (!IsTypePieDoughnut())
        {
            if (IsTypeScatterBubble())
                xml.AppendFormat(
                    "<c:valAx><c:axId val=\"{0}\"/><c:scaling><c:orientation val=\"minMax\"/></c:scaling><c:delete val=\"0\"/><c:axPos val=\"b\"/><c:tickLblPos val=\"nextTo\"/><c:crossAx val=\"{1}\"/><c:crosses val=\"autoZero\"/></c:valAx>",
                    axId, xAxId);
            else
                xml.AppendFormat(
                    "<c:catAx><c:axId val=\"{0}\"/><c:scaling><c:orientation val=\"minMax\"/></c:scaling><c:delete val=\"0\"/><c:axPos val=\"b\"/><c:tickLblPos val=\"nextTo\"/><c:crossAx val=\"{1}\"/><c:crosses val=\"autoZero\"/><c:auto val=\"1\"/><c:lblAlgn val=\"ctr\"/><c:lblOffset val=\"100\"/></c:catAx>",
                    axId, xAxId);
            xml.AppendFormat(
                "<c:valAx><c:axId val=\"{1}\"/><c:scaling><c:orientation val=\"minMax\"/></c:scaling><c:delete val=\"0\"/><c:axPos val=\"l\"/><c:majorGridlines/><c:tickLblPos val=\"nextTo\"/><c:crossAx val=\"{0}\"/><c:crosses val=\"autoZero\"/><c:crossBetween val=\"between\"/></c:valAx>",
                axId, xAxId);
            if (serAxId == 3) //Sureface Chart
                xml.AppendFormat(
                    "<c:serAx><c:axId val=\"{0}\"/><c:scaling><c:orientation val=\"minMax\"/></c:scaling><c:delete val=\"0\"/><c:axPos val=\"b\"/><c:tickLblPos val=\"nextTo\"/><c:crossAx val=\"{1}\"/><c:crosses val=\"autoZero\"/></c:serAx>",
                    serAxId, xAxId);
        }

        // ReSharper disable FormatStringProblem
        xml.AppendFormat(
            "</c:plotArea><c:legend><c:legendPos val=\"r\"/><c:layout/><c:overlay val=\"0\" /></c:legend><c:plotVisOnly val=\"1\"/></c:chart>",
            axId, xAxId);
        // ReSharper restore FormatStringProblem

        xml.Append(
            "<c:printSettings><c:headerFooter/><c:pageMargins b=\"0.75\" l=\"0.7\" r=\"0.7\" t=\"0.75\" header=\"0.3\" footer=\"0.3\"/><c:pageSetup/></c:printSettings></c:chartSpace>");
        return xml.ToString();
    }

    private string GetChartSeriesStartXml(eChartType type, int axId, int xAxId, int serAxId)
    {
        StringBuilder xml = new();

        xml.Append(AddScatterType(type));
        xml.Append(AddRadarType(type));
        xml.Append(AddBarDir(type));
        xml.Append(AddGrouping());
        xml.Append(AddVaryColors());
        xml.Append(AddHasMarker(type));
        xml.Append(AddShape(type));
        xml.Append(AddFirstSliceAng(type));
        xml.Append(AddHoleSize(type));
        if (ChartType == eChartType.BarStacked100 ||
            ChartType == eChartType.BarStacked ||
            ChartType == eChartType.ColumnStacked ||
            ChartType == eChartType.ColumnStacked100)
            xml.Append("<c:overlap val=\"100\"/>");
        if (IsTypeSurface()) xml.Append("<c:bandFmts/>");
        xml.Append(AddAxisId(axId, xAxId, serAxId));

        return xml.ToString();
    }

    private string AddAxisId(int axId, int xAxId, int serAxId)
    {
        if (!IsTypePieDoughnut())
        {
            if (IsTypeSurface())
                return string.Format("<c:axId val=\"{0}\"/><c:axId val=\"{1}\"/><c:axId val=\"{2}\"/>", axId, xAxId,
                    serAxId);
            return string.Format("<c:axId val=\"{0}\"/><c:axId val=\"{1}\"/>", axId, xAxId);
        }

        return "";
    }

    private string AddAxType()
    {
        return ChartType switch
        {
            eChartType.XYScatter => "valAx",
            eChartType.XYScatterLines => "valAx",
            eChartType.XYScatterLinesNoMarkers => "valAx",
            eChartType.XYScatterSmooth => "valAx",
            eChartType.XYScatterSmoothNoMarkers => "valAx",
            eChartType.Bubble => "valAx",
            eChartType.Bubble3DEffect => "valAx",
            _ => "catAx"
        };
    }

    private string AddScatterType(eChartType chartType)
    {
        if (chartType == eChartType.XYScatter ||
            chartType == eChartType.XYScatterLines ||
            chartType == eChartType.XYScatterLinesNoMarkers ||
            chartType == eChartType.XYScatterSmooth ||
            chartType == eChartType.XYScatterSmoothNoMarkers)
            return "<c:scatterStyle val=\"\" />";
        return "";
    }

    private string AddRadarType(eChartType chartType)
    {
        if (chartType == eChartType.Radar ||
            chartType == eChartType.RadarFilled ||
            chartType == eChartType.RadarMarkers)
            return "<c:radarStyle val=\"\" />";
        return "";
    }

    private string AddGrouping()
    {
        //IsTypeClustered() || IsTypePercentStacked() || IsTypeStacked() || 
        if (IsTypeShape() || IsTypeLine())
            return "<c:grouping val=\"standard\"/>";
        return "";
    }

    private string AddHoleSize(eChartType chartType)
    {
        if (chartType == eChartType.Doughnut ||
            chartType == eChartType.DoughnutExploded)
            return "<c:holeSize val=\"50\" />";
        return "";
    }

    private string AddFirstSliceAng(eChartType chartType)
    {
        if (chartType == eChartType.Doughnut ||
            chartType == eChartType.DoughnutExploded)
            return "<c:firstSliceAng val=\"0\" />";
        return "";
    }

    private string AddVaryColors()
    {
        if (IsTypePieDoughnut())
            return "<c:varyColors val=\"1\" />";
        return "<c:varyColors val=\"0\" />";
    }

    private string AddHasMarker(eChartType chartType)
    {
        if (
            chartType == eChartType.LineMarkers ||
            chartType == eChartType.LineMarkersStacked ||
            chartType == eChartType.LineMarkersStacked100 /*||

            ype == eChartType.XYScatterLines ||

            ype == eChartType.XYScatterSmooth*/
        )
            return "<c:marker val=\"1\"/>";
        return "";
    }

    // ReSharper disable UnusedParameter.Local
#pragma warning disable IDE0060

    private string AddShape(eChartType chartType)
    {
        if (IsTypeShape())
            return "<c:shape val=\"box\" />";
        return "";
    }

    private string AddBarDir(eChartType chartType)
    {
        if (IsTypeShape())
            return "<c:barDir val=\"col\" />";
        return "";
    }

    private string AddPerspectiveXml(eChartType chartType)
    {
        //Add for 3D charts
        if (IsType3D())
            return "<c:view3D><c:perspective val=\"30\" /></c:view3D>";
        return "";
    }

    private string AddSurfaceXml(eChartType chartType)
    {
        if (IsTypeSurface())
            return AddSurfacePart("floor") + AddSurfacePart("sideWall") + AddSurfacePart("backWall");
        return "";
    }

#pragma warning restore IDE0060
    // ReSharper restore UnusedParameter.Local

    private string AddSurfacePart(string name)
    {
        return string.Format(
            "<c:{0}><c:thickness val=\"0\"/><c:spPr><a:noFill/><a:ln><a:noFill/></a:ln><a:effectLst/><a:sp3d/></c:spPr></c:{0}>",
            name);
    }

    #endregion

    #endregion

    #region Chart type functions

    internal static bool IsType3D(eChartType chartType)
    {
        return chartType == eChartType.Area3D ||
               chartType == eChartType.AreaStacked3D ||
               chartType == eChartType.AreaStacked1003D ||
               chartType == eChartType.BarClustered3D ||
               chartType == eChartType.BarStacked3D ||
               chartType == eChartType.BarStacked1003D ||
               chartType == eChartType.Column3D ||
               chartType == eChartType.ColumnClustered3D ||
               chartType == eChartType.ColumnStacked3D ||
               chartType == eChartType.ColumnStacked1003D ||
               chartType == eChartType.Line3D ||
               chartType == eChartType.Pie3D ||
               chartType == eChartType.PieExploded3D ||
               chartType == eChartType.ConeBarClustered ||
               chartType == eChartType.ConeBarStacked ||
               chartType == eChartType.ConeBarStacked100 ||
               chartType == eChartType.ConeCol ||
               chartType == eChartType.ConeColClustered ||
               chartType == eChartType.ConeColStacked ||
               chartType == eChartType.ConeColStacked100 ||
               chartType == eChartType.CylinderBarClustered ||
               chartType == eChartType.CylinderBarStacked ||
               chartType == eChartType.CylinderBarStacked100 ||
               chartType == eChartType.CylinderCol ||
               chartType == eChartType.CylinderColClustered ||
               chartType == eChartType.CylinderColStacked ||
               chartType == eChartType.CylinderColStacked100 ||
               chartType == eChartType.PyramidBarClustered ||
               chartType == eChartType.PyramidBarStacked ||
               chartType == eChartType.PyramidBarStacked100 ||
               chartType == eChartType.PyramidCol ||
               chartType == eChartType.PyramidColClustered ||
               chartType == eChartType.PyramidColStacked ||
               chartType == eChartType.PyramidColStacked100 ||
               chartType == eChartType.Surface ||
               chartType == eChartType.SurfaceTopView ||
               chartType == eChartType.SurfaceTopViewWireframe ||
               chartType == eChartType.SurfaceWireframe;
    }

    protected internal bool IsType3D()
    {
        return IsType3D(ChartType);
    }

    protected bool IsTypeLine()
    {
        return ChartType == eChartType.Line ||
               ChartType == eChartType.LineMarkers ||
               ChartType == eChartType.LineMarkersStacked100 ||
               ChartType == eChartType.LineStacked ||
               ChartType == eChartType.LineStacked100 ||
               ChartType == eChartType.Line3D;
    }

    protected bool IsTypeScatterBubble()
    {
        return ChartType == eChartType.XYScatter ||
               ChartType == eChartType.XYScatterLines ||
               ChartType == eChartType.XYScatterLinesNoMarkers ||
               ChartType == eChartType.XYScatterSmooth ||
               ChartType == eChartType.XYScatterSmoothNoMarkers ||
               ChartType == eChartType.Bubble ||
               ChartType == eChartType.Bubble3DEffect;
    }

    protected bool IsTypeSurface()
    {
        return ChartType == eChartType.Surface ||
               ChartType == eChartType.SurfaceTopView ||
               ChartType == eChartType.SurfaceTopViewWireframe ||
               ChartType == eChartType.SurfaceWireframe;
    }

    protected bool IsTypeShape()
    {
        return ChartType == eChartType.BarClustered3D ||
               ChartType == eChartType.BarStacked3D ||
               ChartType == eChartType.BarStacked1003D ||
               ChartType == eChartType.BarClustered3D ||
               ChartType == eChartType.BarStacked3D ||
               ChartType == eChartType.BarStacked1003D ||
               ChartType == eChartType.Column3D ||
               ChartType == eChartType.ColumnClustered3D ||
               ChartType == eChartType.ColumnStacked3D ||
               ChartType == eChartType.ColumnStacked1003D ||
               //ChartType == eChartType.3DPie ||
               //ChartType == eChartType.3DPieExploded ||
               //ChartType == eChartType.Bubble3DEffect ||
               ChartType == eChartType.ConeBarClustered ||
               ChartType == eChartType.ConeBarStacked ||
               ChartType == eChartType.ConeBarStacked100 ||
               ChartType == eChartType.ConeCol ||
               ChartType == eChartType.ConeColClustered ||
               ChartType == eChartType.ConeColStacked ||
               ChartType == eChartType.ConeColStacked100 ||
               ChartType == eChartType.CylinderBarClustered ||
               ChartType == eChartType.CylinderBarStacked ||
               ChartType == eChartType.CylinderBarStacked100 ||
               ChartType == eChartType.CylinderCol ||
               ChartType == eChartType.CylinderColClustered ||
               ChartType == eChartType.CylinderColStacked ||
               ChartType == eChartType.CylinderColStacked100 ||
               ChartType == eChartType.PyramidBarClustered ||
               ChartType == eChartType.PyramidBarStacked ||
               ChartType == eChartType.PyramidBarStacked100 ||
               ChartType == eChartType.PyramidCol ||
               ChartType == eChartType.PyramidColClustered ||
               ChartType == eChartType.PyramidColStacked ||
               ChartType == eChartType.PyramidColStacked100; //||
        //ChartType == eChartType.Doughnut ||
        //ChartType == eChartType.DoughnutExploded;
    }

    protected internal bool IsTypePercentStacked()
    {
        return ChartType == eChartType.AreaStacked100 ||
               ChartType == eChartType.BarStacked100 ||
               ChartType == eChartType.BarStacked1003D ||
               ChartType == eChartType.ColumnStacked100 ||
               ChartType == eChartType.ColumnStacked1003D ||
               ChartType == eChartType.ConeBarStacked100 ||
               ChartType == eChartType.ConeColStacked100 ||
               ChartType == eChartType.CylinderBarStacked100 ||
               ChartType == eChartType.CylinderColStacked ||
               ChartType == eChartType.LineMarkersStacked100 ||
               ChartType == eChartType.LineStacked100 ||
               ChartType == eChartType.PyramidBarStacked100 ||
               ChartType == eChartType.PyramidColStacked100;
    }

    protected internal bool IsTypeStacked()
    {
        return ChartType == eChartType.AreaStacked ||
               ChartType == eChartType.AreaStacked3D ||
               ChartType == eChartType.BarStacked ||
               ChartType == eChartType.BarStacked3D ||
               ChartType == eChartType.ColumnStacked3D ||
               ChartType == eChartType.ColumnStacked ||
               ChartType == eChartType.ConeBarStacked ||
               ChartType == eChartType.ConeColStacked ||
               ChartType == eChartType.CylinderBarStacked ||
               ChartType == eChartType.CylinderColStacked ||
               ChartType == eChartType.LineMarkersStacked ||
               ChartType == eChartType.LineStacked ||
               ChartType == eChartType.PyramidBarStacked ||
               ChartType == eChartType.PyramidColStacked;
    }

    protected bool IsTypeClustered()
    {
        return ChartType == eChartType.BarClustered ||
               ChartType == eChartType.BarClustered3D ||
               ChartType == eChartType.ColumnClustered3D ||
               ChartType == eChartType.ColumnClustered ||
               ChartType == eChartType.ConeBarClustered ||
               ChartType == eChartType.ConeColClustered ||
               ChartType == eChartType.CylinderBarClustered ||
               ChartType == eChartType.CylinderColClustered ||
               ChartType == eChartType.PyramidBarClustered ||
               ChartType == eChartType.PyramidColClustered;
    }

    protected internal bool IsTypePieDoughnut()
    {
        return ChartType == eChartType.Pie ||
               ChartType == eChartType.PieExploded ||
               ChartType == eChartType.PieOfPie ||
               ChartType == eChartType.Pie3D ||
               ChartType == eChartType.PieExploded3D ||
               ChartType == eChartType.BarOfPie ||
               ChartType == eChartType.Doughnut ||
               ChartType == eChartType.DoughnutExploded;
    }

    #endregion

    #region Properties

    /// <summary>
    ///     Reference to the worksheet
    /// </summary>
    public ExcelWorksheet WorkSheet { get; internal set; }

    /// <summary>
    ///     The chart xml document
    /// </summary>
    public XmlDocument ChartXml { get; internal set; }

    /// <summary>
    ///     Type of chart
    /// </summary>
    public eChartType ChartType { get; internal set; }

    // ReSharper disable once InconsistentNaming
    protected internal XmlNode _chartNode;
    internal XmlNode ChartNode => _chartNode;

    /// <summary>
    ///     Title of the chart
    /// </summary>
    public ExcelChartTitle Title =>
        _title ??= new ExcelChartTitle(NameSpaceManager,
            ChartXml.SelectSingleNode("c:chartSpace/c:chart", NameSpaceManager));

    /// <summary>
    ///     Chart series
    /// </summary>
    public virtual ExcelChartSeries Series => _chartSeries;

    /// <summary>
    ///     An array containg all axis of all Charttypes
    /// </summary>
    public ExcelChartAxis[] Axis => _axis;

    /// <summary>
    ///     The XAxis
    /// </summary>
    public ExcelChartAxis XAxis { get; private set; }

    /// <summary>
    ///     The YAxis
    /// </summary>
    public ExcelChartAxis YAxis { get; private set; }

    private bool _secondaryAxis;

    /// <summary>
    ///     If true the charttype will use the secondary axis.
    ///     The chart must contain a least one other charttype that uses the primary axis.
    /// </summary>
    public bool UseSecondaryAxis
    {
        get => _secondaryAxis;
        set
        {
            if (_secondaryAxis != value)
            {
                if (value)
                {
                    if (IsTypePieDoughnut())
                        throw new Exception("Pie charts do not support axis");
                    if (HasPrimaryAxis() == false)
                        throw new Exception("Can't set to secondary axis when no serie uses the primary axis");
                    if (Axis.Length == 2) AddAxis();
                    var nl = ChartNode.SelectNodes("c:axId", NameSpaceManager);
                    nl![0]!.Attributes!["val"]!.Value = Axis[2].Id;
                    nl![1]!.Attributes!["val"]!.Value = Axis[3].Id;
                    XAxis = Axis[2];
                    YAxis = Axis[3];
                }
                else
                {
                    var nl = ChartNode.SelectNodes("c:axId", NameSpaceManager);
                    nl![0]!.Attributes!["val"]!.Value = Axis[0].Id;
                    nl![1]!.Attributes!["val"]!.Value = Axis[1].Id;
                    XAxis = Axis[0];
                    YAxis = Axis[1];
                }

                _secondaryAxis = value;
            }
        }
    }

    /// <summary>
    ///     The build-in chart styles.
    /// </summary>
    public eChartStyle Style
    {
        get
        {
            var node = ChartXml.SelectSingleNode("c:chartSpace/c:style/@val", NameSpaceManager);
            if (node == null) return eChartStyle.None;

            if (int.TryParse(node.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                return (eChartStyle)v;
            return eChartStyle.None;
        }
        set
        {
            if (value == eChartStyle.None)
            {
                var element = ChartXml.SelectSingleNode("c:chartSpace/c:style", NameSpaceManager) as XmlElement;
                if (element?.ParentNode != null) element.ParentNode.RemoveChild(element);
            }
            else
            {
                var element = ChartXml.CreateElement("c:style", ExcelPackage.SchemaChart);
                element.SetAttribute("val", ((int)value).ToString());
                if (ChartXml.SelectSingleNode("c:chartSpace", NameSpaceManager) is XmlElement parent)
                    parent.InsertBefore(element, parent.SelectSingleNode("c:chart", NameSpaceManager));
            }
        }
    }

    private const string RoundedCornersPath = "../../../c:roundedCorners/@val";

    /// <summary>
    ///     Border rounded corners
    /// </summary>
    public bool RoundedCorners
    {
        get => _chartXmlHelper.GetXmlNodeBool(RoundedCornersPath);
        set => _chartXmlHelper.SetXmlNodeBool(RoundedCornersPath, value);
    }

    private const string PlotVisibleOnlyPath = "../../c:plotVisOnly/@val";

    /// <summary>
    ///     Show data in hidden rows and columns
    /// </summary>
    public bool ShowHiddenData
    {
        get => !_chartXmlHelper.GetXmlNodeBool(PlotVisibleOnlyPath); //!!Inverted value!!
        set => _chartXmlHelper.SetXmlNodeBool(PlotVisibleOnlyPath, !value); //!!Inverted value!!
    }

    private const string DisplayBlanksAsPath = "../../c:dispBlanksAs/@val";

    /// <summary>
    ///     Specifies the possible ways to display blanks
    /// </summary>
    public eDisplayBlanksAs DisplayBlanksAs
    {
        get
        {
            var v = _chartXmlHelper.GetXmlNodeString(DisplayBlanksAsPath);
            if (string.IsNullOrEmpty(v))
                return eDisplayBlanksAs.Zero; //Issue 14715 Changed in Office 2010-?
            return (eDisplayBlanksAs)Enum.Parse(typeof(eDisplayBlanksAs), v, true);
        }
        set => _chartSeries.SetXmlNodeString(DisplayBlanksAsPath,
            value.ToString().ToLower(CultureInfo.InvariantCulture));
    }

    private const string ShowDLblsOverMax = "../../c:showDLblsOverMax/@val";

    /// <summary>
    ///     Specifies data labels over the maximum of the chart shall be shown
    /// </summary>
    public bool ShowDataLabelsOverMaximum
    {
        get => _chartXmlHelper.GetXmlNodeBool(ShowDLblsOverMax, true);
        set => _chartXmlHelper.SetXmlNodeBool(ShowDLblsOverMax, value, true);
    }

    private bool HasPrimaryAxis()
    {
        if (_plotArea.ChartTypes.Count == 1) return false;
        foreach (var chart in _plotArea.ChartTypes)
            if (chart != this)
                if (chart.UseSecondaryAxis == false && chart.IsTypePieDoughnut() == false)
                    return true;
        return false;
    }

    ///// <summary>
    ///// Sets position of the axis of a chart-serie
    ///// </summary>
    ///// <param name="XAxis">Left or Right</param>
    ///// <param name="YAxis">Top or Bottom</param>
    //internal void SetAxis(eXAxisPosition XAxis, eYAxisPosition YAxis)
    //{
    //    bool xAxisExists = false, yAxisExists = false;
    //    foreach (var axis in _axis)
    //    {
    //        if (axis.AxisPosition == (eAxisPosition)XAxis)
    //        {
    //            //Found
    //            xAxisExists=true;
    //            if (axis != this.XAxis)
    //            {
    //                CheckRemoveAxis(this.XAxis);
    //                this.XAxis = axis;
    //            }
    //        }
    //        else if (axis.AxisPosition == (eAxisPosition)YAxis)
    //        {
    //            yAxisExists = true;
    //            if (axis != this.YAxis)
    //            {
    //                CheckRemoveAxis(this.YAxis);
    //                this.YAxis = axis;
    //            }
    //        }
    //    }

    //    if (!xAxisExists)
    //    {
    //        if (ExistsAxis(this.XAxis))
    //        {
    //            AddAxis((eAxisPosition)XAxis);
    //            this.XAxis = Axis[Axis.Length - 1];
    //        }
    //        else
    //        {
    //            this.XAxis.AxisPosition = (eAxisPosition)XAxis;
    //        }
    //    }
    //    if (!yAxisExists)
    //    {
    //        if (ExistsAxis(this.XAxis))
    //        {
    //            AddAxis((eAxisPosition)YAxis);
    //            this.YAxis = Axis[Axis.Length - 1];
    //        }
    //        else
    //        {
    //            this.YAxis.AxisPosition = (eAxisPosition)YAxis;
    //        }
    //    }
    //}

    /// <summary>
    ///     Remove all axis that are not used any more
    /// </summary>
    /// <param name="excelChartAxis"></param>
    // ReSharper disable once UnusedMember.Local
    private void CheckRemoveAxis(ExcelChartAxis excelChartAxis)
    {
        if (ExistsAxis(excelChartAxis))
        {
            //Remove the axis
            var newAxis = new ExcelChartAxis[Axis.Length - 1];
            var pos = 0;
            foreach (var ax in Axis)
                if (ax != excelChartAxis)
                    newAxis[pos] = ax;

            //Update all charttypes.
            foreach (var chartType in _plotArea.ChartTypes) chartType._axis = newAxis;
        }
    }

    private bool ExistsAxis(ExcelChartAxis excelChartAxis)
    {
        foreach (var chartType in _plotArea.ChartTypes)
            if (chartType != this)
                if (chartType.XAxis.AxisPosition == excelChartAxis.AxisPosition ||
                    chartType.YAxis.AxisPosition == excelChartAxis.AxisPosition)
                    //The axis exists
                    return true;
        return false;
    }

    private ExcelChartPlotArea _plotArea;

    /// <summary>
    ///     Plotarea
    /// </summary>
    public ExcelChartPlotArea PlotArea =>
        _plotArea ??= new ExcelChartPlotArea(NameSpaceManager,
            ChartXml.SelectSingleNode("c:chartSpace/c:chart/c:plotArea", NameSpaceManager), this);

    private ExcelChartLegend _legend;

    /// <summary>
    ///     Legend
    /// </summary>
    public ExcelChartLegend Legend =>
        _legend ??= new ExcelChartLegend(NameSpaceManager,
            ChartXml.SelectSingleNode("c:chartSpace/c:chart/c:legend", NameSpaceManager), this);

    private ExcelDrawingBorder _border;

    /// <summary>
    ///     Border
    /// </summary>
    public ExcelDrawingBorder Border =>
        _border ??= new ExcelDrawingBorder(NameSpaceManager,
            ChartXml.SelectSingleNode("c:chartSpace", NameSpaceManager), "c:spPr/a:ln");

    private ExcelDrawingFill _fill;

    /// <summary>
    ///     Fill
    /// </summary>
    public ExcelDrawingFill Fill =>
        _fill ??= new ExcelDrawingFill(NameSpaceManager,
            ChartXml.SelectSingleNode("c:chartSpace", NameSpaceManager), "c:spPr");

    /// <summary>
    ///     3D-settings
    /// </summary>
    public ExcelView3D View3D
    {
        get
        {
            if (IsType3D())
                return new ExcelView3D(NameSpaceManager, ChartXml.SelectSingleNode("//c:view3D", NameSpaceManager));
            throw new Exception("Charttype does not support 3D");
        }
    }

    //string _groupingPath = "c:chartSpace/c:chart/c:plotArea/{0}/c:grouping/@val";
    private const string GroupingPath = "c:grouping/@val";

    public eGrouping Grouping
    {
        get => GetGroupingEnum(_chartXmlHelper.GetXmlNodeString(GroupingPath));
        internal set => _chartXmlHelper.SetXmlNodeString(GroupingPath, GetGroupingText(value));
    }

    //string _varyColorsPath = "c:chartSpace/c:chart/c:plotArea/{0}/c:varyColors/@val";
    private const string VaryColorsPath = "c:varyColors/@val";

    /// <summary>
    ///     If the chart has only one serie this varies the colors for each point.
    /// </summary>
    public bool VaryColors
    {
        get => _chartXmlHelper.GetXmlNodeBool(VaryColorsPath);
        set => _chartXmlHelper.SetXmlNodeString(VaryColorsPath, value ? "1" : "0");
    }

    internal ZipPackagePart Part { get; set; }

    /// <summary>
    ///     Package internal URI
    /// </summary>
    internal Uri UriChart { get; set; }

    internal new string Id => "";

    private ExcelChartTitle _title;

    #endregion

    #region Grouping Enum Translation

    private string GetGroupingText(eGrouping grouping)
    {
        return grouping switch
        {
            eGrouping.Clustered => "clustered",
            eGrouping.Stacked => "stacked",
            eGrouping.PercentStacked => "percentStacked",
            _ => "standard"
        };
    }

    private eGrouping GetGroupingEnum(string grouping)
    {
        return grouping switch
        {
            "stacked" => eGrouping.Stacked,
            "percentStacked" => eGrouping.PercentStacked,
            _ => eGrouping.Clustered
        };
    }

    #endregion
}