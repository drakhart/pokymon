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

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase _base;
    public MoveBase Base => _base;

    [SerializeField] private int _level;
    public int Level => _level;
}
