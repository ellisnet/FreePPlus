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
 *******************************************************************************/

using System;
using System.Globalization;

namespace OfficeOpenXml.DataValidation;

/// <summary>
///     Represents a time between 00:00:00 and 23:59:59
/// </summary>
public class ExcelTime
{
    /// <summary>
    ///     Max number of decimals when rounding.
    /// </summary>
    public const int NumberOfDecimals = 15;

    private readonly decimal SecondsPerDay = 3600 * 24;
    private readonly decimal SecondsPerHour = 3600;
    private readonly decimal SecondsPerMinute = 60;

    private int _hour;

    private int _minute;

    private int? _second;

    /// <summary>
    ///     Default constructor
    /// </summary>
    public ExcelTime() { }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="value">An existing time for initialization</param>
    public ExcelTime(decimal value)
    {
        if (value < 0M)
            throw new ArgumentException("Value cannot be less than 0");
        if (value >= 1M) throw new ArgumentException("Value cannot be greater or equal to 1");
        Init(value);
    }

    /// <summary>
    ///     Hour between 0 and 23
    /// </summary>
    public int Hour
    {
        get => _hour;
        set
        {
            if (value < 0) throw new InvalidOperationException("Value for hour cannot be negative");
            if (value > 23) throw new InvalidOperationException("Value for hour cannot be greater than 23");
            _hour = value;
            OnTimeChanged();
        }
    }

    /// <summary>
    ///     Minute between 0 and 59
    /// </summary>
    public int Minute
    {
        get => _minute;
        set
        {
            if (value < 0) throw new InvalidOperationException("Value for minute cannot be negative");
            if (value > 59) throw new InvalidOperationException("Value for minute cannot be greater than 59");
            _minute = value;
            OnTimeChanged();
        }
    }

    /// <summary>
    ///     Second between 0 and 59
    /// </summary>
    public int? Second
    {
        get => _second;
        set
        {
            if (value < 0) throw new InvalidOperationException("Value for second cannot be negative");
            if (value > 59) throw new InvalidOperationException("Value for second cannot be greater than 59");
            _second = value;
            OnTimeChanged();
        }
    }

    private event EventHandler _timeChanged;

    private void Init(decimal value)
    {
        // handle hour
        var totalSeconds = value * SecondsPerDay;
        var hour = Math.Floor(totalSeconds / SecondsPerHour);
        Hour = (int)hour;

        // handle minute
        var remainingSeconds = totalSeconds - hour * SecondsPerHour;
        var minute = Math.Floor(remainingSeconds / SecondsPerMinute);
        Minute = (int)minute;

        // handle second
        remainingSeconds = totalSeconds - hour * SecondsPerHour - minute * SecondsPerMinute;
        var second = Math.Round(remainingSeconds, MidpointRounding.AwayFromZero);
        // Second might be rounded to 60... the SetSecond method handles that.
        SetSecond((int)second);
    }

    /// <summary>
    ///     If we are unlucky second might be rounded up to 60. This will have the minute to be raised and might affect the
    ///     hour.
    /// </summary>
    /// <param name="value"></param>
    private void SetSecond(int value)
    {
        if (value == 60)
        {
            Second = 0;
            var minute = Minute + 1;
            SetMinute(minute);
        }
        else
        {
            Second = value;
        }
    }

    private void SetMinute(int value)
    {
        if (value == 60)
        {
            Minute = 0;
            var hour = Hour + 1;
            SetHour(hour);
        }
        else
        {
            Minute = value;
        }
    }

    private void SetHour(int value)
    {
        if (value == 24) Hour = 0;
    }

    internal event EventHandler TimeChanged
    {
        add => _timeChanged += value;
        remove => _timeChanged -= value;
    }

    private void OnTimeChanged()
    {
        if (_timeChanged != null) _timeChanged(this, EventArgs.Empty);
    }

    private decimal Round(decimal value)
    {
        return Math.Round(value, NumberOfDecimals);
    }

    private decimal ToSeconds()
    {
        var result = Hour * SecondsPerHour;
        result += Minute * SecondsPerMinute;
        result += Second ?? 0;
        return result;
    }

    /// <summary>
    ///     Returns the excel decimal representation of a time.
    /// </summary>
    /// <returns></returns>
    public decimal ToExcelTime()
    {
        var seconds = ToSeconds();
        return Round(seconds / SecondsPerDay);
    }

    /// <summary>
    ///     Returns the excel decimal representation of a time as a string.
    /// </summary>
    /// <returns></returns>
    public string ToExcelString()
    {
        return ToExcelTime().ToString(CultureInfo.InvariantCulture);
    }

    public override string ToString()
    {
        var second = Second ?? 0;
        return string.Format("{0}:{1}:{2}",
            Hour < 10 ? "0" + Hour : Hour.ToString(),
            Minute < 10 ? "0" + Minute : Minute.ToString(),
            second < 10 ? "0" + second : second.ToString());
    }
}