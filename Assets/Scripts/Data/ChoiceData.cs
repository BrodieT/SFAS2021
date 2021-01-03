using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ChoiceData
{
    [SerializeField] private string _text = default; //The text contents of this choice
    [SerializeField] private int _beatId = default; //The ID that this choice leads to
    [SerializeField] public bool AutoProgress = false; //Allows the story to automaticall move onto this choice without player input

    [SerializeField] public DialogueQuestEvent OnSelected = default;
    [SerializeField] public Quest _linkedQuest = default;
    [SerializeField] public int _stageID = -1;
    [SerializeField] public int _subStageID = -1;

    public bool IsAutoProgress()
    {
        return AutoProgress;
    }
    public string DisplayText { get { return _text; } } //return the choice text
    public int NextID { get { return _beatId; } } //return the linked ID
}

