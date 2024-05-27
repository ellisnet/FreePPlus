// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable InconsistentNaming
namespace FreePPlus.Imaging.Formats.Jpeg.Components;

//was previously:namespace SixLabors.ImageSharp.Formats.Jpeg.Components;

/// <summary>
///     Represents a Jpeg block with <see cref="float" /> coefficients.
/// </summary>
internal partial struct Block8x8F : IEquatable<Block8x8F>
{
    /// <summary>
    ///     A number of scalar coefficients in a <see cref="Block8x8F" />
    /// </summary>
    public const int Size = 64;

#pragma warning disable SA1600 // ElementsMustBeDocumented
    public Vector4 V0L;
    public Vector4 V0R;

    public Vector4 V1L;
    public Vector4 V1R;

    public Vector4 V2L;
    public Vector4 V2R;

    public Vector4 V3L;
    public Vector4 V3R;

    public Vector4 V4L;
    public Vector4 V4R;

    public Vector4 V5L;
    public Vector4 V5R;

    public Vector4 V6L;
    public Vector4 V6R;

    public Vector4 V7L;
    public Vector4 V7R;
#pragma warning restore SA1600 // ElementsMustBeDocumented

    private static readonly Vector4 NegativeOne = new(-1);
    private static readonly Vector4 Offset = new(.5F);

    /// <summary>
    ///     Get/Set scalar elements at a given index
    /// </summary>
    /// <param name="idx">The index</param>
    /// <returns>The float value at the specified index</returns>
    public float this[int idx]
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        get
        {
            GuardBlockIndex(idx);
            ref var selfRef = ref Unsafe.As<Block8x8F, float>(ref this);
            return Unsafe.Add(ref selfRef, idx);
        }

        [MethodImpl(InliningOptions.ShortMethod)]
        set
        {
            GuardBlockIndex(idx);
            ref var selfRef = ref Unsafe.As<Block8x8F, float>(ref this);
            Unsafe.Add(ref selfRef, idx) = value;
        }
    }

    public float this[int x, int y]
    {
        get => this[y * 8 + x];
        set => this[y * 8 + x] = value;
    }

    public static Block8x8F operator *(Block8x8F block, float value)
    {
        var result = block;
        for (var i = 0; i < Size; i++)
        {
            var val = result[i];
            val *= value;
            result[i] = val;
        }

        return result;
    }

    public static Block8x8F operator /(Block8x8F block, float value)
    {
        var result = block;
        for (var i = 0; i < Size; i++)
        {
            var val = result[i];
            val /= value;
            result[i] = val;
        }

        return result;
    }

    public static Block8x8F operator +(Block8x8F block, float value)
    {
        var result = block;
        for (var i = 0; i < Size; i++)
        {
            var val = result[i];
            val += value;
            result[i] = val;
        }

        return result;
    }

    public static Block8x8F operator -(Block8x8F block, float value)
    {
        var result = block;
        for (var i = 0; i < Size; i++)
        {
            var val = result[i];
            val -= value;
            result[i] = val;
        }

        return result;
    }

    public static Block8x8F Load(Span<float> data)
    {
        Block8x8F result = default;
        result.LoadFrom(data);
        return result;
    }

    public static Block8x8F Load(Span<int> data)
    {
        Block8x8F result = default;
        result.LoadFrom(data);
        return result;
    }

    /// <summary>
    ///     Fill the block with defaults (zeroes).
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void Clear()
    {
        // The cheapest way to do this in C#:
        this = default;
    }

    /// <summary>
    ///     Load raw 32bit floating point data from source.
    /// </summary>
    /// <param name="source">Source</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void LoadFrom(Span<float> source)
    {
        ref var s = ref Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(source));
        ref var d = ref Unsafe.As<Block8x8F, byte>(ref this);

        Unsafe.CopyBlock(ref d, ref s, Size * sizeof(float));
    }

    /// <summary>
    ///     Load raw 32bit floating point data from source.
    /// </summary>
    /// <param name="blockPtr">Block pointer</param>
    /// <param name="source">Source</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static unsafe void LoadFrom(Block8x8F* blockPtr, Span<float> source)
    {
        blockPtr->LoadFrom(source);
    }

    /// <summary>
    ///     Load raw 32bit floating point data from source
    /// </summary>
    /// <param name="source">Source</param>
    public unsafe void LoadFrom(Span<int> source)
    {
        fixed (Vector4* ptr = &V0L)
        {
            var fp = (float*)ptr;
            for (var i = 0; i < Size; i++) fp[i] = source[i];
        }
    }

    /// <summary>
    ///     Copy raw 32bit floating point data to dest,
    /// </summary>
    /// <param name="dest">Destination</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void ScaledCopyTo(Span<float> dest)
    {
        ref var d = ref Unsafe.As<float, byte>(ref MemoryMarshal.GetReference(dest));
        ref var s = ref Unsafe.As<Block8x8F, byte>(ref this);

        Unsafe.CopyBlock(ref d, ref s, Size * sizeof(float));
    }

    /// <summary>
    ///     Convert scalars to byte-s and copy to dest,
    /// </summary>
    /// <param name="blockPtr">Pointer to block</param>
    /// <param name="dest">Destination</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static unsafe void ScaledCopyTo(Block8x8F* blockPtr, Span<byte> dest)
    {
        var fPtr = (float*)blockPtr;
        for (var i = 0; i < Size; i++)
        {
            dest[i] = (byte)*fPtr;
            fPtr++;
        }
    }

    /// <summary>
    ///     Copy raw 32bit floating point data to dest.
    /// </summary>
    /// <param name="blockPtr">The block pointer.</param>
    /// <param name="dest">The destination.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static unsafe void ScaledCopyTo(Block8x8F* blockPtr, Span<float> dest)
    {
        blockPtr->ScaledCopyTo(dest);
    }

    /// <summary>
    ///     Copy raw 32bit floating point data to dest
    /// </summary>
    /// <param name="dest">Destination</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public unsafe void ScaledCopyTo(float[] dest)
    {
        fixed (void* ptr = &V0L)
        {
            Marshal.Copy((IntPtr)ptr, dest, 0, Size);
        }
    }

    /// <summary>
    ///     Copy raw 32bit floating point data to dest
    /// </summary>
    /// <param name="dest">Destination</param>
    public unsafe void ScaledCopyTo(Span<int> dest)
    {
        fixed (Vector4* ptr = &V0L)
        {
            var fp = (float*)ptr;
            for (var i = 0; i < Size; i++) dest[i] = (int)fp[i];
        }
    }

    public float[] ToArray()
    {
        var result = new float[Size];
        ScaledCopyTo(result);
        return result;
    }

    /// <summary>
    ///     Multiply all elements of the block.
    /// </summary>
    /// <param name="value">The value to multiply by.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void MultiplyInplace(float value)
    {
        V0L *= value;
        V0R *= value;
        V1L *= value;
        V1R *= value;
        V2L *= value;
        V2R *= value;
        V3L *= value;
        V3R *= value;
        V4L *= value;
        V4R *= value;
        V5L *= value;
        V5R *= value;
        V6L *= value;
        V6R *= value;
        V7L *= value;
        V7R *= value;
    }

    /// <summary>
    ///     Multiply all elements of the block by the corresponding elements of 'other'.
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void MultiplyInplace(ref Block8x8F other)
    {
        V0L *= other.V0L;
        V0R *= other.V0R;
        V1L *= other.V1L;
        V1R *= other.V1R;
        V2L *= other.V2L;
        V2R *= other.V2R;
        V3L *= other.V3L;
        V3R *= other.V3R;
        V4L *= other.V4L;
        V4R *= other.V4R;
        V5L *= other.V5L;
        V5R *= other.V5R;
        V6L *= other.V6L;
        V6R *= other.V6R;
        V7L *= other.V7L;
        V7R *= other.V7R;
    }

    /// <summary>
    ///     Adds a vector to all elements of the block.
    /// </summary>
    /// <param name="diff">The added vector</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public void AddToAllInplace(Vector4 diff)
    {
        V0L += diff;
        V0R += diff;
        V1L += diff;
        V1R += diff;
        V2L += diff;
        V2R += diff;
        V3L += diff;
        V3R += diff;
        V4L += diff;
        V4R += diff;
        V5L += diff;
        V5R += diff;
        V6L += diff;
        V6R += diff;
        V7L += diff;
        V7R += diff;
    }

    /// <summary>
    ///     Quantize the block.
    /// </summary>
    /// <param name="blockPtr">The block pointer.</param>
    /// <param name="qtPtr">The qt pointer.</param>
    /// <param name="unzigPtr">Unzig pointer</param>
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void DequantizeBlock(Block8x8F* blockPtr, Block8x8F* qtPtr, byte* unzigPtr)
    {
        var b = (float*)blockPtr;
        var qtp = (float*)qtPtr;
        for (var qtIndex = 0; qtIndex < Size; qtIndex++)
        {
            var blockIndex = unzigPtr[qtIndex];
            var unzigPos = b + blockIndex;

            var val = *unzigPos;
            val *= qtp[qtIndex];
            *unzigPos = val;
        }
    }

    /// <summary>
    ///     Quantize 'block' into 'dest' using the 'qt' quantization table:
    ///     Unzig the elements of block into dest, while dividing them by elements of qt and "pre-rounding" the values.
    ///     To finish the rounding it's enough to (int)-cast these values.
    /// </summary>
    /// <param name="block">Source block</param>
    /// <param name="dest">Destination block</param>
    /// <param name="qt">The quantization table</param>
    /// <param name="unzigPtr">Pointer to elements of <see cref="ZigZag" /></param>
    public static unsafe void Quantize(
        Block8x8F* block,
        Block8x8F* dest,
        Block8x8F* qt,
        byte* unzigPtr)
    {
        var s = (float*)block;
        var d = (float*)dest;

        for (var zig = 0; zig < Size; zig++) d[zig] = s[unzigPtr[zig]];

        DivideRoundAll(ref *dest, ref *qt);
    }

    /// <summary>
    ///     Scales the 16x16 region represented by the 4 source blocks to the 8x8 DST block.
    /// </summary>
    /// <param name="destination">The destination block.</param>
    /// <param name="source">The source block.</param>
    public static unsafe void Scale16X16To8X8(Block8x8F* destination, Block8x8F* source)
    {
        var d = (float*)destination;
        for (var i = 0; i < 4; i++)
        {
            var dstOff = ((i & 2) << 4) | ((i & 1) << 2);

            var iSource = (float*)(source + i);

            for (var y = 0; y < 4; y++)
            for (var x = 0; x < 4; x++)
            {
                var j = 16 * y + 2 * x;
                var sum = iSource[j] + iSource[j + 1] + iSource[j + 8] + iSource[j + 9];
                d[8 * y + x + dstOff] = (sum + 2) / 4;
            }
        }
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static void DivideRoundAll(ref Block8x8F a, ref Block8x8F b)
    {
        a.V0L = DivideRound(a.V0L, b.V0L);
        a.V0R = DivideRound(a.V0R, b.V0R);
        a.V1L = DivideRound(a.V1L, b.V1L);
        a.V1R = DivideRound(a.V1R, b.V1R);
        a.V2L = DivideRound(a.V2L, b.V2L);
        a.V2R = DivideRound(a.V2R, b.V2R);
        a.V3L = DivideRound(a.V3L, b.V3L);
        a.V3R = DivideRound(a.V3R, b.V3R);
        a.V4L = DivideRound(a.V4L, b.V4L);
        a.V4R = DivideRound(a.V4R, b.V4R);
        a.V5L = DivideRound(a.V5L, b.V5L);
        a.V5R = DivideRound(a.V5R, b.V5R);
        a.V6L = DivideRound(a.V6L, b.V6L);
        a.V6R = DivideRound(a.V6R, b.V6R);
        a.V7L = DivideRound(a.V7L, b.V7L);
        a.V7R = DivideRound(a.V7R, b.V7R);
    }

    public void RoundInto(ref Block8x8 dest)
    {
        for (var i = 0; i < Size; i++)
        {
            var val = this[i];
            if (val < 0)
                val -= 0.5f;
            else
                val += 0.5f;

            dest[i] = (short)val;
        }
    }

    public Block8x8 RoundAsInt16Block()
    {
        Block8x8 result = default;
        RoundInto(ref result);
        return result;
    }

    /// <summary>
    ///     Level shift by +maximum/2, clip to [0..maximum], and round all the values in the block.
    /// </summary>
    public void NormalizeColorsAndRoundInplace(float maximum)
    {
        if (SimdUtils.HasVector8)
        {
            NormalizeColorsAndRoundInplaceVector8(maximum);
        }
        else
        {
            NormalizeColorsInplace(maximum);
            RoundInplace();
        }
    }

    /// <summary>
    ///     Rounds all values in the block.
    /// </summary>
    public void RoundInplace()
    {
        for (var i = 0; i < Size; i++) this[i] = MathF.Round(this[i]);
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public void LoadFrom(ref Block8x8 source)
    {
#if SUPPORTS_EXTENDED_INTRINSICS
            if (SimdUtils.HasVector8)
            {
                this.LoadFromInt16ExtendedAvx2(ref source);
                return;
            }
#endif
        LoadFromInt16Scalar(ref source);
    }

    /// <summary>
    ///     Loads values from <paramref name="source" /> using extended AVX2 intrinsics.
    /// </summary>
    /// <param name="source">The source <see cref="Block8x8" /></param>
    public void LoadFromInt16ExtendedAvx2(ref Block8x8 source)
    {
        DebugGuard.IsTrue(
            SimdUtils.HasVector8,
            "LoadFromUInt16ExtendedAvx2 only works on AVX2 compatible architecture!");

        ref var sRef = ref Unsafe.As<Block8x8, Vector<short>>(ref source);
        ref var dRef = ref Unsafe.As<Block8x8F, Vector<float>>(ref this);

        // Vector<ushort>.Count == 16 on AVX2
        // We can process 2 block rows in a single step
        SimdUtils.ExtendedIntrinsics.ConvertToSingle(sRef, out var top, out var bottom);
        dRef = top;
        Unsafe.Add(ref dRef, 1) = bottom;

        SimdUtils.ExtendedIntrinsics.ConvertToSingle(Unsafe.Add(ref sRef, 1), out top, out bottom);
        Unsafe.Add(ref dRef, 2) = top;
        Unsafe.Add(ref dRef, 3) = bottom;

        SimdUtils.ExtendedIntrinsics.ConvertToSingle(Unsafe.Add(ref sRef, 2), out top, out bottom);
        Unsafe.Add(ref dRef, 4) = top;
        Unsafe.Add(ref dRef, 5) = bottom;

        SimdUtils.ExtendedIntrinsics.ConvertToSingle(Unsafe.Add(ref sRef, 3), out top, out bottom);
        Unsafe.Add(ref dRef, 6) = top;
        Unsafe.Add(ref dRef, 7) = bottom;
    }

    /// <inheritdoc />
    public bool Equals(Block8x8F other)
    {
        return V0L == other.V0L
               && V0R == other.V0R
               && V1L == other.V1L
               && V1R == other.V1R
               && V2L == other.V2L
               && V2R == other.V2R
               && V3L == other.V3L
               && V3R == other.V3R
               && V4L == other.V4L
               && V4R == other.V4R
               && V5L == other.V5L
               && V5R == other.V5R
               && V6L == other.V6L
               && V6R == other.V6R
               && V7L == other.V7L
               && V7R == other.V7R;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < Size - 1; i++)
        {
            sb.Append(this[i]);
            sb.Append(',');
        }

        sb.Append(this[Size - 1]);

        sb.Append(']');
        return sb.ToString();
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static Vector<float> NormalizeAndRound(Vector<float> row, Vector<float> off, Vector<float> max)
    {
        row += off;
        row = Vector.Max(row, Vector<float>.Zero);
        row = Vector.Min(row, max);
        return row.FastRound();
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static Vector4 DivideRound(Vector4 dividend, Vector4 divisor)
    {
        // sign(dividend) = max(min(dividend, 1), -1)
        var sign = Vector4Utilities.FastClamp(dividend, NegativeOne, Vector4.One);

        // AlmostRound(dividend/divisor) = dividend/divisor + 0.5*sign(dividend)
        return dividend / divisor + sign * Offset;
    }

    [Conditional("DEBUG")]
    private static void GuardBlockIndex(int idx)
    {
        DebugGuard.MustBeLessThan(idx, Size, nameof(idx));
        DebugGuard.MustBeGreaterThanOrEqualTo(idx, 0, nameof(idx));
    }
}