using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    [SerializeField] Image HealthBar = default;


    public override void UpdateUI()
    {
        base.UpdateUI();
        HealthBar.fillAmount = GetPercentHP();
    }
    
}
