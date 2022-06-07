using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class StatusConditionFactory
{
    public static void InitFactory()
    {
        foreach (var statusCondition in StatusConditionList)
        {
            statusCondition.Value.ID = statusCondition.Key;
        }
    }

    public static Dictionary<StatusConditionID, StatusCondition> StatusConditionList { get; set; } = new Dictionary<StatusConditionID, StatusCondition>()
    {
        {
            StatusConditionID.Burn,
            new StatusCondition()
            {
                Name = "Burn",
                Description = "The burn condition inflicts (BRN) damage every turn and halves damage dealt by a Pokémon's physical moves (except Pokémon with the Guts ability).",
                Tag = "BRN",
                Type = StatusConditionType.NonVolatile,
                OnApplyMessage = "%pokymon.name% was burned!",
                OnFinishTurn = (Pokymon pokymon) =>
                {
                    pokymon.ReceiveDamage(Mathf.Max(pokymon.MaxHP / 16, 1));

                    return $"{pokymon.Name} is hurt by its burn!";
                },
            }
        },
        {
            StatusConditionID.Freeze,
            new StatusCondition()
            {
                Name = "Freeze",
                Description = "The freeze condition (FRZ) causes a Pokémon to be unable to make a move.",
                Tag = "FRZ",
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
                Name = "Paralysis",
                Description = "The paralysis condition (PAR) reduces the Pokémon's Speed stat and causes it to have a 25% chance of being unable to use a move (\"fully paralyzed\") when trying to use one.",
                Tag = "PAR",
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
                Name = "Poison",
                Description = "The poison condition (PSN) inflicts damage every turn.",
                Tag = "PSN",
                Type = StatusConditionType.NonVolatile,
                OnApplyMessage = "%pokymon.name% was poisoned!",
                OnFinishTurn = (Pokymon pokymon) =>
                {
                    pokymon.ReceiveDamage(Mathf.Max(pokymon.MaxHP / 8, 1));

                    return $"{pokymon.Name} is hurt by poison!";
                }
            }
        },
        {
            StatusConditionID.Sleep,
            new StatusCondition()
            {
                Name = "Sleep",
                Description = "The sleep condition (SLP) causes a Pokémon to be unable to use moves, except Snore and Sleep Talk. Sleep lasts for a randomly chosen duration of 1 to 5 turns.",
                Tag = "SLP",
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
