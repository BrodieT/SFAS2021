using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
[CreateAssetMenu(menuName = "My Assets/Quest")]
public class Quest : ScriptableObject
{
    [SerializeField] public int _questID = -1; //Unique identifier for this specific quest. Useful for searching lists in quest log.
    [SerializeField] public string _questName = default; //The name of this quest as displayed in the quest log UI.
    [SerializeField] public string _questDescription = default; //The description of this quest as shown in the quest log UI. Provides context.
    [SerializeField] public List<QuestStage> _questStages = new List<QuestStage>(); //The list of stages in this quest.
}

[Serializable]
public class QuestStage
{
    [SerializeField] public int _stageID = -1; //The unique identifier of this quest stage within the context of the quest it is a part of.
    [SerializeField] public string _stageObjective = default; //The goal of this quest stage as displayed in the quest log UI & main UI when active.
    [SerializeField] public CustomEvent _onQuestStageComplete = default; //The event that is called upon completion of this stage.
    [SerializeField] public int _linkedStageID = -1; //The quest stage that will follow upon completion of this stage.
    [SerializeField] public List<QuestSubStage> _subStages = new List<QuestSubStage>(); //The list of objectives that make up this quest stage.
}

[Serializable]
public class QuestSubStage
{
    [SerializeField] public int _subStageID = -1; //The unique identifier of this sub-stage within the context of the quest stage it is a part of.
    [SerializeField] public string _subStageObjective = default; //The goal of this sub-stage as displayed in the quest log UI & the main UI when active.
    [SerializeField] public CustomEvent _onSubStageCompleted = default; //The event that is called upon completion of this sub-stage.
    [SerializeField] public int _linkedSubStageID = -1; //The ID of the sub-stage that will follow this one
}
