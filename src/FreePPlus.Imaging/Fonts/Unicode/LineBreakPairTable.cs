// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

namespace FreePPlus.Imaging.Fonts.Unicode;

//was previously: namespace SixLabors.Fonts.Unicode;

internal static class LineBreakPairTable
{
    /// <summary>
    ///     Direct break opportunity
    /// </summary>
    public const byte DIBRK = 0;

    /// <summary>
    ///     Indirect break opportunity
    /// </summary>
    public const byte INBRK = 1;

    /// <summary>
    ///     Indirect break opportunity for combining marks
    /// </summary>
    public const byte CIBRK = 2;

    /// <summary>
    ///     Prohibited break for combining marks
    /// </summary>
    public const byte CPBRK = 3;

    /// <summary>
    ///     Prohibited break
    /// </summary>
    public const byte PRBRK = 4;

    // Based on example pair table from https://www.unicode.org/reports/tr14/tr14-37.html#Table2
    // - ZWJ special processing for LB8a
    // - CB manually added as per Rule LB20
    public static byte[][] Table { get; } =
    {
        // .         OP     CL     CP     QU     GL     NS     EX     SY     IS     PR     PO     NU     AL     HL     ID     IN     HY     BA     BB     B2     ZW     CM     WJ     H2     H3     JL     JV     JT     RI     EB     EM     ZWJ    CB
        new[]
        {
            PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK,
            PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, CPBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK, PRBRK,
            PRBRK, PRBRK, PRBRK
        }, // OP
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, PRBRK, INBRK, INBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // CL
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, DIBRK,
            DIBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // CP
        new[]
        {
            PRBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, INBRK,
            INBRK, INBRK, INBRK, INBRK, INBRK, PRBRK, CIBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, INBRK, INBRK,
            INBRK, INBRK, INBRK
        }, // QU
        new[]
        {
            INBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, INBRK,
            INBRK, INBRK, INBRK, INBRK, INBRK, PRBRK, CIBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, INBRK, INBRK,
            INBRK, INBRK, INBRK
        }, // GL
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // NS
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // EX
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, DIBRK, INBRK, DIBRK, INBRK, DIBRK,
            DIBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // SY
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, DIBRK, INBRK, INBRK, INBRK, DIBRK,
            DIBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // IS
        new[]
        {
            INBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, DIBRK, INBRK, INBRK, INBRK, INBRK,
            DIBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, DIBRK, INBRK,
            INBRK, INBRK, DIBRK
        }, // PR
        new[]
        {
            INBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, DIBRK, INBRK, INBRK, INBRK, DIBRK,
            DIBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // PO
        new[]
        {
            INBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // NU
        new[]
        {
            INBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // AL
        new[]
        {
            INBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // HL
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, INBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // ID
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // IN
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, DIBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, DIBRK, INBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // HY
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, DIBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // BA
        new[]
        {
            INBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, INBRK,
            INBRK, INBRK, INBRK, INBRK, INBRK, PRBRK, CIBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, INBRK, INBRK,
            INBRK, INBRK, DIBRK
        }, // BB
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, INBRK, DIBRK, PRBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // B2
        new[]
        {
            DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, DIBRK, DIBRK
        }, // ZW
        new[]
        {
            INBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // CM
        new[]
        {
            INBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, INBRK,
            INBRK, INBRK, INBRK, INBRK, INBRK, PRBRK, CIBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, INBRK, INBRK,
            INBRK, INBRK, INBRK
        }, // WJ
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, INBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, INBRK, INBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // H2
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, INBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, INBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // H3
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, INBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // JL
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, INBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, INBRK, INBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // JV
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, INBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, INBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // JT
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, INBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // RI
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, INBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            INBRK, INBRK, DIBRK
        }, // EB
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, DIBRK, INBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // EM
        new[]
        {
            INBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, PRBRK, PRBRK, PRBRK, INBRK, INBRK, INBRK, INBRK, INBRK, DIBRK,
            INBRK, INBRK, INBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        }, // ZWJ
        new[]
        {
            DIBRK, PRBRK, PRBRK, INBRK, INBRK, DIBRK, PRBRK, PRBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, PRBRK, CIBRK, PRBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK, DIBRK,
            DIBRK, INBRK, DIBRK
        } // CB
    };
}