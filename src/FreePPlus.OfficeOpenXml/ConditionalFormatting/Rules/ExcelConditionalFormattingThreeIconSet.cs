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
 * ******************************************************************************
 * Eyal Seagull        Added       		  2012-04-03
 *******************************************************************************/

using System;
using System.Xml;
using OfficeOpenXml.ConditionalFormatting.Contracts;

// ReSharper disable once CheckNamespace
namespace OfficeOpenXml.ConditionalFormatting;

public class
    ExcelConditionalFormattingThreeIconSet : ExcelConditionalFormattingIconSetBase<
    eExcelconditionalFormatting3IconsSetType>
{
    internal ExcelConditionalFormattingThreeIconSet(
        ExcelAddress address,
        int priority,
        ExcelWorksheet worksheet,
        XmlNode itemElementNode,
        XmlNamespaceManager namespaceManager)
        : base(
            eExcelConditionalFormattingRuleType.ThreeIconSet,
            address,
            priority,
            worksheet,
            itemElementNode,
            namespaceManager == null ? worksheet.NameSpaceManager : namespaceManager) { }
}

/// <summary>
///     ExcelConditionalFormattingThreeIconSet
/// </summary>
public class ExcelConditionalFormattingIconSetBase<T>
    : ExcelConditionalFormattingRule,
        IExcelConditionalFormattingThreeIconSet<T>
{
    private const string ReversePath = "d:iconSet/@reverse";

    private const string ShowValuePath = "d:iconSet/@showValue";

    private const string IconSetPath = "d:iconSet/@iconSet";

    /// <summary>
    ///     Settings for icon 1 in the iconset
    /// </summary>
    public ExcelConditionalFormattingIconDataBarValue Icon1 { get; internal set; }

    /// <summary>
    ///     Settings for icon 2 in the iconset
    /// </summary>
    public ExcelConditionalFormattingIconDataBarValue Icon2 { get; internal set; }

    /// <summary>
    ///     Settings for icon 2 in the iconset
    /// </summary>
    public ExcelConditionalFormattingIconDataBarValue Icon3 { get; internal set; }

    /// <summary>
    ///     Reverse the order of the icons
    /// </summary>
    public bool Reverse
    {
        get => GetXmlNodeBool(ReversePath, false);
        set => SetXmlNodeBool(ReversePath, value);
    }

    /// <summary>
    ///     If the cell values are visible
    /// </summary>
    public bool ShowValue
    {
        get => GetXmlNodeBool(ShowValuePath, true);
        set => SetXmlNodeBool(ShowValuePath, value);
    }

    /// <summary>
    ///     Type of iconset
    /// </summary>
    public T IconSet
    {
        get
        {
            var v = GetXmlNodeString(IconSetPath);
            v = v[1..]; //Skip first icon.
            return (T)Enum.Parse(typeof(T), v, true);
        }
        set => SetXmlNodeString(IconSetPath, GetIconSetString(value));
    }

    private string GetIconSetString(T value)
    {
        if (Type == eExcelConditionalFormattingRuleType.FourIconSet)
            switch (value.ToString())
            {
                case "Arrows":
                    return "4Arrows";
                case "ArrowsGray":
                    return "4ArrowsGray";
                case "Rating":
                    return "4Rating";
                case "RedToBlack":
                    return "4RedToBlack";
                case "TrafficLights":
                    return "4TrafficLights";
                default:
                    throw new ArgumentException("Invalid type");
            }

        if (Type == eExcelConditionalFormattingRuleType.FiveIconSet)
            switch (value.ToString())
            {
                case "Arrows":
                    return "5Arrows";
                case "ArrowsGray":
                    return "5ArrowsGray";
                case "Quarters":
                    return "5Quarters";
                case "Rating":
                    return "5Rating";
                default:
                    throw new ArgumentException("Invalid type");
            }

        switch (value.ToString())
        {
            case "Arrows":
                return "3Arrows";
            case "ArrowsGray":
                return "3ArrowsGray";
            case "Flags":
                return "3Flags";
            case "Signs":
                return "3Signs";
            case "Symbols":
                return "3Symbols";
            case "Symbols2":
                return "3Symbols2";
            case "TrafficLights1":
                return "3TrafficLights1";
            case "TrafficLights2":
                return "3TrafficLights2";
            default:
                throw new ArgumentException("Invalid type");
        }
    }
    /****************************************************************************************/

    #region Private Properties

    #endregion

    /****************************************************************************************/

    #region Constructors

    /// <summary>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="address"></param>
    /// <param name="priority"></param>
    /// <param name="worksheet"></param>
    /// <param name="itemElementNode"></param>
    /// <param name="namespaceManager"></param>
    internal ExcelConditionalFormattingIconSetBase(
        eExcelConditionalFormattingRuleType type,
        ExcelAddress address,
        int priority,
        ExcelWorksheet worksheet,
        XmlNode itemElementNode,
        XmlNamespaceManager namespaceManager)
        : base(
            type,
            address,
            priority,
            worksheet,
            itemElementNode,
            namespaceManager == null ? worksheet.NameSpaceManager : namespaceManager)
    {
        if (itemElementNode != null && itemElementNode.HasChildNodes)
        {
            var pos = 1;
            var iconListNodes = itemElementNode.SelectNodes("d:iconSet/d:cfvo", NameSpaceManager);

            if (iconListNodes != null)
                foreach (XmlNode node in iconListNodes)
                {
                    if (pos == 1)
                        Icon1 = new ExcelConditionalFormattingIconDataBarValue(
                            type,
                            address,
                            worksheet,
                            node,
                            namespaceManager);
                    else if (pos == 2)
                        Icon2 = new ExcelConditionalFormattingIconDataBarValue(
                            type,
                            address,
                            worksheet,
                            node,
                            namespaceManager);
                    else if (pos == 3)
                        Icon3 = new ExcelConditionalFormattingIconDataBarValue(
                            type,
                            address,
                            worksheet,
                            node,
                            namespaceManager);
                    else
                        break;
                    pos++;
                }
        }
        else
        {
            var iconSetNode = CreateComplexNode(
                Node,
                ExcelConditionalFormattingConstants.Paths.IconSet);

            //Create the <iconSet> node inside the <cfRule> node
            double spann;
            if (type == eExcelConditionalFormattingRuleType.ThreeIconSet)
                spann = 3;
            else if (type == eExcelConditionalFormattingRuleType.FourIconSet)
                spann = 4;
            else
                spann = 5;

            var iconNode1 = iconSetNode?.OwnerDocument?.CreateElement(ExcelConditionalFormattingConstants.Paths.Cfvo,
                ExcelPackage.SchemaMain);
            if (iconNode1 != null)
            {
                iconSetNode.AppendChild(iconNode1);
                Icon1 = new ExcelConditionalFormattingIconDataBarValue(
                    eExcelConditionalFormattingValueObjectType.Percent,
                    0,
                    "",
                    eExcelConditionalFormattingRuleType.ThreeIconSet,
                    address,
                    priority,
                    worksheet,
                    iconNode1,
                    namespaceManager);
            }

            var iconNode2 = iconSetNode?.OwnerDocument?.CreateElement(ExcelConditionalFormattingConstants.Paths.Cfvo,
                ExcelPackage.SchemaMain);
            if (iconNode2 != null)
            {
                iconSetNode.AppendChild(iconNode2);
                Icon2 = new ExcelConditionalFormattingIconDataBarValue(
                    eExcelConditionalFormattingValueObjectType.Percent,
                    Math.Round(100D / spann, 0),
                    "",
                    eExcelConditionalFormattingRuleType.ThreeIconSet,
                    address,
                    priority,
                    worksheet,
                    iconNode2,
                    namespaceManager);
            }

            var iconNode3 = iconSetNode?.OwnerDocument?.CreateElement(ExcelConditionalFormattingConstants.Paths.Cfvo,
                ExcelPackage.SchemaMain);
            if (iconNode3 != null)
            {
                iconSetNode.AppendChild(iconNode3);
                Icon3 = new ExcelConditionalFormattingIconDataBarValue(
                    eExcelConditionalFormattingValueObjectType.Percent,
                    Math.Round(100D * (2D / spann), 0),
                    "",
                    eExcelConditionalFormattingRuleType.ThreeIconSet,
                    address,
                    priority,
                    worksheet,
                    iconNode3,
                    namespaceManager);
            }

            Type = type;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="priority"></param>
    /// <param name="address"></param>
    /// <param name="worksheet"></param>
    /// <param name="itemElementNode"></param>
    internal ExcelConditionalFormattingIconSetBase(
        eExcelConditionalFormattingRuleType type,
        ExcelAddress address,
        int priority,
        ExcelWorksheet worksheet,
        XmlNode itemElementNode)
        : this(
            type,
            address,
            priority,
            worksheet,
            itemElementNode,
            null) { }

    /// <summary>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="priority"></param>
    /// <param name="address"></param>
    /// <param name="worksheet"></param>
    internal ExcelConditionalFormattingIconSetBase(
        eExcelConditionalFormattingRuleType type,
        ExcelAddress address,
        int priority,
        ExcelWorksheet worksheet)
        : this(
            type,
            address,
            priority,
            worksheet,
            null,
            null) { }

    #endregion
}