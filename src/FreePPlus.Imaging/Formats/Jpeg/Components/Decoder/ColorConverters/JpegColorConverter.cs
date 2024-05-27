// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Numerics;
using FreePPlus.Imaging.Memory;
using FreePPlus.Imaging.Tuples;

namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder.ColorConverters;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder.ColorConverters;

/// <summary>
///     Encapsulates the conversion of Jpeg channels to RGBA values packed in <see cref="Vector4" /> buffer.
/// </summary>
internal abstract partial class JpegColorConverter
{
    /// <summary>
    ///     The available converters
    /// </summary>
    private static readonly JpegColorConverter[] Converters =
    {
        // 8-bit converters
        GetYCbCrConverter(8),
        new FromYccK(8),
        new FromCmyk(8),
        new FromGrayscale(8),
        new FromRgb(8),

        // 12-bit converters
        GetYCbCrConverter(12),
        new FromYccK(12),
        new FromCmyk(12),
        new FromGrayscale(12),
        new FromRgb(12)
    };

    /// <summary>
    ///     Initializes a new instance of the <see cref="JpegColorConverter" /> class.
    /// </summary>
    protected JpegColorConverter(JpegColorSpace colorSpace, int precision)
    {
        ColorSpace = colorSpace;
        Precision = precision;
        MaximumValue = MathF.Pow(2, precision) - 1;
        HalfValue = MathF.Ceiling(MaximumValue / 2);
    }

    /// <summary>
    ///     Gets the <see cref="JpegColorSpace" /> of this converter.
    /// </summary>
    public JpegColorSpace ColorSpace { get; }

    /// <summary>
    ///     Gets the Precision of this converter in bits.
    /// </summary>
    public int Precision { get; }

    /// <summary>
    ///     Gets the maximum value of a sample
    /// </summary>
    private float MaximumValue { get; }

    /// <summary>
    ///     Gets the half of the maximum value of a sample
    /// </summary>
    private float HalfValue { get; }

    /// <summary>
    ///     Returns the <see cref="JpegColorConverter" /> corresponding to the given <see cref="JpegColorSpace" />
    /// </summary>
    public static JpegColorConverter GetConverter(JpegColorSpace colorSpace, int precision)
    {
        var converter = Array.Find(Converters, c => c.ColorSpace == colorSpace
                                                    && c.Precision == precision);

        if (converter is null) throw new Exception($"Could not find any converter for JpegColorSpace {colorSpace}!");

        return converter;
    }

    /// <summary>
    ///     He implementation of the conversion.
    /// </summary>
    /// <param name="values">The input as a stack-only <see cref="ComponentValues" /> struct</param>
    /// <param name="result">The destination buffer of <see cref="Vector4" /> values</param>
    public abstract void ConvertToRgba(in ComponentValues values, Span<Vector4> result);

    /// <summary>
    ///     Returns the <see cref="JpegColorConverter" /> for the YCbCr colorspace that matches the current CPU architecture.
    /// </summary>
    private static JpegColorConverter GetYCbCrConverter(int precision)
    {
        return FromYCbCrSimdVector8.IsAvailable ? new FromYCbCrSimdVector8(precision) : new FromYCbCrSimd(precision);
    }

    /// <summary>
    ///     A stack-only struct to reference the input buffers using <see cref="ReadOnlySpan{T}" />-s.
    /// </summary>
#pragma warning disable SA1206 // Declaration keywords should follow order
    public readonly ref struct ComponentValues
#pragma warning restore SA1206 // Declaration keywords should follow order
    {
        /// <summary>
        ///     The component count
        /// </summary>
        public readonly int ComponentCount;

        /// <summary>
        ///     The component 0 (eg. Y)
        /// </summary>
        public readonly ReadOnlySpan<float> Component0;

        /// <summary>
        ///     The component 1 (eg. Cb)
        /// </summary>
        public readonly ReadOnlySpan<float> Component1;

        /// <summary>
        ///     The component 2 (eg. Cr)
        /// </summary>
        public readonly ReadOnlySpan<float> Component2;

        /// <summary>
        ///     The component 4
        /// </summary>
        public readonly ReadOnlySpan<float> Component3;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComponentValues" /> struct.
        /// </summary>
        /// <param name="componentBuffers">The 1-4 sized list of component buffers.</param>
        /// <param name="row">The row to convert</param>
        public ComponentValues(IReadOnlyList<Buffer2D<float>> componentBuffers, int row)
        {
            ComponentCount = componentBuffers.Count;

            Component0 = componentBuffers[0].GetRowSpan(row);
            Component1 = Span<float>.Empty;
            Component2 = Span<float>.Empty;
            Component3 = Span<float>.Empty;

            if (ComponentCount > 1)
            {
                Component1 = componentBuffers[1].GetRowSpan(row);
                if (ComponentCount > 2)
                {
                    Component2 = componentBuffers[2].GetRowSpan(row);
                    if (ComponentCount > 3) Component3 = componentBuffers[3].GetRowSpan(row);
                }
            }
        }

        private ComponentValues(
            int componentCount,
            ReadOnlySpan<float> c0,
            ReadOnlySpan<float> c1,
            ReadOnlySpan<float> c2,
            ReadOnlySpan<float> c3)
        {
            ComponentCount = componentCount;
            Component0 = c0;
            Component1 = c1;
            Component2 = c2;
            Component3 = c3;
        }

        public ComponentValues Slice(int start, int length)
        {
            var c0 = Component0.Slice(start, length);
            var c1 = ComponentCount > 1 ? Component1.Slice(start, length) : ReadOnlySpan<float>.Empty;
            var c2 = ComponentCount > 2 ? Component2.Slice(start, length) : ReadOnlySpan<float>.Empty;
            var c3 = ComponentCount > 3 ? Component3.Slice(start, length) : ReadOnlySpan<float>.Empty;

            return new ComponentValues(ComponentCount, c0, c1, c2, c3);
        }
    }

    internal struct Vector4Octet
    {
#pragma warning disable SA1132 // Do not combine fields
        public Vector4 V0, V1, V2, V3, V4, V5, V6, V7;

        /// <summary>
        ///     Pack (r0,r1...r7) (g0,g1...g7) (b0,b1...b7) vector values as (r0,g0,b0,1), (r1,g1,b1,1) ...
        /// </summary>
        public void Pack(ref Vector4Pair r, ref Vector4Pair g, ref Vector4Pair b)
        {
            V0.X = r.A.X;
            V0.Y = g.A.X;
            V0.Z = b.A.X;
            V0.W = 1f;

            V1.X = r.A.Y;
            V1.Y = g.A.Y;
            V1.Z = b.A.Y;
            V1.W = 1f;

            V2.X = r.A.Z;
            V2.Y = g.A.Z;
            V2.Z = b.A.Z;
            V2.W = 1f;

            V3.X = r.A.W;
            V3.Y = g.A.W;
            V3.Z = b.A.W;
            V3.W = 1f;

            V4.X = r.B.X;
            V4.Y = g.B.X;
            V4.Z = b.B.X;
            V4.W = 1f;

            V5.X = r.B.Y;
            V5.Y = g.B.Y;
            V5.Z = b.B.Y;
            V5.W = 1f;

            V6.X = r.B.Z;
            V6.Y = g.B.Z;
            V6.Z = b.B.Z;
            V6.W = 1f;

            V7.X = r.B.W;
            V7.Y = g.B.W;
            V7.Z = b.B.W;
            V7.W = 1f;
        }
    }
}