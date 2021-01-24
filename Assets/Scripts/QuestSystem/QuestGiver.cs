using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] Quest _quest = default;
    private bool _questGiven = false;
    [System.Serializable]private enum GiverType { None = 0, OnStartup = 1, OnTriggerEnter = 2 }
    [SerializeField] GiverType _giveCondition = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (_giveCondition == GiverType.OnStartup && !_questGiven)
        {
            GiveQuest();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(_giveCondition == GiverType.OnTriggerEnter && !_questGiven)
        {
            GiveQuest();
        }
    }

    public void GiveQuest()
    {
        PlayerQuestLog.instance.AddNewActiveQuest(_quest);
        _questGiven = true;
    }
}
