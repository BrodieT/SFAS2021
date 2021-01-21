using System.Collections;
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
    private QuestLogEntry _currentQuestBreakdown = default;

    // Start is called before the first frame update
    void Start()
    {
        _questLog = PlayerQuestLog.instance;
    }

   
    public void QuestLogOpened()
    {
        ShowQuestBreakdown(_questLog.GetCurrentQuest());
        UpdateQuestList();
        UpdateQuestBreakdown();
    }

    private void ShowQuestBreakdown(QuestLogEntry newQuest)
    {
        _currentQuestBreakdown = newQuest;
        UpdateQuestBreakdown();
    }

    public void SetQuestAsCurrent(QuestLogEntry quest)
    {
        _questLog.SetQuestAsCurrent(quest._quest._questID);
    }

    private void UpdateQuestList()
    {
        GameUtility.DestroyAllChildren(_questList.transform);

        foreach (QuestLogEntry quest in _questLog.GetAllActiveQuests())
        {
            GameObject newQuest = Instantiate(_questListEntryPrefab, _questList.transform);
            newQuest.transform.GetComponentInChildren<TMP_Text>().text = quest._name;
            newQuest.GetComponentInChildren<Button>().onClick.AddListener(delegate { ShowQuestBreakdown(quest); SetQuestAsCurrent(quest); });
        }
    }

    private void UpdateQuestBreakdown()
    {
        //Update the name and description of the quest
        _questName.text = _currentQuestBreakdown._quest.name;
        _questDescription.text = _currentQuestBreakdown._quest._questDescription;

        
        GameUtility.DestroyAllChildren(_objectivesList.transform);

        //Loop through the quest stages and instantiate an objective entry for each active one
        foreach (QuestStageLog stage in _currentQuestBreakdown._questStages)
        {
            if (stage._isActive || stage._isCompleted)
            {
                GameObject entry = Instantiate(_questStageEntryPrefab, _objectivesList.transform);
                entry.transform.GetComponentInChildren<TMP_Text>().text = "- " + stage._stage._stageObjective;

                //Strikethrough objective if complete
                if (stage._isCompleted)
                {
                    entry.transform.GetComponentInChildren<TMP_Text>().fontStyle = _completedFontStyle;
                }
            }
        }
    }

}
