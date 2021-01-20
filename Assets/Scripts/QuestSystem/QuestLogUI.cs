using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//This script is used to update the quest log UI in the pause menu
public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private GameObject _questStageEntryPrefab = default; //The UI prefab used to display quest stages
    [SerializeField] private GameObject _questSubStageEntryPrefab = default; //The UI prefab used to display quest stages
    [SerializeField] private GameObject _objectivesList = default; //The gameobject where the objective prefab will be instantiated under
    [SerializeField] private TMP_Text _questName = default;
    [SerializeField] private TMP_Text _questDescription = default;
    private PlayerQuestLog _questLog = default;
    [SerializeField] TMPro.FontStyles _completedFontStyle = default;
    // Start is called before the first frame update
    void Start()
    {
        _questLog = PlayerQuestLog.instance;
        StartCoroutine(TempUpdate());
    }

    IEnumerator TempUpdate()
    {
        while (true)
        {
            UpdateUI();

            yield return new WaitForSeconds(20.0f);
        }
    }


    public void UpdateUI()
    {
        if (_questLog.GetActiveQuest() == null)
            return;
        
        //Update the name and description of the quest
        _questName.text = _questLog.GetActiveQuest()._quest.name;
        _questDescription.text = _questLog.GetActiveQuest()._quest._questDescription;

        //Cleanup the objectives list since last update
        for (int i = 0; i < _objectivesList.transform.childCount; i++)
        {
            Destroy(_objectivesList.transform.GetChild(0).gameObject);
        }

        //Loop through the quest stages and instantiate an objective entry for each active one
        foreach (QuestStageLog stage in _questLog.GetActiveQuest()._questStages)
        {
            GameObject entry = Instantiate(_questStageEntryPrefab, _objectivesList.transform);
            entry.transform.GetComponentInChildren<TMP_Text>().text = "- " + stage._stage._stageObjective;

            //Strikethrough objective if complete
            if (stage._isCompleted)
            {
                entry.transform.GetComponentInChildren<TMP_Text>().fontStyle = _completedFontStyle;
            }

            //foreach (QuestSubStageLog substage in stage._subStages)
            //{
            //    GameObject sub_entry = Instantiate(_questSubStageEntryPrefab, entry.transform.GetChild(1));//.GetChild(0).GetChild(0));
            //    sub_entry.transform.GetComponentInChildren<TMP_Text>().text = "- " + substage._subStage._subStageObjective;

            //    //Strikethrough objective if complete
            //    if (substage._isCompleted)
            //    {
            //        sub_entry.transform.GetComponentInChildren<TMP_Text>().fontStyle = _completedFontStyle;
            //    }
            //}

        }
    }

}
