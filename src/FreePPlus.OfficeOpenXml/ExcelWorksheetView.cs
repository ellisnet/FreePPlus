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
 * Jan Källman		    Initial Release		       2009-10-01
 * Jan Källman		    License changed GPL-->LGPL 2011-12-27
 *******************************************************************************/

using System;
using System.Xml;

namespace OfficeOpenXml;

/// <summary>
///     Represents the different view states of the worksheet
/// </summary>
public class ExcelWorksheetView : XmlHelper
{
    private readonly ExcelWorksheet _worksheet;

    #region ExcelWorksheetView Constructor

    /// <summary>
    ///     Creates a new ExcelWorksheetView which provides access to all the view states of the worksheet.
    /// </summary>
    /// <param name="ns"></param>
    /// <param name="node"></param>
    /// <param name="xlWorksheet"></param>
    internal ExcelWorksheetView(XmlNamespaceManager ns, XmlNode node, ExcelWorksheet xlWorksheet) :
        base(ns, node)
    {
        _worksheet = xlWorksheet;
        SchemaNodeOrder = new[] { "sheetViews", "sheetView", "pane", "selection" };
        Panes = LoadPanes();
    }

    #endregion

    #region SheetViewElement

    /// <summary>
    ///     Returns a reference to the sheetView element
    /// </summary>
    protected internal XmlElement SheetViewElement => (XmlElement)TopNode;

    #endregion

    private ExcelWorksheetPanes[] LoadPanes()
    {
        var nodes = TopNode.SelectNodes("//d:selection", NameSpaceManager);
        if (nodes.Count == 0) return new ExcelWorksheetPanes[] { new(NameSpaceManager, TopNode) };

        var panes = new ExcelWorksheetPanes[nodes.Count];
        var i = 0;
        foreach (XmlElement elem in nodes) panes[i++] = new ExcelWorksheetPanes(NameSpaceManager, elem);
        return panes;
    }

    /// <summary>
    ///     The worksheet panes after a freeze or split.
    /// </summary>
    public class ExcelWorksheetPanes : XmlHelper
    {
        private const string _activeCellPath = "@activeCell";
        private const string _selectionRangePath = "@sqref";
        private XmlElement _selectionNode;

        internal ExcelWorksheetPanes(XmlNamespaceManager ns, XmlNode topNode) :
            base(ns, topNode)
        {
            if (topNode.Name == "selection") _selectionNode = topNode as XmlElement;
        }

        /// <summary>
        ///     Set the active cell. Must be set within the SelectedRange.
        /// </summary>
        public string ActiveCell
        {
            get
            {
                var address = GetXmlNodeString(_activeCellPath);
                if (address == "") return "A1";
                return address;
            }
            set
            {
                int fromCol, fromRow, toCol, toRow;
                if (_selectionNode == null) CreateSelectionElement();
                ExcelCellBase.GetRowColFromAddress(value, out fromRow, out fromCol, out toRow, out toCol);
                SetXmlNodeString(_activeCellPath, value);
                if (((XmlElement)TopNode).GetAttribute("sqref") == "")
                    SelectedRange = ExcelCellBase.GetAddress(fromRow, fromCol);
                //TODO:Add fix for out of range here
            }
        }

        /// <summary>
        ///     Selected Cells.Used in combination with ActiveCell
        /// </summary>
        public string SelectedRange
        {
            get
            {
                var address = GetXmlNodeString(_selectionRangePath);
                if (address == "") return "A1";
                return address;
            }
            set
            {
                int fromCol, fromRow, toCol, toRow;
                if (_selectionNode == null) CreateSelectionElement();
                ExcelCellBase.GetRowColFromAddress(value, out fromRow, out fromCol, out toRow, out toCol);
                SetXmlNodeString(_selectionRangePath, value);
                if (((XmlElement)TopNode).GetAttribute("activeCell") == "")
                    ActiveCell = ExcelCellBase.GetAddress(fromRow, fromCol);
                //TODO:Add fix for out of range here
            }
        }

        private void CreateSelectionElement()
        {
            _selectionNode = TopNode.OwnerDocument.CreateElement("selection", ExcelPackage.SchemaMain);
            TopNode.AppendChild(_selectionNode);
            TopNode = _selectionNode;
        }
    }

    #region TabSelected

    private XmlElement _selectionNode;

    private XmlElement SelectionNode
    {
        get
        {
            _selectionNode =
                SheetViewElement.SelectSingleNode("//d:selection", _worksheet.NameSpaceManager) as XmlElement;
            if (_selectionNode == null)
            {
                _selectionNode = _worksheet.WorksheetXml.CreateElement("selection", ExcelPackage.SchemaMain);
                SheetViewElement.AppendChild(_selectionNode);
            }

            return _selectionNode;
        }
    }

    #endregion

    #region Public Methods & Properties

    /// <summary>
    ///     The active cell. Single Cell address.
    ///     This cell must be inside the selected range. If not, the selected range is set to the active cell address
    /// </summary>
    public string ActiveCell
    {
        get => Panes[Panes.GetUpperBound(0)].ActiveCell;
        set
        {
            var ac = new ExcelAddressBase(value);
            if (ac.IsSingleCell == false) throw new InvalidOperationException("ActiveCell must be a single cell.");

            /*** Active cell must be inside SelectedRange ***/
            var sd = new ExcelAddressBase(SelectedRange.Replace(" ", ","));
            Panes[Panes.GetUpperBound(0)].ActiveCell = value;

            if (IsActiveCellInSelection(ac, sd) == false) SelectedRange = value;
        }
    }

    /// <summary>
    ///     Selected Cells in the worksheet. Used in combination with ActiveCell.
    ///     If the active cell is not inside the selected range, the active cell will be set to the first cell in the selected
    ///     range.
    ///     If the selected range has multiple adresses, these are separated with space. If the active cell is not within the
    ///     first address in this list, the attribute ActiveCellId must be set (not supported, so it must be set via the XML).
    /// </summary>
    public string SelectedRange
    {
        get => Panes[Panes.GetUpperBound(0)].SelectedRange;
        set
        {
            var ac = new ExcelAddressBase(ActiveCell);

            /*** Active cell must be inside SelectedRange ***/
            var sd = new ExcelAddressBase(value.Replace(" ", ",")); //Space delimitered here, replace

            Panes[Panes.GetUpperBound(0)].SelectedRange = value;
            if (IsActiveCellInSelection(ac, sd) == false)
                ActiveCell = new ExcelCellAddress(sd._fromRow, sd._fromCol).Address;
        }
    }

    private bool IsActiveCellInSelection(ExcelAddressBase ac, ExcelAddressBase sd)
    {
        var c = sd.Collide(ac);
        if (c == ExcelAddressBase.eAddressCollition.Equal || c == ExcelAddressBase.eAddressCollition.Inside)
            return true;

        if (sd.Addresses != null)
            foreach (var sds in sd.Addresses)
            {
                c = sds.Collide(ac);
                if (c == ExcelAddressBase.eAddressCollition.Equal ||
                    c == ExcelAddressBase.eAddressCollition.Inside) return true;
            }

        return false;
    }

    /// <summary>
    ///     Indicates if the worksheet is selected within the workbook. NOTE: Setter clears other selected tabs.
    /// </summary>
    public bool TabSelected
    {
        get => GetXmlNodeBool("@tabSelected");
        set => SetTabSelected(value);
    }

    /// <summary>
    ///     Indicates if the worksheet is selected within the workbook. NOTE: Setter keeps other selected tabs.
    /// </summary>
    public bool TabSelectedMulti
    {
        get => GetXmlNodeBool("@tabSelected");
        set => SetTabSelected(value, true);
    }

    /// <summary>
    ///     Sets whether the worksheet is selected within the workbook.
    /// </summary>
    /// <param name="isSelected">Whether the tab is selected, defaults to true.</param>
    /// <param name="allowMultiple">Whether to allow multiple active tabs, defaults to false.</param>
    public void SetTabSelected(bool isSelected = true, bool allowMultiple = false)
    {
        if (isSelected)
        {
            SheetViewElement.SetAttribute("tabSelected", "1");
            if (!allowMultiple)
                //    // ensure no other worksheet has its tabSelected attribute set to 1
                foreach (var sheet in _worksheet._package.Workbook.Worksheets)
                    sheet.View.TabSelected = false;
            var bookView =
                _worksheet.Workbook.WorkbookXml.SelectSingleNode("//d:workbookView", _worksheet.NameSpaceManager) as
                    XmlElement;
            if (bookView != null) bookView.SetAttribute("activeTab", (_worksheet.PositionID - 1).ToString());
        }
        else
        {
            SetXmlNodeString("@tabSelected", "0");
        }
    }

    /// <summary>
    ///     Sets the view mode of the worksheet to pagelayout
    /// </summary>
    public bool PageLayoutView
    {
        get => GetXmlNodeString("@view") == "pageLayout";
        set
        {
            if (value)
                SetXmlNodeString("@view", "pageLayout");
            else
                SheetViewElement.RemoveAttribute("view");
        }
    }

    /// <summary>
    ///     Sets the view mode of the worksheet to pagebreak
    /// </summary>
    public bool PageBreakView
    {
        get => GetXmlNodeString("@view") == "pageBreakPreview";
        set
        {
            if (value)
                SetXmlNodeString("@view", "pageBreakPreview");
            else
                SheetViewElement.RemoveAttribute("view");
        }
    }

    /// <summary>
    ///     Show gridlines in the worksheet
    /// </summary>
    public bool ShowGridLines
    {
        get => GetXmlNodeBool("@showGridLines", true);
        set => SetXmlNodeString("@showGridLines", value ? "1" : "0");
    }

    /// <summary>
    ///     Show the Column/Row headers (containg column letters and row numbers)
    /// </summary>
    public bool ShowHeaders
    {
        get => GetXmlNodeBool("@showRowColHeaders");
        set => SetXmlNodeString("@showRowColHeaders", value ? "1" : "0");
    }

    /// <summary>
    ///     Window zoom magnification for current view representing percent values.
    /// </summary>
    public int ZoomScale
    {
        get => GetXmlNodeInt("@zoomScale");
        set
        {
            if (value < 10 || value > 400) throw new ArgumentOutOfRangeException("Zoome scale out of range (10-400)");
            SetXmlNodeString("@zoomScale", value.ToString());
        }
    }

    /// <summary>
    ///     Flag indicating whether the sheet is in 'right to left' display mode. When in this mode,Column A is on the far
    ///     right, Column B ;is one column left of Column A, and so on. Also,information in cells is displayed in the Right to
    ///     Left format.
    /// </summary>
    public bool RightToLeft
    {
        get => GetXmlNodeBool("@rightToLeft");
        set => SetXmlNodeString("@rightToLeft", value ? "1" : "0");
    }

    internal bool WindowProtection
    {
        get => GetXmlNodeBool("@windowProtection", false);
        set => SetXmlNodeBool("@windowProtection", value, false);
    }

    /// <summary>
    ///     Reference to the panes
    /// </summary>
    public ExcelWorksheetPanes[] Panes { get; internal set; }

    private readonly string _paneNodePath = "d:pane";
    private readonly string _selectionNodePath = "d:selection";

    /// <summary>
    ///     Freeze the columns/rows to left and above the cell
    /// </summary>
    /// <param name="Row"></param>
    /// <param name="Column"></param>
    public void FreezePanes(int Row, int Column)
    {
        //TODO:fix this method to handle splits as well.
        if (Row == 1 && Column == 1) UnFreezePanes();
        string sqRef = SelectedRange, activeCell = ActiveCell;

        var paneNode = TopNode.SelectSingleNode(_paneNodePath, NameSpaceManager) as XmlElement;
        if (paneNode == null)
        {
            CreateNode(_paneNodePath);
            paneNode = TopNode.SelectSingleNode(_paneNodePath, NameSpaceManager) as XmlElement;
        }

        paneNode.RemoveAll(); //Clear all attributes
        if (Column > 1) paneNode.SetAttribute("xSplit", (Column - 1).ToString());
        if (Row > 1) paneNode.SetAttribute("ySplit", (Row - 1).ToString());
        paneNode.SetAttribute("topLeftCell", ExcelCellBase.GetAddress(Row, Column));
        paneNode.SetAttribute("state", "frozen");

        RemoveSelection();

        if (Row > 1 && Column == 1)
        {
            paneNode.SetAttribute("activePane", "bottomLeft");
            var sel = TopNode.OwnerDocument.CreateElement("selection", ExcelPackage.SchemaMain);
            sel.SetAttribute("pane", "bottomLeft");
            if (activeCell != "") sel.SetAttribute("activeCell", activeCell);
            if (sqRef != "") sel.SetAttribute("sqref", sqRef);
            sel.SetAttribute("sqref", sqRef);
            TopNode.InsertAfter(sel, paneNode);
        }
        else if (Column > 1 && Row == 1)
        {
            paneNode.SetAttribute("activePane", "topRight");
            var sel = TopNode.OwnerDocument.CreateElement("selection", ExcelPackage.SchemaMain);
            sel.SetAttribute("pane", "topRight");
            if (activeCell != "") sel.SetAttribute("activeCell", activeCell);
            if (sqRef != "") sel.SetAttribute("sqref", sqRef);
            TopNode.InsertAfter(sel, paneNode);
        }
        else
        {
            paneNode.SetAttribute("activePane", "bottomRight");
            var sel1 = TopNode.OwnerDocument.CreateElement("selection", ExcelPackage.SchemaMain);
            sel1.SetAttribute("pane", "topRight");
            var cell = ExcelCellBase.GetAddress(1, Column);
            sel1.SetAttribute("activeCell", cell);
            sel1.SetAttribute("sqref", cell);
            paneNode.ParentNode.InsertAfter(sel1, paneNode);

            var sel2 = TopNode.OwnerDocument.CreateElement("selection", ExcelPackage.SchemaMain);
            cell = ExcelCellBase.GetAddress(Row, 1);
            sel2.SetAttribute("pane", "bottomLeft");
            sel2.SetAttribute("activeCell", cell);
            sel2.SetAttribute("sqref", cell);
            sel1.ParentNode.InsertAfter(sel2, sel1);

            var sel3 = TopNode.OwnerDocument.CreateElement("selection", ExcelPackage.SchemaMain);
            sel3.SetAttribute("pane", "bottomRight");
            if (activeCell != "") sel3.SetAttribute("activeCell", activeCell);
            if (sqRef != "") sel3.SetAttribute("sqref", sqRef);
            sel2.ParentNode.InsertAfter(sel3, sel2);
        }

        Panes = LoadPanes();
    }

    private void RemoveSelection()
    {
        //Find selection nodes and remove them            
        var selections = TopNode.SelectNodes(_selectionNodePath, NameSpaceManager);
        foreach (XmlNode sel in selections) sel.ParentNode.RemoveChild(sel);
    }

    /// <summary>
    ///     Unlock all rows and columns to scroll freely
    ///     ///
    /// </summary>
    public void UnFreezePanes()
    {
        string sqRef = SelectedRange, activeCell = ActiveCell;

        var paneNode = TopNode.SelectSingleNode(_paneNodePath, NameSpaceManager) as XmlElement;
        if (paneNode != null) paneNode.ParentNode.RemoveChild(paneNode);
        RemoveSelection();

        Panes = LoadPanes();

        SelectedRange = sqRef;
        ActiveCell = activeCell;
    }

    #endregion
}