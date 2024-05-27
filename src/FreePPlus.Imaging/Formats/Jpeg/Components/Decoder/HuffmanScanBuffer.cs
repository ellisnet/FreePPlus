// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.IO;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0290
#pragma warning disable IDE0251

namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;

/// <summary>
///     Used to buffer and track the bits read from the Huffman entropy encoded data.
/// </summary>
internal struct HuffmanScanBuffer
{
    private readonly DoubleBufferedStreamReader stream;

    // The entropy encoded code buffer.
    private ulong data;

    // The number of valid bits left to read in the buffer.
    private int remainingBits;

    // Whether there is no more good data to pull from the stream for the current mcu.
    private bool badData;

    public HuffmanScanBuffer(DoubleBufferedStreamReader stream)
    {
        this.stream = stream;
        data = 0ul;
        remainingBits = 0;
        Marker = JpegConstants.Markers.XFF;
        MarkerPosition = 0;
        badData = false;
        NoData = false;
    }

    /// <summary>
    ///     Gets the current, if any, marker in the input stream.
    /// </summary>
    public byte Marker { get; private set; }

    /// <summary>
    ///     Gets the opening position of an identified marker.
    /// </summary>
    public long MarkerPosition { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether to continue reading the input stream.
    /// </summary>
    public bool NoData { get; private set; }

    [MethodImpl(InliningOptions.ShortMethod)]
    public void CheckBits()
    {
        if (remainingBits < JpegConstants.Huffman.MinBits) FillBuffer();
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public void Reset()
    {
        data = 0ul;
        remainingBits = 0;
        Marker = JpegConstants.Markers.XFF;
        MarkerPosition = 0;
        badData = false;
        NoData = false;
    }

    /// <summary>
    ///     Whether a RST marker has been detected, I.E. One that is between RST0 and RST7
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool HasRestartMarker()
    {
        return HasRestart(Marker);
    }

    /// <summary>
    ///     Whether a bad marker has been detected, I.E. One that is not between RST0 and RST7
    /// </summary>
    [MethodImpl(InliningOptions.ShortMethod)]
    public bool HasBadMarker()
    {
        return Marker != JpegConstants.Markers.XFF && !HasRestartMarker();
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public void FillBuffer()
    {
        // Attempt to load at least the minimum number of required bits into the buffer.
        // We fail to do so only if we hit a marker or reach the end of the input stream.
        remainingBits += JpegConstants.Huffman.FetchBits;
        data = (data << JpegConstants.Huffman.FetchBits) | GetBytes();
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public unsafe int DecodeHuffman(ref HuffmanTable h)
    {
        CheckBits();
        var v = PeekBits(JpegConstants.Huffman.LookupBits);
        int symbol = h.LookaheadValue[v];
        int size = h.LookaheadSize[v];

        if (size == JpegConstants.Huffman.SlowBits)
        {
            var x = data << (JpegConstants.Huffman.RegisterSize - remainingBits);
            while (x > h.MaxCode[size]) size++;

            v = (int)(x >> (JpegConstants.Huffman.RegisterSize - size));
            symbol = h.Values[(h.ValOffset[size] + v) & 0xFF];
        }

        remainingBits -= size;

        return symbol;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public int Receive(int nbits)
    {
        CheckBits();
        return Extend(GetBits(nbits), nbits);
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static bool HasRestart(byte marker)
    {
        return marker >= JpegConstants.Markers.RST0 && marker <= JpegConstants.Markers.RST7;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public int GetBits(int nbits)
    {
        return (int)ExtractBits(data, remainingBits -= nbits, nbits);
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public int PeekBits(int nbits)
    {
        return (int)ExtractBits(data, remainingBits - nbits, nbits);
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static ulong ExtractBits(ulong value, int offset, int size)
    {
        return (value >> offset) & (ulong)((1 << size) - 1);
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private static int Extend(int v, int nbits)
    {
        return v - ((((v + v) >> nbits) - 1) & ((1 << nbits) - 1));
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private ulong GetBytes()
    {
        ulong temp = 0;
        for (var i = 0; i < JpegConstants.Huffman.FetchLoop; i++)
        {
            var b = ReadStream();

            // Found a marker.
            if (b == JpegConstants.Markers.XFF)
            {
                var c = ReadStream();
                while (c == JpegConstants.Markers.XFF)
                    // Loop here to discard any padding FF bytes on terminating marker,
                    // so that we can save a valid marker value.
                    c = ReadStream();

                // We accept multiple FF bytes followed by a 0 as meaning a single FF data byte.
                // This data pattern is not valid according to the standard.
                if (c != 0)
                {
                    Marker = (byte)c;
                    badData = true;
                    MarkerPosition = stream.Position - 2;
                }
            }

#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
            temp = (temp << 8) | (ulong)b;
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
        }

        return temp;
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    public bool FindNextMarker()
    {
        while (true)
        {
            var b = stream.ReadByte();
            if (b == -1) return false;

            // Found a marker.
            if (b == JpegConstants.Markers.XFF)
            {
                while (b == JpegConstants.Markers.XFF)
                {
                    // Loop here to discard any padding FF bytes on terminating marker.
                    b = stream.ReadByte();
                    if (b == -1) return false;
                }

                // Found a valid marker. Exit loop
                if (b != 0)
                {
                    Marker = (byte)b;
                    badData = true;
                    MarkerPosition = stream.Position - 2;
                    return true;
                }
            }
        }
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private int ReadStream()
    {
        var value = badData ? 0 : stream.ReadByte();
        if (value == -1)
        {
            // We've encountered the end of the file stream which means there's no EOI marker
            // in the image or the SOS marker has the wrong dimensions set.
            badData = true;
            NoData = true;
            value = 0;
        }

        return value;
    }
}