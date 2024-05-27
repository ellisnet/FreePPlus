// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;

namespace FreePPlus.Imaging.Memory;

//was previously: namespace SixLabors.ImageSharp.Memory;

internal static class MemoryGroupExtensions
{
    internal static void Fill<T>(this IMemoryGroup<T> group, T value)
        where T : struct
    {
        foreach (var memory in group) memory.Span.Fill(value);
    }

    internal static void Clear<T>(this IMemoryGroup<T> group)
        where T : struct
    {
        foreach (var memory in group) memory.Span.Clear();
    }

    /// <summary>
    ///     Returns a slice that is expected to be within the bounds of a single buffer.
    ///     Otherwise <see cref="ArgumentOutOfRangeException" /> is thrown.
    /// </summary>
    internal static Memory<T> GetBoundedSlice<T>(this IMemoryGroup<T> group, long start, int length)
        where T : struct
    {
        Guard.NotNull(group, nameof(group));
        Guard.IsTrue(group.IsValid, nameof(group), "Group must be valid!");
        Guard.MustBeGreaterThanOrEqualTo(length, 0, nameof(length));
        Guard.MustBeLessThan(start, group.TotalLength, nameof(start));

        var bufferIdx = (int)(start / group.BufferLength);

        if (bufferIdx < 0) throw new ArgumentOutOfRangeException(nameof(start));

        if (bufferIdx >= group.Count) throw new ArgumentOutOfRangeException(nameof(start));

        var bufferStart = (int)(start % group.BufferLength);
        var bufferEnd = bufferStart + length;
        var memory = group[bufferIdx];

        if (bufferEnd > memory.Length) throw new ArgumentOutOfRangeException(nameof(length));

        return memory.Slice(bufferStart, length);
    }

    internal static void CopyTo<T>(this IMemoryGroup<T> source, Span<T> target)
        where T : struct
    {
        Guard.NotNull(source, nameof(source));
        Guard.MustBeGreaterThanOrEqualTo(target.Length, source.TotalLength, nameof(target));

        var cur = new MemoryGroupCursor<T>(source);
        long position = 0;
        while (position < source.TotalLength)
        {
            var fwd = Math.Min(cur.LookAhead(), target.Length);
            cur.GetSpan(fwd).CopyTo(target);

            cur.Forward(fwd);
            target = target.Slice(fwd);
            position += fwd;
        }
    }

    internal static void CopyTo<T>(this Span<T> source, IMemoryGroup<T> target)
        where T : struct
    {
        CopyTo((ReadOnlySpan<T>)source, target);
    }

    internal static void CopyTo<T>(this ReadOnlySpan<T> source, IMemoryGroup<T> target)
        where T : struct
    {
        Guard.NotNull(target, nameof(target));
        Guard.MustBeGreaterThanOrEqualTo(target.TotalLength, source.Length, nameof(target));

        var cur = new MemoryGroupCursor<T>(target);

        while (!source.IsEmpty)
        {
            var fwd = Math.Min(cur.LookAhead(), source.Length);
            source.Slice(0, fwd).CopyTo(cur.GetSpan(fwd));
            cur.Forward(fwd);
            source = source.Slice(fwd);
        }
    }

    internal static void CopyTo<T>(this IMemoryGroup<T> source, IMemoryGroup<T> target)
        where T : struct
    {
        Guard.NotNull(source, nameof(source));
        Guard.NotNull(target, nameof(target));
        Guard.IsTrue(source.IsValid, nameof(source), "Source group must be valid.");
        Guard.IsTrue(target.IsValid, nameof(target), "Target group must be valid.");
        Guard.MustBeLessThanOrEqualTo(source.TotalLength, target.TotalLength, "Destination buffer too short!");

        if (source.IsEmpty()) return;

        long position = 0;
        var srcCur = new MemoryGroupCursor<T>(source);
        var trgCur = new MemoryGroupCursor<T>(target);

        while (position < source.TotalLength)
        {
            var fwd = Math.Min(srcCur.LookAhead(), trgCur.LookAhead());
            var srcSpan = srcCur.GetSpan(fwd);
            var trgSpan = trgCur.GetSpan(fwd);
            srcSpan.CopyTo(trgSpan);

            srcCur.Forward(fwd);
            trgCur.Forward(fwd);
            position += fwd;
        }
    }

    internal static void TransformTo<TSource, TTarget>(
        this IMemoryGroup<TSource> source,
        IMemoryGroup<TTarget> target,
        TransformItemsDelegate<TSource, TTarget> transform)
        where TSource : struct
        where TTarget : struct
    {
        Guard.NotNull(source, nameof(source));
        Guard.NotNull(target, nameof(target));
        Guard.NotNull(transform, nameof(transform));
        Guard.IsTrue(source.IsValid, nameof(source), "Source group must be valid.");
        Guard.IsTrue(target.IsValid, nameof(target), "Target group must be valid.");
        Guard.MustBeLessThanOrEqualTo(source.TotalLength, target.TotalLength, "Destination buffer too short!");

        if (source.IsEmpty()) return;

        long position = 0;
        var srcCur = new MemoryGroupCursor<TSource>(source);
        var trgCur = new MemoryGroupCursor<TTarget>(target);

        while (position < source.TotalLength)
        {
            var fwd = Math.Min(srcCur.LookAhead(), trgCur.LookAhead());
            var srcSpan = srcCur.GetSpan(fwd);
            var trgSpan = trgCur.GetSpan(fwd);
            transform(srcSpan, trgSpan);

            srcCur.Forward(fwd);
            trgCur.Forward(fwd);
            position += fwd;
        }
    }

    internal static void TransformInplace<T>(
        this IMemoryGroup<T> memoryGroup,
        TransformItemsInplaceDelegate<T> transform)
        where T : struct
    {
        foreach (var memory in memoryGroup) transform(memory.Span);
    }

    internal static bool IsEmpty<T>(this IMemoryGroup<T> group)
        where T : struct
    {
        return group.Count == 0;
    }

    private struct MemoryGroupCursor<T>
        where T : struct
    {
        private readonly IMemoryGroup<T> memoryGroup;

        private int bufferIndex;

        private int elementIndex;

        public MemoryGroupCursor(IMemoryGroup<T> memoryGroup)
        {
            this.memoryGroup = memoryGroup;
            bufferIndex = 0;
            elementIndex = 0;
        }

        private bool IsAtLastBuffer => bufferIndex == memoryGroup.Count - 1;

        private int CurrentBufferLength => memoryGroup[bufferIndex].Length;

        public Span<T> GetSpan(int length)
        {
            return memoryGroup[bufferIndex].Span.Slice(elementIndex, length);
        }

        public int LookAhead()
        {
            return CurrentBufferLength - elementIndex;
        }

        public void Forward(int steps)
        {
            var nextIdx = elementIndex + steps;
            var currentBufferLength = CurrentBufferLength;

            if (nextIdx < currentBufferLength)
            {
                elementIndex = nextIdx;
            }
            else if (nextIdx == currentBufferLength)
            {
                bufferIndex++;
                elementIndex = 0;
            }
            else
            {
                // If we get here, it indicates a bug in CopyTo<T>:
                throw new ArgumentException("Can't forward multiple buffers!", nameof(steps));
            }
        }
    }
}