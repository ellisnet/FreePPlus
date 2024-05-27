﻿/*******************************************************************************
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
using System.Diagnostics;
using FreePPlus.Imaging.Fonts;

namespace OfficeOpenXml.Style;

/// <summary>
///     Cell style Font
/// </summary>
public sealed class ExcelFont : StyleBase
{
    internal ExcelFont(ExcelStyles styles, XmlHelper.ChangedEventHandler ChangedEvent, int PositionID, string address,
        int index) :
        base(styles, ChangedEvent, PositionID, address)

    {
        Index = index;
    }

    /// <summary>
    ///     The name of the font
    /// </summary>
    public string Name
    {
        get => _styles.Fonts[Index].Name;
        set => _ChangedEvent(this,
            new StyleChangeEventArgs(eStyleClass.Font, eStyleProperty.Name, value, _positionID, _address));
    }

    /// <summary>
    ///     The Size of the font
    /// </summary>
    public float Size
    {
        get => _styles.Fonts[Index].Size;
        set => _ChangedEvent(this,
            new StyleChangeEventArgs(eStyleClass.Font, eStyleProperty.Size, value, _positionID, _address));
    }

    /// <summary>
    ///     Font family
    /// </summary>
    public int Family
    {
        get => _styles.Fonts[Index].Family;
        set => _ChangedEvent(this,
            new StyleChangeEventArgs(eStyleClass.Font, eStyleProperty.Family, value, _positionID, _address));
    }

    /// <summary>
    ///     Cell color
    /// </summary>
    public ExcelColor Color => new(_styles, _ChangedEvent, _positionID, _address, eStyleClass.Font, this);

    /// <summary>
    ///     Scheme
    /// </summary>
    public string Scheme
    {
        get => _styles.Fonts[Index].Scheme;
        set => _ChangedEvent(this,
            new StyleChangeEventArgs(eStyleClass.Font, eStyleProperty.Scheme, value, _positionID, _address));
    }

    /// <summary>
    ///     Font-bold
    /// </summary>
    public bool Bold
    {
        get => _styles.Fonts[Index].Bold;
        set => _ChangedEvent(this,
            new StyleChangeEventArgs(eStyleClass.Font, eStyleProperty.Bold, value, _positionID, _address));
    }

    /// <summary>
    ///     Font-italic
    /// </summary>
    public bool Italic
    {
        get => _styles.Fonts[Index].Italic;
        set => _ChangedEvent(this,
            new StyleChangeEventArgs(eStyleClass.Font, eStyleProperty.Italic, value, _positionID, _address));
    }

    /// <summary>
    ///     Font-Strikeout
    /// </summary>
    public bool Strike
    {
        get => _styles.Fonts[Index].Strike;
        set => _ChangedEvent(this,
            new StyleChangeEventArgs(eStyleClass.Font, eStyleProperty.Strike, value, _positionID, _address));
    }

    /// <summary>
    ///     Font-Underline
    /// </summary>
    public bool UnderLine
    {
        get => _styles.Fonts[Index].UnderLine;
        set => UnderLineType = value ? ExcelUnderLineType.Single : ExcelUnderLineType.None;
        //_ChangedEvent(this, new StyleChangeEventArgs(eStyleClass.Font, eStyleProperty.UnderlineType, value, _positionID, _address));
    }

    public ExcelUnderLineType UnderLineType
    {
        get => _styles.Fonts[Index].UnderLineType;
        set => _ChangedEvent(this,
            new StyleChangeEventArgs(eStyleClass.Font, eStyleProperty.UnderlineType, value, _positionID, _address));
    }

    /// <summary>
    ///     Font-Vertical Align
    /// </summary>
    public ExcelVerticalAlignmentFont VerticalAlign
    {
        get
        {
            if (_styles.Fonts[Index].VerticalAlign == "")
                return ExcelVerticalAlignmentFont.None;
            return (ExcelVerticalAlignmentFont)Enum.Parse(typeof(ExcelVerticalAlignmentFont),
                _styles.Fonts[Index].VerticalAlign, true);
        }
        set => _ChangedEvent(this,
            new StyleChangeEventArgs(eStyleClass.Font, eStyleProperty.VerticalAlign, value, _positionID, _address));
    }

    internal override string Id => Name + Size + Family + Scheme + Bold.ToString()[0] + Italic.ToString()[0] +
                                   Strike.ToString()[0] + UnderLine.ToString()[0] + VerticalAlign;

    /// <summary>
    ///     Set the font from a Font object
    /// </summary>
    /// <param name="font"></param>
    public void SetFromFont(Font font)
    {
        //Somehow getting the font "Arial Bold" from somewhere, but font doesn't seem to exist in Windows anymore.
        //  Swapping out normal Arial
        var isBold = font.IsBold;
        var name = font.Name;

        if ((!string.IsNullOrWhiteSpace(name)) && name.Trim().ToLowerInvariant() == "arial bold")
        {
            name = "Arial";
            isBold = true;
        }

        Name = name;
        //Family=fnt.FontFamily.;
        Size = (int)font.Size;
        Strike = font.IsStrikeout;
        Bold = isBold;
        UnderLine = font.IsUnderline;
        Italic = font.IsItalic;
    }
}