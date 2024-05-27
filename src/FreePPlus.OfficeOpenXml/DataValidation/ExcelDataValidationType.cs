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
 * Mats Alm   		                Added       		        2011-01-01
 * Jan Källman		                License changed GPL-->LGPL  2011-12-27
 * Raziq York 		                Added support for Any type  2014-08-08
 *******************************************************************************/

using System;

namespace OfficeOpenXml.DataValidation;

/// <summary>
///     Enum for available data validation types
/// </summary>
public enum eDataValidationType
{
    /// <summary>
    ///     Any value
    /// </summary>
    Any,

    /// <summary>
    ///     Integer value
    /// </summary>
    Whole,

    /// <summary>
    ///     Decimal values
    /// </summary>
    Decimal,

    /// <summary>
    ///     List of values
    /// </summary>
    List,

    /// <summary>
    ///     Text length validation
    /// </summary>
    TextLength,

    /// <summary>
    ///     DateTime validation
    /// </summary>
    DateTime,

    /// <summary>
    ///     Time validation
    /// </summary>
    Time,

    /// <summary>
    ///     Custom validation
    /// </summary>
    Custom
}

internal static class DataValidationSchemaNames
{
    public const string Any = "";
    public const string Whole = "whole";
    public const string Decimal = "decimal";
    public const string List = "list";
    public const string TextLength = "textLength";
    public const string Date = "date";
    public const string Time = "time";
    public const string Custom = "custom";
}

/// <summary>
///     Types of datavalidation
/// </summary>
public class ExcelDataValidationType
{
    /// <summary>
    ///     Integer values
    /// </summary>
    private static ExcelDataValidationType _any;

    /// <summary>
    ///     Integer values
    /// </summary>
    private static ExcelDataValidationType _whole;

    /// <summary>
    ///     List of allowed values
    /// </summary>
    private static ExcelDataValidationType _list;

    private static ExcelDataValidationType _decimal;

    private static ExcelDataValidationType _textLength;

    private static ExcelDataValidationType _dateTime;

    private static ExcelDataValidationType _time;

    private static ExcelDataValidationType _custom;

    private ExcelDataValidationType(eDataValidationType validationType, bool allowOperator, string schemaName)
    {
        Type = validationType;
        AllowOperator = allowOperator;
        SchemaName = schemaName;
    }

    /// <summary>
    ///     Validation type
    /// </summary>
    public eDataValidationType Type { get; }

    internal string SchemaName { get; private set; }

    /// <summary>
    ///     This type allows operator to be set
    /// </summary>
    internal bool AllowOperator { get; private set; }

    public static ExcelDataValidationType Any
    {
        get
        {
            if (_any == null)
                _any = new ExcelDataValidationType(eDataValidationType.Any, false, DataValidationSchemaNames.Any);
            return _any;
        }
    }

    public static ExcelDataValidationType Whole
    {
        get
        {
            if (_whole == null)
                _whole = new ExcelDataValidationType(eDataValidationType.Whole, true, DataValidationSchemaNames.Whole);
            return _whole;
        }
    }

    public static ExcelDataValidationType List
    {
        get
        {
            if (_list == null)
                _list = new ExcelDataValidationType(eDataValidationType.List, false, DataValidationSchemaNames.List);
            return _list;
        }
    }

    public static ExcelDataValidationType Decimal
    {
        get
        {
            if (_decimal == null)
                _decimal = new ExcelDataValidationType(eDataValidationType.Decimal, true,
                    DataValidationSchemaNames.Decimal);
            return _decimal;
        }
    }

    public static ExcelDataValidationType TextLength
    {
        get
        {
            if (_textLength == null)
                _textLength = new ExcelDataValidationType(eDataValidationType.TextLength, true,
                    DataValidationSchemaNames.TextLength);
            return _textLength;
        }
    }

    public static ExcelDataValidationType DateTime
    {
        get
        {
            if (_dateTime == null)
                _dateTime = new ExcelDataValidationType(eDataValidationType.DateTime, true,
                    DataValidationSchemaNames.Date);
            return _dateTime;
        }
    }

    public static ExcelDataValidationType Time
    {
        get
        {
            if (_time == null)
                _time = new ExcelDataValidationType(eDataValidationType.Time, true, DataValidationSchemaNames.Time);
            return _time;
        }
    }

    public static ExcelDataValidationType Custom
    {
        get
        {
            if (_custom == null)
                _custom = new ExcelDataValidationType(eDataValidationType.Custom, true,
                    DataValidationSchemaNames.Custom);
            return _custom;
        }
    }

    /// <summary>
    ///     Returns a validation type by <see cref="eDataValidationType" />
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    internal static ExcelDataValidationType GetByValidationType(eDataValidationType type)
    {
        switch (type)
        {
            case eDataValidationType.Any:
                return Any;
            case eDataValidationType.Whole:
                return Whole;
            case eDataValidationType.List:
                return List;
            case eDataValidationType.Decimal:
                return Decimal;
            case eDataValidationType.TextLength:
                return TextLength;
            case eDataValidationType.DateTime:
                return DateTime;
            case eDataValidationType.Time:
                return Time;
            case eDataValidationType.Custom:
                return Custom;
            default:
                throw new InvalidOperationException("Non supported Validationtype : " + type);
        }
    }

    internal static ExcelDataValidationType GetBySchemaName(string schemaName)
    {
        switch (schemaName)
        {
            case DataValidationSchemaNames.Any:
                return Any;
            case DataValidationSchemaNames.Whole:
                return Whole;
            case DataValidationSchemaNames.Decimal:
                return Decimal;
            case DataValidationSchemaNames.List:
                return List;
            case DataValidationSchemaNames.TextLength:
                return TextLength;
            case DataValidationSchemaNames.Date:
                return DateTime;
            case DataValidationSchemaNames.Time:
                return Time;
            case DataValidationSchemaNames.Custom:
                return Custom;
            default:
                throw new ArgumentException("Invalid schemaname: " + schemaName);
        }
    }

    /// <summary>
    ///     Overridden Equals, compares on internal validation type
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (!(obj is ExcelDataValidationType)) return false;
        return ((ExcelDataValidationType)obj).Type == Type;
    }

    /// <summary>
    ///     Overrides GetHashCode()
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}