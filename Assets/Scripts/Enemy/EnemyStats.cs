using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyStats : CharacterStats
{
    [SerializeField] Image _healthBar = default;
    [SerializeField] TMP_Text _percentageTxt = default;

    [SerializeField] CustomEvent _onDeath = default;
    
    public override void Die()
    {
        base.Die();

        _healthBar.gameObject.SetActive(false);

        if(_onDeath != null)
        {
            _onDeath?.Invoke();
        }

    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        _healthBar.fillAmount = GetPercentHP();
        _percentageTxt.text = ((GetPercentHP() * 100)).ToString() + "%";
    }
}
