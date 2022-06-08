using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCondition
{
    public StatusConditionID ID { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Tag { get; set; }

    public StatusConditionType Type { get; set; }

    public int RemainingTurns { get; set; }

    public Action<Pokymon> OnApply { get; set; }

    public string OnApplyMessage { get; set; }

    public Func<Pokymon, string> OnFinishTurn { get; set; }

    public Func<Pokymon, (bool, string)> OnStartMove { get; set; }

    public bool IsNonVolatile => Type == StatusConditionType.NonVolatile;
}

public enum StatusConditionType
{
    NonVolatile,
    Volatile,
    VolatileBattle,
}

public enum StatusConditionID
{
    AquaRing,
    BadlyPoisoned,
    Bound,
    Bracing,
    Burn,
    CantEscape,
    CenterOfAttention,
    ChargingTurn,
    Confusion,
    Curse,
    DefenseCurl,
    Drowsy,
    Embargo,
    Encore,
    Fixated,
    Flinch,
    Freeze,
    Frostbite,
    HealBlock,
    Identified,
    Infatuation,
    LeechSeed,
    MagicCoat,
    MagneticLevitation,
    Mimic,
    Minimize,
    Nightmare,
    Paralysis,
    PerishSong,
    Poison,
    Primed,
    Protection,
    Recharging,
    Rooting,
    SemiInvulnerableTurn,
    Sleep,
    Substitute,
    TakingAim,
    Taunt,
    Telekinesis,
    Thrashing,
    Torment,
    Transformed,
    TypeChange,
}
