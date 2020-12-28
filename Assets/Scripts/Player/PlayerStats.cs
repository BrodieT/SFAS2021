using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    [SerializeField] Image _healthBar = default;


    public override void UpdateUI()
    {
        base.UpdateUI();
        _healthBar.fillAmount = GetPercentHP();
    }
    
}
