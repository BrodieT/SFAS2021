using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class handles the health of a character in the game
[DisallowMultipleComponent]
public class CharacterStats : MonoBehaviour
{
    [SerializeField] private int _maxHP = 100;
    [SerializeField] private int _regenRate = 1;
    private int _currentHP = 0;
    [HideInInspector] public bool _isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        _currentHP = _maxHP;
        UpdateUI();
    }

   public int GetHP()
    {
        return _currentHP;
    }

    //Uding the current and max health return a percentage value
    public float GetPercentHP()
    {
        return (float)(_currentHP) / (float)(_maxHP);
    }

    //begin the process of slowly regenerating health
    public void RestoreHP(int amount)
    {
        StartCoroutine(ChangeHP(_currentHP + amount));
    }

    //Reduce health and die if below 0
    public void TakeDamage(int amount)
    {
        if (_currentHP - amount <= 0)
        {
            _currentHP = 0;
            Die();
        }
        else
        {
            _currentHP -= amount;
        }
        UpdateUI();
    }

    public virtual void Die()
    {
        _isDead = true;
    }

    public virtual void UpdateUI()
    {

    }

    //This coroutine restores health by the regen rate per second
    IEnumerator ChangeHP(int target)
    {
        bool done = false;
        while(_currentHP != target && !done)
        {
            _currentHP += _regenRate;

            if(_currentHP >= _maxHP)
            {
                _currentHP = _maxHP;
                done = true;
            }

            if(_currentHP == target)
            {
                done = true;
            }

            UpdateUI();
            yield return new WaitForSeconds(1.0f);
        }

        UpdateUI();
    }
}
