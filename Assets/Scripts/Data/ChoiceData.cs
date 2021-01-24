using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ChoiceData
{
    [SerializeField] private string _text = default; //The text contents of this choice
    [SerializeField] private int _beatId = default; //The ID that this choice leads to
    [SerializeField] public bool AutoProgress = false; //Allows the story to automaticall move onto this choice without player input

    [SerializeField] public UnityEvent OnSelected = default; //Event called when this dialogue option is selected
    [SerializeField] public Quest _linkedQuest = default; //The linked quest that will be given upon selection of this quest
    [SerializeField] public int _stageID = -1; //The associated quest stage ID that will be progressed to 

    public bool IsAutoProgress(){ return AutoProgress; }    //This function returns if this choice should be automatically selected
    public string DisplayText { get { return _text; } } //return the choice text
    public int NextID { get { return _beatId; } } //return the linked ID
}

