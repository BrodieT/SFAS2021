using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyStats : CharacterStats
{
    [SerializeField] Image _healthBar = default;
    [SerializeField] TMP_Text _percentageTxt = default;

    public override void Die()
    {
        base.Die();
        Debug.Log("Enemy is dead");
        _healthBar.gameObject.SetActive(false);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        _healthBar.fillAmount = GetPercentHP();
        _percentageTxt.text = ((GetPercentHP() * 100)).ToString() + "%";
    }
}
