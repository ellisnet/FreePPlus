// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Numerics;
using FreePPlus.Imaging.Fonts.Tables.TrueType.Hinting;
#if SUPPORTS_RUNTIME_INTRINSICS
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif

namespace FreePPlus.Imaging.Fonts.Tables.TrueType.Glyphs;

//was previously: namespace SixLabors.Fonts.Tables.TrueType.Glyphs;

/// <summary>
///     Represents the raw glyph outlines for a given glyph comprised of a collection of glyph table entries.
///     The type is mutable by design to reduce copying during transformation.
/// </summary>
internal struct GlyphVector
{
    private readonly List<GlyphTableEntry> entries;
    private readonly Bounds compositeBounds;

    internal GlyphVector(
        Vector2[] controlPoints,
        bool[] onCurves,
        ushort[] endPoints,
        Bounds bounds,
        ReadOnlyMemory<byte> instructions)
    {
        entries = new List<GlyphTableEntry>
        {
            new(controlPoints, onCurves, endPoints, bounds, instructions)
        };

        compositeBounds = default;
    }

    private GlyphVector(List<GlyphTableEntry> entries, Bounds compositeBounds = default)
    {
        this.entries = entries;
        this.compositeBounds = compositeBounds;
    }

    public static GlyphVector Empty(Bounds bounds = default)
    {
        return new GlyphVector(Array.Empty<Vector2>(), Array.Empty<bool>(), Array.Empty<ushort>(), bounds,
            Array.Empty<byte>());
    }

    /// <summary>
    ///     Transforms a glyph vector by a specified 3x2 matrix.
    /// </summary>
    /// <param name="src">The glyph vector to transform.</param>
    /// <param name="matrix">The transformation matrix.</param>
    /// <returns>The new <see cref="GlyphVector" />.</returns>
    public static GlyphVector Transform(GlyphVector src, Matrix3x2 matrix)
    {
        List<GlyphTableEntry> entries = new(src.entries.Count);
        for (var i = 0; i < src.entries.Count; i++) entries.Add(GlyphTableEntry.Transform(src.entries[i], matrix));

        if (src.compositeBounds == default) return new GlyphVector(entries, src.compositeBounds);

        return new GlyphVector(entries, Bounds.Transform(src.compositeBounds, matrix));
    }

    /// <summary>
    ///     Scales a glyph vector uniformly by a specified scale.
    /// </summary>
    /// <param name="src">The glyph vector to translate.</param>
    /// <param name="scale">The uniform scale to use.</param>
    /// <returns>The new <see cref="GlyphVector" />.</returns>
    public static GlyphVector Scale(GlyphVector src, float scale)
    {
        return Transform(src, Matrix3x2.CreateScale(scale));
    }

    /// <summary>
    ///     Scales a glyph vector uniformly by a specified scale.
    /// </summary>
    /// <param name="src">The glyph vector to translate.</param>
    /// <param name="scales">The vector scale to use.</param>
    /// <returns>The new <see cref="GlyphVector" />.</returns>
    public static GlyphVector Scale(GlyphVector src, Vector2 scales)
    {
        return Transform(src, Matrix3x2.CreateScale(scales));
    }

    /// <summary>
    ///     Translates a glyph vector by a specified x and y coordinates.
    /// </summary>
    /// <param name="src">The glyph vector to translate.</param>
    /// <param name="dx">The x-offset.</param>
    /// <param name="dy">The y-offset.</param>
    /// <returns>The new <see cref="GlyphVector" />.</returns>
    public static GlyphVector Translate(GlyphVector src, float dx, float dy)
    {
        return Transform(src, Matrix3x2.CreateTranslation(dx, dy));
    }

    /// <summary>
    ///     Appends the second glyph vector's control points to the first.
    /// </summary>
    /// <param name="first">The first glyph vector.</param>
    /// <param name="second">The second glyph vector.</param>
    /// <param name="compositeBounds">The bounds for the composite glyph.</param>
    /// <returns>The new <see cref="GlyphVector" />.</returns>
    public static GlyphVector Append(GlyphVector first, GlyphVector second, Bounds compositeBounds)
    {
        if (!first.HasValue()) return second;

        List<GlyphTableEntry> entries = new(first.entries.Count + second.entries.Count);
        for (var i = 0; i < first.entries.Count; i++) entries.Add(first.entries[i]);

        for (var i = 0; i < second.entries.Count; i++) entries.Add(second.entries[i]);

        return new GlyphVector(entries, compositeBounds);
    }

    /// <summary>
    ///     Applies True Type hinting to the specified glyph vector.
    /// </summary>
    /// <param name="hintingMode">The hinting mode.</param>
    /// <param name="glyph">The glyph vector to hint.</param>
    /// <param name="interpreter">The True Type interpreter.</param>
    /// <param name="pp1">The first phantom point.</param>
    /// <param name="pp2">The second phantom point.</param>
    /// <param name="pp3">The third phantom point.</param>
    /// <param name="pp4">The fourth phantom point.</param>
    public static void Hint(HintingMode hintingMode, ref GlyphVector glyph, TrueTypeInterpreter interpreter,
        Vector2 pp1, Vector2 pp2, Vector2 pp3, Vector2 pp4)
    {
        if (hintingMode == HintingMode.None) return;

        for (var i = 0; i < glyph.entries.Count; i++)
        {
            var entry = glyph.entries[i];

            // TODO: Figure out a way to pool this.
            var controlPoints = new Vector2[entry.ControlPoints.Length + 4];
            controlPoints[controlPoints.Length - 4] = pp1;
            controlPoints[controlPoints.Length - 3] = pp2;
            controlPoints[controlPoints.Length - 2] = pp3;
            controlPoints[controlPoints.Length - 1] = pp4;
            entry.ControlPoints.AsSpan().CopyTo(controlPoints.AsSpan());

            // To keep vertical hinting but discard horizontal we simply cheat the hinter.
            // We stretch the symbols horizontally so that the hinter would have to work with high accuracy in the X direction.
            if (hintingMode == HintingMode.HintY) ScaleX(controlPoints, 1000F);

            var withPhantomPoints = new GlyphTableEntry(controlPoints, entry.OnCurves, entry.EndPoints, entry.Bounds,
                entry.Instructions);
            interpreter.HintGlyph(withPhantomPoints);

            if (hintingMode == HintingMode.HintY) ScaleX(controlPoints, 1F / 1000F);

            controlPoints.AsSpan(0, entry.ControlPoints.Length).CopyTo(entry.ControlPoints.AsSpan());
            glyph.entries[i] = entry;
        }
    }

    private static void ScaleX(Span<Vector2> controlPoints, float scale)
    {
        var remainder = 0;
#if SUPPORTS_RUNTIME_INTRINSICS
            if (Avx.IsSupported)
            {
                int length = controlPoints.Length;
                remainder = length - (ModuloP2(length * 2, Vector256<float>.Count) / 2);
                Span<Vector256<float>> vectors = MemoryMarshal.Cast<Vector2, Vector256<float>>(controlPoints);
                Vector256<float> mutiplier = Avx.UnpackLow(Vector256.Create(scale), Vector256.Create(1F));
                for (int i = 0; i < vectors.Length; i++)
                {
                    vectors[i] = Avx.Multiply(vectors[i], mutiplier);
                }
            }
            else if (Sse.IsSupported)
            {
                int length = controlPoints.Length;
                remainder = length - (ModuloP2(length * 2, Vector128<float>.Count) / 2);
                Span<Vector128<float>> vectors = MemoryMarshal.Cast<Vector2, Vector128<float>>(controlPoints);
                Vector128<float> mutiplier = Sse.UnpackLow(Vector128.Create(scale), Vector128.Create(1F));
                for (int i = 0; i < vectors.Length; i++)
                {
                    vectors[i] = Sse.Multiply(vectors[i], mutiplier);
                }
            }
#endif
        Vector2 v = new(scale, 1F);
        for (var i = remainder; i < controlPoints.Length; i++) controlPoints[i] *= v;
    }

    /// <summary>
    ///     Creates a new glyph vector that is a deep copy of the specified instance.
    /// </summary>
    /// <param name="src">The source glyph vector to copy.</param>
    /// <returns>The cloned <see cref="GlyphVector" />.</returns>
    public static GlyphVector DeepClone(GlyphVector src)
    {
        List<GlyphTableEntry> entries = new(src.entries.Count);
        for (var i = 0; i < src.entries.Count; i++) entries.Add(GlyphTableEntry.DeepClone(src.entries[i]));

        return new GlyphVector(entries, src.compositeBounds);
    }

    /// <summary>
    ///     Returns a value indicating whether the current instance is empty.
    /// </summary>
    /// <returns>The <see cref="bool" /> indicating the result.</returns>
    public bool HasValue()
    {
        return entries?[0].ControlPoints.Length > 0;
    }

    /// <summary>
    ///     Returns the bounds for the current instance.
    /// </summary>
    /// <returns>The <see cref="GetBounds" />.</returns>
    public Bounds GetBounds()
    {
        return compositeBounds != default ? compositeBounds : entries[0].Bounds;
    }

    /// <summary>
    ///     Returns the result of combining each glyph within this instance as a single outline.
    /// </summary>
    /// <returns>The <see cref="GlyphOutline" />.</returns>
    public GlyphOutline GetOutline()
    {
        List<Vector2> controlPoints = new();
        List<bool> onCurves = new();
        List<ushort> endPoints = new();

        for (var resultIndex = 0; resultIndex < entries.Count; resultIndex++)
        {
            var glyph = entries[resultIndex];
            var pointCount = glyph.PointCount;
            var endPointOffset = (ushort)controlPoints.Count;
            for (var i = 0; i < pointCount; i++)
            {
                controlPoints.Add(glyph.ControlPoints[i]);
                onCurves.Add(glyph.OnCurves[i]);
            }

            foreach (var p in glyph.EndPoints) endPoints.Add((ushort)(p + endPointOffset));
        }

        return new GlyphOutline(controlPoints.ToArray(), endPoints.ToArray(), onCurves.ToArray());
    }

    /// <summary>
    ///     Returns a new instance with the composite bounds set to the specified value
    /// </summary>
    /// <param name="src">The src glyph vector.</param>
    /// <param name="bounds">The composite bounds.</param>
    /// <returns>The <see cref="GlyphVector" />.</returns>
    public static GlyphVector WithCompositeBounds(GlyphVector src, Bounds bounds)
    {
        return new GlyphVector(src.entries, bounds);
    }
#if SUPPORTS_RUNTIME_INTRINSICS
        /// <summary>
        /// Fast (x mod m) calculator, with the restriction that
        /// <paramref name="m"/> should be power of 2.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ModuloP2(int x, int m) => x & (m - 1);
#endif
}