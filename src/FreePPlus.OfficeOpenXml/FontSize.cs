﻿using System;
using System.Collections.Generic;

namespace OfficeOpenXml;

public class FontSizeInfo
{
    public FontSizeInfo(float height, float width)
    {
        Width = width;
        Height = height;
    }

    public float Height { get; set; }
    public float Width { get; set; }
}

public static class FontSize
{
    public static readonly Dictionary<string, Dictionary<float, FontSizeInfo>> FontHeights =
        new(StringComparer.OrdinalIgnoreCase)
        {
            {
                "Times New Roman",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(20, 7) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 9) },
                    { 16, new FontSizeInfo(27, 11) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(35, 14) }, { 22, new FontSizeInfo(37, 14) },
                    { 24, new FontSizeInfo(41, 16) }, { 26, new FontSizeInfo(44, 18) },
                    { 28, new FontSizeInfo(47, 19) }, { 36, new FontSizeInfo(61, 24) },
                    { 48, new FontSizeInfo(82, 32) }, { 72, new FontSizeInfo(122, 48) },
                    { 96, new FontSizeInfo(164, 64) }, { 128, new FontSizeInfo(216, 86) },
                    { 256, new FontSizeInfo(428, 171) }
                }
            },
            {
                "Arial",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(36, 16) },
                    { 24, new FontSizeInfo(40, 18) }, { 26, new FontSizeInfo(44, 19) },
                    { 28, new FontSizeInfo(46, 21) }, { 36, new FontSizeInfo(59, 27) },
                    { 48, new FontSizeInfo(79, 36) }, { 72, new FontSizeInfo(120, 53) },
                    { 96, new FontSizeInfo(159, 71) }, { 128, new FontSizeInfo(213, 95) },
                    { 256, new FontSizeInfo(424, 190) }
                }
            },
            {
                "Courier New",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 10) }, { 14, new FontSizeInfo(25, 11) },
                    { 16, new FontSizeInfo(28, 13) }, { 18, new FontSizeInfo(32, 14) },
                    { 20, new FontSizeInfo(35, 16) }, { 22, new FontSizeInfo(38, 17) },
                    { 24, new FontSizeInfo(42, 19) }, { 26, new FontSizeInfo(46, 21) },
                    { 28, new FontSizeInfo(48, 22) }, { 36, new FontSizeInfo(62, 29) },
                    { 48, new FontSizeInfo(83, 38) }, { 72, new FontSizeInfo(123, 58) },
                    { 96, new FontSizeInfo(164, 77) }, { 128, new FontSizeInfo(219, 103) },
                    { 256, new FontSizeInfo(444, 205) }
                }
            },
            {
                "Symbol",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 4) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(24, 10) },
                    { 16, new FontSizeInfo(29, 11) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(35, 14) }, { 22, new FontSizeInfo(38, 15) },
                    { 24, new FontSizeInfo(40, 16) }, { 26, new FontSizeInfo(44, 18) },
                    { 28, new FontSizeInfo(48, 19) }, { 36, new FontSizeInfo(60, 24) },
                    { 48, new FontSizeInfo(79, 32) }, { 72, new FontSizeInfo(120, 48) },
                    { 96, new FontSizeInfo(159, 64) }, { 128, new FontSizeInfo(212, 86) },
                    { 256, new FontSizeInfo(421, 171) }
                }
            },
            {
                "Wingdings",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 11) }, { 8, new FontSizeInfo(20, 15) }, { 10, new FontSizeInfo(20, 17) },
                    { 11, new FontSizeInfo(20, 20) }, { 12, new FontSizeInfo(21, 22) },
                    { 14, new FontSizeInfo(24, 26) }, { 16, new FontSizeInfo(26, 28) },
                    { 18, new FontSizeInfo(30, 32) }, { 20, new FontSizeInfo(34, 36) },
                    { 22, new FontSizeInfo(36, 39) }, { 24, new FontSizeInfo(40, 43) },
                    { 26, new FontSizeInfo(43, 47) }, { 28, new FontSizeInfo(46, 50) },
                    { 36, new FontSizeInfo(59, 64) }, { 48, new FontSizeInfo(79, 86) },
                    { 72, new FontSizeInfo(117, 129) }, { 96, new FontSizeInfo(156, 172) },
                    { 128, new FontSizeInfo(208, 230) }, { 256, new FontSizeInfo(414, 457) }
                }
            },
            {
                "SimSun",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(27, 11) }, { 18, new FontSizeInfo(30, 12) },
                    { 20, new FontSizeInfo(34, 14) }, { 22, new FontSizeInfo(36, 15) },
                    { 24, new FontSizeInfo(42, 16) }, { 26, new FontSizeInfo(45, 18) },
                    { 28, new FontSizeInfo(47, 19) }, { 36, new FontSizeInfo(62, 24) },
                    { 48, new FontSizeInfo(82, 32) }, { 72, new FontSizeInfo(123, 48) },
                    { 96, new FontSizeInfo(163, 64) }, { 128, new FontSizeInfo(218, 86) },
                    { 256, new FontSizeInfo(436, 171) }
                }
            },
            {
                "Century",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(30, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(36, 16) },
                    { 24, new FontSizeInfo(40, 18) }, { 26, new FontSizeInfo(44, 19) },
                    { 28, new FontSizeInfo(46, 21) }, { 36, new FontSizeInfo(59, 27) },
                    { 48, new FontSizeInfo(79, 36) }, { 72, new FontSizeInfo(118, 53) },
                    { 96, new FontSizeInfo(157, 71) }, { 128, new FontSizeInfo(209, 95) },
                    { 256, new FontSizeInfo(416, 190) }
                }
            },
            {
                "Sylfaen",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(21, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(24, 8) }, { 14, new FontSizeInfo(26, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(32, 12) },
                    { 20, new FontSizeInfo(36, 14) }, { 22, new FontSizeInfo(41, 15) },
                    { 24, new FontSizeInfo(44, 16) }, { 26, new FontSizeInfo(48, 18) },
                    { 28, new FontSizeInfo(49, 19) }, { 36, new FontSizeInfo(63, 24) },
                    { 48, new FontSizeInfo(86, 32) }, { 72, new FontSizeInfo(129, 48) },
                    { 96, new FontSizeInfo(167, 64) }, { 128, new FontSizeInfo(224, 86) },
                    { 256, new FontSizeInfo(452, 171) }
                }
            },
            {
                "Cambria Math",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(85, 6) }, { 10, new FontSizeInfo(102, 7) },
                    { 11, new FontSizeInfo(117, 8) }, { 12, new FontSizeInfo(124, 9) },
                    { 14, new FontSizeInfo(147, 11) }, { 16, new FontSizeInfo(162, 12) },
                    { 18, new FontSizeInfo(186, 13) }, { 20, new FontSizeInfo(209, 15) },
                    { 22, new FontSizeInfo(223, 16) }, { 24, new FontSizeInfo(248, 18) },
                    { 26, new FontSizeInfo(270, 19) }, { 28, new FontSizeInfo(285, 20) },
                    { 36, new FontSizeInfo(371, 27) }, { 48, new FontSizeInfo(493, 35) },
                    { 72, new FontSizeInfo(739, 53) }, { 96, new FontSizeInfo(986, 71) },
                    { 128, new FontSizeInfo(1317, 95) }, { 256, new FontSizeInfo(2047, 189) }
                }
            },
            {
                "Yu Gothic",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(22, 7) },
                    { 11, new FontSizeInfo(25, 8) }, { 12, new FontSizeInfo(26, 9) }, { 14, new FontSizeInfo(32, 11) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(40, 13) },
                    { 20, new FontSizeInfo(44, 15) }, { 22, new FontSizeInfo(47, 16) },
                    { 24, new FontSizeInfo(53, 18) }, { 26, new FontSizeInfo(57, 19) },
                    { 28, new FontSizeInfo(59, 21) }, { 36, new FontSizeInfo(78, 27) },
                    { 48, new FontSizeInfo(102, 36) }, { 72, new FontSizeInfo(154, 53) },
                    { 96, new FontSizeInfo(206, 71) }, { 128, new FontSizeInfo(276, 95) },
                    { 256, new FontSizeInfo(551, 190) }
                }
            },
            {
                "DengXian",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(31, 14) },
                    { 20, new FontSizeInfo(35, 16) }, { 22, new FontSizeInfo(37, 17) },
                    { 24, new FontSizeInfo(41, 19) }, { 26, new FontSizeInfo(45, 20) },
                    { 28, new FontSizeInfo(47, 21) }, { 36, new FontSizeInfo(61, 28) },
                    { 48, new FontSizeInfo(81, 37) }, { 72, new FontSizeInfo(121, 56) },
                    { 96, new FontSizeInfo(161, 74) }, { 128, new FontSizeInfo(215, 99) },
                    { 256, new FontSizeInfo(428, 197) }
                }
            },
            {
                "Arial Unicode MS",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(21, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 11) },
                    { 16, new FontSizeInfo(30, 12) }, { 18, new FontSizeInfo(36, 13) },
                    { 20, new FontSizeInfo(39, 15) }, { 22, new FontSizeInfo(42, 16) },
                    { 24, new FontSizeInfo(46, 18) }, { 26, new FontSizeInfo(49, 19) },
                    { 28, new FontSizeInfo(54, 21) }, { 36, new FontSizeInfo(68, 27) },
                    { 48, new FontSizeInfo(90, 36) }, { 72, new FontSizeInfo(137, 53) },
                    { 96, new FontSizeInfo(182, 71) }, { 128, new FontSizeInfo(242, 95) },
                    { 256, new FontSizeInfo(480, 190) }
                }
            },
            {
                "Calibri Light",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(35, 14) }, { 22, new FontSizeInfo(38, 15) },
                    { 24, new FontSizeInfo(42, 16) }, { 26, new FontSizeInfo(45, 18) },
                    { 28, new FontSizeInfo(48, 19) }, { 36, new FontSizeInfo(62, 24) },
                    { 48, new FontSizeInfo(82, 32) }, { 72, new FontSizeInfo(123, 49) },
                    { 96, new FontSizeInfo(163, 65) }, { 128, new FontSizeInfo(218, 87) },
                    { 256, new FontSizeInfo(434, 173) }
                }
            },
            {
                "Calibri",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 7) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(35, 14) }, { 22, new FontSizeInfo(38, 15) },
                    { 24, new FontSizeInfo(42, 16) }, { 26, new FontSizeInfo(45, 18) },
                    { 28, new FontSizeInfo(48, 19) }, { 36, new FontSizeInfo(62, 24) },
                    { 48, new FontSizeInfo(82, 32) }, { 72, new FontSizeInfo(123, 49) },
                    { 96, new FontSizeInfo(163, 65) }, { 128, new FontSizeInfo(218, 87) },
                    { 256, new FontSizeInfo(434, 173) }
                }
            },
            {
                "Georgia",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 8) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 10) }, { 14, new FontSizeInfo(24, 12) },
                    { 16, new FontSizeInfo(27, 13) }, { 18, new FontSizeInfo(31, 15) },
                    { 20, new FontSizeInfo(34, 17) }, { 22, new FontSizeInfo(36, 18) },
                    { 24, new FontSizeInfo(40, 20) }, { 26, new FontSizeInfo(44, 21) },
                    { 28, new FontSizeInfo(46, 23) }, { 36, new FontSizeInfo(60, 29) },
                    { 48, new FontSizeInfo(79, 39) }, { 72, new FontSizeInfo(118, 59) },
                    { 96, new FontSizeInfo(157, 79) }, { 128, new FontSizeInfo(209, 105) },
                    { 256, new FontSizeInfo(417, 209) }
                }
            },
            {
                "Cambria",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(30, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(36, 16) },
                    { 24, new FontSizeInfo(40, 18) }, { 26, new FontSizeInfo(44, 19) },
                    { 28, new FontSizeInfo(46, 20) }, { 36, new FontSizeInfo(60, 27) },
                    { 48, new FontSizeInfo(79, 35) }, { 72, new FontSizeInfo(118, 53) },
                    { 96, new FontSizeInfo(157, 71) }, { 128, new FontSizeInfo(210, 95) },
                    { 256, new FontSizeInfo(418, 189) }
                }
            },
            {
                "Marlett",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 8) }, { 8, new FontSizeInfo(20, 11) }, { 10, new FontSizeInfo(20, 13) },
                    { 11, new FontSizeInfo(22, 15) }, { 12, new FontSizeInfo(23, 16) },
                    { 14, new FontSizeInfo(26, 19) }, { 16, new FontSizeInfo(28, 21) },
                    { 18, new FontSizeInfo(31, 24) }, { 20, new FontSizeInfo(34, 27) },
                    { 22, new FontSizeInfo(36, 29) }, { 24, new FontSizeInfo(39, 32) },
                    { 26, new FontSizeInfo(42, 35) }, { 28, new FontSizeInfo(44, 37) },
                    { 36, new FontSizeInfo(55, 48) }, { 48, new FontSizeInfo(71, 64) },
                    { 72, new FontSizeInfo(103, 96) }, { 96, new FontSizeInfo(135, 128) },
                    { 128, new FontSizeInfo(178, 171) }, { 256, new FontSizeInfo(348, 341) }
                }
            },
            {
                "Arial Black",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(21, 9) },
                    { 11, new FontSizeInfo(25, 10) }, { 12, new FontSizeInfo(26, 11) },
                    { 14, new FontSizeInfo(30, 13) }, { 16, new FontSizeInfo(33, 14) },
                    { 18, new FontSizeInfo(36, 16) }, { 20, new FontSizeInfo(42, 18) },
                    { 22, new FontSizeInfo(45, 19) }, { 24, new FontSizeInfo(49, 21) },
                    { 26, new FontSizeInfo(55, 23) }, { 28, new FontSizeInfo(57, 25) },
                    { 36, new FontSizeInfo(74, 32) }, { 48, new FontSizeInfo(97, 43) },
                    { 72, new FontSizeInfo(147, 64) }, { 96, new FontSizeInfo(195, 85) },
                    { 128, new FontSizeInfo(259, 114) }, { 256, new FontSizeInfo(516, 227) }
                }
            },
            {
                "Candara",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(28, 12) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(35, 15) }, { 22, new FontSizeInfo(38, 16) },
                    { 24, new FontSizeInfo(42, 18) }, { 26, new FontSizeInfo(45, 19) },
                    { 28, new FontSizeInfo(48, 20) }, { 36, new FontSizeInfo(62, 26) },
                    { 48, new FontSizeInfo(82, 35) }, { 72, new FontSizeInfo(123, 53) },
                    { 96, new FontSizeInfo(163, 71) }, { 128, new FontSizeInfo(218, 94) },
                    { 256, new FontSizeInfo(434, 188) }
                }
            },
            {
                "Comic Sans MS",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(21, 8) },
                    { 11, new FontSizeInfo(22, 9) }, { 12, new FontSizeInfo(26, 10) }, { 14, new FontSizeInfo(28, 12) },
                    { 16, new FontSizeInfo(32, 13) }, { 18, new FontSizeInfo(36, 15) },
                    { 20, new FontSizeInfo(42, 16) }, { 22, new FontSizeInfo(44, 18) },
                    { 24, new FontSizeInfo(50, 20) }, { 26, new FontSizeInfo(54, 21) },
                    { 28, new FontSizeInfo(57, 23) }, { 36, new FontSizeInfo(73, 29) },
                    { 48, new FontSizeInfo(98, 39) }, { 72, new FontSizeInfo(147, 59) },
                    { 96, new FontSizeInfo(190, 78) }, { 128, new FontSizeInfo(258, 104) },
                    { 256, new FontSizeInfo(511, 208) }
                }
            },
            {
                "Consolas",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(35, 15) }, { 22, new FontSizeInfo(37, 16) },
                    { 24, new FontSizeInfo(41, 18) }, { 26, new FontSizeInfo(45, 19) },
                    { 28, new FontSizeInfo(47, 20) }, { 36, new FontSizeInfo(61, 26) },
                    { 48, new FontSizeInfo(81, 35) }, { 72, new FontSizeInfo(121, 53) },
                    { 96, new FontSizeInfo(161, 70) }, { 128, new FontSizeInfo(215, 94) },
                    { 256, new FontSizeInfo(428, 187) }
                }
            },
            {
                "Constantia",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(35, 15) }, { 22, new FontSizeInfo(38, 16) },
                    { 24, new FontSizeInfo(42, 17) }, { 26, new FontSizeInfo(45, 19) },
                    { 28, new FontSizeInfo(48, 20) }, { 36, new FontSizeInfo(62, 26) },
                    { 48, new FontSizeInfo(82, 35) }, { 72, new FontSizeInfo(123, 52) },
                    { 96, new FontSizeInfo(163, 70) }, { 128, new FontSizeInfo(218, 93) },
                    { 256, new FontSizeInfo(434, 186) }
                }
            },
            {
                "Corbel",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(35, 14) }, { 22, new FontSizeInfo(38, 15) },
                    { 24, new FontSizeInfo(42, 17) }, { 26, new FontSizeInfo(45, 18) },
                    { 28, new FontSizeInfo(48, 19) }, { 36, new FontSizeInfo(62, 25) },
                    { 48, new FontSizeInfo(82, 34) }, { 72, new FontSizeInfo(123, 50) },
                    { 96, new FontSizeInfo(163, 67) }, { 128, new FontSizeInfo(218, 90) },
                    { 256, new FontSizeInfo(434, 179) }
                }
            },
            {
                "Ebrima",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 17) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(94, 35) }, { 72, new FontSizeInfo(138, 52) },
                    { 96, new FontSizeInfo(184, 69) }, { 128, new FontSizeInfo(245, 92) },
                    { 256, new FontSizeInfo(490, 184) }
                }
            },
            {
                "Franklin Gothic Medium",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(21, 9) }, { 12, new FontSizeInfo(22, 9) }, { 14, new FontSizeInfo(26, 11) },
                    { 16, new FontSizeInfo(28, 12) }, { 18, new FontSizeInfo(32, 14) },
                    { 20, new FontSizeInfo(36, 16) }, { 22, new FontSizeInfo(39, 17) },
                    { 24, new FontSizeInfo(40, 19) }, { 26, new FontSizeInfo(44, 21) },
                    { 28, new FontSizeInfo(46, 22) }, { 36, new FontSizeInfo(64, 28) },
                    { 48, new FontSizeInfo(85, 38) }, { 72, new FontSizeInfo(126, 56) },
                    { 96, new FontSizeInfo(168, 75) }, { 128, new FontSizeInfo(224, 100) },
                    { 256, new FontSizeInfo(416, 200) }
                }
            },
            {
                "Gabriola",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(25, 4) }, { 10, new FontSizeInfo(26, 5) },
                    { 11, new FontSizeInfo(32, 6) }, { 12, new FontSizeInfo(33, 6) }, { 14, new FontSizeInfo(40, 8) },
                    { 16, new FontSizeInfo(44, 8) }, { 18, new FontSizeInfo(51, 10) }, { 20, new FontSizeInfo(56, 11) },
                    { 22, new FontSizeInfo(61, 12) }, { 24, new FontSizeInfo(66, 13) },
                    { 26, new FontSizeInfo(73, 14) }, { 28, new FontSizeInfo(76, 15) },
                    { 36, new FontSizeInfo(98, 19) }, { 48, new FontSizeInfo(131, 26) },
                    { 72, new FontSizeInfo(195, 38) }, { 96, new FontSizeInfo(262, 51) },
                    { 128, new FontSizeInfo(349, 69) }, { 256, new FontSizeInfo(693, 137) }
                }
            },
            {
                "Gadugi",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(24, 10) },
                    { 16, new FontSizeInfo(27, 11) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(37, 16) },
                    { 24, new FontSizeInfo(40, 17) }, { 26, new FontSizeInfo(44, 19) },
                    { 28, new FontSizeInfo(47, 20) }, { 36, new FontSizeInfo(60, 26) },
                    { 48, new FontSizeInfo(81, 35) }, { 72, new FontSizeInfo(120, 52) },
                    { 96, new FontSizeInfo(159, 69) }, { 128, new FontSizeInfo(213, 92) },
                    { 256, new FontSizeInfo(482, 184) }
                }
            },
            {
                "Impact",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(21, 8) }, { 12, new FontSizeInfo(22, 9) }, { 14, new FontSizeInfo(24, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(30, 13) },
                    { 20, new FontSizeInfo(36, 15) }, { 22, new FontSizeInfo(38, 16) },
                    { 24, new FontSizeInfo(40, 17) }, { 26, new FontSizeInfo(45, 19) },
                    { 28, new FontSizeInfo(46, 20) }, { 36, new FontSizeInfo(63, 26) },
                    { 48, new FontSizeInfo(83, 35) }, { 72, new FontSizeInfo(119, 52) },
                    { 96, new FontSizeInfo(158, 69) }, { 128, new FontSizeInfo(212, 93) },
                    { 256, new FontSizeInfo(420, 185) }
                }
            },
            {
                "Javanese Text",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(30, 6) }, { 10, new FontSizeInfo(33, 8) },
                    { 11, new FontSizeInfo(39, 9) }, { 12, new FontSizeInfo(41, 9) }, { 14, new FontSizeInfo(49, 11) },
                    { 16, new FontSizeInfo(53, 12) }, { 18, new FontSizeInfo(61, 14) },
                    { 20, new FontSizeInfo(70, 16) }, { 22, new FontSizeInfo(74, 17) },
                    { 24, new FontSizeInfo(82, 19) }, { 26, new FontSizeInfo(90, 21) },
                    { 28, new FontSizeInfo(94, 22) }, { 36, new FontSizeInfo(122, 28) },
                    { 48, new FontSizeInfo(162, 38) }, { 72, new FontSizeInfo(243, 57) },
                    { 96, new FontSizeInfo(324, 76) }, { 128, new FontSizeInfo(433, 101) },
                    { 256, new FontSizeInfo(860, 200) }
                }
            },
            {
                "Leelawadee UI",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 17) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(92, 35) }, { 72, new FontSizeInfo(136, 52) },
                    { 96, new FontSizeInfo(180, 69) }, { 128, new FontSizeInfo(241, 92) },
                    { 256, new FontSizeInfo(482, 184) }
                }
            },
            {
                "Leelawadee UI Semilight",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 18) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(92, 35) }, { 72, new FontSizeInfo(136, 53) },
                    { 96, new FontSizeInfo(180, 70) }, { 128, new FontSizeInfo(241, 94) },
                    { 256, new FontSizeInfo(482, 187) }
                }
            },
            {
                "Lucida Console",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 10) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(26, 13) }, { 18, new FontSizeInfo(30, 14) },
                    { 20, new FontSizeInfo(34, 16) }, { 22, new FontSizeInfo(36, 17) },
                    { 24, new FontSizeInfo(40, 19) }, { 26, new FontSizeInfo(43, 21) },
                    { 28, new FontSizeInfo(46, 22) }, { 36, new FontSizeInfo(59, 29) },
                    { 48, new FontSizeInfo(79, 39) }, { 72, new FontSizeInfo(117, 58) },
                    { 96, new FontSizeInfo(156, 77) }, { 128, new FontSizeInfo(208, 103) },
                    { 256, new FontSizeInfo(414, 205) }
                }
            },
            {
                "Lucida Sans Unicode",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(22, 10) }, { 14, new FontSizeInfo(24, 12) },
                    { 16, new FontSizeInfo(26, 13) }, { 18, new FontSizeInfo(30, 15) },
                    { 20, new FontSizeInfo(36, 17) }, { 22, new FontSizeInfo(36, 18) },
                    { 24, new FontSizeInfo(40, 20) }, { 26, new FontSizeInfo(43, 22) },
                    { 28, new FontSizeInfo(46, 23) }, { 36, new FontSizeInfo(61, 30) },
                    { 48, new FontSizeInfo(79, 40) }, { 72, new FontSizeInfo(117, 61) },
                    { 96, new FontSizeInfo(158, 81) }, { 128, new FontSizeInfo(210, 108) },
                    { 256, new FontSizeInfo(558, 216) }
                }
            },
            {
                "Malgun Gothic",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(35, 12) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(42, 15) }, { 22, new FontSizeInfo(45, 16) },
                    { 24, new FontSizeInfo(51, 18) }, { 26, new FontSizeInfo(52, 19) },
                    { 28, new FontSizeInfo(55, 20) }, { 36, new FontSizeInfo(72, 26) },
                    { 48, new FontSizeInfo(93, 35) }, { 72, new FontSizeInfo(137, 53) },
                    { 96, new FontSizeInfo(181, 71) }, { 128, new FontSizeInfo(242, 94) },
                    { 256, new FontSizeInfo(484, 188) }
                }
            },
            {
                "Malgun Gothic Semilight",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 11) },
                    { 16, new FontSizeInfo(35, 12) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(42, 15) }, { 22, new FontSizeInfo(45, 16) },
                    { 24, new FontSizeInfo(51, 18) }, { 26, new FontSizeInfo(52, 20) },
                    { 28, new FontSizeInfo(55, 21) }, { 36, new FontSizeInfo(72, 27) },
                    { 48, new FontSizeInfo(93, 36) }, { 72, new FontSizeInfo(137, 54) },
                    { 96, new FontSizeInfo(181, 72) }, { 128, new FontSizeInfo(242, 96) },
                    { 256, new FontSizeInfo(484, 190) }
                }
            },
            {
                "Microsoft Himalaya",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(21, 4) },
                    { 11, new FontSizeInfo(22, 5) }, { 12, new FontSizeInfo(24, 5) }, { 14, new FontSizeInfo(28, 6) },
                    { 16, new FontSizeInfo(31, 7) }, { 18, new FontSizeInfo(35, 8) }, { 20, new FontSizeInfo(39, 9) },
                    { 22, new FontSizeInfo(42, 10) }, { 24, new FontSizeInfo(46, 11) },
                    { 26, new FontSizeInfo(50, 12) }, { 28, new FontSizeInfo(53, 12) },
                    { 36, new FontSizeInfo(69, 16) }, { 48, new FontSizeInfo(91, 21) },
                    { 72, new FontSizeInfo(136, 32) }, { 96, new FontSizeInfo(181, 43) },
                    { 128, new FontSizeInfo(242, 57) }, { 256, new FontSizeInfo(481, 114) }
                }
            },
            {
                "Microsoft JhengHei",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 11) },
                    { 16, new FontSizeInfo(28, 12) }, { 18, new FontSizeInfo(31, 14) },
                    { 20, new FontSizeInfo(35, 16) }, { 22, new FontSizeInfo(37, 17) },
                    { 24, new FontSizeInfo(41, 19) }, { 26, new FontSizeInfo(45, 20) },
                    { 28, new FontSizeInfo(48, 21) }, { 36, new FontSizeInfo(62, 28) },
                    { 48, new FontSizeInfo(82, 37) }, { 72, new FontSizeInfo(122, 56) },
                    { 96, new FontSizeInfo(162, 74) }, { 128, new FontSizeInfo(217, 99) },
                    { 256, new FontSizeInfo(481, 198) }
                }
            },
            {
                "Microsoft JhengHei UI",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(31, 14) },
                    { 20, new FontSizeInfo(37, 16) }, { 22, new FontSizeInfo(38, 17) },
                    { 24, new FontSizeInfo(42, 19) }, { 26, new FontSizeInfo(45, 20) },
                    { 28, new FontSizeInfo(48, 21) }, { 36, new FontSizeInfo(62, 28) },
                    { 48, new FontSizeInfo(82, 37) }, { 72, new FontSizeInfo(123, 56) },
                    { 96, new FontSizeInfo(164, 74) }, { 128, new FontSizeInfo(218, 99) },
                    { 256, new FontSizeInfo(439, 198) }
                }
            },
            {
                "Microsoft JhengHei Light",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 11) },
                    { 16, new FontSizeInfo(28, 12) }, { 18, new FontSizeInfo(31, 14) },
                    { 20, new FontSizeInfo(35, 16) }, { 22, new FontSizeInfo(37, 17) },
                    { 24, new FontSizeInfo(41, 18) }, { 26, new FontSizeInfo(45, 20) },
                    { 28, new FontSizeInfo(48, 21) }, { 36, new FontSizeInfo(62, 28) },
                    { 48, new FontSizeInfo(82, 37) }, { 72, new FontSizeInfo(122, 55) },
                    { 96, new FontSizeInfo(162, 74) }, { 128, new FontSizeInfo(217, 98) },
                    { 256, new FontSizeInfo(481, 196) }
                }
            },
            {
                "Microsoft JhengHei UI Light",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 11) },
                    { 16, new FontSizeInfo(28, 12) }, { 18, new FontSizeInfo(31, 14) },
                    { 20, new FontSizeInfo(35, 16) }, { 22, new FontSizeInfo(37, 17) },
                    { 24, new FontSizeInfo(41, 18) }, { 26, new FontSizeInfo(45, 20) },
                    { 28, new FontSizeInfo(48, 21) }, { 36, new FontSizeInfo(62, 28) },
                    { 48, new FontSizeInfo(82, 37) }, { 72, new FontSizeInfo(122, 55) },
                    { 96, new FontSizeInfo(162, 74) }, { 128, new FontSizeInfo(217, 98) },
                    { 256, new FontSizeInfo(439, 196) }
                }
            },
            {
                "Microsoft New Tai Lue",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(21, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(30, 11) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(38, 15) }, { 22, new FontSizeInfo(41, 16) },
                    { 24, new FontSizeInfo(45, 17) }, { 26, new FontSizeInfo(49, 19) },
                    { 28, new FontSizeInfo(52, 20) }, { 36, new FontSizeInfo(67, 26) },
                    { 48, new FontSizeInfo(89, 35) }, { 72, new FontSizeInfo(134, 52) },
                    { 96, new FontSizeInfo(178, 69) }, { 128, new FontSizeInfo(237, 92) },
                    { 256, new FontSizeInfo(472, 184) }
                }
            },
            {
                "Microsoft PhagsPa",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(26, 10) },
                    { 16, new FontSizeInfo(29, 11) }, { 18, new FontSizeInfo(33, 13) },
                    { 20, new FontSizeInfo(36, 15) }, { 22, new FontSizeInfo(39, 16) },
                    { 24, new FontSizeInfo(43, 17) }, { 26, new FontSizeInfo(48, 19) },
                    { 28, new FontSizeInfo(51, 20) }, { 36, new FontSizeInfo(64, 26) },
                    { 48, new FontSizeInfo(86, 35) }, { 72, new FontSizeInfo(128, 52) },
                    { 96, new FontSizeInfo(171, 69) }, { 128, new FontSizeInfo(228, 92) },
                    { 256, new FontSizeInfo(452, 184) }
                }
            },
            {
                "Microsoft Sans Serif",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(33, 15) }, { 22, new FontSizeInfo(36, 16) },
                    { 24, new FontSizeInfo(41, 18) }, { 26, new FontSizeInfo(43, 19) },
                    { 28, new FontSizeInfo(45, 21) }, { 36, new FontSizeInfo(60, 27) },
                    { 48, new FontSizeInfo(79, 36) }, { 72, new FontSizeInfo(117, 53) },
                    { 96, new FontSizeInfo(157, 71) }, { 128, new FontSizeInfo(208, 95) },
                    { 256, new FontSizeInfo(414, 190) }
                }
            },
            {
                "Microsoft Tai Le",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(21, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(26, 10) },
                    { 16, new FontSizeInfo(29, 11) }, { 18, new FontSizeInfo(33, 13) },
                    { 20, new FontSizeInfo(37, 15) }, { 22, new FontSizeInfo(40, 16) },
                    { 24, new FontSizeInfo(44, 17) }, { 26, new FontSizeInfo(48, 19) },
                    { 28, new FontSizeInfo(51, 20) }, { 36, new FontSizeInfo(66, 26) },
                    { 48, new FontSizeInfo(87, 35) }, { 72, new FontSizeInfo(130, 52) },
                    { 96, new FontSizeInfo(173, 69) }, { 128, new FontSizeInfo(231, 92) },
                    { 256, new FontSizeInfo(459, 184) }
                }
            },
            {
                "Microsoft YaHei",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(22, 8) },
                    { 11, new FontSizeInfo(22, 9) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 11) },
                    { 16, new FontSizeInfo(30, 12) }, { 18, new FontSizeInfo(33, 14) },
                    { 20, new FontSizeInfo(37, 16) }, { 22, new FontSizeInfo(40, 17) },
                    { 24, new FontSizeInfo(43, 19) }, { 26, new FontSizeInfo(49, 21) },
                    { 28, new FontSizeInfo(51, 22) }, { 36, new FontSizeInfo(65, 28) },
                    { 48, new FontSizeInfo(86, 38) }, { 72, new FontSizeInfo(128, 56) },
                    { 96, new FontSizeInfo(170, 75) }, { 128, new FontSizeInfo(228, 100) },
                    { 256, new FontSizeInfo(471, 200) }
                }
            },
            {
                "Microsoft YaHei UI",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(22, 8) },
                    { 11, new FontSizeInfo(22, 9) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 11) },
                    { 16, new FontSizeInfo(30, 12) }, { 18, new FontSizeInfo(33, 14) },
                    { 20, new FontSizeInfo(37, 16) }, { 22, new FontSizeInfo(40, 17) },
                    { 24, new FontSizeInfo(43, 19) }, { 26, new FontSizeInfo(49, 21) },
                    { 28, new FontSizeInfo(51, 22) }, { 36, new FontSizeInfo(65, 28) },
                    { 48, new FontSizeInfo(86, 38) }, { 72, new FontSizeInfo(128, 56) },
                    { 96, new FontSizeInfo(170, 75) }, { 128, new FontSizeInfo(228, 100) },
                    { 256, new FontSizeInfo(439, 200) }
                }
            },
            {
                "Microsoft YaHei Light",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(22, 7) },
                    { 11, new FontSizeInfo(22, 9) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 11) },
                    { 16, new FontSizeInfo(30, 12) }, { 18, new FontSizeInfo(33, 14) },
                    { 20, new FontSizeInfo(37, 15) }, { 22, new FontSizeInfo(42, 17) },
                    { 24, new FontSizeInfo(45, 18) }, { 26, new FontSizeInfo(51, 20) },
                    { 28, new FontSizeInfo(53, 21) }, { 36, new FontSizeInfo(67, 28) },
                    { 48, new FontSizeInfo(88, 37) }, { 72, new FontSizeInfo(132, 55) },
                    { 96, new FontSizeInfo(176, 73) }, { 128, new FontSizeInfo(236, 98) },
                    { 256, new FontSizeInfo(459, 195) }
                }
            },
            {
                "Microsoft YaHei UI Light",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(22, 7) },
                    { 11, new FontSizeInfo(22, 9) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 11) },
                    { 16, new FontSizeInfo(30, 12) }, { 18, new FontSizeInfo(33, 14) },
                    { 20, new FontSizeInfo(37, 15) }, { 22, new FontSizeInfo(40, 17) },
                    { 24, new FontSizeInfo(43, 18) }, { 26, new FontSizeInfo(49, 20) },
                    { 28, new FontSizeInfo(51, 21) }, { 36, new FontSizeInfo(65, 28) },
                    { 48, new FontSizeInfo(86, 37) }, { 72, new FontSizeInfo(128, 55) },
                    { 96, new FontSizeInfo(170, 73) }, { 128, new FontSizeInfo(228, 98) },
                    { 256, new FontSizeInfo(469, 195) }
                }
            },
            {
                "Microsoft Yi Baiti",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(24, 10) },
                    { 16, new FontSizeInfo(26, 11) }, { 18, new FontSizeInfo(29, 12) },
                    { 20, new FontSizeInfo(32, 14) }, { 22, new FontSizeInfo(34, 15) },
                    { 24, new FontSizeInfo(38, 16) }, { 26, new FontSizeInfo(41, 18) },
                    { 28, new FontSizeInfo(43, 19) }, { 36, new FontSizeInfo(56, 25) },
                    { 48, new FontSizeInfo(74, 33) }, { 72, new FontSizeInfo(0, 49) },
                    { 96, new FontSizeInfo(149, 66) }, { 128, new FontSizeInfo(198, 88) },
                    { 256, new FontSizeInfo(398, 175) }
                }
            },
            {
                "MingLiU-ExtB",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(21, 8) }, { 12, new FontSizeInfo(22, 8) }, { 14, new FontSizeInfo(26, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(34, 12) },
                    { 20, new FontSizeInfo(37, 14) }, { 22, new FontSizeInfo(40, 15) },
                    { 24, new FontSizeInfo(43, 16) }, { 26, new FontSizeInfo(49, 18) },
                    { 28, new FontSizeInfo(51, 19) }, { 36, new FontSizeInfo(67, 24) },
                    { 48, new FontSizeInfo(90, 32) }, { 72, new FontSizeInfo(0, 48) },
                    { 96, new FontSizeInfo(179, 64) }, { 128, new FontSizeInfo(238, 86) },
                    { 256, new FontSizeInfo(476, 171) }
                }
            },
            {
                "PMingLiU-ExtB",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(21, 7) }, { 12, new FontSizeInfo(22, 8) }, { 14, new FontSizeInfo(26, 9) },
                    { 16, new FontSizeInfo(28, 10) }, { 18, new FontSizeInfo(34, 11) },
                    { 20, new FontSizeInfo(37, 13) }, { 22, new FontSizeInfo(40, 14) },
                    { 24, new FontSizeInfo(43, 15) }, { 26, new FontSizeInfo(49, 16) },
                    { 28, new FontSizeInfo(51, 17) }, { 36, new FontSizeInfo(67, 23) },
                    { 48, new FontSizeInfo(90, 30) }, { 72, new FontSizeInfo(0, 45) },
                    { 96, new FontSizeInfo(179, 60) }, { 128, new FontSizeInfo(238, 80) },
                    { 256, new FontSizeInfo(476, 160) }
                }
            },
            {
                "MingLiU_HKSCS-ExtB",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(21, 8) }, { 12, new FontSizeInfo(22, 8) }, { 14, new FontSizeInfo(26, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(34, 12) },
                    { 20, new FontSizeInfo(37, 14) }, { 22, new FontSizeInfo(40, 15) },
                    { 24, new FontSizeInfo(43, 16) }, { 26, new FontSizeInfo(49, 18) },
                    { 28, new FontSizeInfo(51, 19) }, { 36, new FontSizeInfo(67, 24) },
                    { 48, new FontSizeInfo(90, 32) }, { 72, new FontSizeInfo(0, 48) },
                    { 96, new FontSizeInfo(179, 64) }, { 128, new FontSizeInfo(238, 86) },
                    { 256, new FontSizeInfo(476, 171) }
                }
            },
            {
                "Mongolian Baiti",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(24, 10) },
                    { 16, new FontSizeInfo(27, 11) }, { 18, new FontSizeInfo(30, 12) },
                    { 20, new FontSizeInfo(34, 14) }, { 22, new FontSizeInfo(38, 15) },
                    { 24, new FontSizeInfo(42, 16) }, { 26, new FontSizeInfo(46, 18) },
                    { 28, new FontSizeInfo(48, 19) }, { 36, new FontSizeInfo(61, 24) },
                    { 48, new FontSizeInfo(83, 32) }, { 72, new FontSizeInfo(0, 48) },
                    { 96, new FontSizeInfo(167, 64) }, { 128, new FontSizeInfo(223, 86) },
                    { 256, new FontSizeInfo(447, 171) }
                }
            },
            {
                "MV Boli",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 8) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(22, 11) }, { 12, new FontSizeInfo(23, 11) },
                    { 14, new FontSizeInfo(27, 13) }, { 16, new FontSizeInfo(30, 15) },
                    { 18, new FontSizeInfo(33, 17) }, { 20, new FontSizeInfo(35, 19) },
                    { 22, new FontSizeInfo(42, 20) }, { 24, new FontSizeInfo(43, 22) },
                    { 26, new FontSizeInfo(49, 25) }, { 28, new FontSizeInfo(51, 26) },
                    { 36, new FontSizeInfo(65, 34) }, { 48, new FontSizeInfo(89, 45) }, { 72, new FontSizeInfo(0, 67) },
                    { 96, new FontSizeInfo(171, 90) }, { 128, new FontSizeInfo(231, 120) },
                    { 256, new FontSizeInfo(597, 239) }
                }
            },
            {
                "Myanmar Text",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(25, 6) }, { 10, new FontSizeInfo(26, 7) },
                    { 11, new FontSizeInfo(29, 8) }, { 12, new FontSizeInfo(31, 9) }, { 14, new FontSizeInfo(36, 10) },
                    { 16, new FontSizeInfo(39, 11) }, { 18, new FontSizeInfo(45, 13) },
                    { 20, new FontSizeInfo(50, 15) }, { 22, new FontSizeInfo(53, 16) },
                    { 24, new FontSizeInfo(58, 17) }, { 26, new FontSizeInfo(64, 19) },
                    { 28, new FontSizeInfo(67, 20) }, { 36, new FontSizeInfo(88, 26) },
                    { 48, new FontSizeInfo(116, 35) }, { 72, new FontSizeInfo(0, 52) },
                    { 96, new FontSizeInfo(233, 69) }, { 128, new FontSizeInfo(309, 92) },
                    { 256, new FontSizeInfo(647, 184) }
                }
            },
            {
                "Nirmala UI",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 17) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(92, 35) }, { 72, new FontSizeInfo(0, 52) },
                    { 96, new FontSizeInfo(180, 69) }, { 128, new FontSizeInfo(241, 92) },
                    { 256, new FontSizeInfo(482, 184) }
                }
            },
            {
                "Nirmala UI Semilight",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 18) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(92, 35) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(180, 70) }, { 128, new FontSizeInfo(241, 94) },
                    { 256, new FontSizeInfo(482, 187) }
                }
            },
            {
                "Palatino Linotype",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(21, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(24, 8) }, { 14, new FontSizeInfo(28, 10) },
                    { 16, new FontSizeInfo(30, 11) }, { 18, new FontSizeInfo(34, 12) },
                    { 20, new FontSizeInfo(38, 14) }, { 22, new FontSizeInfo(40, 15) },
                    { 24, new FontSizeInfo(47, 16) }, { 26, new FontSizeInfo(50, 18) },
                    { 28, new FontSizeInfo(53, 19) }, { 36, new FontSizeInfo(67, 24) },
                    { 48, new FontSizeInfo(90, 32) }, { 72, new FontSizeInfo(0, 48) },
                    { 96, new FontSizeInfo(178, 64) }, { 128, new FontSizeInfo(240, 86) },
                    { 256, new FontSizeInfo(478, 171) }
                }
            },
            {
                "Segoe MDL2 Assets",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(22, 10) }, { 12, new FontSizeInfo(23, 10) },
                    { 14, new FontSizeInfo(26, 12) }, { 16, new FontSizeInfo(28, 14) },
                    { 18, new FontSizeInfo(31, 15) }, { 20, new FontSizeInfo(34, 17) },
                    { 22, new FontSizeInfo(36, 19) }, { 24, new FontSizeInfo(39, 21) },
                    { 26, new FontSizeInfo(42, 23) }, { 28, new FontSizeInfo(44, 24) },
                    { 36, new FontSizeInfo(55, 31) }, { 48, new FontSizeInfo(71, 41) }, { 72, new FontSizeInfo(0, 62) },
                    { 96, new FontSizeInfo(135, 83) }, { 128, new FontSizeInfo(178, 110) },
                    { 256, new FontSizeInfo(348, 219) }
                }
            },
            {
                "Segoe Print",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(24, 8) }, { 10, new FontSizeInfo(28, 10) },
                    { 11, new FontSizeInfo(31, 11) }, { 12, new FontSizeInfo(33, 12) },
                    { 14, new FontSizeInfo(39, 14) }, { 16, new FontSizeInfo(42, 15) },
                    { 18, new FontSizeInfo(49, 18) }, { 20, new FontSizeInfo(55, 20) },
                    { 22, new FontSizeInfo(58, 21) }, { 24, new FontSizeInfo(65, 23) },
                    { 26, new FontSizeInfo(71, 26) }, { 28, new FontSizeInfo(74, 27) },
                    { 36, new FontSizeInfo(97, 35) }, { 48, new FontSizeInfo(129, 47) },
                    { 72, new FontSizeInfo(0, 70) }, { 96, new FontSizeInfo(259, 94) },
                    { 128, new FontSizeInfo(347, 125) }, { 256, new FontSizeInfo(687, 248) }
                }
            },
            {
                "Segoe Script",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(22, 8) }, { 10, new FontSizeInfo(23, 10) },
                    { 11, new FontSizeInfo(25, 11) }, { 12, new FontSizeInfo(27, 12) },
                    { 14, new FontSizeInfo(33, 14) }, { 16, new FontSizeInfo(36, 15) },
                    { 18, new FontSizeInfo(41, 18) }, { 20, new FontSizeInfo(45, 20) },
                    { 22, new FontSizeInfo(50, 21) }, { 24, new FontSizeInfo(55, 23) },
                    { 26, new FontSizeInfo(59, 26) }, { 28, new FontSizeInfo(62, 27) },
                    { 36, new FontSizeInfo(81, 35) }, { 48, new FontSizeInfo(109, 47) },
                    { 72, new FontSizeInfo(0, 70) }, { 96, new FontSizeInfo(214, 94) },
                    { 128, new FontSizeInfo(287, 125) }, { 256, new FontSizeInfo(571, 248) }
                }
            },
            {
                "Segoe UI",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 17) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(92, 35) }, { 72, new FontSizeInfo(0, 52) },
                    { 96, new FontSizeInfo(180, 69) }, { 128, new FontSizeInfo(241, 92) },
                    { 256, new FontSizeInfo(482, 184) }
                }
            },
            {
                "Segoe UI Black",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(22, 9) }, { 12, new FontSizeInfo(23, 10) }, { 14, new FontSizeInfo(27, 12) },
                    { 16, new FontSizeInfo(34, 13) }, { 18, new FontSizeInfo(35, 15) },
                    { 20, new FontSizeInfo(41, 17) }, { 22, new FontSizeInfo(44, 18) },
                    { 24, new FontSizeInfo(50, 20) }, { 26, new FontSizeInfo(51, 22) },
                    { 28, new FontSizeInfo(54, 23) }, { 36, new FontSizeInfo(70, 30) },
                    { 48, new FontSizeInfo(92, 40) }, { 72, new FontSizeInfo(0, 60) },
                    { 96, new FontSizeInfo(182, 79) }, { 128, new FontSizeInfo(245, 106) },
                    { 256, new FontSizeInfo(488, 212) }
                }
            },
            {
                "Segoe UI Emoji",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 17) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(92, 35) }, { 72, new FontSizeInfo(0, 52) },
                    { 96, new FontSizeInfo(180, 69) }, { 128, new FontSizeInfo(241, 92) },
                    { 256, new FontSizeInfo(482, 184) }
                }
            },
            {
                "Segoe UI Historic",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 17) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(92, 35) }, { 72, new FontSizeInfo(0, 52) },
                    { 96, new FontSizeInfo(180, 69) }, { 128, new FontSizeInfo(241, 92) },
                    { 256, new FontSizeInfo(482, 184) }
                }
            },
            {
                "Segoe UI Light",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 8) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 14) }, { 22, new FontSizeInfo(44, 15) },
                    { 24, new FontSizeInfo(50, 17) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 25) },
                    { 48, new FontSizeInfo(92, 34) }, { 72, new FontSizeInfo(0, 51) },
                    { 96, new FontSizeInfo(180, 68) }, { 128, new FontSizeInfo(241, 91) },
                    { 256, new FontSizeInfo(482, 181) }
                }
            },
            {
                "Segoe UI Semibold",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 9) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 11) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(35, 14) },
                    { 20, new FontSizeInfo(41, 16) }, { 22, new FontSizeInfo(44, 17) },
                    { 24, new FontSizeInfo(50, 18) }, { 26, new FontSizeInfo(51, 20) },
                    { 28, new FontSizeInfo(54, 21) }, { 36, new FontSizeInfo(70, 28) },
                    { 48, new FontSizeInfo(92, 37) }, { 72, new FontSizeInfo(0, 55) },
                    { 96, new FontSizeInfo(180, 74) }, { 128, new FontSizeInfo(241, 99) },
                    { 256, new FontSizeInfo(482, 196) }
                }
            },
            {
                "Segoe UI Semilight",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 18) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(92, 35) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(180, 70) }, { 128, new FontSizeInfo(241, 94) },
                    { 256, new FontSizeInfo(482, 187) }
                }
            },
            {
                "Segoe UI Symbol",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 17) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(92, 35) }, { 72, new FontSizeInfo(0, 52) },
                    { 96, new FontSizeInfo(180, 69) }, { 128, new FontSizeInfo(241, 92) },
                    { 256, new FontSizeInfo(482, 184) }
                }
            },
            {
                "NSimSun",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(27, 11) }, { 18, new FontSizeInfo(30, 12) },
                    { 20, new FontSizeInfo(34, 14) }, { 22, new FontSizeInfo(36, 15) },
                    { 24, new FontSizeInfo(42, 16) }, { 26, new FontSizeInfo(45, 18) },
                    { 28, new FontSizeInfo(47, 19) }, { 36, new FontSizeInfo(62, 24) },
                    { 48, new FontSizeInfo(82, 32) }, { 72, new FontSizeInfo(0, 48) },
                    { 96, new FontSizeInfo(163, 64) }, { 128, new FontSizeInfo(218, 86) },
                    { 256, new FontSizeInfo(436, 171) }
                }
            },
            {
                "SimSun-ExtB",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(24, 10) },
                    { 16, new FontSizeInfo(26, 11) }, { 18, new FontSizeInfo(29, 12) },
                    { 20, new FontSizeInfo(32, 14) }, { 22, new FontSizeInfo(34, 15) },
                    { 24, new FontSizeInfo(38, 16) }, { 26, new FontSizeInfo(41, 18) },
                    { 28, new FontSizeInfo(43, 19) }, { 36, new FontSizeInfo(56, 24) },
                    { 48, new FontSizeInfo(74, 32) }, { 72, new FontSizeInfo(0, 48) },
                    { 96, new FontSizeInfo(147, 64) }, { 128, new FontSizeInfo(196, 86) },
                    { 256, new FontSizeInfo(390, 171) }
                }
            },
            {
                "Sitka Small",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 8) }, { 10, new FontSizeInfo(22, 9) },
                    { 11, new FontSizeInfo(27, 10) }, { 12, new FontSizeInfo(28, 11) },
                    { 14, new FontSizeInfo(32, 13) }, { 16, new FontSizeInfo(36, 15) },
                    { 18, new FontSizeInfo(40, 17) }, { 20, new FontSizeInfo(46, 19) },
                    { 22, new FontSizeInfo(49, 20) }, { 24, new FontSizeInfo(55, 22) },
                    { 26, new FontSizeInfo(59, 24) }, { 28, new FontSizeInfo(61, 26) },
                    { 36, new FontSizeInfo(80, 33) }, { 48, new FontSizeInfo(106, 44) },
                    { 72, new FontSizeInfo(0, 66) }, { 96, new FontSizeInfo(212, 88) },
                    { 128, new FontSizeInfo(284, 118) }, { 256, new FontSizeInfo(564, 235) }
                }
            },
            {
                "Sitka Text",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 7) }, { 10, new FontSizeInfo(22, 8) },
                    { 11, new FontSizeInfo(24, 10) }, { 12, new FontSizeInfo(26, 10) },
                    { 14, new FontSizeInfo(32, 12) }, { 16, new FontSizeInfo(34, 13) },
                    { 18, new FontSizeInfo(40, 15) }, { 20, new FontSizeInfo(44, 17) },
                    { 22, new FontSizeInfo(47, 18) }, { 24, new FontSizeInfo(53, 20) },
                    { 26, new FontSizeInfo(56, 22) }, { 28, new FontSizeInfo(59, 24) },
                    { 36, new FontSizeInfo(77, 31) }, { 48, new FontSizeInfo(102, 41) },
                    { 72, new FontSizeInfo(0, 61) }, { 96, new FontSizeInfo(205, 81) },
                    { 128, new FontSizeInfo(273, 109) }, { 256, new FontSizeInfo(544, 216) }
                }
            },
            {
                "Sitka Subheading",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 7) }, { 10, new FontSizeInfo(22, 8) },
                    { 11, new FontSizeInfo(24, 9) }, { 12, new FontSizeInfo(26, 9) }, { 14, new FontSizeInfo(32, 11) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(40, 14) },
                    { 20, new FontSizeInfo(44, 16) }, { 22, new FontSizeInfo(47, 17) },
                    { 24, new FontSizeInfo(53, 19) }, { 26, new FontSizeInfo(56, 21) },
                    { 28, new FontSizeInfo(59, 22) }, { 36, new FontSizeInfo(77, 28) },
                    { 48, new FontSizeInfo(102, 38) }, { 72, new FontSizeInfo(0, 57) },
                    { 96, new FontSizeInfo(205, 76) }, { 128, new FontSizeInfo(273, 101) },
                    { 256, new FontSizeInfo(544, 201) }
                }
            },
            {
                "Sitka Heading",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(22, 7) },
                    { 11, new FontSizeInfo(24, 8) }, { 12, new FontSizeInfo(26, 9) }, { 14, new FontSizeInfo(32, 11) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(40, 13) },
                    { 20, new FontSizeInfo(44, 15) }, { 22, new FontSizeInfo(47, 16) },
                    { 24, new FontSizeInfo(53, 18) }, { 26, new FontSizeInfo(56, 20) },
                    { 28, new FontSizeInfo(59, 21) }, { 36, new FontSizeInfo(77, 27) },
                    { 48, new FontSizeInfo(102, 36) }, { 72, new FontSizeInfo(0, 54) },
                    { 96, new FontSizeInfo(205, 71) }, { 128, new FontSizeInfo(273, 95) },
                    { 256, new FontSizeInfo(544, 190) }
                }
            },
            {
                "Sitka Display",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(22, 7) },
                    { 11, new FontSizeInfo(24, 8) }, { 12, new FontSizeInfo(26, 8) }, { 14, new FontSizeInfo(32, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(40, 13) },
                    { 20, new FontSizeInfo(44, 14) }, { 22, new FontSizeInfo(47, 15) },
                    { 24, new FontSizeInfo(53, 17) }, { 26, new FontSizeInfo(56, 19) },
                    { 28, new FontSizeInfo(59, 20) }, { 36, new FontSizeInfo(77, 25) },
                    { 48, new FontSizeInfo(102, 34) }, { 72, new FontSizeInfo(0, 51) },
                    { 96, new FontSizeInfo(205, 68) }, { 128, new FontSizeInfo(273, 91) },
                    { 256, new FontSizeInfo(544, 181) }
                }
            },
            {
                "Sitka Banner",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(22, 7) },
                    { 11, new FontSizeInfo(24, 8) }, { 12, new FontSizeInfo(26, 8) }, { 14, new FontSizeInfo(32, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(40, 12) },
                    { 20, new FontSizeInfo(44, 14) }, { 22, new FontSizeInfo(47, 15) },
                    { 24, new FontSizeInfo(53, 16) }, { 26, new FontSizeInfo(56, 18) },
                    { 28, new FontSizeInfo(59, 19) }, { 36, new FontSizeInfo(77, 24) },
                    { 48, new FontSizeInfo(102, 32) }, { 72, new FontSizeInfo(0, 49) },
                    { 96, new FontSizeInfo(205, 65) }, { 128, new FontSizeInfo(273, 87) },
                    { 256, new FontSizeInfo(544, 173) }
                }
            },
            {
                "Tahoma",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(24, 10) },
                    { 16, new FontSizeInfo(26, 11) }, { 18, new FontSizeInfo(30, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(36, 16) },
                    { 24, new FontSizeInfo(40, 17) }, { 26, new FontSizeInfo(43, 19) },
                    { 28, new FontSizeInfo(46, 20) }, { 36, new FontSizeInfo(59, 26) },
                    { 48, new FontSizeInfo(78, 35) }, { 72, new FontSizeInfo(0, 52) },
                    { 96, new FontSizeInfo(155, 70) }, { 128, new FontSizeInfo(207, 93) },
                    { 256, new FontSizeInfo(412, 186) }
                }
            },
            {
                "Trebuchet MS",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(21, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(24, 8) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(37, 14) }, { 22, new FontSizeInfo(38, 15) },
                    { 24, new FontSizeInfo(41, 17) }, { 26, new FontSizeInfo(45, 18) },
                    { 28, new FontSizeInfo(48, 19) }, { 36, new FontSizeInfo(62, 25) },
                    { 48, new FontSizeInfo(82, 34) }, { 72, new FontSizeInfo(0, 50) },
                    { 96, new FontSizeInfo(164, 67) }, { 128, new FontSizeInfo(219, 90) },
                    { 256, new FontSizeInfo(418, 179) }
                }
            },
            {
                "Verdana",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 10) }, { 12, new FontSizeInfo(21, 10) },
                    { 14, new FontSizeInfo(24, 12) }, { 16, new FontSizeInfo(26, 13) },
                    { 18, new FontSizeInfo(30, 15) }, { 20, new FontSizeInfo(33, 17) },
                    { 22, new FontSizeInfo(36, 18) }, { 24, new FontSizeInfo(39, 20) },
                    { 26, new FontSizeInfo(43, 22) }, { 28, new FontSizeInfo(47, 24) },
                    { 36, new FontSizeInfo(61, 31) }, { 48, new FontSizeInfo(80, 41) }, { 72, new FontSizeInfo(0, 61) },
                    { 96, new FontSizeInfo(156, 81) }, { 128, new FontSizeInfo(207, 109) },
                    { 256, new FontSizeInfo(418, 216) }
                }
            },
            {
                "Webdings",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(24, 8) }, { 8, new FontSizeInfo(22, 11) }, { 10, new FontSizeInfo(21, 13) },
                    { 11, new FontSizeInfo(21, 15) }, { 12, new FontSizeInfo(21, 16) },
                    { 14, new FontSizeInfo(26, 19) }, { 16, new FontSizeInfo(27, 21) },
                    { 18, new FontSizeInfo(31, 24) }, { 20, new FontSizeInfo(35, 27) },
                    { 22, new FontSizeInfo(36, 29) }, { 24, new FontSizeInfo(40, 32) },
                    { 26, new FontSizeInfo(43, 35) }, { 28, new FontSizeInfo(46, 37) },
                    { 36, new FontSizeInfo(59, 48) }, { 48, new FontSizeInfo(78, 64) }, { 72, new FontSizeInfo(0, 96) },
                    { 96, new FontSizeInfo(155, 128) }, { 128, new FontSizeInfo(206, 171) },
                    { 256, new FontSizeInfo(410, 341) }
                }
            },
            {
                "Yu Gothic UI",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 17) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(92, 35) }, { 72, new FontSizeInfo(0, 52) },
                    { 96, new FontSizeInfo(180, 69) }, { 128, new FontSizeInfo(241, 92) },
                    { 256, new FontSizeInfo(482, 184) }
                }
            },
            {
                "Yu Gothic UI Semibold",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 9) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 11) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(35, 14) },
                    { 20, new FontSizeInfo(41, 16) }, { 22, new FontSizeInfo(44, 17) },
                    { 24, new FontSizeInfo(50, 18) }, { 26, new FontSizeInfo(51, 20) },
                    { 28, new FontSizeInfo(54, 21) }, { 36, new FontSizeInfo(70, 28) },
                    { 48, new FontSizeInfo(92, 37) }, { 72, new FontSizeInfo(0, 55) },
                    { 96, new FontSizeInfo(180, 74) }, { 128, new FontSizeInfo(241, 99) },
                    { 256, new FontSizeInfo(482, 196) }
                }
            },
            {
                "Yu Gothic Light",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(22, 7) },
                    { 11, new FontSizeInfo(24, 8) }, { 12, new FontSizeInfo(26, 9) }, { 14, new FontSizeInfo(32, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(40, 13) },
                    { 20, new FontSizeInfo(44, 14) }, { 22, new FontSizeInfo(47, 16) },
                    { 24, new FontSizeInfo(53, 17) }, { 26, new FontSizeInfo(56, 19) },
                    { 28, new FontSizeInfo(61, 20) }, { 36, new FontSizeInfo(77, 26) },
                    { 48, new FontSizeInfo(102, 34) }, { 72, new FontSizeInfo(0, 51) },
                    { 96, new FontSizeInfo(205, 69) }, { 128, new FontSizeInfo(275, 92) },
                    { 256, new FontSizeInfo(550, 183) }
                }
            },
            {
                "Yu Gothic UI Light",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 14) }, { 22, new FontSizeInfo(44, 15) },
                    { 24, new FontSizeInfo(50, 17) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 25) },
                    { 48, new FontSizeInfo(92, 34) }, { 72, new FontSizeInfo(0, 51) },
                    { 96, new FontSizeInfo(180, 68) }, { 128, new FontSizeInfo(241, 91) },
                    { 256, new FontSizeInfo(482, 181) }
                }
            },
            {
                "Yu Gothic Medium",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(22, 7) },
                    { 11, new FontSizeInfo(24, 8) }, { 12, new FontSizeInfo(26, 9) }, { 14, new FontSizeInfo(32, 11) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(40, 13) },
                    { 20, new FontSizeInfo(44, 15) }, { 22, new FontSizeInfo(47, 16) },
                    { 24, new FontSizeInfo(53, 18) }, { 26, new FontSizeInfo(56, 19) },
                    { 28, new FontSizeInfo(61, 21) }, { 36, new FontSizeInfo(77, 27) },
                    { 48, new FontSizeInfo(102, 36) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(205, 71) }, { 128, new FontSizeInfo(275, 95) },
                    { 256, new FontSizeInfo(549, 189) }
                }
            },
            {
                "Yu Gothic UI Semilight",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 18) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(92, 35) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(180, 70) }, { 128, new FontSizeInfo(241, 94) },
                    { 256, new FontSizeInfo(482, 187) }
                }
            },
            {
                "MT Extra",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 8) }, { 8, new FontSizeInfo(20, 11) }, { 10, new FontSizeInfo(20, 13) },
                    { 11, new FontSizeInfo(21, 15) }, { 12, new FontSizeInfo(24, 16) },
                    { 14, new FontSizeInfo(28, 19) }, { 16, new FontSizeInfo(30, 21) },
                    { 18, new FontSizeInfo(36, 24) }, { 20, new FontSizeInfo(40, 28) },
                    { 22, new FontSizeInfo(42, 30) }, { 24, new FontSizeInfo(48, 33) },
                    { 26, new FontSizeInfo(51, 36) }, { 28, new FontSizeInfo(56, 38) },
                    { 36, new FontSizeInfo(71, 49) }, { 48, new FontSizeInfo(95, 65) }, { 72, new FontSizeInfo(0, 98) },
                    { 96, new FontSizeInfo(192, 131) }, { 128, new FontSizeInfo(257, 174) },
                    { 256, new FontSizeInfo(511, 347) }
                }
            },
            {
                "Agency FB",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(20, 6) }, { 12, new FontSizeInfo(21, 6) }, { 14, new FontSizeInfo(26, 8) },
                    { 16, new FontSizeInfo(26, 9) }, { 18, new FontSizeInfo(30, 10) }, { 20, new FontSizeInfo(34, 11) },
                    { 22, new FontSizeInfo(36, 11) }, { 24, new FontSizeInfo(44, 14) },
                    { 26, new FontSizeInfo(45, 15) }, { 28, new FontSizeInfo(48, 16) },
                    { 36, new FontSizeInfo(62, 21) }, { 48, new FontSizeInfo(81, 28) }, { 72, new FontSizeInfo(0, 42) },
                    { 96, new FontSizeInfo(161, 56) }, { 128, new FontSizeInfo(216, 74) },
                    { 256, new FontSizeInfo(430, 148) }
                }
            },
            {
                "Algerian",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(21, 9) }, { 12, new FontSizeInfo(23, 10) }, { 14, new FontSizeInfo(26, 11) },
                    { 16, new FontSizeInfo(29, 13) }, { 18, new FontSizeInfo(34, 14) },
                    { 20, new FontSizeInfo(38, 16) }, { 22, new FontSizeInfo(40, 17) },
                    { 24, new FontSizeInfo(46, 19) }, { 26, new FontSizeInfo(50, 21) },
                    { 28, new FontSizeInfo(52, 22) }, { 36, new FontSizeInfo(68, 29) },
                    { 48, new FontSizeInfo(91, 38) }, { 72, new FontSizeInfo(0, 58) },
                    { 96, new FontSizeInfo(184, 77) }, { 128, new FontSizeInfo(244, 103) },
                    { 256, new FontSizeInfo(488, 205) }
                }
            },
            {
                "Book Antiqua",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(35, 14) }, { 22, new FontSizeInfo(38, 15) },
                    { 24, new FontSizeInfo(42, 16) }, { 26, new FontSizeInfo(45, 18) },
                    { 28, new FontSizeInfo(48, 19) }, { 36, new FontSizeInfo(62, 24) },
                    { 48, new FontSizeInfo(83, 32) }, { 72, new FontSizeInfo(0, 48) },
                    { 96, new FontSizeInfo(164, 64) }, { 128, new FontSizeInfo(220, 86) },
                    { 256, new FontSizeInfo(438, 171) }
                }
            },
            {
                "Arial Narrow",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(22, 7) }, { 12, new FontSizeInfo(21, 7) }, { 14, new FontSizeInfo(24, 9) },
                    { 16, new FontSizeInfo(27, 10) }, { 18, new FontSizeInfo(31, 11) },
                    { 20, new FontSizeInfo(34, 12) }, { 22, new FontSizeInfo(36, 13) },
                    { 24, new FontSizeInfo(40, 15) }, { 26, new FontSizeInfo(45, 16) },
                    { 28, new FontSizeInfo(47, 17) }, { 36, new FontSizeInfo(61, 22) },
                    { 48, new FontSizeInfo(80, 29) }, { 72, new FontSizeInfo(0, 44) },
                    { 96, new FontSizeInfo(157, 58) }, { 128, new FontSizeInfo(210, 78) },
                    { 256, new FontSizeInfo(418, 156) }
                }
            },
            {
                "Arial Rounded MT Bold",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 10) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(26, 12) }, { 18, new FontSizeInfo(30, 14) },
                    { 20, new FontSizeInfo(34, 16) }, { 22, new FontSizeInfo(36, 17) },
                    { 24, new FontSizeInfo(40, 19) }, { 26, new FontSizeInfo(43, 21) },
                    { 28, new FontSizeInfo(46, 22) }, { 36, new FontSizeInfo(59, 29) },
                    { 48, new FontSizeInfo(79, 38) }, { 72, new FontSizeInfo(0, 57) },
                    { 96, new FontSizeInfo(156, 76) }, { 128, new FontSizeInfo(208, 102) },
                    { 256, new FontSizeInfo(414, 202) }
                }
            },
            {
                "Baskerville Old Face",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(20, 7) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 9) },
                    { 16, new FontSizeInfo(27, 10) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(35, 13) }, { 22, new FontSizeInfo(38, 14) },
                    { 24, new FontSizeInfo(41, 16) }, { 26, new FontSizeInfo(45, 17) },
                    { 28, new FontSizeInfo(48, 18) }, { 36, new FontSizeInfo(61, 24) },
                    { 48, new FontSizeInfo(82, 31) }, { 72, new FontSizeInfo(0, 47) },
                    { 96, new FontSizeInfo(162, 63) }, { 128, new FontSizeInfo(216, 84) },
                    { 256, new FontSizeInfo(430, 168) }
                }
            },
            {
                "Bauhaus 93",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(23, 9) }, { 12, new FontSizeInfo(25, 9) }, { 14, new FontSizeInfo(28, 11) },
                    { 16, new FontSizeInfo(33, 12) }, { 18, new FontSizeInfo(37, 14) },
                    { 20, new FontSizeInfo(42, 15) }, { 22, new FontSizeInfo(45, 16) },
                    { 24, new FontSizeInfo(50, 18) }, { 26, new FontSizeInfo(54, 20) },
                    { 28, new FontSizeInfo(57, 21) }, { 36, new FontSizeInfo(74, 27) },
                    { 48, new FontSizeInfo(100, 36) }, { 72, new FontSizeInfo(0, 55) },
                    { 96, new FontSizeInfo(199, 73) }, { 128, new FontSizeInfo(266, 97) },
                    { 256, new FontSizeInfo(531, 194) }
                }
            },
            {
                "Bell MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(21, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(35, 14) }, { 22, new FontSizeInfo(38, 15) },
                    { 24, new FontSizeInfo(42, 16) }, { 26, new FontSizeInfo(45, 18) },
                    { 28, new FontSizeInfo(48, 19) }, { 36, new FontSizeInfo(62, 24) },
                    { 48, new FontSizeInfo(82, 32) }, { 72, new FontSizeInfo(0, 48) },
                    { 96, new FontSizeInfo(163, 64) }, { 128, new FontSizeInfo(218, 86) },
                    { 256, new FontSizeInfo(433, 171) }
                }
            },
            {
                "Bernard MT Condensed",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(20, 7) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(24, 9) },
                    { 16, new FontSizeInfo(26, 10) }, { 18, new FontSizeInfo(30, 12) },
                    { 20, new FontSizeInfo(33, 13) }, { 22, new FontSizeInfo(36, 14) },
                    { 24, new FontSizeInfo(39, 16) }, { 26, new FontSizeInfo(43, 17) },
                    { 28, new FontSizeInfo(45, 18) }, { 36, new FontSizeInfo(59, 24) },
                    { 48, new FontSizeInfo(79, 31) }, { 72, new FontSizeInfo(0, 47) },
                    { 96, new FontSizeInfo(156, 63) }, { 128, new FontSizeInfo(208, 84) },
                    { 256, new FontSizeInfo(416, 167) }
                }
            },
            {
                "Bodoni MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(20, 7) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(26, 9) },
                    { 16, new FontSizeInfo(27, 10) }, { 18, new FontSizeInfo(32, 12) },
                    { 20, new FontSizeInfo(35, 13) }, { 22, new FontSizeInfo(39, 14) },
                    { 24, new FontSizeInfo(42, 16) }, { 26, new FontSizeInfo(45, 17) },
                    { 28, new FontSizeInfo(48, 18) }, { 36, new FontSizeInfo(61, 24) },
                    { 48, new FontSizeInfo(83, 31) }, { 72, new FontSizeInfo(0, 47) },
                    { 96, new FontSizeInfo(164, 63) }, { 128, new FontSizeInfo(220, 84) },
                    { 256, new FontSizeInfo(438, 167) }
                }
            },
            {
                "Bodoni MT Black",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(20, 10) }, { 12, new FontSizeInfo(21, 11) },
                    { 14, new FontSizeInfo(25, 12) }, { 16, new FontSizeInfo(27, 14) },
                    { 18, new FontSizeInfo(31, 16) }, { 20, new FontSizeInfo(35, 18) },
                    { 22, new FontSizeInfo(37, 19) }, { 24, new FontSizeInfo(41, 21) },
                    { 26, new FontSizeInfo(45, 23) }, { 28, new FontSizeInfo(47, 24) },
                    { 36, new FontSizeInfo(61, 31) }, { 48, new FontSizeInfo(81, 42) }, { 72, new FontSizeInfo(0, 63) },
                    { 96, new FontSizeInfo(161, 84) }, { 128, new FontSizeInfo(215, 112) },
                    { 256, new FontSizeInfo(428, 224) }
                }
            },
            {
                "Bodoni MT Condensed",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(20, 4) },
                    { 11, new FontSizeInfo(20, 5) }, { 12, new FontSizeInfo(21, 5) }, { 14, new FontSizeInfo(25, 6) },
                    { 16, new FontSizeInfo(27, 7) }, { 18, new FontSizeInfo(31, 8) }, { 20, new FontSizeInfo(35, 9) },
                    { 22, new FontSizeInfo(38, 10) }, { 24, new FontSizeInfo(41, 11) },
                    { 26, new FontSizeInfo(45, 12) }, { 28, new FontSizeInfo(48, 13) },
                    { 36, new FontSizeInfo(61, 16) }, { 48, new FontSizeInfo(82, 22) }, { 72, new FontSizeInfo(0, 32) },
                    { 96, new FontSizeInfo(162, 43) }, { 128, new FontSizeInfo(216, 58) },
                    { 256, new FontSizeInfo(431, 115) }
                }
            },
            {
                "Bodoni MT Poster Compressed",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(20, 5) }, { 12, new FontSizeInfo(21, 6) }, { 14, new FontSizeInfo(24, 6) },
                    { 16, new FontSizeInfo(27, 7) }, { 18, new FontSizeInfo(30, 8) }, { 20, new FontSizeInfo(34, 8) },
                    { 22, new FontSizeInfo(36, 9) }, { 24, new FontSizeInfo(40, 9) }, { 26, new FontSizeInfo(44, 10) },
                    { 28, new FontSizeInfo(46, 10) }, { 36, new FontSizeInfo(59, 14) },
                    { 48, new FontSizeInfo(79, 18) }, { 72, new FontSizeInfo(0, 28) },
                    { 96, new FontSizeInfo(157, 38) }, { 128, new FontSizeInfo(209, 49) },
                    { 256, new FontSizeInfo(416, 96) }
                }
            },
            {
                "Bookman Old Style",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(21, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 10) }, { 14, new FontSizeInfo(24, 12) },
                    { 16, new FontSizeInfo(27, 13) }, { 18, new FontSizeInfo(31, 15) },
                    { 20, new FontSizeInfo(34, 17) }, { 22, new FontSizeInfo(37, 18) },
                    { 24, new FontSizeInfo(42, 20) }, { 26, new FontSizeInfo(44, 22) },
                    { 28, new FontSizeInfo(47, 23) }, { 36, new FontSizeInfo(61, 30) },
                    { 48, new FontSizeInfo(82, 40) }, { 72, new FontSizeInfo(0, 60) },
                    { 96, new FontSizeInfo(163, 79) }, { 128, new FontSizeInfo(218, 106) },
                    { 256, new FontSizeInfo(437, 211) }
                }
            },
            {
                "Bradley Hand ITC",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(21, 8) },
                    { 11, new FontSizeInfo(22, 10) }, { 12, new FontSizeInfo(23, 10) },
                    { 14, new FontSizeInfo(28, 12) }, { 16, new FontSizeInfo(30, 14) },
                    { 18, new FontSizeInfo(35, 15) }, { 20, new FontSizeInfo(39, 17) },
                    { 22, new FontSizeInfo(42, 19) }, { 24, new FontSizeInfo(46, 21) },
                    { 26, new FontSizeInfo(50, 23) }, { 28, new FontSizeInfo(53, 24) },
                    { 36, new FontSizeInfo(68, 31) }, { 48, new FontSizeInfo(90, 41) }, { 72, new FontSizeInfo(0, 62) },
                    { 96, new FontSizeInfo(180, 82) }, { 128, new FontSizeInfo(240, 110) },
                    { 256, new FontSizeInfo(478, 219) }
                }
            },
            {
                "Britannic Bold",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 10) }, { 14, new FontSizeInfo(24, 12) },
                    { 16, new FontSizeInfo(26, 13) }, { 18, new FontSizeInfo(30, 15) },
                    { 20, new FontSizeInfo(34, 17) }, { 22, new FontSizeInfo(36, 18) },
                    { 24, new FontSizeInfo(40, 20) }, { 26, new FontSizeInfo(43, 21) },
                    { 28, new FontSizeInfo(46, 23) }, { 36, new FontSizeInfo(59, 29) },
                    { 48, new FontSizeInfo(79, 39) }, { 72, new FontSizeInfo(0, 59) },
                    { 96, new FontSizeInfo(156, 78) }, { 128, new FontSizeInfo(208, 105) },
                    { 256, new FontSizeInfo(414, 209) }
                }
            },
            {
                "Berlin Sans FB",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 10) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(26, 13) }, { 18, new FontSizeInfo(30, 14) },
                    { 20, new FontSizeInfo(33, 16) }, { 22, new FontSizeInfo(36, 17) },
                    { 24, new FontSizeInfo(39, 19) }, { 26, new FontSizeInfo(43, 21) },
                    { 28, new FontSizeInfo(45, 22) }, { 36, new FontSizeInfo(59, 29) },
                    { 48, new FontSizeInfo(78, 38) }, { 72, new FontSizeInfo(0, 57) },
                    { 96, new FontSizeInfo(155, 76) }, { 128, new FontSizeInfo(207, 102) },
                    { 256, new FontSizeInfo(411, 203) }
                }
            },
            {
                "Berlin Sans FB Demi",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 10) }, { 14, new FontSizeInfo(24, 12) },
                    { 16, new FontSizeInfo(26, 13) }, { 18, new FontSizeInfo(30, 15) },
                    { 20, new FontSizeInfo(34, 17) }, { 22, new FontSizeInfo(36, 18) },
                    { 24, new FontSizeInfo(40, 20) }, { 26, new FontSizeInfo(43, 22) },
                    { 28, new FontSizeInfo(46, 23) }, { 36, new FontSizeInfo(59, 30) },
                    { 48, new FontSizeInfo(78, 40) }, { 72, new FontSizeInfo(0, 60) },
                    { 96, new FontSizeInfo(155, 81) }, { 128, new FontSizeInfo(207, 108) },
                    { 256, new FontSizeInfo(412, 215) }
                }
            },
            {
                "Broadway",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 10) }, { 12, new FontSizeInfo(21, 10) },
                    { 14, new FontSizeInfo(24, 12) }, { 16, new FontSizeInfo(27, 14) },
                    { 18, new FontSizeInfo(30, 16) }, { 20, new FontSizeInfo(34, 17) },
                    { 22, new FontSizeInfo(36, 19) }, { 24, new FontSizeInfo(40, 21) },
                    { 26, new FontSizeInfo(44, 23) }, { 28, new FontSizeInfo(46, 24) },
                    { 36, new FontSizeInfo(60, 31) }, { 48, new FontSizeInfo(79, 41) }, { 72, new FontSizeInfo(0, 62) },
                    { 96, new FontSizeInfo(157, 83) }, { 128, new FontSizeInfo(210, 111) },
                    { 256, new FontSizeInfo(417, 219) }
                }
            },
            {
                "Brush Script MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(21, 8) }, { 12, new FontSizeInfo(22, 8) }, { 14, new FontSizeInfo(26, 10) },
                    { 16, new FontSizeInfo(29, 11) }, { 18, new FontSizeInfo(33, 12) },
                    { 20, new FontSizeInfo(37, 14) }, { 22, new FontSizeInfo(40, 15) },
                    { 24, new FontSizeInfo(44, 16) }, { 26, new FontSizeInfo(48, 18) },
                    { 28, new FontSizeInfo(51, 19) }, { 36, new FontSizeInfo(65, 24) },
                    { 48, new FontSizeInfo(87, 33) }, { 72, new FontSizeInfo(0, 49) },
                    { 96, new FontSizeInfo(172, 65) }, { 128, new FontSizeInfo(230, 87) },
                    { 256, new FontSizeInfo(457, 174) }
                }
            },
            {
                "Bookshelf Symbol 7",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 8) }, { 10, new FontSizeInfo(20, 10) },
                    { 11, new FontSizeInfo(20, 11) }, { 12, new FontSizeInfo(21, 12) },
                    { 14, new FontSizeInfo(24, 14) }, { 16, new FontSizeInfo(26, 16) },
                    { 18, new FontSizeInfo(29, 18) }, { 20, new FontSizeInfo(32, 20) },
                    { 22, new FontSizeInfo(34, 21) }, { 24, new FontSizeInfo(38, 24) },
                    { 26, new FontSizeInfo(41, 26) }, { 28, new FontSizeInfo(43, 27) },
                    { 36, new FontSizeInfo(56, 35) }, { 48, new FontSizeInfo(74, 47) }, { 72, new FontSizeInfo(0, 71) },
                    { 96, new FontSizeInfo(147, 95) }, { 128, new FontSizeInfo(196, 126) },
                    { 256, new FontSizeInfo(390, 251) }
                }
            },
            {
                "Californian FB",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 5) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(35, 14) }, { 22, new FontSizeInfo(37, 15) },
                    { 24, new FontSizeInfo(41, 17) }, { 26, new FontSizeInfo(45, 18) },
                    { 28, new FontSizeInfo(47, 19) }, { 36, new FontSizeInfo(61, 26) },
                    { 48, new FontSizeInfo(81, 34) }, { 72, new FontSizeInfo(0, 51) },
                    { 96, new FontSizeInfo(162, 68) }, { 128, new FontSizeInfo(216, 91) },
                    { 256, new FontSizeInfo(429, 181) }
                }
            },
            {
                "Calisto MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(24, 10) },
                    { 16, new FontSizeInfo(27, 11) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(34, 14) }, { 22, new FontSizeInfo(37, 15) },
                    { 24, new FontSizeInfo(40, 16) }, { 26, new FontSizeInfo(44, 18) },
                    { 28, new FontSizeInfo(46, 19) }, { 36, new FontSizeInfo(60, 24) },
                    { 48, new FontSizeInfo(80, 33) }, { 72, new FontSizeInfo(0, 49) },
                    { 96, new FontSizeInfo(158, 65) }, { 128, new FontSizeInfo(211, 87) },
                    { 256, new FontSizeInfo(420, 174) }
                }
            },
            {
                "Castellar",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 8) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(20, 11) }, { 12, new FontSizeInfo(21, 12) },
                    { 14, new FontSizeInfo(25, 14) }, { 16, new FontSizeInfo(28, 15) },
                    { 18, new FontSizeInfo(32, 17) }, { 20, new FontSizeInfo(36, 19) },
                    { 22, new FontSizeInfo(38, 21) }, { 24, new FontSizeInfo(42, 23) },
                    { 26, new FontSizeInfo(46, 25) }, { 28, new FontSizeInfo(48, 27) },
                    { 36, new FontSizeInfo(62, 35) }, { 48, new FontSizeInfo(83, 46) }, { 72, new FontSizeInfo(0, 69) },
                    { 96, new FontSizeInfo(165, 92) }, { 128, new FontSizeInfo(220, 123) },
                    { 256, new FontSizeInfo(437, 245) }
                }
            },
            {
                "Century Schoolbook",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(30, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(36, 16) },
                    { 24, new FontSizeInfo(40, 18) }, { 26, new FontSizeInfo(44, 19) },
                    { 28, new FontSizeInfo(46, 21) }, { 36, new FontSizeInfo(59, 27) },
                    { 48, new FontSizeInfo(79, 36) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(157, 71) }, { 128, new FontSizeInfo(209, 95) },
                    { 256, new FontSizeInfo(416, 190) }
                }
            },
            {
                "Centaur",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(20, 7) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 9) },
                    { 16, new FontSizeInfo(28, 10) }, { 18, new FontSizeInfo(32, 12) },
                    { 20, new FontSizeInfo(36, 13) }, { 22, new FontSizeInfo(38, 14) },
                    { 24, new FontSizeInfo(42, 16) }, { 26, new FontSizeInfo(46, 17) },
                    { 28, new FontSizeInfo(48, 18) }, { 36, new FontSizeInfo(62, 24) },
                    { 48, new FontSizeInfo(83, 31) }, { 72, new FontSizeInfo(0, 47) },
                    { 96, new FontSizeInfo(165, 63) }, { 128, new FontSizeInfo(220, 84) },
                    { 256, new FontSizeInfo(440, 167) }
                }
            },
            {
                "Chiller",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(21, 7) }, { 12, new FontSizeInfo(22, 8) }, { 14, new FontSizeInfo(26, 9) },
                    { 16, new FontSizeInfo(28, 10) }, { 18, new FontSizeInfo(32, 11) },
                    { 20, new FontSizeInfo(36, 13) }, { 22, new FontSizeInfo(39, 14) },
                    { 24, new FontSizeInfo(43, 15) }, { 26, new FontSizeInfo(47, 16) },
                    { 28, new FontSizeInfo(49, 17) }, { 36, new FontSizeInfo(64, 23) },
                    { 48, new FontSizeInfo(85, 30) }, { 72, new FontSizeInfo(0, 45) },
                    { 96, new FontSizeInfo(168, 60) }, { 128, new FontSizeInfo(224, 80) },
                    { 256, new FontSizeInfo(446, 160) }
                }
            },
            {
                "Colonna MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(21, 8) }, { 12, new FontSizeInfo(23, 8) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(29, 11) }, { 18, new FontSizeInfo(33, 12) },
                    { 20, new FontSizeInfo(37, 14) }, { 22, new FontSizeInfo(40, 15) },
                    { 24, new FontSizeInfo(44, 16) }, { 26, new FontSizeInfo(48, 18) },
                    { 28, new FontSizeInfo(51, 19) }, { 36, new FontSizeInfo(66, 24) },
                    { 48, new FontSizeInfo(87, 32) }, { 72, new FontSizeInfo(0, 48) },
                    { 96, new FontSizeInfo(173, 64) }, { 128, new FontSizeInfo(231, 86) },
                    { 256, new FontSizeInfo(459, 171) }
                }
            },
            {
                "Cooper Black",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 10) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(27, 13) }, { 18, new FontSizeInfo(30, 14) },
                    { 20, new FontSizeInfo(34, 16) }, { 22, new FontSizeInfo(37, 17) },
                    { 24, new FontSizeInfo(40, 19) }, { 26, new FontSizeInfo(44, 21) },
                    { 28, new FontSizeInfo(46, 22) }, { 36, new FontSizeInfo(60, 29) },
                    { 48, new FontSizeInfo(80, 38) }, { 72, new FontSizeInfo(0, 58) },
                    { 96, new FontSizeInfo(158, 77) }, { 128, new FontSizeInfo(211, 103) },
                    { 256, new FontSizeInfo(420, 205) }
                }
            },
            {
                "Copperplate Gothic Bold",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 8) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(20, 11) }, { 12, new FontSizeInfo(21, 11) },
                    { 14, new FontSizeInfo(24, 14) }, { 16, new FontSizeInfo(27, 15) },
                    { 18, new FontSizeInfo(30, 17) }, { 20, new FontSizeInfo(34, 19) },
                    { 22, new FontSizeInfo(37, 21) }, { 24, new FontSizeInfo(40, 23) },
                    { 26, new FontSizeInfo(44, 25) }, { 28, new FontSizeInfo(46, 26) },
                    { 36, new FontSizeInfo(60, 34) }, { 48, new FontSizeInfo(80, 46) }, { 72, new FontSizeInfo(0, 68) },
                    { 96, new FontSizeInfo(158, 91) }, { 128, new FontSizeInfo(211, 122) },
                    { 256, new FontSizeInfo(420, 243) }
                }
            },
            {
                "Copperplate Gothic Light",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 8) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(20, 11) }, { 12, new FontSizeInfo(21, 12) },
                    { 14, new FontSizeInfo(24, 14) }, { 16, new FontSizeInfo(26, 15) },
                    { 18, new FontSizeInfo(30, 17) }, { 20, new FontSizeInfo(34, 19) },
                    { 22, new FontSizeInfo(36, 21) }, { 24, new FontSizeInfo(40, 23) },
                    { 26, new FontSizeInfo(43, 25) }, { 28, new FontSizeInfo(46, 27) },
                    { 36, new FontSizeInfo(59, 35) }, { 48, new FontSizeInfo(78, 46) }, { 72, new FontSizeInfo(0, 69) },
                    { 96, new FontSizeInfo(157, 92) }, { 128, new FontSizeInfo(209, 123) },
                    { 256, new FontSizeInfo(418, 246) }
                }
            },
            {
                "Curlz MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(22, 9) }, { 14, new FontSizeInfo(28, 10) },
                    { 16, new FontSizeInfo(30, 11) }, { 18, new FontSizeInfo(34, 13) },
                    { 20, new FontSizeInfo(38, 15) }, { 22, new FontSizeInfo(41, 16) },
                    { 24, new FontSizeInfo(45, 17) }, { 26, new FontSizeInfo(48, 19) },
                    { 28, new FontSizeInfo(51, 20) }, { 36, new FontSizeInfo(65, 26) },
                    { 48, new FontSizeInfo(88, 34) }, { 72, new FontSizeInfo(0, 52) },
                    { 96, new FontSizeInfo(173, 69) }, { 128, new FontSizeInfo(233, 92) },
                    { 256, new FontSizeInfo(462, 183) }
                }
            },
            {
                "Elephant",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 8) }, { 10, new FontSizeInfo(20, 10) },
                    { 11, new FontSizeInfo(21, 11) }, { 12, new FontSizeInfo(22, 12) },
                    { 14, new FontSizeInfo(26, 14) }, { 16, new FontSizeInfo(28, 16) },
                    { 18, new FontSizeInfo(32, 18) }, { 20, new FontSizeInfo(36, 20) },
                    { 22, new FontSizeInfo(39, 22) }, { 24, new FontSizeInfo(43, 24) },
                    { 26, new FontSizeInfo(47, 27) }, { 28, new FontSizeInfo(49, 28) },
                    { 36, new FontSizeInfo(64, 36) }, { 48, new FontSizeInfo(85, 49) }, { 72, new FontSizeInfo(0, 73) },
                    { 96, new FontSizeInfo(168, 97) }, { 128, new FontSizeInfo(224, 130) },
                    { 256, new FontSizeInfo(446, 258) }
                }
            },
            {
                "Engravers MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 10) }, { 12, new FontSizeInfo(21, 10) },
                    { 14, new FontSizeInfo(24, 12) }, { 16, new FontSizeInfo(27, 14) },
                    { 18, new FontSizeInfo(30, 16) }, { 20, new FontSizeInfo(34, 17) },
                    { 22, new FontSizeInfo(37, 19) }, { 24, new FontSizeInfo(40, 21) },
                    { 26, new FontSizeInfo(44, 23) }, { 28, new FontSizeInfo(46, 24) },
                    { 36, new FontSizeInfo(60, 31) }, { 48, new FontSizeInfo(80, 41) }, { 72, new FontSizeInfo(0, 62) },
                    { 96, new FontSizeInfo(158, 83) }, { 128, new FontSizeInfo(211, 110) },
                    { 256, new FontSizeInfo(419, 219) }
                }
            },
            {
                "Eras Bold ITC",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 10) }, { 12, new FontSizeInfo(21, 10) },
                    { 14, new FontSizeInfo(25, 12) }, { 16, new FontSizeInfo(27, 14) },
                    { 18, new FontSizeInfo(31, 16) }, { 20, new FontSizeInfo(35, 18) },
                    { 22, new FontSizeInfo(37, 19) }, { 24, new FontSizeInfo(41, 21) },
                    { 26, new FontSizeInfo(45, 23) }, { 28, new FontSizeInfo(47, 24) },
                    { 36, new FontSizeInfo(61, 31) }, { 48, new FontSizeInfo(81, 42) }, { 72, new FontSizeInfo(0, 63) },
                    { 96, new FontSizeInfo(161, 83) }, { 128, new FontSizeInfo(215, 111) },
                    { 256, new FontSizeInfo(425, 222) }
                }
            },
            {
                "Eras Demi ITC",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 10) }, { 14, new FontSizeInfo(25, 12) },
                    { 16, new FontSizeInfo(27, 13) }, { 18, new FontSizeInfo(31, 15) },
                    { 20, new FontSizeInfo(35, 16) }, { 22, new FontSizeInfo(37, 18) },
                    { 24, new FontSizeInfo(41, 20) }, { 26, new FontSizeInfo(45, 21) },
                    { 28, new FontSizeInfo(47, 23) }, { 36, new FontSizeInfo(61, 29) },
                    { 48, new FontSizeInfo(81, 39) }, { 72, new FontSizeInfo(0, 59) },
                    { 96, new FontSizeInfo(161, 78) }, { 128, new FontSizeInfo(215, 104) },
                    { 256, new FontSizeInfo(425, 208) }
                }
            },
            {
                "Eras Light ITC",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(22, 9) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(35, 14) }, { 22, new FontSizeInfo(37, 16) },
                    { 24, new FontSizeInfo(41, 17) }, { 26, new FontSizeInfo(45, 19) },
                    { 28, new FontSizeInfo(47, 20) }, { 36, new FontSizeInfo(61, 26) },
                    { 48, new FontSizeInfo(81, 34) }, { 72, new FontSizeInfo(0, 51) },
                    { 96, new FontSizeInfo(161, 69) }, { 128, new FontSizeInfo(215, 92) },
                    { 256, new FontSizeInfo(427, 183) }
                }
            },
            {
                "Eras Medium ITC",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(31, 14) },
                    { 20, new FontSizeInfo(35, 15) }, { 22, new FontSizeInfo(37, 17) },
                    { 24, new FontSizeInfo(41, 18) }, { 26, new FontSizeInfo(45, 20) },
                    { 28, new FontSizeInfo(47, 21) }, { 36, new FontSizeInfo(61, 27) },
                    { 48, new FontSizeInfo(81, 37) }, { 72, new FontSizeInfo(0, 55) },
                    { 96, new FontSizeInfo(161, 73) }, { 128, new FontSizeInfo(215, 98) },
                    { 256, new FontSizeInfo(426, 195) }
                }
            },
            {
                "Felix Titling",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(31, 14) },
                    { 20, new FontSizeInfo(35, 16) }, { 22, new FontSizeInfo(37, 17) },
                    { 24, new FontSizeInfo(41, 19) }, { 26, new FontSizeInfo(44, 20) },
                    { 28, new FontSizeInfo(47, 22) }, { 36, new FontSizeInfo(61, 28) },
                    { 48, new FontSizeInfo(80, 37) }, { 72, new FontSizeInfo(0, 56) },
                    { 96, new FontSizeInfo(160, 75) }, { 128, new FontSizeInfo(213, 100) },
                    { 256, new FontSizeInfo(424, 198) }
                }
            },
            {
                "Forte",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(21, 8) }, { 12, new FontSizeInfo(22, 8) }, { 14, new FontSizeInfo(28, 10) },
                    { 16, new FontSizeInfo(30, 11) }, { 18, new FontSizeInfo(34, 13) },
                    { 20, new FontSizeInfo(40, 14) }, { 22, new FontSizeInfo(42, 15) },
                    { 24, new FontSizeInfo(46, 17) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(71, 25) },
                    { 48, new FontSizeInfo(95, 34) }, { 72, new FontSizeInfo(0, 51) },
                    { 96, new FontSizeInfo(188, 68) }, { 128, new FontSizeInfo(253, 91) },
                    { 256, new FontSizeInfo(503, 181) }
                }
            },
            {
                "Franklin Gothic Book",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(21, 9) }, { 12, new FontSizeInfo(22, 9) }, { 14, new FontSizeInfo(26, 11) },
                    { 16, new FontSizeInfo(28, 12) }, { 18, new FontSizeInfo(32, 14) },
                    { 20, new FontSizeInfo(36, 16) }, { 22, new FontSizeInfo(39, 17) },
                    { 24, new FontSizeInfo(40, 19) }, { 26, new FontSizeInfo(44, 21) },
                    { 28, new FontSizeInfo(46, 22) }, { 36, new FontSizeInfo(64, 28) },
                    { 48, new FontSizeInfo(85, 38) }, { 72, new FontSizeInfo(0, 56) },
                    { 96, new FontSizeInfo(168, 75) }, { 128, new FontSizeInfo(224, 100) },
                    { 256, new FontSizeInfo(416, 200) }
                }
            },
            {
                "Franklin Gothic Demi",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(21, 9) }, { 12, new FontSizeInfo(22, 9) }, { 14, new FontSizeInfo(26, 11) },
                    { 16, new FontSizeInfo(28, 12) }, { 18, new FontSizeInfo(32, 14) },
                    { 20, new FontSizeInfo(36, 16) }, { 22, new FontSizeInfo(39, 17) },
                    { 24, new FontSizeInfo(40, 19) }, { 26, new FontSizeInfo(44, 21) },
                    { 28, new FontSizeInfo(46, 22) }, { 36, new FontSizeInfo(64, 28) },
                    { 48, new FontSizeInfo(85, 38) }, { 72, new FontSizeInfo(0, 56) },
                    { 96, new FontSizeInfo(168, 75) }, { 128, new FontSizeInfo(224, 100) },
                    { 256, new FontSizeInfo(416, 200) }
                }
            },
            {
                "Franklin Gothic Demi Cond",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(21, 8) }, { 12, new FontSizeInfo(22, 8) }, { 14, new FontSizeInfo(26, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(32, 12) },
                    { 20, new FontSizeInfo(36, 14) }, { 22, new FontSizeInfo(39, 15) },
                    { 24, new FontSizeInfo(40, 16) }, { 26, new FontSizeInfo(44, 18) },
                    { 28, new FontSizeInfo(46, 19) }, { 36, new FontSizeInfo(64, 25) },
                    { 48, new FontSizeInfo(85, 33) }, { 72, new FontSizeInfo(0, 49) },
                    { 96, new FontSizeInfo(168, 65) }, { 128, new FontSizeInfo(224, 87) },
                    { 256, new FontSizeInfo(416, 174) }
                }
            },
            {
                "Franklin Gothic Heavy",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(21, 9) }, { 12, new FontSizeInfo(22, 9) }, { 14, new FontSizeInfo(26, 11) },
                    { 16, new FontSizeInfo(28, 12) }, { 18, new FontSizeInfo(32, 14) },
                    { 20, new FontSizeInfo(36, 16) }, { 22, new FontSizeInfo(39, 17) },
                    { 24, new FontSizeInfo(40, 19) }, { 26, new FontSizeInfo(44, 21) },
                    { 28, new FontSizeInfo(46, 22) }, { 36, new FontSizeInfo(64, 28) },
                    { 48, new FontSizeInfo(85, 38) }, { 72, new FontSizeInfo(0, 56) },
                    { 96, new FontSizeInfo(168, 75) }, { 128, new FontSizeInfo(224, 100) },
                    { 256, new FontSizeInfo(416, 200) }
                }
            },
            {
                "Franklin Gothic Medium Cond",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(21, 8) }, { 12, new FontSizeInfo(22, 8) }, { 14, new FontSizeInfo(26, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(32, 12) },
                    { 20, new FontSizeInfo(36, 14) }, { 22, new FontSizeInfo(39, 15) },
                    { 24, new FontSizeInfo(40, 16) }, { 26, new FontSizeInfo(44, 18) },
                    { 28, new FontSizeInfo(46, 19) }, { 36, new FontSizeInfo(64, 25) },
                    { 48, new FontSizeInfo(85, 33) }, { 72, new FontSizeInfo(0, 49) },
                    { 96, new FontSizeInfo(168, 65) }, { 128, new FontSizeInfo(224, 87) },
                    { 256, new FontSizeInfo(416, 174) }
                }
            },
            {
                "Freestyle Script",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(21, 6) }, { 12, new FontSizeInfo(23, 6) }, { 14, new FontSizeInfo(26, 7) },
                    { 16, new FontSizeInfo(29, 8) }, { 18, new FontSizeInfo(33, 9) }, { 20, new FontSizeInfo(37, 10) },
                    { 22, new FontSizeInfo(40, 10) }, { 24, new FontSizeInfo(43, 12) },
                    { 26, new FontSizeInfo(48, 13) }, { 28, new FontSizeInfo(50, 13) },
                    { 36, new FontSizeInfo(65, 17) }, { 48, new FontSizeInfo(86, 23) }, { 72, new FontSizeInfo(0, 33) },
                    { 96, new FontSizeInfo(172, 44) }, { 128, new FontSizeInfo(228, 58) },
                    { 256, new FontSizeInfo(454, 117) }
                }
            },
            {
                "French Script MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(21, 5) }, { 12, new FontSizeInfo(22, 6) }, { 14, new FontSizeInfo(26, 7) },
                    { 16, new FontSizeInfo(28, 8) }, { 18, new FontSizeInfo(32, 9) }, { 20, new FontSizeInfo(36, 10) },
                    { 22, new FontSizeInfo(39, 11) }, { 24, new FontSizeInfo(43, 12) },
                    { 26, new FontSizeInfo(47, 13) }, { 28, new FontSizeInfo(49, 14) },
                    { 36, new FontSizeInfo(64, 18) }, { 48, new FontSizeInfo(85, 23) }, { 72, new FontSizeInfo(0, 35) },
                    { 96, new FontSizeInfo(168, 47) }, { 128, new FontSizeInfo(224, 62) },
                    { 256, new FontSizeInfo(446, 125) }
                }
            },
            {
                "Footlight MT Light",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(30, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(37, 16) },
                    { 24, new FontSizeInfo(40, 18) }, { 26, new FontSizeInfo(44, 19) },
                    { 28, new FontSizeInfo(46, 20) }, { 36, new FontSizeInfo(60, 27) },
                    { 48, new FontSizeInfo(79, 35) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(158, 71) }, { 128, new FontSizeInfo(210, 94) },
                    { 256, new FontSizeInfo(419, 188) }
                }
            },
            {
                "Garamond",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(20, 7) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 9) },
                    { 16, new FontSizeInfo(28, 10) }, { 18, new FontSizeInfo(31, 11) },
                    { 20, new FontSizeInfo(35, 13) }, { 22, new FontSizeInfo(38, 14) },
                    { 24, new FontSizeInfo(41, 15) }, { 26, new FontSizeInfo(45, 16) },
                    { 28, new FontSizeInfo(48, 17) }, { 36, new FontSizeInfo(62, 23) },
                    { 48, new FontSizeInfo(82, 30) }, { 72, new FontSizeInfo(0, 45) },
                    { 96, new FontSizeInfo(163, 60) }, { 128, new FontSizeInfo(217, 80) },
                    { 256, new FontSizeInfo(432, 160) }
                }
            },
            {
                "Gigi",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 5) }, { 10, new FontSizeInfo(22, 7) },
                    { 11, new FontSizeInfo(22, 7) }, { 12, new FontSizeInfo(24, 7) }, { 14, new FontSizeInfo(28, 10) },
                    { 16, new FontSizeInfo(31, 12) }, { 18, new FontSizeInfo(35, 12) },
                    { 20, new FontSizeInfo(39, 14) }, { 22, new FontSizeInfo(42, 14) },
                    { 24, new FontSizeInfo(46, 17) }, { 26, new FontSizeInfo(50, 19) },
                    { 28, new FontSizeInfo(53, 19) }, { 36, new FontSizeInfo(69, 24) },
                    { 48, new FontSizeInfo(91, 33) }, { 72, new FontSizeInfo(0, 50) },
                    { 96, new FontSizeInfo(182, 66) }, { 128, new FontSizeInfo(242, 88) },
                    { 256, new FontSizeInfo(482, 175) }
                }
            },
            {
                "Gill Sans MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(21, 7) },
                    { 11, new FontSizeInfo(23, 8) }, { 12, new FontSizeInfo(26, 8) }, { 14, new FontSizeInfo(29, 10) },
                    { 16, new FontSizeInfo(33, 11) }, { 18, new FontSizeInfo(37, 12) },
                    { 20, new FontSizeInfo(41, 14) }, { 22, new FontSizeInfo(43, 15) },
                    { 24, new FontSizeInfo(48, 16) }, { 26, new FontSizeInfo(51, 18) },
                    { 28, new FontSizeInfo(56, 19) }, { 36, new FontSizeInfo(71, 24) },
                    { 48, new FontSizeInfo(91, 32) }, { 72, new FontSizeInfo(0, 48) },
                    { 96, new FontSizeInfo(184, 64) }, { 128, new FontSizeInfo(243, 86) },
                    { 256, new FontSizeInfo(421, 171) }
                }
            },
            {
                "Gill Sans MT Condensed",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(21, 5) },
                    { 11, new FontSizeInfo(21, 5) }, { 12, new FontSizeInfo(22, 6) }, { 14, new FontSizeInfo(27, 7) },
                    { 16, new FontSizeInfo(29, 8) }, { 18, new FontSizeInfo(32, 9) }, { 20, new FontSizeInfo(37, 10) },
                    { 22, new FontSizeInfo(39, 11) }, { 24, new FontSizeInfo(43, 12) },
                    { 26, new FontSizeInfo(47, 13) }, { 28, new FontSizeInfo(50, 14) },
                    { 36, new FontSizeInfo(64, 18) }, { 48, new FontSizeInfo(86, 23) }, { 72, new FontSizeInfo(0, 35) },
                    { 96, new FontSizeInfo(169, 47) }, { 128, new FontSizeInfo(226, 62) },
                    { 256, new FontSizeInfo(434, 125) }
                }
            },
            {
                "Gill Sans Ultra Bold Condensed",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(22, 8) },
                    { 11, new FontSizeInfo(21, 9) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 11) },
                    { 16, new FontSizeInfo(30, 12) }, { 18, new FontSizeInfo(33, 14) },
                    { 20, new FontSizeInfo(38, 16) }, { 22, new FontSizeInfo(41, 17) },
                    { 24, new FontSizeInfo(45, 19) }, { 26, new FontSizeInfo(49, 20) },
                    { 28, new FontSizeInfo(51, 22) }, { 36, new FontSizeInfo(69, 28) },
                    { 48, new FontSizeInfo(89, 37) }, { 72, new FontSizeInfo(0, 56) },
                    { 96, new FontSizeInfo(176, 74) }, { 128, new FontSizeInfo(235, 99) },
                    { 256, new FontSizeInfo(428, 198) }
                }
            },
            {
                "Gill Sans Ultra Bold",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(23, 9) }, { 10, new FontSizeInfo(22, 11) },
                    { 11, new FontSizeInfo(21, 13) }, { 12, new FontSizeInfo(24, 14) },
                    { 14, new FontSizeInfo(27, 16) }, { 16, new FontSizeInfo(31, 18) },
                    { 18, new FontSizeInfo(35, 21) }, { 20, new FontSizeInfo(41, 23) },
                    { 22, new FontSizeInfo(44, 25) }, { 24, new FontSizeInfo(45, 27) },
                    { 26, new FontSizeInfo(50, 30) }, { 28, new FontSizeInfo(53, 32) },
                    { 36, new FontSizeInfo(71, 41) }, { 48, new FontSizeInfo(93, 55) }, { 72, new FontSizeInfo(0, 82) },
                    { 96, new FontSizeInfo(181, 109) }, { 128, new FontSizeInfo(241, 146) },
                    { 256, new FontSizeInfo(428, 291) }
                }
            },
            {
                "Gloucester MT Extra Condensed",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(20, 6) }, { 12, new FontSizeInfo(21, 6) }, { 14, new FontSizeInfo(24, 7) },
                    { 16, new FontSizeInfo(27, 8) }, { 18, new FontSizeInfo(30, 9) }, { 20, new FontSizeInfo(34, 10) },
                    { 22, new FontSizeInfo(36, 11) }, { 24, new FontSizeInfo(40, 12) },
                    { 26, new FontSizeInfo(44, 14) }, { 28, new FontSizeInfo(46, 14) },
                    { 36, new FontSizeInfo(59, 19) }, { 48, new FontSizeInfo(79, 25) }, { 72, new FontSizeInfo(0, 37) },
                    { 96, new FontSizeInfo(157, 49) }, { 128, new FontSizeInfo(209, 66) },
                    { 256, new FontSizeInfo(416, 132) }
                }
            },
            {
                "Gill Sans MT Ext Condensed Bold",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(20, 4) },
                    { 11, new FontSizeInfo(20, 5) }, { 12, new FontSizeInfo(23, 6) }, { 14, new FontSizeInfo(25, 7) },
                    { 16, new FontSizeInfo(27, 7) }, { 18, new FontSizeInfo(33, 8) }, { 20, new FontSizeInfo(35, 9) },
                    { 22, new FontSizeInfo(37, 10) }, { 24, new FontSizeInfo(41, 11) },
                    { 26, new FontSizeInfo(45, 12) }, { 28, new FontSizeInfo(47, 13) },
                    { 36, new FontSizeInfo(61, 17) }, { 48, new FontSizeInfo(81, 22) }, { 72, new FontSizeInfo(0, 33) },
                    { 96, new FontSizeInfo(161, 44) }, { 128, new FontSizeInfo(214, 59) },
                    { 256, new FontSizeInfo(410, 117) }
                }
            },
            {
                "Century Gothic",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(26, 12) }, { 18, new FontSizeInfo(32, 13) },
                    { 20, new FontSizeInfo(35, 15) }, { 22, new FontSizeInfo(38, 16) },
                    { 24, new FontSizeInfo(41, 18) }, { 26, new FontSizeInfo(44, 19) },
                    { 28, new FontSizeInfo(46, 21) }, { 36, new FontSizeInfo(61, 27) },
                    { 48, new FontSizeInfo(82, 35) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(158, 71) }, { 128, new FontSizeInfo(214, 95) },
                    { 256, new FontSizeInfo(427, 189) }
                }
            },
            {
                "Goudy Old Style",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(21, 8) }, { 12, new FontSizeInfo(22, 8) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(28, 11) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(35, 14) }, { 22, new FontSizeInfo(40, 15) },
                    { 24, new FontSizeInfo(43, 16) }, { 26, new FontSizeInfo(47, 18) },
                    { 28, new FontSizeInfo(50, 19) }, { 36, new FontSizeInfo(63, 24) },
                    { 48, new FontSizeInfo(86, 32) }, { 72, new FontSizeInfo(0, 48) },
                    { 96, new FontSizeInfo(173, 64) }, { 128, new FontSizeInfo(231, 86) },
                    { 256, new FontSizeInfo(451, 171) }
                }
            },
            {
                "Goudy Stout",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 9) }, { 10, new FontSizeInfo(21, 11) },
                    { 11, new FontSizeInfo(21, 12) }, { 12, new FontSizeInfo(23, 13) },
                    { 14, new FontSizeInfo(27, 16) }, { 16, new FontSizeInfo(29, 17) },
                    { 18, new FontSizeInfo(33, 20) }, { 20, new FontSizeInfo(39, 22) },
                    { 22, new FontSizeInfo(42, 24) }, { 24, new FontSizeInfo(46, 26) },
                    { 26, new FontSizeInfo(50, 29) }, { 28, new FontSizeInfo(53, 31) },
                    { 36, new FontSizeInfo(68, 40) }, { 48, new FontSizeInfo(89, 53) }, { 72, new FontSizeInfo(0, 79) },
                    { 96, new FontSizeInfo(178, 106) }, { 128, new FontSizeInfo(238, 141) },
                    { 256, new FontSizeInfo(473, 280) }
                }
            },
            {
                "Harlow Solid Italic",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(21, 6) },
                    { 11, new FontSizeInfo(22, 7) }, { 12, new FontSizeInfo(23, 7) }, { 14, new FontSizeInfo(27, 9) },
                    { 16, new FontSizeInfo(30, 10) }, { 18, new FontSizeInfo(34, 11) },
                    { 20, new FontSizeInfo(38, 13) }, { 22, new FontSizeInfo(41, 14) },
                    { 24, new FontSizeInfo(45, 15) }, { 26, new FontSizeInfo(50, 16) },
                    { 28, new FontSizeInfo(52, 17) }, { 36, new FontSizeInfo(68, 22) },
                    { 48, new FontSizeInfo(90, 30) }, { 72, new FontSizeInfo(0, 45) },
                    { 96, new FontSizeInfo(179, 60) }, { 128, new FontSizeInfo(238, 80) },
                    { 256, new FontSizeInfo(474, 159) }
                }
            },
            {
                "Harrington",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(37, 16) },
                    { 24, new FontSizeInfo(40, 18) }, { 26, new FontSizeInfo(44, 19) },
                    { 28, new FontSizeInfo(47, 21) }, { 36, new FontSizeInfo(60, 27) },
                    { 48, new FontSizeInfo(80, 36) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(159, 71) }, { 128, new FontSizeInfo(211, 95) },
                    { 256, new FontSizeInfo(421, 189) }
                }
            },
            {
                "Haettenschweiler",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(20, 7) }, { 12, new FontSizeInfo(21, 7) }, { 14, new FontSizeInfo(24, 8) },
                    { 16, new FontSizeInfo(26, 9) }, { 18, new FontSizeInfo(29, 11) }, { 20, new FontSizeInfo(32, 12) },
                    { 22, new FontSizeInfo(34, 13) }, { 24, new FontSizeInfo(37, 14) },
                    { 26, new FontSizeInfo(40, 15) }, { 28, new FontSizeInfo(43, 16) },
                    { 36, new FontSizeInfo(55, 21) }, { 48, new FontSizeInfo(73, 28) }, { 72, new FontSizeInfo(0, 42) },
                    { 96, new FontSizeInfo(148, 56) }, { 128, new FontSizeInfo(198, 75) },
                    { 256, new FontSizeInfo(391, 150) }
                }
            },
            {
                "High Tower Text",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(20, 7) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 9) },
                    { 16, new FontSizeInfo(27, 10) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(35, 13) }, { 22, new FontSizeInfo(37, 14) },
                    { 24, new FontSizeInfo(41, 15) }, { 26, new FontSizeInfo(45, 17) },
                    { 28, new FontSizeInfo(48, 18) }, { 36, new FontSizeInfo(62, 23) },
                    { 48, new FontSizeInfo(82, 31) }, { 72, new FontSizeInfo(0, 46) },
                    { 96, new FontSizeInfo(163, 62) }, { 128, new FontSizeInfo(217, 82) },
                    { 256, new FontSizeInfo(432, 164) }
                }
            },
            {
                "Imprint MT Shadow",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(27, 11) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(35, 14) }, { 22, new FontSizeInfo(37, 15) },
                    { 24, new FontSizeInfo(41, 16) }, { 26, new FontSizeInfo(45, 18) },
                    { 28, new FontSizeInfo(47, 19) }, { 36, new FontSizeInfo(61, 24) },
                    { 48, new FontSizeInfo(81, 33) }, { 72, new FontSizeInfo(0, 49) },
                    { 96, new FontSizeInfo(161, 65) }, { 128, new FontSizeInfo(215, 87) },
                    { 256, new FontSizeInfo(427, 174) }
                }
            },
            {
                "Informal Roman",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(29, 11) }, { 18, new FontSizeInfo(33, 13) },
                    { 20, new FontSizeInfo(39, 15) }, { 22, new FontSizeInfo(42, 16) },
                    { 24, new FontSizeInfo(45, 17) }, { 26, new FontSizeInfo(49, 19) },
                    { 28, new FontSizeInfo(53, 20) }, { 36, new FontSizeInfo(69, 25) },
                    { 48, new FontSizeInfo(91, 34) }, { 72, new FontSizeInfo(0, 51) },
                    { 96, new FontSizeInfo(183, 68) }, { 128, new FontSizeInfo(245, 91) },
                    { 256, new FontSizeInfo(479, 181) }
                }
            },
            {
                "Blackadder ITC",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 4) }, { 10, new FontSizeInfo(21, 5) },
                    { 11, new FontSizeInfo(22, 6) }, { 12, new FontSizeInfo(23, 6) }, { 14, new FontSizeInfo(28, 8) },
                    { 16, new FontSizeInfo(30, 8) }, { 18, new FontSizeInfo(34, 9) }, { 20, new FontSizeInfo(38, 11) },
                    { 22, new FontSizeInfo(41, 11) }, { 24, new FontSizeInfo(45, 13) },
                    { 26, new FontSizeInfo(50, 14) }, { 28, new FontSizeInfo(52, 15) },
                    { 36, new FontSizeInfo(68, 19) }, { 48, new FontSizeInfo(90, 25) }, { 72, new FontSizeInfo(0, 38) },
                    { 96, new FontSizeInfo(179, 51) }, { 128, new FontSizeInfo(238, 67) },
                    { 256, new FontSizeInfo(474, 135) }
                }
            },
            {
                "Edwardian Script ITC",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(21, 7) }, { 12, new FontSizeInfo(22, 8) }, { 14, new FontSizeInfo(26, 9) },
                    { 16, new FontSizeInfo(29, 10) }, { 18, new FontSizeInfo(33, 11) },
                    { 20, new FontSizeInfo(37, 13) }, { 22, new FontSizeInfo(40, 14) },
                    { 24, new FontSizeInfo(44, 15) }, { 26, new FontSizeInfo(47, 17) },
                    { 28, new FontSizeInfo(50, 18) }, { 36, new FontSizeInfo(65, 23) },
                    { 48, new FontSizeInfo(86, 31) }, { 72, new FontSizeInfo(0, 46) },
                    { 96, new FontSizeInfo(171, 61) }, { 128, new FontSizeInfo(228, 82) },
                    { 256, new FontSizeInfo(454, 163) }
                }
            },
            {
                "Kristen ITC",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 7) }, { 10, new FontSizeInfo(21, 8) },
                    { 11, new FontSizeInfo(24, 9) }, { 12, new FontSizeInfo(25, 10) }, { 14, new FontSizeInfo(29, 11) },
                    { 16, new FontSizeInfo(31, 13) }, { 18, new FontSizeInfo(35, 14) },
                    { 20, new FontSizeInfo(37, 16) }, { 22, new FontSizeInfo(42, 17) },
                    { 24, new FontSizeInfo(46, 19) }, { 26, new FontSizeInfo(49, 21) },
                    { 28, new FontSizeInfo(53, 22) }, { 36, new FontSizeInfo(68, 29) },
                    { 48, new FontSizeInfo(91, 38) }, { 72, new FontSizeInfo(0, 58) },
                    { 96, new FontSizeInfo(178, 77) }, { 128, new FontSizeInfo(240, 103) },
                    { 256, new FontSizeInfo(473, 205) }
                }
            },
            {
                "Jokerman",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(22, 8) }, { 10, new FontSizeInfo(23, 9) },
                    { 11, new FontSizeInfo(25, 11) }, { 12, new FontSizeInfo(27, 11) },
                    { 14, new FontSizeInfo(31, 14) }, { 16, new FontSizeInfo(35, 15) },
                    { 18, new FontSizeInfo(39, 17) }, { 20, new FontSizeInfo(43, 19) },
                    { 22, new FontSizeInfo(48, 21) }, { 24, new FontSizeInfo(52, 23) },
                    { 26, new FontSizeInfo(56, 25) }, { 28, new FontSizeInfo(61, 26) },
                    { 36, new FontSizeInfo(76, 34) }, { 48, new FontSizeInfo(101, 46) },
                    { 72, new FontSizeInfo(0, 69) }, { 96, new FontSizeInfo(202, 91) },
                    { 128, new FontSizeInfo(270, 122) }, { 256, new FontSizeInfo(569, 244) }
                }
            },
            {
                "Juice ITC",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(20, 6) }, { 12, new FontSizeInfo(21, 6) }, { 14, new FontSizeInfo(24, 7) },
                    { 16, new FontSizeInfo(27, 8) }, { 18, new FontSizeInfo(30, 8) }, { 20, new FontSizeInfo(34, 10) },
                    { 22, new FontSizeInfo(36, 11) }, { 24, new FontSizeInfo(40, 11) },
                    { 26, new FontSizeInfo(44, 13) }, { 28, new FontSizeInfo(46, 14) },
                    { 36, new FontSizeInfo(62, 18) }, { 48, new FontSizeInfo(81, 24) }, { 72, new FontSizeInfo(0, 36) },
                    { 96, new FontSizeInfo(164, 48) }, { 128, new FontSizeInfo(218, 64) },
                    { 256, new FontSizeInfo(436, 128) }
                }
            },
            {
                "Kunstler Script",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(20, 5) }, { 12, new FontSizeInfo(21, 6) }, { 14, new FontSizeInfo(25, 7) },
                    { 16, new FontSizeInfo(28, 7) }, { 18, new FontSizeInfo(31, 8) }, { 20, new FontSizeInfo(35, 10) },
                    { 22, new FontSizeInfo(38, 10) }, { 24, new FontSizeInfo(42, 11) },
                    { 26, new FontSizeInfo(45, 12) }, { 28, new FontSizeInfo(48, 13) },
                    { 36, new FontSizeInfo(62, 17) }, { 48, new FontSizeInfo(82, 23) }, { 72, new FontSizeInfo(0, 34) },
                    { 96, new FontSizeInfo(163, 45) }, { 128, new FontSizeInfo(218, 60) },
                    { 256, new FontSizeInfo(437, 120) }
                }
            },
            {
                "Wide Latin",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 14) }, { 10, new FontSizeInfo(20, 16) },
                    { 11, new FontSizeInfo(20, 19) }, { 12, new FontSizeInfo(21, 20) },
                    { 14, new FontSizeInfo(25, 24) }, { 16, new FontSizeInfo(28, 26) },
                    { 18, new FontSizeInfo(31, 30) }, { 20, new FontSizeInfo(35, 33) },
                    { 22, new FontSizeInfo(38, 36) }, { 24, new FontSizeInfo(42, 40) },
                    { 26, new FontSizeInfo(45, 43) }, { 28, new FontSizeInfo(48, 46) },
                    { 36, new FontSizeInfo(62, 59) }, { 48, new FontSizeInfo(82, 79) },
                    { 72, new FontSizeInfo(0, 119) }, { 96, new FontSizeInfo(163, 158) },
                    { 128, new FontSizeInfo(218, 212) }, { 256, new FontSizeInfo(433, 422) }
                }
            },
            {
                "Lucida Bright",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 10) }, { 14, new FontSizeInfo(24, 12) },
                    { 16, new FontSizeInfo(26, 13) }, { 18, new FontSizeInfo(30, 15) },
                    { 20, new FontSizeInfo(34, 16) }, { 22, new FontSizeInfo(36, 18) },
                    { 24, new FontSizeInfo(39, 19) }, { 26, new FontSizeInfo(43, 21) },
                    { 28, new FontSizeInfo(45, 23) }, { 36, new FontSizeInfo(59, 29) },
                    { 48, new FontSizeInfo(80, 39) }, { 72, new FontSizeInfo(0, 58) },
                    { 96, new FontSizeInfo(159, 78) }, { 128, new FontSizeInfo(213, 104) },
                    { 256, new FontSizeInfo(426, 207) }
                }
            },
            {
                "Lucida Calligraphy",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 8) }, { 10, new FontSizeInfo(20, 10) },
                    { 11, new FontSizeInfo(23, 11) }, { 12, new FontSizeInfo(24, 12) },
                    { 14, new FontSizeInfo(26, 14) }, { 16, new FontSizeInfo(29, 15) },
                    { 18, new FontSizeInfo(35, 17) }, { 20, new FontSizeInfo(39, 19) },
                    { 22, new FontSizeInfo(41, 20) }, { 24, new FontSizeInfo(45, 22) },
                    { 26, new FontSizeInfo(49, 24) }, { 28, new FontSizeInfo(52, 26) },
                    { 36, new FontSizeInfo(69, 33) }, { 48, new FontSizeInfo(92, 44) }, { 72, new FontSizeInfo(0, 65) },
                    { 96, new FontSizeInfo(183, 86) }, { 128, new FontSizeInfo(246, 115) },
                    { 256, new FontSizeInfo(489, 227) }
                }
            },
            {
                "Leelawadee",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(31, 14) },
                    { 20, new FontSizeInfo(34, 16) }, { 22, new FontSizeInfo(37, 17) },
                    { 24, new FontSizeInfo(41, 18) }, { 26, new FontSizeInfo(44, 20) },
                    { 28, new FontSizeInfo(47, 21) }, { 36, new FontSizeInfo(60, 28) },
                    { 48, new FontSizeInfo(80, 37) }, { 72, new FontSizeInfo(0, 55) },
                    { 96, new FontSizeInfo(160, 74) }, { 128, new FontSizeInfo(213, 98) },
                    { 256, new FontSizeInfo(423, 196) }
                }
            },
            {
                "Lucida Fax",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(20, 10) }, { 12, new FontSizeInfo(21, 11) },
                    { 14, new FontSizeInfo(24, 13) }, { 16, new FontSizeInfo(27, 14) },
                    { 18, new FontSizeInfo(30, 16) }, { 20, new FontSizeInfo(34, 18) },
                    { 22, new FontSizeInfo(36, 19) }, { 24, new FontSizeInfo(40, 21) },
                    { 26, new FontSizeInfo(44, 23) }, { 28, new FontSizeInfo(46, 25) },
                    { 36, new FontSizeInfo(59, 32) }, { 48, new FontSizeInfo(78, 42) }, { 72, new FontSizeInfo(0, 64) },
                    { 96, new FontSizeInfo(157, 85) }, { 128, new FontSizeInfo(211, 113) },
                    { 256, new FontSizeInfo(422, 226) }
                }
            },
            {
                "Lucida Handwriting",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 8) }, { 10, new FontSizeInfo(20, 10) },
                    { 11, new FontSizeInfo(21, 11) }, { 12, new FontSizeInfo(24, 12) },
                    { 14, new FontSizeInfo(26, 14) }, { 16, new FontSizeInfo(29, 15) },
                    { 18, new FontSizeInfo(33, 17) }, { 20, new FontSizeInfo(39, 19) },
                    { 22, new FontSizeInfo(41, 21) }, { 24, new FontSizeInfo(45, 23) },
                    { 26, new FontSizeInfo(49, 25) }, { 28, new FontSizeInfo(52, 26) },
                    { 36, new FontSizeInfo(69, 33) }, { 48, new FontSizeInfo(90, 44) }, { 72, new FontSizeInfo(0, 66) },
                    { 96, new FontSizeInfo(181, 87) }, { 128, new FontSizeInfo(242, 116) },
                    { 256, new FontSizeInfo(483, 231) }
                }
            },
            {
                "Lucida Sans",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(20, 10) }, { 12, new FontSizeInfo(21, 11) },
                    { 14, new FontSizeInfo(24, 13) }, { 16, new FontSizeInfo(27, 14) },
                    { 18, new FontSizeInfo(30, 16) }, { 20, new FontSizeInfo(34, 18) },
                    { 22, new FontSizeInfo(36, 19) }, { 24, new FontSizeInfo(40, 21) },
                    { 26, new FontSizeInfo(43, 23) }, { 28, new FontSizeInfo(46, 24) },
                    { 36, new FontSizeInfo(59, 32) }, { 48, new FontSizeInfo(80, 42) }, { 72, new FontSizeInfo(0, 63) },
                    { 96, new FontSizeInfo(159, 84) }, { 128, new FontSizeInfo(213, 113) },
                    { 256, new FontSizeInfo(426, 225) }
                }
            },
            {
                "Lucida Sans Typewriter",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 10) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(26, 13) }, { 18, new FontSizeInfo(30, 14) },
                    { 20, new FontSizeInfo(34, 16) }, { 22, new FontSizeInfo(36, 17) },
                    { 24, new FontSizeInfo(40, 19) }, { 26, new FontSizeInfo(43, 21) },
                    { 28, new FontSizeInfo(46, 22) }, { 36, new FontSizeInfo(59, 29) },
                    { 48, new FontSizeInfo(80, 39) }, { 72, new FontSizeInfo(0, 58) },
                    { 96, new FontSizeInfo(159, 77) }, { 128, new FontSizeInfo(213, 103) },
                    { 256, new FontSizeInfo(424, 205) }
                }
            },
            {
                "Magneto",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 8) }, { 10, new FontSizeInfo(20, 10) },
                    { 11, new FontSizeInfo(21, 12) }, { 12, new FontSizeInfo(22, 12) },
                    { 14, new FontSizeInfo(26, 15) }, { 16, new FontSizeInfo(28, 16) },
                    { 18, new FontSizeInfo(32, 19) }, { 20, new FontSizeInfo(36, 21) },
                    { 22, new FontSizeInfo(39, 22) }, { 24, new FontSizeInfo(43, 25) },
                    { 26, new FontSizeInfo(47, 27) }, { 28, new FontSizeInfo(49, 29) },
                    { 36, new FontSizeInfo(64, 37) }, { 48, new FontSizeInfo(84, 49) }, { 72, new FontSizeInfo(0, 74) },
                    { 96, new FontSizeInfo(168, 99) }, { 128, new FontSizeInfo(224, 132) },
                    { 256, new FontSizeInfo(445, 263) }
                }
            },
            {
                "Maiandra GD",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(20, 10) }, { 12, new FontSizeInfo(21, 10) },
                    { 14, new FontSizeInfo(24, 12) }, { 16, new FontSizeInfo(27, 13) },
                    { 18, new FontSizeInfo(31, 15) }, { 20, new FontSizeInfo(34, 17) },
                    { 22, new FontSizeInfo(37, 18) }, { 24, new FontSizeInfo(41, 20) },
                    { 26, new FontSizeInfo(44, 22) }, { 28, new FontSizeInfo(47, 23) },
                    { 36, new FontSizeInfo(60, 29) }, { 48, new FontSizeInfo(80, 39) }, { 72, new FontSizeInfo(0, 57) },
                    { 96, new FontSizeInfo(159, 76) }, { 128, new FontSizeInfo(212, 102) },
                    { 256, new FontSizeInfo(422, 200) }
                }
            },
            {
                "Matura MT Script Capitals",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(21, 8) }, { 10, new FontSizeInfo(22, 10) },
                    { 11, new FontSizeInfo(23, 11) }, { 12, new FontSizeInfo(25, 12) },
                    { 14, new FontSizeInfo(29, 13) }, { 16, new FontSizeInfo(32, 15) },
                    { 18, new FontSizeInfo(36, 17) }, { 20, new FontSizeInfo(41, 19) },
                    { 22, new FontSizeInfo(44, 20) }, { 24, new FontSizeInfo(48, 22) },
                    { 26, new FontSizeInfo(53, 24) }, { 28, new FontSizeInfo(56, 25) },
                    { 36, new FontSizeInfo(72, 33) }, { 48, new FontSizeInfo(95, 42) }, { 72, new FontSizeInfo(0, 63) },
                    { 96, new FontSizeInfo(190, 84) }, { 128, new FontSizeInfo(253, 111) },
                    { 256, new FontSizeInfo(504, 221) }
                }
            },
            {
                "Mistral",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(21, 8) }, { 12, new FontSizeInfo(22, 8) }, { 14, new FontSizeInfo(26, 10) },
                    { 16, new FontSizeInfo(29, 10) }, { 18, new FontSizeInfo(33, 12) },
                    { 20, new FontSizeInfo(37, 13) }, { 22, new FontSizeInfo(39, 14) },
                    { 24, new FontSizeInfo(43, 15) }, { 26, new FontSizeInfo(47, 17) },
                    { 28, new FontSizeInfo(50, 18) }, { 36, new FontSizeInfo(64, 23) },
                    { 48, new FontSizeInfo(85, 30) }, { 72, new FontSizeInfo(0, 44) },
                    { 96, new FontSizeInfo(169, 59) }, { 128, new FontSizeInfo(226, 78) },
                    { 256, new FontSizeInfo(450, 154) }
                }
            },
            {
                "Modern No. 20",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(24, 10) },
                    { 16, new FontSizeInfo(27, 11) }, { 18, new FontSizeInfo(30, 12) },
                    { 20, new FontSizeInfo(34, 13) }, { 22, new FontSizeInfo(36, 14) },
                    { 24, new FontSizeInfo(40, 16) }, { 26, new FontSizeInfo(43, 17) },
                    { 28, new FontSizeInfo(46, 18) }, { 36, new FontSizeInfo(59, 23) },
                    { 48, new FontSizeInfo(80, 30) }, { 72, new FontSizeInfo(0, 45) },
                    { 96, new FontSizeInfo(160, 59) }, { 128, new FontSizeInfo(214, 79) },
                    { 256, new FontSizeInfo(427, 156) }
                }
            },
            {
                "Microsoft Uighur",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(23, 4) }, { 10, new FontSizeInfo(23, 5) },
                    { 11, new FontSizeInfo(24, 6) }, { 12, new FontSizeInfo(25, 6) }, { 14, new FontSizeInfo(30, 7) },
                    { 16, new FontSizeInfo(33, 8) }, { 18, new FontSizeInfo(37, 9) }, { 20, new FontSizeInfo(42, 10) },
                    { 22, new FontSizeInfo(45, 11) }, { 24, new FontSizeInfo(52, 12) },
                    { 26, new FontSizeInfo(56, 13) }, { 28, new FontSizeInfo(59, 14) },
                    { 36, new FontSizeInfo(76, 18) }, { 48, new FontSizeInfo(102, 24) },
                    { 72, new FontSizeInfo(0, 36) }, { 96, new FontSizeInfo(202, 48) },
                    { 128, new FontSizeInfo(272, 64) }, { 256, new FontSizeInfo(476, 128) }
                }
            },
            {
                "Monotype Corsiva",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 9) },
                    { 16, new FontSizeInfo(28, 10) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(36, 13) }, { 22, new FontSizeInfo(39, 14) },
                    { 24, new FontSizeInfo(42, 15) }, { 26, new FontSizeInfo(47, 16) },
                    { 28, new FontSizeInfo(49, 17) }, { 36, new FontSizeInfo(62, 22) },
                    { 48, new FontSizeInfo(85, 29) }, { 72, new FontSizeInfo(0, 43) },
                    { 96, new FontSizeInfo(170, 57) }, { 128, new FontSizeInfo(225, 76) },
                    { 256, new FontSizeInfo(453, 151) }
                }
            },
            {
                "Niagara Engraved",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(21, 5) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(20, 5) }, { 12, new FontSizeInfo(21, 6) }, { 14, new FontSizeInfo(24, 7) },
                    { 16, new FontSizeInfo(26, 8) }, { 18, new FontSizeInfo(29, 9) }, { 20, new FontSizeInfo(33, 9) },
                    { 22, new FontSizeInfo(35, 10) }, { 24, new FontSizeInfo(39, 11) },
                    { 26, new FontSizeInfo(43, 11) }, { 28, new FontSizeInfo(45, 12) },
                    { 36, new FontSizeInfo(58, 15) }, { 48, new FontSizeInfo(77, 20) }, { 72, new FontSizeInfo(0, 30) },
                    { 96, new FontSizeInfo(154, 39) }, { 128, new FontSizeInfo(207, 52) },
                    { 256, new FontSizeInfo(407, 103) }
                }
            },
            {
                "Niagara Solid",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(21, 5) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(20, 6) }, { 12, new FontSizeInfo(21, 6) }, { 14, new FontSizeInfo(24, 7) },
                    { 16, new FontSizeInfo(26, 8) }, { 18, new FontSizeInfo(29, 9) }, { 20, new FontSizeInfo(33, 9) },
                    { 22, new FontSizeInfo(35, 10) }, { 24, new FontSizeInfo(39, 11) },
                    { 26, new FontSizeInfo(43, 11) }, { 28, new FontSizeInfo(45, 12) },
                    { 36, new FontSizeInfo(58, 15) }, { 48, new FontSizeInfo(77, 20) }, { 72, new FontSizeInfo(0, 30) },
                    { 96, new FontSizeInfo(154, 39) }, { 128, new FontSizeInfo(207, 52) },
                    { 256, new FontSizeInfo(407, 103) }
                }
            },
            {
                "OCR A Extended",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 8) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(20, 10) }, { 12, new FontSizeInfo(21, 11) },
                    { 14, new FontSizeInfo(24, 12) }, { 16, new FontSizeInfo(26, 14) },
                    { 18, new FontSizeInfo(29, 15) }, { 20, new FontSizeInfo(33, 17) },
                    { 22, new FontSizeInfo(35, 19) }, { 24, new FontSizeInfo(39, 20) },
                    { 26, new FontSizeInfo(42, 22) }, { 28, new FontSizeInfo(45, 23) },
                    { 36, new FontSizeInfo(57, 30) }, { 48, new FontSizeInfo(76, 40) }, { 72, new FontSizeInfo(0, 59) },
                    { 96, new FontSizeInfo(152, 78) }, { 128, new FontSizeInfo(202, 104) },
                    { 256, new FontSizeInfo(402, 207) }
                }
            },
            {
                "Old English Text MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(21, 9) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(30, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(36, 16) },
                    { 24, new FontSizeInfo(40, 17) }, { 26, new FontSizeInfo(44, 19) },
                    { 28, new FontSizeInfo(46, 20) }, { 36, new FontSizeInfo(59, 25) },
                    { 48, new FontSizeInfo(79, 34) }, { 72, new FontSizeInfo(0, 50) },
                    { 96, new FontSizeInfo(157, 66) }, { 128, new FontSizeInfo(209, 88) },
                    { 256, new FontSizeInfo(418, 175) }
                }
            },
            {
                "Onyx",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(20, 6) }, { 12, new FontSizeInfo(21, 7) }, { 14, new FontSizeInfo(24, 7) },
                    { 16, new FontSizeInfo(26, 8) }, { 18, new FontSizeInfo(29, 9) }, { 20, new FontSizeInfo(33, 9) },
                    { 22, new FontSizeInfo(35, 10) }, { 24, new FontSizeInfo(39, 10) },
                    { 26, new FontSizeInfo(42, 11) }, { 28, new FontSizeInfo(45, 11) },
                    { 36, new FontSizeInfo(58, 15) }, { 48, new FontSizeInfo(77, 19) }, { 72, new FontSizeInfo(0, 29) },
                    { 96, new FontSizeInfo(152, 39) }, { 128, new FontSizeInfo(203, 50) },
                    { 256, new FontSizeInfo(416, 97) }
                }
            },
            {
                "MS Outlook",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(26, 12) }, { 18, new FontSizeInfo(29, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(36, 16) },
                    { 24, new FontSizeInfo(40, 17) }, { 26, new FontSizeInfo(43, 19) },
                    { 28, new FontSizeInfo(46, 20) }, { 36, new FontSizeInfo(59, 25) },
                    { 48, new FontSizeInfo(78, 33) }, { 72, new FontSizeInfo(0, 49) },
                    { 96, new FontSizeInfo(154, 65) }, { 128, new FontSizeInfo(206, 87) },
                    { 256, new FontSizeInfo(410, 172) }
                }
            },
            {
                "Palace Script MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(20, 6) }, { 12, new FontSizeInfo(21, 6) }, { 14, new FontSizeInfo(25, 7) },
                    { 16, new FontSizeInfo(27, 8) }, { 18, new FontSizeInfo(31, 9) }, { 20, new FontSizeInfo(35, 9) },
                    { 22, new FontSizeInfo(37, 10) }, { 24, new FontSizeInfo(41, 11) },
                    { 26, new FontSizeInfo(44, 12) }, { 28, new FontSizeInfo(47, 13) },
                    { 36, new FontSizeInfo(61, 16) }, { 48, new FontSizeInfo(81, 21) }, { 72, new FontSizeInfo(0, 31) },
                    { 96, new FontSizeInfo(160, 41) }, { 128, new FontSizeInfo(213, 55) },
                    { 256, new FontSizeInfo(425, 108) }
                }
            },
            {
                "Papyrus",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(23, 7) }, { 10, new FontSizeInfo(24, 9) },
                    { 11, new FontSizeInfo(26, 10) }, { 12, new FontSizeInfo(27, 10) },
                    { 14, new FontSizeInfo(32, 12) }, { 16, new FontSizeInfo(35, 14) },
                    { 18, new FontSizeInfo(40, 15) }, { 20, new FontSizeInfo(44, 16) },
                    { 22, new FontSizeInfo(49, 18) }, { 24, new FontSizeInfo(54, 20) },
                    { 26, new FontSizeInfo(58, 22) }, { 28, new FontSizeInfo(61, 22) },
                    { 36, new FontSizeInfo(80, 29) }, { 48, new FontSizeInfo(104, 38) },
                    { 72, new FontSizeInfo(0, 56) }, { 96, new FontSizeInfo(210, 74) },
                    { 128, new FontSizeInfo(279, 99) }, { 256, new FontSizeInfo(556, 197) }
                }
            },
            {
                "Parchment",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 6) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(21, 4) },
                    { 11, new FontSizeInfo(22, 4) }, { 12, new FontSizeInfo(23, 4) }, { 14, new FontSizeInfo(27, 5) },
                    { 16, new FontSizeInfo(29, 5) }, { 18, new FontSizeInfo(33, 6) }, { 20, new FontSizeInfo(36, 7) },
                    { 22, new FontSizeInfo(39, 7) }, { 24, new FontSizeInfo(43, 8) }, { 26, new FontSizeInfo(46, 8) },
                    { 28, new FontSizeInfo(49, 9) }, { 36, new FontSizeInfo(64, 11) }, { 48, new FontSizeInfo(85, 14) },
                    { 72, new FontSizeInfo(0, 21) }, { 96, new FontSizeInfo(169, 28) },
                    { 128, new FontSizeInfo(226, 37) }, { 256, new FontSizeInfo(450, 73) }
                }
            },
            {
                "Perpetua",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(21, 7) }, { 12, new FontSizeInfo(22, 7) }, { 14, new FontSizeInfo(26, 9) },
                    { 16, new FontSizeInfo(29, 10) }, { 18, new FontSizeInfo(33, 11) },
                    { 20, new FontSizeInfo(37, 12) }, { 22, new FontSizeInfo(39, 13) },
                    { 24, new FontSizeInfo(43, 15) }, { 26, new FontSizeInfo(47, 16) },
                    { 28, new FontSizeInfo(50, 17) }, { 36, new FontSizeInfo(65, 22) },
                    { 48, new FontSizeInfo(86, 29) }, { 72, new FontSizeInfo(0, 44) },
                    { 96, new FontSizeInfo(171, 59) }, { 128, new FontSizeInfo(228, 78) },
                    { 256, new FontSizeInfo(453, 156) }
                }
            },
            {
                "Perpetua Titling MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(20, 10) }, { 12, new FontSizeInfo(21, 11) },
                    { 14, new FontSizeInfo(24, 13) }, { 16, new FontSizeInfo(27, 14) },
                    { 18, new FontSizeInfo(30, 16) }, { 20, new FontSizeInfo(34, 18) },
                    { 22, new FontSizeInfo(36, 19) }, { 24, new FontSizeInfo(40, 21) },
                    { 26, new FontSizeInfo(44, 23) }, { 28, new FontSizeInfo(46, 25) },
                    { 36, new FontSizeInfo(60, 32) }, { 48, new FontSizeInfo(79, 43) }, { 72, new FontSizeInfo(0, 64) },
                    { 96, new FontSizeInfo(158, 85) }, { 128, new FontSizeInfo(210, 114) },
                    { 256, new FontSizeInfo(418, 227) }
                }
            },
            {
                "Playbill",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(20, 5) }, { 12, new FontSizeInfo(21, 6) }, { 14, new FontSizeInfo(24, 7) },
                    { 16, new FontSizeInfo(26, 7) }, { 18, new FontSizeInfo(29, 9) }, { 20, new FontSizeInfo(33, 10) },
                    { 22, new FontSizeInfo(35, 10) }, { 24, new FontSizeInfo(39, 11) },
                    { 26, new FontSizeInfo(42, 12) }, { 28, new FontSizeInfo(45, 13) },
                    { 36, new FontSizeInfo(58, 17) }, { 48, new FontSizeInfo(77, 23) }, { 72, new FontSizeInfo(0, 34) },
                    { 96, new FontSizeInfo(153, 46) }, { 128, new FontSizeInfo(203, 61) },
                    { 256, new FontSizeInfo(405, 122) }
                }
            },
            {
                "Poor Richard",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(30, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(37, 16) },
                    { 24, new FontSizeInfo(40, 18) }, { 26, new FontSizeInfo(45, 19) },
                    { 28, new FontSizeInfo(48, 20) }, { 36, new FontSizeInfo(60, 26) },
                    { 48, new FontSizeInfo(79, 35) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(157, 70) }, { 128, new FontSizeInfo(209, 94) },
                    { 256, new FontSizeInfo(416, 187) }
                }
            },
            {
                "Pristina",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 5) }, { 10, new FontSizeInfo(22, 7) },
                    { 11, new FontSizeInfo(23, 8) }, { 12, new FontSizeInfo(24, 8) }, { 14, new FontSizeInfo(29, 9) },
                    { 16, new FontSizeInfo(32, 10) }, { 18, new FontSizeInfo(36, 12) },
                    { 20, new FontSizeInfo(40, 13) }, { 22, new FontSizeInfo(43, 14) },
                    { 24, new FontSizeInfo(47, 15) }, { 26, new FontSizeInfo(52, 16) },
                    { 28, new FontSizeInfo(55, 17) }, { 36, new FontSizeInfo(71, 23) },
                    { 48, new FontSizeInfo(94, 30) }, { 72, new FontSizeInfo(0, 45) },
                    { 96, new FontSizeInfo(187, 61) }, { 128, new FontSizeInfo(248, 81) },
                    { 256, new FontSizeInfo(494, 162) }
                }
            },
            {
                "Rage Italic",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(21, 6) }, { 10, new FontSizeInfo(21, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(24, 8) }, { 14, new FontSizeInfo(28, 10) },
                    { 16, new FontSizeInfo(31, 11) }, { 18, new FontSizeInfo(35, 12) },
                    { 20, new FontSizeInfo(39, 14) }, { 22, new FontSizeInfo(42, 15) },
                    { 24, new FontSizeInfo(46, 17) }, { 26, new FontSizeInfo(50, 18) },
                    { 28, new FontSizeInfo(53, 19) }, { 36, new FontSizeInfo(69, 25) },
                    { 48, new FontSizeInfo(91, 33) }, { 72, new FontSizeInfo(0, 50) },
                    { 96, new FontSizeInfo(182, 66) }, { 128, new FontSizeInfo(242, 89) },
                    { 256, new FontSizeInfo(482, 177) }
                }
            },
            {
                "Ravie",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(22, 10) }, { 10, new FontSizeInfo(22, 12) },
                    { 11, new FontSizeInfo(24, 14) }, { 12, new FontSizeInfo(25, 15) },
                    { 14, new FontSizeInfo(29, 18) }, { 16, new FontSizeInfo(34, 20) },
                    { 18, new FontSizeInfo(37, 23) }, { 20, new FontSizeInfo(38, 26) },
                    { 22, new FontSizeInfo(42, 28) }, { 24, new FontSizeInfo(46, 30) },
                    { 26, new FontSizeInfo(52, 33) }, { 28, new FontSizeInfo(53, 35) },
                    { 36, new FontSizeInfo(67, 46) }, { 48, new FontSizeInfo(91, 61) }, { 72, new FontSizeInfo(0, 91) },
                    { 96, new FontSizeInfo(179, 122) }, { 128, new FontSizeInfo(238, 162) },
                    { 256, new FontSizeInfo(472, 324) }
                }
            },
            {
                "MS Reference Sans Serif",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 10) }, { 12, new FontSizeInfo(21, 10) },
                    { 14, new FontSizeInfo(26, 12) }, { 16, new FontSizeInfo(27, 13) },
                    { 18, new FontSizeInfo(30, 15) }, { 20, new FontSizeInfo(36, 17) },
                    { 22, new FontSizeInfo(36, 18) }, { 24, new FontSizeInfo(42, 20) },
                    { 26, new FontSizeInfo(46, 22) }, { 28, new FontSizeInfo(48, 24) },
                    { 36, new FontSizeInfo(62, 31) }, { 48, new FontSizeInfo(81, 41) }, { 72, new FontSizeInfo(0, 61) },
                    { 96, new FontSizeInfo(159, 81) }, { 128, new FontSizeInfo(211, 109) },
                    { 256, new FontSizeInfo(418, 216) }
                }
            },
            {
                "MS Reference Specialty",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 10) }, { 8, new FontSizeInfo(20, 13) }, { 10, new FontSizeInfo(20, 16) },
                    { 11, new FontSizeInfo(20, 18) }, { 12, new FontSizeInfo(21, 19) },
                    { 14, new FontSizeInfo(24, 23) }, { 16, new FontSizeInfo(27, 25) },
                    { 18, new FontSizeInfo(32, 29) }, { 20, new FontSizeInfo(34, 32) },
                    { 22, new FontSizeInfo(36, 35) }, { 24, new FontSizeInfo(42, 38) },
                    { 26, new FontSizeInfo(44, 42) }, { 28, new FontSizeInfo(48, 44) },
                    { 36, new FontSizeInfo(59, 58) }, { 48, new FontSizeInfo(81, 77) },
                    { 72, new FontSizeInfo(0, 115) }, { 96, new FontSizeInfo(159, 153) },
                    { 128, new FontSizeInfo(213, 205) }, { 256, new FontSizeInfo(423, 408) }
                }
            },
            {
                "Rockwell Condensed",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(20, 5) }, { 12, new FontSizeInfo(21, 6) }, { 14, new FontSizeInfo(24, 7) },
                    { 16, new FontSizeInfo(27, 8) }, { 18, new FontSizeInfo(31, 9) }, { 20, new FontSizeInfo(34, 10) },
                    { 22, new FontSizeInfo(37, 11) }, { 24, new FontSizeInfo(41, 12) },
                    { 26, new FontSizeInfo(44, 13) }, { 28, new FontSizeInfo(47, 14) },
                    { 36, new FontSizeInfo(60, 18) }, { 48, new FontSizeInfo(80, 23) }, { 72, new FontSizeInfo(0, 35) },
                    { 96, new FontSizeInfo(159, 47) }, { 128, new FontSizeInfo(212, 62) },
                    { 256, new FontSizeInfo(422, 125) }
                }
            },
            {
                "Rockwell",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(24, 10) },
                    { 16, new FontSizeInfo(27, 11) }, { 18, new FontSizeInfo(30, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(36, 16) },
                    { 24, new FontSizeInfo(40, 17) }, { 26, new FontSizeInfo(44, 19) },
                    { 28, new FontSizeInfo(46, 20) }, { 36, new FontSizeInfo(60, 26) },
                    { 48, new FontSizeInfo(80, 35) }, { 72, new FontSizeInfo(0, 52) },
                    { 96, new FontSizeInfo(158, 69) }, { 128, new FontSizeInfo(210, 93) },
                    { 256, new FontSizeInfo(420, 185) }
                }
            },
            {
                "Rockwell Extra Bold",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 8) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(20, 10) }, { 12, new FontSizeInfo(21, 11) },
                    { 14, new FontSizeInfo(24, 13) }, { 16, new FontSizeInfo(27, 14) },
                    { 18, new FontSizeInfo(30, 17) }, { 20, new FontSizeInfo(34, 19) },
                    { 22, new FontSizeInfo(37, 20) }, { 24, new FontSizeInfo(40, 22) },
                    { 26, new FontSizeInfo(44, 24) }, { 28, new FontSizeInfo(46, 25) },
                    { 36, new FontSizeInfo(60, 33) }, { 48, new FontSizeInfo(80, 44) }, { 72, new FontSizeInfo(0, 66) },
                    { 96, new FontSizeInfo(158, 88) }, { 128, new FontSizeInfo(211, 118) },
                    { 256, new FontSizeInfo(420, 234) }
                }
            },
            {
                "Script MT Bold",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 8) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(27, 11) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(35, 14) }, { 22, new FontSizeInfo(37, 15) },
                    { 24, new FontSizeInfo(41, 17) }, { 26, new FontSizeInfo(45, 18) },
                    { 28, new FontSizeInfo(47, 20) }, { 36, new FontSizeInfo(61, 25) },
                    { 48, new FontSizeInfo(81, 34) }, { 72, new FontSizeInfo(0, 51) },
                    { 96, new FontSizeInfo(161, 68) }, { 128, new FontSizeInfo(215, 90) },
                    { 256, new FontSizeInfo(428, 180) }
                }
            },
            {
                "Showcard Gothic",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(22, 9) }, { 14, new FontSizeInfo(24, 11) },
                    { 16, new FontSizeInfo(30, 12) }, { 18, new FontSizeInfo(32, 14) },
                    { 20, new FontSizeInfo(35, 16) }, { 22, new FontSizeInfo(38, 17) },
                    { 24, new FontSizeInfo(42, 19) }, { 26, new FontSizeInfo(47, 21) },
                    { 28, new FontSizeInfo(48, 22) }, { 36, new FontSizeInfo(63, 28) },
                    { 48, new FontSizeInfo(82, 38) }, { 72, new FontSizeInfo(0, 57) },
                    { 96, new FontSizeInfo(165, 76) }, { 128, new FontSizeInfo(219, 101) },
                    { 256, new FontSizeInfo(435, 200) }
                }
            },
            {
                "Snap ITC",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 10) }, { 10, new FontSizeInfo(20, 11) },
                    { 11, new FontSizeInfo(20, 13) }, { 12, new FontSizeInfo(24, 14) },
                    { 14, new FontSizeInfo(26, 16) }, { 16, new FontSizeInfo(28, 18) },
                    { 18, new FontSizeInfo(32, 21) }, { 20, new FontSizeInfo(37, 23) },
                    { 22, new FontSizeInfo(38, 25) }, { 24, new FontSizeInfo(44, 28) },
                    { 26, new FontSizeInfo(46, 30) }, { 28, new FontSizeInfo(50, 32) },
                    { 36, new FontSizeInfo(65, 42) }, { 48, new FontSizeInfo(84, 55) }, { 72, new FontSizeInfo(0, 83) },
                    { 96, new FontSizeInfo(167, 111) }, { 128, new FontSizeInfo(225, 148) },
                    { 256, new FontSizeInfo(443, 295) }
                }
            },
            {
                "Stencil",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 9) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 11) },
                    { 16, new FontSizeInfo(28, 12) }, { 18, new FontSizeInfo(32, 14) },
                    { 20, new FontSizeInfo(35, 16) }, { 22, new FontSizeInfo(38, 17) },
                    { 24, new FontSizeInfo(42, 18) }, { 26, new FontSizeInfo(46, 20) },
                    { 28, new FontSizeInfo(48, 21) }, { 36, new FontSizeInfo(62, 28) },
                    { 48, new FontSizeInfo(83, 37) }, { 72, new FontSizeInfo(0, 55) },
                    { 96, new FontSizeInfo(164, 74) }, { 128, new FontSizeInfo(219, 98) },
                    { 256, new FontSizeInfo(436, 196) }
                }
            },
            {
                "Tw Cen MT",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(20, 8) }, { 12, new FontSizeInfo(21, 9) }, { 14, new FontSizeInfo(25, 10) },
                    { 16, new FontSizeInfo(27, 12) }, { 18, new FontSizeInfo(31, 13) },
                    { 20, new FontSizeInfo(34, 15) }, { 22, new FontSizeInfo(37, 16) },
                    { 24, new FontSizeInfo(40, 18) }, { 26, new FontSizeInfo(44, 19) },
                    { 28, new FontSizeInfo(47, 20) }, { 36, new FontSizeInfo(60, 26) },
                    { 48, new FontSizeInfo(80, 35) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(159, 71) }, { 128, new FontSizeInfo(212, 94) },
                    { 256, new FontSizeInfo(421, 188) }
                }
            },
            {
                "Tw Cen MT Condensed",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 4) }, { 10, new FontSizeInfo(20, 5) },
                    { 11, new FontSizeInfo(20, 5) }, { 12, new FontSizeInfo(21, 6) }, { 14, new FontSizeInfo(25, 7) },
                    { 16, new FontSizeInfo(27, 8) }, { 18, new FontSizeInfo(31, 9) }, { 20, new FontSizeInfo(34, 10) },
                    { 22, new FontSizeInfo(37, 11) }, { 24, new FontSizeInfo(41, 12) },
                    { 26, new FontSizeInfo(45, 13) }, { 28, new FontSizeInfo(48, 14) },
                    { 36, new FontSizeInfo(61, 18) }, { 48, new FontSizeInfo(80, 23) }, { 72, new FontSizeInfo(0, 35) },
                    { 96, new FontSizeInfo(160, 47) }, { 128, new FontSizeInfo(214, 62) },
                    { 256, new FontSizeInfo(424, 125) }
                }
            },
            {
                "Tw Cen MT Condensed Extra Bold",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(20, 7) }, { 12, new FontSizeInfo(22, 8) }, { 14, new FontSizeInfo(25, 9) },
                    { 16, new FontSizeInfo(27, 10) }, { 18, new FontSizeInfo(31, 12) },
                    { 20, new FontSizeInfo(35, 13) }, { 22, new FontSizeInfo(38, 14) },
                    { 24, new FontSizeInfo(42, 16) }, { 26, new FontSizeInfo(45, 17) },
                    { 28, new FontSizeInfo(48, 18) }, { 36, new FontSizeInfo(62, 24) },
                    { 48, new FontSizeInfo(81, 31) }, { 72, new FontSizeInfo(0, 47) },
                    { 96, new FontSizeInfo(165, 63) }, { 128, new FontSizeInfo(218, 84) },
                    { 256, new FontSizeInfo(414, 167) }
                }
            },
            {
                "Tempus Sans ITC",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 9) },
                    { 11, new FontSizeInfo(21, 10) }, { 12, new FontSizeInfo(22, 11) },
                    { 14, new FontSizeInfo(26, 13) }, { 16, new FontSizeInfo(29, 14) },
                    { 18, new FontSizeInfo(33, 16) }, { 20, new FontSizeInfo(37, 18) },
                    { 22, new FontSizeInfo(40, 19) }, { 24, new FontSizeInfo(44, 21) },
                    { 26, new FontSizeInfo(48, 23) }, { 28, new FontSizeInfo(51, 25) },
                    { 36, new FontSizeInfo(65, 32) }, { 48, new FontSizeInfo(87, 43) }, { 72, new FontSizeInfo(0, 64) },
                    { 96, new FontSizeInfo(173, 85) }, { 128, new FontSizeInfo(230, 114) },
                    { 256, new FontSizeInfo(458, 226) }
                }
            },
            {
                "Viner Hand ITC",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(23, 7) }, { 10, new FontSizeInfo(24, 9) },
                    { 11, new FontSizeInfo(25, 10) }, { 12, new FontSizeInfo(27, 11) },
                    { 14, new FontSizeInfo(32, 13) }, { 16, new FontSizeInfo(35, 14) },
                    { 18, new FontSizeInfo(40, 16) }, { 20, new FontSizeInfo(45, 18) },
                    { 22, new FontSizeInfo(48, 20) }, { 24, new FontSizeInfo(53, 22) },
                    { 26, new FontSizeInfo(58, 24) }, { 28, new FontSizeInfo(61, 25) },
                    { 36, new FontSizeInfo(79, 33) }, { 48, new FontSizeInfo(104, 43) },
                    { 72, new FontSizeInfo(0, 65) }, { 96, new FontSizeInfo(208, 87) },
                    { 128, new FontSizeInfo(277, 116) }, { 256, new FontSizeInfo(552, 232) }
                }
            },
            {
                "Vivaldi",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(20, 6) }, { 12, new FontSizeInfo(22, 7) }, { 14, new FontSizeInfo(25, 8) },
                    { 16, new FontSizeInfo(28, 9) }, { 18, new FontSizeInfo(32, 10) }, { 20, new FontSizeInfo(36, 12) },
                    { 22, new FontSizeInfo(38, 12) }, { 24, new FontSizeInfo(42, 14) },
                    { 26, new FontSizeInfo(46, 15) }, { 28, new FontSizeInfo(49, 16) },
                    { 36, new FontSizeInfo(63, 20) }, { 48, new FontSizeInfo(83, 27) }, { 72, new FontSizeInfo(0, 41) },
                    { 96, new FontSizeInfo(168, 55) }, { 128, new FontSizeInfo(225, 73) },
                    { 256, new FontSizeInfo(448, 146) }
                }
            },
            {
                "Vladimir Script",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(21, 7) }, { 12, new FontSizeInfo(22, 7) }, { 14, new FontSizeInfo(26, 9) },
                    { 16, new FontSizeInfo(29, 10) }, { 18, new FontSizeInfo(32, 11) },
                    { 20, new FontSizeInfo(36, 13) }, { 22, new FontSizeInfo(39, 13) },
                    { 24, new FontSizeInfo(43, 15) }, { 26, new FontSizeInfo(47, 16) },
                    { 28, new FontSizeInfo(50, 17) }, { 36, new FontSizeInfo(64, 22) },
                    { 48, new FontSizeInfo(85, 30) }, { 72, new FontSizeInfo(0, 44) },
                    { 96, new FontSizeInfo(169, 59) }, { 128, new FontSizeInfo(225, 79) },
                    { 256, new FontSizeInfo(448, 158) }
                }
            },
            {
                "Wingdings 2",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 9) }, { 8, new FontSizeInfo(20, 12) }, { 10, new FontSizeInfo(20, 15) },
                    { 11, new FontSizeInfo(20, 17) }, { 12, new FontSizeInfo(21, 18) },
                    { 14, new FontSizeInfo(24, 22) }, { 16, new FontSizeInfo(26, 24) },
                    { 18, new FontSizeInfo(30, 27) }, { 20, new FontSizeInfo(34, 31) },
                    { 22, new FontSizeInfo(36, 33) }, { 24, new FontSizeInfo(40, 36) },
                    { 26, new FontSizeInfo(43, 40) }, { 28, new FontSizeInfo(46, 42) },
                    { 36, new FontSizeInfo(59, 54) }, { 48, new FontSizeInfo(79, 73) },
                    { 72, new FontSizeInfo(0, 109) }, { 96, new FontSizeInfo(156, 145) },
                    { 128, new FontSizeInfo(208, 194) }, { 256, new FontSizeInfo(414, 386) }
                }
            },
            {
                "Wingdings 3",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 4) }, { 8, new FontSizeInfo(20, 10) }, { 10, new FontSizeInfo(20, 12) },
                    { 11, new FontSizeInfo(20, 13) }, { 12, new FontSizeInfo(21, 14) },
                    { 14, new FontSizeInfo(25, 17) }, { 16, new FontSizeInfo(27, 19) },
                    { 18, new FontSizeInfo(30, 21) }, { 20, new FontSizeInfo(34, 24) },
                    { 22, new FontSizeInfo(35, 26) }, { 24, new FontSizeInfo(40, 29) },
                    { 26, new FontSizeInfo(43, 19) }, { 28, new FontSizeInfo(46, 33) },
                    { 36, new FontSizeInfo(59, 27) }, { 48, new FontSizeInfo(79, 35) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(156, 71) }, { 128, new FontSizeInfo(208, 95) },
                    { 256, new FontSizeInfo(414, 189) }
                }
            },
            {
                "Buxton Sketch",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(21, 7) },
                    { 11, new FontSizeInfo(22, 9) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 11) },
                    { 16, new FontSizeInfo(30, 12) }, { 18, new FontSizeInfo(34, 14) },
                    { 20, new FontSizeInfo(38, 16) }, { 22, new FontSizeInfo(41, 17) },
                    { 24, new FontSizeInfo(45, 18) }, { 26, new FontSizeInfo(49, 20) },
                    { 28, new FontSizeInfo(52, 21) }, { 36, new FontSizeInfo(67, 28) },
                    { 48, new FontSizeInfo(90, 37) }, { 72, new FontSizeInfo(0, 55) },
                    { 96, new FontSizeInfo(178, 74) }, { 128, new FontSizeInfo(238, 99) },
                    { 256, new FontSizeInfo(473, 196) }
                }
            },
            {
                "Segoe Marker",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 5) }, { 10, new FontSizeInfo(20, 6) },
                    { 11, new FontSizeInfo(20, 6) }, { 12, new FontSizeInfo(21, 7) }, { 14, new FontSizeInfo(24, 8) },
                    { 16, new FontSizeInfo(27, 9) }, { 18, new FontSizeInfo(31, 10) }, { 20, new FontSizeInfo(34, 12) },
                    { 22, new FontSizeInfo(37, 12) }, { 24, new FontSizeInfo(41, 14) },
                    { 26, new FontSizeInfo(44, 15) }, { 28, new FontSizeInfo(47, 16) },
                    { 36, new FontSizeInfo(60, 20) }, { 48, new FontSizeInfo(80, 27) }, { 72, new FontSizeInfo(0, 41) },
                    { 96, new FontSizeInfo(159, 55) }, { 128, new FontSizeInfo(212, 73) },
                    { 256, new FontSizeInfo(422, 146) }
                }
            },
            {
                "SketchFlow Print",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 8) }, { 10, new FontSizeInfo(20, 10) },
                    { 11, new FontSizeInfo(20, 11) }, { 12, new FontSizeInfo(21, 12) },
                    { 14, new FontSizeInfo(25, 14) }, { 16, new FontSizeInfo(28, 15) },
                    { 18, new FontSizeInfo(31, 17) }, { 20, new FontSizeInfo(35, 20) },
                    { 22, new FontSizeInfo(38, 21) }, { 24, new FontSizeInfo(41, 23) },
                    { 26, new FontSizeInfo(45, 25) }, { 28, new FontSizeInfo(48, 27) },
                    { 36, new FontSizeInfo(61, 35) }, { 48, new FontSizeInfo(81, 47) }, { 72, new FontSizeInfo(0, 70) },
                    { 96, new FontSizeInfo(161, 93) }, { 128, new FontSizeInfo(215, 124) },
                    { 256, new FontSizeInfo(427, 248) }
                }
            },
            {
                "Microsoft MHei",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(29, 10) },
                    { 16, new FontSizeInfo(31, 12) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(39, 15) }, { 22, new FontSizeInfo(41, 16) },
                    { 24, new FontSizeInfo(47, 18) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(53, 20) }, { 36, new FontSizeInfo(69, 26) },
                    { 48, new FontSizeInfo(91, 35) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(181, 70) }, { 128, new FontSizeInfo(243, 94) },
                    { 256, new FontSizeInfo(482, 187) }
                }
            },
            {
                "Microsoft NeoGothic",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(29, 11) },
                    { 16, new FontSizeInfo(31, 12) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(39, 15) }, { 22, new FontSizeInfo(41, 16) },
                    { 24, new FontSizeInfo(47, 18) }, { 26, new FontSizeInfo(51, 20) },
                    { 28, new FontSizeInfo(53, 21) }, { 36, new FontSizeInfo(69, 27) },
                    { 48, new FontSizeInfo(91, 36) }, { 72, new FontSizeInfo(0, 54) },
                    { 96, new FontSizeInfo(181, 72) }, { 128, new FontSizeInfo(243, 96) },
                    { 256, new FontSizeInfo(482, 191) }
                }
            },
            {
                "Segoe WP Black",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 7) }, { 10, new FontSizeInfo(20, 8) },
                    { 11, new FontSizeInfo(22, 9) }, { 12, new FontSizeInfo(23, 10) }, { 14, new FontSizeInfo(29, 12) },
                    { 16, new FontSizeInfo(31, 13) }, { 18, new FontSizeInfo(35, 15) },
                    { 20, new FontSizeInfo(39, 17) }, { 22, new FontSizeInfo(41, 18) },
                    { 24, new FontSizeInfo(47, 20) }, { 26, new FontSizeInfo(51, 22) },
                    { 28, new FontSizeInfo(53, 23) }, { 36, new FontSizeInfo(69, 30) },
                    { 48, new FontSizeInfo(91, 40) }, { 72, new FontSizeInfo(0, 60) },
                    { 96, new FontSizeInfo(181, 79) }, { 128, new FontSizeInfo(243, 106) },
                    { 256, new FontSizeInfo(482, 212) }
                }
            },
            {
                "Segoe WP",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 11) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 18) }, { 26, new FontSizeInfo(51, 20) },
                    { 28, new FontSizeInfo(54, 21) }, { 36, new FontSizeInfo(70, 27) },
                    { 48, new FontSizeInfo(92, 36) }, { 72, new FontSizeInfo(0, 54) },
                    { 96, new FontSizeInfo(180, 72) }, { 128, new FontSizeInfo(241, 96) },
                    { 256, new FontSizeInfo(482, 191) }
                }
            },
            {
                "Segoe WP Semibold",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 9) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 11) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(35, 14) },
                    { 20, new FontSizeInfo(41, 16) }, { 22, new FontSizeInfo(44, 17) },
                    { 24, new FontSizeInfo(50, 18) }, { 26, new FontSizeInfo(51, 20) },
                    { 28, new FontSizeInfo(54, 21) }, { 36, new FontSizeInfo(70, 28) },
                    { 48, new FontSizeInfo(92, 37) }, { 72, new FontSizeInfo(0, 55) },
                    { 96, new FontSizeInfo(180, 74) }, { 128, new FontSizeInfo(241, 99) },
                    { 256, new FontSizeInfo(482, 196) }
                }
            },
            {
                "Segoe WP Light",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 8) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 11) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 14) }, { 22, new FontSizeInfo(44, 15) },
                    { 24, new FontSizeInfo(50, 17) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 25) },
                    { 48, new FontSizeInfo(92, 34) }, { 72, new FontSizeInfo(0, 51) },
                    { 96, new FontSizeInfo(180, 68) }, { 128, new FontSizeInfo(241, 91) },
                    { 256, new FontSizeInfo(482, 181) }
                }
            },
            {
                "Segoe WP SemiLight",
                new Dictionary<float, FontSizeInfo>
                {
                    { 6, new FontSizeInfo(20, 5) }, { 8, new FontSizeInfo(20, 6) }, { 10, new FontSizeInfo(20, 7) },
                    { 11, new FontSizeInfo(22, 8) }, { 12, new FontSizeInfo(23, 9) }, { 14, new FontSizeInfo(27, 10) },
                    { 16, new FontSizeInfo(34, 12) }, { 18, new FontSizeInfo(35, 13) },
                    { 20, new FontSizeInfo(41, 15) }, { 22, new FontSizeInfo(44, 16) },
                    { 24, new FontSizeInfo(50, 18) }, { 26, new FontSizeInfo(51, 19) },
                    { 28, new FontSizeInfo(54, 20) }, { 36, new FontSizeInfo(70, 26) },
                    { 48, new FontSizeInfo(92, 35) }, { 72, new FontSizeInfo(0, 53) },
                    { 96, new FontSizeInfo(180, 70) }, { 128, new FontSizeInfo(241, 94) },
                    { 256, new FontSizeInfo(482, 187) }
                }
            }
        };
}