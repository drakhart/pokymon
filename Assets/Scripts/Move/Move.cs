using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    private MoveBase _base;
    public MoveBase Base
    {
        get => _base;
        set => _base = value;
    }

    private int _pp;
    public int PP
    {
        get => _pp;
        set => _pp = Mathf.Clamp(value, 0, MaxPP);
    }

    public Move(MoveBase mBase)
    {
        _base = mBase;

        _pp = _base.PP;
    }

    public int MaxPP => _base.PP;

    public bool HasAvailablePP => _pp > 0;

    public float NormalizedPP => _pp / (float)MaxPP;

    public bool IsPhysicalMove => _base.Category == MoveCategory.Physical;

    public bool IsSpecialMove => _base.Category == MoveCategory.Special;

    public bool IsStatusMove => _base.Category == MoveCategory.Status;

    public bool HasStatModifierEffects => _base.StatModifierEffectList.Count > 0;

    public bool HasStatusConditionEffects => _base.StatusConditionEffectList.Count > 0;
}
