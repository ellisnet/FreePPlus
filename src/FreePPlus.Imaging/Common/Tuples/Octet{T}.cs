// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FreePPlus.Imaging.Tuples;

//was previously: namespace SixLabors.ImageSharp.Tuples;

/// <summary>
///     Contains 8 element value tuples of various types.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct Octet<T>
    where T : unmanaged
{
    public T V0;
    public T V1;
    public T V2;
    public T V3;
    public T V4;
    public T V5;
    public T V6;
    public T V7;

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return $"Octet<{typeof(T)}>({V0},{V1},{V2},{V3},{V4},{V5},{V6},{V7})";
    }
}

/// <summary>
///     Extension methods for the <see cref="Octet{T}" /> type.
/// </summary>
internal static class OctetExtensions
{
    /// <summary>
    ///     Loads the fields in a target <see cref="Octet{T}" /> of <see cref="uint" /> from one of <see cref="byte" /> type.
    /// </summary>
    /// <param name="destination">The target <see cref="Octet{T}" /> of <see cref="uint" /> instance.</param>
    /// <param name="source">The source <see cref="Octet{T}" /> of <see cref="byte" /> instance.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void LoadFrom(ref this Octet<uint> destination, ref Octet<byte> source)
    {
        destination.V0 = source.V0;
        destination.V1 = source.V1;
        destination.V2 = source.V2;
        destination.V3 = source.V3;
        destination.V4 = source.V4;
        destination.V5 = source.V5;
        destination.V6 = source.V6;
        destination.V7 = source.V7;
    }

    /// <summary>
    ///     Loads the fields in a target <see cref="Octet{T}" /> of <see cref="byte" /> from one of <see cref="uint" /> type.
    /// </summary>
    /// <param name="destination">The target <see cref="Octet{T}" /> of <see cref="byte" /> instance.</param>
    /// <param name="source">The source <see cref="Octet{T}" /> of <see cref="uint" /> instance.</param>
    [MethodImpl(InliningOptions.ShortMethod)]
    public static void LoadFrom(ref this Octet<byte> destination, ref Octet<uint> source)
    {
        destination.V0 = (byte)source.V0;
        destination.V1 = (byte)source.V1;
        destination.V2 = (byte)source.V2;
        destination.V3 = (byte)source.V3;
        destination.V4 = (byte)source.V4;
        destination.V5 = (byte)source.V5;
        destination.V6 = (byte)source.V6;
        destination.V7 = (byte)source.V7;
    }
}