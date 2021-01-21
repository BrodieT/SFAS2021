using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This interactable type triggers a quest progression when interacting with it
public class QuestTriggerInteract : Interactable
{
    private QuestTrigger _questTrigger = default;
    
    private void OnValidate()
    {
        if(!transform.TryGetComponent<QuestTrigger>(out _questTrigger))
        {
            _questTrigger = gameObject.AddComponent(typeof(QuestTrigger)) as QuestTrigger;
        }
    }

    public override void DoInteract()
    {
        base.DoInteract();

        if (_questTrigger != null)
        {
            _questTrigger.Trigger();
        }
        else
        {
            Debug.Log("No Quest Trigger Found on " + name);
        }
    }
}
