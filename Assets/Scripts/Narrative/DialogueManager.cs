using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TMPro;

public class DialogueManager : BranchingNarrative
{
    GameObject _dialogueUI = default;

    public override void OutputSetup()
    {
        _dialogueUI = Game_Manager.instance._UIManager.GetDialogueWindow();
        _outputScreen = _dialogueUI.transform.GetComponentInChildren<TextDisplay>();
        _buttonList = _dialogueUI.transform.GetChild(1).gameObject;
        base.OutputSetup();
    }

    public override void StartDisplay()
    {
        _dialogueUI.SetActive(true);
        base.StartDisplay();
    }

    public override void FinishDisplay()
    {
        _dialogueUI.SetActive(false);
        base.FinishDisplay();
    }
}

