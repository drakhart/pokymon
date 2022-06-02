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

        _pokymonNameText.text = pokymon.Name;
        _pokymonLevelText.text = $"Lvl {pokymon.Level}";
        UpdateExpBar();
        UpdateHPBar();
        UpdateHPText();
    }

    public void UpdateExpBar()
    {
        if (_expBar != null)
        {
            _expBar.SetScale(GetNormalizedExp());
        }
    }

    public YieldInstruction UpdateExpBarAnimated()
    {
        if (_expBar != null)
        {
            return _expBar.SetScaleAnimated(GetNormalizedExp());
        }

        return new YieldInstruction();
    }

    public void UpdateHPBar()
    {
        _hpBar.SetScale(_pokymon.HP / (float)_pokymon.MaxHP);
    }

    public YieldInstruction UpdateHPBarAnimated()
    {
        return _hpBar.SetScaleAnimated(_pokymon.HP / (float)_pokymon.MaxHP);
    }

    public void UpdateHPText()
    {
        _hpText.text = $"{_pokymon.HP}/{_pokymon.MaxHP}";
    }

    public YieldInstruction UpdateHPTextAnimated(int damage)
    {
        int currentHP = _pokymon.HP + damage;

        return DOTween.To(() => currentHP, x => currentHP = x, _pokymon.HP, 1f).OnUpdate(() => {
            _hpText.text = $"{currentHP}/{_pokymon.MaxHP}";
        }).WaitForCompletion();
    }

    private float GetNormalizedExp()
    {
        int currentLevelExp = _pokymon.Base.GetLevelRequiredExp(_pokymon.Level);
        int nextLevelExp = _pokymon.Base.GetLevelRequiredExp(_pokymon.Level + 1);
        float normalizedExp = (_pokymon.Exp - currentLevelExp) / (float)(nextLevelExp - currentLevelExp);

        return Mathf.Clamp01(normalizedExp);
    }
}
