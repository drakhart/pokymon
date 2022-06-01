using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokymon/New Move")]
public class MoveBase : ScriptableObject
{
    // Basic
    [SerializeField] private int id;
    public int ID => id;

    [SerializeField] private string _name;
    public string Name => _name;

    [TextArea] [SerializeField] private string _description;
    public string Description => _description;

    [SerializeField] private PokymonType _type = PokymonType.Normal;
    public PokymonType Type => _type;

    [SerializeField] private MoveCategory _category = MoveCategory.Physical;
    public MoveCategory Category => _category;

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
