using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusConditionFactory
{
    public static Dictionary<StatusConditionID, StatusCondition> StatusConditionList { get; set; } =
        new Dictionary<StatusConditionID, StatusCondition>()
        {
            {
                StatusConditionID.Poison,
                new StatusCondition()
                {
                    Name = "Poison",
                    Description = "The poison condition inflicts damage every turn.",
                    Tag = "PSN",
                    Color = new Color32(0x9f, 0x41, 0x9d, 0xff),
                    Type = StatusConditionType.NonVolatile,
                    StartMessage = "was poisoned!",
                }
            }
        };
}

public enum StatusConditionID
{
    // Non volatile
    Burn,
    Freeze,
    Paralysis,
    Poison,
    BadlyPoisoned,
    Sleep,
    Frostbite,

    // Volatile
    Bound,
    CantEscape,
    Confusion,
    Curse,
    Drowsy,
    Embargo,
    Encore,
    Flinch,
    HealBlock,
    Identified,
    Infatuation,
    LeechSeed,
    Nightmare,
    PerishSong,
    Taunt,
    Telekinesis,
    Torment,
    TypeChange,

    // Volatile Battle
    AquaRing,
    Bracing,
    ChargingTurn,
    CenterOfAttention,
    DefenseCurl,
    Rooting,
    MagicCoat,
    MagneticLevitation,
    Mimic,
    Minimize,
    Protection,
    Recharging,
    SemiInvulnerableTurn,
    Substitute,
    TakingAim,
    Thrashing,
    Transformed,
    Fixated,
    Primed,
}