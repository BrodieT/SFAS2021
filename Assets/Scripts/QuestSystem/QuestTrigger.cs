using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [SerializeField] public Quest _linkedQuest;
    [SerializeField] public int _questStageID = -1;
    [SerializeField] public int _questSubStageID = -1;

    public void Trigger()
    {
        PlayerQuestLog.instance.ProgressQuest(_linkedQuest._questID, _questStageID, _questSubStageID);
    }
}
