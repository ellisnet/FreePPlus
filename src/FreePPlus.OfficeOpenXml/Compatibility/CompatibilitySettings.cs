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
 * Jan Källman		    Added       		        2017-11-02
 *******************************************************************************/

namespace OfficeOpenXml.Compatibility;

/// <summary>
///     Settings to stay compatible with older versions of EPPlus
/// </summary>
public class CompatibilitySettings
{
    private readonly ExcelPackage _excelPackage;

    internal CompatibilitySettings(ExcelPackage excelPackage)
    {
        _excelPackage = excelPackage;
    }

    /// <summary>
    ///     If the worksheets collection of the ExcelWorkbook class is 1 based.
    ///     This property can be set from appsettings.json file.
    ///     <code>
    ///     {
    ///       "FreePPlus": {
    ///         "ExcelPackage": {
    ///           "Compatibility": {
    ///             "IsWorksheets1Based": false //Default value is false
    ///           }
    ///         }
    ///       }
    ///     }
    /// </code>
    /// </summary>

    public bool IsWorksheets1Based
    {
        get => _excelPackage.WorksheetAddId == 1;
        set
        {
            _excelPackage.WorksheetAddId = value ? 1 : 0;
            if (_excelPackage._workbook is { _worksheets: not null })
                _excelPackage.Workbook.Worksheets.ReindexWorksheetDictionary();
        }
    }
}