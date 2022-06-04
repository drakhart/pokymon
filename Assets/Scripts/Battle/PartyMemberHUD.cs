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
    [SerializeField] private Text _pokymonPrimaryTypeText;
    [SerializeField] private Text _pokymonSecondaryTypeText;
    [SerializeField] private Image _pokymonImage;

    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _selectedSprite;

    private Pokymon _pokymon;

    public void SetPokymonData(Pokymon pokymon)
    {
        _pokymon = pokymon;

        _pokymonNameText.text = _pokymon.Name;
        _pokymonLevelText.text = $"Lvl {_pokymon.Level}";
        _pokymonPrimaryTypeText.text = _pokymon.Base.PrimaryType.ToString();
        _pokymonPrimaryTypeText.color = ColorManager.SharedInstance.PokymonType(_pokymon.Base.PrimaryType);
        _expBar.SetScale(_pokymon.NormalizedExp);
        _hpBar.SetScale(_pokymon.NormalizedHP);
        _hpText.text = $"{_pokymon.HP}/{_pokymon.MaxHP}";
        _pokymonImage.sprite = _pokymon.Base.FrontSprite;

        SetSecondaryType();
    }

    public void SetSelectedPokymon(bool selected)
    {
        gameObject.GetComponent<Image>().sprite = selected ? _selectedSprite : _defaultSprite;
        _pokymonNameText.color = selected
            ? ColorManager.SharedInstance.SelectedText
            : ColorManager.SharedInstance.DefaultText;
    }

    public void SetSecondaryType()
    {
        var active = _pokymon.Base.SecondaryType != PokymonType.None;
        _pokymonSecondaryTypeText.gameObject.SetActive(active);
        _pokymonSecondaryTypeText.text = _pokymon.Base.SecondaryType.ToString();
        _pokymonSecondaryTypeText.color = ColorManager.SharedInstance.PokymonType(_pokymon.Base.SecondaryType);
    }
}
