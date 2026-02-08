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
 * Jan Källman		                Initial Release		        2009-10-01
 * Jan Källman		License changed GPL-->LGPL 2011-12-16
 *******************************************************************************/

using System;
using System.Globalization;
using System.Xml;
using CodeBrix.Imaging;

namespace OfficeOpenXml.Style.XmlAccess;

/// <summary>
///     Xml access class xfs records. This is the top level style object.
/// </summary>
public sealed class ExcelXfs : StyleXmlHelper
{
    private readonly ExcelStyles _styles;

    internal ExcelXfs(XmlNamespaceManager nameSpaceManager, ExcelStyles styles) : base(nameSpaceManager)
    {
        _styles = styles;
        isBuildIn = false;
    }

    internal ExcelXfs(XmlNamespaceManager nsm, XmlNode topNode, ExcelStyles styles) :
        base(nsm, topNode)
    {
        _styles = styles;
        XfId = GetXmlNodeInt("@xfId");
        if (XfId == 0) isBuildIn = true; //Normal taggen
        _numFmtId = GetXmlNodeInt("@numFmtId");
        FontId = GetXmlNodeInt("@fontId");
        FillId = GetXmlNodeInt("@fillId");
        BorderId = GetXmlNodeInt("@borderId");
        ReadingOrder = GetReadingOrder(GetXmlNodeString(readingOrderPath));
        _indent = GetXmlNodeInt(indentPath);
        ShrinkToFit = GetXmlNodeString(shrinkToFitPath) == "1" ? true : false;
        VerticalAlignment = GetVerticalAlign(GetXmlNodeString(verticalAlignPath));
        HorizontalAlignment = GetHorizontalAlign(GetXmlNodeString(horizontalAlignPath));
        WrapText = GetXmlNodeBool(wrapTextPath);
        _textRotation = GetXmlNodeInt(textRotationPath);
        Hidden = GetXmlNodeBool(hiddenPath);
        Locked = GetXmlNodeBool(lockedPath, true);
        QuotePrefix = GetXmlNodeBool(quotePrefixPath);
    }

    /// <summary>
    ///     Style index
    /// </summary>
    public int XfId { get; set; }

    internal override string Id => XfId + "|" + NumberFormatId + "|" + FontId + "|" + FillId + "|" + BorderId +
                                   VerticalAlignment + "|" + HorizontalAlignment + "|" + WrapText + "|" + ReadingOrder +
                                   "|" + isBuildIn + TextRotation + Locked + Hidden + ShrinkToFit + Indent +
                                   QuotePrefix;

    //return Numberformat.Id + "|" + Font.Id + "|" + Fill.Id + "|" + Border.Id + VerticalAlignment.ToString() + "|" + HorizontalAlignment.ToString() + "|" + WrapText.ToString() + "|" + ReadingOrder.ToString(); 
    private ExcelReadingOrder GetReadingOrder(string value)
    {
        switch (value)
        {
            case "1":
                return ExcelReadingOrder.LeftToRight;
            case "2":
                return ExcelReadingOrder.RightToLeft;
            default:
                return ExcelReadingOrder.ContextDependent;
        }
    }

    private ExcelHorizontalAlignment GetHorizontalAlign(string align)
    {
        if (align == "") return ExcelHorizontalAlignment.General;
        align = align[..1].ToUpper(CultureInfo.InvariantCulture) + align.Substring(1, align.Length - 1);
        try
        {
            return (ExcelHorizontalAlignment)Enum.Parse(typeof(ExcelHorizontalAlignment), align);
        }
        catch
        {
            return ExcelHorizontalAlignment.General;
        }
    }

    private ExcelVerticalAlignment GetVerticalAlign(string align)
    {
        if (align == "") return ExcelVerticalAlignment.Bottom;
        align = align[..1].ToUpper(CultureInfo.InvariantCulture) + align.Substring(1, align.Length - 1);
        try
        {
            return (ExcelVerticalAlignment)Enum.Parse(typeof(ExcelVerticalAlignment), align);
        }
        catch
        {
            return ExcelVerticalAlignment.Bottom;
        }
    }

    internal void Xf_ChangedEvent(object sender, EventArgs e)
    {
        //if (_cell != null)
        //{
        //    if (!Styles.ChangedCells.ContainsKey(_cell.Id))
        //    {
        //        //_cell.Style = "";
        //        _cell.SetNewStyleID(int.MinValue.ToString());
        //        Styles.ChangedCells.Add(_cell.Id, _cell);
        //    }
        //}
    }

    internal void RegisterEvent(ExcelXfs xf)
    {
        //                RegisterEvent(xf, xf.Xf_ChangedEvent);
    }

    internal ExcelXfs Copy()
    {
        return Copy(_styles);
    }

    internal ExcelXfs Copy(ExcelStyles styles)
    {
        var newXF = new ExcelXfs(NameSpaceManager, styles);
        newXF.NumberFormatId = _numFmtId;
        newXF.FontId = FontId;
        newXF.FillId = FillId;
        newXF.BorderId = BorderId;
        newXF.XfId = XfId;
        newXF.ReadingOrder = ReadingOrder;
        newXF.HorizontalAlignment = HorizontalAlignment;
        newXF.VerticalAlignment = VerticalAlignment;
        newXF.WrapText = WrapText;
        newXF.ShrinkToFit = ShrinkToFit;
        newXF.Indent = _indent;
        newXF.TextRotation = _textRotation;
        newXF.Locked = Locked;
        newXF.Hidden = Hidden;
        newXF.QuotePrefix = QuotePrefix;
        return newXF;
    }

    internal int GetNewID(ExcelStyleCollection<ExcelXfs> xfsCol, StyleBase styleObject, eStyleClass styleClass,
        eStyleProperty styleProperty, object value)
    {
        var newXfs = Copy();
        switch (styleClass)
        {
            case eStyleClass.Numberformat:
                newXfs.NumberFormatId = GetIdNumberFormat(styleProperty, value);
                styleObject.SetIndex(newXfs.NumberFormatId);
                break;
            case eStyleClass.Font:
            {
                newXfs.FontId = GetIdFont(styleProperty, value);
                styleObject.SetIndex(newXfs.FontId);
                break;
            }
            case eStyleClass.Fill:
            case eStyleClass.FillBackgroundColor:
            case eStyleClass.FillPatternColor:
                newXfs.FillId = GetIdFill(styleClass, styleProperty, value);
                styleObject.SetIndex(newXfs.FillId);
                break;
            case eStyleClass.GradientFill:
            case eStyleClass.FillGradientColor1:
            case eStyleClass.FillGradientColor2:
                newXfs.FillId = GetIdGradientFill(styleClass, styleProperty, value);
                styleObject.SetIndex(newXfs.FillId);
                break;
            case eStyleClass.Border:
            case eStyleClass.BorderBottom:
            case eStyleClass.BorderDiagonal:
            case eStyleClass.BorderLeft:
            case eStyleClass.BorderRight:
            case eStyleClass.BorderTop:
                newXfs.BorderId = GetIdBorder(styleClass, styleProperty, value);
                styleObject.SetIndex(newXfs.BorderId);
                break;
            case eStyleClass.Style:
                switch (styleProperty)
                {
                    case eStyleProperty.XfId:
                        newXfs.XfId = (int)value;
                        break;
                    case eStyleProperty.HorizontalAlign:
                        newXfs.HorizontalAlignment = (ExcelHorizontalAlignment)value;
                        break;
                    case eStyleProperty.VerticalAlign:
                        newXfs.VerticalAlignment = (ExcelVerticalAlignment)value;
                        break;
                    case eStyleProperty.WrapText:
                        newXfs.WrapText = (bool)value;
                        break;
                    case eStyleProperty.ReadingOrder:
                        newXfs.ReadingOrder = (ExcelReadingOrder)value;
                        break;
                    case eStyleProperty.ShrinkToFit:
                        newXfs.ShrinkToFit = (bool)value;
                        break;
                    case eStyleProperty.Indent:
                        newXfs.Indent = (int)value;
                        break;
                    case eStyleProperty.TextRotation:
                        newXfs.TextRotation = (int)value;
                        break;
                    case eStyleProperty.Locked:
                        newXfs.Locked = (bool)value;
                        break;
                    case eStyleProperty.Hidden:
                        newXfs.Hidden = (bool)value;
                        break;
                    case eStyleProperty.QuotePrefix:
                        newXfs.QuotePrefix = (bool)value;
                        break;
                    default:
                        throw new Exception("Invalid property for class style.");
                }

                break;
        }

        var id = xfsCol.FindIndexByID(newXfs.Id);
        if (id < 0) return xfsCol.Add(newXfs.Id, newXfs);
        return id;
    }

    private int GetIdBorder(eStyleClass styleClass, eStyleProperty styleProperty, object value)
    {
        var border = Border.Copy();

        switch (styleClass)
        {
            case eStyleClass.BorderBottom:
                SetBorderItem(border.Bottom, styleProperty, value);
                break;
            case eStyleClass.BorderDiagonal:
                SetBorderItem(border.Diagonal, styleProperty, value);
                break;
            case eStyleClass.BorderLeft:
                SetBorderItem(border.Left, styleProperty, value);
                break;
            case eStyleClass.BorderRight:
                SetBorderItem(border.Right, styleProperty, value);
                break;
            case eStyleClass.BorderTop:
                SetBorderItem(border.Top, styleProperty, value);
                break;
            case eStyleClass.Border:
                if (styleProperty == eStyleProperty.BorderDiagonalUp)
                    border.DiagonalUp = (bool)value;
                else if (styleProperty == eStyleProperty.BorderDiagonalDown)
                    border.DiagonalDown = (bool)value;
                else
                    throw new Exception("Invalid property for class Border.");
                break;
            default:
                throw new Exception("Invalid class/property for class Border.");
        }

        int subId;
        var id = border.Id;
        subId = _styles.Borders.FindIndexByID(id);
        if (subId == int.MinValue) return _styles.Borders.Add(id, border);
        return subId;
    }

    private void SetBorderItem(ExcelBorderItemXml excelBorderItem, eStyleProperty styleProperty, object value)
    {
        if (styleProperty == eStyleProperty.Style)
        {
            excelBorderItem.Style = (ExcelBorderStyle)value;
        }
        else if (styleProperty == eStyleProperty.Color || styleProperty == eStyleProperty.Tint ||
                 styleProperty == eStyleProperty.IndexedColor)
        {
            if (excelBorderItem.Style == ExcelBorderStyle.None)
                throw new Exception("Can't set bordercolor when style is not set.");
            excelBorderItem.Color.Rgb = value.ToString();
        }
    }

    private int GetIdFill(eStyleClass styleClass, eStyleProperty styleProperty, object value)
    {
        var fill = Fill.Copy();

        switch (styleProperty)
        {
            case eStyleProperty.PatternType:
                if (fill is ExcelGradientFillXml) fill = new ExcelFillXml(NameSpaceManager);
                fill.PatternType = (ExcelFillStyle)value;
                break;
            case eStyleProperty.Color:
            case eStyleProperty.Tint:
            case eStyleProperty.IndexedColor:
            case eStyleProperty.AutoColor:
                if (fill is ExcelGradientFillXml) fill = new ExcelFillXml(NameSpaceManager);
                if (fill.PatternType == ExcelFillStyle.None)
                    throw new ArgumentException("Can't set color when patterntype is not set.");
                ExcelColorXml destColor;
                if (styleClass == eStyleClass.FillPatternColor)
                    destColor = fill.PatternColor;
                else
                    destColor = fill.BackgroundColor;

                if (styleProperty == eStyleProperty.Color)
                    destColor.Rgb = value.ToString();
                else if (styleProperty == eStyleProperty.Tint)
                    destColor.Tint = (decimal)value;
                else if (styleProperty == eStyleProperty.IndexedColor)
                    destColor.Indexed = (int)value;
                else
                    destColor.Auto = (bool)value;

                break;
            default:
                throw new ArgumentException("Invalid class/property for class Fill.");
        }

        int subId;
        var id = fill.Id;
        subId = _styles.Fills.FindIndexByID(id);
        if (subId == int.MinValue) return _styles.Fills.Add(id, fill);
        return subId;
    }

    private int GetIdGradientFill(eStyleClass styleClass, eStyleProperty styleProperty, object value)
    {
        ExcelGradientFillXml fill;
        if (Fill is ExcelGradientFillXml)
        {
            fill = (ExcelGradientFillXml)Fill.Copy();
        }
        else
        {
            fill = new ExcelGradientFillXml(Fill.NameSpaceManager);
            fill.GradientColor1.SetColor(Color.White);
            fill.GradientColor2.SetColor(Color.FromArgb(79, 129, 189));
            fill.Type = ExcelFillGradientType.Linear;
            fill.Degree = 90;
            fill.Top = double.NaN;
            fill.Bottom = double.NaN;
            fill.Left = double.NaN;
            fill.Right = double.NaN;
        }

        switch (styleProperty)
        {
            case eStyleProperty.GradientType:
                fill.Type = (ExcelFillGradientType)value;
                break;
            case eStyleProperty.GradientDegree:
                fill.Degree = (double)value;
                break;
            case eStyleProperty.GradientTop:
                fill.Top = (double)value;
                break;
            case eStyleProperty.GradientBottom:
                fill.Bottom = (double)value;
                break;
            case eStyleProperty.GradientLeft:
                fill.Left = (double)value;
                break;
            case eStyleProperty.GradientRight:
                fill.Right = (double)value;
                break;
            case eStyleProperty.Color:
            case eStyleProperty.Tint:
            case eStyleProperty.IndexedColor:
            case eStyleProperty.AutoColor:
                ExcelColorXml destColor;

                if (styleClass == eStyleClass.FillGradientColor1)
                    destColor = fill.GradientColor1;
                else
                    destColor = fill.GradientColor2;

                if (styleProperty == eStyleProperty.Color)
                    destColor.Rgb = value.ToString();
                else if (styleProperty == eStyleProperty.Tint)
                    destColor.Tint = (decimal)value;
                else if (styleProperty == eStyleProperty.IndexedColor)
                    destColor.Indexed = (int)value;
                else
                    destColor.Auto = (bool)value;
                break;
            default:
                throw new ArgumentException("Invalid class/property for class Fill.");
        }

        int subId;
        var id = fill.Id;
        subId = _styles.Fills.FindIndexByID(id);
        if (subId == int.MinValue) return _styles.Fills.Add(id, fill);
        return subId;
    }

    private int GetIdNumberFormat(eStyleProperty styleProperty, object value)
    {
        if (styleProperty == eStyleProperty.Format)
        {
            ExcelNumberFormatXml item = null;
            if (!_styles.NumberFormats.FindByID(value.ToString(), ref item))
            {
                item = new ExcelNumberFormatXml(NameSpaceManager)
                    { Format = value.ToString(), NumFmtId = _styles.NumberFormats.NextId++ };
                _styles.NumberFormats.Add(value.ToString(), item);
            }

            return item.NumFmtId;
        }

        throw new Exception("Invalid property for class Numberformat");
    }

    private int GetIdFont(eStyleProperty styleProperty, object value)
    {
        var fnt = Font.Copy();

        switch (styleProperty)
        {
            case eStyleProperty.Name:
                fnt.Name = value.ToString();
                break;
            case eStyleProperty.Size:
                fnt.Size = (float)value;
                break;
            case eStyleProperty.Family:
                fnt.Family = (int)value;
                break;
            case eStyleProperty.Bold:
                fnt.Bold = (bool)value;
                break;
            case eStyleProperty.Italic:
                fnt.Italic = (bool)value;
                break;
            case eStyleProperty.Strike:
                fnt.Strike = (bool)value;
                break;
            case eStyleProperty.UnderlineType:
                fnt.UnderLineType = (ExcelUnderLineType)value;
                break;
            case eStyleProperty.Color:
                fnt.Color.Rgb = value.ToString();
                break;
            case eStyleProperty.Tint:
                fnt.Color.Tint = (decimal)value;
                break;
            case eStyleProperty.IndexedColor:
                fnt.Color.Indexed = (int)value;
                break;
            case eStyleProperty.AutoColor:
                fnt.Color.Auto = (bool)value;
                break;
            case eStyleProperty.VerticalAlign:
                fnt.VerticalAlign = (ExcelVerticalAlignmentFont)value == ExcelVerticalAlignmentFont.None
                    ? ""
                    : value.ToString().ToLower(CultureInfo.InvariantCulture);
                break;
            default:
                throw new Exception("Invalid property for class Font");
        }

        int subId;
        var id = fnt.Id;
        subId = _styles.Fonts.FindIndexByID(id);
        if (subId == int.MinValue) return _styles.Fonts.Add(id, fnt);
        return subId;
    }

    internal override XmlNode CreateXmlNode(XmlNode topNode)
    {
        return CreateXmlNode(topNode, false);
    }

    internal XmlNode CreateXmlNode(XmlNode topNode, bool isCellStyleXsf)
    {
        TopNode = topNode;
        var doSetXfId = !isCellStyleXsf && XfId > int.MinValue && _styles.CellStyleXfs.Count > 0 &&
                        _styles.CellStyleXfs[XfId].newID > int.MinValue;
        if (_numFmtId >= 0)
        {
            SetXmlNodeString("@numFmtId", _numFmtId.ToString());
            if (doSetXfId) SetXmlNodeString("@applyNumberFormat", "1");
        }

        if (FontId >= 0)
        {
            SetXmlNodeString("@fontId", _styles.Fonts[FontId].newID.ToString());
            if (doSetXfId) SetXmlNodeString("@applyFont", "1");
        }

        if (FillId >= 0)
        {
            SetXmlNodeString("@fillId", _styles.Fills[FillId].newID.ToString());
            if (doSetXfId) SetXmlNodeString("@applyFill", "1");
        }

        if (BorderId >= 0)
        {
            SetXmlNodeString("@borderId", _styles.Borders[BorderId].newID.ToString());
            if (doSetXfId) SetXmlNodeString("@applyBorder", "1");
        }

        if (HorizontalAlignment != ExcelHorizontalAlignment.General)
            SetXmlNodeString(horizontalAlignPath, SetAlignString(HorizontalAlignment));
        if (doSetXfId) SetXmlNodeString("@xfId", _styles.CellStyleXfs[XfId].newID.ToString());
        if (VerticalAlignment != ExcelVerticalAlignment.Bottom)
            SetXmlNodeString(verticalAlignPath, SetAlignString(VerticalAlignment));
        if (WrapText) SetXmlNodeString(wrapTextPath, "1");
        if (ReadingOrder != ExcelReadingOrder.ContextDependent)
            SetXmlNodeString(readingOrderPath, ((int)ReadingOrder).ToString());
        if (ShrinkToFit) SetXmlNodeString(shrinkToFitPath, "1");
        if (_indent > 0) SetXmlNodeString(indentPath, _indent.ToString());
        if (_textRotation > 0) SetXmlNodeString(textRotationPath, _textRotation.ToString());
        if (!Locked) SetXmlNodeString(lockedPath, "0");
        if (Hidden) SetXmlNodeString(hiddenPath, "1");
        if (QuotePrefix) SetXmlNodeString(quotePrefixPath, "1");

        if ((Locked || Hidden) && doSetXfId) SetXmlNodeString("@applyProtection", "1");

        if ((HorizontalAlignment != ExcelHorizontalAlignment.General ||
             VerticalAlignment != ExcelVerticalAlignment.Bottom) && doSetXfId) SetXmlNodeString("@applyAlignment", "1");

        return TopNode;
    }

    private string SetAlignString(Enum align)
    {
        var newName = Enum.GetName(align.GetType(), align);
        return newName[..1].ToLower(CultureInfo.InvariantCulture) + newName.Substring(1, newName.Length - 1);
    }

    #region Internal Properties

    private int _numFmtId;

    internal int NumberFormatId
    {
        get => _numFmtId;
        set
        {
            _numFmtId = value;
            ApplyNumberFormat = value > 0;
        }
    }

    internal int FontId { get; set; }

    internal int FillId { get; set; }

    internal int BorderId { get; set; }

    private bool isBuildIn { get; }

    internal bool ApplyNumberFormat { get; set; }

    internal bool ApplyFont { get; set; }

    internal bool ApplyFill { get; set; }

    internal bool ApplyBorder { get; set; }

    internal bool ApplyAlignment { get; set; }

    internal bool ApplyProtection { get; set; }

    #endregion

    #region Public Properties

    public ExcelStyles Styles { get; }

    /// <summary>
    ///     Numberformat properties
    /// </summary>
    public ExcelNumberFormatXml Numberformat => _styles.NumberFormats[_numFmtId < 0 ? 0 : _numFmtId];

    /// <summary>
    ///     Font properties
    /// </summary>
    public ExcelFontXml Font => _styles.Fonts[FontId < 0 ? 0 : FontId];

    /// <summary>
    ///     Fill properties
    /// </summary>
    public ExcelFillXml Fill => _styles.Fills[FillId < 0 ? 0 : FillId];

    /// <summary>
    ///     Border style properties
    /// </summary>
    public ExcelBorderXml Border => _styles.Borders[BorderId < 0 ? 0 : BorderId];

    private const string horizontalAlignPath = "d:alignment/@horizontal";

    /// <summary>
    ///     Horizontal alignment
    /// </summary>
    public ExcelHorizontalAlignment HorizontalAlignment { get; set; } = ExcelHorizontalAlignment.General;

    private const string verticalAlignPath = "d:alignment/@vertical";

    /// <summary>
    ///     Vertical alignment
    /// </summary>
    public ExcelVerticalAlignment VerticalAlignment { get; set; } = ExcelVerticalAlignment.Bottom;

    private const string wrapTextPath = "d:alignment/@wrapText";

    /// <summary>
    ///     Wraped text
    /// </summary>
    public bool WrapText { get; set; }

    private readonly string textRotationPath = "d:alignment/@textRotation";
    private int _textRotation;

    /// <summary>
    ///     Text rotation angle
    /// </summary>
    public int TextRotation
    {
        get => _textRotation == int.MinValue ? 0 : _textRotation;
        set => _textRotation = value;
    }

    private const string lockedPath = "d:protection/@locked";

    /// <summary>
    ///     Locked when sheet is protected
    /// </summary>
    public bool Locked { get; set; } = true;

    private const string hiddenPath = "d:protection/@hidden";

    /// <summary>
    ///     Hide formulas when sheet is protected
    /// </summary>
    public bool Hidden { get; set; }

    private const string quotePrefixPath = "@quotePrefix";

    /// <summary>
    ///     Prefix the formula with a quote.
    /// </summary>
    public bool QuotePrefix { get; set; }

    private const string readingOrderPath = "d:alignment/@readingOrder";

    /// <summary>
    ///     Readingorder
    /// </summary>
    public ExcelReadingOrder ReadingOrder { get; set; } = ExcelReadingOrder.ContextDependent;

    private const string shrinkToFitPath = "d:alignment/@shrinkToFit";

    /// <summary>
    ///     Shrink to fit
    /// </summary>
    public bool ShrinkToFit { get; set; }

    private const string indentPath = "d:alignment/@indent";
    private int _indent;

    /// <summary>
    ///     Indentation
    /// </summary>
    public int Indent
    {
        get => _indent == int.MinValue ? 0 : _indent;
        set => _indent = value;
    }

    #endregion
}