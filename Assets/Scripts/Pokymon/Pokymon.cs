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
    public int HP { get; set; }
    public bool IsEvaded { get; set; }
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
    public int Exp => _exp;

    private int _hp;
    public int HP => _hp;

    private List<Move> _moveList;
    public List<Move> MoveList => _moveList;

    private bool _isWild;
    public bool IsWild
    {
        get => _isWild;
        set => _isWild = value;
    }

    private Dictionary<PokymonStat, int> _statList;
    public Dictionary<PokymonStat, int> StatList => _statList;

    private Dictionary<PokymonStat, int> _statStageList;
    public Dictionary<PokymonStat, int> StatStageList => _statStageList;

    private List<StatusCondition> _statusConditionList;
    public List<StatusCondition> StatusConditionList => _statusConditionList;

    public Action OnChangeHP;
    public Action OnChangeStatusConditionList;
    public Action<bool> OnChangeExp;
    public Action OnChangeLevel;

    public string Name => _isWild ? $"Wild {_base.Name}" : _base.Name;

    public int CurrentLevelExp => CalculateLevelExperience(_level);

    public int NextLevelExp => CalculateLevelExperience(_level + 1);

    public int MaxHP => (int)(2 * _base.HP * _level / 100) + _level + 10;

    public int Attack => GetStat(PokymonStat.Attack);

    public int Defense => GetStat(PokymonStat.Defense);

    public int SpAttack => GetStat(PokymonStat.SpAttack);

    public int SpDefense => GetStat(PokymonStat.SpDefense);

    public int Speed => GetStat(PokymonStat.Speed);

    public bool IsKnockedOut => HP <= 0;

    public int KnockOutExp => (int)(_base.BaseExp * _level * (_isWild ? 1f : 1.5f) / 7);

    public int MoveCount => _moveList.Count;

    public bool HasFreeMoveSlot => MoveCount < Constants.MAX_POKYMON_MOVE_COUNT;

    public float NormalizedExp => Mathf.Clamp01((_exp - CurrentLevelExp) / (float)(NextLevelExp - CurrentLevelExp));

    public float NormalizedHP => _hp / (float)MaxHP;

    public LearnableMove LearnableMove => _base.LearnableMoves.Where(lm => lm.Level == _level).FirstOrDefault();

    public bool HasNonVolatileStatusCondition => _statusConditionList.Count(sc => sc.IsNonVolatile) > 0;

    public StatusCondition NonVolatileStatusCondition => _statusConditionList.Where(sc => sc.IsNonVolatile).FirstOrDefault();

    public bool HasFinishTurnStatusConditions => _statusConditionList.Count(sc => sc.OnFinishTurn != null) > 0;

    public List<StatusCondition> FinishTurnStatusConditionList => _statusConditionList.Where(sc => sc.OnFinishTurn != null).ToList();

    public bool HasStartMoveStatusConditions => _statusConditionList.Count(sc => sc.OnStartMove != null) > 0;

    public List<StatusCondition> StartMoveStatusConditionList => _statusConditionList.Where(sc => sc.OnStartMove != null).ToList();

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
        _statusConditionList = new List<StatusCondition>();

        InitStatStages();
        InitStats();

        foreach (LearnableMove learnableMove in _base.LearnableMoves)
        {
            if (learnableMove.Level <= _level)
            {
                _moveList.Add(new Move(learnableMove.Base));
            }

            // TODO: improve starting moves list
            if (!HasFreeMoveSlot) {
                break;
            }
        }
    }

    private void InitStats()
    {
        _statList = new Dictionary<PokymonStat, int>();
        _statList.Add(PokymonStat.Accuracy, 100);
        _statList.Add(PokymonStat.Attack, (int)(2 * _base.Attack * _level / 100) + _level + 5);
        _statList.Add(PokymonStat.Defense, (int)(2 * _base.Defense * _level / 100) + _level + 5);
        _statList.Add(PokymonStat.Evasion, 100);
        _statList.Add(PokymonStat.SpAttack, (int)(2 * _base.SpAttack * _level / 100) + _level + 5);
        _statList.Add(PokymonStat.SpDefense, (int)(2 * _base.SpDefense * _level / 100) + _level + 5);
        _statList.Add(PokymonStat.Speed, (int)(2 * _base.Speed * _level / 100) + _level + 5);
    }

    private void InitStatStages()
    {
        _statStageList = new Dictionary<PokymonStat, int>()
        {
            { PokymonStat.Accuracy, 0 },
            { PokymonStat.Attack, 0 },
            { PokymonStat.Defense, 0 },
            { PokymonStat.Evasion, 0 },
            { PokymonStat.SpAttack, 0 },
            { PokymonStat.SpDefense, 0 },
            { PokymonStat.Speed, 0 },
        };
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

    public bool CanEvadeMove(Move move, Pokymon attacker)
    {
        // Always hit move
        if (move.IsStatusMove && move.Base.Accuracy == 0)
        {
            return false;
        }

        var accuracy = attacker.GetStat(PokymonStat.Accuracy);
        var evasion = GetStat(PokymonStat.Evasion);
        var finalAccuracy = move.Base.Accuracy * accuracy / evasion;

        return Random.Range(0, 100) >= finalAccuracy;
    }

    public DamageDescription ReceivePhysicalMove(Move move, Pokymon attacker)
    {
        if (CanEvadeMove(move, attacker))
        {
            return new DamageDescription()
            {
                IsEvaded = true,
            };
        }

        int attack = move.IsSpecialMove ? attacker.SpAttack : attacker.Attack;
        int defense = move.IsSpecialMove ? SpDefense : Defense;
        float baseDamage = ((2 * attacker.Level / 5f + 2) * move.Base.Power * (attack / (float)defense)) / 50f + 2;

        float criticalMultiplier = IsDamageCritical() ? 2f : 1f;
        float randomMultiplier = Random.Range(0.85f, 1f);
        float primaryTypeEffectivenessMultiplier = PokymonTypeMatrix.GetTypeEffectivenessMultiplier(move.Base.Type, _base.PrimaryType);
        float secondaryTypeEffectivenessMultiplier = PokymonTypeMatrix.GetTypeEffectivenessMultiplier(move.Base.Type, _base.SecondaryType);
        float typeEffectivenessMultiplier = primaryTypeEffectivenessMultiplier * secondaryTypeEffectivenessMultiplier;

        int totalDamage = (int)(baseDamage * criticalMultiplier * randomMultiplier * typeEffectivenessMultiplier);
        totalDamage = Mathf.Min(totalDamage, HP);

        ReceiveDamage(totalDamage);

        return new DamageDescription()
        {
            Critical = criticalMultiplier,
            HP = totalDamage,
            IsEvaded = false,
            IsKnockedOut = IsKnockedOut,
            Type = typeEffectivenessMultiplier,
        };
    }

    public void ReceiveDamage(int damage)
    {
        _hp = Mathf.Max(_hp - damage, 0);

        OnChangeHP?.Invoke();
    }

    private bool IsDamageCritical()
    {
        if (Random.Range(0, 100) < Constants.CRITICAL_HIT_ODDS)
        {
            return true;
        }

        return false;
    }

    private int CalculateLevelExperience(int level)
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

    public void EarnExp(int earnedExp)
    {
        _exp += earnedExp;

        OnChangeExp?.Invoke(false);
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

    private int GetStat(PokymonStat stat)
    {
        int statBase = _statList[stat];
        int statStage = _statStageList[stat];

        float divider = stat == PokymonStat.Accuracy || stat == PokymonStat.Evasion ? 3f : 2f;
        float multiplier = 1 + Math.Abs(statStage) / divider;

        return statStage >= 0 ? (int)(statBase * multiplier) : (int)(statBase / multiplier);
    }

    public int GetStatStage(PokymonStat stat)
    {
        return _statStageList[stat];
    }

    public bool ModifyStatStage(PokymonStat stat, int stageModifier)
    {
        var prevStage = _statStageList[stat];

        _statStageList[stat] = Mathf.Clamp(_statStageList[stat] + stageModifier,
            Constants.MIN_STAT_STAGE,
            Constants.MAX_STAT_STAGE
        );

        return _statStageList[stat] != prevStage ? true : false;
    }

    public StatusCondition ApplyStatusCondition(StatusConditionID statusConditionID)
    {
        var statusCondition = StatusConditionFactory.StatusConditionList[statusConditionID];

        // Status conditions cannot be repeated
        if (GetStatusCondition(statusConditionID) != null)
        {
            return null;
        }

        // There can only be one non volatile status condition
        if (statusCondition.IsNonVolatile && HasNonVolatileStatusCondition)
        {
            return null;
        }

        _statusConditionList.Add(statusCondition);

        statusCondition.OnApply?.Invoke(this);

        OnChangeStatusConditionList?.Invoke();

        return statusCondition;
    }

    public StatusCondition GetStatusCondition(StatusConditionID statusConditionID)
    {
        return _statusConditionList.Where(sc => sc.ID == statusConditionID).FirstOrDefault();
    }

    public void RemoveStatusCondition(StatusConditionID statusConditionID)
    {
        _statusConditionList.RemoveAll(sc => sc.ID == statusConditionID);

        OnChangeStatusConditionList?.Invoke();
    }

    public void RemoveVolatileStatusConditions()
    {
        _statusConditionList.RemoveAll(sc => sc.Type == StatusConditionType.Volatile
            || sc.Type == StatusConditionType.VolatileBattle);

        OnChangeStatusConditionList?.Invoke();
    }

    public bool LevelUp()
    {
        if (_exp > NextLevelExp)
        {
            int prevMaxHp = MaxHP;

            _level++;
            _hp += MaxHP - prevMaxHp;

            OnChangeExp?.Invoke(true);
            OnChangeHP?.Invoke();
            OnChangeLevel?.Invoke();

            InitStats();

            return true;
        }

        return false;
    }

    public bool LearnMove(LearnableMove learnableMove)
    {
        if (!HasFreeMoveSlot)
        {
            return false;
        }

        _moveList.Add(new Move(learnableMove.Base));

        return true;
    }

    public void OnBattleFinish()
    {
        RemoveVolatileStatusConditions();
        InitStatStages();
    }
}
