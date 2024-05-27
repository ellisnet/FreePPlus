// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FreePPlus.Imaging.IO;

namespace FreePPlus.Imaging.Formats.Jpeg.Components.Decoder;

//was previously: namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;

/// <summary>
///     Decodes the Huffman encoded spectral scan.
///     Originally ported from <see href="https://github.com/t0rakka/mango" />
///     with additional fixes for both performance and common encoding errors.
/// </summary>
internal class HuffmanScanDecoder
{
    private readonly HuffmanTable[] acHuffmanTables;
    private readonly JpegComponent[] components;

    // The number of interleaved components.
    private readonly int componentsLength;
    private readonly HuffmanTable[] dcHuffmanTables;
    private readonly JpegFrame frame;

    // The restart interval.
    private readonly int restartInterval;

    // The spectral selection end.
    private readonly int spectralEnd;

    // The spectral selection start.
    private readonly int spectralStart;
    private readonly DoubleBufferedStreamReader stream;

    // The successive approximation high bit end.
    private readonly int successiveHigh;

    // The successive approximation low bit end.
    private readonly int successiveLow;

    // The unzig data.
    private ZigZag dctZigZag;

    // The End-Of-Block countdown for ending the sequence prematurely when the remaining coefficients are zero.
    private int eobrun;

    private HuffmanScanBuffer scanBuffer;

    // How many mcu's are left to do.
    private int todo;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HuffmanScanDecoder" /> class.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <param name="frame">The image frame.</param>
    /// <param name="dcHuffmanTables">The DC Huffman tables.</param>
    /// <param name="acHuffmanTables">The AC Huffman tables.</param>
    /// <param name="componentsLength">The length of the components. Different to the array length.</param>
    /// <param name="restartInterval">The reset interval.</param>
    /// <param name="spectralStart">The spectral selection start.</param>
    /// <param name="spectralEnd">The spectral selection end.</param>
    /// <param name="successiveHigh">The successive approximation bit high end.</param>
    /// <param name="successiveLow">The successive approximation bit low end.</param>
    public HuffmanScanDecoder(
        DoubleBufferedStreamReader stream,
        JpegFrame frame,
        HuffmanTable[] dcHuffmanTables,
        HuffmanTable[] acHuffmanTables,
        int componentsLength,
        int restartInterval,
        int spectralStart,
        int spectralEnd,
        int successiveHigh,
        int successiveLow)
    {
        dctZigZag = ZigZag.CreateUnzigTable();
        this.stream = stream;
        scanBuffer = new HuffmanScanBuffer(stream);
        this.frame = frame;
        this.dcHuffmanTables = dcHuffmanTables;
        this.acHuffmanTables = acHuffmanTables;
        components = frame.Components;
        this.componentsLength = componentsLength;
        this.restartInterval = restartInterval;
        todo = restartInterval;
        this.spectralStart = spectralStart;
        this.spectralEnd = spectralEnd;
        this.successiveHigh = successiveHigh;
        this.successiveLow = successiveLow;
    }

    /// <summary>
    ///     Decodes the entropy coded data.
    /// </summary>
    public void ParseEntropyCodedData()
    {
        if (!frame.Progressive)
            ParseBaselineData();
        else
            ParseProgressiveData();

        if (scanBuffer.HasBadMarker()) stream.Position = scanBuffer.MarkerPosition;
    }

    private void ParseBaselineData()
    {
        if (componentsLength == 1)
            ParseBaselineDataNonInterleaved();
        else
            ParseBaselineDataInterleaved();
    }

    private void ParseBaselineDataInterleaved()
    {
        // Interleaved
        var mcu = 0;
        var mcusPerColumn = frame.McusPerColumn;
        var mcusPerLine = frame.McusPerLine;
        ref var buffer = ref scanBuffer;

        // Pre-derive the huffman table to avoid in-loop checks.
        for (var i = 0; i < componentsLength; i++)
        {
            int order = frame.ComponentOrder[i];
            var component = components[order];

            ref var dcHuffmanTable = ref dcHuffmanTables[component.DCHuffmanTableId];
            ref var acHuffmanTable = ref acHuffmanTables[component.ACHuffmanTableId];
            dcHuffmanTable.Configure();
            acHuffmanTable.Configure();
        }

        for (var j = 0; j < mcusPerColumn; j++)
        for (var i = 0; i < mcusPerLine; i++)
        {
            // Scan an interleaved mcu... process components in order
            var mcuRow = mcu / mcusPerLine;
            var mcuCol = mcu % mcusPerLine;
            for (var k = 0; k < componentsLength; k++)
            {
                int order = frame.ComponentOrder[k];
                var component = components[order];

                ref var dcHuffmanTable = ref dcHuffmanTables[component.DCHuffmanTableId];
                ref var acHuffmanTable = ref acHuffmanTables[component.ACHuffmanTableId];

                var h = component.HorizontalSamplingFactor;
                var v = component.VerticalSamplingFactor;

                // Scan out an mcu's worth of this component; that's just determined
                // by the basic H and V specified for the component
                for (var y = 0; y < v; y++)
                {
                    var blockRow = mcuRow * v + y;
                    var blockSpan = component.SpectralBlocks.GetRowSpan(blockRow);
                    ref var blockRef = ref MemoryMarshal.GetReference(blockSpan);

                    for (var x = 0; x < h; x++)
                    {
                        if (buffer.NoData) return;

                        var blockCol = mcuCol * h + x;

                        DecodeBlockBaseline(
                            component,
                            ref Unsafe.Add(ref blockRef, blockCol),
                            ref dcHuffmanTable,
                            ref acHuffmanTable);
                    }
                }
            }

            // After all interleaved components, that's an interleaved MCU,
            // so now count down the restart interval
            mcu++;
            HandleRestart();
        }
    }

    private void ParseBaselineDataNonInterleaved()
    {
        var component = components[frame.ComponentOrder[0]];
        ref var buffer = ref scanBuffer;

        var w = component.WidthInBlocks;
        var h = component.HeightInBlocks;

        ref var dcHuffmanTable = ref dcHuffmanTables[component.DCHuffmanTableId];
        ref var acHuffmanTable = ref acHuffmanTables[component.ACHuffmanTableId];
        dcHuffmanTable.Configure();
        acHuffmanTable.Configure();

        for (var j = 0; j < h; j++)
        {
            var blockSpan = component.SpectralBlocks.GetRowSpan(j);
            ref var blockRef = ref MemoryMarshal.GetReference(blockSpan);

            for (var i = 0; i < w; i++)
            {
                if (buffer.NoData) return;

                DecodeBlockBaseline(
                    component,
                    ref Unsafe.Add(ref blockRef, i),
                    ref dcHuffmanTable,
                    ref acHuffmanTable);

                HandleRestart();
            }
        }
    }

    private void CheckProgressiveData()
    {
        // Validate successive scan parameters.
        // Logic has been adapted from libjpeg.
        // See Table B.3 – Scan header parameter size and values. itu-t81.pdf
        var invalid = false;
        if (spectralStart == 0)
        {
            if (spectralEnd != 0) invalid = true;
        }
        else
        {
            // Need not check Ss/Se < 0 since they came from unsigned bytes.
            if (spectralEnd < spectralStart || spectralEnd > 63) invalid = true;

            // AC scans may have only one component.
            if (componentsLength != 1) invalid = true;
        }

        if (successiveHigh != 0)
            // Successive approximation refinement scan: must have Al = Ah-1.
            if (successiveHigh - 1 != successiveLow)
                invalid = true;

        // TODO: How does this affect 12bit jpegs.
        // According to libjpeg the range covers 8bit only?
        if (successiveLow > 13) invalid = true;

        if (invalid) JpegThrowHelper.ThrowBadProgressiveScan(spectralStart, spectralEnd, successiveHigh, successiveLow);
    }

    private void ParseProgressiveData()
    {
        CheckProgressiveData();

        if (componentsLength == 1)
            ParseProgressiveDataNonInterleaved();
        else
            ParseProgressiveDataInterleaved();
    }

    private void ParseProgressiveDataInterleaved()
    {
        // Interleaved
        var mcu = 0;
        var mcusPerColumn = frame.McusPerColumn;
        var mcusPerLine = frame.McusPerLine;
        ref var buffer = ref scanBuffer;

        // Pre-derive the huffman table to avoid in-loop checks.
        for (var k = 0; k < componentsLength; k++)
        {
            int order = frame.ComponentOrder[k];
            var component = components[order];
            ref var dcHuffmanTable = ref dcHuffmanTables[component.DCHuffmanTableId];
            dcHuffmanTable.Configure();
        }

        for (var j = 0; j < mcusPerColumn; j++)
        for (var i = 0; i < mcusPerLine; i++)
        {
            // Scan an interleaved mcu... process components in order
            var mcuRow = mcu / mcusPerLine;
            var mcuCol = mcu % mcusPerLine;
            for (var k = 0; k < componentsLength; k++)
            {
                int order = frame.ComponentOrder[k];
                var component = components[order];
                ref var dcHuffmanTable = ref dcHuffmanTables[component.DCHuffmanTableId];

                var h = component.HorizontalSamplingFactor;
                var v = component.VerticalSamplingFactor;

                // Scan out an mcu's worth of this component; that's just determined
                // by the basic H and V specified for the component
                for (var y = 0; y < v; y++)
                {
                    var blockRow = mcuRow * v + y;
                    var blockSpan = component.SpectralBlocks.GetRowSpan(blockRow);
                    ref var blockRef = ref MemoryMarshal.GetReference(blockSpan);

                    for (var x = 0; x < h; x++)
                    {
                        if (buffer.NoData) return;

                        var blockCol = mcuCol * h + x;

                        DecodeBlockProgressiveDC(
                            component,
                            ref Unsafe.Add(ref blockRef, blockCol),
                            ref dcHuffmanTable);
                    }
                }
            }

            // After all interleaved components, that's an interleaved MCU,
            // so now count down the restart interval
            mcu++;
            HandleRestart();
        }
    }

    private void ParseProgressiveDataNonInterleaved()
    {
        var component = components[frame.ComponentOrder[0]];
        ref var buffer = ref scanBuffer;

        var w = component.WidthInBlocks;
        var h = component.HeightInBlocks;

        if (spectralStart == 0)
        {
            ref var dcHuffmanTable = ref dcHuffmanTables[component.DCHuffmanTableId];
            dcHuffmanTable.Configure();

            for (var j = 0; j < h; j++)
            {
                var blockSpan = component.SpectralBlocks.GetRowSpan(j);
                ref var blockRef = ref MemoryMarshal.GetReference(blockSpan);

                for (var i = 0; i < w; i++)
                {
                    if (buffer.NoData) return;

                    DecodeBlockProgressiveDC(
                        component,
                        ref Unsafe.Add(ref blockRef, i),
                        ref dcHuffmanTable);

                    HandleRestart();
                }
            }
        }
        else
        {
            ref var acHuffmanTable = ref acHuffmanTables[component.ACHuffmanTableId];
            acHuffmanTable.Configure();

            for (var j = 0; j < h; j++)
            {
                var blockSpan = component.SpectralBlocks.GetRowSpan(j);
                ref var blockRef = ref MemoryMarshal.GetReference(blockSpan);

                for (var i = 0; i < w; i++)
                {
                    if (buffer.NoData) return;

                    DecodeBlockProgressiveAC(
                        ref Unsafe.Add(ref blockRef, i),
                        ref acHuffmanTable);

                    HandleRestart();
                }
            }
        }
    }

    private void DecodeBlockBaseline(
        JpegComponent component,
        ref Block8x8 block,
        ref HuffmanTable dcTable,
        ref HuffmanTable acTable)
    {
        ref var blockDataRef = ref Unsafe.As<Block8x8, short>(ref block);
        ref var buffer = ref scanBuffer;
        ref var zigzag = ref dctZigZag;

        // DC
        var t = buffer.DecodeHuffman(ref dcTable);
        if (t != 0) t = buffer.Receive(t);

        t += component.DcPredictor;
        component.DcPredictor = t;
        blockDataRef = (short)t;

        // AC
        for (var i = 1; i < 64;)
        {
            var s = buffer.DecodeHuffman(ref acTable);

            var r = s >> 4;
            s &= 15;

            if (s != 0)
            {
                i += r;
                s = buffer.Receive(s);
                Unsafe.Add(ref blockDataRef, zigzag[i++]) = (short)s;
            }
            else
            {
                if (r == 0) break;

                i += 16;
            }
        }
    }

    private void DecodeBlockProgressiveDC(JpegComponent component, ref Block8x8 block, ref HuffmanTable dcTable)
    {
        ref var blockDataRef = ref Unsafe.As<Block8x8, short>(ref block);
        ref var buffer = ref scanBuffer;

        if (successiveHigh == 0)
        {
            // First scan for DC coefficient, must be first
            var s = buffer.DecodeHuffman(ref dcTable);
            if (s != 0) s = buffer.Receive(s);

            s += component.DcPredictor;
            component.DcPredictor = s;
            blockDataRef = (short)(s << successiveLow);
        }
        else
        {
            // Refinement scan for DC coefficient
            buffer.CheckBits();
            blockDataRef |= (short)(buffer.GetBits(1) << successiveLow);
        }
    }

    private void DecodeBlockProgressiveAC(ref Block8x8 block, ref HuffmanTable acTable)
    {
        ref var blockDataRef = ref Unsafe.As<Block8x8, short>(ref block);
        if (successiveHigh == 0)
        {
            // MCU decoding for AC initial scan (either spectral selection,
            // or first pass of successive approximation).
            if (eobrun != 0)
            {
                --eobrun;
                return;
            }

            ref var buffer = ref scanBuffer;
            ref var zigzag = ref dctZigZag;
            var start = spectralStart;
            var end = spectralEnd;
            var low = successiveLow;

            for (var i = start; i <= end; ++i)
            {
                var s = buffer.DecodeHuffman(ref acTable);
                var r = s >> 4;
                s &= 15;

                i += r;

                if (s != 0)
                {
                    s = buffer.Receive(s);
                    Unsafe.Add(ref blockDataRef, zigzag[i]) = (short)(s << low);
                }
                else
                {
                    if (r != 15)
                    {
                        eobrun = 1 << r;
                        if (r != 0)
                        {
                            buffer.CheckBits();
                            eobrun += buffer.GetBits(r);
                        }

                        --eobrun;
                        break;
                    }
                }
            }
        }
        else
        {
            // Refinement scan for these AC coefficients
            DecodeBlockProgressiveACRefined(ref blockDataRef, ref acTable);
        }
    }

    private void DecodeBlockProgressiveACRefined(ref short blockDataRef, ref HuffmanTable acTable)
    {
        // Refinement scan for these AC coefficients
        ref var buffer = ref scanBuffer;
        ref var zigzag = ref dctZigZag;
        var start = spectralStart;
        var end = spectralEnd;

        var p1 = 1 << successiveLow;
        var m1 = -1 << successiveLow;

        var k = start;

        if (eobrun == 0)
            for (; k <= end; k++)
            {
                var s = buffer.DecodeHuffman(ref acTable);
                var r = s >> 4;
                s &= 15;

                if (s != 0)
                {
                    buffer.CheckBits();
                    if (buffer.GetBits(1) != 0)
                        s = p1;
                    else
                        s = m1;
                }
                else
                {
                    if (r != 15)
                    {
                        eobrun = 1 << r;

                        if (r != 0)
                        {
                            buffer.CheckBits();
                            eobrun += buffer.GetBits(r);
                        }

                        break;
                    }
                }

                do
                {
                    ref var coef = ref Unsafe.Add(ref blockDataRef, zigzag[k]);
                    if (coef != 0)
                    {
                        buffer.CheckBits();
                        if (buffer.GetBits(1) != 0)
                            if ((coef & p1) == 0)
                                coef += (short)(coef >= 0 ? p1 : m1);
                    }
                    else
                    {
                        if (--r < 0) break;
                    }

                    k++;
                } while (k <= end);

                if (s != 0 && k < 64) Unsafe.Add(ref blockDataRef, zigzag[k]) = (short)s;
            }

        if (eobrun > 0)
        {
            for (; k <= end; k++)
            {
                ref var coef = ref Unsafe.Add(ref blockDataRef, zigzag[k]);

                if (coef != 0)
                {
                    buffer.CheckBits();
                    if (buffer.GetBits(1) != 0)
                        if ((coef & p1) == 0)
                            coef += (short)(coef >= 0 ? p1 : m1);
                }
            }

            --eobrun;
        }
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private void Reset()
    {
        for (var i = 0; i < components.Length; i++) components[i].DcPredictor = 0;

        eobrun = 0;
        scanBuffer.Reset();
    }

    [MethodImpl(InliningOptions.ShortMethod)]
    private bool HandleRestart()
    {
        if (restartInterval > 0 && --todo == 0)
        {
            if (scanBuffer.Marker == JpegConstants.Markers.XFF)
                if (!scanBuffer.FindNextMarker())
                    return false;

            todo = restartInterval;

            if (scanBuffer.HasRestartMarker())
            {
                Reset();
                return true;
            }

            if (scanBuffer.HasBadMarker())
            {
                stream.Position = scanBuffer.MarkerPosition;
                Reset();
                return true;
            }
        }

        return false;
    }
}