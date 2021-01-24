using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//This script is used to update the quest log UI in the pause menu
public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private GameObject _questStageEntryPrefab = default; //The UI prefab used to display quest stages
    [SerializeField] private GameObject _objectivesList = default; //The gameobject where the objective prefab will be instantiated under

    [SerializeField] private GameObject _questListEntryPrefab = default; //The UI prefab used to display the quests in the quest list
    [SerializeField] private GameObject _questList = default; //The gameobject the quests will be listed under


    [SerializeField] private TMP_Text _questName = default; //The name of the current quest in the breakdown
    [SerializeField] private TMP_Text _questDescription = default; //The description of the quest in the breakdown

    private PlayerQuestLog _questLog = default; //Local store of the player's quest log
    [SerializeField] TMPro.FontStyles _completedFontStyle = default; //Strikethrough font style to show completed objectives
    private ProgressionTracker.QuestInfo _currentQuestBreakdown = default;

    // Start is called before the first frame update
    void Start()
    {
        _questLog = PlayerQuestLog.instance;
    }

   
    public void QuestLogOpened()
    {
        ShowQuestBreakdown(ProgressionTracker.instance.GetCurrentQuest());
        UpdateQuestList();
        UpdateQuestBreakdown();
    }

    private void ShowQuestBreakdown(ProgressionTracker.QuestInfo newQuest)
    {
        _currentQuestBreakdown = newQuest;
        UpdateQuestBreakdown();
    }

    public void SetQuestAsCurrent(ProgressionTracker.QuestInfo quest)
    {
        Debug.Log("Setting " + quest._quest._questName.ToString() + " as current");
        ProgressionTracker.instance.SetCurrentQuest(quest._quest);
        PlayerQuestLog.instance.UpdateQuestmarker();
    }

    private void UpdateQuestList()
    {
        GameUtility.DestroyAllChildren(_questList.transform);

        foreach (ProgressionTracker.QuestInfo quest in ProgressionTracker.instance.GetAllActiveQuests())
        {
            GameObject newQuest = Instantiate(_questListEntryPrefab, _questList.transform);
            newQuest.transform.GetComponentInChildren<TMP_Text>().text = quest._quest._questName;
            newQuest.GetComponentInChildren<Button>().onClick.AddListener(delegate { ShowQuestBreakdown(quest); SetQuestAsCurrent(quest); });
        }
    }

    private void UpdateQuestBreakdown()
    {
        if(_currentQuestBreakdown._quest == null)
        {
            _questName.text = "No Quest To Show";
            _questDescription.text = "";
            GameUtility.DestroyAllChildren(_objectivesList.transform);
            return;
        }

        //Update the name and description of the quest
        _questName.text = _currentQuestBreakdown._quest.name;
        _questDescription.text = _currentQuestBreakdown._quest._questDescription;

        
        GameUtility.DestroyAllChildren(_objectivesList.transform);

        //Loop through the quest stages and instantiate an objective entry for each active one
        foreach (QuestStage stage in _currentQuestBreakdown._quest._questStages)
        {

            //Previously completed stages
            if(_currentQuestBreakdown._currentStage >= stage._stageID)
            {
                GameObject entry = Instantiate(_questStageEntryPrefab, _objectivesList.transform);
                entry.transform.GetComponentInChildren<TMP_Text>().text = "- " + stage._stageObjective;

                if (_currentQuestBreakdown._currentStage != stage._stageID)
                    entry.transform.GetComponentInChildren<TMP_Text>().fontStyle = _completedFontStyle;
            }

        
        }
    }

}
