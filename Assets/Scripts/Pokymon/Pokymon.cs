using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public struct CaptureDescription
{
    public float ShakeCount { get; set; }
    public bool IsCaptured { get; set; }
}

public struct DamageDescription
{
    public float Critical { get; set; }
    public int Damage { get; set; }
    public bool IsKnockedOut { get; set; }
    public float Type { get; set; }
}

[Serializable]
public class Pokymon
{
    [SerializeField] private PokymonBase _base;
    public PokymonBase Base => _base;

    [SerializeField] private int _level;
    public int Level => _level;

    private int _exp;
    public int Exp
    {
        get => _exp;
        set => _exp = value;
    }

    private int _hp;
    public int HP
    {
        get => _hp;
        set => _hp = value;
    }

    private List<Move> _moveList;
    public List<Move> MoveList{
        get => _moveList;
        set => _moveList = value;
    }

    private bool _isWild;
    public bool IsWild
    {
        get => _isWild;
        set => _isWild = value;
    }

    public string Name => _isWild ? $"Wild {_base.Name}" : _base.Name;

    public int CurrentLevelExp => CalculateLevelExperience(_level);

    public int NextLevelExp => CalculateLevelExperience(_level + 1);

    public int MaxHP => Mathf.FloorToInt(2 * _base.HP * _level / 100) + _level + 10;

    public int Attack => Mathf.FloorToInt(2 * _base.Attack * _level / 100) + _level + 5;

    public int Defense => Mathf.FloorToInt(2 * _base.Defense * _level / 100) + _level + 5;

    public int SpAttack => Mathf.FloorToInt(2 * _base.SpAttack * _level / 100) + _level + 5;

    public int SpDefense => Mathf.FloorToInt(2 * _base.SpDefense * _level / 100) + _level + 5;

    public int Speed => Mathf.FloorToInt(2 * _base.Speed * _level / 100) + _level + 5;

    public bool IsKnockedOut => HP <= 0;

    public int KnockOutExp => (int)(_base.BaseExp * _level * (_isWild ? 1f : 1.5f) / 7);

    public float NormalizedExp => Mathf.Clamp01((_exp - CurrentLevelExp) / (float)(NextLevelExp - CurrentLevelExp));

    public float NormalizedHP => _hp / (float)MaxHP;

    public Pokymon(PokymonBase pBase, int pLevel, bool isWild)
    {
        _base = pBase;
        _level = pLevel;
        _isWild = isWild;

        InitPokymon();
    }

    public void InitPokymon()
    {
        _hp = MaxHP;
        _exp = CurrentLevelExp;
        _moveList = new List<Move>();

        foreach (LearnableMove learnableMove in _base.LearnableMoves)
        {
            if (learnableMove.Level <= _level)
            {
                _moveList.Add(new Move(learnableMove.Move));
            }

            // TODO: improve starting moves list
            if (_moveList.Count >= Constants.MAX_POKYMON_MOVE_COUNT) {
                break;
            }
        }
    }

    public CaptureDescription CalculateCapture(float pokyballBonus = 1f)
    {
        float statusBonus = 1f; // TODO: implement actual statuses and their capture bonus
        float a = (3 * MaxHP - 2 * HP) * _base.CatchRate * pokyballBonus * statusBonus / (3 * MaxHP);

        if (a >= 255)
        {
            return new CaptureDescription()
            {
                ShakeCount = Constants.MIN_CAPTURE_SHAKE_COUNT,
                IsCaptured = true,
            };
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;

        while (shakeCount < Constants.MIN_CAPTURE_SHAKE_COUNT)
        {
            if (Random.Range(0, 65636) >= b)
            {
                break;
            }
            else
            {
                shakeCount++;
            }
        }

        return new CaptureDescription()
        {
            ShakeCount = shakeCount,
            IsCaptured = shakeCount == Constants.MIN_CAPTURE_SHAKE_COUNT,
        };
    }

    public DamageDescription CalculateDamage(Pokymon attacker, Move move)
    {
        int attack = move.Base.Category == MoveCategory.Special ? attacker.SpAttack : attacker.Attack;
        int defense = move.Base.Category == MoveCategory.Special ? SpDefense : Defense;
        float baseDamage = ((2 * attacker.Level / 5f + 2) * move.Base.Power * (attack / (float)defense)) / 50f + 2;

        float criticalMultiplier = IsDamageCritical() ? 2f : 1f;
        float randomMultiplier = Random.Range(0.85f, 1f);
        float primaryTypeEffectivenessMultiplier = PokymonTypeMatrix.GetTypeEffectivenessMultiplier(move.Base.Type, _base.PrimaryType);
        float secondaryTypeEffectivenessMultiplier = PokymonTypeMatrix.GetTypeEffectivenessMultiplier(move.Base.Type, _base.SecondaryType);
        float typeEffectivenessMultiplier = primaryTypeEffectivenessMultiplier * secondaryTypeEffectivenessMultiplier;

        int totalDamage = Mathf.FloorToInt(baseDamage * criticalMultiplier * randomMultiplier * typeEffectivenessMultiplier);
        totalDamage = Mathf.Min(totalDamage, HP);

        HP -= totalDamage;

        return new DamageDescription()
        {
            Critical = criticalMultiplier,
            Damage = totalDamage,
            IsKnockedOut = IsKnockedOut,
            Type = typeEffectivenessMultiplier,
        };
    }

    public int CalculateLevelExperience(int level)
    {
        switch (_base.GrowthRate)
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

    public Move GetRandomAvailableMove()
    {
        var movesWithAvailablePP = MoveList.Where(m => m.HasAvailablePP).ToList();

        if (movesWithAvailablePP.Count > 0)
        {
            int randId = Random.Range(0, movesWithAvailablePP.Count);

            return movesWithAvailablePP[randId];
        }

        // TODO: implement basic combat move if there are no available moves
        return null;
    }

    private bool IsDamageCritical()
    {
        if (Random.Range(0f, 100f) < Constants.CRITICAL_HIT_ODDS)
        {
            return true;
        }

        return false;
    }
}