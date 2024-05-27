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
 * Jan Källman		    Initial Release		        2010-03-14
 * Jan Källman		    License changed GPL-->LGPL 2011-12-27
 *******************************************************************************/

using System.Xml;
using OfficeOpenXml.Encryption;

namespace OfficeOpenXml;

/// <summary>
///     Sheet protection
///     <seealso cref="ExcelEncryption" />
///     <seealso cref="ExcelProtection" />
/// </summary>
public sealed class ExcelSheetProtection : XmlHelper
{
    private const string _isProtectedPath = "d:sheetProtection/@sheet";
    private const string _allowSelectLockedCellsPath = "d:sheetProtection/@selectLockedCells";
    private const string _allowSelectUnlockedCellsPath = "d:sheetProtection/@selectUnlockedCells";
    private const string _allowObjectPath = "d:sheetProtection/@objects";
    private const string _allowScenariosPath = "d:sheetProtection/@scenarios";
    private const string _allowFormatCellsPath = "d:sheetProtection/@formatCells";
    private const string _allowFormatColumnsPath = "d:sheetProtection/@formatColumns";
    private const string _allowFormatRowsPath = "d:sheetProtection/@formatRows";

    private const string _allowInsertColumnsPath = "d:sheetProtection/@insertColumns";

    private const string _allowInsertRowsPath = "d:sheetProtection/@insertRows";
    private const string _allowInsertHyperlinksPath = "d:sheetProtection/@insertHyperlinks";
    private const string _allowDeleteColumns = "d:sheetProtection/@deleteColumns";
    private const string _allowDeleteRowsPath = "d:sheetProtection/@deleteRows";

    private const string _allowSortPath = "d:sheetProtection/@sort";

    private const string _allowAutoFilterPath = "d:sheetProtection/@autoFilter";
    private const string _allowPivotTablesPath = "d:sheetProtection/@pivotTables";

    private const string _passwordPath = "d:sheetProtection/@password";

    internal ExcelSheetProtection(XmlNamespaceManager nsm, XmlNode topNode, ExcelWorksheet ws) :
        base(nsm, topNode)
    {
        SchemaNodeOrder = ws.SchemaNodeOrder;
    }

    /// <summary>
    ///     If the worksheet is protected.
    /// </summary>
    public bool IsProtected
    {
        get => GetXmlNodeBool(_isProtectedPath, false);
        set
        {
            SetXmlNodeBool(_isProtectedPath, value, false);
            if (value)
            {
                AllowEditObject = true;
                AllowEditScenarios = true;
            }
            else
            {
                DeleteAllNode(_isProtectedPath); //delete the whole sheetprotection node
            }
        }
    }

    /// <summary>
    ///     Allow users to select locked cells
    /// </summary>
    public bool AllowSelectLockedCells
    {
        get => !GetXmlNodeBool(_allowSelectLockedCellsPath, false);
        set => SetXmlNodeBool(_allowSelectLockedCellsPath, !value, false);
    }

    /// <summary>
    ///     Allow users to select unlocked cells
    /// </summary>
    public bool AllowSelectUnlockedCells
    {
        get => !GetXmlNodeBool(_allowSelectUnlockedCellsPath, false);
        set => SetXmlNodeBool(_allowSelectUnlockedCellsPath, !value, false);
    }

    /// <summary>
    ///     Allow users to edit objects
    /// </summary>
    public bool AllowEditObject
    {
        get => !GetXmlNodeBool(_allowObjectPath, false);
        set => SetXmlNodeBool(_allowObjectPath, !value, false);
    }

    /// <summary>
    ///     Allow users to edit senarios
    /// </summary>
    public bool AllowEditScenarios
    {
        get => !GetXmlNodeBool(_allowScenariosPath, false);
        set => SetXmlNodeBool(_allowScenariosPath, !value, false);
    }

    /// <summary>
    ///     Allow users to format cells
    /// </summary>
    public bool AllowFormatCells
    {
        get => !GetXmlNodeBool(_allowFormatCellsPath, true);
        set => SetXmlNodeBool(_allowFormatCellsPath, !value, true);
    }

    /// <summary>
    ///     Allow users to Format columns
    /// </summary>
    public bool AllowFormatColumns
    {
        get => !GetXmlNodeBool(_allowFormatColumnsPath, true);
        set => SetXmlNodeBool(_allowFormatColumnsPath, !value, true);
    }

    /// <summary>
    ///     Allow users to Format rows
    /// </summary>
    public bool AllowFormatRows
    {
        get => !GetXmlNodeBool(_allowFormatRowsPath, true);
        set => SetXmlNodeBool(_allowFormatRowsPath, !value, true);
    }

    /// <summary>
    ///     Allow users to insert columns
    /// </summary>
    public bool AllowInsertColumns
    {
        get => !GetXmlNodeBool(_allowInsertColumnsPath, true);
        set => SetXmlNodeBool(_allowInsertColumnsPath, !value, true);
    }

    /// <summary>
    ///     Allow users to Format rows
    /// </summary>
    public bool AllowInsertRows
    {
        get => !GetXmlNodeBool(_allowInsertRowsPath, true);
        set => SetXmlNodeBool(_allowInsertRowsPath, !value, true);
    }

    /// <summary>
    ///     Allow users to insert hyperlinks
    /// </summary>
    public bool AllowInsertHyperlinks
    {
        get => !GetXmlNodeBool(_allowInsertHyperlinksPath, true);
        set => SetXmlNodeBool(_allowInsertHyperlinksPath, !value, true);
    }

    /// <summary>
    ///     Allow users to delete columns
    /// </summary>
    public bool AllowDeleteColumns
    {
        get => !GetXmlNodeBool(_allowDeleteColumns, true);
        set => SetXmlNodeBool(_allowDeleteColumns, !value, true);
    }

    /// <summary>
    ///     Allow users to delete rows
    /// </summary>
    public bool AllowDeleteRows
    {
        get => !GetXmlNodeBool(_allowDeleteRowsPath, true);
        set => SetXmlNodeBool(_allowDeleteRowsPath, !value, true);
    }

    /// <summary>
    ///     Allow users to sort a range
    /// </summary>
    public bool AllowSort
    {
        get => !GetXmlNodeBool(_allowSortPath, true);
        set => SetXmlNodeBool(_allowSortPath, !value, true);
    }

    /// <summary>
    ///     Allow users to use autofilters
    /// </summary>
    public bool AllowAutoFilter
    {
        get => !GetXmlNodeBool(_allowAutoFilterPath, true);
        set => SetXmlNodeBool(_allowAutoFilterPath, !value, true);
    }

    /// <summary>
    ///     Allow users to use pivottables
    /// </summary>
    public bool AllowPivotTables
    {
        get => !GetXmlNodeBool(_allowPivotTablesPath, true);
        set => SetXmlNodeBool(_allowPivotTablesPath, !value, true);
    }

    /// <summary>
    ///     Sets a password for the sheet.
    /// </summary>
    /// <param name="Password"></param>
    public void SetPassword(string Password)
    {
        if (IsProtected == false) IsProtected = true;

        Password = Password.Trim();
        if (Password == "")
        {
            var node = TopNode.SelectSingleNode(_passwordPath, NameSpaceManager);
            if (node != null) (node as XmlAttribute).OwnerElement.Attributes.Remove(node as XmlAttribute);
            return;
        }

        int hash = EncryptedPackageHandler.CalculatePasswordHash(Password);
        SetXmlNodeString(_passwordPath, hash.ToString("x"));
    }
}