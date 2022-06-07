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

    public string ConditionMessage { get; set; }

    public Action<Pokymon> OnFinishTurn { get; set; }

    public string OnFinishTurnMessage { get; set; }

    public Func<Pokymon, bool> OnStartTurn { get; set; }

    public string OnStartTurnMessage { get; set; }

    public bool IsNonVolatile => Type == StatusConditionType.NonVolatile;
}

public enum StatusConditionType
{
    NonVolatile,
    Volatile,
    VolatileBattle,
}
