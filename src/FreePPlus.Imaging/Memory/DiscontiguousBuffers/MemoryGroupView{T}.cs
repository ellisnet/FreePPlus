// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.Memory;

//was previously: namespace SixLabors.ImageSharp.Memory;

/// <summary>
///     Implements <see cref="IMemoryGroup{T}" />, defining a view for <see cref="Memory.MemoryGroup{T}" />
///     rather than owning the segments.
/// </summary>
/// <remarks>
///     This type provides an indirection, protecting the users of publicly exposed memory API-s
///     from internal memory-swaps. Whenever an internal swap happens, the <see cref="MemoryGroupView{T}" />
///     instance becomes invalid, throwing an exception on all operations.
/// </remarks>
/// <typeparam name="T">The element type.</typeparam>
internal class MemoryGroupView<T> : IMemoryGroup<T>
    where T : struct
{
    private readonly MemoryOwnerWrapper[] memoryWrappers;
    private MemoryGroup<T> owner;

    public MemoryGroupView(MemoryGroup<T> owner)
    {
        this.owner = owner;
        memoryWrappers = new MemoryOwnerWrapper[owner.Count];

        for (var i = 0; i < owner.Count; i++) memoryWrappers[i] = new MemoryOwnerWrapper(this, i);
    }

    public int Count
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        get
        {
            EnsureIsValid();
            return owner.Count;
        }
    }

    public int BufferLength
    {
        get
        {
            EnsureIsValid();
            return owner.BufferLength;
        }
    }

    public long TotalLength
    {
        get
        {
            EnsureIsValid();
            return owner.TotalLength;
        }
    }

    public bool IsValid => owner != null;

    public Memory<T> this[int index]
    {
        get
        {
            EnsureIsValid();
            return memoryWrappers[index].Memory;
        }
    }

    /// <inheritdoc />
    [MethodImpl(InliningOptions.ShortMethod)]
    public MemoryGroupEnumerator<T> GetEnumerator()
    {
        return new MemoryGroupEnumerator<T>(this);
    }

    /// <inheritdoc />
    IEnumerator<Memory<T>> IEnumerable<Memory<T>>.GetEnumerator()
    {
        EnsureIsValid();
        for (var i = 0; i < Count; i++) yield return memoryWrappers[i].Memory;
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<Memory<T>>)this).GetEnumerator();
    }

    internal void Invalidate()
    {
        owner = null;
    }

    private void EnsureIsValid()
    {
        if (!IsValid) throw new InvalidMemoryOperationException("Can not access an invalidated MemoryGroupView!");
    }

    private class MemoryOwnerWrapper : MemoryManager<T>
    {
        private readonly int index;
        private readonly MemoryGroupView<T> view;

        public MemoryOwnerWrapper(MemoryGroupView<T> view, int index)
        {
            this.view = view;
            this.index = index;
        }

        protected override void Dispose(bool disposing) { }

        public override Span<T> GetSpan()
        {
            view.EnsureIsValid();
            return view.owner[index].Span;
        }

        public override MemoryHandle Pin(int elementIndex = 0)
        {
            view.EnsureIsValid();
            return view.owner[index].Pin();
        }

        public override void Unpin()
        {
            throw new NotSupportedException();
        }
    }
}