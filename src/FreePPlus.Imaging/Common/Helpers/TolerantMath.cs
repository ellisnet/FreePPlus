// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

/// <summary>
///     Implements basic math operations using tolerant comparison
///     whenever an equality check is needed.
/// </summary>
internal readonly struct TolerantMath
{
    private readonly double epsilon;

    private readonly double negEpsilon;

    /// <summary>
    ///     A read-only default instance for <see cref="TolerantMath" /> using 1e-8 as epsilon.
    ///     It is a field so it can be passed as an 'in' parameter.
    ///     Does not necessarily fit all use cases!
    /// </summary>
    public static readonly TolerantMath Default = new(1e-8);

    public TolerantMath(double epsilon)
    {
        DebugGuard.MustBeGreaterThan(epsilon, 0, nameof(epsilon));

        this.epsilon = epsilon;
        negEpsilon = -epsilon;
    }

    /// <summary>
    ///     <paramref name="a" /> == 0
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool IsZero(double a)
    {
        return a > negEpsilon && a < epsilon;
    }

    /// <summary>
    ///     <paramref name="a" /> &gt; 0
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool IsPositive(double a)
    {
        return a > epsilon;
    }

    /// <summary>
    ///     <paramref name="a" /> &lt; 0
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool IsNegative(double a)
    {
        return a < negEpsilon;
    }

    /// <summary>
    ///     <paramref name="a" /> == <paramref name="b" />
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool AreEqual(double a, double b)
    {
        return IsZero(a - b);
    }

    /// <summary>
    ///     <paramref name="a" /> &gt; <paramref name="b" />
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool IsGreater(double a, double b)
    {
        return a > b + epsilon;
    }

    /// <summary>
    ///     <paramref name="a" /> &lt; <paramref name="b" />
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool IsLess(double a, double b)
    {
        return a < b - epsilon;
    }

    /// <summary>
    ///     <paramref name="a" /> &gt;= <paramref name="b" />
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool IsGreaterOrEqual(double a, double b)
    {
        return a >= b - epsilon;
    }

    /// <summary>
    ///     <paramref name="a" /> &lt;= <paramref name="b" />
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool IsLessOrEqual(double a, double b)
    {
        return b >= a - epsilon;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public double Ceiling(double a)
    {
        var rem = Math.IEEERemainder(a, 1);
        if (IsZero(rem)) return Math.Round(a);

        return Math.Ceiling(a);
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public double Floor(double a)
    {
        var rem = Math.IEEERemainder(a, 1);
        if (IsZero(rem)) return Math.Round(a);

        return Math.Floor(a);
    }
}