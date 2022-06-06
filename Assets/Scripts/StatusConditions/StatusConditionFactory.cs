using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusConditionFactory
{
    public static Dictionary<StatusConditionID, StatusCondition> StatusConditionList { get; set; } =
        new Dictionary<StatusConditionID, StatusCondition>()
        {
            {
                StatusConditionID.Burn,
                new StatusCondition()
                {
                    Name = "Burn",
                    Description = "The burn condition inflicts damage every turn and halves damage dealt by a Pokémon's physical moves (except Pokémon with the Guts ability).",
                    Tag = "BRN",
                    Color = new Color32(0xef, 0x81, 0x3c, 0xff),
                    Type = StatusConditionType.NonVolatile,
                    StartMessage = "was burned!",
                    OnFinishTurn = (Pokymon pokymon) => BurnEffect(pokymon),
                }
            },
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
                    OnFinishTurn = (Pokymon pokymon) => PoisonEffect(pokymon),
                }
            },
        };

    private static void BurnEffect(Pokymon pokymon)
    {
        pokymon.ReceiveDamage(pokymon.MaxHP / 8);
    }

    private static void PoisonEffect(Pokymon pokymon)
    {
        pokymon.ReceiveDamage(pokymon.MaxHP / 8);
    }
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