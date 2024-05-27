// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using FreePPlus.Imaging.Fonts.Unicode;

#pragma warning disable IDE0251
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts;

//was previously: namespace SixLabors.Fonts;

/// <summary>
///     Encapsulated logic or laying out text.
/// </summary>
internal static class TextLayout
{
    public static IReadOnlyList<GlyphLayout> GenerateLayout(ReadOnlySpan<char> text, TextOptions options)
    {
        if (text.IsEmpty) return Array.Empty<GlyphLayout>();

        var textBox = ProcessText(text, options);
        return LayoutText(textBox, options);
    }

    public static IReadOnlyList<TextRun> BuildTextRuns(ReadOnlySpan<char> text, TextOptions options)
    {
        if (options.TextRuns is null || options.TextRuns.Count == 0)
            return new TextRun[]
            {
                new()
                {
                    Start = 0,
                    End = text.GetGraphemeCount(),
                    Font = options.Font
                }
            };

        var start = 0;
        var end = text.GetGraphemeCount();
        List<TextRun> textRuns = new();
        foreach (var textRun in options.TextRuns!.OrderBy(x => x.Start))
        {
            // Fill gaps within runs.
            if (textRun.Start > start)
                textRuns.Add(new TextRun
                {
                    Start = start,
                    End = textRun.Start,
                    Font = options.Font
                });

            // Add the current run, ensuring the font is not null.
            textRun.Font ??= options.Font;

            // Ensure that the previous run does not overlap the current.
            if (textRuns.Count > 0)
            {
                var prevIndex = textRuns.Count - 1;
                var previous = textRuns[prevIndex];
                previous.End = Math.Min(previous.End, textRun.Start);
            }

            textRuns.Add(textRun);
            start = textRun.End;
        }

        // Add a final run if required.
        if (start < end)
        {
            // Offset error by user, last index in input string
            // instead of exclusive index.
            if (start == end - 1)
            {
                var prevIndex = textRuns.Count - 1;
                var previous = textRuns[prevIndex];
                previous.End++;
            }
            else
            {
                textRuns.Add(new TextRun
                {
                    Start = start,
                    End = end,
                    Font = options.Font
                });
            }
        }

        return textRuns;
    }

    private static TextBox ProcessText(ReadOnlySpan<char> text, TextOptions options)
    {
        // Gather the font and fallbacks.
        var fallbackFonts = options.FallbackFontFamilies?.Count > 0
            ? options.FallbackFontFamilies.Select(x => new Font(x, options.Font.Size, options.Font.RequestedStyle))
                .ToArray()
            : Array.Empty<Font>();

        var layoutMode = options.LayoutMode;
        GlyphSubstitutionCollection substitutions = new(options);
        GlyphPositioningCollection positionings = new(options);

        // Analyse the text for bidi directional runs.
        var bidi = BidiAlgorithm.Instance.Value!;
        var bidiData = new BidiData();
        bidiData.Init(text, (sbyte)options.TextDirection);

        // If we have embedded directional overrides then change those
        // ranges to neutral.
        if (options.TextDirection != TextDirection.Auto)
        {
            bidiData.SaveTypes();
            bidiData.Types.Span.Fill(BidiCharacterType.OtherNeutral);
            bidiData.PairedBracketTypes.Span.Fill(BidiPairedBracketType.None);
        }

        bidi.Process(bidiData);

        // Get the list of directional runs
        var bidiRuns = BidiRun.CoalesceLevels(bidi.ResolvedLevels).ToArray();
        Dictionary<int, int> bidiMap = new();

        // Incrementally build out collection of glyphs.
        var textRuns = BuildTextRuns(text, options);

        // First do multiple font runs using the individual text runs.
        var complete = true;
        var textRunIndex = 0;
        var codePointIndex = 0;
        var bidiRunIndex = 0;
        foreach (var textRun in textRuns)
            if (!DoFontRun(
                    textRun.Slice(text),
                    textRun.Start,
                    textRuns,
                    ref textRunIndex,
                    ref codePointIndex,
                    ref bidiRunIndex,
                    false,
                    textRun.Font!,
                    bidiRuns,
                    bidiMap,
                    substitutions,
                    positionings))
                complete = false;

        if (!complete)
            // Finally try our fallback fonts.
            // We do a complete run here across the whole collection.
            foreach (var font in fallbackFonts)
            {
                textRunIndex = 0;
                codePointIndex = 0;
                bidiRunIndex = 0;
                if (DoFontRun(
                        text,
                        0,
                        textRuns,
                        ref textRunIndex,
                        ref codePointIndex,
                        ref bidiRunIndex,
                        true,
                        font,
                        bidiRuns,
                        bidiMap,
                        substitutions,
                        positionings))
                    break;
            }

        // Update the positions of the glyphs in the completed collection.
        // Each set of metrics is associated with single font and will only be updated
        // by that font so it's safe to use a single collection.
        foreach (var textRun in textRuns) textRun.Font!.FontMetrics.UpdatePositions(positionings);

        foreach (var font in fallbackFonts) font.FontMetrics.UpdatePositions(positionings);

        return BreakLines(text, options, bidiRuns, bidiMap, positionings, layoutMode);
    }

    private static IReadOnlyList<GlyphLayout> LayoutText(TextBox textBox, TextOptions options)
    {
        var layoutMode = options.LayoutMode;
        List<GlyphLayout> glyphs = new();
        var location = options.Origin / options.Dpi;
        var maxScaledAdvance = textBox.ScaledMaxAdvance();
        var direction = textBox.TextDirection();

        if (layoutMode == LayoutMode.HorizontalTopBottom)
        {
            for (var i = 0; i < textBox.TextLines.Count; i++)
                glyphs.AddRange(LayoutLineHorizontal(textBox, textBox.TextLines[i], direction, maxScaledAdvance,
                    options, i, ref location));
        }
        else if (layoutMode == LayoutMode.HorizontalBottomTop)
        {
            var index = 0;
            for (var i = textBox.TextLines.Count - 1; i >= 0; i--)
                glyphs.AddRange(LayoutLineHorizontal(textBox, textBox.TextLines[i], direction, maxScaledAdvance,
                    options, index++, ref location));
        }
        else if (layoutMode == LayoutMode.VerticalLeftRight)
        {
            for (var i = 0; i < textBox.TextLines.Count; i++)
                glyphs.AddRange(LayoutLineVertical(textBox, textBox.TextLines[i], direction, maxScaledAdvance, options,
                    i, ref location));
        }
        else
        {
            var index = 0;
            for (var i = textBox.TextLines.Count - 1; i >= 0; i--)
                glyphs.AddRange(LayoutLineVertical(textBox, textBox.TextLines[i], direction, maxScaledAdvance, options,
                    index++, ref location));
        }

        return glyphs;
    }

    private static IEnumerable<GlyphLayout> LayoutLineHorizontal(
        TextBox textBox,
        TextLine textLine,
        TextDirection direction,
        float maxScaledAdvance,
        TextOptions options,
        int index,
        ref Vector2 location)
    {
        var scaledMaxLineGap = textBox.ScaledMaxLineGap(textLine.MaxPointSize);
        var scaledMaxAscender = textBox.ScaledMaxAscender(textLine.MaxPointSize);
        var scaledMaxDescender = textBox.ScaledMaxDescender(textLine.MaxPointSize);
        var scaledMaxLineHeight = textBox.ScaledMaxLineHeight(textLine.MaxPointSize);

        var isFirstLine = index == 0;
        var isLastLine = index == textBox.TextLines.Count - 1;
        var scaledLineAdvance = scaledMaxLineHeight * options.LineSpacing;

        // Recalculate the advance based upon the next line.
        // If larger, we want to scale it up to ensure it it pushed down far enough.
        // We split the different at 2/3 (heuristically determined value based upon extensive visual testing).
        if (!isFirstLine && !isLastLine)
        {
            var next = textBox.TextLines[index + 1];
            var nextLineAdvance = textBox.ScaledMaxLineHeight(next.MaxPointSize) * options.LineSpacing;
            scaledLineAdvance += (nextLineAdvance - scaledLineAdvance) * .667F;
        }

        var originX = location.X;
        float offsetY = 0;
        float offsetX = 0;

        // Set the Y-Origin for the line.
        if (isFirstLine)
        {
            switch (options.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    offsetY = scaledMaxAscender;
                    break;
                case VerticalAlignment.Center:
                    offsetY = (scaledMaxAscender - (scaledMaxDescender + scaledMaxLineGap)) * .5F;
                    for (var i = index; i < textBox.TextLines.Count - 1; i++)
                    {
                        var advance = textBox.ScaledMaxLineHeight(textBox.TextLines[i].MaxPointSize);
                        if (i != 0)
                        {
                            var next = textBox.TextLines[index + 1];
                            var nextLineAdvance = textBox.ScaledMaxLineHeight(next.MaxPointSize);
                            advance += (nextLineAdvance - advance) * .667F;
                        }

                        offsetY -= advance * options.LineSpacing * .5F;
                    }

                    break;
                case VerticalAlignment.Bottom:
                    offsetY = -(scaledMaxDescender + scaledMaxLineGap);
                    for (var i = index; i < textBox.TextLines.Count - 1; i++)
                    {
                        var advance = textBox.ScaledMaxLineHeight(textBox.TextLines[i].MaxPointSize);
                        if (i != 0)
                        {
                            var next = textBox.TextLines[index + 1];
                            var nextLineAdvance = textBox.ScaledMaxLineHeight(next.MaxPointSize);
                            advance += (nextLineAdvance - advance) * .667F;
                        }

                        offsetY -= advance * options.LineSpacing;
                    }

                    break;
            }

            location.Y += offsetY;
        }

        // Set the X-Origin for horizontal alignment.
        switch (options.HorizontalAlignment)
        {
            case HorizontalAlignment.Right:
                offsetX = -maxScaledAdvance;
                break;
            case HorizontalAlignment.Center:
                offsetX = -(maxScaledAdvance * .5F);
                break;
        }

        // Set the alignment of lines within the text.
        if (direction == TextDirection.LeftToRight)
            switch (options.TextAlignment)
            {
                case TextAlignment.End:
                    offsetX += maxScaledAdvance - textLine.ScaledLineAdvance;
                    break;
                case TextAlignment.Center:
                    offsetX += maxScaledAdvance * .5F - textLine.ScaledLineAdvance * .5F;
                    break;
            }
        else
            switch (options.TextAlignment)
            {
                case TextAlignment.Start:
                    offsetX += maxScaledAdvance - textLine.ScaledLineAdvance;
                    break;
                case TextAlignment.Center:
                    offsetX += maxScaledAdvance * .5F - textLine.ScaledLineAdvance * .5F;
                    break;
            }

        location.X += offsetX;

        List<GlyphLayout> glyphs = new();
        for (var i = 0; i < textLine.Count; i++)
        {
            var data = textLine[i];
            if (data.IsNewLine)
            {
                location.Y += scaledLineAdvance;
                continue;
            }

            foreach (var metric in data.Metrics)
            {
                // Advance Width & Height can be 0 which is fine for layout but not for measuring.
                var scale = new Vector2(data.PointSize) / metric.ScaleFactor;
                var advanceX = data.ScaledAdvance;
                var advanceY = metric.AdvanceHeight * scale.Y;
                if (advanceX == 0) advanceX = (metric.LeftSideBearing + metric.Width) * scale.X;

                if (advanceY == 0) advanceY = (metric.TopSideBearing + metric.Height) * scale.Y;

                glyphs.Add(new GlyphLayout(
                    new Glyph(metric, data.PointSize),
                    location,
                    scaledMaxAscender,
                    scaledMaxDescender,
                    scaledMaxLineGap,
                    scaledLineAdvance,
                    advanceX,
                    advanceY,
                    i == 0));
            }

            location.X += data.ScaledAdvance;
        }

        location.X = originX;
        if (glyphs.Count > 0) location.Y += scaledLineAdvance;

        return glyphs;
    }

    private static IEnumerable<GlyphLayout> LayoutLineVertical(
        TextBox textBox,
        TextLine textLine,
        TextDirection direction,
        float maxScaledAdvance,
        TextOptions options,
        int index,
        ref Vector2 location)
    {
        var originY = location.Y;
        float offsetY = 0;
        float offsetX = 0;

        // Set the Y-Origin for the line.
        var scaledMaxLineGap = textBox.ScaledMaxLineGap(textLine.MaxPointSize);
        var scaledMaxAscender = textBox.ScaledMaxAscender(textLine.MaxPointSize);
        var scaledMaxDescender = textBox.ScaledMaxDescender(textLine.MaxPointSize);
        var scaledMaxLineHeight = textBox.ScaledMaxLineHeight(textLine.MaxPointSize);

        switch (options.VerticalAlignment)
        {
            case VerticalAlignment.Top:
                offsetY = 0;
                break;
            case VerticalAlignment.Center:
                offsetY -= maxScaledAdvance * .5F;
                break;
            case VerticalAlignment.Bottom:
                offsetY -= maxScaledAdvance;
                break;
        }

        // Set the alignment of lines within the text.
        if (direction == TextDirection.LeftToRight)
            switch (options.TextAlignment)
            {
                case TextAlignment.End:
                    offsetY += maxScaledAdvance - textLine.ScaledLineAdvance;
                    break;
                case TextAlignment.Center:
                    offsetY += maxScaledAdvance * .5F - textLine.ScaledLineAdvance * .5F;
                    break;
            }
        else
            switch (options.TextAlignment)
            {
                case TextAlignment.Start:
                    offsetY += maxScaledAdvance - textLine.ScaledLineAdvance;
                    break;
                case TextAlignment.Center:
                    offsetY += maxScaledAdvance * .5F - textLine.ScaledLineAdvance * .5F;
                    break;
            }

        location.Y += offsetY;

        var isFirstLine = index == 0;
        if (isFirstLine)
            // Set the X-Origin for horizontal alignment.
            switch (options.HorizontalAlignment)
            {
                case HorizontalAlignment.Right:
                    // The textline methods are memoized so we're safe to call them multiple times.
                    for (var i = 0; i < textBox.TextLines.Count; i++)
                        offsetX -= textBox.ScaledMaxLineHeight(textBox.TextLines[i].MaxPointSize) * options.LineSpacing;

                    break;
                case HorizontalAlignment.Center:
                    for (var i = 0; i < textBox.TextLines.Count; i++)
                        offsetX -= textBox.ScaledMaxLineHeight(textBox.TextLines[i].MaxPointSize) *
                                   options.LineSpacing * .5F;

                    break;
            }

        location.X += offsetX;

        List<GlyphLayout> glyphs = new();
        var xWidth = scaledMaxLineHeight * (isFirstLine ? 1F : options.LineSpacing);
        var xLineAdvance = scaledMaxLineHeight * options.LineSpacing;

        if (isFirstLine) xLineAdvance -= (xLineAdvance - scaledMaxLineHeight) * .5F;

        for (var i = 0; i < textLine.Count; i++)
        {
            var data = textLine[i];
            if (data.IsNewLine)
            {
                location.X += xLineAdvance;
                location.Y = originY;
                continue;
            }

            foreach (var metric in data.Metrics)
            {
                var scale = new Vector2(data.PointSize) / metric.ScaleFactor;
                var advanceX = xLineAdvance;
                var advanceY = data.ScaledAdvance;

                // Advance Width & Height can be 0 which is fine for layout but not for measuring.
                if (advanceX == 0) advanceX = (metric.LeftSideBearing + metric.Width) * scale.X;

                if (advanceY == 0) advanceY = (metric.TopSideBearing + metric.Height) * scale.Y;

                glyphs.Add(new GlyphLayout(
                    new Glyph(metric, data.PointSize),
                    location + new Vector2((xWidth - metric.AdvanceWidth * scale.X) * .5F, data.ScaledAscender),
                    scaledMaxAscender,
                    scaledMaxDescender,
                    scaledMaxLineGap,
                    scaledMaxLineHeight,
                    advanceX,
                    advanceY,
                    i == 0));
            }

            location.Y += data.ScaledAdvance;
        }

        location.Y = originY;
        if (glyphs.Count > 0) location.X += xLineAdvance;

        return glyphs;
    }

    private static bool DoFontRun(
        ReadOnlySpan<char> text,
        int start,
        IReadOnlyList<TextRun> textRuns,
        ref int textRunIndex,
        ref int codePointIndex,
        ref int bidiRunIndex,
        bool isFallbackRun,
        Font font,
        BidiRun[] bidiRuns,
        Dictionary<int, int> bidiMap,
        GlyphSubstitutionCollection substitutions,
        GlyphPositioningCollection positionings)
    {
        // For each run we start with a fresh substitution collection to avoid
        // overwriting the glyph ids.
        substitutions.Clear();

        // Enumerate through each grapheme in the text.
        var graphemeIndex = start;
        var graphemeEnumerator = new SpanGraphemeEnumerator(text);
        while (graphemeEnumerator.MoveNext())
        {
            var graphemeMax = graphemeEnumerator.Current.Length - 1;
            var graphemeCodePointIndex = 0;
            var charIndex = 0;

            if (graphemeIndex == textRuns[textRunIndex].End) textRunIndex++;

            // Now enumerate through each codepoint in the grapheme.
            var skipNextCodePoint = false;
            var codePointEnumerator = new SpanCodePointEnumerator(graphemeEnumerator.Current);
            while (codePointEnumerator.MoveNext())
            {
                if (codePointIndex == bidiRuns[bidiRunIndex].End) bidiRunIndex++;

                if (skipNextCodePoint)
                {
                    codePointIndex++;
                    graphemeCodePointIndex++;
                    continue;
                }

                bidiMap[codePointIndex] = bidiRunIndex;

                var charsConsumed = 0;
                var current = codePointEnumerator.Current;
                charIndex += current.Utf16SequenceLength;
                CodePoint? next = graphemeCodePointIndex < graphemeMax
                    ? CodePoint.DecodeFromUtf16At(graphemeEnumerator.Current, charIndex, out charsConsumed)
                    : null;

                charIndex += charsConsumed;

                // Get the glyph id for the codepoint and add to the collection.
                font.FontMetrics.TryGetGlyphId(current, next, out var glyphId, out skipNextCodePoint);
                substitutions.AddGlyph(glyphId, current, (TextDirection)bidiRuns[bidiRunIndex].Direction,
                    textRuns[textRunIndex], codePointIndex);

                codePointIndex++;
                graphemeCodePointIndex++;
            }

            graphemeIndex++;
        }

        // Apply the simple and complex substitutions.
        // TODO: Investigate HarfBuzz normalizer.
        SubstituteBidiMirrors(font.FontMetrics, substitutions);
        font.FontMetrics.ApplySubstitution(substitutions);

        return !isFallbackRun
            ? positionings.TryAdd(font, substitutions)
            : positionings.TryUpdate(font, substitutions);
    }

    private static void SubstituteBidiMirrors(FontMetrics fontMetrics, GlyphSubstitutionCollection collection)
    {
        for (var i = 0; i < collection.Count; i++)
        {
            var data = collection.GetGlyphShapingData(i);

            if (data.Direction != TextDirection.RightToLeft) continue;

            if (!CodePoint.TryGetBidiMirror(data.CodePoint, out var mirror)) continue;

            if (fontMetrics.TryGetGlyphId(mirror, out var glyphId)) collection.Replace(i, glyphId);
        }

        // TODO: This only replaces certain glyphs. We should investigate the specification further.
        // https://www.unicode.org/reports/tr50/#vertical_alternates
        if (collection.IsVerticalLayoutMode)
            for (var i = 0; i < collection.Count; i++)
            {
                var data = collection.GetGlyphShapingData(i);
                if (!CodePoint.TryGetVerticalMirror(data.CodePoint, out var mirror)) continue;

                if (fontMetrics.TryGetGlyphId(mirror, out var glyphId)) collection.Replace(i, glyphId);
            }
    }

    private static TextBox BreakLines(
        ReadOnlySpan<char> text,
        TextOptions options,
        BidiRun[] bidiRuns,
        Dictionary<int, int> bidiMap,
        GlyphPositioningCollection positionings,
        LayoutMode layoutMode)
    {
        var shouldWrap = options.WrappingLength > 0;
        var wrappingLength = shouldWrap ? options.WrappingLength / options.Dpi : float.MaxValue;
        var breakAll = options.WordBreaking == WordBreaking.BreakAll;
        var keepAll = options.WordBreaking == WordBreaking.KeepAll;
        var isHorizontal = !layoutMode.IsVertical();

        // Calculate the position of potential line breaks.
        var lineBreakEnumerator = new LineBreakEnumerator(text);
        List<LineBreak> lineBreaks = new();
        while (lineBreakEnumerator.MoveNext()) lineBreaks.Add(lineBreakEnumerator.Current);

        var lineBreakIndex = 0;
        var lastLineBreak = lineBreaks[lineBreakIndex];
        var currentLineBreak = lineBreaks[lineBreakIndex];
        int graphemeIndex;
        var codePointIndex = 0;
        float lineAdvance = 0;
        List<TextLine> textLines = new();
        TextLine textLine = new();
        var glyphCount = 0;

        // Enumerate through each grapheme in the text.
        var graphemeEnumerator = new SpanGraphemeEnumerator(text);
        for (graphemeIndex = 0; graphemeEnumerator.MoveNext(); graphemeIndex++)
        {
            // Now enumerate through each codepoint in the grapheme.
            var graphemeCodePointIndex = 0;
            var codePointEnumerator = new SpanCodePointEnumerator(graphemeEnumerator.Current);
            while (codePointEnumerator.MoveNext())
            {
                if (!positionings.TryGetGlyphMetricsAtOffset(codePointIndex, out var pointSize, out var isDecomposed,
                        out var metrics))
                {
                    // Codepoint was skipped during original enumeration.
                    codePointIndex++;
                    graphemeCodePointIndex++;
                    continue;
                }

                var codePoint = codePointEnumerator.Current;
                if (CodePoint.IsVariationSelector(codePoint))
                {
                    codePointIndex++;
                    graphemeCodePointIndex++;
                    continue;
                }

                // Calculate the advance for the current codepoint.
                var glyph = metrics[0];
                float glyphAdvance = isHorizontal ? glyph.AdvanceWidth : glyph.AdvanceHeight;
                if (CodePoint.IsTabulation(codePoint))
                {
                    glyphAdvance *= options.TabWidth;
                }
                else if (metrics.Count == 1 &&
                         (CodePoint.IsZeroWidthJoiner(codePoint) || CodePoint.IsZeroWidthNonJoiner(codePoint)))
                {
                    // The zero-width joiner characters should be ignored when determining word or
                    // line break boundaries so are safe to skip here. Any existing instances are the result of font error
                    // unless multiple metrics are associated with code point. In this case they are most likely the result
                    // of a substitution and shouldn't be ignored.
                    glyphAdvance = 0;
                }
                else if (!CodePoint.IsNewLine(codePoint))
                {
                    // Standard text.
                    // If decomposed we need to add the advance; otherwise, use the largest advance for the metrics.
                    if (isHorizontal)
                        for (var i = 1; i < metrics.Count; i++)
                        {
                            float a = metrics[i].AdvanceWidth;
                            if (isDecomposed)
                                glyphAdvance += a;
                            else if (a > glyphAdvance) glyphAdvance = a;
                        }
                    else
                        for (var i = 1; i < metrics.Count; i++)
                        {
                            float a = metrics[i].AdvanceHeight;
                            if (isDecomposed)
                                glyphAdvance += a;
                            else if (a > glyphAdvance) glyphAdvance = a;
                        }
                }

                glyphAdvance *= pointSize / (isHorizontal ? glyph.ScaleFactor.X : glyph.ScaleFactor.Y);

                // Should we start a new line?
                var requiredBreak = false;
                if (graphemeCodePointIndex == 0)
                {
                    // Mandatory wrap at index.
                    if (currentLineBreak.PositionWrap == codePointIndex && currentLineBreak.Required)
                    {
                        textLines.Add(textLine.Finalize());
                        glyphCount += textLine.Count;
                        textLine = new TextLine();
                        lineAdvance = 0;
                        requiredBreak = true;
                    }
                    else if (shouldWrap && lineAdvance + glyphAdvance >= wrappingLength)
                    {
                        // Forced wordbreak
                        if (breakAll)
                        {
                            textLines.Add(textLine.Finalize());
                            glyphCount += textLine.Count;
                            textLine = new TextLine();
                            lineAdvance = 0;
                        }
                        else if (currentLineBreak.PositionMeasure == codePointIndex)
                        {
                            // Exact length match. Check for CJK
                            if (keepAll)
                            {
                                var split = textLine.SplitAt(lastLineBreak, keepAll);
                                if (split != textLine)
                                {
                                    textLines.Add(textLine.Finalize());
                                    textLine = split;
                                    lineAdvance = split.ScaledLineAdvance;
                                }
                            }
                            else
                            {
                                textLines.Add(textLine.Finalize());
                                glyphCount += textLine.Count;
                                textLine = new TextLine();
                                lineAdvance = 0;
                            }
                        }
                        else if (currentLineBreak.PositionWrap == codePointIndex)
                        {
                            // Exact length match. Check for CJK
                            var split = textLine.SplitAt(currentLineBreak, keepAll);
                            if (split != textLine)
                            {
                                textLines.Add(textLine.Finalize());
                                textLine = split;
                                lineAdvance = split.ScaledLineAdvance;
                            }
                        }
                        else if (lastLineBreak.PositionWrap < codePointIndex)
                        {
                            // Split the current textline into two at the last wrapping point.
                            var split = textLine.SplitAt(lastLineBreak, keepAll);
                            if (split != textLine)
                            {
                                textLines.Add(textLine.Finalize());
                                textLine = split;
                                lineAdvance = split.ScaledLineAdvance;
                            }
                        }
                    }
                }

                // Find the next line break.
                if (currentLineBreak.PositionWrap == codePointIndex)
                {
                    lastLineBreak = currentLineBreak;
                    currentLineBreak = lineBreaks[++lineBreakIndex];
                }

                // Do not start a line following a break with breaking whitespace
                // unless the break was required.
                if (textLine.Count == 0
                    && textLines.Count > 0
                    && !requiredBreak
                    && CodePoint.IsWhiteSpace(codePoint)
                    && !CodePoint.IsNonBreakingSpace(codePoint)
                    && !CodePoint.IsTabulation(codePoint)
                    && !CodePoint.IsNewLine(codePoint))
                {
                    codePointIndex++;
                    graphemeCodePointIndex++;
                    continue;
                }

                if (textLine.Count > 0 && CodePoint.IsNewLine(codePoint))
                {
                    // Do not add new lines unless at position zero.
                    codePointIndex++;
                    graphemeCodePointIndex++;
                    continue;
                }

                var metric = metrics[0];
                var scaleY = pointSize / metric.ScaleFactor.Y;
                var ascender = metric.FontMetrics.Ascender * scaleY;

                // Adjust ascender for glyphs with a negative tsb. e.g. emoji to prevent cutoff.
                short tsbOffset = 0;
                for (var i = 0; i < metrics.Count; i++) tsbOffset = Math.Min(tsbOffset, metrics[i].TopSideBearing);

                if (tsbOffset < 0) ascender -= tsbOffset * scaleY;

                var descender = Math.Abs(metric.FontMetrics.Descender * scaleY);
                var lineHeight = metric.FontMetrics.LineHeight * scaleY;
                var lineGap = lineHeight - (ascender + descender);

                // Add our metrics to the line.
                lineAdvance += glyphAdvance;
                textLine.Add(
                    metrics,
                    pointSize,
                    glyphAdvance,
                    lineHeight,
                    ascender,
                    descender,
                    lineGap,
                    bidiRuns[bidiMap[codePointIndex]],
                    graphemeIndex,
                    codePointIndex);

                codePointIndex++;
                graphemeCodePointIndex++;
            }
        }

        // Add the final line.
        if (textLine.Count > 0) textLines.Add(textLine.Finalize());

        return new TextBox(options, textLines);
    }

    internal sealed class TextBox
    {
        public TextBox(TextOptions options, IReadOnlyList<TextLine> textLines)
        {
            TextLines = textLines;
            for (var i = 0; i < TextLines.Count - 1; i++) TextLines[i].Justify(options);
        }

        public IReadOnlyList<TextLine> TextLines { get; }

        // TODO: It would be very good to cache these.
        public float ScaledMaxAdvance()
        {
            return TextLines.Max(x => x.ScaledLineAdvance);
        }

        public float ScaledMaxLineHeight(float pointSize)
        {
            return TextLines.Where(x => x.MaxPointSize == pointSize).Max(x => x.ScaledMaxLineHeight);
        }

        public float ScaledMaxAscender(float pointSize)
        {
            return TextLines.Where(x => x.MaxPointSize == pointSize).Max(x => x.ScaledMaxAscender);
        }

        public float ScaledMaxDescender(float pointSize)
        {
            return TextLines.Where(x => x.MaxPointSize == pointSize).Max(x => x.ScaledMaxDescender);
        }

        public float ScaledMaxLineGap(float pointSize)
        {
            return TextLines.Where(x => x.MaxPointSize == pointSize).Max(x => x.ScaledMaxLineGap);
        }

        public TextDirection TextDirection()
        {
            return TextLines[0][0].TextDirection;
        }
    }

    internal sealed class TextLine
    {
        private readonly List<GlyphLayoutData> data = new();

        public int Count => data.Count;

        public float MaxPointSize { get; private set; } = -1;

        public float ScaledLineAdvance { get; private set; }

        public float ScaledMaxLineHeight { get; private set; } = -1;

        public float ScaledMaxAscender { get; private set; } = -1;

        public float ScaledMaxDescender { get; private set; } = -1;

        public float ScaledMaxLineGap { get; private set; } = -1;

        public GlyphLayoutData this[int index] => data[index];

        public void Add(
            IReadOnlyList<GlyphMetrics> metrics,
            float pointSize,
            float scaledAdvance,
            float scaledLineHeight,
            float scaledAscender,
            float scaledDescender,
            float scaledLineGap,
            BidiRun bidiRun,
            int graphemeIndex,
            int offset)
        {
            // Reset metrics.
            // We track the maximum metrics for each line to ensure glyphs can be aligned.
            // These will be grouped by the point size for each run within the text to ensure
            // multi-line text maintains an even layout for equal point sizes.
            MaxPointSize = MathF.Max(MaxPointSize, pointSize);
            ScaledLineAdvance += scaledAdvance;
            ScaledMaxLineHeight = MathF.Max(ScaledMaxLineHeight, scaledLineHeight);
            ScaledMaxAscender = MathF.Max(ScaledMaxAscender, scaledAscender);
            ScaledMaxDescender = MathF.Max(ScaledMaxDescender, scaledDescender);
            ScaledMaxLineGap = MathF.Max(ScaledMaxLineGap, scaledLineGap);

            data.Add(new GlyphLayoutData(metrics, pointSize, scaledAdvance, scaledLineHeight, scaledAscender,
                scaledDescender, scaledLineGap, bidiRun, graphemeIndex, offset));
        }

        public TextLine SplitAt(LineBreak lineBreak, bool keepAll)
        {
            var index = data.Count;
            GlyphLayoutData glyphWrap = default;
            while (index > 0)
            {
                glyphWrap = data[--index];

                if (glyphWrap.Offset == lineBreak.PositionWrap) break;
            }

            if (index == 0) return this;

            // Word breaks should not be used for Chinese/Japanese/Korean (CJK) text
            // when word-breaking mode is keep-all.
            if (keepAll && UnicodeUtility.IsCJKCodePoint((uint)glyphWrap.CodePoint.Value))
            {
                // Loop through previous glyphs to see if there is
                // a non CJK codepoint we can break at.
                while (index > 0)
                {
                    glyphWrap = data[--index];
                    if (!UnicodeUtility.IsCJKCodePoint((uint)glyphWrap.CodePoint.Value))
                    {
                        index++;
                        break;
                    }
                }

                if (index == 0) return this;
            }

            // Create a new line ensuring we capture the intitial metrics.
            TextLine result = new();
            result.data.AddRange(data.GetRange(index, data.Count - index));
            result.ScaledLineAdvance = result.data.Sum(x => x.ScaledAdvance);
            result.MaxPointSize = result.data.Max(x => x.PointSize);
            result.ScaledMaxAscender = result.data.Max(x => x.ScaledAscender);
            result.ScaledMaxDescender = result.data.Max(x => x.ScaledDescender);
            result.ScaledMaxLineHeight = result.data.Max(x => x.ScaledLineHeight);
            result.ScaledMaxLineGap = result.data.Max(x => x.ScaledLineGap);

            // Remove those items from this line.
            data.RemoveRange(index, data.Count - index);

            // Now trim trailing whitespace from this line.
            index = data.Count;
            while (index > 0)
            {
                if (!CodePoint.IsWhiteSpace(data[index - 1].CodePoint)) break;

                index--;
            }

            if (index < data.Count) data.RemoveRange(index, data.Count - index);

            // Lastly recalculate this line metrics.
            ScaledLineAdvance = data.Sum(x => x.ScaledAdvance);
            MaxPointSize = data.Max(x => x.PointSize);
            ScaledMaxAscender = data.Max(x => x.ScaledAscender);
            ScaledMaxDescender = data.Max(x => x.ScaledDescender);
            ScaledMaxLineHeight = data.Max(x => x.ScaledLineHeight);
            ScaledMaxLineGap = data.Max(x => x.ScaledLineGap);

            return result;
        }

        public TextLine Finalize()
        {
            return BidiReOrder();
        }

        public void Justify(TextOptions options)
        {
            if (options.WrappingLength == -1F || options.TextJustification == TextJustification.None) return;

            if (ScaledLineAdvance == 0) return;

            var delta = options.WrappingLength / options.Dpi - ScaledLineAdvance;
            if (delta <= 0) return;

            // Increase the advance for all non zero-width glyphs but the last.
            if (options.TextJustification == TextJustification.InterCharacter)
            {
                var nonZeroCount = 0;
                for (var i = 0; i < data.Count - 1; i++)
                {
                    var glyph = data[i];
                    if (!CodePoint.IsZeroWidthJoiner(glyph.CodePoint) &&
                        !CodePoint.IsZeroWidthNonJoiner(glyph.CodePoint)) nonZeroCount++;
                }

                var padding = delta / nonZeroCount;
                for (var i = 0; i < data.Count - 1; i++)
                {
                    var glyph = data[i];
                    if (!CodePoint.IsZeroWidthJoiner(glyph.CodePoint) &&
                        !CodePoint.IsZeroWidthNonJoiner(glyph.CodePoint))
                    {
                        glyph.ScaledAdvance += padding;
                        data[i] = glyph;
                    }
                }

                return;
            }

            // Increase the advance for all spaces but the last.
            if (options.TextJustification == TextJustification.InterWord)
            {
                // Count all the whitespace characters.
                var whiteSpaceCount = 0;
                for (var i = 0; i < data.Count - 1; i++)
                {
                    var glyph = data[i];
                    if (CodePoint.IsWhiteSpace(glyph.CodePoint)) whiteSpaceCount++;
                }

                var padding = delta / whiteSpaceCount;
                for (var i = 0; i < data.Count - 1; i++)
                {
                    var glyph = data[i];
                    if (CodePoint.IsWhiteSpace(glyph.CodePoint))
                    {
                        glyph.ScaledAdvance += padding;
                        data[i] = glyph;
                    }
                }
            }
        }

        private TextLine BidiReOrder()
        {
            // Build up the collection of ordered runs.
            var run = data[0].BidiRun;
            OrderedBidiRun orderedRun = new(run.Level);
            var current = orderedRun;
            for (var i = 0; i < data.Count; i++)
            {
                var g = data[i];
                if (run != g.BidiRun)
                {
                    run = g.BidiRun;
                    current.Next = new OrderedBidiRun(run.Level);
                    current = current.Next;
                }

                current.Add(g);
            }

            // Reorder them into visual order.
            orderedRun = LinearReOrder(orderedRun);

            // Now perform a recursive reversal of each run.
            // From the highest level found in the text to the lowest odd level on each line, including intermediate levels
            // not actually present in the text, reverse any contiguous sequence of characters that are at that level or higher.
            // https://unicode.org/reports/tr9/#L2
            var max = 0;
            var min = int.MaxValue;
            for (var i = 0; i < data.Count; i++)
            {
                var level = data[i].BidiRun.Level;
                if (level > max) max = level;

                if ((level & 1) != 0 && level < min) min = level;
            }

            if (min > max) min = max;

            if (max == 0 || (min == max && (max & 1) == 0))
                // Nothing to reverse.
                return this;

            // Now apply the reversal and replace the original contents.
            var minLevelToReverse = max;
            while (minLevelToReverse >= min)
            {
                current = orderedRun;
                while (current != null)
                {
                    if (current.Level >= minLevelToReverse) current.Reverse();

                    current = current.Next;
                }

                minLevelToReverse--;
            }

            data.Clear();
            current = orderedRun;
            while (current != null)
            {
                data.AddRange(current.AsSlice());
                current = current.Next;
            }

            return this;
        }

        /// <summary>
        ///     Reorders a series of runs from logical to visual order, returning the left most run.
        ///     <see
        ///         href="https://github.com/fribidi/linear-reorder/blob/f2f872257d4d8b8e137fcf831f254d6d4db79d3c/linear-reorder.c" />
        /// </summary>
        /// <param name="line">The ordered bidi run.</param>
        /// <returns>The <see cref="OrderedBidiRun" />.</returns>
        private static OrderedBidiRun LinearReOrder(OrderedBidiRun? line)
        {
            BidiRange? range = null;
            var run = line;

            while (run != null)
            {
                var next = run.Next;

                while (range != null && range.Level > run.Level
                                     && range.Previous != null && range.Previous.Level >= run.Level)
                    range = BidiRange.MergeWithPrevious(range);

                if (range != null && range.Level >= run.Level)
                {
                    // Attach run to the range.
                    if ((run.Level & 1) != 0)
                    {
                        // Odd, range goes to the right of run.
                        run.Next = range.Left;
                        range.Left = run;
                    }
                    else
                    {
                        // Even, range goes to the left of run.
                        range.Right!.Next = run;
                        range.Right = run;
                    }

                    range.Level = run.Level;
                }
                else
                {
                    BidiRange r = new();
                    r.Left = r.Right = run;
                    r.Level = run.Level;
                    r.Previous = range;
                    range = r;
                }

                run = next;
            }

            while (range?.Previous != null) range = BidiRange.MergeWithPrevious(range);

            // Terminate.
            range!.Right!.Next = null;
            return range!.Left!;
        }

        [DebuggerDisplay("{DebuggerDisplay,nq}")]
        internal struct GlyphLayoutData
        {
            public GlyphLayoutData(
                IReadOnlyList<GlyphMetrics> metrics,
                float pointSize,
                float scaledAdvance,
                float scaledLineHeight,
                float scaledAscender,
                float scaledDescender,
                float scaledLineGap,
                BidiRun bidiRun,
                int graphemeIndex,
                int offset)
            {
                Metrics = metrics;
                PointSize = pointSize;
                ScaledAdvance = scaledAdvance;
                ScaledLineHeight = scaledLineHeight;
                ScaledAscender = scaledAscender;
                ScaledDescender = scaledDescender;
                ScaledLineGap = scaledLineGap;
                BidiRun = bidiRun;
                GraphemeIndex = graphemeIndex;
                Offset = offset;
            }

            public CodePoint CodePoint => Metrics[0].CodePoint;

            public IReadOnlyList<GlyphMetrics> Metrics { get; }

            public float PointSize { get; }

            public float ScaledAdvance { get; set; }

            public float ScaledLineHeight { get; }

            public float ScaledAscender { get; }

            public float ScaledDescender { get; }

            public float ScaledLineGap { get; }

            public BidiRun BidiRun { get; }

            public TextDirection TextDirection => (TextDirection)BidiRun.Direction;

            public int GraphemeIndex { get; }

            public int Offset { get; }

            public bool IsNewLine => CodePoint.IsNewLine(CodePoint);

            private string DebuggerDisplay => FormattableString
                .Invariant($"{CodePoint.ToDebuggerDisplay()} : {TextDirection} : {Offset}, level: {BidiRun.Level}");
        }

        private sealed class OrderedBidiRun
        {
            private ArrayBuilder<GlyphLayoutData> info;

            public OrderedBidiRun(int level)
            {
                Level = level;
            }

            public int Level { get; }

            public OrderedBidiRun? Next { get; set; }

            public void Add(GlyphLayoutData info)
            {
                this.info.Add(info);
            }

            public ArraySlice<GlyphLayoutData> AsSlice()
            {
                return info.AsSlice();
            }

            public void Reverse()
            {
                AsSlice().Span.Reverse();
            }
        }

        private sealed class BidiRange
        {
            public int Level { get; set; }

            public OrderedBidiRun? Left { get; set; }

            public OrderedBidiRun? Right { get; set; }

            public BidiRange? Previous { get; set; }

            public static BidiRange MergeWithPrevious(BidiRange? range)
            {
                var previous = range!.Previous!;
                BidiRange left;
                BidiRange right;

                if ((previous.Level & 1) != 0)
                {
                    // Odd, previous goes to the right of range.
                    left = range;
                    right = previous;
                }
                else
                {
                    // Even, previous goes to the left of range.
                    left = previous;
                    right = range;
                }

                // Stitch them
                left.Right!.Next = right.Left;
                previous.Left = left.Left;
                previous.Right = right.Right;

                return previous;
            }
        }
    }
}