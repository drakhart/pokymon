using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{

    [SerializeField] private Color _defaultText;
    public Color DefaultText => _defaultText;

    [SerializeField] private Color _selectedText;
    public Color SelectedText => _selectedText;

    [SerializeField] private Color _highBar;
    public Color HighBar => _highBar;

    [SerializeField] private Color _mediumBar;
    public Color MediumBar => _mediumBar;

    [SerializeField] private Color _lowBar;
    public Color LowBar => _lowBar;

    [SerializeField] private Color _highPP;
    public Color HighPP => _highPP;

    [SerializeField] private Color _mediumPP;
    public Color MediumPP => _mediumPP;

    [SerializeField] private Color _lowPP;
    public Color LowPP => _lowPP;

    private static Dictionary<StatusConditionID, Color32> _statusConditionColorList = new Dictionary<StatusConditionID, Color32>()
    {
        { StatusConditionID.Burn,          new Color32(0xef, 0x81, 0x3c, 0xff) },
        { StatusConditionID.Freeze,        new Color32(0x99, 0xd8, 0xd8, 0xff) },
        { StatusConditionID.Paralysis,     new Color32(0xf8, 0xd0, 0x49, 0xff) },
        { StatusConditionID.Poison,        new Color32(0x9f, 0x41, 0x9d, 0xff) },
        { StatusConditionID.BadlyPoisoned, new Color32(0x9f, 0x41, 0x9d, 0xff) },
        { StatusConditionID.Sleep,         new Color32(0xa8, 0x90, 0xec, 0xff) },
    };

    static Dictionary<PokymonType, Color32> _typeColorList = new Dictionary<PokymonType, Color32>()
    {
        { PokymonType.Bug,      new Color32(0xb7, 0xc5, 0x58, 0xff) },
        { PokymonType.Dark,     new Color32(0x8c, 0x6f, 0x62, 0xff) },
        { PokymonType.Dragon,   new Color32(0x8c, 0x7d, 0xec, 0xff) },
        { PokymonType.Electric, new Color32(0xff, 0xd4, 0x66, 0xff) },
        { PokymonType.Fairy,    new Color32(0xf0, 0xa9, 0xee, 0xff) },
        { PokymonType.Fighting, new Color32(0xc5, 0x70, 0x63, 0xff) },
        { PokymonType.Fire,     new Color32(0xfe, 0x62, 0x4d, 0xff) },
        { PokymonType.Flying,   new Color32(0x9a, 0xa9, 0xfc, 0xff) },
        { PokymonType.Ghost,    new Color32(0x7d, 0x7d, 0xc3, 0xff) },
        { PokymonType.Grass,    new Color32(0x8d, 0xd4, 0x78, 0xff) },
        { PokymonType.Ground,   new Color32(0xe2, 0xc5, 0x78, 0xff) },
        { PokymonType.Ice,      new Color32(0x7f, 0xd4, 0xfc, 0xff) },
        { PokymonType.None,     new Color32(0xff, 0xff, 0xff, 0xff) },
        { PokymonType.Normal,   new Color32(0xb6, 0xb6, 0xa9, 0xff) },
        { PokymonType.Poison,   new Color32(0xb6, 0x70, 0xa7, 0xff) },
        { PokymonType.Psychic,  new Color32(0xfe, 0x71, 0xa8, 0xff) },
        { PokymonType.Rock,     new Color32(0xc5, 0xb7, 0x82, 0xff) },
        { PokymonType.Steel,    new Color32(0xb7, 0xb7, 0xc5, 0xff) },
        { PokymonType.Water,    new Color32(0x55, 0xa9, 0xfb, 0xff) },
    };

    public static ColorManager SharedInstance;

    private void Awake() {
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            print("There's more than one ColorManager instance!");

            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public Color32 ByStatusCondition(StatusConditionID statusConditionID)
    {
        return _statusConditionColorList.ContainsKey(statusConditionID)
            ? _statusConditionColorList[statusConditionID]
            : new Color32(0x32, 0x32, 0x32, 0xff);
    }

    public Color32 ByPokymonType(PokymonType type)
    {
        return _typeColorList.ContainsKey(type)
            ? _typeColorList[type]
            : new Color32(0x32, 0x32, 0x32, 0xff);
    }
}
