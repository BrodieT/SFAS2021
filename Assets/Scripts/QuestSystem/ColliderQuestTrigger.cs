using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderQuestTrigger : QuestTrigger
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Trigger();
        }
    }
}
