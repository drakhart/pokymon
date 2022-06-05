using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCondition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Tag { get; set; }
    public Color Color { get; set; }
    public StatusConditionType Type { get; set; }
    public string StartMessage { get; set; }
}

public enum StatusConditionType
{
    NonVolatile,
    Volatile,
    VolatileBattle,
}
