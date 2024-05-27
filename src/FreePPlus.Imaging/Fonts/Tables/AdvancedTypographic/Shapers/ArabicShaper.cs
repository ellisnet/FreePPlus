// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using FreePPlus.Imaging.Fonts.Unicode;

namespace FreePPlus.Imaging.Fonts.Tables.AdvancedTypographic.Shapers;

//was previously: namespace SixLabors.Fonts.Tables.AdvancedTypographic.Shapers;

/// <summary>
///     This is a shaper for Arabic, and other cursive scripts.
///     The shaping state machine was ported from fontkit.
///     <see href="https://github.com/foliojs/fontkit/blob/master/src/opentype/shapers/ArabicShaper.js" />
/// </summary>
internal sealed class ArabicShaper : DefaultShaper
{
    private const byte None = 0;

    private const byte Isol = 1;

    private const byte Fina = 2;

    private const byte Fin2 = 3;

    private const byte Fin3 = 4;

    private const byte Medi = 5;

    private const byte Med2 = 6;

    private const byte Init = 7;
    private static readonly Tag MsetTag = Tag.Parse("mset");

    private static readonly Tag FinaTag = Tag.Parse("fina");

    private static readonly Tag Fin2Tag = Tag.Parse("fin2");

    private static readonly Tag Fin3Tag = Tag.Parse("fin3");

    private static readonly Tag IsolTag = Tag.Parse("isol");

    private static readonly Tag InitTag = Tag.Parse("init");

    private static readonly Tag MediTag = Tag.Parse("medi");

    private static readonly Tag Med2Tag = Tag.Parse("med2");

    // Each entry is [prevAction, curAction, nextState]
    private static readonly byte[,][] StateTable =
    {
        // #           NonJoining,                    LeftJoining,                 RightJoining,                 DualJoining,                    ALAPH,                     DALATH RISH
        // State 0: prev was U,  not willing to join.
        {
            new byte[] { None, None, 0 }, new byte[] { None, Isol, 2 }, new byte[] { None, Isol, 1 },
            new byte[] { None, Isol, 2 }, new byte[] { None, Isol, 1 }, new byte[] { None, Isol, 6 }
        },

        // State 1: prev was R or ISOL/ALAPH,  not willing to join.
        {
            new byte[] { None, None, 0 }, new byte[] { None, Isol, 2 }, new byte[] { None, Isol, 1 },
            new byte[] { None, Isol, 2 }, new byte[] { None, Fin2, 5 }, new byte[] { None, Isol, 6 }
        },

        // State 2: prev was D/L in ISOL form,  willing to join.
        {
            new byte[] { None, None, 0 }, new byte[] { None, Isol, 2 }, new byte[] { Init, Fina, 1 },
            new byte[] { Init, Fina, 3 }, new byte[] { Init, Fina, 4 }, new byte[] { Init, Fina, 6 }
        },

        // State 3: prev was D in FINA form,  willing to join.
        {
            new byte[] { None, None, 0 }, new byte[] { None, Isol, 2 }, new byte[] { Medi, Fina, 1 },
            new byte[] { Medi, Fina, 3 }, new byte[] { Medi, Fina, 4 }, new byte[] { Medi, Fina, 6 }
        },

        // State 4: prev was FINA ALAPH,  not willing to join.
        {
            new byte[] { None, None, 0 }, new byte[] { None, Isol, 2 }, new byte[] { Med2, Isol, 1 },
            new byte[] { Med2, Isol, 2 }, new byte[] { Med2, Fin2, 5 }, new byte[] { Med2, Isol, 6 }
        },

        // State 5: prev was FIN2/FIN3 ALAPH,  not willing to join.
        {
            new byte[] { None, None, 0 }, new byte[] { None, Isol, 2 }, new byte[] { Isol, Isol, 1 },
            new byte[] { Isol, Isol, 2 }, new byte[] { Isol, Fin2, 5 }, new byte[] { Isol, Isol, 6 }
        },

        // State 6: prev was DALATH/RISH,  not willing to join.
        {
            new byte[] { None, None, 0 }, new byte[] { None, Isol, 2 }, new byte[] { None, Isol, 1 },
            new byte[] { None, Isol, 2 }, new byte[] { None, Fin3, 5 }, new byte[] { None, Isol, 6 }
        }
    };

    public ArabicShaper(TextOptions textOptions)
        : base(MarkZeroingMode.PostGpos, textOptions) { }

    /// <inheritdoc />
    public override void AssignFeatures(IGlyphShapingCollection collection, int index, int count)
    {
        AddFeature(collection, index, count, CcmpTag);
        AddFeature(collection, index, count, LoclTag);
        AddFeature(collection, index, count, IsolTag, false);
        AddFeature(collection, index, count, FinaTag, false);
        AddFeature(collection, index, count, Fin2Tag, false);
        AddFeature(collection, index, count, Fin3Tag, false);
        AddFeature(collection, index, count, MediTag, false);
        AddFeature(collection, index, count, Med2Tag, false);
        AddFeature(collection, index, count, InitTag, false);
        AddFeature(collection, index, count, MsetTag);

        base.AssignFeatures(collection, index, count);

        var prev = -1;
        var state = 0;
        var actions = new byte[count];

        // Apply the state machine to map glyphs to features.
        for (var i = 0; i < count; i++)
        {
            var data = collection.GetGlyphShapingData(i + index);
            var joiningClass = CodePoint.GetJoiningClass(data.CodePoint);
            var joiningType = joiningClass.JoiningType;
            if (joiningType == JoiningType.Transparent)
            {
                actions[i] = None;
                continue;
            }

            var shapingClassIndex = GetShapingClassIndex(joiningType);
            var actionsWithState = StateTable[state, shapingClassIndex];
            var prevAction = actionsWithState[0];
            var curAction = actionsWithState[1];
            state = actionsWithState[2];

            if (prevAction != None && prev != -1) actions[prev] = prevAction;

            actions[i] = curAction;
            prev = i;
        }

        // Apply the chosen features to their respective glyphs.
        for (var i = 0; i < actions.Length; i++)
            switch (actions[i])
            {
                case Fina:
                    collection.EnableShapingFeature(i + index, FinaTag);
                    break;
                case Fin2:
                    collection.EnableShapingFeature(i + index, Fin2Tag);
                    break;
                case Fin3:
                    collection.EnableShapingFeature(i + index, Fin3Tag);
                    break;
                case Isol:
                    collection.EnableShapingFeature(i + index, IsolTag);
                    break;
                case Init:
                    collection.EnableShapingFeature(i + index, InitTag);
                    break;
                case Medi:
                    collection.EnableShapingFeature(i + index, MediTag);
                    break;
                case Med2:
                    collection.EnableShapingFeature(i + index, Med2Tag);
                    break;
            }
    }

    private static int GetShapingClassIndex(JoiningType joiningType)
    {
        return joiningType switch
        {
            JoiningType.NonJoining => 0,
            JoiningType.LeftJoining => 1,
            JoiningType.RightJoining => 2,
            JoiningType.DualJoining or JoiningType.JoinCausing => 3,

            // TODO: ALAPH: 4
            // TODO: DALATH RISH': 5
            JoiningType.Transparent => 6,
            _ => 0
        };
    }
}