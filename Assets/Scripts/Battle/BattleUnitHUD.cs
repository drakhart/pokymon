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

        UpdateLevelText();
        UpdateExpBar();
        UpdateHPBar();
        UpdateHPText();
        UpdateStatusCondition();

        _pokymon.OnChangeStatusConditions += UpdateStatusCondition;
    }

    public void UpdateExpBar()
    {
        if (_expBar != null)
        {
            _expBar.SetScale(_pokymon.NormalizedExp);
        }
    }

    public YieldInstruction UpdateExpBarAnimated(bool startFromZero = false)
    {
        if (_expBar != null)
        {
            if (startFromZero)
            {
                _expBar.SetScale(0f);
            }

            return _expBar.SetScaleAnimated(_pokymon.NormalizedExp);
        }

        return new YieldInstruction();
    }

    public void UpdateHPBar()
    {
        _hpBar.SetScale(_pokymon.NormalizedHP);
    }

    public YieldInstruction UpdateHPBarAnimated()
    {
        return _hpBar.SetScaleAnimated(_pokymon.NormalizedHP);
    }

    public void UpdateHPText()
    {
        _hpText.text = $"{_pokymon.HP}/{_pokymon.MaxHP}";
        _prevHP = _pokymon.HP;
    }

    public YieldInstruction UpdateHPTextAnimated()
    {
        return DOTween.To(() => _prevHP, x => _prevHP = x, _pokymon.HP, 1f)
            .OnUpdate(() => {
                _hpText.text = $"{_prevHP}/{_pokymon.MaxHP}";
            })
            .WaitForCompletion();
    }

    public void UpdateLevelText()
    {
        _pokymonLevelText.text = $"Lvl {_pokymon.Level}";
    }

    public void UpdateStatusCondition()
    {
        var statusCondition = _pokymon.NonVolatileStatusCondition;

        if (statusCondition != null)
        {
            _statusConditionText.text = statusCondition.Tag;
            _statusConditionImage.color = statusCondition.Color;
            _statusConditionImage.gameObject.SetActive(true);
        }
        else
        {
            _statusConditionImage.gameObject.SetActive(false);
        }
    }
}
