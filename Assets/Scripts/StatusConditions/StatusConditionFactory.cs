using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class StatusConditionFactory
{
    public static Dictionary<StatusConditionID, StatusCondition> StatusConditionList { get; set; } = new Dictionary<StatusConditionID, StatusCondition>()
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
                OnApplyMessage = "%pokymon.name% was burned!",
                OnFinishTurn = (Pokymon pokymon) =>
                {
                    pokymon.ReceiveDamage(Mathf.Max(pokymon.MaxHP / 16, 1));

                    return (true, $"{pokymon.Name} is hurt by its burn!");
                },
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
                OnApplyMessage = "%pokymon.name% was frozen solid!",
                OnStartTurn = (Pokymon pokymon) =>
                {
                    if (Random.Range(0, 100) < 25)
                    {
                        pokymon.RemoveStatusCondition(StatusConditionID.Freeze);

                        return (false, $"{pokymon.Name} is no longer frozen!");
                    }

                    return (true, $"{pokymon.Name} is frozen! It can't move!");
                },
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
                OnApplyMessage = "%pokymon.name% was paralyzed!",
                OnStartTurn = (Pokymon pokymon) =>
                {
                    if (Random.Range(0, 100) >= 25)
                    {
                        return (false, null);
                    }

                    return (true, $"{pokymon.Name} is paralyzed! It can't move!");
                },
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
                OnApplyMessage = "%pokymon.name% was poisoned!",
                OnFinishTurn = (Pokymon pokymon) =>
                {
                    pokymon.ReceiveDamage(Mathf.Max(pokymon.MaxHP / 8, 1));

                    return (true, $"{pokymon.Name} is hurt by poison!");
                }
            }
        },
        {
            StatusConditionID.Sleep,
            new StatusCondition()
            {
                ID = StatusConditionID.Sleep,
                Name = "Sleep",
                Description = "The sleep condition (SLP) causes a Pokémon to be unable to use moves, except Snore and Sleep Talk. Sleep lasts for a randomly chosen duration of 1 to 5 turns.",
                Tag = "SLP",
                Color = new Color32(0xa8, 0x90, 0xec, 0xff),
                Type = StatusConditionType.NonVolatile,
                OnApply = (Pokymon pokymon) => pokymon.GetStatusCondition(StatusConditionID.Sleep).RemainingTurns = Random.Range(1, 6),
                OnApplyMessage = "%pokymon.name% was fast asleep!",
                OnStartTurn = (Pokymon pokymon) =>
                {
                    if (pokymon.GetStatusCondition(StatusConditionID.Sleep).RemainingTurns-- <= 0)
                    {
                        pokymon.RemoveStatusCondition(StatusConditionID.Sleep);

                        return (false, $"Beep Beep! {pokymon.Name} woke up!");
                    }

                    return (true, $"{pokymon.Name} is sleeping! It can't move!");
                },
            }
        },
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