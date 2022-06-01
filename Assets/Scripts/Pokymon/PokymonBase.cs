using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokymon", menuName = "Pokymon/New Pokymon")]
public class PokymonBase : ScriptableObject
{
    // Basic
    [SerializeField] private int _id;
    public int ID => _id;

    [SerializeField] private string _name;
    public string Name => _name;

    [TextArea] [SerializeField] private string _description;
    public string Description => _description;

    [SerializeField] private PokymonType _primaryType = PokymonType.Normal;
    public PokymonType PrimaryType => _primaryType;

    [SerializeField] private PokymonType _secondaryType = PokymonType.None;
    public PokymonType SecondaryType => _secondaryType;

    [SerializeField] private int _catchRate;
    public int CatchRate => _catchRate;

    [SerializeField] private int _baseExp;
    public int BaseExp => _baseExp;

    [SerializeField] private PokymonGrowthRate _growthRate = PokymonGrowthRate.MediumSlow;
    public PokymonGrowthRate GrowthRate => _growthRate;

    // Sprites
    [SerializeField] private Sprite _frontSprite;
    public Sprite FrontSprite => _frontSprite;

    [SerializeField] private Sprite _backSprite;
    public Sprite BackSprite => _backSprite;

    // Stats
    [SerializeField] private int _hp;
    public int HP => _hp;

    [SerializeField] private int _attack;
    public int Attack => _attack;

    [SerializeField] private int _defense;
    public int Defense => _defense;

    [SerializeField] private int _spAttack;
    public int SpAttack => _spAttack;

    [SerializeField] private int _spDefense;
    public int SpDefense => _spDefense;

    [SerializeField] private int _speed;
    public int Speed => _speed;

    // Moves
    [SerializeField] private List<LearnableMove> _learnableMoves;
    public List<LearnableMove> LearnableMoves => _learnableMoves;

    public int GetLevelRequiredExp(int level)
    {
        switch (_growthRate)
        {
            case PokymonGrowthRate.Erratic:
                if (level < 50)
                {
                    return (int)(Mathf.Pow(level, 3) * (100 - level) / 50);
                }
                else if (level < 68)
                {
                    return (int)(Mathf.Pow(level, 3) * (150 - level) / 100);
                }
                else if (level < 98)
                {
                    return (int)(Mathf.Pow(level, 3) * (int)((1911 - 10 * level) / 3) / 500);
                }
                else
                {
                    return (int)(Mathf.Pow(level, 3) * (160 - level) / 100);;
                }

            case PokymonGrowthRate.Fast:
                return (int)(4 * Mathf.Pow(level, 3) / 5);

            case PokymonGrowthRate.MediumFast:
                return (int)Mathf.Pow(level, 3);

            case PokymonGrowthRate.MediumSlow:
                return (int)(6 * Mathf.Pow(level, 3) / 5 - 15 * Mathf.Pow(level, 2) + 100 * level - 140);

            case PokymonGrowthRate.Slow:
                return (int)(5 * Mathf.Pow(level, 3) / 4);

            case PokymonGrowthRate.Fluctuating:
                if (level < 15)
                {
                    return (int)(Mathf.Pow(level, 3) * ((int)((level + 1) / 3) + 24) / 50);
                }
                else if (level < 36)
                {
                    return (int)(Mathf.Pow(level, 3) * (level + 14) / 50);
                }
                else
                {
                    return (int)(Mathf.Pow(level, 3) * ((int)(level / 2) + 32) / 50);
                }
        }

        return -1;
    }
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
