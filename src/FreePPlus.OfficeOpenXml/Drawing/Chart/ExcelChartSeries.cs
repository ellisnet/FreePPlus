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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using OfficeOpenXml.Table.PivotTable;

#pragma warning disable CA1822

// ReSharper disable RedundantBaseQualifier

namespace OfficeOpenXml.Drawing.Chart;

public sealed class ExcelBubbleChartSeries : ExcelChartSeries
{
    internal ExcelBubbleChartSeries(ExcelChart chart, XmlNamespaceManager ns, XmlNode node, bool isPivot)
        : base(chart, ns, node, isPivot)
    {
        //_chartSeries = new ExcelChartSeries(this, _drawings.NameSpaceManager, _chartNode, isPivot);
    }

    public ExcelChartSeriesItem Add(ExcelRangeBase series, ExcelRangeBase xSeries, ExcelRangeBase bubbleSize)
    {
        return base.AddSeries(series.FullAddressAbsolute, xSeries.FullAddressAbsolute, bubbleSize.FullAddressAbsolute);
    }

    public ExcelChartSeriesItem Add(string seriesAddress, string xSeriesAddress, string bubbleSizeAddress)
    {
        return base.AddSeries(seriesAddress, xSeriesAddress, bubbleSizeAddress);
    }
}

/// <summary>
///     Collection class for chart series
/// </summary>
public class ExcelChartSeries : XmlHelper, IEnumerable
{
    private readonly List<ExcelChartSeriesItem> _list = new();
    private readonly XmlNode _node;
    private readonly XmlNamespaceManager _ns;

    // ReSharper disable once InconsistentNaming
    internal ExcelChart _chart;

    internal ExcelChartSeries(ExcelChart chart, XmlNamespaceManager ns, XmlNode node, bool isPivot)
        : base(ns, node)
    {
        _ns = ns;
        _chart = chart;
        _node = node;
        _isPivot = isPivot;
        SchemaNodeOrder = new[]
        {
            "view3D", "plotArea", "barDir", "grouping", "scatterStyle", "varyColors", "ser", "explosion", "dLbls",
            "firstSliceAng", "holeSize", "shape", "legend", "axId"
        };

        if (node != null)
            foreach (XmlNode n in node.SelectNodes("c:ser", ns)!)
            {
                ExcelChartSeriesItem s;
                if (chart.ChartNode.LocalName == "scatterChart")
                    s = new ExcelScatterChartSeriesItem(this, ns, n, isPivot);
                else if (chart.ChartNode.LocalName == "lineChart")
                    s = new ExcelLineChartSeriesItem(this, ns, n, isPivot);
                else if (chart.ChartNode.LocalName == "pieChart" ||
                         chart.ChartNode.LocalName == "ofPieChart" ||
                         chart.ChartNode.LocalName == "pie3DChart" ||
                         chart.ChartNode.LocalName == "doughnutChart")
                    s = new ExcelPieChartSeriesItem(this, ns, n, isPivot);
                else if (chart.ChartNode.LocalName == "bubbleChart")
                    s = new ExcelBubbleChartSeriesItem(this, ns, n, isPivot);
                else
                    s = new ExcelChartSeriesItem(this, ns, n, isPivot);
                _list.Add(s);
            }
    }

    /// <summary>
    ///     A reference to the chart object
    /// </summary>
    public ExcelChart Chart => _chart;

    #region IEnumerable Members

    public IEnumerator GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    /// <summary>
    ///     Returns the serie at the specified position.
    /// </summary>
    /// <param name="positionId">The position of the series.</param>
    /// <returns></returns>
    public ExcelChartSeriesItem this[int positionId] => _list[positionId];

    public int Count => _list.Count;

    /// <summary>
    ///     Delete the chart at the specific position
    /// </summary>
    /// <param name="positionId">Zero based</param>
    public void Delete(int positionId)
    {
        var ser = _list[positionId];
        if (ser?.TopNode?.ParentNode != null) ser.TopNode.ParentNode.RemoveChild(ser.TopNode);
        _list.RemoveAt(positionId);
    }

    #endregion

    #region Add Series

    /// <summary>
    ///     Add a new series item to the chart. Do not apply to pivotcharts.
    /// </summary>
    /// <param name="series">The Y-Axis range</param>
    /// <param name="xSeries">The X-Axis range</param>
    /// <returns></returns>
    public virtual ExcelChartSeriesItem Add(ExcelRangeBase series, ExcelRangeBase xSeries)
    {
        if (_chart.PivotTableSource != null) throw new InvalidOperationException("Can't add a serie to a pivotchart");
        return AddSeries(series.FullAddressAbsolute, xSeries.FullAddressAbsolute, "");
    }

    /// <summary>
    ///     Add a new serie to the chart.Do not apply to pivotcharts.
    /// </summary>
    /// <param name="seriesAddress">The Y-Axis range</param>
    /// <param name="xSeriesAddress">The X-Axis range</param>
    /// <returns></returns>
    public virtual ExcelChartSeriesItem Add(string seriesAddress, string xSeriesAddress)
    {
        if (_chart.PivotTableSource != null) throw new InvalidOperationException("Can't add a serie to a pivotchart");
        return AddSeries(seriesAddress, xSeriesAddress, "");
    }

    protected internal ExcelChartSeriesItem AddSeries(string seriesAddress, string xSeriesAddress,
        string bubbleSizeAddress)
    {
        if (_node?.OwnerDocument == null)
            throw new InvalidOperationException($"The node's {nameof(_node.OwnerDocument)} value is null.");

        var ser = _node.OwnerDocument.CreateElement("ser", ExcelPackage.SchemaChart);
        var node = _node.SelectNodes("c:ser", _ns);
        if (node!.Count > 0)
            _node.InsertAfter(ser, node[^1]);
        else
            InserAfter(_node, "c:varyColors,c:grouping,c:barDir,c:scatterStyle,c:ofPieType", ser);
        var idx = FindIndex();
        ser.InnerXml = string.Format(
            "<c:idx val=\"{1}\" /><c:order val=\"{1}\" /><c:tx><c:strRef><c:f></c:f><c:strCache><c:ptCount val=\"1\" /></c:strCache></c:strRef></c:tx>{5}{0}{2}{3}{4}",
            AddExplosion(Chart.ChartType), idx, AddScatterPoint(Chart.ChartType), AddAxisNodes(Chart.ChartType),
            AddSmooth(Chart.ChartType), AddMarker(Chart.ChartType));
        ExcelChartSeriesItem series;

        switch (Chart.ChartType)
        {
            case eChartType.Bubble:
            case eChartType.Bubble3DEffect:
                series = new ExcelBubbleChartSeriesItem(this, NameSpaceManager, ser, _isPivot)
                {
                    Bubble3D = Chart.ChartType == eChartType.Bubble3DEffect,
                    Series = seriesAddress,
                    XSeries = xSeriesAddress,
                    BubbleSize = bubbleSizeAddress
                };
                break;
            case eChartType.XYScatter:
            case eChartType.XYScatterLines:
            case eChartType.XYScatterLinesNoMarkers:
            case eChartType.XYScatterSmooth:
            case eChartType.XYScatterSmoothNoMarkers:
                series = new ExcelScatterChartSeriesItem(this, NameSpaceManager, ser, _isPivot);
                break;
            case eChartType.Radar:
            case eChartType.RadarFilled:
            case eChartType.RadarMarkers:
                series = new ExcelRadarChartSeriesItem(this, NameSpaceManager, ser, _isPivot);
                break;
            case eChartType.Surface:
            case eChartType.SurfaceTopView:
            case eChartType.SurfaceTopViewWireframe:
            case eChartType.SurfaceWireframe:
                series = new ExcelSurfaceChartSeriesItem(this, NameSpaceManager, ser, _isPivot);
                break;
            case eChartType.Pie:
            case eChartType.Pie3D:
            case eChartType.PieExploded:
            case eChartType.PieExploded3D:
            case eChartType.PieOfPie:
            case eChartType.Doughnut:
            case eChartType.DoughnutExploded:
            case eChartType.BarOfPie:
                series = new ExcelPieChartSeriesItem(this, NameSpaceManager, ser, _isPivot);
                break;
            case eChartType.Line:
            case eChartType.LineMarkers:
            case eChartType.LineMarkersStacked:
            case eChartType.LineMarkersStacked100:
            case eChartType.LineStacked:
            case eChartType.LineStacked100:
                series = new ExcelLineChartSeriesItem(this, NameSpaceManager, ser, _isPivot);
                if (Chart.ChartType == eChartType.LineMarkers ||
                    Chart.ChartType == eChartType.LineMarkersStacked ||
                    Chart.ChartType == eChartType.LineMarkersStacked100)
                    ((ExcelLineChartSeriesItem)series).Marker = eMarkerStyle.Square;
                ((ExcelLineChartSeriesItem)series).Smooth = ((ExcelLineChart)Chart).Smooth;
                break;
            case eChartType.BarClustered:
            case eChartType.BarStacked:
            case eChartType.BarStacked100:
            case eChartType.ColumnClustered:
            case eChartType.ColumnStacked:
            case eChartType.ColumnStacked100:
            case eChartType.BarClustered3D:
            case eChartType.BarStacked3D:
            case eChartType.BarStacked1003D:
            case eChartType.ColumnClustered3D:
            case eChartType.ColumnStacked3D:
            case eChartType.ColumnStacked1003D:
            case eChartType.ConeBarClustered:
            case eChartType.ConeBarStacked:
            case eChartType.ConeBarStacked100:
            case eChartType.ConeCol:
            case eChartType.ConeColClustered:
            case eChartType.ConeColStacked:
            case eChartType.ConeColStacked100:
            case eChartType.CylinderBarClustered:
            case eChartType.CylinderBarStacked:
            case eChartType.CylinderBarStacked100:
            case eChartType.CylinderCol:
            case eChartType.CylinderColClustered:
            case eChartType.CylinderColStacked:
            case eChartType.CylinderColStacked100:
            case eChartType.PyramidBarClustered:
            case eChartType.PyramidBarStacked:
            case eChartType.PyramidBarStacked100:
            case eChartType.PyramidCol:
            case eChartType.PyramidColClustered:
            case eChartType.PyramidColStacked:
            case eChartType.PyramidColStacked100:
                series = new ExcelBarChartSeriesItem(this, NameSpaceManager, ser, _isPivot);
                ((ExcelBarChartSeriesItem)series).InvertIfNegative = false;
                break;
            default:
                series = new ExcelChartSeriesItem(this, NameSpaceManager, ser, _isPivot);
                break;
        }

        series.Series = seriesAddress;
        series.XSeries = xSeriesAddress;
        _list.Add(series);
        return series;
    }

    private bool _isPivot;

    internal void AddPivotSeries(ExcelPivotTable pivotTableSource)
    {
        var r = pivotTableSource.WorkSheet.Cells[pivotTableSource.Address.Address];
        _isPivot = true;
        AddSeries(r.Offset(0, 1, r._toRow - r._fromRow + 1, 1).FullAddressAbsolute,
            r.Offset(0, 0, r._toRow - r._fromRow + 1, 1).FullAddressAbsolute, "");
    }

    private int FindIndex()
    {
        int ret = 0, newId = 0;
        if (_chart.PlotArea.ChartTypes.Count > 1)
        {
            foreach (var chart in _chart.PlotArea.ChartTypes)
                if (newId > 0)
                {
                    foreach (ExcelChartSeriesItem series in chart.Series) series.SetId((++newId).ToString());
                }
                else
                {
                    if (chart == _chart)
                    {
                        ret += _list.Count + 1;
                        newId = ret;
                    }
                    else
                    {
                        ret += chart.Series.Count;
                    }
                }

            return ret - 1;
        }

        return _list.Count;
    }

    #endregion

    #region Xml init Functions

    private string AddMarker(eChartType chartType)
    {
        if (chartType == eChartType.Line ||
            chartType == eChartType.LineStacked ||
            chartType == eChartType.LineStacked100 ||
            chartType == eChartType.XYScatterLines ||
            chartType == eChartType.XYScatterSmooth ||
            chartType == eChartType.XYScatterLinesNoMarkers ||
            chartType == eChartType.XYScatterSmoothNoMarkers)
            return "<c:marker><c:symbol val=\"none\" /></c:marker>";
        return "";
    }

    private string AddScatterPoint(eChartType chartType)
    {
        if (chartType == eChartType.XYScatter)
            return "<c:spPr><a:ln w=\"28575\"><a:noFill /></a:ln></c:spPr>";
        return "";
    }

    private string AddAxisNodes(eChartType chartType)
    {
        if (chartType == eChartType.XYScatter ||
            chartType == eChartType.XYScatterLines ||
            chartType == eChartType.XYScatterLinesNoMarkers ||
            chartType == eChartType.XYScatterSmooth ||
            chartType == eChartType.XYScatterSmoothNoMarkers ||
            chartType == eChartType.Bubble ||
            chartType == eChartType.Bubble3DEffect)
            return "<c:xVal /><c:yVal />";
        return "<c:val />";
    }

    private string AddExplosion(eChartType chartType)
    {
        if (chartType == eChartType.PieExploded3D ||
            chartType == eChartType.PieExploded ||
            chartType == eChartType.DoughnutExploded)
            return "<c:explosion val=\"25\" />"; //Default 25;
        return "";
    }

    private string AddSmooth(eChartType chartType)
    {
        if (chartType == eChartType.XYScatterSmooth ||
            chartType == eChartType.XYScatterSmoothNoMarkers)
            return "<c:smooth val=\"1\" />"; //Default 25;
        return "";
    }

    #endregion
}