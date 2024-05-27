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
    ///     Implementation methods based on newer <see cref="Vector{T}" /> API-s (Vector.Widen, Vector.Narrow,
    ///     Vector.ConvertTo*).
    ///     Only accelerated only on RyuJIT having dotnet/coreclr#10662 merged (.NET Core 2.1+ .NET 4.7.2+)
    ///     See:
    ///     https://github.com/dotnet/coreclr/pull/10662
    ///     API Proposal:
    ///     https://github.com/dotnet/corefx/issues/15957
    /// </summary>
    public static class ExtendedIntrinsics
    {
        public static bool IsAvailable { get; } =
#if SUPPORTS_EXTENDED_INTRINSICS
                Vector.IsHardwareAccelerated;
#else
            false;
#endif

        /// <summary>
        ///     Widen and convert a vector of <see cref="short" /> values into 2 vectors of <see cref="float" />-s.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ConvertToSingle(
            Vector<short> source,
            out Vector<float> dest1,
            out Vector<float> dest2)
        {
            Vector.Widen(source, out var i1, out var i2);
            dest1 = Vector.ConvertToSingle(i1);
            dest2 = Vector.ConvertToSingle(i2);
        }

        /// <summary>
        ///     <see cref="ByteToNormalizedFloat" /> as many elements as possible, slicing them down (keeping the remainder).
        /// </summary>
        [MethodImpl(InliningOptions.ShortMethod)]
        internal static void ByteToNormalizedFloatReduce(
            ref ReadOnlySpan<byte> source,
            ref Span<float> dest)
        {
            DebugGuard.IsTrue(source.Length == dest.Length, nameof(source), "Input spans must be of same length!");

            if (!IsAvailable) return;

            var remainder = ImageMaths.ModuloP2(source.Length, Vector<byte>.Count);
            var adjustedCount = source.Length - remainder;

            if (adjustedCount > 0)
            {
                ByteToNormalizedFloat(source.Slice(0, adjustedCount), dest.Slice(0, adjustedCount));

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

            if (!IsAvailable) return;

            var remainder = ImageMaths.ModuloP2(source.Length, Vector<byte>.Count);
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
        ///     Implementation <see cref="SimdUtils.ByteToNormalizedFloat" />, which is faster on new RyuJIT runtime.
        /// </summary>
        internal static void ByteToNormalizedFloat(ReadOnlySpan<byte> source, Span<float> dest)
        {
            VerifySpanInput(source, dest, Vector<byte>.Count);

            var n = dest.Length / Vector<byte>.Count;

            ref var sourceBase = ref Unsafe.As<byte, Vector<byte>>(ref MemoryMarshal.GetReference(source));
            ref var destBase = ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(dest));

            for (var i = 0; i < n; i++)
            {
                var b = Unsafe.Add(ref sourceBase, i);

                Vector.Widen(b, out var s0, out var s1);
                Vector.Widen(s0, out var w0, out var w1);
                Vector.Widen(s1, out var w2, out var w3);

                var f0 = ConvertToSingle(w0);
                var f1 = ConvertToSingle(w1);
                var f2 = ConvertToSingle(w2);
                var f3 = ConvertToSingle(w3);

                ref var d = ref Unsafe.Add(ref destBase, i * 4);
                d = f0;
                Unsafe.Add(ref d, 1) = f1;
                Unsafe.Add(ref d, 2) = f2;
                Unsafe.Add(ref d, 3) = f3;
            }
        }

        /// <summary>
        ///     Implementation of <see cref="SimdUtils.NormalizedFloatToByteSaturate" />, which is faster on new .NET runtime.
        /// </summary>
        internal static void NormalizedFloatToByteSaturate(
            ReadOnlySpan<float> source,
            Span<byte> dest)
        {
            VerifySpanInput(source, dest, Vector<byte>.Count);

            var n = dest.Length / Vector<byte>.Count;

            ref var sourceBase =
                ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(source));
            ref var destBase = ref Unsafe.As<byte, Vector<byte>>(ref MemoryMarshal.GetReference(dest));

            for (var i = 0; i < n; i++)
            {
                ref var s = ref Unsafe.Add(ref sourceBase, i * 4);

                var f0 = s;
                var f1 = Unsafe.Add(ref s, 1);
                var f2 = Unsafe.Add(ref s, 2);
                var f3 = Unsafe.Add(ref s, 3);

                var w0 = ConvertToUInt32(f0);
                var w1 = ConvertToUInt32(f1);
                var w2 = ConvertToUInt32(f2);
                var w3 = ConvertToUInt32(f3);

                var u0 = Vector.Narrow(w0, w1);
                var u1 = Vector.Narrow(w2, w3);

                var b = Vector.Narrow(u0, u1);

                Unsafe.Add(ref destBase, i) = b;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector<uint> ConvertToUInt32(Vector<float> vf)
        {
            var maxBytes = new Vector<float>(255f);
            vf *= maxBytes;
            vf += new Vector<float>(0.5f);
            vf = Vector.Min(Vector.Max(vf, Vector<float>.Zero), maxBytes);
            var vi = Vector.ConvertToInt32(vf);
            return Vector.AsVectorUInt32(vi);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector<float> ConvertToSingle(Vector<uint> u)
        {
            var vi = Vector.AsVectorInt32(u);
            var v = Vector.ConvertToSingle(vi);
            v *= new Vector<float>(1f / 255f);
            return v;
        }
    }
}