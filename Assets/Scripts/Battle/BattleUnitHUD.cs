using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnitHUD : MonoBehaviour
{
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
        UpdatePokymonData();
    }

    public void UpdateHealthText(int damage)
    {
        StartCoroutine(AnimateHealthText(damage));
    }

    public void UpdateHealthBar()
    {
        _hpBar.SetLength(_pokymon.HP / (float)_pokymon.MaxHP);
    }

    public void UpdatePokymonData(int damage = 0)
    {
        UpdateHealthText(damage);
        UpdateHealthBar();
    }

    private IEnumerator AnimateHealthText(int damage)
    {
        float currentHealthNormalized = (_pokymon.HP + damage) / (float)_pokymon.MaxHP;
        float targetHealthNormalized = _pokymon.HP / (float)_pokymon.MaxHP;
        float updateQuantity = damage / (float)_pokymon.MaxHP;

        while (currentHealthNormalized - targetHealthNormalized > Mathf.Epsilon)
        {
            currentHealthNormalized -= updateQuantity * Time.deltaTime;
            int currentHealth = Mathf.Max((int)(currentHealthNormalized * _pokymon.MaxHP), _pokymon.HP);

            _hpText.text = $"{currentHealth}/{_pokymon.MaxHP}";

            yield return null;
        }

        _hpText.text = $"{_pokymon.HP}/{_pokymon.MaxHP}";
    }
}
