// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FreePPlus.Imaging.Memory;

//was previously: namespace SixLabors.ImageSharp.Memory;

/// <summary>
///     A value-type enumerator for <see cref="MemoryGroup{T}" /> instances.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
[EditorBrowsable(EditorBrowsableState.Never)]
public ref struct MemoryGroupEnumerator<T>
    where T : struct
{
    private readonly IMemoryGroup<T> memoryGroup;
    private readonly int count;
    private int index;

    [MethodImpl(InliningOptions.ShortMethod)]
    internal MemoryGroupEnumerator(MemoryGroup<T>.Owned memoryGroup)
    {
        this.memoryGroup = memoryGroup;
        count = memoryGroup.Count;
        index = -1;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    internal MemoryGroupEnumerator(MemoryGroup<T>.Consumed memoryGroup)
    {
        this.memoryGroup = memoryGroup;
        count = memoryGroup.Count;
        index = -1;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    internal MemoryGroupEnumerator(MemoryGroupView<T> memoryGroup)
    {
        this.memoryGroup = memoryGroup;
        count = memoryGroup.Count;
        index = -1;
    }

    /// <inheritdoc cref="System.Collections.Generic.IEnumerator{T}.Current" />
    public Memory<T> Current
    {
        [MethodImpl(InliningOptions.ShortMethod)]
        get => memoryGroup[index];
    }

    /// <inheritdoc cref="System.Collections.IEnumerator.MoveNext" />
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool MoveNext()
    {
        var index = this.index + 1;

        if (index < count)
        {
            this.index = index;

            return true;
        }

        return false;
    }
}