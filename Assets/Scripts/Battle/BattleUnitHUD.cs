using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnitHUD : MonoBehaviour
{
    [SerializeField] private Bar _expBar;
    [SerializeField] private Bar _hpBar;
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _pokymonLevelText;
    [SerializeField] private Text _pokymonNameText;
    [SerializeField] private Image _statusConditionImage;
    [SerializeField] private Text _statusConditionText;

    private Pokymon _pokymon;

    private int _prevHP;

    public void SetPokymonData(Pokymon pokymon)
    {
        _pokymon = pokymon;
        _prevHP = pokymon.HP;

        _pokymonNameText.text = _pokymon.Name;

        UpdateExpBar();
        UpdateHPBar();
        UpdateHPText();
        UpdateLevelText();
        UpdateStatusCondition();

        _pokymon.OnChangeExp += UpdateExpBarAnimated;
        _pokymon.OnChangeHP += UpdateHPBarAnimated;
        _pokymon.OnChangeHP += UpdateHPTextAnimated;
        _pokymon.OnChangeLevel += UpdateLevelText;
        _pokymon.OnChangeStatusConditionList += UpdateStatusCondition;
    }

    public void UpdateExpBar()
    {
        _expBar?.SetScale(_pokymon.NormalizedExp);
    }

    public void UpdateExpBarAnimated(bool startFromZero = false)
    {
        if (startFromZero)
        {
            _expBar?.SetScale(0f);
        }

        _expBar?.SetScaleAnimated(_pokymon.NormalizedExp);
    }

    public void UpdateHPBar()
    {
        _hpBar.SetScale(_pokymon.NormalizedHP);
    }

    public void UpdateHPBarAnimated()
    {
        _hpBar.SetScaleAnimated(_pokymon.NormalizedHP);
    }

    public void UpdateHPText()
    {
        _hpText.text = $"{_pokymon.HP}/{_pokymon.MaxHP}";
        _prevHP = _pokymon.HP;
    }

    public void UpdateHPTextAnimated()
    {
        DOTween.To(() => _prevHP, x => _prevHP = x, _pokymon.HP, 1f)
            .OnUpdate(() =>
            {
                _hpText.text = $"{_prevHP}/{_pokymon.MaxHP}";
            });
    }

    public void UpdateLevelText()
    {
        _pokymonLevelText.text = $"Lvl {_pokymon.Level}";
    }

    public void UpdateStatusCondition()
    {
        var statusCondition = _pokymon.NonVolatileStatusCondition;

        if (statusCondition?.Tag != null)
        {
            _statusConditionText.text = statusCondition.Tag;
            _statusConditionImage.color = ColorManager.SharedInstance.ByStatusCondition(statusCondition.ID);
            _statusConditionImage.gameObject.SetActive(true);
        }
        else
        {
            _statusConditionImage.gameObject.SetActive(false);
        }
    }
}
