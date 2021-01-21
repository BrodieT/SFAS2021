using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Events;

//This script updates the players quest log data as well as the in-game UI
public class PlayerQuestLog : AutoCleanupSingleton<PlayerQuestLog>
{

    [SerializeField] public List<QuestLogEntry> _allQuests = new List<QuestLogEntry>(); //A list of all quests in the game, with the appropriate events assigned
    private int _currentQuestIndex = -1; //The ID of the currently selected quest
    [SerializeField] TMP_Text _currentQuestName = default; //The UI Text for the quest name
    [SerializeField] TMP_Text _currentQuestObjective = default; //The UI text for the quest objective
    public Quest currentQuest = default;

    //This function adds the provided quest to the active quests list
    public void AddNewActiveQuest(Quest quest)
    {
        int index = -1;
        index = _allQuests.FindIndex(x => x._quest._questID == quest._questID);

        if (index >= 0)
        {
            _allQuests[index]._isActive = true;
            _allQuests[index]._currentQuestStageID = 0;
            _allQuests[index].GetCurrentQuestStage()._isActive = true;
            SetQuestAsCurrent(_allQuests[index]._quest._questID);
        }
    }

    private void Start()
    {
        UpdateQuestmarker(-1);
    }

    //This function returns the currently active quest
    public QuestLogEntry GetCurrentQuest()
    {
        if (_currentQuestIndex < 0)
            return null;

        return _allQuests[_currentQuestIndex];
    }

    public List<QuestLogEntry> GetAllActiveQuests()
    {
        return _allQuests.Where(x => x._isActive == true).ToList();
    }

    //This function selects a new active quest and updates the UI and quest marker accordingly
    public void SetQuestAsCurrent(System.Guid ID)
    {
        int index = -1;
        index = _allQuests.FindIndex(x => x._quest._questID == ID);

        if (index < 0)
            return;

        if (_currentQuestIndex == index)
        {
            _currentQuestIndex = -1;
        }
        else
        {
            //Update the current ID
            _currentQuestIndex = index;
        }


        //Update the quest marker
        UpdateQuestmarker(_currentQuestIndex);
    }

   

    //This function updates the quest marker where appropriate
    private void UpdateQuestmarker(int id)
    {
        if (id >= 0 && id < _allQuests.Count)
        {
            Game_Manager.instance._UIManager.ShowQuestUI();

            //If a quest marker in the scene
            if (Game_Manager.instance._UIManager.GetQuestMarker(out QuestMarker marker))
            {
                //If the current objective has a linked quest marker
                if (_allQuests[id].GetQuestMarkerLocation(out Transform questMarker))
                {
                    Game_Manager.instance._UIManager.ShowQuestMarker();
                    //Update the marker position
                    marker.UpdateQuestTarget(questMarker);
                }
                else
                {
                    Game_Manager.instance._UIManager.HideQuestMarker();
                }
            }

            //Update the In-Game UI text
            _currentQuestName.text = _allQuests[id]._quest._questName;
            _currentQuestObjective.text = _allQuests[id].GetCurrentQuestStage()._stage._stageObjective;
        }
        else
        {
            Game_Manager.instance._UIManager.HideQuestUI();
            Game_Manager.instance._UIManager.HideQuestMarker();
        }
    }

    //This function progresses the provided quest using the provided quest stage's linked ID
    public void ProgressQuest(System.Guid questID, int questStageID)
    {
        //Find the index of the current quest being progressed
        int targetIndex = -1;
        targetIndex = _allQuests.FindIndex(x => x._quest._questID == questID);


        if (targetIndex < 0)
            return;

        if (_allQuests[targetIndex]._isCompleted)
            return;

        //If the quest stage being progressed is the current quest stage
        if (_allQuests[targetIndex].GetCurrentQuestStage()._stage._stageID == questStageID)
        {
            _allQuests[targetIndex].GetCurrentQuestStage().CompleteStage();

            //Set the new stage ID to be the linked ID of the current stage
            _allQuests[targetIndex]._currentQuestStageID = _allQuests[targetIndex].GetCurrentQuestStage()._stage._linkedStageID;

            //If the new stage ID is less than 0 then the quest has been completed
            if (_allQuests[targetIndex]._currentQuestStageID < 0)
            {
                _allQuests[targetIndex].CompleteQuest();


                if (_allQuests[targetIndex].GetNextQuest(out Quest next))
                {
                    AddNewActiveQuest(next);
                    return;
                }


            }
            else
            {
                _allQuests[targetIndex].GetCurrentQuestStage()._isActive = true;
            }
        }

        //Update the quest marker accordingly
        UpdateQuestmarker(targetIndex);
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
    public Quest _nextQuest = default;

    [HideInInspector] public int _currentQuestStageID = -1;

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
                
        foreach (QuestStage stage in q._questStages)
        {
            _questStages.Add(new QuestStageLog(stage));
        }
    }

    public QuestStageLog GetCurrentQuestStage()
    {
        return _questStages[_currentQuestStageID];
    }

    public bool GetQuestMarkerLocation(out Transform marker)
    {
        if (_currentQuestStageID < 0)
        {
            marker = null;
            return false;
        }

        if (_questStages[_currentQuestStageID]._questMarkerLocation != null)
        {
            marker = _questStages[_currentQuestStageID]._questMarkerLocation;
            return true;
        }
        else
        {
            marker = null;
            return false;
        }
    }
    
    

    public void CompleteQuest()
    {
        _currentQuestStageID = 0;
        _isCompleted = true;
        _isActive = false;
    }

    public bool GetNextQuest(out Quest next)
    {
        if(_nextQuest != null)
        {
            next = _nextQuest;
            return true;
        }
        else
        {
            next = null;
            return false;
        }
    }
}

[System.Serializable]
public class QuestStageLog
{
    [HideInInspector] public string _name = default; //Used only for clarity in the inspector
    [HideInInspector] public QuestStage _stage;
    [SerializeField] public bool _isActive = false;
    [SerializeField] public bool _isCompleted = false;
    [SerializeField] public UnityEvent _onCompleted;
    [SerializeField] public Transform _questMarkerLocation = default;

    //[SerializeField] public List<QuestSubStageLog> _subStages = new List<QuestSubStageLog>();

    public QuestStageLog(QuestStage stage)
    {
        _stage = stage;
        _name = stage._stageObjective;
    }

    public void CompleteStage()
    {
        _isActive = false;
        _isCompleted = true;
        _onCompleted?.Invoke();
    }
}
