using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokymon/New Move")]
public class MoveBase : ScriptableObject
{
    // Basic
    [SerializeField] private int id;
    public int ID => id;

    [SerializeField] private string name;
    public string Name => name;

    [TextArea] [SerializeField] private string description;
    public string Description => description;

    [SerializeField] private PokymonType type = PokymonType.Normal;
    public PokymonType Type => type;

    [SerializeField] private MoveCategory category = MoveCategory.Physical;
    public MoveCategory Category => category;

    [SerializeField] private int pp;
    public int PP => pp;

    [SerializeField] private int power;
    public int Power => power;

    [SerializeField] private int accuracy;
    public int Accuracy => accuracy;
}

public enum MoveCategory {
    Physical,
    Special,
    Status,
}
