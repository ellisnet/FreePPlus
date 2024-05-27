// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Tuples;

// ReSharper disable ImpureMethodCallOnReadonlyValueField
namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder.ColorConverters;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder.ColorConverters;

internal abstract partial class JpegColorConverter
{
    internal sealed class FromYCbCrSimdVector8 : JpegColorConverter
    {
        public FromYCbCrSimdVector8(int precision)
            : base(JpegColorSpace.YCbCr, precision) { }

        public static bool IsAvailable => Vector.IsHardwareAccelerated && SimdUtils.HasVector8;

        public override void ConvertToRgba(in ComponentValues values, Span<Vector4> result)
        {
            var remainder = result.Length % 8;
            var simdCount = result.Length - remainder;
            if (simdCount > 0)
                ConvertCore(values.Slice(0, simdCount), result.Slice(0, simdCount), MaximumValue, HalfValue);

            FromYCbCrBasic.ConvertCore(values.Slice(simdCount, remainder), result.Slice(simdCount, remainder),
                MaximumValue, HalfValue);
        }

        /// <summary>
        ///     SIMD convert using buffers of sizes divisible by 8.
        /// </summary>
        internal static void ConvertCore(in ComponentValues values, Span<Vector4> result, float maxValue,
            float halfValue)
        {
            // This implementation is actually AVX specific.
            // An AVX register is capable of storing 8 float-s.
            if (!IsAvailable)
                throw new InvalidOperationException(
                    "JpegColorConverter.FromYCbCrSimd256 can be used only on architecture having 256 byte floating point SIMD registers!");

            ref var yBase =
                ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(values.Component0));
            ref var cbBase =
                ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(values.Component1));
            ref var crBase =
                ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(values.Component2));

            ref var resultBase =
                ref Unsafe.As<Vector4, Vector4Octet>(ref MemoryMarshal.GetReference(result));

            var chromaOffset = new Vector<float>(-halfValue);

            // Walking 8 elements at one step:
            var n = result.Length / 8;

            Vector4Pair rr = default;
            Vector4Pair gg = default;
            Vector4Pair bb = default;

            ref var rrRefAsVector = ref Unsafe.As<Vector4Pair, Vector<float>>(ref rr);
            ref var ggRefAsVector = ref Unsafe.As<Vector4Pair, Vector<float>>(ref gg);
            ref var bbRefAsVector = ref Unsafe.As<Vector4Pair, Vector<float>>(ref bb);

            var scale = new Vector<float>(1 / maxValue);

            for (var i = 0; i < n; i++)
            {
                // y = yVals[i];
                // cb = cbVals[i] - 128F;
                // cr = crVals[i] - 128F;
                var y = Unsafe.Add(ref yBase, i);
                var cb = Unsafe.Add(ref cbBase, i) + chromaOffset;
                var cr = Unsafe.Add(ref crBase, i) + chromaOffset;

                // r = y + (1.402F * cr);
                // g = y - (0.344136F * cb) - (0.714136F * cr);
                // b = y + (1.772F * cb);
                // Adding & multiplying 8 elements at one time:
                var r = y + cr * new Vector<float>(1.402F);
                var g = y - cb * new Vector<float>(0.344136F) - cr * new Vector<float>(0.714136F);
                var b = y + cb * new Vector<float>(1.772F);

                r = r.FastRound();
                g = g.FastRound();
                b = b.FastRound();
                r *= scale;
                g *= scale;
                b *= scale;

                rrRefAsVector = r;
                ggRefAsVector = g;
                bbRefAsVector = b;

                // Collect (r0,r1...r8) (g0,g1...g8) (b0,b1...b8) vector values in the expected (r0,g0,g1,1), (r1,g1,g2,1) ... order:
                ref var destination = ref Unsafe.Add(ref resultBase, i);
                destination.Pack(ref rr, ref gg, ref bb);
            }
        }
    }
}