// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;

namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder.ColorConverters;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder.ColorConverters;

internal abstract partial class JpegColorConverter
{
    internal sealed class FromYccK : JpegColorConverter
    {
        public FromYccK(int precision)
            : base(JpegColorSpace.Ycck, precision) { }

        public override void ConvertToRgba(in ComponentValues values, Span<Vector4> result)
        {
            // TODO: We can optimize a lot here with Vector<float> and SRCS.Unsafe()!
            var yVals = values.Component0;
            var cbVals = values.Component1;
            var crVals = values.Component2;
            var kVals = values.Component3;

            var v = new Vector4(0, 0, 0, 1F);

            var maximum = 1 / MaximumValue;
            var scale = new Vector4(maximum, maximum, maximum, 1F);

            for (var i = 0; i < result.Length; i++)
            {
                var y = yVals[i];
                var cb = cbVals[i] - HalfValue;
                var cr = crVals[i] - HalfValue;
                var k = kVals[i] / MaximumValue;

                v.X = (MaximumValue - MathF.Round(y + 1.402F * cr, MidpointRounding.AwayFromZero)) * k;
                v.Y = (MaximumValue - MathF.Round(y - 0.344136F * cb - 0.714136F * cr, MidpointRounding.AwayFromZero)) *
                      k;
                v.Z = (MaximumValue - MathF.Round(y + 1.772F * cb, MidpointRounding.AwayFromZero)) * k;
                v.W = 1F;

                v *= scale;

                result[i] = v;
            }
        }
    }
}