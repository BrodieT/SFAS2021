using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomEvent : UnityEvent
{

}

[System.Serializable]
public class DialogueQuestEvent : UnityEvent < Quest, int>
{

}
