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

    private int _hp;
    public int HP
    {
        get => _hp;
        set => _hp = value;
    }

    private List<Move> _moves;
    public List<Move> Moves{
        get => _moves;
        set => _moves = value;
    }

    public int MaxHP => Mathf.FloorToInt(2 * _base.HP * _level / 100) + _level + 10;

    public int Attack => Mathf.FloorToInt(2 * _base.Attack * _level / 100) + _level + 5;

    public int Defense => Mathf.FloorToInt(2 * _base.Defense * _level / 100) + _level + 5;

    public int SpAttack => Mathf.FloorToInt(2 * _base.SpAttack * _level / 100) + _level + 5;

    public int SpDefense => Mathf.FloorToInt(2 * _base.SpDefense * _level / 100) + _level + 5;

    public int Speed => Mathf.FloorToInt(2 * _base.Speed * _level / 100) + _level + 5;

    public bool IsKnockedOut => HP <= 0;

    public Pokymon(PokymonBase pBase, int pLevel)
    {
        _base = pBase;
        _level = pLevel;

        InitPokymon();
    }

    public void InitPokymon()
    {
        _hp = MaxHP;
        _moves = new List<Move>();

        foreach (LearnableMove learnableMove in _base.LearnableMoves)
        {
            if (learnableMove.Level <= _level)
            {
                _moves.Add(new Move(learnableMove.Move));
            }

            // TODO: improve starting moves list
            if (_moves.Count >= Constants.MAX_POKYMON_MOVE_COUNT) {
                break;
            }
        }
    }

    public Move GetRandomAvailableMove()
    {
        var movesWithAvailablePP = Moves.Where(m => m.HasAvailablePP).ToList();

        if (movesWithAvailablePP.Count > 0)
        {
            int randId = Random.Range(0, movesWithAvailablePP.Count - 1);

            return movesWithAvailablePP[randId];
        }

        // TODO: implement basic combat move if there are no available moves
        return null;
    }

    private bool IsDamageCritical()
    {
        if (Random.Range(0f, 100f) < 8f)
        {
            return true;
        }

        return false;
    }

    public DamageDescription ReceiveDamage(Pokymon attacker, Move move)
    {
        int attack = move.Base.Category == MoveCategory.Special ? attacker.SpAttack : attacker.Attack;
        int defense = move.Base.Category == MoveCategory.Special ? SpDefense : Defense;
        float baseDamage = ((2 * attacker.Level / 5f + 2) * move.Base.Power * (attack / (float)defense)) / 50f + 2;

        float criticalMultiplier = IsDamageCritical() ? 2f : 1f;
        float randomMultiplier = Random.Range(0.85f, 1f);
        float primaryTypeEffectivenessMultiplier = TypeMatrix.GetTypeEffectivenessMultiplier(move.Base.Type, _base.PrimaryType);
        float secondaryTypeEffectivenessMultiplier = TypeMatrix.GetTypeEffectivenessMultiplier(move.Base.Type, _base.SecondaryType);
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

    public CaptureDescription ReceiveCaptureAttempt(float pokyballBonus = 1f)
    {
        float statusBonus = 1f; // TODO: implement actual statuses and their capture bonus
        float a = (3 * MaxHP - 2 * HP) * _base.CatchRate * pokyballBonus * statusBonus / (3 * MaxHP);

        if (a >= 255)
        {
            return new CaptureDescription()
            {
                ShakeCount = 4,
                IsCaptured = true,
            };
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;

        while (shakeCount < 4)
        {
            if (Random.Range(0, 65635) >= b)
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
            IsCaptured = shakeCount == 4,
        };
    }
}