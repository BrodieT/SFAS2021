using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerQuestLog : MonoBehaviour
{
    [HideInInspector] public List<QuestLogEntry> _activeQuests = new List<QuestLogEntry>(); //A list of all active quests in the players quest log
    [HideInInspector] public List<Quest> _completedQuests = new List<Quest>(); //Quests that are completed are moved from active quests to this list
    
    
    public int _currentQuestID = 0; //The ID of the currently selected quest
    
    [SerializeField] TMP_Text _mainObjective = default;
    [SerializeField] TMP_Text _currentObjective = default;
    
    public void AddNewActiveQuest(Quest quest)
    {
        _activeQuests.Add(new QuestLogEntry(quest));
    }

    public void SetQuestAsCurrent(int ID)
    {
        _currentQuestID = ID;
        _mainObjective.text = _activeQuests[ID].GetCurrentQuestStage()._stageObjective;
        _currentObjective.text = _activeQuests[ID].GetCurrentSubStage()._subStageObjective;
    }

    public void ProgressCurrentQuestStage()
    {
        _activeQuests[_currentQuestID]._questStageID = _activeQuests[_currentQuestID].GetCurrentQuestStage()._linkedStageID;
        _activeQuests[_currentQuestID].GetCurrentQuestStage()._onQuestStageComplete?.Invoke();

        if (_activeQuests[_currentQuestID].GetCurrentQuestStage()._linkedStageID < 0)
        {
            _activeQuests[_currentQuestID]._isCompleted = true;
        }
    }

    public void ProgressCurrentSubStage()
    {
        _activeQuests[_currentQuestID]._subStageID = _activeQuests[_currentQuestID].GetCurrentSubStage()._linkedSubStageID;
        _activeQuests[_currentQuestID].GetCurrentSubStage()._onSubStageCompleted?.Invoke();

        if (_activeQuests[_currentQuestID].GetCurrentSubStage()._linkedSubStageID < 0)
        {
            ProgressCurrentQuestStage();
        }
    }

}
public class QuestLogEntry
{
    public Quest _quest;
    public bool _isCompleted = false;
    public int _questStageID = -1;
    public int _subStageID = -1;

    public QuestLogEntry(Quest q, bool complete = false, int stageID = 0, int substageID = 0)
    {
        _quest = q;
        _isCompleted = complete;
        _questStageID = stageID;
        _subStageID = substageID;
    }
    
    public QuestStage GetCurrentQuestStage()
    {
        return _quest._questStages[_questStageID];
    }

    public QuestSubStage GetCurrentSubStage()
    {
        return _quest._questStages[_questStageID]._subStages[_subStageID];
    }
}
