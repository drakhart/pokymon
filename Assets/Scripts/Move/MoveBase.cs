using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokymon/New Move")]
public class MoveBase : ScriptableObject
{
    // Basic
    [SerializeField] private int _id;
    public int ID => _id;

    [SerializeField] private string _name;
    public string Name => _name;

    [TextArea] [SerializeField] private string _description;
    public string Description => _description;

    [SerializeField] private PokymonType _type = PokymonType.Normal;
    public PokymonType Type => _type;

    [SerializeField] private MoveCategory _category = MoveCategory.Physical;
    public MoveCategory Category => _category;

    [SerializeField] private List<StatusMoveEffect> _statusMoveEffectList;
    public List<StatusMoveEffect> StatusMoveEffectList => _statusMoveEffectList;

    [SerializeField] private int _pp;
    public int PP => _pp;

    [SerializeField] private int _power;
    public int Power => _power;

    [SerializeField] private int _accuracy;
    public int Accuracy => _accuracy;
}

public enum MoveCategory {
    Physical,
    Special,
    Status,
}

public enum StatusMoveTarget {
    Ally,
    Foe,
    Self,
}

[Serializable]
public class StatusMoveEffect
{
    [SerializeField] private StatusMoveTarget _target = StatusMoveTarget.Foe;
    public StatusMoveTarget Target => _target;

    [SerializeField] private PokymonStat _stat;
    public PokymonStat Stat => _stat;

    [SerializeField] private int _stageModifier = -1;
    public int StageModifier => _stageModifier;
}
