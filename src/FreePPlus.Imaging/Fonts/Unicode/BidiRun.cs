// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;

#pragma warning disable CS8632

namespace FreePPlus.Imaging.Fonts.Unicode;

//was previously: namespace SixLabors.Fonts.Unicode;

internal readonly struct BidiRun : IEquatable<BidiRun>
{
    public BidiRun(BidiCharacterType direction, int level, int start, int length)
    {
        Direction = direction;
        Level = level;
        Start = start;
        Length = length;
    }

    public BidiCharacterType Direction { get; }

    public int Level { get; }

    public int Start { get; }

    public int Length { get; }

    public int End => Start + Length;

    public static bool operator ==(BidiRun left, BidiRun right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BidiRun left, BidiRun right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"{Start} - {End} - {Direction}";
    }

    public static IEnumerable<BidiRun> CoalesceLevels(ReadOnlyArraySlice<sbyte> levels)
    {
        if (levels.Length == 0) yield break;

        var startRun = 0;
        var runLevel = levels[0];
        BidiCharacterType direction;
        for (var i = 1; i < levels.Length; i++)
        {
            if (levels[i] == runLevel) continue;

            // End of this run
            direction = (runLevel & 0x01) == 0 ? BidiCharacterType.LeftToRight : BidiCharacterType.RightToLeft;
            yield return new BidiRun(direction, runLevel, startRun, i - startRun);

            // Move to next run
            startRun = i;
            runLevel = levels[i];
        }

        direction = (runLevel & 0x01) == 0 ? BidiCharacterType.LeftToRight : BidiCharacterType.RightToLeft;
        yield return new BidiRun(direction, runLevel, startRun, levels.Length - startRun);
    }

    public override bool Equals(object? obj)
    {
        return obj is BidiRun run && Equals(run);
    }

    public bool Equals(BidiRun other)
    {
        return Direction == other.Direction
               && Level == other.Level
               && Start == other.Start
               && Length == other.Length
               && End == other.End;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Direction, Level, Start, Length, End);
    }
}