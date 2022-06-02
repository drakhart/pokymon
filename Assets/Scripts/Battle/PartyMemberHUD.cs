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

    public void SetPokymonData(Pokymon pokymon)
    {
        _pokymon = pokymon;

        _pokymonNameText.text = _pokymon.Name;
        _pokymonLevelText.text = $"Lvl {_pokymon.Level}";
        _pokymonTypeText.text = GetPokymonType();
        _expBar.SetScale(GetNormalizedExp());
        _hpBar.SetScale(_pokymon.HP / (float)_pokymon.MaxHP);
        _hpText.text = $"{_pokymon.HP}/{_pokymon.MaxHP}";
        _pokymonImage.sprite = _pokymon.Base.FrontSprite;
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
        _pokymonNameText.color = selected ? _selectedColor : _defaultColor;
    }

    private float GetNormalizedExp()
    {
        int currentLevelExp = _pokymon.Base.GetLevelRequiredExp(_pokymon.Level);
        int nextLevelExp = _pokymon.Base.GetLevelRequiredExp(_pokymon.Level + 1);
        float normalizedExp = (_pokymon.Exp - currentLevelExp) / (float)(nextLevelExp - currentLevelExp);

        return Mathf.Clamp01(normalizedExp);
    }
}
