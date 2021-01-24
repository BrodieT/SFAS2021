using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    [SerializeField] Image _healthBar = default;


    public override void Die()
    {
        base.Die();

        if (!_isDead)
        {
            _isDead = true;
            GameUtility._isPaused = true;
            Game_Manager.instance._UIManager._discoveryUI.Discover(new DiscoveryUI.Discovery("Game Over", "You Are Dead"));

            Invoke("ReturnToMenu", 2.0f);
        }
    }

    private void ReturnTomenu()
    {
        SceneLoader.instance.ReturnToMenu();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        _healthBar.fillAmount = GetPercentHP();
    }
    
}
