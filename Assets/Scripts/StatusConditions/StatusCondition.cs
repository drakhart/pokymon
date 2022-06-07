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

    public Color Color { get; set; }

    public StatusConditionType Type { get; set; }

    public int RemainingTurns { get; set; }

    public Action<Pokymon> OnApply { get; set; }

    public string OnApplyMessage { get; set; }

    public Func<Pokymon, (bool, string)> OnFinishTurn { get; set; }

    public Func<Pokymon, (bool, string)> OnStartTurn { get; set; }

    public bool IsNonVolatile => Type == StatusConditionType.NonVolatile;
}

public enum StatusConditionType
{
    NonVolatile,
    Volatile,
    VolatileBattle,
}
