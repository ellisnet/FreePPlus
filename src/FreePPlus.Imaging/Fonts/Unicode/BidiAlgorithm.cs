// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

#pragma warning disable CA1822
#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Unicode;

//was previously: namespace SixLabors.Fonts.Unicode;

/// <summary>
///     Implementation of Unicode Bidirection Algorithm (UAX #9)
///     https://unicode.org/reports/tr9/
/// </summary>
/// <remarks>
///     <para>
///         The Bidi algorithm uses a number of memory arrays for resolved
///         types, level information, bracket types, x9 removal maps and
///         more...
///     </para>
///     <para>
///         This implementation of the Bidi algorithm has been designed
///         to reduce memory pressure on the GC by re-using the same
///         work buffers, so instances of this class should be re-used
///         as much as possible.
///     </para>
/// </remarks>
internal sealed class BidiAlgorithm
{
    /// <summary>
    ///     Maximum pairing depth for paired brackets
    /// </summary>
    private const int MaxPairedBracketDepth = 63;

    /// <summary>
    ///     Two directional mapping of isolate start/end pairs
    /// </summary>
    /// <remarks>
    ///     The forward mapping maps the start index to the end index.
    ///     The reverse mapping maps the end index to the start index.
    /// </remarks>
    private readonly BidiDictionary<int, int> isolatePairs = new();

    /// <summary>
    ///     Re-usable list of level runs
    /// </summary>
    private readonly List<LevelRun> levelRuns = new();

    /// <summary>
    ///     Resolved list of paired brackets
    /// </summary>
    private readonly List<BracketPair> pairedBrackets = new();

    /// <summary>
    ///     A stack of pending isolate openings used by FindIsolatePairs()
    /// </summary>
    private readonly Stack<int> pendingIsolateOpenings = new();

    /// <summary>
    ///     Reusable list of pending opening brackets used by the
    ///     LocatePairedBrackets method
    /// </summary>
    private readonly List<int> pendingOpeningBrackets = new();

    /// <summary>
    ///     The status stack used during resolution of explicit
    ///     embedding and isolating runs
    /// </summary>
    private readonly Stack<Status> statusStack = new();

    /// <summary>
    ///     Try if the incoming data is known to contain brackets
    /// </summary>
    private bool hasBrackets;

    /// <summary>
    ///     True if the incoming data is known to contain embedding runs
    /// </summary>
    private bool hasEmbeddings;

    /// <summary>
    ///     True if the incoming data is known to contain isolating runs
    /// </summary>
    private bool hasIsolates;

    /// <summary>
    ///     Mapping for the current isolating sequence, built
    ///     by joining level runs from the x9 map.
    /// </summary>
    private ArrayBuilder<int> isolatedRunMapping;

    /// <summary>
    ///     The original BidiCharacterType types as provided by the caller
    /// </summary>
    private ReadOnlyArraySlice<BidiCharacterType> originalTypes;

    /// <summary>
    ///     Paired bracket types as provided by caller
    /// </summary>
    private ReadOnlyArraySlice<BidiPairedBracketType> pairedBracketTypes;

    /// <summary>
    ///     Paired bracket values as provided by caller
    /// </summary>
    private ReadOnlyArraySlice<int> pairedBracketValues;

    /// <summary>
    ///     The resolve paragraph embedding level
    /// </summary>
    private sbyte paragraphEmbeddingLevel;

    /// <summary>
    ///     The buffer underlying resolvedLevels
    /// </summary>
    private ArrayBuilder<sbyte> resolvedLevelsBuffer;

    /// <summary>
    ///     A mapped slice of the paired bracket types of the isolating
    ///     run currently being processed
    /// </summary>
    private MappedArraySlice<BidiPairedBracketType> runBidiPairedBracketTypes;

    /// <summary>
    ///     The direction of the isolating run currently being processed
    /// </summary>
    private BidiCharacterType runDirection;

    /// <summary>
    ///     The length of the isolating run currently being processed
    /// </summary>
    private int runLength;

    /// <summary>
    ///     The level of the isolating run currently being processed
    /// </summary>
    private int runLevel;

    /// <summary>
    ///     A mapped slice of the run levels for the isolating run currently
    ///     being processed
    /// </summary>
    private MappedArraySlice<sbyte> runLevels;

    /// <summary>
    ///     A mapped slice of the original types for the isolating run currently
    ///     being processed
    /// </summary>
    private MappedArraySlice<BidiCharacterType> runOriginalTypes;

    /// <summary>
    ///     A mapped slice of the paired bracket values of the isolating
    ///     run currently being processed
    /// </summary>
    private MappedArraySlice<int> runPairedBracketValues;

    /// <summary>
    ///     A mapped slice of the resolved types for the isolating run currently
    ///     being processed
    /// </summary>
    private MappedArraySlice<BidiCharacterType> runResolvedTypes;

    /// <summary>
    ///     The working BidiCharacterType types
    /// </summary>
    private ArraySlice<BidiCharacterType> workingTypes;

    /// <summary>
    ///     The buffer underlying _workingTypes
    /// </summary>
    private ArrayBuilder<BidiCharacterType> workingTypesBuffer;

    /// <summary>
    ///     Mapping used to virtually remove characters for rule X9
    /// </summary>
    private ArrayBuilder<int> x9Map;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BidiAlgorithm" /> class.
    /// </summary>
    public BidiAlgorithm() { }

    /// <summary>
    ///     Gets a per-thread instance that can be re-used as often
    ///     as necessary.
    /// </summary>
    public static ThreadLocal<BidiAlgorithm> Instance { get; } = new(() => new BidiAlgorithm());

    /// <summary>
    ///     Gets the resolved levels.
    /// </summary>
    public ArraySlice<sbyte> ResolvedLevels { get; private set; }

    /// <summary>
    ///     Gets the resolved paragraph embedding level
    /// </summary>
    public int ResolvedParagraphEmbeddingLevel => paragraphEmbeddingLevel;

    /// <summary>
    ///     Process data from a BidiData instance
    /// </summary>
    /// <param name="data">The Bidi Unicode data.</param>
    public void Process(BidiData data)
    {
        Process(
            data.Types,
            data.PairedBracketTypes,
            data.PairedBracketValues,
            data.ParagraphEmbeddingLevel,
            data.HasBrackets,
            data.HasEmbeddings,
            data.HasIsolates,
            null);
    }

    /// <summary>
    ///     Processes Bidi Data
    /// </summary>
    public void Process(
        ReadOnlyArraySlice<BidiCharacterType> types,
        ReadOnlyArraySlice<BidiPairedBracketType> pairedBracketTypes,
        ReadOnlyArraySlice<int> pairedBracketValues,
        sbyte paragraphEmbeddingLevel,
        bool? hasBrackets,
        bool? hasEmbeddings,
        bool? hasIsolates,
        ArraySlice<sbyte>? outLevels)
    {
        // Reset state
        isolatePairs.Clear();
        workingTypesBuffer.Clear();
        levelRuns.Clear();
        resolvedLevelsBuffer.Clear();

        // Setup original types and working types
        originalTypes = types;
        workingTypes = workingTypesBuffer.Add(types);

        // Capture paired bracket values and types
        this.pairedBracketTypes = pairedBracketTypes;
        this.pairedBracketValues = pairedBracketValues;

        // Store things we know
        this.hasBrackets = hasBrackets ?? this.pairedBracketTypes.Length == originalTypes.Length;
        this.hasEmbeddings = hasEmbeddings ?? true;
        this.hasIsolates = hasIsolates ?? true;

        // Find all isolate pairs
        FindIsolatePairs();

        // Resolve the paragraph embedding level
        if (paragraphEmbeddingLevel == 2)
            this.paragraphEmbeddingLevel = ResolveEmbeddingLevel(originalTypes);
        else
            this.paragraphEmbeddingLevel = paragraphEmbeddingLevel;

        // Create resolved levels buffer
        if (outLevels.HasValue)
        {
            if (outLevels.Value.Length != originalTypes.Length)
                throw new ArgumentException("Out levels must be the same length as the input data");

            ResolvedLevels = outLevels.Value;
        }
        else
        {
            ResolvedLevels = resolvedLevelsBuffer.Add(originalTypes.Length);
            ResolvedLevels.Fill(this.paragraphEmbeddingLevel);
        }

        // Resolve explicit embedding levels (Rules X1-X8)
        ResolveExplicitEmbeddingLevels();

        // Build the rule X9 map
        BuildX9RemovalMap();

        // Process all isolated run sequences
        ProcessIsolatedRunSequences();

        // Reset whitespace levels
        ResetWhitespaceLevels();

        // Clean up
        AssignLevelsToCodePointsRemovedByX9();
    }

    /// <summary>
    ///     Resolve the paragraph embedding level if not explicitly passed
    ///     by the caller. Also used by rule X5c for FSI isolating sequences.
    /// </summary>
    /// <param name="data">The data to be evaluated</param>
    /// <returns>The resolved embedding level</returns>
    public sbyte ResolveEmbeddingLevel(ReadOnlyArraySlice<BidiCharacterType> data)
    {
        // P2
        for (var i = 0; i < data.Length; ++i)
            switch (data[i])
            {
                case BidiCharacterType.LeftToRight:
                    // P3
                    return 0;

                case BidiCharacterType.ArabicLetter:
                case BidiCharacterType.RightToLeft:
                    // P3
                    return 1;

                case BidiCharacterType.FirstStrongIsolate:
                case BidiCharacterType.LeftToRightIsolate:
                case BidiCharacterType.RightToLeftIsolate:
                    // Skip isolate pairs
                    // (Because we're working with a slice, we need to adjust the indices
                    //  we're using for the isolatePairs map)
                    if (isolatePairs.TryGetValue(data.Start + i, out i))
                        i -= data.Start;
                    else
                        i = data.Length;

                    break;
            }

        // P3
        return 0;
    }

    /// <summary>
    ///     Build a list of matching isolates for a directionality slice
    ///     Implements BD9
    /// </summary>
    private void FindIsolatePairs()
    {
        // Redundant?
        if (!hasIsolates) return;

        // Lets double check this as we go and clear the flag
        // if there actually aren't any isolate pairs as this might
        // mean we can skip some later steps
        hasIsolates = false;

        // BD9...
        pendingIsolateOpenings.Clear();
        for (var i = 0; i < originalTypes.Length; i++)
        {
            var t = originalTypes[i];
            if (t is BidiCharacterType.LeftToRightIsolate
                or BidiCharacterType.RightToLeftIsolate
                or BidiCharacterType.FirstStrongIsolate)
            {
                pendingIsolateOpenings.Push(i);
                hasIsolates = true;
            }
            else if (t == BidiCharacterType.PopDirectionalIsolate)
            {
                if (pendingIsolateOpenings.Count > 0) isolatePairs.Add(pendingIsolateOpenings.Pop(), i);

                hasIsolates = true;
            }
        }
    }

    /// <summary>
    ///     Resolve the explicit embedding levels from the original
    ///     data.  Implements rules X1 to X8.
    /// </summary>
    private void ResolveExplicitEmbeddingLevels()
    {
        // Redundant?
        if (!hasIsolates && !hasEmbeddings) return;

        // Work variables
        statusStack.Clear();
        var overflowIsolateCount = 0;
        var overflowEmbeddingCount = 0;
        var validIsolateCount = 0;

        // Constants
        const int maxStackDepth = 125;

        // Rule X1 - setup initial state
        statusStack.Clear();

        // Neutral
        statusStack.Push(new Status(paragraphEmbeddingLevel, BidiCharacterType.OtherNeutral, false));

        // Process all characters
        for (var i = 0; i < originalTypes.Length; i++)
            switch (originalTypes[i])
            {
                case BidiCharacterType.RightToLeftEmbedding:
                {
                    // Rule X2
                    var newLevel = (sbyte)((statusStack.Peek().EmbeddingLevel + 1) | 1);
                    if (newLevel <= maxStackDepth && overflowIsolateCount == 0 && overflowEmbeddingCount == 0)
                    {
                        statusStack.Push(new Status(newLevel, BidiCharacterType.OtherNeutral, false));
                        ResolvedLevels[i] = newLevel;
                    }
                    else if (overflowIsolateCount == 0)
                    {
                        overflowEmbeddingCount++;
                    }

                    break;
                }

                case BidiCharacterType.LeftToRightEmbedding:
                {
                    // Rule X3
                    var newLevel = (sbyte)((statusStack.Peek().EmbeddingLevel + 2) & ~1);
                    if (newLevel < maxStackDepth && overflowIsolateCount == 0 && overflowEmbeddingCount == 0)
                    {
                        statusStack.Push(new Status(newLevel, BidiCharacterType.OtherNeutral, false));
                        ResolvedLevels[i] = newLevel;
                    }
                    else if (overflowIsolateCount == 0)
                    {
                        overflowEmbeddingCount++;
                    }

                    break;
                }

                case BidiCharacterType.RightToLeftOverride:
                {
                    // Rule X4
                    var newLevel = (sbyte)((statusStack.Peek().EmbeddingLevel + 1) | 1);
                    if (newLevel <= maxStackDepth && overflowIsolateCount == 0 && overflowEmbeddingCount == 0)
                    {
                        statusStack.Push(new Status(newLevel, BidiCharacterType.RightToLeft, false));
                        ResolvedLevels[i] = newLevel;
                    }
                    else if (overflowIsolateCount == 0)
                    {
                        overflowEmbeddingCount++;
                    }

                    break;
                }

                case BidiCharacterType.LeftToRightOverride:
                {
                    // Rule X5
                    var newLevel = (sbyte)((statusStack.Peek().EmbeddingLevel + 2) & ~1);
                    if (newLevel <= maxStackDepth && overflowIsolateCount == 0 && overflowEmbeddingCount == 0)
                    {
                        statusStack.Push(new Status(newLevel, BidiCharacterType.LeftToRight, false));
                        ResolvedLevels[i] = newLevel;
                    }
                    else if (overflowIsolateCount == 0)
                    {
                        overflowEmbeddingCount++;
                    }

                    break;
                }

                case BidiCharacterType.RightToLeftIsolate:
                case BidiCharacterType.LeftToRightIsolate:
                case BidiCharacterType.FirstStrongIsolate:
                {
                    // Rule X5a, X5b and X5c
                    var resolvedIsolate = originalTypes[i];

                    if (resolvedIsolate == BidiCharacterType.FirstStrongIsolate)
                    {
                        if (!isolatePairs.TryGetValue(i, out var endOfIsolate)) endOfIsolate = originalTypes.Length;

                        // Rule X5c
                        var start = i + 1;
                        resolvedIsolate = ResolveEmbeddingLevel(originalTypes[start..endOfIsolate]) == 1
                            ? BidiCharacterType.RightToLeftIsolate
                            : BidiCharacterType.LeftToRightIsolate;
                    }

                    // Replace RLI's level with current embedding level
                    var tos = statusStack.Peek();
                    ResolvedLevels[i] = tos.EmbeddingLevel;

                    // Apply override
                    if (tos.OverrideStatus != BidiCharacterType.OtherNeutral) workingTypes[i] = tos.OverrideStatus;

                    // Work out new level
                    sbyte newLevel;
                    if (resolvedIsolate == BidiCharacterType.RightToLeftIsolate)
                        newLevel = (sbyte)((tos.EmbeddingLevel + 1) | 1);
                    else
                        newLevel = (sbyte)((tos.EmbeddingLevel + 2) & ~1);

                    // Valid?
                    if (newLevel <= maxStackDepth && overflowIsolateCount == 0 && overflowEmbeddingCount == 0)
                    {
                        validIsolateCount++;
                        statusStack.Push(new Status(newLevel, BidiCharacterType.OtherNeutral, true));
                    }
                    else
                    {
                        overflowIsolateCount++;
                    }

                    break;
                }

                case BidiCharacterType.BoundaryNeutral:
                {
                    // Mentioned in rule X6 - "for all types besides ..., BN, ..."
                    // no-op
                    break;
                }

                default:
                {
                    // Rule X6
                    var tos = statusStack.Peek();
                    ResolvedLevels[i] = tos.EmbeddingLevel;
                    if (tos.OverrideStatus != BidiCharacterType.OtherNeutral) workingTypes[i] = tos.OverrideStatus;

                    break;
                }

                case BidiCharacterType.PopDirectionalIsolate:
                {
                    // Rule X6a
                    if (overflowIsolateCount > 0)
                    {
                        overflowIsolateCount--;
                    }
                    else if (validIsolateCount != 0)
                    {
                        overflowEmbeddingCount = 0;
                        while (!statusStack.Peek().IsolateStatus) statusStack.Pop();

                        statusStack.Pop();
                        validIsolateCount--;
                    }

                    var tos = statusStack.Peek();
                    ResolvedLevels[i] = tos.EmbeddingLevel;
                    if (tos.OverrideStatus != BidiCharacterType.OtherNeutral) workingTypes[i] = tos.OverrideStatus;

                    break;
                }

                case BidiCharacterType.PopDirectionalFormat:
                {
                    // Rule X7
                    if (overflowIsolateCount == 0)
                    {
                        if (overflowEmbeddingCount > 0)
                            overflowEmbeddingCount--;
                        else if (!statusStack.Peek().IsolateStatus && statusStack.Count >= 2) statusStack.Pop();
                    }

                    break;
                }

                case BidiCharacterType.ParagraphSeparator:
                {
                    // Rule X8
                    ResolvedLevels[i] = paragraphEmbeddingLevel;
                    break;
                }
            }
    }

    /// <summary>
    ///     Build a map to the original data positions that excludes all
    ///     the types defined by rule X9
    /// </summary>
    private void BuildX9RemovalMap()
    {
        // Reserve room for the x9 map
        x9Map.Length = originalTypes.Length;

        if (hasEmbeddings || hasIsolates)
        {
            // Build a map the removes all x9 characters
            var j = 0;
            for (var i = 0; i < originalTypes.Length; i++)
                if (!IsRemovedByX9(originalTypes[i]))
                    x9Map[j++] = i;

            // Set the final length
            x9Map.Length = j;
        }
        else
        {
            for (int i = 0, count = originalTypes.Length; i < count; i++) x9Map[i] = i;
        }
    }

    /// <summary>
    ///     Find the original character index for an entry in the X9 map
    /// </summary>
    /// <param name="index">Index in the x9 removal map</param>
    /// <returns>Index to the original data</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MapX9(int index)
    {
        return x9Map[index];
    }

    /// <summary>
    ///     Add a new level run
    /// </summary>
    /// <remarks>
    ///     This method resolves the sos and eos values for the run
    ///     and adds the run to the list
    ///     ///
    /// </remarks>
    /// <param name="start">The index of the start of the run (in x9 removed units)</param>
    /// <param name="length">The length of the run (in x9 removed units)</param>
    /// <param name="level">The level of the run</param>
    private void AddLevelRun(int start, int length, int level)
    {
        // Get original indices to first and last character in this run
        var firstCharIndex = MapX9(start);
        var lastCharIndex = MapX9(start + length - 1);

        // Work out sos
        var i = firstCharIndex - 1;
        while (i >= 0 && IsRemovedByX9(originalTypes[i])) i--;

        var prevLevel = i < 0 ? paragraphEmbeddingLevel : ResolvedLevels[i];
        var sos = DirectionFromLevel(Math.Max(prevLevel, level));

        // Work out eos
        var lastType = workingTypes[lastCharIndex];
        int nextLevel;
        if (lastType is BidiCharacterType.LeftToRightIsolate
            or BidiCharacterType.RightToLeftIsolate
            or BidiCharacterType.FirstStrongIsolate)
        {
            nextLevel = paragraphEmbeddingLevel;
        }
        else
        {
            i = lastCharIndex + 1;
            while (i < originalTypes.Length && IsRemovedByX9(originalTypes[i])) i++;

            nextLevel = i >= originalTypes.Length ? paragraphEmbeddingLevel : ResolvedLevels[i];
        }

        var eos = DirectionFromLevel(Math.Max(nextLevel, level));

        // Add the run
        levelRuns.Add(new LevelRun(start, length, level, sos, eos));
    }

    /// <summary>
    ///     Find all runs of the same level, populating the _levelRuns
    ///     collection
    /// </summary>
    private void FindLevelRuns()
    {
        var currentLevel = -1;
        var runStart = 0;
        for (var i = 0; i < x9Map.Length; ++i)
        {
            int level = ResolvedLevels[MapX9(i)];
            if (level != currentLevel)
            {
                if (currentLevel != -1) AddLevelRun(runStart, i - runStart, currentLevel);

                currentLevel = level;
                runStart = i;
            }
        }

        // Don't forget the final level run
        if (currentLevel != -1) AddLevelRun(runStart, x9Map.Length - runStart, currentLevel);
    }

    /// <summary>
    ///     Given a character index, find the level run that starts at that position
    /// </summary>
    /// <param name="index">The index into the original (unmapped) data</param>
    /// <returns>The index of the run that starts at that index</returns>
    private int FindRunForIndex(int index)
    {
        for (var i = 0; i < levelRuns.Count; i++)
            // Passed index is for the original non-x9 filtered data, however
            // the level run ranges are for the x9 filtered data.  Convert before
            // comparing
            if (MapX9(levelRuns[i].Start) == index)
                return i;

        throw new InvalidOperationException("Internal error");
    }

    /// <summary>
    ///     Determine and the process all isolated run sequences
    /// </summary>
    private void ProcessIsolatedRunSequences()
    {
        // Find all runs with the same level
        FindLevelRuns();

        // Process them one at a time by first building
        // a mapping using slices from the x9 map for each
        // run section that needs to be joined together to
        // form an complete run.  That full run mapping
        // will be placed in _isolatedRunMapping and then
        // processed by ProcessIsolatedRunSequence().
        while (levelRuns.Count > 0)
        {
            // Clear the mapping
            isolatedRunMapping.Clear();

            // Combine mappings from this run and all runs that continue on from it
            var runIndex = 0;
            BidiCharacterType eos;
            var sos = levelRuns[0].Sos;
            var level = levelRuns[0].Level;
            while (true)
            {
                // Get the run
                var r = levelRuns[runIndex];

                // The eos of the isolating run is the eos of the
                // last level run that comprises it.
                eos = r.Eos;

                // Remove this run as we've now processed it
                levelRuns.RemoveAt(runIndex);

                // Add the x9 map indices for the run range to the mapping
                // for this isolated run
                isolatedRunMapping.Add(x9Map.AsSlice(r.Start, r.Length));

                // Get the last character and see if it's an isolating run with a matching
                // PDI and concatenate that run to this one
                var lastCharacterIndex = isolatedRunMapping[^1];
                var lastType = originalTypes[lastCharacterIndex];
                if ((lastType == BidiCharacterType.LeftToRightIsolate ||
                     lastType == BidiCharacterType.RightToLeftIsolate ||
                     lastType == BidiCharacterType.FirstStrongIsolate) &&
                    isolatePairs.TryGetValue(lastCharacterIndex, out var nextRunIndex))
                    // Find the continuing run index
                    runIndex = FindRunForIndex(nextRunIndex);
                else
                    break;
            }

            // Process this isolated run
            ProcessIsolatedRunSequence(sos, eos, level);
        }
    }

    /// <summary>
    ///     Process a single isolated run sequence, where the character sequence
    ///     mapping is currently held in _isolatedRunMapping.
    /// </summary>
    private void ProcessIsolatedRunSequence(BidiCharacterType sos, BidiCharacterType eos, int runLevel)
    {
        // Create mappings onto the underlying data
        runResolvedTypes = new MappedArraySlice<BidiCharacterType>(workingTypes, isolatedRunMapping.AsSlice());
        runOriginalTypes = new MappedArraySlice<BidiCharacterType>(originalTypes, isolatedRunMapping.AsSlice());
        runLevels = new MappedArraySlice<sbyte>(ResolvedLevels, isolatedRunMapping.AsSlice());
        if (hasBrackets)
        {
            runBidiPairedBracketTypes =
                new MappedArraySlice<BidiPairedBracketType>(pairedBracketTypes, isolatedRunMapping.AsSlice());
            runPairedBracketValues = new MappedArraySlice<int>(pairedBracketValues, isolatedRunMapping.AsSlice());
        }

        this.runLevel = runLevel;
        runDirection = DirectionFromLevel(runLevel);
        runLength = runResolvedTypes.Length;

        // By tracking the types of characters known to be in the current run, we can
        // skip some of the rules that we know won't apply.  The flags will be
        // initialized while we're processing rule W1 below.
        var hasEN = false;
        var hasAL = false;
        var hasES = false;
        var hasCS = false;
        var hasAN = false;
        var hasET = false;

        // Rule W1
        // Also, set hasXX flags
        int i;
        var prevType = sos;
        for (i = 0; i < runLength; i++)
        {
            var t = runResolvedTypes[i];
            switch (t)
            {
                case BidiCharacterType.NonspacingMark:
                    runResolvedTypes[i] = prevType;
                    break;

                case BidiCharacterType.LeftToRightIsolate:
                case BidiCharacterType.RightToLeftIsolate:
                case BidiCharacterType.FirstStrongIsolate:
                case BidiCharacterType.PopDirectionalIsolate:
                    prevType = BidiCharacterType.OtherNeutral;
                    break;

                case BidiCharacterType.EuropeanNumber:
                    hasEN = true;
                    prevType = t;
                    break;

                case BidiCharacterType.ArabicLetter:
                    hasAL = true;
                    prevType = t;
                    break;

                case BidiCharacterType.EuropeanSeparator:
                    hasES = true;
                    prevType = t;
                    break;

                case BidiCharacterType.CommonSeparator:
                    hasCS = true;
                    prevType = t;
                    break;

                case BidiCharacterType.ArabicNumber:
                    hasAN = true;
                    prevType = t;
                    break;

                case BidiCharacterType.EuropeanTerminator:
                    hasET = true;
                    prevType = t;
                    break;

                default:
                    prevType = t;
                    break;
            }
        }

        // Rule W2
        if (hasEN)
            for (i = 0; i < runLength; i++)
                if (runResolvedTypes[i] == BidiCharacterType.EuropeanNumber)
                    for (var j = i - 1; j >= 0; j--)
                    {
                        var t = runResolvedTypes[j];
                        if (t is BidiCharacterType.LeftToRight
                            or BidiCharacterType.RightToLeft
                            or BidiCharacterType.ArabicLetter)
                        {
                            if (t == BidiCharacterType.ArabicLetter)
                            {
                                runResolvedTypes[i] = BidiCharacterType.ArabicNumber;
                                hasAN = true;
                            }

                            break;
                        }
                    }

        // Rule W3
        if (hasAL)
            for (i = 0; i < runLength; i++)
                if (runResolvedTypes[i] == BidiCharacterType.ArabicLetter)
                    runResolvedTypes[i] = BidiCharacterType.RightToLeft;

        // Rule W4
        if ((hasES || hasCS) && (hasEN || hasAN))
            for (i = 1; i < runLength - 1; ++i)
            {
                ref var rt = ref runResolvedTypes[i];
                if (rt == BidiCharacterType.EuropeanSeparator)
                {
                    var prevSepType = runResolvedTypes[i - 1];
                    var succSepType = runResolvedTypes[i + 1];

                    if (prevSepType == BidiCharacterType.EuropeanNumber &&
                        succSepType == BidiCharacterType.EuropeanNumber)
                        // ES between EN and EN
                        rt = BidiCharacterType.EuropeanNumber;
                }
                else if (rt == BidiCharacterType.CommonSeparator)
                {
                    var prevSepType = runResolvedTypes[i - 1];
                    var succSepType = runResolvedTypes[i + 1];

                    if ((prevSepType == BidiCharacterType.ArabicNumber &&
                         succSepType == BidiCharacterType.ArabicNumber) ||
                        (prevSepType == BidiCharacterType.EuropeanNumber &&
                         succSepType == BidiCharacterType.EuropeanNumber))
                        // CS between (AN and AN) or (EN and EN)
                        rt = prevSepType;
                }
            }

        // Rule W5
        if (hasET && hasEN)
            for (i = 0; i < runLength; ++i)
                if (runResolvedTypes[i] == BidiCharacterType.EuropeanTerminator)
                {
                    // Locate end of sequence
                    var seqStart = i;
                    var seqEnd = i;
                    while (seqEnd < runLength && runResolvedTypes[seqEnd] == BidiCharacterType.EuropeanTerminator)
                        seqEnd++;

                    // Preceded by, or followed by EN?
                    if ((seqStart == 0 ? sos : runResolvedTypes[seqStart - 1]) == BidiCharacterType.EuropeanNumber
                        || (seqEnd == runLength ? eos : runResolvedTypes[seqEnd]) == BidiCharacterType.EuropeanNumber)
                        // Change the entire range
                        for (_ = seqStart; i < seqEnd; ++i)
                            runResolvedTypes[i] = BidiCharacterType.EuropeanNumber;

                    // continue at end of sequence
                    i = seqEnd;
                }

        // Rule W6
        if (hasES || hasET || hasCS)
            for (i = 0; i < runLength; ++i)
            {
                ref var t = ref runResolvedTypes[i];
                if (t is BidiCharacterType.EuropeanSeparator
                    or BidiCharacterType.EuropeanTerminator
                    or BidiCharacterType.CommonSeparator)
                    t = BidiCharacterType.OtherNeutral;
            }

        // Rule W7.
        if (hasEN)
        {
            var prevStrongType = sos;
            for (i = 0; i < runLength; ++i)
            {
                ref var rt = ref runResolvedTypes[i];
                if (rt == BidiCharacterType.EuropeanNumber)
                    // If prev strong type was an L change this to L too
                    if (prevStrongType == BidiCharacterType.LeftToRight)
                        runResolvedTypes[i] = BidiCharacterType.LeftToRight;

                // Remember previous strong type (NB: AL should already be changed to R)
                if (rt is BidiCharacterType.LeftToRight or BidiCharacterType.RightToLeft) prevStrongType = rt;
            }
        }

        // Rule N0 - process bracket pairs
        if (hasBrackets)
        {
            int count;
            var pairedBrackets = LocatePairedBrackets();
            for (i = 0, count = pairedBrackets.Count; i < count; i++)
            {
                var pb = pairedBrackets[i];
                var dir = InspectPairedBracket(pb);

                // Case "d" - no strong types in the brackets, ignore
                if (dir == BidiCharacterType.OtherNeutral) continue;

                // Case "b" - strong type found that matches the embedding direction
                if ((dir == BidiCharacterType.LeftToRight || dir == BidiCharacterType.RightToLeft) &&
                    dir == runDirection)
                {
                    SetPairedBracketDirection(pb, dir);
                    continue;
                }

                // Case "c" - found opposite strong type found, look before to establish context
                dir = InspectBeforePairedBracket(pb, sos);
                if (dir == runDirection || dir == BidiCharacterType.OtherNeutral) dir = runDirection;

                SetPairedBracketDirection(pb, dir);
            }
        }

        // Rules N1 and N2 - resolve neutral types
        for (i = 0; i < runLength; ++i)
        {
            var t = runResolvedTypes[i];
            if (IsNeutralType(t))
            {
                // Locate end of sequence
                var seqStart = i;
                var seqEnd = i;
                while (seqEnd < runLength && IsNeutralType(runResolvedTypes[seqEnd])) seqEnd++;

                // Work out the preceding type
                BidiCharacterType typeBefore;
                if (seqStart == 0)
                {
                    typeBefore = sos;
                }
                else
                {
                    typeBefore = runResolvedTypes[seqStart - 1];
                    if (typeBefore is BidiCharacterType.ArabicNumber or BidiCharacterType.EuropeanNumber)
                        typeBefore = BidiCharacterType.RightToLeft;
                }

                // Work out the following type
                BidiCharacterType typeAfter;
                if (seqEnd == runLength)
                {
                    typeAfter = eos;
                }
                else
                {
                    typeAfter = runResolvedTypes[seqEnd];
                    if (typeAfter is BidiCharacterType.ArabicNumber or BidiCharacterType.EuropeanNumber)
                        typeAfter = BidiCharacterType.RightToLeft;
                }

                // Work out the final resolved type
                BidiCharacterType resolvedType;
                if (typeBefore == typeAfter)
                    // Rule N1
                    resolvedType = typeBefore;
                else
                    // Rule N2
                    resolvedType = runDirection;

                // Apply changes
                for (var j = seqStart; j < seqEnd; j++) runResolvedTypes[j] = resolvedType;

                // continue after this run
                i = seqEnd;
            }
        }

        // Rules I1 and I2 - resolve implicit types
        if ((this.runLevel & 0x01) == 0)
            // Rule I1 - even
            for (i = 0; i < runLength; i++)
            {
                var t = runResolvedTypes[i];
                ref var l = ref runLevels[i];
                if (t == BidiCharacterType.RightToLeft)
                    l++;
                else if (t is BidiCharacterType.ArabicNumber or BidiCharacterType.EuropeanNumber) l += 2;
            }
        else
            // Rule I2 - odd
            for (i = 0; i < runLength; i++)
            {
                var t = runResolvedTypes[i];
                ref var l = ref runLevels[i];
                if (t != BidiCharacterType.RightToLeft) l++;
            }
    }

    /// <summary>
    ///     Locate all pair brackets in the current isolating run
    /// </summary>
    /// <returns>A sorted list of BracketPairs</returns>
    private List<BracketPair> LocatePairedBrackets()
    {
        // Clear work collections
        pendingOpeningBrackets.Clear();
        pairedBrackets.Clear();

        // Since List.Sort is expensive on memory if called often (it internally
        // allocates an ArraySorted object) and since we will rarely have many
        // items in this list (most paragraphs will only have a handful of bracket
        // pairs - if that), we use a simple linear lookup and insert most of the
        // time.  If there are more that `sortLimit` paired brackets we abort th
        // linear searching/inserting and using List.Sort at the end.
        const int sortLimit = 8;

        // Process all characters in the run, looking for paired brackets
        for (int ich = 0, length = runLength; ich < length; ich++)
        {
            // Ignore non-neutral characters
            if (runResolvedTypes[ich] != BidiCharacterType.OtherNeutral) continue;

            switch (runBidiPairedBracketTypes[ich])
            {
                case BidiPairedBracketType.Open:
                    if (pendingOpeningBrackets.Count == MaxPairedBracketDepth) goto exit;

                    pendingOpeningBrackets.Insert(0, ich);
                    break;

                case BidiPairedBracketType.Close:
                    // see if there is a match
                    for (var i = 0; i < pendingOpeningBrackets.Count; i++)
                        if (runPairedBracketValues[ich] == runPairedBracketValues[pendingOpeningBrackets[i]])
                        {
                            // Add this paired bracket set
                            var opener = pendingOpeningBrackets[i];
                            if (pairedBrackets.Count < sortLimit)
                            {
                                var ppi = 0;
                                while (ppi < pairedBrackets.Count && pairedBrackets[ppi].OpeningIndex < opener) ppi++;

                                pairedBrackets.Insert(ppi, new BracketPair(opener, ich));
                            }
                            else
                            {
                                pairedBrackets.Add(new BracketPair(opener, ich));
                            }

                            // remove up to and including matched opener
                            pendingOpeningBrackets.RemoveRange(0, i + 1);
                            break;
                        }

                    break;
            }
        }

        exit:

        // Is a sort pending?
        if (pairedBrackets.Count > sortLimit) pairedBrackets.Sort();

        return pairedBrackets;
    }

    /// <summary>
    ///     Inspect a paired bracket set and determine its strong direction
    /// </summary>
    /// <param name="pb">The paired bracket to be inspected</param>
    /// <returns>The direction of the bracket set content</returns>
    private BidiCharacterType InspectPairedBracket(in BracketPair pb)
    {
        var dirEmbed = DirectionFromLevel(runLevel);
        var dirOpposite = BidiCharacterType.OtherNeutral;
        for (var ich = pb.OpeningIndex + 1; ich < pb.ClosingIndex; ich++)
        {
            var dir = GetStrongTypeN0(runResolvedTypes[ich]);
            if (dir == BidiCharacterType.OtherNeutral) continue;

            if (dir == dirEmbed) return dir;

            dirOpposite = dir;
        }

        return dirOpposite;
    }

    /// <summary>
    ///     Look for a strong type before a paired bracket
    /// </summary>
    /// <param name="pb">The paired bracket set to be inspected</param>
    /// <param name="sos">The sos in case nothing found before the bracket</param>
    /// <returns>The strong direction before the brackets</returns>
    private BidiCharacterType InspectBeforePairedBracket(in BracketPair pb, BidiCharacterType sos)
    {
        for (var ich = pb.OpeningIndex - 1; ich >= 0; --ich)
        {
            var dir = GetStrongTypeN0(runResolvedTypes[ich]);
            if (dir != BidiCharacterType.OtherNeutral) return dir;
        }

        return sos;
    }

    /// <summary>
    ///     Sets the direction of a bracket pair, including setting the direction of
    ///     NSM's inside the brackets and following.
    /// </summary>
    /// <param name="pb">The paired brackets</param>
    /// <param name="dir">The resolved direction for the bracket pair</param>
    private void SetPairedBracketDirection(in BracketPair pb, BidiCharacterType dir)
    {
        // Set the direction of the brackets
        runResolvedTypes[pb.OpeningIndex] = dir;
        runResolvedTypes[pb.ClosingIndex] = dir;

        // Set the directionality of NSM's inside the brackets
        // BN  characters (such as ZWJ or ZWSP) that appear between the base bracket character
        // and the nonspacing mark should be ignored.
        for (var i = pb.OpeningIndex + 1; i < pb.ClosingIndex; i++)
            if (runOriginalTypes[i] == BidiCharacterType.NonspacingMark)
                runOriginalTypes[i] = dir;
            else if (runOriginalTypes[i] != BidiCharacterType.BoundaryNeutral) break;

        // Set the directionality of NSM's following the brackets
        for (var i = pb.ClosingIndex + 1; i < runLength; i++)
            if (runOriginalTypes[i] == BidiCharacterType.NonspacingMark)
                runResolvedTypes[i] = dir;
            else if (runOriginalTypes[i] != BidiCharacterType.BoundaryNeutral) break;
    }

    /// <summary>
    ///     Resets whitespace levels. Implements rule L1
    /// </summary>
    private void ResetWhitespaceLevels()
    {
        for (var i = 0; i < ResolvedLevels.Length; i++)
        {
            var t = originalTypes[i];
            if (t is BidiCharacterType.ParagraphSeparator or BidiCharacterType.SegmentSeparator)
            {
                // Rule L1, clauses one and two.
                ResolvedLevels[i] = paragraphEmbeddingLevel;

                // Rule L1, clause three.
                for (var j = i - 1; j >= 0; --j)
                    if (IsWhitespace(originalTypes[j]))
                        // including format codes
                        ResolvedLevels[j] = paragraphEmbeddingLevel;
                    else
                        break;
            }
        }

        // Rule L1, clause four.
        for (var j = ResolvedLevels.Length - 1; j >= 0; j--)
            if (IsWhitespace(originalTypes[j]))
                // including format codes
                ResolvedLevels[j] = paragraphEmbeddingLevel;
            else
                break;
    }

    /// <summary>
    ///     Assign levels to any characters that would be have been
    ///     removed by rule X9.  The idea is to keep level runs together
    ///     that would otherwise be broken by an interfering isolate/embedding
    ///     control character.
    /// </summary>
    private void AssignLevelsToCodePointsRemovedByX9()
    {
        // Redundant?
        if (!hasIsolates && !hasEmbeddings) return;

        // No-op?
        if (workingTypes.Length == 0) return;

        // Fix up first character
        if (ResolvedLevels[0] < 0) ResolvedLevels[0] = paragraphEmbeddingLevel;

        if (IsRemovedByX9(originalTypes[0])) workingTypes[0] = originalTypes[0];

        for (int i = 1, length = workingTypes.Length; i < length; i++)
        {
            var t = originalTypes[i];
            if (IsRemovedByX9(t))
            {
                workingTypes[i] = t;
                ResolvedLevels[i] = ResolvedLevels[i - 1];
            }
        }
    }

    /// <summary>
    ///     Check if a directionality type represents whitespace
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsWhitespace(BidiCharacterType biditype)
    {
        return biditype switch
        {
            BidiCharacterType.LeftToRightEmbedding
                or BidiCharacterType.RightToLeftEmbedding
                or BidiCharacterType.LeftToRightOverride
                or BidiCharacterType.RightToLeftOverride
                or BidiCharacterType.PopDirectionalFormat
                or BidiCharacterType.LeftToRightIsolate
                or BidiCharacterType.RightToLeftIsolate
                or BidiCharacterType.FirstStrongIsolate
                or BidiCharacterType.PopDirectionalIsolate
                or BidiCharacterType.BoundaryNeutral
                or BidiCharacterType.Whitespace => true,
            _ => false
        };
    }

    /// <summary>
    ///     Convert a level to a direction where odd is RTL and
    ///     even is LTR
    /// </summary>
    /// <param name="level">The level to convert</param>
    /// <returns>A directionality</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BidiCharacterType DirectionFromLevel(int level)
    {
        return (level & 0x1) == 0 ? BidiCharacterType.LeftToRight : BidiCharacterType.RightToLeft;
    }

    /// <summary>
    ///     Helper to check if a directionality is removed by rule X9
    /// </summary>
    /// <param name="biditype">The bidi type to check</param>
    /// <returns>True if rule X9 would remove this character; otherwise false</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsRemovedByX9(BidiCharacterType biditype)
    {
        return biditype switch
        {
            BidiCharacterType.LeftToRightEmbedding
                or BidiCharacterType.RightToLeftEmbedding
                or BidiCharacterType.LeftToRightOverride
                or BidiCharacterType.RightToLeftOverride
                or BidiCharacterType.PopDirectionalFormat
                or BidiCharacterType.BoundaryNeutral => true,
            _ => false
        };
    }

    /// <summary>
    ///     Check if a a directionality is neutral for rules N1 and N2
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsNeutralType(BidiCharacterType dir)
    {
        return dir switch
        {
            BidiCharacterType.ParagraphSeparator
                or BidiCharacterType.SegmentSeparator
                or BidiCharacterType.Whitespace
                or BidiCharacterType.OtherNeutral
                or BidiCharacterType.RightToLeftIsolate
                or BidiCharacterType.LeftToRightIsolate
                or BidiCharacterType.FirstStrongIsolate
                or BidiCharacterType.PopDirectionalIsolate => true,
            _ => false
        };
    }

    /// <summary>
    ///     Maps a direction to a strong type for rule N0
    /// </summary>
    /// <param name="dir">The direction to map</param>
    /// <returns>A strong direction - R, L or ON</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private BidiCharacterType GetStrongTypeN0(BidiCharacterType dir)
    {
        return dir switch
        {
            BidiCharacterType.EuropeanNumber
                or BidiCharacterType.ArabicNumber
                or BidiCharacterType.ArabicLetter
                or BidiCharacterType.RightToLeft => BidiCharacterType.RightToLeft,
            BidiCharacterType.LeftToRight => BidiCharacterType.LeftToRight,
            _ => BidiCharacterType.OtherNeutral
        };
    }

    /// <summary>
    ///     Hold the start and end index of a pair of brackets
    /// </summary>
    private readonly struct BracketPair : IComparable<BracketPair>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BracketPair" /> struct.
        /// </summary>
        /// <param name="openingIndex">Index of the opening bracket</param>
        /// <param name="closingIndex">Index of the closing bracket</param>
        public BracketPair(int openingIndex, int closingIndex)
        {
            OpeningIndex = openingIndex;
            ClosingIndex = closingIndex;
        }

        /// <summary>
        ///     Gets the index of the opening bracket
        /// </summary>
        public int OpeningIndex { get; }

        /// <summary>
        ///     Gets the index of the closing bracket
        /// </summary>
        public int ClosingIndex { get; }

        public int CompareTo(BracketPair other)
        {
            return OpeningIndex.CompareTo(other.OpeningIndex);
        }
    }

    /// <summary>
    ///     Status stack entry used while resolving explicit
    ///     embedding levels
    /// </summary>
    private readonly struct Status
    {
        public Status(sbyte embeddingLevel, BidiCharacterType overrideStatus, bool isolateStatus)
        {
            EmbeddingLevel = embeddingLevel;
            OverrideStatus = overrideStatus;
            IsolateStatus = isolateStatus;
        }

        public sbyte EmbeddingLevel { get; }

        public BidiCharacterType OverrideStatus { get; }

        public bool IsolateStatus { get; }
    }

    /// <summary>
    ///     Provides information about a level run - a continuous
    ///     sequence of equal levels.
    /// </summary>
    private readonly struct LevelRun
    {
        public LevelRun(int start, int length, int level, BidiCharacterType sos, BidiCharacterType eos)
        {
            Start = start;
            Length = length;
            Level = level;
            Sos = sos;
            Eos = eos;
        }

        public int Start { get; }

        public int Length { get; }

        public int Level { get; }

        public BidiCharacterType Sos { get; }

        public BidiCharacterType Eos { get; }
    }
}