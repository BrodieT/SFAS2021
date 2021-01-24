using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

//This class handles the enemy health 
public class EnemyStats : CharacterStats
{
    [SerializeField] Image _healthBar = default; //the health bar to be updated
    [SerializeField] TMP_Text _percentageTxt = default; //the text for display HP%
    [SerializeField] UnityEvent _onDeath = default; //Event that will be triggered on the death of this enemy
    
    public override void Die()
    {
        //Handle normal death functionality
        base.Die();

        //Remove the healthbar
        _healthBar.gameObject.SetActive(false);

        //If this is a charger enemy type update the progress tracker accordingly
        if(transform.TryGetComponent<ChargerController>(out ChargerController charger))
            ProgressionTracker.instance.IncrementChargerCounter();

        //If this is a turret enemy type update the progress tracker accordingly
        if (transform.TryGetComponent<TurretController>(out TurretController turret))
            ProgressionTracker.instance.IncrementTurretCounter();

        //Tell the enemy manager that this enemy is dead
        EnemyManager.instance.Killed(gameObject);

        //Invoke the on death event if appropriate
        if (_onDeath != null)
        {
            _onDeath?.Invoke();
        }

    }

    //Update the healthbar and percentage text
    public override void UpdateUI()
    {
        base.UpdateUI();
        _healthBar.fillAmount = GetPercentHP();
        _percentageTxt.text = ((GetPercentHP() * 100)).ToString() + "%";
    }
}
