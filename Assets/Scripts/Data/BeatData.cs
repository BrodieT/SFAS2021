using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BeatData
{
    [SerializeField] private List<ChoiceData> _choices = default; //A list of dialogue choices available in the current story beat
    [SerializeField] private string _text = default; //The text for this story beat that will be displayed
    [SerializeField] private int _id = default; //The ID of this story beat that will connect it to previous choices

    public List<ChoiceData> Decision { get { return _choices; } } //return the choices list
    public string DisplayText { get { return _text; } } //return the display text
    public int ID { get { return _id; } } //return the ID
}
