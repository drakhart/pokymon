using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnitHUD : MonoBehaviour
{
    public Text pokymonName;
    public Text pokymonLevel;
    public Text healthText;
    public Bar healthBar;

    private Pokymon _pokymon;

    public void SetPokymonData(Pokymon pokymon)
    {
        _pokymon = pokymon;

        pokymonName.text = pokymon.Base.Name;
        pokymonLevel.text = $"Lvl {pokymon.Level}";
        UpdatePokymonData();
    }

    public void UpdateHealthText(int damage)
    {
        StartCoroutine(AnimateHealthText(damage));
    }

    public void UpdateHealthBar()
    {
        healthBar.SetLength(_pokymon.HP / (float)_pokymon.MaxHP);
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

            healthText.text = $"{currentHealth}/{_pokymon.MaxHP}";

            yield return null;
        }

        healthText.text = $"{_pokymon.HP}/{_pokymon.MaxHP}";
    }
}
