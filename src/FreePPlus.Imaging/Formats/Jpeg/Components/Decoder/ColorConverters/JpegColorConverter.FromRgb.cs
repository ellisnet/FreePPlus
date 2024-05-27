// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Numerics;

namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder.ColorConverters;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder.ColorConverters;

internal abstract partial class JpegColorConverter
{
    internal sealed class FromRgb : JpegColorConverter
    {
        public FromRgb(int precision)
            : base(JpegColorSpace.RGB, precision) { }

        public override void ConvertToRgba(in ComponentValues values, Span<Vector4> result)
        {
            // TODO: We can optimize a lot here with Vector<float> and SRCS.Unsafe()!
            var rVals = values.Component0;
            var gVals = values.Component1;
            var bVals = values.Component2;

            var v = new Vector4(0, 0, 0, 1);

            var maximum = 1 / MaximumValue;
            var scale = new Vector4(maximum, maximum, maximum, 1F);

            for (var i = 0; i < result.Length; i++)
            {
                var r = rVals[i];
                var g = gVals[i];
                var b = bVals[i];

                v.X = r;
                v.Y = g;
                v.Z = b;

                v *= scale;

                result[i] = v;
            }
        }
    }
}