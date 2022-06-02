using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberHUD : MonoBehaviour
{
    [SerializeField] private Bar _expBar;
    [SerializeField] private Bar _hpBar;
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _pokymonLevelText;
    [SerializeField] private Text _pokymonNameText;
    [SerializeField] private Text _pokymonTypeText;
    [SerializeField] private Image _pokymonImage;

    [SerializeField] private Color _defaultColor = Color.black;
    [SerializeField] private Color _selectedColor = Color.blue;

    private Pokymon _pokymon;

    private string GetPokymonType()
    {
        if (_pokymon.Base.SecondaryType == PokymonType.None)
        {
            return _pokymon.Base.PrimaryType.ToString();
        }

        return $"{_pokymon.Base.PrimaryType} & {_pokymon.Base.SecondaryType}";
    }

    public void SetPokymonData(Pokymon pokymon)
    {
        _pokymon = pokymon;

        _pokymonNameText.text = _pokymon.Name;
        _pokymonLevelText.text = $"Lvl {_pokymon.Level}";
        _pokymonTypeText.text = GetPokymonType();
        _expBar.SetScale(_pokymon.NormalizedExp);
        _hpBar.SetScale(_pokymon.NormalizedHP);
        _hpText.text = $"{_pokymon.HP}/{_pokymon.MaxHP}";
        _pokymonImage.sprite = _pokymon.Base.FrontSprite;
    }

    public void SetSelectedPokymon(bool selected)
    {
        _pokymonNameText.color = selected ? _selectedColor : _defaultColor;
    }
}
