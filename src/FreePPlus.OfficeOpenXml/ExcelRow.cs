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
 * Jan Källman		    Initial Release		        2009-10-01
 * Jan Källman		    License changed GPL-->LGPL 2011-12-27
 *******************************************************************************/

using System;
using System.Xml;
using OfficeOpenXml.Style;

namespace OfficeOpenXml;

internal class RowInternal
{
    internal bool Collapsed;
    internal bool CustomHeight;
    internal double Height = -1;
    internal bool Hidden;
    internal int MergeID;
    internal short OutlineLevel;
    internal bool PageBreak;
    internal bool Phonetic;

    internal RowInternal Clone()
    {
        return new RowInternal
        {
            Height = Height,
            Hidden = Hidden,
            Collapsed = Collapsed,
            OutlineLevel = OutlineLevel,
            PageBreak = PageBreak,
            Phonetic = Phonetic,
            CustomHeight = CustomHeight,
            MergeID = MergeID
        };
    }
}

/// <summary>
///     Represents an individual row in the spreadsheet.
/// </summary>
public class ExcelRow : IRangeID
{
    private readonly XmlElement _rowElement = null;
    private readonly ExcelWorksheet _worksheet;

    internal string _styleName = "";

    #region ExcelRow Constructor

    /// <summary>
    ///     Creates a new instance of the ExcelRow class.
    ///     For internal use only!
    /// </summary>
    /// <param name="Worksheet">The parent worksheet</param>
    /// <param name="row">The row number</param>
    internal ExcelRow(ExcelWorksheet Worksheet, int row)
    {
        _worksheet = Worksheet;
        Row = row;
    }

    #endregion

    /// <summary>
    ///     Internal RowID.
    /// </summary>
    [Obsolete]
    public ulong RowID => GetRowID(_worksheet.SheetID, Row);

    /// <summary>
    ///     Provides access to the node representing the row.
    /// </summary>
    internal XmlNode Node => _rowElement;

    #region ExcelRow Hidden

    /// <summary>
    ///     Allows the row to be hidden in the worksheet
    /// </summary>
    public bool Hidden
    {
        get
        {
            var r = (RowInternal)_worksheet.GetValueInner(Row, 0);
            if (r == null)
                return false;
            return r.Hidden;
        }
        set
        {
            var r = GetRowInternal();
            r.Hidden = value;
        }
    }

    #endregion

    /// <summary>
    ///     Sets the style for the entire column using a style name.
    /// </summary>
    public string StyleName
    {
        get => _styleName;
        set
        {
            StyleID = _worksheet.Workbook.Styles.GetStyleIdFromName(value);
            _styleName = value;
        }
    }

    /// <summary>
    ///     Sets the style for the entire row using the style ID.
    /// </summary>
    public int StyleID
    {
        get => _worksheet.GetStyleInner(Row, 0);
        set => _worksheet.SetStyleInner(Row, 0, value);
    }

    /// <summary>
    ///     Rownumber
    /// </summary>
    public int Row { get; set; }

    /// <summary>
    ///     If outline level is set this tells that the row is collapsed
    /// </summary>
    public bool Collapsed
    {
        get
        {
            var r = (RowInternal)_worksheet.GetValueInner(Row, 0);
            if (r == null)
                return false;
            return r.Collapsed;
        }
        set
        {
            var r = GetRowInternal();
            r.Collapsed = value;
        }
    }

    /// <summary>
    ///     Outline level.
    /// </summary>
    public int OutlineLevel
    {
        get
        {
            var r = (RowInternal)_worksheet.GetValueInner(Row, 0);
            if (r == null)
                return 0;
            return r.OutlineLevel;
        }
        set
        {
            var r = GetRowInternal();
            r.OutlineLevel = (short)value;
        }
    }

    /// <summary>
    ///     Show phonetic Information
    /// </summary>
    public bool Phonetic
    {
        get
        {
            var r = (RowInternal)_worksheet.GetValueInner(Row, 0);
            if (r == null)
                return false;
            return r.Phonetic;
        }
        set
        {
            var r = GetRowInternal();
            r.Phonetic = value;
        }
    }

    /// <summary>
    ///     The Style applied to the whole row. Only effekt cells with no individual style set.
    ///     Use ExcelRange object if you want to set specific styles.
    /// </summary>
    public ExcelStyle Style =>
        _worksheet.Workbook.Styles.GetStyleObject(StyleID, _worksheet.PositionID, Row + ":" + Row);

    /// <summary>
    ///     Adds a manual page break after the row.
    /// </summary>
    public bool PageBreak
    {
        get
        {
            var r = (RowInternal)_worksheet.GetValueInner(Row, 0);
            if (r == null)
                return false;
            return r.PageBreak;
        }
        set
        {
            var r = GetRowInternal();
            r.PageBreak = value;
        }
    }

    public bool Merged
    {
        get => _worksheet.MergedCells[Row, 0] != null;
        set => _worksheet.MergedCells.Add(new ExcelAddressBase(Row, 1, Row, ExcelPackage.MaxColumns), true);
    }

    #region IRangeID Members

    [Obsolete]
    ulong IRangeID.RangeID
    {
        get => RowID;
        set => Row = (int)(value >> 29);
    }

    #endregion

    private RowInternal GetRowInternal()
    {
        var r = (RowInternal)_worksheet.GetValueInner(Row, 0);
        if (r == null)
        {
            r = new RowInternal();
            _worksheet.SetValueInner(Row, 0, r);
        }

        return r;
    }

    internal static ulong GetRowID(int sheetID, int row)
    {
        return (ulong)sheetID + ((ulong)row << 29);
    }

    /// <summary>
    ///     Copies the current row to a new worksheet
    /// </summary>
    /// <param name="added">The worksheet where the copy will be created</param>
    internal void Clone(ExcelWorksheet added)
    {
        var newRow = added.Row(Row);
        newRow.Collapsed = Collapsed;
        newRow.Height = Height;
        newRow.CustomHeight = newRow.CustomHeight;
        newRow.Hidden = Hidden;
        newRow.OutlineLevel = OutlineLevel;
        newRow.PageBreak = PageBreak;
        newRow.Phonetic = Phonetic;
        newRow._styleName = _styleName;
        newRow.StyleID = StyleID;
    }

    #region ExcelRow Height

    /// <summary>
    ///     Sets the height of the row
    /// </summary>
    public double Height
    {
        get
        {
            var r = (RowInternal)_worksheet.GetValueInner(Row, 0);
            if (r == null || r.Height < 0)
                return _worksheet.DefaultRowHeight;
            return r.Height;
        }
        set
        {
            var r = GetRowInternal();
            if (_worksheet._package.DoAdjustDrawings)
            {
                var pos = _worksheet.Drawings.GetDrawingHeight(); //Fixes issue 14846
                r.Height = value;
                _worksheet.Drawings.AdjustHeight(pos);
            }
            else
            {
                r.Height = value;
            }

            if (r.Hidden && value != 0) Hidden = false;
            r.CustomHeight = true;
        }
    }

    /// <summary>
    ///     Set to true if You don't want the row to Autosize
    /// </summary>
    public bool CustomHeight
    {
        get
        {
            var r = (RowInternal)_worksheet.GetValueInner(Row, 0);
            if (r == null)
                return false;
            return r.CustomHeight;
        }
        set
        {
            var r = GetRowInternal();
            r.CustomHeight = value;
        }
    }

    #endregion
}