// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;

namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder.ColorConverters;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder.ColorConverters;

internal abstract partial class JpegColorConverter
{
    internal sealed class FromCmyk : JpegColorConverter
    {
        public FromCmyk(int precision)
            : base(JpegColorSpace.Cmyk, precision) { }

        public override void ConvertToRgba(in ComponentValues values, Span<Vector4> result)
        {
            // TODO: We can optimize a lot here with Vector<float> and SRCS.Unsafe()!
            var cVals = values.Component0;
            var mVals = values.Component1;
            var yVals = values.Component2;
            var kVals = values.Component3;

            var v = new Vector4(0, 0, 0, 1F);

            var maximum = 1 / MaximumValue;
            var scale = new Vector4(maximum, maximum, maximum, 1F);

            for (var i = 0; i < result.Length; i++)
            {
                var c = cVals[i];
                var m = mVals[i];
                var y = yVals[i];
                var k = kVals[i] / MaximumValue;

                v.X = c * k;
                v.Y = m * k;
                v.Z = y * k;
                v.W = 1F;

                v *= scale;

                result[i] = v;
            }
        }
    }
}