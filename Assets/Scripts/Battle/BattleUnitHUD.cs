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

    private Pokymon _pokymon;

    public void SetPokymonData(Pokymon pokymon)
    {
        _pokymon = pokymon;

        _pokymonNameText.text = _pokymon.Name;
        UpdateLevelText();
        UpdateExpBar();
        UpdateHPBar();
        UpdateHPText();
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
    }

    public YieldInstruction UpdateHPTextAnimated(int prevHP)
    {
        return DOTween.To(() => prevHP, x => prevHP = x, _pokymon.HP, 1f)
            .OnUpdate(() => {
                _hpText.text = $"{prevHP}/{_pokymon.MaxHP}";
            })
            .WaitForCompletion();
    }

    public void UpdateLevelText()
    {
        _pokymonLevelText.text = $"Lvl {_pokymon.Level}";
    }
}
