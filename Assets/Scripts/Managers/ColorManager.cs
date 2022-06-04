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

    private Color[] _typeColors =
    {
        /* BUG */ new Color32(0xb7, 0xc5, 0x58, 0xff),
        /* DAR */ new Color32(0x8c, 0x6f, 0x62, 0xff),
        /* DRA */ new Color32(0x8c, 0x7d, 0xec, 0xff),
        /* ELE */ new Color32(0xff, 0xd4, 0x66, 0xff),
        /* FAI */ new Color32(0xf0, 0xa9, 0xee, 0xff),
        /* FIG */ new Color32(0xc5, 0x70, 0x63, 0xff),
        /* FIR */ new Color32(0xfe, 0x62, 0x4d, 0xff),
        /* FLY */ new Color32(0x9a, 0xa9, 0xfc, 0xff),
        /* GHO */ new Color32(0x7d, 0x7d, 0xc3, 0xff),
        /* GRA */ new Color32(0x8d, 0xd4, 0x78, 0xff),
        /* GRO */ new Color32(0xe2, 0xc5, 0x78, 0xff),
        /* ICE */ new Color32(0x7f, 0xd4, 0xfc, 0xff),
        /* NON */ new Color32(0xff, 0xff, 0xff, 0xff),
        /* NOR */ new Color32(0xb6, 0xb6, 0xa9, 0xff),
        /* POI */ new Color32(0xb6, 0x70, 0xa7, 0xff),
        /* PSY */ new Color32(0xfe, 0x71, 0xa8, 0xff),
        /* ROC */ new Color32(0xc5, 0xb7, 0x82, 0xff),
        /* STE */ new Color32(0xb7, 0xb7, 0xc5, 0xff),
        /* WAT */ new Color32(0x55, 0xa9, 0xfb, 0xff),
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

    public Color PokymonType(PokymonType type)
    {
        return _typeColors[(int)type];
    }
}