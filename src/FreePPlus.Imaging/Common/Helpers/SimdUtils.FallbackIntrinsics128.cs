// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable MemberHidesStaticFromOuterClass
namespace FreePPlus.Imaging;

//was previously: namespace SixLabors.ImageSharp;

internal static partial class SimdUtils
{
    /// <summary>
    ///     Fallback implementation based on <see cref="Vector4" /> (128bit).
    ///     For <see cref="Vector4" />, efficient software fallback implementations are present,
    ///     and we hope that even mono's JIT is able to emit SIMD instructions for that type :P
    /// </summary>
    public static class FallbackIntrinsics128
    {
        /// <summary>
        ///     <see cref="ByteToNormalizedFloat" /> as many elements as possible, slicing them down (keeping the remainder).
        /// </summary>
        [MethodImpl(InliningOptions.ShortMethod)]
        internal static void ByteToNormalizedFloatReduce(
            ref ReadOnlySpan<byte> source,
            ref Span<float> dest)
        {
            DebugGuard.IsTrue(source.Length == dest.Length, nameof(source), "Input spans must be of same length!");

            var remainder = ImageMaths.Modulo4(source.Length);
            var adjustedCount = source.Length - remainder;

            if (adjustedCount > 0)
            {
                ByteToNormalizedFloat(
                    source.Slice(0, adjustedCount),
                    dest.Slice(0, adjustedCount));

                source = source.Slice(adjustedCount);
                dest = dest.Slice(adjustedCount);
            }
        }

        /// <summary>
        ///     <see cref="NormalizedFloatToByteSaturate" /> as many elements as possible, slicing them down (keeping the
        ///     remainder).
        /// </summary>
        [MethodImpl(InliningOptions.ShortMethod)]
        internal static void NormalizedFloatToByteSaturateReduce(
            ref ReadOnlySpan<float> source,
            ref Span<byte> dest)
        {
            DebugGuard.IsTrue(source.Length == dest.Length, nameof(source), "Input spans must be of same length!");

            var remainder = ImageMaths.Modulo4(source.Length);
            var adjustedCount = source.Length - remainder;

            if (adjustedCount > 0)
            {
                NormalizedFloatToByteSaturate(
                    source.Slice(0, adjustedCount),
                    dest.Slice(0, adjustedCount));

                source = source.Slice(adjustedCount);
                dest = dest.Slice(adjustedCount);
            }
        }

        /// <summary>
        ///     Implementation of <see cref="SimdUtils.ByteToNormalizedFloat" /> using <see cref="Vector4" />.
        /// </summary>
        [MethodImpl(InliningOptions.ColdPath)]
        internal static void ByteToNormalizedFloat(ReadOnlySpan<byte> source, Span<float> dest)
        {
            VerifySpanInput(source, dest, 4);

            var count = dest.Length / 4;
            if (count == 0) return;

            ref var sBase = ref Unsafe.As<byte, ByteVector4>(ref MemoryMarshal.GetReference(source));
            ref var dBase = ref Unsafe.As<float, Vector4>(ref MemoryMarshal.GetReference(dest));

            const float Scale = 1f / 255f;
            Vector4 d = default;

            for (var i = 0; i < count; i++)
            {
                ref var s = ref Unsafe.Add(ref sBase, i);
                d.X = s.X;
                d.Y = s.Y;
                d.Z = s.Z;
                d.W = s.W;
                d *= Scale;
                Unsafe.Add(ref dBase, i) = d;
            }
        }

        /// <summary>
        ///     Implementation of <see cref="SimdUtils.NormalizedFloatToByteSaturate" /> using <see cref="Vector4" />.
        /// </summary>
        [MethodImpl(InliningOptions.ColdPath)]
        internal static void NormalizedFloatToByteSaturate(
            ReadOnlySpan<float> source,
            Span<byte> dest)
        {
            VerifySpanInput(source, dest, 4);

            var count = source.Length / 4;
            if (count == 0) return;

            ref var sBase = ref Unsafe.As<float, Vector4>(ref MemoryMarshal.GetReference(source));
            ref var dBase = ref Unsafe.As<byte, ByteVector4>(ref MemoryMarshal.GetReference(dest));

            var half = new Vector4(0.5f);
            var maxBytes = new Vector4(255f);

            for (var i = 0; i < count; i++)
            {
                var s = Unsafe.Add(ref sBase, i);
                s *= maxBytes;
                s += half;
                s = Vector4Utilities.FastClamp(s, Vector4.Zero, maxBytes);

                ref var d = ref Unsafe.Add(ref dBase, i);
                d.X = (byte)s.X;
                d.Y = (byte)s.Y;
                d.Z = (byte)s.Z;
                d.W = (byte)s.W;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ByteVector4
        {
            public byte X;
            public byte Y;
            public byte Z;
            public byte W;
        }
    }
}