// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;

namespace FreePPlus.Imaging.Metadata.Profiles.Icc;

//was previously: namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

/// <summary>
///     Provides methods to write ICC data types
/// </summary>
internal sealed partial class IccDataWriter : IDisposable
{
    /// <summary>
    ///     The underlying stream where the data is written to
    /// </summary>
    private readonly MemoryStream dataStream;

    /// <summary>
    ///     To detect redundant calls
    /// </summary>
    private bool isDisposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IccDataWriter" /> class.
    /// </summary>
    public IccDataWriter()
    {
        dataStream = new MemoryStream();
    }

    /// <summary>
    ///     Gets the currently written length in bytes
    /// </summary>
    public uint Length => (uint)dataStream.Length;

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    ///     Gets the written data bytes
    /// </summary>
    /// <returns>The written data</returns>
    public byte[] GetData()
    {
        return dataStream.ToArray();
    }

    /// <summary>
    ///     Sets the writing position to the given value
    /// </summary>
    /// <param name="index">The new index position</param>
    public void SetIndex(int index)
    {
        dataStream.Position = index;
    }

    /// <summary>
    ///     Writes a byte array
    /// </summary>
    /// <param name="data">The array to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteArray(byte[] data)
    {
        dataStream.Write(data, 0, data.Length);
        return data.Length;
    }

    /// <summary>
    ///     Writes a ushort array
    /// </summary>
    /// <param name="data">The array to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteArray(ushort[] data)
    {
        for (var i = 0; i < data.Length; i++) WriteUInt16(data[i]);

        return data.Length * 2;
    }

    /// <summary>
    ///     Writes a short array
    /// </summary>
    /// <param name="data">The array to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteArray(short[] data)
    {
        for (var i = 0; i < data.Length; i++) WriteInt16(data[i]);

        return data.Length * 2;
    }

    /// <summary>
    ///     Writes a uint array
    /// </summary>
    /// <param name="data">The array to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteArray(uint[] data)
    {
        for (var i = 0; i < data.Length; i++) WriteUInt32(data[i]);

        return data.Length * 4;
    }

    /// <summary>
    ///     Writes an int array
    /// </summary>
    /// <param name="data">The array to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteArray(int[] data)
    {
        for (var i = 0; i < data.Length; i++) WriteInt32(data[i]);

        return data.Length * 4;
    }

    /// <summary>
    ///     Writes a ulong array
    /// </summary>
    /// <param name="data">The array to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteArray(ulong[] data)
    {
        for (var i = 0; i < data.Length; i++) WriteUInt64(data[i]);

        return data.Length * 8;
    }

    /// <summary>
    ///     Write a number of empty bytes
    /// </summary>
    /// <param name="length">The number of bytes to write</param>
    /// <returns>The number of bytes written</returns>
    public int WriteEmpty(int length)
    {
        for (var i = 0; i < length; i++) dataStream.WriteByte(0);

        return length;
    }

    /// <summary>
    ///     Writes empty bytes to a 4-byte margin
    /// </summary>
    /// <returns>The number of bytes written</returns>
    public int WritePadding()
    {
        var p = 4 - (int)dataStream.Position % 4;
        return WriteEmpty(p >= 4 ? 0 : p);
    }

    /// <summary>
    ///     Writes given bytes from pointer
    /// </summary>
    /// <param name="data">Pointer to the bytes to write</param>
    /// <param name="length">The number of bytes to write</param>
    /// <returns>The number of bytes written</returns>
    private unsafe int WriteBytes(byte* data, int length)
    {
        if (BitConverter.IsLittleEndian)
            for (var i = length - 1; i >= 0; i--)
                dataStream.WriteByte(data[i]);
        else
            WriteBytesDirect(data, length);

        return length;
    }

    /// <summary>
    ///     Writes given bytes from pointer ignoring endianness
    /// </summary>
    /// <param name="data">Pointer to the bytes to write</param>
    /// <param name="length">The number of bytes to write</param>
    /// <returns>The number of bytes written</returns>
    private unsafe int WriteBytesDirect(byte* data, int length)
    {
        for (var i = 0; i < length; i++) dataStream.WriteByte(data[i]);

        return length;
    }

    /// <summary>
    ///     Writes curve data
    /// </summary>
    /// <param name="curves">The curves to write</param>
    /// <returns>The number of bytes written</returns>
    private int WriteCurves(IccTagDataEntry[] curves)
    {
        var count = 0;
        foreach (var curve in curves)
        {
            if (curve.Signature != IccTypeSignature.Curve && curve.Signature != IccTypeSignature.ParametricCurve)
                throw new InvalidIccProfileException(
                    $"Curve has to be either \"{nameof(IccTypeSignature)}.{nameof(IccTypeSignature.Curve)}\" or" +
                    $" \"{nameof(IccTypeSignature)}.{nameof(IccTypeSignature.ParametricCurve)}\" for LutAToB- and LutBToA-TagDataEntries");

            count += WriteTagDataEntry(curve);
            count += WritePadding();
        }

        return count;
    }

    private void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing) dataStream?.Dispose();

            isDisposed = true;
        }
    }
}