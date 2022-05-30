using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberHUD : MonoBehaviour
{
    public Text pokymonName;
    public Text pokymonLevel;
    public Text pokymonType;
    public Text healthText;
    public Bar healthBar;
    public Image pokymonImage;

    [SerializeField] private Color selectedColor = Color.blue;
    [SerializeField] private Color defaultColor = Color.black;

    private Pokymon _pokymon;

    public void SetPokymonData(Pokymon pokymon)
    {
        _pokymon = pokymon;

        pokymonName.text = _pokymon.Base.Name;
        pokymonLevel.text = $"Lvl {_pokymon.Level}";
        pokymonType.text = GetPokymonType();
        healthText.text = $"{_pokymon.HP}/{_pokymon.MaxHP}";
        healthBar.SetLength(_pokymon.HP / (float)_pokymon.MaxHP);
        pokymonImage.sprite = _pokymon.Base.FrontSprite;
    }

    private string GetPokymonType()
    {
        if (_pokymon.Base.SecondaryType == PokymonType.None)
        {
            return _pokymon.Base.PrimaryType.ToString();
        }

        return $"{_pokymon.Base.PrimaryType} & {_pokymon.Base.SecondaryType}";
    }

    public void SetSelectedPokymon(bool selected)
    {
        pokymonName.color = selected ? selectedColor : defaultColor;
    }
}
