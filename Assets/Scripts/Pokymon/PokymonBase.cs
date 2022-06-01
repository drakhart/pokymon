using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokymon", menuName = "Pokymon/New Pokymon")]
public class PokymonBase : ScriptableObject
{
    // Basic
    [SerializeField] private int id;
    public int ID => id;

    [SerializeField] private string name;
    public string Name => name;

    [TextArea] [SerializeField] private string description;
    public string Description => description;

    [SerializeField] private PokymonType primaryType = PokymonType.Normal;
    public PokymonType PrimaryType => primaryType;

    [SerializeField] private PokymonType secondaryType = PokymonType.None;
    public PokymonType SecondaryType => secondaryType;

    [SerializeField] private int catchRate;
    public int CatchRate => catchRate;

    [SerializeField] private int baseExp;
    public int BaseExp => baseExp;

    [SerializeField] private PokymonGrowthRate growthRate = PokymonGrowthRate.MediumSlow;
    public PokymonGrowthRate GrowthRate => growthRate;

    // Sprites
    [SerializeField] private Sprite frontSprite;
    public Sprite FrontSprite => frontSprite;

    [SerializeField] private Sprite backSprite;
    public Sprite BackSprite => backSprite;

    // Stats
    [SerializeField] private int hp;
    public int HP => hp;

    [SerializeField] private int attack;
    public int Attack => attack;

    [SerializeField] private int defense;
    public int Defense => defense;

    [SerializeField] private int spAttack;
    public int SpAttack => spAttack;

    [SerializeField] private int spDefense;
    public int SpDefense => spDefense;

    [SerializeField] private int speed;
    public int Speed => speed;

    // Moves
    [SerializeField] private List<LearnableMove> learnableMoves;
    public List<LearnableMove> LearnableMoves => learnableMoves;
}

public enum PokymonType
{
    Bug,
    Dark,
    Dragon,
    Electric,
    Fairy,
    Fighting,
    Fire,
    Flying,
    Ghost,
    Grass,
    Ground,
    Ice,
    None,
    Normal,
    Poison,
    Psychic,
    Rock,
    Steel,
    Water,
}

public enum PokymonGrowthRate
{
    Erratic,
    Fast,
    MediumFast,
    MediumSlow,
    Slow,
    Fluctuating,
}

public class PokymonTypeMatrix
{
    private static float[][] matrix = {
        //                      BUG   DAR   DRA   ELE   FAI   FIG   FIR   FLY   GHO   GRA   GRO   ICE   NON   NOR   POI   PSY   ROC   STE   WAT
        /* BUG */ new float[] { 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 1.0f },
        /* DAR */ new float[] { 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f },
        /* DRA */ new float[] { 1.0f, 1.0f, 2.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f },
        /* ELE */ new float[] { 1.0f, 1.0f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f },
        /* FAI */ new float[] { 1.0f, 2.0f, 2.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f },
        /* FIG */ new float[] { 0.5f, 2.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 0.0f, 1.0f, 1.0f, 2.0f, 1.0f, 2.0f, 0.5f, 0.5f, 2.0f, 2.0f, 1.0f },
        /* FIR */ new float[] { 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 0.5f },
        /* FLY */ new float[] { 2.0f, 1.0f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f },
        /* GHO */ new float[] { 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f },
        /* GRA */ new float[] { 0.5f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 2.0f, 0.5f, 2.0f },
        /* GRO */ new float[] { 0.5f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 2.0f, 0.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 2.0f, 2.0f, 1.0f },
        /* ICE */ new float[] { 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 2.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f },
        /* NON */ new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f },
        /* NOR */ new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f },
        /* POI */ new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 0.5f, 0.0f, 1.0f },
        /* PSY */ new float[] { 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 0.5f, 1.0f },
        /* ROC */ new float[] { 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 2.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f },
        /* STE */ new float[] { 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 0.5f },
        /* WAT */ new float[] { 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f },
    };

    public static float GetTypeEffectivenessMultiplier(PokymonType attackType, PokymonType defenseType)
    {
        int row = (int)attackType;
        int col = (int)defenseType;

        return matrix[row][col];
    }
}

public class PokymonTypeColor
{
    private static Color[] colors =
    {
        /* BUG */ new Color(0.8193042f, 0.9333333f, 0.5254902f),
        /* DAR */ new Color(0.735849f, 0.6178355f, 0.5588287f),
        /* DRA */ new Color(0.6556701f, 0.5568628f, 0.7647059f),
        /* ELE */ new Color(0.9942768f, 1f, 0.5707547f),
        /* FAI */ new Color(0.9339623f, 0.7621484f, 0.9339623f),
        /* FIG */ new Color(0.735849f, 0.5600574f, 0.5310609f),
        /* FIR */ new Color(0.990566f, 0.5957404f, 0.5279903f),
        /* FLY */ new Color(0.7358491f, 0.7708895f, 0.9811321f),
        /* GHO */ new Color(0.6094251f, 0.6094251f, 0.7830189f),
        /* GRA */ new Color(0.4103774f, 1, 0.6846618f),
        /* GRO */ new Color(0.9433962f, 0.7780005f, 0.5562478f),
        /* ICE */ new Color(0.7216981f, 0.9072328f, 1),
        /* NON */ Color.black,
        /* NOR */ new Color(0.8734059f, 0.8773585f, 0.8235582f),
        /* POI */ new Color(0.6981132f, 0.4774831f, 0.6539872f),
        /* PSY */ new Color(1, 0.6650944f, 0.7974522f),
        /* ROC */ new Color(0.8584906f, 0.8171859f, 0.6519669f),
        /* STE */ new Color(0.7889819f, 0.7889819f, 0.8490566f),
        /* WAT */ new Color(0.5613208f, 0.7828107f, 1)
    };

    public static Color GetColorFromType(PokymonType type)
    {
        return colors[(int)type];
    }
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase move;
    public MoveBase Move => move;

    [SerializeField] private int level;
    public int Level => level;
}
