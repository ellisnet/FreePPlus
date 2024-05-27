// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Memory;

//was previously: namespace SixLabors.ImageSharp.Memory;

internal delegate void TransformItemsDelegate<TSource, TTarget>(ReadOnlySpan<TSource> source, Span<TTarget> target);