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

public enum PokymonStat
{
    Accuracy,
    Attack,
    Defense,
    Evasion,
    SpAttack,
    SpDefense,
    Speed,
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase _base;
    public MoveBase Base => _base;

    [SerializeField] private int _level;
    public int Level => _level;
}
