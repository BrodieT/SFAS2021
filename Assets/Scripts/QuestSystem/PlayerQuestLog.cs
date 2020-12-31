using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class PlayerQuestLog : AutoCleanupSingleton<PlayerQuestLog>
{

    [SerializeField] public List<QuestLogEntry> _allQuests = new List<QuestLogEntry>(); //A list of all quests in the game, with the appropriate events assigned

    [HideInInspector] public List<QuestLogEntry> _activeQuests = new List<QuestLogEntry>(); //A list of all active quests in the players quest log
   // [HideInInspector] public List<Quest> _completedQuests = new List<Quest>(); //Quests that are completed are moved from active quests to this list

    public int _currentQuestID = 0; //The ID of the currently selected quest
    
    [SerializeField] TMP_Text _mainObjective = default;
    [SerializeField] TMP_Text _currentObjective = default;
    
    public void AddNewActiveQuest(Quest quest)
    {
        Debug.Log(quest._questName + " added to active quests");
        _activeQuests.Add(new QuestLogEntry(quest));
    }

    public void SetQuestAsCurrent(int ID)
    {
        _currentQuestID = ID;
    //    _mainObjective.text = _activeQuests[ID].GetCurrentQuestStage()._stageObjective;
    //    _currentObjective.text = _activeQuests[ID].GetCurrentSubStage()._subStageObjective;
    }

    private void Update()
    {
        if (_currentQuestID < _activeQuests.Count)
        {
            Debug.Log(_activeQuests[_currentQuestID].GetCurrentQuestStage()._stageObjective);
            Debug.Log(_activeQuests[_currentQuestID].GetCurrentSubStage()._subStageObjective);
        }
    }

    public void ProgressQuest(int questID, int questStageID, int questSubStageID)
    {
        int targetIndex = _activeQuests.FindIndex(x => x._quest._questID == questID);

       if( _activeQuests[targetIndex].GetCurrentSubStage()._subStageID == questSubStageID)
        {
            _activeQuests[targetIndex]._currentSubStageID = _activeQuests[targetIndex].GetCurrentSubStage()._linkedSubStageID;
            //call event for completion of this current sub stage here

            if (_activeQuests[targetIndex]._currentSubStageID < 0)
            {
                _activeQuests[targetIndex]._currentSubStageID = 0;
                _activeQuests[targetIndex]._currentQuestStageID = _activeQuests[targetIndex].GetCurrentQuestStage()._linkedStageID;
                //Call event for completion of this quest stage here

                if (_activeQuests[targetIndex]._currentQuestStageID < 0)
                {
                    _activeQuests[targetIndex]._currentQuestStageID = -1;
                    _activeQuests[targetIndex]._isCompleted = true;
                    _activeQuests[targetIndex]._isActive = false;
                    _activeQuests.RemoveAt(targetIndex);
                }
            }
        }



    }

    private void OnValidate()
    {
        foreach (QuestLogEntry entry in _allQuests)
        {
            if(entry._quest != entry._prevQuest)
                entry.UpdateValues(entry._quest);
        }
    }
}


[System.Serializable]
public class QuestLogEntry
{
    [HideInInspector] public string _name = default; //Simply used to give some extra clarity to the inspector
    [HideInInspector] public Quest _prevQuest; //Used to check when the quest for this entry has changed 
    public Quest _quest;
    public bool _isActive = false;
    public bool _isCompleted = false;
    public List<QuestStageLog> _questStages = new List<QuestStageLog>();


    [HideInInspector] public int _currentQuestStageID = -1;
    [HideInInspector] public int _currentSubStageID = -1;

    public QuestLogEntry(Quest q)
    {
        UpdateValues(q);
    }
    
    public void UpdateValues(Quest q)
    {
        _questStages.Clear();

        if (q == null)
            return;

        
        _prevQuest = _quest;
        _quest = q;
        _name = q._questName;

        if (q._questStages.Count > 0)
            _currentQuestStageID = 0;

        if(q._questStages[0]._subStages.Count > 0)
            _currentSubStageID = 0;
        
        foreach (QuestStage stage in q._questStages)
        {
            _questStages.Add(new QuestStageLog(stage));
        }
    }

    public QuestStage GetCurrentQuestStage()
    {
        return _quest._questStages[_currentQuestStageID];
    }

    public QuestSubStage GetCurrentSubStage()
    {
        return _quest._questStages[_currentQuestStageID]._subStages[_currentSubStageID];
    }
}

[System.Serializable]
public class QuestStageLog
{
    [HideInInspector] public string _name = default; //Used only for clarity in the inspector
    [HideInInspector] public QuestStage _stage;
    [SerializeField] public bool _isCompleted;
    [SerializeField] public bool _isActive;
    [SerializeField] public CustomEvent _onCompleted;
    [SerializeField] public List<QuestSubStageLog> _subStages = new List<QuestSubStageLog>();

    public QuestStageLog(QuestStage stage)
    {
        _stage = stage;
        _name = stage._stageObjective;

        foreach (QuestSubStage subStage in stage._subStages)
        {
            _subStages.Add(new QuestSubStageLog(subStage));
        }
    }
}

[System.Serializable]
public class QuestSubStageLog
{
    [HideInInspector] public string _name = default; //Used only for clarity in inspector
    [HideInInspector] public QuestSubStage _subStage;
    [SerializeField] public bool _isCompleted;
    [SerializeField] public bool _isActive;
    [SerializeField] public CustomEvent _onCompleted;

    public QuestSubStageLog(QuestSubStage subStage)
    {
        _subStage = subStage;
        _name = subStage._subStageObjective;
    }
}