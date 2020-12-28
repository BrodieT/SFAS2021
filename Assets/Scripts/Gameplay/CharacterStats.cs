using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class CharacterStats : MonoBehaviour
{
    [SerializeField] private int _maxHP = 100;
    [SerializeField] private int _regenRate = 1;
    private int _currentHP = 0;

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

    public float GetPercentHP()
    {
        return (float)(_currentHP) / (float)(_maxHP);
    }

    public void RestoreHP(int amount)
    {
        StartCoroutine(ChangeHP(_currentHP + amount));
    }

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
        Debug.Log("I am dead");
    }

    public virtual void UpdateUI()
    {

    }

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
