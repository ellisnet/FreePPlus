// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     A formula based curve segment
/// </summary>
internal sealed class IccFormulaCurveElement : IccCurveSegment, IEquatable<IccFormulaCurveElement>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IccFormulaCurveElement" /> class.
    /// </summary>
    /// <param name="type">The type of this segment</param>
    /// <param name="gamma">Gamma segment parameter</param>
    /// <param name="a">A segment parameter</param>
    /// <param name="b">B segment parameter</param>
    /// <param name="c">C segment parameter</param>
    /// <param name="d">D segment parameter</param>
    /// <param name="e">E segment parameter</param>
    public IccFormulaCurveElement(IccFormulaCurveType type, float gamma, float a, float b, float c, float d, float e)
        : base(IccCurveSegmentSignature.FormulaCurve)
    {
        Type = type;
        Gamma = gamma;
        A = a;
        B = b;
        C = c;
        D = d;
        E = e;
    }

    /// <summary>
    ///     Gets the type of this curve
    /// </summary>
    public IccFormulaCurveType Type { get; }

    /// <summary>
    ///     Gets the gamma curve parameter
    /// </summary>
    public float Gamma { get; }

    /// <summary>
    ///     Gets the A curve parameter
    /// </summary>
    public float A { get; }

    /// <summary>
    ///     Gets the B curve parameter
    /// </summary>
    public float B { get; }

    /// <summary>
    ///     Gets the C curve parameter
    /// </summary>
    public float C { get; }

    /// <summary>
    ///     Gets the D curve parameter
    /// </summary>
    public float D { get; }

    /// <summary>
    ///     Gets the E curve parameter
    /// </summary>
    public float E { get; }

    /// <inheritdoc />
    public bool Equals(IccFormulaCurveElement other)
    {
        return Equals((IccCurveSegment)other);
    }

    /// <inheritdoc />
    public override bool Equals(IccCurveSegment other)
    {
        if (base.Equals(other) && other is IccFormulaCurveElement segment)
            return Type == segment.Type
                   && Gamma == segment.Gamma
                   && A == segment.A
                   && B == segment.B
                   && C == segment.C
                   && D == segment.D
                   && E == segment.E;

        return false;
    }
}