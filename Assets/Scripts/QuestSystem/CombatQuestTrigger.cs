using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatQuestTrigger : QuestTrigger
{
    [SerializeField] List<CharacterStats> _enemies = new List<CharacterStats>();
    private bool _hasTriggered = false;
    private void FixedUpdate()
    {
        if (!_hasTriggered)
        {
            //If unable to find an alive enemy in the list
            if (_enemies.Find(x => x._isDead == false) == null)
            {
                _hasTriggered = true;
                Trigger();
            }
        }
        
    }
}
