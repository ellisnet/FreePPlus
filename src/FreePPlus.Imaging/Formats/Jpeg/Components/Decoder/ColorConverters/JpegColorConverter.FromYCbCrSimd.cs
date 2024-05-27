// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.Tuples;

namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder.ColorConverters;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder.ColorConverters;

internal abstract partial class JpegColorConverter
{
    internal sealed class FromYCbCrSimd : JpegColorConverter
    {
        public FromYCbCrSimd(int precision)
            : base(JpegColorSpace.YCbCr, precision) { }

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
            DebugGuard.IsTrue(result.Length % 8 == 0, nameof(result), "result.Length should be divisible by 8!");

            ref var yBase =
                ref Unsafe.As<float, Vector4Pair>(ref MemoryMarshal.GetReference(values.Component0));
            ref var cbBase =
                ref Unsafe.As<float, Vector4Pair>(ref MemoryMarshal.GetReference(values.Component1));
            ref var crBase =
                ref Unsafe.As<float, Vector4Pair>(ref MemoryMarshal.GetReference(values.Component2));

            ref var resultBase =
                ref Unsafe.As<Vector4, Vector4Octet>(ref MemoryMarshal.GetReference(result));

            var chromaOffset = new Vector4(-halfValue);

            // Walking 8 elements at one step:
            var n = result.Length / 8;

            for (var i = 0; i < n; i++)
            {
                // y = yVals[i];
                var y = Unsafe.Add(ref yBase, i);

                // cb = cbVals[i] - halfValue);
                var cb = Unsafe.Add(ref cbBase, i);
                cb.AddInplace(chromaOffset);

                // cr = crVals[i] - halfValue;
                var cr = Unsafe.Add(ref crBase, i);
                cr.AddInplace(chromaOffset);

                // r = y + (1.402F * cr);
                var r = y;
                var tmp = cr;
                tmp.MultiplyInplace(1.402F);
                r.AddInplace(ref tmp);

                // g = y - (0.344136F * cb) - (0.714136F * cr);
                var g = y;
                tmp = cb;
                tmp.MultiplyInplace(-0.344136F);
                g.AddInplace(ref tmp);
                tmp = cr;
                tmp.MultiplyInplace(-0.714136F);
                g.AddInplace(ref tmp);

                // b = y + (1.772F * cb);
                var b = y;
                tmp = cb;
                tmp.MultiplyInplace(1.772F);
                b.AddInplace(ref tmp);

                if (Vector<float>.Count == 4)
                {
                    // TODO: Find a way to properly run & test this path on AVX2 PC-s! (Have I already mentioned that Vector<T> is terrible?)
                    r.RoundAndDownscalePreVector8(maxValue);
                    g.RoundAndDownscalePreVector8(maxValue);
                    b.RoundAndDownscalePreVector8(maxValue);
                }
                else if (SimdUtils.HasVector8)
                {
                    r.RoundAndDownscaleVector8(maxValue);
                    g.RoundAndDownscaleVector8(maxValue);
                    b.RoundAndDownscaleVector8(maxValue);
                }
                else
                {
                    // TODO: Run fallback scalar code here
                    // However, no issues expected before someone implements this: https://github.com/dotnet/coreclr/issues/12007
                    JpegThrowHelper.ThrowNotImplementedException("Your CPU architecture is too modern!");
                }

                // Collect (r0,r1...r8) (g0,g1...g8) (b0,b1...b8) vector values in the expected (r0,g0,g1,1), (r1,g1,g2,1) ... order:
                ref var destination = ref Unsafe.Add(ref resultBase, i);
                destination.Pack(ref r, ref g, ref b);
            }
        }
    }
}