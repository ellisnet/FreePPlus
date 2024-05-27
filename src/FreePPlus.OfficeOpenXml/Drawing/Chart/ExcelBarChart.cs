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
using System.Globalization;
using System.Xml;
using OfficeOpenXml.Packaging;
using OfficeOpenXml.Table.PivotTable;

#pragma warning disable CA1822
#pragma warning disable IDE0059
#pragma warning disable IDE0060

namespace OfficeOpenXml.Drawing.Chart;

/// <summary>
///     Bar chart
/// </summary>
public sealed class ExcelBarChart : ExcelChart
{
    internal override eChartType GetChartType(string name)
    {
        if (name == "barChart")
        {
            if (Direction == eDirection.Bar)
            {
                if (Grouping == eGrouping.Stacked)
                    return eChartType.BarStacked;
                if (Grouping == eGrouping.PercentStacked)
                    return eChartType.BarStacked100;
                return eChartType.BarClustered;
            }

            if (Grouping == eGrouping.Stacked)
                return eChartType.ColumnStacked;
            if (Grouping == eGrouping.PercentStacked)
                return eChartType.ColumnStacked100;
            return eChartType.ColumnClustered;
        }

        if (name == "bar3DChart")
        {
            #region Bar Shape

            if (Shape == eShape.Box)
            {
                if (Direction == eDirection.Bar)
                {
                    if (Grouping == eGrouping.Stacked)
                        return eChartType.BarStacked3D;
                    if (Grouping == eGrouping.PercentStacked)
                        return eChartType.BarStacked1003D;
                    return eChartType.BarClustered3D;
                }

                if (Grouping == eGrouping.Stacked)
                    return eChartType.ColumnStacked3D;
                if (Grouping == eGrouping.PercentStacked)
                    return eChartType.ColumnStacked1003D;
                return eChartType.ColumnClustered3D;
            }

            #endregion

            #region Cone Shape

            if (Shape == eShape.Cone || Shape == eShape.ConeToMax)
            {
                if (Direction == eDirection.Bar)
                {
                    if (Grouping == eGrouping.Stacked)
                        return eChartType.ConeBarStacked;
                    if (Grouping == eGrouping.PercentStacked)
                        return eChartType.ConeBarStacked100;
                    if (Grouping == eGrouping.Clustered) return eChartType.ConeBarClustered;
                }
                else
                {
                    if (Grouping == eGrouping.Stacked)
                        return eChartType.ConeColStacked;
                    if (Grouping == eGrouping.PercentStacked)
                        return eChartType.ConeColStacked100;
                    if (Grouping == eGrouping.Clustered)
                        return eChartType.ConeColClustered;
                    return eChartType.ConeCol;
                }
            }

            #endregion

            #region Cylinder Shape

            if (Shape == eShape.Cylinder)
            {
                if (Direction == eDirection.Bar)
                {
                    if (Grouping == eGrouping.Stacked)
                        return eChartType.CylinderBarStacked;
                    if (Grouping == eGrouping.PercentStacked)
                        return eChartType.CylinderBarStacked100;
                    if (Grouping == eGrouping.Clustered) return eChartType.CylinderBarClustered;
                }
                else
                {
                    if (Grouping == eGrouping.Stacked)
                        return eChartType.CylinderColStacked;
                    if (Grouping == eGrouping.PercentStacked)
                        return eChartType.CylinderColStacked100;
                    if (Grouping == eGrouping.Clustered)
                        return eChartType.CylinderColClustered;
                    return eChartType.CylinderCol;
                }
            }

            #endregion

            #region Pyramid Shape

            if (Shape == eShape.Pyramid || Shape == eShape.PyramidToMax)
            {
                if (Direction == eDirection.Bar)
                {
                    if (Grouping == eGrouping.Stacked)
                        return eChartType.PyramidBarStacked;
                    if (Grouping == eGrouping.PercentStacked)
                        return eChartType.PyramidBarStacked100;
                    if (Grouping == eGrouping.Clustered) return eChartType.PyramidBarClustered;
                }
                else
                {
                    if (Grouping == eGrouping.Stacked)
                        return eChartType.PyramidColStacked;
                    if (Grouping == eGrouping.PercentStacked)
                        return eChartType.PyramidColStacked100;
                    if (Grouping == eGrouping.Clustered)
                        return eChartType.PyramidColClustered;
                    return eChartType.PyramidCol;
                }
            }

            #endregion
        }

        return base.GetChartType(name);
    }

    #region Constructors

    //internal ExcelBarChart(ExcelDrawings drawings, XmlNode node) :
    //    base(drawings, node/*, 1*/)
    //{
    //    SetChartNodeText("");
    //}
    //internal ExcelBarChart(ExcelDrawings drawings, XmlNode node, eChartType type) :
    //    base(drawings, node, type)
    //{
    //    SetChartNodeText("");

    //    SetTypeProperties(drawings, type);
    //}
    internal ExcelBarChart(ExcelDrawings drawings, XmlNode node, eChartType type, ExcelChart topChart,
        ExcelPivotTable pivotTableSource) :
        base(drawings, node, type, topChart, pivotTableSource)
    {
        SetChartNodeText("");
        SetTypeProperties(drawings, type);
    }

    internal ExcelBarChart(ExcelDrawings drawings, XmlNode node, Uri uriChart, ZipPackagePart part,
        XmlDocument chartXml, XmlNode chartNode) :
        base(drawings, node, uriChart, part, chartXml, chartNode)
    {
        SetChartNodeText(chartNode.Name);
    }

    internal ExcelBarChart(ExcelChart topChart, XmlNode chartNode) :
        base(topChart, chartNode)
    {
        SetChartNodeText(chartNode.Name);
    }

    #endregion

    #region Private functions

    //string _chartTopPath="c:chartSpace/c:chart/c:plotArea/{0}";
    private void SetChartNodeText(string chartNodeText)
    {
        if (string.IsNullOrEmpty(chartNodeText))
            // ReSharper disable once RedundantAssignment
            chartNodeText = GetChartNodeText();
        //_chartTopPath = string.Format(_chartTopPath, chartNodeText);
        //_directionPath = string.Format(_directionPath, _chartTopPath);
        //_shapePath = string.Format(_shapePath, _chartTopPath);
    }

    // ReSharper disable once UnusedParameter.Local
    private void SetTypeProperties(ExcelDrawings drawings, eChartType type)
    {
        switch (type)
        {
            /******* Bar direction *******/
            case eChartType.BarClustered:
            case eChartType.BarStacked:
            case eChartType.BarStacked100:
            case eChartType.BarClustered3D:
            case eChartType.BarStacked3D:
            case eChartType.BarStacked1003D:
            case eChartType.ConeBarClustered:
            case eChartType.ConeBarStacked:
            case eChartType.ConeBarStacked100:
            case eChartType.CylinderBarClustered:
            case eChartType.CylinderBarStacked:
            case eChartType.CylinderBarStacked100:
            case eChartType.PyramidBarClustered:
            case eChartType.PyramidBarStacked:
            case eChartType.PyramidBarStacked100:
                Direction = eDirection.Bar;
                break;

            case eChartType.ColumnClustered:
            case eChartType.ColumnStacked:
            case eChartType.ColumnStacked100:
            case eChartType.Column3D:
            case eChartType.ColumnClustered3D:
            case eChartType.ColumnStacked3D:
            case eChartType.ColumnStacked1003D:
            case eChartType.ConeCol:
            case eChartType.ConeColClustered:
            case eChartType.ConeColStacked:
            case eChartType.ConeColStacked100:
            case eChartType.CylinderCol:
            case eChartType.CylinderColClustered:
            case eChartType.CylinderColStacked:
            case eChartType.CylinderColStacked100:
            case eChartType.PyramidCol:
            case eChartType.PyramidColClustered:
            case eChartType.PyramidColStacked:
            case eChartType.PyramidColStacked100:
                Direction = eDirection.Column;
                break;
        }

        switch (type)
        {
            /****** Shape ******/
            case eChartType.Column3D:
            case eChartType.ColumnClustered3D:
            case eChartType.ColumnStacked3D:
            case eChartType.ColumnStacked1003D:
            case eChartType.BarClustered3D:
            case eChartType.BarStacked3D:
            /*type == eChartType.ColumnClustered ||
            type == eChartType.ColumnStacked ||
            type == eChartType.ColumnStacked100 ||*/
            case eChartType.BarStacked1003D:
                Shape = eShape.Box;
                break;

            case eChartType.CylinderBarClustered:
            case eChartType.CylinderBarStacked:
            case eChartType.CylinderBarStacked100:
            case eChartType.CylinderCol:
            case eChartType.CylinderColClustered:
            case eChartType.CylinderColStacked:
            case eChartType.CylinderColStacked100:
                Shape = eShape.Cylinder;
                break;

            case eChartType.ConeBarClustered:
            case eChartType.ConeBarStacked:
            case eChartType.ConeBarStacked100:
            case eChartType.ConeCol:
            case eChartType.ConeColClustered:
            case eChartType.ConeColStacked:
            case eChartType.ConeColStacked100:
                Shape = eShape.Cone;
                break;

            case eChartType.PyramidBarClustered:
            case eChartType.PyramidBarStacked:
            case eChartType.PyramidBarStacked100:
            case eChartType.PyramidCol:
            case eChartType.PyramidColClustered:
            case eChartType.PyramidColStacked:
            case eChartType.PyramidColStacked100:
                Shape = eShape.Pyramid;
                break;
        }
    }

    #endregion

    #region Properties

    private const string DirectionPath = "c:barDir/@val";

    /// <summary>
    ///     Direction, Bar or columns
    /// </summary>
    public eDirection Direction
    {
        get => GetDirectionEnum(_chartXmlHelper.GetXmlNodeString(DirectionPath));
        internal set => _chartXmlHelper.SetXmlNodeString(DirectionPath, GetDirectionText(value));
    }

    private const string ShapePath = "c:shape/@val";

    /// <summary>
    ///     The shape of the bar/columns
    /// </summary>
    public eShape Shape
    {
        get => GetShapeEnum(_chartXmlHelper.GetXmlNodeString(ShapePath));
        internal set => _chartXmlHelper.SetXmlNodeString(ShapePath, GetShapeText(value));
    }

    private ExcelChartDataLabel _dataLabel;

    /// <summary>
    ///     Access to datalabel properties
    /// </summary>
    public ExcelChartDataLabel DataLabel => _dataLabel ??= new ExcelChartDataLabel(NameSpaceManager, ChartNode);

    private const string GapWidthPath = "c:gapWidth/@val";

    /// <summary>
    ///     The size of the gap between two adjacent bars/columns
    /// </summary>
    public int GapWidth
    {
        get => _chartXmlHelper.GetXmlNodeInt(GapWidthPath);
        set => _chartXmlHelper.SetXmlNodeString(GapWidthPath, value.ToString(CultureInfo.InvariantCulture));
    }

    #endregion

    #region Direction Enum Traslation

    private string GetDirectionText(eDirection direction)
    {
        return direction switch
        {
            eDirection.Bar => "bar",
            _ => "col"
        };
    }

    private eDirection GetDirectionEnum(string direction)
    {
        return direction switch
        {
            "bar" => eDirection.Bar,
            _ => eDirection.Column
        };
    }

    #endregion

    #region Shape Enum Translation

    private string GetShapeText(eShape shape)
    {
        return shape switch
        {
            eShape.Box => "box",
            eShape.Cone => "cone",
            eShape.ConeToMax => "coneToMax",
            eShape.Cylinder => "cylinder",
            eShape.Pyramid => "pyramid",
            eShape.PyramidToMax => "pyramidToMax",
            _ => "box"
        };
    }

    private eShape GetShapeEnum(string text)
    {
        return text switch
        {
            "box" => eShape.Box,
            "cone" => eShape.Cone,
            "coneToMax" => eShape.ConeToMax,
            "cylinder" => eShape.Cylinder,
            "pyramid" => eShape.Pyramid,
            "pyramidToMax" => eShape.PyramidToMax,
            _ => eShape.Box
        };
    }

    #endregion
}