using System;
using UnityEngine;

[Serializable]
public class ChoiceData
{
    [SerializeField] private string _text = default; //The text contents of this choice
    [SerializeField] private int _beatId = default; //The ID that this choice leads to

    public string DisplayText { get { return _text; } } //return the choice text
    public int NextID { get { return _beatId; } } //return the linked ID
}
