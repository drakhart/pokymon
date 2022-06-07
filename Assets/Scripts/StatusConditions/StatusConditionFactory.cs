using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StatusConditionFactory
{
    public static Dictionary<StatusConditionID, StatusCondition> StatusConditionList { get; set; } =
        new Dictionary<StatusConditionID, StatusCondition>()
        {
            {
                StatusConditionID.Burn,
                new StatusCondition()
                {
                    ID = StatusConditionID.Burn,
                    Name = "Burn",
                    Description = "The burn condition inflicts (BRN) damage every turn and halves damage dealt by a Pokémon's physical moves (except Pokémon with the Guts ability).",
                    Tag = "BRN",
                    Color = new Color32(0xef, 0x81, 0x3c, 0xff),
                    Type = StatusConditionType.NonVolatile,
                    ConditionMessage = "%pokymon.name% was burned!",
                    OnFinishTurn = (Pokymon pokymon) => BurnEffect(pokymon),
                    OnFinishTurnMessage = "%pokymon.name% is hurt by its burn!",
                }
            },
            {
                StatusConditionID.Freeze,
                new StatusCondition()
                {
                    ID = StatusConditionID.Freeze,
                    Name = "Freeze",
                    Description = "The freeze condition (FRZ) causes a Pokémon to be unable to make a move.",
                    Tag = "FRZ",
                    Color = new Color32(0x99, 0xd8, 0xd8, 0xff),
                    Type = StatusConditionType.NonVolatile,
                    ConditionMessage = "%pokymon.name% was frozen solid!",
                    OnStartTurn = (Pokymon pokymon) => FreezeEffect(pokymon),
                    OnStartTurnMessage = "%pokymon.name% is frozen! It can't move!",
                }
            },
            {
                StatusConditionID.Paralysis,
                new StatusCondition()
                {
                    ID = StatusConditionID.Paralysis,
                    Name = "Paralysis",
                    Description = "The paralysis condition (PAR) reduces the Pokémon's Speed stat and causes it to have a 25% chance of being unable to use a move (\"fully paralyzed\") when trying to use one.",
                    Tag = "PAR",
                    Color = new Color32(0xf8, 0xd0, 0x49, 0xff),
                    Type = StatusConditionType.NonVolatile,
                    ConditionMessage = "%pokymon.name% was paralyzed!",
                    OnStartTurn = (Pokymon pokymon) => ParalysisEffect(pokymon),
                    OnStartTurnMessage = "%pokymon.name% is paralyzed! It can't move!",
                }
            },
            {
                StatusConditionID.Poison,
                new StatusCondition()
                {
                    ID = StatusConditionID.Poison,
                    Name = "Poison",
                    Description = "The poison condition (PSN) inflicts damage every turn.",
                    Tag = "PSN",
                    Color = new Color32(0x9f, 0x41, 0x9d, 0xff),
                    Type = StatusConditionType.NonVolatile,
                    ConditionMessage = "%pokymon.name% was poisoned!",
                    OnFinishTurn = (Pokymon pokymon) => PoisonEffect(pokymon),
                    OnFinishTurnMessage = "%pokymon.name% is hurt by poison!",
                }
            },
        };

    private static void BurnEffect(Pokymon pokymon)
    {
        pokymon.ReceiveDamage(Mathf.Max(pokymon.MaxHP / 16, 1));
    }

    private static bool FreezeEffect(Pokymon pokymon)
    {
        if (Random.Range(0, 100) < 25)
        {
            pokymon.RemoveStatusCondition(StatusConditionID.Freeze);

            return false;
        }

        return true;
    }

    private static bool ParalysisEffect(Pokymon pokymon)
    {
        return Random.Range(0, 100) < 25;
    }

    private static void PoisonEffect(Pokymon pokymon)
    {
        pokymon.ReceiveDamage(Mathf.Max(pokymon.MaxHP / 8, 1));
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